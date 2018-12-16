using SharePoint.Explorer.Modules.UI;
using JScriptSuite.Html;
using JScriptSuite.Common;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JScriptSuite.JScriptLib.UI.Controls.Lazy;
using SharePoint.Explorer.Modules.Nodes;
using James.SharePoint;
using JScriptSuite.JScriptLib.UI.Controls.DataWindows;
using System.Threading;
using System.Threading.Tasks;
using JScriptSuite.JScriptLib.DataBinding.Providers.DependencyObjects;

namespace SharePoint.Explorer.ViewModels
{
    class AddRoot : DependencyObject, ISavable
    {
        AddRootWindow window;
        readonly App app;

        internal AddRoot(App app)
        {
            this.app = app;
        }

        readonly static DependencyProperty<string> url = DependencyProperty<string>.Register(typeof(AddRoot));
        internal string Url {
            get {
                return GetValue(url);
            }
            set {
                SetValue(url, value);
            }
        }

        readonly static DependencyProperty<bool> isSite = DependencyProperty<bool>.Register(typeof(AddRoot));
        internal bool IsSite {
            get { return GetValue(isSite); }
            set { SetValue(isSite, value); }
        }

        internal void Show()
        {
            if (window == null)
            {
                window = new AddRootWindow() { Data = this };
            }

            window.Show();
        }

        void Add(RootNode rootNode)
        {
            window.Hide();
            rootNode.RootNodes = app.RootNodes;
            app.RootNodes.Configuration.Roots.Add(rootNode);
        }

        bool ISavable.IsDirty
        {
            get { return false; }
        }

        async Task ISavable.Save(CancellationToken cancellationToken)
        {
            Url.NotEmpty("Please enter url first!");
            LazyWindow.ShowWaiting();
            RootNode web = new RootNode() { Url = Url, IsSite = IsSite };

            if (IsSite)
            {
                await web.Webs.Get(cancellationToken);
            }
            else
            {
                await web.Detail.Get(cancellationToken);
            }

            Add(web);
        }

        void ISavable.Close()
        {
        }
    }
}
