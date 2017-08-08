using SharePoint.Explorer.Modules.Lists.UI;
using SharePoint.Explorer.Modules.Lists.ViewModels;
using JScriptSuite.DataBinding;
using JScriptSuite.Html;
using JScriptSuite.JScriptLib.UI.Controls;
using JScriptSuite.JScriptLib.UI.Controls.DataBinding;
using JScriptSuite.JScriptLib.UI.Controls.DataWindows;
using JScriptSuite.JScriptLib.UI.Controls.GridLayout;
using JScriptSuite.JScriptLib.UI.Controls.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Lists.UI
{
    class SchemaXmlEditorWindow : DataWindow<SchemaXmlEditor>
    {
        public SchemaXmlEditorWindow()
        {
            Buttons.Add(new MaximizeButton());
            Titlebar.Visible = true;
            Resizable = true;
            Movable = true;
            InsertTemplate();
        }

        [Binder]
        static void Bind(IBinder<SchemaXmlEditorWindow> binder)
        {
            binder.AddBinding<string>(window => window.Data.Title, window => window.Titlebar.Text, BindingDirection.OneWay);
        }
    }
}
