using James.SharePoint;
using JScriptSuite.InteropServices;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class FieldsEditor
    {
        internal readonly ListView<Field, string, object[]> FieldsListView;
        
        internal FieldsEditor()
        {
            FieldsListView = new ListView<Field, string, object[]>(JMapFactories.String,
                delegate(Field field) { return field.ID.ToString(); }, null, null, null);
        }


        internal void EditField(Field field)
        {
            FieldEditor.Show(field);
        }

    }
}
