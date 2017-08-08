using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using James.SharePoint;
using jui = James.SharePoint.UI;
using JScriptSuite.Common;
using JScriptSuite.DataBinding;
using JScriptSuite.Html;
using JScriptSuite.InteropServices;
using JScriptSuite.JScriptLib.UI.Controls;
using JScriptSuite.JScriptLib.UI.Controls.Grids;

namespace SharePoint.Explorer.Modules.Lists.UI
{
    class ItemFieldsTable : IControl<HtmlTableSection>, IDisposable
    {
        readonly JArray<jui.ListItemEditor> editors;

        ListItem listItem;
        ContentTypeDetail contentType;

        internal ItemFieldsTable()
        {
            editors = new JArray<jui.ListItemEditor>();
        }


        public HtmlTableSection Element
        {
            get;
            set;
        }


        internal ListItem ListItem
        {
            get
            {
                return listItem;
            }

            set
            {
                if (listItem != value)
                {
                    listItem = value;
                    Synchronize();
                }
            }
        }

        internal ContentTypeDetail ContentType
        {
            get
            {
                return contentType;
            }

            set
            {
                if (contentType != value)
                {
                    contentType = value;
                    Synchronize();
                }
            }
        }

        public void Dispose()
        {
            DisposeEditors();
        }

        void DisposeEditors()
        {
            for (int i = 0; i < editors.Length; i++)
            {
                editors[i].Dispose();
            }

            editors.Clear();
        }

        

        void Synchronize()
        {
            DisposeEditors();
            while (Element.Rows.Length != 0)
            {
                Element.RemoveChild(Element.Rows[0]);
            }

            if (contentType != null && listItem != null)
            {
                for (int i = 0; i < contentType.Fields.Count; i++)
                {
                    Field field = contentType.Fields[i];
                    if ( (listItem.IsNew ? field.ShowInNewForm : field.ShowInEditForm)
                        && !field.ReadOnly
                        && field.ID != BuiltInFields.ContentType.ID)
                    {
                        HtmlTableRow row = Element.InsertRow(-1);
                        row.AppendChild(Element.OwnerDocument.CreateElement("TH")).InnerText = field.Title;
                        HtmlTableCell cell = row.InsertCell(-1);
                        jui.ListItemEditor editor = field.ReadOnly ? null : jui.UIUtility.CreateEditor(field, cell, listItem.ParentList);
                        if (editor != null)
                        {
                            editors.Push(editor);
                            if (listItem != null)
                            {
                                editor.ListItem = listItem;
                            }
                        }
                        else
                        {
                            jui.UIUtility.CreateGridColumn(field).Renderer(cell, listItem, null);
                        }
                    }
                }
            }
        }
    }
}
