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
using JScriptSuite.JScriptLib.DataBinding.Providers.DependencyObjects;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class FolderItems : DependencyObject, IDisposable
    {
        internal readonly FolderNode ParentFolder;
        ContentTypeDetail contentTypeDetail;
        LazyObservable<FolderListItemCollection> items;
        ListDetail listDetail;

        internal FolderItems(FolderNode parentFolder)
        {
            ParentFolder = parentFolder;
        }

        readonly DependencyProperty<IEnumerable<Field>> fields = DependencyProperty<IEnumerable<Field>>.Register(typeof(FolderItems));
        internal IEnumerable<Field> Fields
        {
            get
            {
                return GetValue(fields);
            }

            private set
            {
                SetValue(fields, value);
            }
        }

        readonly static DependencyProperty<ContentType> contentType = DependencyProperty<ContentType>.Register(typeof(FolderItems));
        internal ContentType ContentType
        {
            get
            {
                return GetValue(contentType);
            }

            set
            {
                SetValue(contentType, value);
                
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if(e.Property == contentType)
                ParentFolder.DeepLink.ContentTypeId = ContentType?.ID;
        }

        internal bool CanRefresh
        {
            get
            {
                return contentTypeDetail != null && contentTypeDetail.ContentType == ContentType;
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
                Fields = null;
                DisposeItems();
                return null;
            }

            if (this.contentTypeDetail != contentTypeDetail || this.listDetail != listDetail)
            {
                Fields = null;
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
                        Fields = fields;
                    }
                }
                else if(ParentFolder.ParentList.EnableAttachments)
                {
                    Field[] fields = new Field[contentTypeDetail.Fields.Count + 1];
                    fields[0] = listDetail.Fields.TryGetFieldById(BuiltInFields.Attachments.ID);
                    contentTypeDetail.Fields.CopyTo(fields, 1);
                    Fields = fields;
                }

                if (Fields == null)
                {
                    Fields = contentTypeDetail.Fields;
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
