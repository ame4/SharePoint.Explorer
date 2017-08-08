using JScriptSuite.Html;
using JScriptSuite.JScriptLib.UI.Controls.Windows;
using JScriptSuite.JScriptLib.UI.DragDrops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.UI.Controls.Trees;
using James.SharePoint;
using SharePoint.Explorer.Modules.Lists;
using SharePoint.Explorer.Modules.Common;
using JScriptSuite.JScriptLib.UI.Controls.Lazy;
using JScriptSuite.JScriptLib.UI.Controls.Trees.DragDrops;

namespace SharePoint.Explorer.Modules.Nodes
{
    class ListItemsNodeDropTarget : TreeViewDropTarget, IDropTarget
    {
        readonly IList<ListItem> listItems;
        object node;
        DropEffects dropEffects;

        internal ListItemsNodeDropTarget(DataTreeView treeView, IList<ListItem> listItems) : base(treeView)
        {
            this.listItems = listItems;
        }

        DropEffects IDropTarget.DragOver(HtmlMouseEvent e)
        {
            base.DragOver(e);
            if(Feedback == FeedbackMode.None) return DropEffects.None;
            object node = HitNode;
            if (!Observable.Equals(this.node, node))
            {
                this.node = node;
                dropEffects = DropEffects.None;
                if (listItems != null)
                {
                    ListNode list = node as ListNode;
                    if (list != null)
                    {
                        dropEffects = ListItemsDropUtility.DropEffects(listItems, list.RootFolder);
                    }
                    else
                    {
                        FolderNode folder = node as FolderNode;
                        if (folder != null)
                        {

                            dropEffects = ListItemsDropUtility.DropEffects(listItems, folder);
                        }
                    }
                }
            }

            return dropEffects & DropEffects.Move;
        }

        void IDropTarget.Drop(HtmlMouseEvent e, DropEffects dropEffect)
        {
            TreeViewHit hit = base.Drop(e);
            if (Feedback != FeedbackMode.None)
            {
                try
                {
                    object node = hit.Nodes.List[hit.Index];
                    ListNode list = node as ListNode;
                    ListItemsDropUtility.Drop(listItems, list != null ? list.RootFolder : node as Folder);
                }
                catch (Exception ex)
                {
                    LazyWindow.ShowError(ex);
                }
            }
        }

        public override void ShowFeedback()
        {
            ShowFeedback(FeedbackMode.Over);
        }
    }
}
