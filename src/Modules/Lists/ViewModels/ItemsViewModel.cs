using SharePoint.Explorer.Modules.Lists.UI;
using James.SharePoint;
using JScriptSuite.InteropServices;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.UI.Controls.Lazy;
using JScriptSuite.JScriptLib.UI.Controls.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class ItemsView : ActivableListViewModel
    {
        internal readonly ListView<ListItem, int, object[]> OverviewListView;
        internal readonly ListView<FieldValue, Guid, object[]> DetailListView;
        internal readonly ListView<PermissionValue, string, object[]> PermissionsListView;

        readonly ObservableProperty<FolderItems> folderItems;
        readonly ObservableProperty<ListItem> selectedItem;

        IDisposable adviseOverview;

        internal ItemsView()
        {
            folderItems = new ObservableProperty<FolderItems>(this);
            selectedItem = new ObservableProperty<ListItem>(this);
            OverviewListView = new ListView<ListItem, int, object[]>(JMapFactories.Int,
                delegate(ListItem listItem)
                {
                    return listItem.ID;
                },
                null,
                null,
                null);

            DetailListView = new ListView<FieldValue, Guid, object[]>(JMapFactories.Guid,
                delegate(FieldValue fieldValue)
                {
                    return fieldValue.Field.ID;
                });

            PermissionsListView = new ListView<PermissionValue, string, object[]>(JMapFactories.String,
                delegate(PermissionValue permissionValue)
                {
                    return permissionValue.Key;
                });

        }

        internal FolderItems FolderItems
        {
            get
            {
                return folderItems.Value;
            }

            set
            {
                if (value != null) SelectedList = value.ParentFolder.ParentList;
                folderItems.Value = value;
            }
        }

        internal ListItem SelectedItem
        {
            get
            {
                return selectedItem.Value;
            }

            set
            {
                selectedItem.Value = value;
            }
        }

        internal FolderListItemCollection Items
        {
            get
            {
                return (FolderListItemCollection)OverviewListView.Source;
            }

            set
            {
                if (!Observable.Equals(OverviewListView.Source, value))
                {
                    adviseOverview = adviseOverview.EnsureDispose();
                    if (value != null)
                    {
                        adviseOverview = value.Advise(delegate(ListItem listItem) { OverviewListView.Update(listItem); });
                    }

                    OverviewListView.Source = value;
                }
            }
        }

        internal void EditItem()
        {
            EditItem(SelectedItem);
        }

        internal void CopyItem()
        {
            CopyItem(SelectedItem);
        }

        internal void DeleteItem()
        {
            DeleteItem(SelectedItem);
        }


        internal void AddItem()
        {
            ListItemEditor.Show(new EditAction() { EditMode = EditMode.AddItem,
                TargetFolder = FolderItems.ParentFolder,
                TargetList = FolderItems.ParentFolder.ParentList });
        }

        internal void AddFolder()
        {
            ListItemEditor.Show(new EditAction() { EditMode = EditMode.AddFolder,
                TargetFolder = FolderItems.ParentFolder,
                TargetList = FolderItems.ParentFolder.ParentList
            });
        }


        internal void EditItem(ListItem listItem)
        {
            ListItemEditor.Show(new EditAction() { EditMode = EditMode.Edit, ListItem = listItem,
                TargetFolder = FolderItems.ParentFolder,
                TargetList = FolderItems.ParentFolder.ParentList
            });
        }

        internal void CopyItem(ListItem listItem)
        {
            ListItemEditor.Show(new EditAction() { EditMode = EditMode.Copy, ListItem = listItem,
                TargetFolder = FolderItems.ParentFolder,
                TargetList = FolderItems.ParentFolder.ParentList
            });
        }


        internal async void DeleteItem(ListItem listItem)
        {
            if(await MessageBox.Show(string.Format("Are you sure to delete item (ID={0})", listItem.ID),
                "Deleting...",
                MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    await LazyWindow.ShowWaiting(listItem.Delete);
                }
        }
    }
}
