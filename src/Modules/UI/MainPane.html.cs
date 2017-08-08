using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePoint.Explorer.Modules.Common;
using SharePoint.Explorer.Modules.Informations;
using SharePoint.Explorer.Modules.Nodes;
using JScriptSuite.DataBinding;
using JScriptSuite.Html;
using JScriptSuite.InteropServices;
using JScriptSuite.JScriptLib.Common;
using JScriptSuite.JScriptLib.DataBinding;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.UI.Controls;
using JScriptSuite.JScriptLib.UI.Controls.Menus;
using JScriptSuite.JScriptLib.UI.Controls.Trees;
using SharePoint.Explorer.Modules.Lists;
using James.SharePoint;
using SharePoint.Explorer.Modules.Configurations;
using SharePoint.Explorer.Modules.Webs;
using JScriptSuite.JScriptLib.UI.Controls.Windows;
using JScriptSuite.JScriptLib.UI.Renderers;
using JScriptSuite.JScriptLib.UI.Controls.DataBinding;
using SharePoint.Explorer.Modules.Lists.ViewModels;
using SharePoint.Explorer.ViewModels;
using JScriptSuite.JScriptLib.UI.Controls.Lazy;
using JScriptSuite.JScriptLib.UI.Controls.Tabs;
using JScriptSuite.JScriptLib.Html;
using SharePoint.Explorer.Modules.Lists.UI;
using JScriptSuite.Html5;

namespace SharePoint.Explorer.Modules.UI
{
    class MainPane : UserControl<App>
    {
        internal readonly TreeView<IExplorerNode> TreeView = new TreeView<IExplorerNode>();
        internal readonly TabStrip InfoTabStrip = new TabStrip();
        internal readonly ListNodePane ListNodePane = new ListNodePane();
        internal readonly FolderNodePane FolderNodePane = new FolderNodePane();

        internal MainPane(App app)
        {
            Data = app;
            TreeView.AdviseContextMenu(TreeView_ContextMenu);
            TreeView.AdviseDrag(delegate(HtmlMouseEvent e)
            {
                TreeViewHit hit = TreeView.Hit(e);
                if(hit != null)
                {
                    object node = hit.Nodes.List[hit.Index];
                    FolderNode folder = node as FolderNode;
                    if(folder != null)
                    {
                        return new ListItemsDragSource(new ListItem[] { folder.Item });
                    }

                    WebNode web = node as WebNode;
                    if(web != null)
                    {
                       return new WebNodeDragSource(web);
                    }
                }

                return null;
            });

            TreeView.AdviseDrop(delegate(object dataObject)
            {
                IList<ListItem> listItems = dataObject as IList<ListItem>;
                if (listItems != null)
                {
                    return new ListItemsNodeDropTarget(TreeView, listItems);
                }

                WebNode web = dataObject as WebNode;
                return web != null ? new WebNodeDragTarget(TreeView, web) : null;
            });

            if (SupportedHtml5Features.File)
            {
                PostAdvise(
                    delegate ()
                    {
                        return Element.AdviseDrop(null, null);
                    });

                TreeViewFileDropTarget fileDropTarget = new TreeViewFileDropTarget(TreeView);
            }


            TreeView.AdviseKeyDown(
                delegate(HtmlKeyboardEvent e)
                {
                    if(e.KeyCode == KeyCode.Delete)
                    {
                        FolderNode folder = TreeView.Selection.Data as FolderNode;
                        if (folder != null)
                        {
                            NodeUtil.DeleteFolder(folder);
                        }
                    }
                });
        }

        void DragOver(HtmlMouseEvent e)
        {

        }
        ContextMenu TreeView_ContextMenu(IHierarchyLevel level, long index)
        {
            if (level == null)
            {
                return NodeUtil.RootsContextMenu(this.Data.AddRoot,
                    Data.LoadConfiguration);
            }

            IExplorerNode node = level.List[index] as IExplorerNode;
            return node != null ? node.ContextMenu(level, index) : null;
        }

        internal void Navigate(DeepLink deepLink)
        {
            TreeView.Selection.Navigate(
                delegate(IExplorerNode node)
                {
                    return node.Navigate(deepLink);
                },
                delegate(JArray<long> path, bool complete)
                {
                    if(complete || TreeView.Selection.Data == null)
                    {
                        TreeView.Selection.Select(path);
                        if(complete)
                        {
                            IExplorerNode node = TreeView.Selection.Data;
                            node.DeepLink.ContentTypeId = deepLink.ContentTypeId;
                            node.DeepLink.TabItemIndex = deepLink.TabItemIndex;
                            switch(deepLink.NodeType)
                            {
                                case NodeType.Info:
                                case NodeType.Lists:
                                case NodeType.Webs:
                                    InfoTabStrip.Selection.Data = InfoTabStrip.TabItems[Math.Max(deepLink.TabItemIndex, 0)];
                                    break;

                                case NodeType.List:
                                    ListNodePane.Data = (ListNode)node;
                                    // ListNodePane.Navigate(deepLink);
                                    break;

                                case NodeType.Folder:
                                    FolderNodePane.Data = (FolderNode)node;
                                    // FolderNodePane.Navigate(deepLink);
                                    break;
                            }
                        }
                    }
                });
        }

        [Binder]
        static void Bind(IBinder<MainPane> binder)
        {
            binder.AddBinding<int>(mainPane => mainPane.InfoTabStrip.TabItems.IndexOf(mainPane.InfoTabStrip.Selection.Data),
                mainPane => mainPane.TreeView.Selection.Data.DeepLink.TabItemIndex,
                mainPane => 
                    Operations.Or(
                        Operations.CanCast<InformationNode>(mainPane.TreeView.Selection.Data),
                        Operations.CanCast<WebsNode>(mainPane.TreeView.Selection.Data),
                        Operations.CanCast<ListsNode>(mainPane.TreeView.Selection.Data)
                    ),
                false
            );
        }
    }
}
