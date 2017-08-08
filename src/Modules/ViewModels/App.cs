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

namespace SharePoint.Explorer.ViewModels
{
    public class App : Observable
    {
        readonly WebPart webPart;
        readonly ObservableProperty<RootNodes> rootNodes;
        readonly AddRoot addRoot;
        DeepLinkViewModel deepLink;
        MainPane mainPane;

        Screen screen;

        App()
        {
            webPart = new WebPart();
            rootNodes = new ObservableProperty<RootNodes>(this);
            addRoot = new AddRoot(this);
            HtmlWindow.Current.AdviseLoad(Load);
        }

        public static void Start()
        {
            new App();
        }

        void Load()
        {
            screen = new Screen(webPart.Outer);
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

        internal RootNodes RootNodes
        {
            get
            {
                return rootNodes.Value;
            }

            private set
            {
                rootNodes.Value = value;
            }
        }

        internal async void SaveConfiguration()
        {
            string json = JsonSerializing.Serialize(RootNodes.Configuration);
            if(await LazyWindow.ShowWaiting("Saving configuration...",
                async delegate()
                {
                    await webPart.UpdateSettings(json);
                }))
                await MessageBox.Show(string.Format("Configuration has been successfully saved. {0}",
                        RootNodes.Configuration.DisplayCurrent.ToString()));
        }

        internal void LoadConfiguration()
        {
            Configuration<RootNode> configuration = ReadConfiguration(LazyWindow.ShowError);
            screen.Apply(configuration.Screen);
            RootNodes = new RootNodes() { Configuration = configuration };
        }

        internal void AddRoot()
        {
            addRoot.Show();
        }

        Configuration<RootNode> ReadConfiguration(Action<Exception> failed)
        {
            Configuration<RootNode> configuration = null;
            try
            {
                configuration = JsonSerializing.Deserialize<Configuration<RootNode>>(webPart.Settings);
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
                configuration.Roots = new ObservableList<RootNode>();
            }

            return configuration;
        }
    }
}
