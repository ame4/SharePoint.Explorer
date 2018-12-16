using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePoint.Explorer.Modules.UI;
using JScriptSuite.Common;
using JScriptSuite.DataBinding;
using JScriptSuite.Html;
using JScriptSuite.JScriptLib.Common;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.DataBinding.Synchronizers;
using JScriptSuite.JScriptLib.UI.Components;
using James.SharePoint;
using SharePoint.Explorer.Modules.Common;
using James.SharePoint.WebParts;
using JScriptSuite.JScriptLib.Serializing;
using SharePoint.Explorer.Modules.Configurations;
using JScriptSuite.JScriptLib.UI.Controls.Lazy;
using JScriptSuite.JScriptLib.Remoting;
using JScriptSuite.JScriptLib.UI.Controls.Windows;
using JScriptSuite.JScriptLib.Html;
using SharePoint.Explorer.Modules.Nodes;
using JScriptSuite.InteropServices;
using System.Threading;
using JScriptSuite.JScriptLib.DataBinding.Providers.DependencyObjects;

namespace SharePoint.Explorer.ViewModels
{
    public class App : DependencyObject
    {
        readonly WebPart webPart;
        readonly AddRoot addRoot;
        DeepLinkViewModel deepLink;
        MainPane mainPane;

        Screen screen;

        App()
        {
            webPart = new WebPart();
            addRoot = new AddRoot(this);
            HtmlWindow.Current.AdviseLoad(Load);
        }

        internal static void Start()
        {
            new App();
        }

        string user;
        void Load()
        {
            screen = new Screen(webPart.Outer);
            user = HtmlWindow.Current.Get<JObject>("_spPageContextInfo")?.Get<string>("userLoginName")?.Replace(".", "_")?.Replace("@", "_");
            LoadConfiguration();
            this.deepLink = new DeepLinkViewModel(Navigate);
            DeepLink deepLink = DeepLink;
            mainPane = new MainPane(this) { Element = screen.Outer };
            if (deepLink != null)
            {
                Navigate(new DeepLink(deepLink));
            }
        }

        void Navigate(DeepLink deepLink)
        {
            mainPane.Navigate(deepLink);
        }

        internal DeepLink DeepLink
        {
            get
            {
                return deepLink.DeepLink;
            }

            set
            {
                deepLink.DeepLink = value;
            }
        }

        void SaveDeepLink()
        {
            DeepLink deepLink = new DeepLink();

        }

        readonly static DependencyProperty<RootNodes> rootNodes = DependencyProperty<RootNodes>.Register(typeof(App));
        internal RootNodes RootNodes
        {
            get
            {
                return GetValue(rootNodes);
            }

            private set
            {
                SetValue(rootNodes, value);
            }
        }

        internal async void SaveConfiguration()
        {
            string json = JsonSerializer.Serialize(RootNodes.Configuration);
            if(await LazyWindow.ShowWaiting("Saving configuration...",
                async delegate()
                {
                    await webPart.UpdateSettings(json);
                }))
                await MessageBox.Show(string.Format("Configuration has been successfully saved."));
        }

        internal void LoadConfiguration()
        {
            Configuration<RootNode> configuration = ReadConfiguration(LazyWindow.ShowError);
            screen.Apply(configuration.Screen);
            RootNodes = new RootNodes(this) { Configuration = configuration };
        }

        internal void AddRoot()
        {
            addRoot.Show();
        }

        internal void ApplyRootUrl(RootNode rootNode)
        {
            rootNode.ApplyRootUrl(user);
        }

        Configuration<RootNode> ReadConfiguration(Action<Exception> failed)
        {
            Configuration<RootNode> configuration = null;
            try
            {
                configuration = JsonDeserializer.Deserialize<Configuration<RootNode>>(webPart.Settings);
                if(configuration?.Roots != null)
                {
                    foreach (RootNode rootNode in configuration.Roots)
                    {
                        RootNode node = rootNode;
                        ApplyRootUrl(node);
                        if(rootNode.Url.Contains('{'))
                        {
                            node.WebNode.Detail.Get(
                                delegate (WebDetail detail)
                                {
                                    node.Title = detail.Web.Title;

                                }, Dummies.Action<Exception>);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                failed(e);
            }

            if (configuration == null)
            {
                configuration = new Configuration<RootNode>();
            }

            if (configuration.Screen == null)
            {
                configuration.Screen = new ScreenConfiguration();
            }

            if(configuration.DisplayCurrent == null)
            {
                configuration.DisplayCurrent = DisplayCurrentMode.Web;
            }

            if(configuration.Roots == null)
            {
                configuration.Roots = new RootsObservableList<RootNode>();
            }

            return configuration;
        }
    }
}
