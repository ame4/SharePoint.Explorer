using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePoint.Explorer.Modules.Common;
using SharePoint.Explorer.Modules.Informations;
using James.SharePoint;
using JScriptSuite.JScriptLib.Common;
using JScriptSuite.JScriptLib.DataBinding;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.InteropServices;
using SharePoint.Explorer.Modules.Configurations;
using JScriptSuite.Html;
using JScriptSuite.JScriptLib.Remoting;
using JScriptSuite.JScriptLib.Html;
using JScriptSuite.JScriptLib.DataBinding.Providers.Lazy;
using System.Threading.Tasks;
using System.Threading;
using SharePoint.Explorer.ViewModels;

namespace SharePoint.Explorer.Modules.Nodes
{
    class RootNodes : ReferenceObservableHierarchyList
    {
        readonly ReferenceObservableList currenSite;
        readonly ReferenceObservableList roots;
        readonly LazyObservable<RootNode> lazyCurrenSite;
        readonly App app;
        LazyLoadingList loadingCurrenSite;
        readonly ReferenceObservable<Configuration<RootNode>> configuration;
        bool currentSiteLoadingAsSite;

        internal RootNodes(App app)
        {
            this.app = app;
            configuration = new ReferenceObservable<Configuration<RootNode>>();
            configuration.Advise(ConfigurationChange);
            roots = new ReferenceObservableList();
            currenSite = new ReferenceObservableList();
            lazyCurrenSite = new LazyObservable<RootNode>(LoadCurrentSite);
            loadingCurrenSite = new LazyLoadingList(LoadCurrentSiteList);
            List = new ObservableUnionList()
            {
                Lists = new ObservableList<IObservableList>()
                {
                    new ObservableList<ITreeNode>() { new InformationNode() },
                    currenSite,
                    roots,
                }
            };
        }

        internal void RemoveRootNode(long index)
        {
            index -= 1 + currenSite.Count;
            if(index < 0)
            {
                Configuration.DisplayCurrent = DisplayCurrentMode.None;
            }
            else
            {
                ((ObservableList<RootNode>)roots.List).RemoveAt((int)index);
            }
        }

        internal void InsertRootNode(long index, RootNode rootNode)
        {
            index -= 1 + currenSite.Count;
            ((ObservableList<RootNode>)roots.List).Insert((int)index, rootNode);
        }

        internal bool IsCurrent(RootNode rootNode)
        {
            return lazyCurrenSite.State == LazyState.Loaded && lazyCurrenSite.Value == rootNode;
        }

        async Task<RootNode> LoadCurrentSite(CancellationToken cancellationToken)
        {
            Web web = await Web.CurrentWeb.Get(cancellationToken);
            WebDetail detail = await web.Detail.Get(cancellationToken);

            RootNode rootNode = new RootNode()
            {
                Url = detail.Web.Url,
                IsSite = Configuration.EffectiveDisplayCurrent == DisplayCurrentMode.Site,
                Title = detail.Web.Url,
                RootNodes = this
            };

            app.ApplyRootUrl(rootNode);
            currentSiteLoadingAsSite = rootNode.IsSite;
            return rootNode;
        }


        internal Configuration<RootNode> Configuration
        {
            get
            {
                return configuration.Value;
            }

            set
            {
                configuration.Value = value;
            }
        }

        IDisposable LoadCurrentSiteList()
        {
            return lazyCurrenSite.Get(
                delegate (RootNode root)
                {
                    currenSite.List = root != null ? new ObservableList<Web>() { root.WebNode } : null;
                },
                loadingCurrenSite.SetException);
        }

        void ConfigurationChange()
        {
            if(Configuration != null)
            {
                roots.List = Configuration.Roots;
                if (Configuration.Roots != null)
                {
                    foreach (RootNode rootNode in Configuration.Roots)
                    {
                        rootNode.RootNodes = this;
                    }
                }

                switch (Configuration.EffectiveDisplayCurrent)
                {
                    case DisplayCurrentMode.None:
                        currenSite.List = null;
                        break;

                    case DisplayCurrentMode.Web:
                        if (currentSiteLoadingAsSite)
                        {
                            lazyCurrenSite.Clear();
                            loadingCurrenSite = new LazyLoadingList(LoadCurrentSiteList);
                            currenSite.List = loadingCurrenSite;
                        }

                        currenSite.List = loadingCurrenSite;
                        break;

                    case DisplayCurrentMode.Site:
                        if (!currentSiteLoadingAsSite)
                        {
                            lazyCurrenSite.Clear();
                        }

                        loadingCurrenSite = new LazyLoadingList(LoadCurrentSiteList);
                        currenSite.List = loadingCurrenSite;
                        break;
                }
            }
            else
            {
                roots.List = null;
                currenSite.List = null;
            }
        }
    }
}
