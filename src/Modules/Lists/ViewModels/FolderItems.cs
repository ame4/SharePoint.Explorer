using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePoint.Explorer.Modules.Common;
using SharePoint.Explorer.Modules.Nodes;
using James.SharePoint;
using James.SharePoint.Services;
using JScriptSuite.Html;
using JScriptSuite.InteropServices;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.Xml;
using JScriptSuite.JScriptLib.DataBinding.Providers.Lazy;
using JScriptSuite.JScriptLib.Common;


namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class FolderItems : Observable, IDisposable
    {
        internal readonly FolderNode ParentFolder;
        readonly ObservableProperty<ContentType> contentType;
        ContentTypeDetail contentTypeDetail;
        LazyObservable<FolderListItemCollection> items;
        readonly ObservableProperty<IEnumerable<Field>> fields;
        ListDetail listDetail;

        internal FolderItems(FolderNode parentFolder)
        {
            ParentFolder = parentFolder;
            contentType = new ObservableProperty<ContentType>(this);
            fields = new ObservableProperty<IEnumerable<Field>>(this);
        }

        internal IEnumerable<Field> Fields
        {
            get
            {
                return fields.Value;
            }
        }

        internal ContentType ContentType
        {
            get
            {
                return contentType.Value;
            }

            set
            {
                contentType.Value = value;
                ParentFolder.DeepLink.ContentTypeId = value != null ? value.ID : null;
            }
        }

        internal bool CanRefresh
        {
            get
            {
                return contentTypeDetail != null && contentTypeDetail.ContentType == contentType.Value;
            }
        }

        internal void Refresh()
        {
            LazyObservable<FolderListItemCollection> ret = null;
            ret = new LazyObservable<FolderListItemCollection>(delegate()
            {
                return ParentFolder.GetListItems(null, 
                    Recursive.No,
                    Fields,
                    delegate(JArray<ListItem> listItems)
                    {
                        if (items == ret)
                        {
                            ret.Value = new FolderListItemCollection(ParentFolder, listItems);
                        }
                        else
                        {
                            Dispose(listItems);
                        }
                    },
                    delegate(Exception e)
                    {
                        ret.Exception = e;
                    });
            });

            DisposeItems();
            items = ret;
            Notify();
        }

        internal ILazyObservable<FolderListItemCollection> LoadItems(ListDetail listDetail, ContentTypeDetail contentTypeDetail)
        {
            if (contentTypeDetail == null || listDetail == null)
            {
                fields.Value = null;
                DisposeItems();
                return null;
            }

            if (this.contentTypeDetail != contentTypeDetail || this.listDetail != listDetail)
            {
                fields.Value = null;
                DisposeItems();
                this.contentTypeDetail = contentTypeDetail;
                this.listDetail = listDetail;

                if (ParentFolder.ParentList.BaseType == BaseType.DocumentLibrary)
                {
                    Field fileSizeDisplay = null;
                    int count = contentTypeDetail.Fields.Count;
                    if (contentTypeDetail.Fields.TryGetFieldById(BuiltInFields.FileSizeDisplay.ID) == null)
                    {
                        fileSizeDisplay = listDetail.Fields.TryGetFieldById(BuiltInFields.FileSizeDisplay.ID)
                            ?? BuiltInFields.FileSizeDisplay;
                        count++;
                    }


                    if (count != contentTypeDetail.Fields.Count)
                    {
                        Field[] fields = new Field[count];
                        contentTypeDetail.Fields.CopyTo(fields, 0);
                        count = contentTypeDetail.Fields.Count;
                        if (fileSizeDisplay != null) fields[count++] = fileSizeDisplay;
                        this.fields.Value = fields;
                    }
                }

                if(this.fields.Value == null)
                {
                    this.fields.Value = contentTypeDetail.Fields;
                }

                Refresh();
            }

            return items;
        }

        public void Dispose()
        {
            DisposeItems();
        }

        void DisposeItems()
        {
            if (items != null)
            {
                items.Disconnect();
                if (items.State == LazyState.Loaded)
                {
                    items.Value.Dispose();
                }

                items = null;
            }
        }

        void Dispose(JArray<ListItem> ar)
        {
            if (ar != null)
            {
                for (int i = 0; i < ar.Length; i++)
                {
                    ar[i].Dispose();
                }
            }
        }

        internal void Navigate(DeepLink deepLink)
        {
            if(!string.IsNullOrEmpty(deepLink.ContentTypeId))
            {
                ParentFolder.ParentList.ContentTypes.Get(
                    delegate(ObservableList<ListContentType> contentTypes){
                        ListContentType contentType = contentTypes.FirstOrDefault(delegate(ListContentType ct)
                        {
                            return ct.ID == deepLink.ContentTypeId;
                        });

                        if(contentType != null)
                        {
                            ContentType = contentType;
                        }
                    }, 
                    Dummies.Action);
            }
        }
    }
}
