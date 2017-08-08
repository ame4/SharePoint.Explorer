using SharePoint.Explorer.Modules.Configurations;
using SharePoint.Explorer.Modules.Nodes;
using SharePoint.Explorer.ViewModels;
using James.SharePoint;
using JScriptSuite.Common;
using JScriptSuite.Html;
using JScriptSuite.JScriptLib.Common;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.UI.Controls.DataWindows;
using JScriptSuite.JScriptLib.UI.Controls.Lazy;
using JScriptSuite.JScriptLib.UI.Controls.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.UI
{
    class AddRootWindow : DataWindow<AddRoot>
    {
        internal AddRootWindow ()
        {
            Width = 300;
            Height = 130;
            Top = Math.Max((HtmlWindow.Current.InnerHeight - Height) / 2, 0);
            Left = Math.Max((HtmlWindow.Current.InnerWidth - Width) / 2, 0);
            AlwaysSave = true;
            Resizable = false;
            Movable = true;
            Titlebar.Visible = true;
            Titlebar.Text = "Add root node";
            InsertTemplate();
        }
    }
}
