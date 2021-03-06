﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePoint.Explorer.Modules.Common;
using James.SharePoint;
using JScriptSuite.InteropServices;
using JScriptSuite.JScriptLib.DataBinding;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.UI.Controls.Menus;
using JScriptSuite.JScriptLib.Html;
using JScriptSuite.JScriptLib.UI.Controls.Trees;
using JScriptSuite.Html;
using JScriptSuite.JScriptLib.DataBinding.Providers.DependencyObjects;
using JScriptSuite.JScriptLib.DataBinding.Providers.Lazy;
using System.Runtime.Serialization;

namespace SharePoint.Explorer.Modules.Nodes
{
    public class WebNode : Web, IExplorerNode
    {
        readonly WebsNode webs;
        readonly ListsNode lists;
        DeepLink deepLink;
        readonly ObservableHierarchyList<IExplorerNode> children;

        internal readonly RootNode RootNode;
        public WebNode(RootNode rootNode)
        {
            RootNode = rootNode;
            webs = new WebsNode(this);
            lists = new ListsNode(this);
            children = new ObservableHierarchyList<IExplorerNode>( new IExplorerNode[] { webs, lists });
        }

        protected override Web CreateWebInstance()
        {
            return new WebNode(null);
        }

        protected override List CreateListInstance()
        {
            return new ListNode();
        }

        public IObservableList Children
        {
            get
            {
                return children;
            }
        }

        public virtual ContextMenu ContextMenu(IHierarchyLevel level, long index)
        {
            return NodeUtil.RefeshContextMenu(Refresh);
        }

        internal void Refresh()
        {
            webs.Refresh();
            lists.Refresh();
        }

        public string Image
        {
            get { return "/_layouts/images/web.gif"; }
        }

        public string Description
        {
            get { return Url; }
        }

        string ITreeNode.Title
        {
            get
            {
                return string.IsNullOrEmpty(Title) ? Url : Title;
            }
        }
        NavigateDisposition IExplorerNode.Navigate(DeepLink deepLink)
        {
            if((deepLink.WebUrl + '/').StartsWith(DeepLink.WebUrl + '/', StringComparison.InvariantCultureIgnoreCase))
            {
                return deepLink.NodeType == DeepLink.NodeType && deepLink.WebUrl.Length == deepLink.WebUrl.Length 
                    ? NavigateDisposition.Complete : NavigateDisposition.Expand;
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
                        NodeType = NodeType.Web,
                        WebUrl = UriUtility.GetServerRelativeUrl(Url)
                    };
                }

                return deepLink;
            }
        }

        protected override IDisposable LoadSubsites()
        {
            return RootNode != null && RootNode.IsSite ? LoadSiteCollection() : base.LoadSubsites();
        }

    }
}
