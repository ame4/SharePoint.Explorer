using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using James.SharePoint;
using JScriptSuite.DataBinding;
using JScriptSuite.Html;
using JScriptSuite.InteropServices;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.UI.Components;
using JScriptSuite.JScriptLib.UI.Controls;
using JScriptSuite.JScriptLib.UI.Controls.Buttons;
using JScriptSuite.JScriptLib.UI.Controls.GridLayout;
using JScriptSuite.JScriptLib.UI.Controls.Lazy;
using JScriptSuite.JScriptLib.UI.Controls.Popups;
using JScriptSuite.JScriptLib.UI.Controls.Windows;
using JScriptSuite.Common;
using JScriptSuite.JScriptLib.DataBinding;
using SharePoint.Explorer.Modules.Common;
using JScriptSuite.JScriptLib.UI.Controls.DataWindows;
using JScriptSuite.JScriptLib.Html;
using SharePoint.Explorer.Modules.Lists.UI;
using System.Threading.Tasks;
using System.Threading;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class ListItemEditor : Observable, ISavable
    {
        readonly static HtmlLazy<ListItemEditorWindow> window = new DefaultHtmlLazy<ListItemEditorWindow>();

        readonly ListSplitter<int, ListContentType> contentTypesSplitter;
        readonly ObservableProperty<ListItem> listItem;
        readonly EditAction editAction;

        internal readonly ObservableProperty<ContentType> contentType;
        internal ObservableList<ListContentType> ContentTypes;

        internal readonly string Title;
        IDisposable disposable;

        ListItemEditor(EditAction editAction)
        {
            this.editAction = editAction;

            contentTypesSplitter = new ListSplitter<int, ListContentType>(JMapFactories.Int.CreateJSMap<int>(),
                delegate(ListContentType ct)
                {
                    return (int)(ct.ID.StartsWith(BuiltInContentTypeIds.Folder) ? FileSystemObjectType.Folder : FileSystemObjectType.File);
                });

            contentType = new ObservableProperty<ContentType>(this);
            this.listItem = new ObservableProperty<ListItem>(this);

            ListItem listItem;

            switch (editAction.EditMode)
            {
                case EditMode.Edit:
                    Title = string.Format("Edit item {0}", editAction.ListItem.ID);
                    LazyWindow.ShowWaiting();
                    editAction.ListItem.ParentList.GetItemById(editAction.ListItem.ID,
                        Show,
                        LazyWindow.ShowError);
                    break;

                case EditMode.Copy:
                    Title = string.Format("Copy item {0}", editAction.ListItem.ID);
                    LazyWindow.ShowWaiting();
                    editAction.ListItem.ParentList.GetItemById(editAction.ListItem.ID,
                        Show,
                        LazyWindow.ShowError);
                    break;

                case EditMode.AddItem:
                    Title = "Add new item";
                    listItem = editAction.TargetList.Add(FileSystemObjectType.File);
                    listItem.MoveTo(editAction.TargetFolder);
                    Show(listItem);
                    break;

                case EditMode.AddFolder:
                    Title = "Add new folder";
                    listItem = editAction.TargetList.Add(FileSystemObjectType.Folder);
                    listItem.MoveTo(editAction.TargetFolder);
                    Show(listItem);
                    break;

                default:
                    throw new ApplicationException("Unexpected");
            }

        }

        internal static void Show(EditAction action)
        {
            new ListItemEditor(action);
        }

        internal ContentType DefaultContentType
        {
            get
            {
                return editAction.TargetList.ContentTypes.Value != null ? editAction.TargetList.ContentTypes.Value[0] : null;
            }
        }

        void Show(ListItem listItem)
        {
            this.contentType.Value = null;
            ListItem = listItem;
            LazyWindow.ShowWaiting();
            editAction.TargetList.ContentTypes.Get(delegate(ObservableList<ListContentType> contentTypes)
                {
                    contentTypesSplitter.List = contentTypes;
                    ContentTypes = contentTypesSplitter[(int)listItem.FileSystemObjectType];
                    this.ContentType = ContentTypes.FirstOrDefault(delegate(ListContentType contentType) { return contentType.ID == listItem.ContentTypeId; });

                    if (editAction.EditMode == EditMode.Copy)
                    {
                        this.ContentType.Detail.Get(delegate(ContentTypeDetail contentTypeDetail)
                        {
                            ListItem newListItem = listItem.ParentList.Add(listItem.FileSystemObjectType);
                            newListItem.ContentTypeId = listItem.ContentTypeId;
                            for (int i = 0; i < contentTypeDetail.Fields.Count; i++)
                            {
                                Field field = contentTypeDetail.Fields[i];
                                if (field.ShowInEditForm && !field.ReadOnly)
                                {
                                    newListItem[field] = listItem[field];
                                }
                            }
                            newListItem.MoveTo(editAction.TargetFolder);
                            ListItem = newListItem;
                            listItem.Dispose();
                            LoadSuccess();
                        },
                        LazyWindow.ShowError);
                    }
                    else
                    {
                        LoadSuccess();
                    }
                },
                LazyWindow.ShowError);
        }

        void LoadSuccess()
        {
            if (ContentType == null && ContentTypes.Count == 1)
            {
                this.ContentType = ContentTypes[0];
                ListItem.ContentTypeId = ContentType.ID;
            }

            LazyWindow.HideWaiting();
            window.Value.Data = this;
            window.Value.Show();
            disposable = ListItem.Advise(Notify);
        }

        internal ListItem ListItem
        {
            get
            {
                return listItem.Value;
            }

            set
            {
                listItem.Value = value;
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
                if (contentType.Value != value)
                {
                    contentType.Value = value;
                    if (ListItem != null)
                    {
                        ListItem.ContentTypeId = value != null ? value.ID : null;
                    }
                }
            }
        }

        public Task Save(CancellationToken cancellationToken)
        {
            return listItem.Value.Update(cancellationToken);     
        }

        public bool IsDirty
        {
            get
            {
                return ListItem != null && ListItem.IsDirty;
            }
        }

        public void Close()
        {
            disposable = disposable.EnsureDispose();
            ListItem = ListItem.EnsureDispose();
        }
    }

    class EditAction
    {
        internal EditMode EditMode;
        internal ListItem ListItem;
        internal Folder TargetFolder; // for copy and add only
        internal List TargetList;
    }

    enum EditMode
    {
        Edit,
        Open,
        Upload,
        AddFolder,
        AddItem,
        Copy,
        Delete,
        Refresh
    }
}
