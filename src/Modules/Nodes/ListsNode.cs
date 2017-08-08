using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePoint.Explorer.Modules.Common;
using JScriptSuite.JScriptLib.DataBinding;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.UI.Controls.Menus;
using JScriptSuite.Logging;
using James.SharePoint;
using JScriptSuite.InteropServices;
using JScriptSuite.JScriptLib.DataBinding.Providers.Lazy;
using JScriptSuite.JScriptLib.UI.Controls.Trees;
using JScriptSuite.JScriptLib.DataBinding.Providers.DependencyObjects;

namespace SharePoint.Explorer.Modules.Nodes
{
    class ListsNode : DependencyObject, IExplorerNode
    {
        readonly WebNode parent;
        DeepLink deepLink;
        LazyLoadingList lazyList;

        internal ListsNode(WebNode parent)
        {
            this.parent = parent;
            lazyList = new LazyLoadingList(LoadChildren);
            Children = lazyList;
            parent.Lists.Advise(Notify);
        }

        protected override void Notify()
        {
            if (lazyList == null && (parent.Lists.State == LazyState.Empty || parent.Lists.State == LazyState.Loading))
            {
                lazyList = new LazyLoadingList(LoadChildren);
                Children = lazyList;
            }
            base.Notify();
        }
        IDisposable LoadChildren()
        {
            return parent.Lists.Get(
                delegate (ObservableList<List> source)
                {
                    if (source == null)
                    {
                        Children = null;
                    }
                    else
                    {
                        JArray<List> ar = new JArray<List>((int)source.Count);
                        for (int i = 0; i < ar.Length; i++)
                        {
                            ar[i] = source[i];
                        }

                        ar.Sort(CompareByTitle);
                        Children = new ObservableHierarchyList<List>(ar);
                    }

                    lazyList = lazyList.EnsureDispose();
                },
                lazyList.SetException);
        }

        readonly static DependencyProperty<IObservableList> children = DependencyProperty<IObservableList>.Register(typeof(ListsNode));
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
        ContextMenu IExplorerNode.ContextMenu(IHierarchyLevel level, long index)
        {
            return NodeUtil.RefeshContextMenu(Refresh);
        }

        internal void Refresh()
        {
            parent.ClearLists();
        }

        string ITreeNode.Image
        {
            get { return "/_layouts/images/GRID.GIF"; }
        }

        string ITreeNode.Title
        {
            get {
                return parent.Lists.State == LazyState.Loaded ? string.Format("Lists ({0})", parent.Lists.Value.Count) : "Lists";
            }
        }

        string ITreeNode.Description
        {
            get { return null; }
        }

        public DeepLink DeepLink
        {
            get
            {
                if(deepLink == null)
                {
                    deepLink = new DeepLink()
                    {
                        NodeType = NodeType.Lists,
                        WebUrl = parent.DeepLink.WebUrl
                    };
                }

                return deepLink;
            }
        }

        NavigateDisposition IExplorerNode.Navigate(DeepLink deepLink)
        {
            if(deepLink.WebUrl.Length == DeepLink.WebUrl.Length)
            {
                if(deepLink.NodeType == DeepLink.NodeType)
                {
                    return NavigateDisposition.Complete;
                }

                if(deepLink.ListId != null)
                {
                    return NavigateDisposition.Expand;
                }
            }

            return NavigateDisposition.Next;
        }


        static int CompareByTitle(List left, List right) { return left.Title.CompareLocale(right.Title); }

    }
}
