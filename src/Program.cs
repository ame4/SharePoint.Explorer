using SharePoint.Explorer.ViewModels;
using JScriptSuite.BrowserHost;
using JScriptSuite.BrowserHost.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SharePoint.Explorer
{
    class Program
    {
        readonly static string sharePointLibrary = "http://<server>/<web>/<Documentlibrary>/<Folder>";

        [STAThread]
        static void Main(string[] args)
        {
            IBrowserHost host = Host.CreateBrowserHost(); 
            new WebDavPageFactory() // or SharePointPageFactory
                {
                    PageUrlFormat = sharePointLibrary + "/{0}.aspx",    // SharePoint.Explorer.aspx -  WebPartPage
                    ScriptPathFormat = sharePointLibrary + "/{0}.js"    // SharePoint.Explorer.js
                }
                .AddPage(host, "SharePoint.Explorer")
                .Add(App.Start, "StartExplorer")
                .Navigate();

            host.Run();
        }
    }
}
