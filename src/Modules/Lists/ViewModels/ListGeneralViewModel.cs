using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class ListGeneralViewModel : ActivableListViewModel
    {
        internal readonly FieldsEditor FieldsEditor;

        internal ListGeneralViewModel()
        {
            FieldsEditor = new FieldsEditor();
        }
    }
}
