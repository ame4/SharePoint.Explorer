using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePoint.Explorer.Modules.Common;
using SharePoint.Explorer.Modules.Lists;
using James.SharePoint;
using JScriptSuite.JScriptLib.DataBinding;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.UI.Controls.Menus;
using JScriptSuite.InteropServices;
using JScriptSuite.JScriptLib.DataBinding.Providers.Lazy;
using SharePoint.Explorer.Modules.Lists.ViewModels;
using JScriptSuite.JScriptLib.Html;
using JScriptSuite.JScriptLib.UI.Controls.Trees;
using JScriptSuite.Html;
using JScriptSuite.JScriptLib.DataBinding.Providers.DependencyObjects;

namespace SharePoint.Explorer.Modules.Nodes
{
    class FolderNode : Folder, IExplorerNode
    {
        readonly ListView<Folder, int, string> sorter;
        DeepLink deepLink;

        FolderItems folderItems;
        bool hasChildren;
        IDisposable disposable;

        LazyLoadingList lazyChildren;

        public FolderNode()
        {
            Folders.Advise(folders_Changed);
            sorter = new ListView<Folder, int, string>(JMapFactories.Int,
                delegate(Folder folder)
                { 
                    return folder.Item.ID; 
                },
                null,
                delegate(Folder folder)
                { 
                    return folder.Name.ToLowerInvariant();
                },
                String.Compare);

            lazyChildren = new LazyLoadingList(LoadChildren);
            Children = lazyChildren;
        }

        readonly static DependencyProperty<IObservableList> children = DependencyProperty<IObservableList>.Register(typeof(FolderNode));
        public IObservableList Children
        {
            get
            {
                return GetValue(children);
            }

            set
            {
                SetValue(children, value);
            }
        }

        IDisposable LoadChildren()
        {
            return Folders.Get(
                delegate (FolderListItemCollection folders)
                {
                    if (folders != null)
                    {
                        sorter.Source = new ListConverter(ConvertListItem) { List = folders };
                        folders.Advise(Update);
                    }
                    else
                    {
                        sorter.Source = null;
                    }
                    Children = new ReferenceObservableHierarchyList() { List = sorter.Target };
                },
                lazyChildren.SetException);
        }

        internal new ListNode ParentList
        {
            get
            {
                return (ListNode)base.ParentList;
            }
        }

        static int CompareByUrl(object left, object right)
        {
            return ((Folder)left).ServerRelativeUrl.CompareTo(((Folder)right).ServerRelativeUrl);
        }


        void Update(ListItem listItem)
        {
            sorter.Update(listItem.Folder);
        }

        static Folder ConvertListItem(object listItem)
        {
            return ((ListItem)listItem).Folder;
        }

        ContextMenu IExplorerNode.ContextMenu(IHierarchyLevel level, long index)
        {
            return NodeUtil.FolderContextMenu(this);
        }

        internal void Refresh()
        {
            folderItems = folderItems.EnsureDispose();
            ClearFolders();
            lazyChildren = new LazyLoadingList(LoadChildren);
            Children = lazyChildren;
        }

        string ITreeNode.Title
        {
            get
            {
                return Name;
            }
        }

        string ITreeNode.Image
        {
            get { return "/_layouts/images/folder.gif"; }
        }


        string ITreeNode.Description
        {
            get { return null; }
        }

        internal FolderItems FolderItems
        {
            get
            {
                if (folderItems == null)
                {
                    folderItems = new FolderItems(this);
                }

                return folderItems;
            }
        }

        public override void Dispose()
        {
            folderItems = folderItems.EnsureDispose();
            base.Dispose();
        }

        void folders_Changed()
        {
            bool hasChildren = false;
            if (Folders.State == LazyState.Loaded)
            {
                disposable = disposable.EnsureDispose();
                if (Folders.Value != null)
                {
                    disposable = Folders.Value.Advise(folders_Changed);
                    hasChildren = Folders.Value.Count == 0;
                }
            }

            if (this.hasChildren != hasChildren)
            {
                this.hasChildren = hasChildren;
                Notify();
            }
        }

        NavigateDisposition IExplorerNode.Navigate(DeepLink deepLink)
        {
            if((deepLink.FolderUrl + '/').StartsWith(Url + '/', StringComparison.InvariantCultureIgnoreCase))
            {
                if (Url.Length == deepLink.FolderUrl.Length)
                {
                    FolderItems.Navigate(deepLink);
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
                if (deepLink == null)
                {
                    deepLink = new DeepLink()
                    {
                        NodeType = IsRootFolder ? NodeType.List : NodeType.Folder,
                        FolderUrl = Url,
                        ListId = ParentList.ID,
                        WebUrl = UriUtility.GetServerRelativeUrl(ParentList.ParentWeb.Url)
                    };
                }

                return deepLink;
            }
        }
    }
}
