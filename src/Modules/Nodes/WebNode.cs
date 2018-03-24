using System;
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
        public WebNode()
        {
            webs = new WebsNode(this);
            lists = new ListsNode(this);
            children = new ObservableHierarchyList<IExplorerNode>( new IExplorerNode[] { webs, lists });
        }

        protected override Web CreateWebInstance()
        {
            return new WebNode();
        }

        protected override List CreateListInstance()
        {
            return new ListNode();
        }

        [IgnoreDataMember]
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

        protected void Refresh()
        {
            webs.Refresh();
            lists.Refresh();
        }

        [IgnoreDataMember]
        public string Image
        {
            get { return "/_layouts/images/web.gif"; }
        }

        [IgnoreDataMember]
        public string Description
        {
            get { return Url; }
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

        [IgnoreDataMember]
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
    }
}
