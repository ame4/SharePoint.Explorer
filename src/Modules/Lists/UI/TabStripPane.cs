using SharePoint.Explorer.Modules.Common;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.UI.Controls.DataBinding;
using JScriptSuite.JScriptLib.UI.Controls.Tabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Lists.UI
{
    class TabStripPane<TData> : UserControl<TData>
    {
        internal readonly TabStrip TabStrip;
        internal readonly TabStripSelectionConverter SelectedIndex;
        internal TabStripPane()
        {
            TabStrip = new TabStrip();
            SelectedIndex = new TabStripSelectionConverter(TabStrip);
        }
    }
}
