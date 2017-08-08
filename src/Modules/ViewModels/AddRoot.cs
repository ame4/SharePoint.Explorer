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

namespace SharePoint.Explorer.ViewModels
{
    class AddRoot : Observable, ISavable
    {
        AddRootWindow window;

        readonly App app;
        readonly ObservableProperty<string> url;
        readonly ObservableProperty<bool> isSite;

        internal AddRoot(App app)
        {
            this.app = app;
            url = new ObservableProperty<string>(this);
            isSite = new ObservableProperty<bool>(this);
        }

        internal string Url { get { return url.Value; } set { url.Value = value; } }
        internal bool IsSite { get { return isSite.Value; } set { isSite.Value = value; } }

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
