using James.SharePoint;
using JScriptSuite.InteropServices;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class ViewsViewModel : ActivableListViewModel
    {
        internal readonly ListView<View, Guid, object[]> OverviewListView;
        readonly ObservableProperty<View> selectedItem;
        internal readonly FieldsEditor FieldsEditor;
        internal ViewsViewModel()
        {
            selectedItem = new ObservableProperty<View>(this);
            OverviewListView = new ListView<View, Guid, object[]>(JMapFactories.Guid, delegate(View view) { return view.ID; });
            FieldsEditor = new FieldsEditor();
        }

        internal View SelectedItem
        {
            get { return selectedItem.Value; }
            set { selectedItem.Value = value; }
        }

    }
}
