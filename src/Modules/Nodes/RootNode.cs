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

namespace SharePoint.Explorer.Modules.Nodes
{
    public class RootNode : WebNode
    {
        internal RootNodes RootNodes;

        public RootNode()
        {
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

        public new string Title
        {
            get
            {
                return base.Title;
            }

            set
            {
                base.Title = value;
            }
        }

        public new string Url
        {
            get
            {
                string url = base.Url;
                string server = UriUtility.GetServer(url);
                if(server != null)
                {
                    string currentServer = UriUtility.GetServer(HtmlWindow.Current.Location.Href);
                    if(currentServer.EqualsNoCase(server))
                    {
                        return UriUtility.GetServerRelativeUrl(url);
                    }
                }

                return base.Url;
            }

            set
            {
                if(!string.IsNullOrEmpty(value) && value[0] == '/')
                {
                    value = UriUtility.GetServer(HtmlWindow.Current.Location.Href) + value;
                }

                base.Url = value;
            }
        }

        internal bool IsCurrent
        {
            get
            {
                return RootNodes.IsCurrent(this);
            }
        }

        public override ContextMenu ContextMenu(IHierarchyLevel level, long index)
        {
            return NodeUtil.RootContextMenu(delegate()
            {
                RootNodes.RemoveRootNode(index);
            },
            Refresh);
        }

        protected override IDisposable LoadSubsites()
        {
            return IsSite ? LoadSiteCollection() : base.LoadSubsites();
        }
    }
}
