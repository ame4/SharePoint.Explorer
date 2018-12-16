using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePoint.Explorer.Modules.Common;
using SharePoint.Explorer.Modules.Lists;
using James.SharePoint;
using sp = James.SharePoint;
using JScriptSuite.JScriptLib.DataBinding;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.UI.Controls.Menus;
using JScriptSuite.JScriptLib.UI.Controls.Trees;
using JScriptSuite.JScriptLib.DataBinding.Providers.DependencyObjects;

namespace SharePoint.Explorer.Modules.Nodes
{
    class ListNode : sp.List, IExplorerNode
    {
        public ListNode()
        {
            RootFolder.Advise(Notify);
        }


        readonly static DependencyProperty<ContentType> selectedContentType = DependencyProperty<ContentType>.Register(typeof(ListNode));
        internal ContentType SelectedContentType
        {
            get
            {
                return GetValue(selectedContentType);
            }

            set
            {
                SetValue(selectedContentType, value);
            }
        }

        public IObservableList Children
        {
            get {
                return ((FolderNode)RootFolder).Children; 
            }
        }

        ContextMenu IExplorerNode.ContextMenu(IHierarchyLevel level, long index)
        {
            return NodeUtil.FolderContextMenu((FolderNode)RootFolder);
        }

        internal void Refresh()
        {
            ((FolderNode)RootFolder).Refresh();
            // Folders.Clear();
        }

        string ITreeNode.Title
        {
            get { return string.Format("{0} ({1})", Title, ItemCount); }
        }

        NavigateDisposition IExplorerNode.Navigate(DeepLink deepLink)
        {
            if(deepLink.ListId == ID)
            {
                if(string.IsNullOrEmpty(deepLink.FolderUrl))
                {
                    ((FolderNode)RootFolder).FolderItems.Navigate(deepLink);
                    return NavigateDisposition.Complete;
                }

                return NavigateDisposition.Expand;
            }

            return NavigateDisposition.Next;
        }

        public DeepLink DeepLink
        {
            get
            {
                return ((FolderNode)RootFolder).DeepLink;
            }
        }

        protected override Folder CreateFolder()
        {
            return new FolderNode();
        }

    }
}
