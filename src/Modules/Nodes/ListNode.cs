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

namespace SharePoint.Explorer.Modules.Nodes
{
    class ListNode : sp.List, IExplorerNode
    {
        readonly ObservableProperty<ContentType> selectedContentType;

        public ListNode()
        {
            selectedContentType = new ObservableProperty<ContentType>(this);
            RootFolder.Advise(Notify);
        }


        internal ContentType SelectedContentType
        {
            get
            {
                return selectedContentType.Value;
            }

            set
            {
                selectedContentType.Value = value;
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
