using James.SharePoint.Styles;
using JScriptSuite.JScriptLib.UI.Controls.DataBinding;
using JScriptSuite.JScriptLib.UI.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.UI
{
    class Styles : SharePointStyles
    {
        public Styles()
        {
            new AllSharePointStyles();
        }
    }
}
