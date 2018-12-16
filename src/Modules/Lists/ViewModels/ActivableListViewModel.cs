using SharePoint.Explorer.Modules.Common;
using SharePoint.Explorer.Modules.Nodes;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JScriptSuite.JScriptLib.DataBinding.Providers.DependencyObjects;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class ActivableListViewModel : Activable
    {

        readonly static DependencyProperty<ListNode> selectedList = DependencyProperty<ListNode>.Register(typeof(ActivableListViewModel));
        internal virtual ListNode SelectedList
        {
            get
            {
                return GetValue(selectedList);
            }

            set
            {
                SetValue(selectedList, value);
            }
        }
    }
}
