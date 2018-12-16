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
    class ContentTypesViewModel : ActivableListViewModel
    {
        internal readonly ListView<ListContentType, string, object[]> OverviewListView;
        internal readonly FieldsEditor FieldsEditor;

        internal ContentTypesViewModel()
        {
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

        readonly static DependencyProperty<ListContentType> selectedItem = DependencyProperty<ListContentType>.Register(typeof(ContentTypesViewModel));
        internal ListContentType SelectedItem
        {
            get { 
                return GetValue(selectedItem); 
            }
            
            set { 
                SetValue(selectedItem, value); 
            }
        }
    }
}
