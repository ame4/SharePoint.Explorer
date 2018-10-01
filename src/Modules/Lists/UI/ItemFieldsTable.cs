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
using JScriptSuite.JScriptLib.UI.Components;
using JScriptSuite.JScriptLib.UI.Controls.Buttons;
using James.SharePoint.Styles;
using JScriptSuite.Html5.IO;
using JScriptSuite.JScriptLib.Common;
using JScriptSuite.Logging;

namespace SharePoint.Explorer.Modules.Lists.UI
{
    class ItemFieldsTable : IControl<HtmlTableSection>, IDisposable
    {
        readonly JArray<jui.ListItemEditor> editors;
        AddFilesButton<ImageTextButton> addFiles;
        ListItem listItem;
        IDisposable disposable;
        ContentTypeDetail contentType;
        HtmlTableSection element;
        internal ItemFieldsTable()
        {
            editors = new JArray<jui.ListItemEditor>();
        }


        public HtmlTableSection Element
        {
            get
            {
                return element;
            }
            set
            {
                if(element != value)
                {
                    if (element != null) Logger.Detach(element, this);
                    if (value != null) Logger.Attach(value, this);
                    element = value;
                }
            }
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
            disposable?.Dispose();
            if(addFiles != null) addFiles.Element = null;
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
                        AddEditor(field);
                    }
                }

                if(listItem.FileSystemObjectType == FileSystemObjectType.File && listItem.ParentList.EnableAttachments)
                {
                    Field field = listItem.ParentList.Detail.Value.Fields.TryGetFieldById(BuiltInFields.Attachments.ID);
                    HtmlTableCell cell = AddEditor(field).Cells[0];
                    HtmlElement element = cell.AppendChild(cell.OwnerDocument.CreateElement("SPAN"));
                    element.TabIndex = 1;
                    addFiles = new AddFilesButton<ImageTextButton>()
                    {
                        Element = element,
                        AllowMultipleSelection = true,
                        Button =
                        {
                            ImageClass = ListStyles.AddAttachments
                        },
                    };

                    Action<HtmlCollection<File>> addAttachments = (files) => listItem.Attachments.Value.Add(files);
                    disposable = listItem.Attachments.Advise(delegate ()
                    {
                        addFiles.Execute = listItem.Attachments.Value != null ? addAttachments : null;
                    });

                    if (listItem.Attachments.Value != null)
                        addFiles.Execute = addAttachments;
                }
            }
        }

        HtmlTableRow AddEditor(Field field)
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

            return row;
        }
    }
}
