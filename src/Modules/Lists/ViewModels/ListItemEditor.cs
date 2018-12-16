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
using JScriptSuite.JScriptLib.DataBinding.Providers.DependencyObjects;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class ListItemEditor : DependencyObject, ISavable
    {
        readonly static HtmlLazy<ListItemEditorWindow> window = new DefaultHtmlLazy<ListItemEditorWindow>();

        readonly EditAction editAction;

        internal readonly ListItem ListItem;
        internal readonly ObservableList<ListContentType> ContentTypes;

        internal readonly string Title;
        readonly IDisposable disposable;

        ListItemEditor(EditAction editAction, ListItem listItem, ObservableList<ListContentType> contentTypes, ListContentType contentType)
        {
            this.editAction = editAction;
            ContentTypes = contentTypes;
            ContentType = contentType;


            switch (editAction.EditMode)
            {
                case EditMode.Edit:
                    Title = string.Format("Edit item {0}", listItem.ID);
                    break;

                case EditMode.Copy:
                    Title = string.Format("Copy item {0}", listItem.ID);
                    break;

                case EditMode.AddItem:
                    Title = "Add new item";
                    break;

                case EditMode.AddFolder:
                    Title = "Add new folder";
                    break;
            }

            ListItem = listItem;
            window.Value.Data = this;
            window.Value.Show();
            disposable = ListItem.Advise(Notify);
        }

        internal static async void Show(EditAction editAction)
        {
            ListItem listItem = null;
            ObservableList<ListContentType> affectedContentTypes = null;
            ListContentType contentType = null;
            if (await LazyWindow.ShowWaiting(
                async delegate (CancellationToken cancellationToken)
                {
                    switch (editAction.EditMode)
                    {
                        case EditMode.Edit:
                            listItem = await editAction.ListItem.ParentList.GetItemById(editAction.ListItem.ID, cancellationToken);
                            break;

                        case EditMode.Copy:
                            listItem = await editAction.ListItem.ParentList.GetItemById(editAction.ListItem.ID, cancellationToken);
                            break;

                        case EditMode.AddItem:
                            listItem = editAction.TargetList.Add(FileSystemObjectType.File);
                            listItem.MoveTo(editAction.TargetFolder);
                            break;

                        case EditMode.AddFolder:
                            listItem = editAction.TargetList.Add(FileSystemObjectType.Folder);
                            listItem.MoveTo(editAction.TargetFolder);
                            break;

                        default:
                            throw new ApplicationException("Unexpected");
                    }

                    ObservableList<ListContentType> contentTypes = await editAction.TargetList.ContentTypes.GetAsync(cancellationToken);
                    affectedContentTypes = new ObservableList<ListContentType>();
                    foreach (ListContentType hit in contentTypes)
                    {
                        if(hit.ID.StartsWith(BuiltInContentTypeIds.Folder) == (listItem.FileSystemObjectType == FileSystemObjectType.Folder))
                        {
                            affectedContentTypes.Add(hit);
                            if (hit.ID == listItem.ContentTypeId) contentType = hit;
                        }
                    }
                    if (contentType == null && affectedContentTypes.Count != 0)
                    {
                        contentType = affectedContentTypes[0];
                        listItem.ContentTypeId = contentType.ID;
                    }

                    contentType.NotNull("Unknown content type");

                    if (editAction.EditMode == EditMode.Copy)
                    {
                        ContentTypeDetail contentTypeDetail = await contentType.Detail.GetAsync(cancellationToken);
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
                        listItem.Dispose();
                        listItem = newListItem;
                    }
                }))
            {
                new ListItemEditor(editAction, listItem, affectedContentTypes, contentType);
            }
            else
                listItem?.Dispose();
        }

        internal ContentType DefaultContentType
        {
            get
            {
                return editAction.TargetList.ContentTypes.Value != null ? editAction.TargetList.ContentTypes.Value[0] : null;
            }
        }


        readonly static DependencyProperty<ContentType> contentType = DependencyProperty<ContentType>.Register(typeof(ListItemEditor));
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
            if(e.Property == contentType && ListItem != null) ListItem.ContentTypeId = ContentType?.ID;
        }

        public Task Save(CancellationToken cancellationToken)
        {
            return ListItem.Update(cancellationToken);     
        }

        public bool IsDirty
        {
            get
            {
                return ListItem.IsDirty;
            }
        }

        public void Close()
        {
            disposable.Dispose();
            ListItem.Dispose();
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
