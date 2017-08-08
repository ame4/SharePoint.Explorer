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
using JScriptSuite.JScriptLib.Html;
using JScriptSuite.JScriptLib.DataBinding.Providers.Lazy;
using JScriptSuite.JScriptLib.UI.Controls.Trees;
using JScriptSuite.Html;
using JScriptSuite.JScriptLib.DataBinding.Providers.DependencyObjects;

namespace SharePoint.Explorer.Modules.Nodes
{
    class WebsNode : DependencyObject, IExplorerNode
    {
        readonly WebNode parent;
        readonly ListView<Web, string, string> sorter;
        DeepLink deepLink;
        LazyLoadingList lazyList;

        internal WebsNode(WebNode parent)
        {
            this.parent = parent;
            sorter = new ListView<Web, string, string>(JMapFactories.String, 
                delegate(Web web) { return UriUtility.GetFileName(web.Url).ToLowerInvariant(); },
                null,
                delegate(Web web) { return web.Title; },
                String.Compare);

            lazyList = new LazyLoadingList(LoadChildren);
            Children = lazyList;
            parent.Webs.Advise(Notify);
        }

        protected override void Notify()
        {
            if(lazyList == null && (parent.Webs.State == LazyState.Empty || parent.Webs.State == LazyState.Loading))
            {
                lazyList = new LazyLoadingList(LoadChildren);
                Children = lazyList;
            }
            base.Notify();
        }
        IDisposable LoadChildren()
        {
            return parent.Webs.Get(
                delegate (ObservableList<Web> source)
                {
                    if (source == null)
                    {
                        Children = null;
                    }
                    else
                    {
                        sorter.Source = source;
                        Children = sorter.Target;
                    }

                    lazyList = lazyList.EnsureDispose();
                },
                lazyList.SetException);
        }

        readonly static DependencyProperty<IObservableList> children = DependencyProperty<IObservableList>.Register(typeof(WebsNode));
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
            parent.ClearWebs();
        }

        string ITreeNode.Image
        {
            get { return "/_layouts/images/web.gif"; }
        }

        string ITreeNode.Title
        {
            get
            {
                return parent.Webs.Value != null ? string.Format("Webs ({0})", parent.Webs.Value.Count) : "Webs (?)";
            }
        }

        string ITreeNode.Description
        {
            get { return null; }
        }

        NavigateDisposition IExplorerNode.Navigate(DeepLink deepLink)
        {
            if(DeepLink.WebUrl.Length < deepLink.WebUrl.Length)
            {
                return NavigateDisposition.Expand;
            }

            return DeepLink.NodeType == deepLink.NodeType ? NavigateDisposition.Complete : NavigateDisposition.Next;
        }

        public DeepLink DeepLink
        {
            get
            {
                if(deepLink == null)
                {
                    deepLink = new DeepLink()
                    {
                        NodeType = NodeType.Webs,
                        WebUrl = parent.DeepLink.WebUrl
                    };
                }

                return deepLink;
            }
        }
    }
}
