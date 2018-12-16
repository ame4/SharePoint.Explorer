using James.SharePoint;
using JScriptSuite.InteropServices;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.DataBinding.Providers.DependencyObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class ViewsViewModel : ActivableListViewModel
    {
        internal readonly ListView<View, Guid, object[]> OverviewListView;
        
        internal readonly FieldsEditor FieldsEditor;
        internal ViewsViewModel()
        {
            OverviewListView = new ListView<View, Guid, object[]>(JMapFactories.Guid, delegate(View view) { return view.ID; });
            FieldsEditor = new FieldsEditor();
        }


        readonly static DependencyProperty<View> selectedItem = DependencyProperty<View>.Register(typeof(ViewsViewModel));
        internal View SelectedItem
        {
            get { return GetValue(selectedItem); }
            set { SetValue(selectedItem, value); }
        }

    }
}
