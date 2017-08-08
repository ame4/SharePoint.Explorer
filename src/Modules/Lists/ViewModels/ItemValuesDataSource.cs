using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using James.SharePoint;
using James.SharePoint.UI;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.UI.Controls.Grids;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class ItemValuesDataSource : ObservableList<FieldValue>
    {
        readonly static FieldCustom<string> urlField = new FieldCustom<string>(FieldType.Text,
            delegate(ListItem listItem) { return listItem.Url; },
            null) { ID = new Guid("{26BB9E6E-A242-4893-8C4E-77774D30ED85}"), Title = "_Url" },
            serverRelativeUrl = new FieldCustom<string>(FieldType.Text,
            delegate(ListItem listItem) { return listItem.ServerRelativeUrl; },
            null) { ID = new Guid("{D2BBE451-20E9-4FCF-8403-E6EFCD0DC3E4}"), Title = "_ServerRelativeUrl" };

        readonly ListItem listItem;
        readonly IEnumerable<Field> fields;
        IDisposable disposable;

        internal ItemValuesDataSource(ListItem listItem, IEnumerable<Field> fields)
        {
            this.listItem = listItem;
            this.fields = fields;
            if (listItem != null && fields != null)
            {
                disposable = listItem.Advise(Synchronize);
                Synchronize();
            }
        }

        public override void Dispose()
        {
            disposable = disposable.EnsureDispose();
            base.Dispose();
        }

        void Synchronize()
        {
            Clear();
            bool hasIDField = false;
            foreach (Field field in fields)
            {
                Add(field, listItem);
                if(field.ID == BuiltInFields.ID.ID)
                {
                    hasIDField = true;
                }
            }

            if(!hasIDField)
            {
                Add(BuiltInFields.ID, listItem);
            }

            
            Add(urlField, listItem);
            Add(serverRelativeUrl, listItem );
        }

        void Add(Field field, ListItem listItem)
        {
            Add(new FieldValue()
            {
                Field = field,
                Column = UIUtility.CreateGridColumn(field),
                ListItem = listItem
            });
        }

        public override bool Equals(object obj)
        {
            ItemValuesDataSource other = obj as ItemValuesDataSource;
            return other != null && other.fields == fields && other.listItem == listItem;
        }
    }

    class FieldValue
    {
        internal Field Field;
        internal GridColumn<ListItem> Column;
        internal ListItem ListItem;
    }
}
