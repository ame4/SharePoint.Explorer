using SharePoint.Explorer.Modules.Lists.ViewModels;
using SharePoint.Explorer.Modules.Nodes;
using JScriptSuite.JScriptLib.UI.Controls.DataBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Lists.UI
{
    class ListGeneralPane : UserControl<ListGeneralViewModel>
    {
        internal ListGeneralPane()
        {
            Data = new ListGeneralViewModel();
        }
    }
}
