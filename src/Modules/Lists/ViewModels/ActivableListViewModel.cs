using SharePoint.Explorer.Modules.Common;
using SharePoint.Explorer.Modules.Nodes;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class ActivableListViewModel : Activable
    {
        readonly ObservableProperty<ListNode> selectedList;

        internal ActivableListViewModel()
        {
            selectedList = new ObservableProperty<ListNode>(this);
        }

        internal virtual ListNode SelectedList
        {
            get
            {
                return selectedList.Value;
            }

            set
            {
                selectedList.Value = value;
            }
        }
    }
}
