using SharePoint.Explorer.Modules.Common;
using SharePoint.Explorer.Modules.Nodes;
using JScriptSuite.DataBinding;
using JScriptSuite.JScriptLib.UI.Controls.DataBinding;
using JScriptSuite.JScriptLib.UI.Controls.Tabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Lists.UI
{
    class FolderNodePane : TabStripPane<FolderNode>
    {
        [Binder]
        static void Bind(IBinder<FolderNodePane> binder)
        {
            binder.AddBinding<int>(pane => pane.Data.DeepLink.TabItemIndex,
                pane => pane.SelectedIndex.Value,
                BindingDirection.TwoWay
            );
        }
    }
}
