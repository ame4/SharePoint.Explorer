using SharePoint.Explorer.Modules.Common;
using SharePoint.Explorer.Modules.Configurations;
using SharePoint.Explorer.Modules.Webs;
using James.SharePoint;
using JScriptSuite.Html;
using JScriptSuite.InteropServices;
using JScriptSuite.JScriptLib.DataBinding;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.DataBinding.Providers.DependencyObjects;
using JScriptSuite.JScriptLib.Html;
using JScriptSuite.JScriptLib.UI.Controls.Menus;
using JScriptSuite.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JScriptSuite.JScriptLib.UI.Controls.Trees;

namespace SharePoint.Explorer.Modules.Nodes
{
    public class RootNode : DependencyObject, IExplorerNode
    {
        internal RootNodes RootNodes;
        internal readonly WebNode WebNode;

        public RootNode()
        {
            WebNode = new WebNode(this);
        }

        readonly static DependencyProperty<bool> isSite = DependencyProperty<bool>.Register(typeof(RootNode));

        public bool IsSite
        {
            get
            {
                return GetValue(isSite);
            }

            set
            {
                SetValue(isSite, value);
            }
        }

        public string Title
        {
            get
            {
                return ((ITreeNode)WebNode).Title;
            }

            set
            {
                WebNode.Title = value;
            }
        }

        string url;
        public string Url
        {
            get
            {
                string server = UriUtility.GetServer(url);
                if(server != null)
                {
                    string currentServer = UriUtility.GetServer(HtmlWindow.Current.Location.Href);
                    if(currentServer.EqualsNoCase(server))
                    {
                        return UriUtility.GetServerRelativeUrl(url);
                    }
                }

                return url;
            }

            set
            {
                if(!string.IsNullOrEmpty(value) && value[0] == '/')
                {
                    value = UriUtility.GetServer(HtmlWindow.Current.Location.Href) + value;
                }

                url = value;
            }
        }

        internal void ApplyRootUrl(params string[] parameters)
        {
            WebNode.Url = string.Format(url, parameters);
        }

        internal bool IsCurrent
        {
            get
            {
                return RootNodes.IsCurrent(this);
            }
        }

        DeepLink IExplorerNode.DeepLink => WebNode.DeepLink;

        string ITreeNode.Image => WebNode.Image;

        string ITreeNode.Description => WebNode.Description;

        IObservableList IHierarchyItem.Children => WebNode.Children;

        public ContextMenu ContextMenu(IHierarchyLevel level, long index)
        {
            return NodeUtil.RootContextMenu(
                delegate()
                {
                    RootNodes.RemoveRootNode(index);
                },
                WebNode.Refresh);
        }

        public NavigateDisposition Navigate(DeepLink deepLink)
        {
            return ((IExplorerNode)WebNode).Navigate(deepLink);
        }
    }
}
