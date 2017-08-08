using James.SharePoint;
using James.SharePoint.Services;
using JScriptSuite.Html;
using JScriptSuite.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class FieldEditor : SchemaXmlEditor
    {
        readonly Field field;

        internal FieldEditor(Field field)
            : base(field.SchemaXml, "View field")
        {
            this.field = field;
        }

        internal static void Show(Field field)
        {
            new FieldEditor(field).Show();
        }
    }
}
