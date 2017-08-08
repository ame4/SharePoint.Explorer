using James.SharePoint;
using JScriptSuite.InteropServices;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class ContentTypesViewModel : ActivableListViewModel
    {
        internal readonly ListView<ListContentType, string, object[]> OverviewListView;
        readonly ObservableProperty<ListContentType> selectedItem;
        internal readonly FieldsEditor FieldsEditor;

        internal ContentTypesViewModel()
        {
            selectedItem = new ObservableProperty<ListContentType>(this);
            OverviewListView = new ListView<ListContentType, string, object[]>(JMapFactories.String,
                delegate(ListContentType contentType) { return contentType.ID; },
                null,
                null,
                null);

            FieldsEditor = new FieldsEditor();
        }

        internal void EditContentType(ListContentType contentType)
        {
            if (contentType != null)
            {
                ContentTypeEditor.Show(contentType);
            }
        }

        internal ListContentType SelectedItem
        {
            get { 
                return selectedItem.Value; 
            }
            
            set { 
                selectedItem.Value = value; 
            }
        }
    }
}
