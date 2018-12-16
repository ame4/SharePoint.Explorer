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
using JScriptSuite.JScriptLib.DataBinding.Providers.DependencyObjects;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class ItemsView : ActivableListViewModel
    {
        internal readonly ListView<ListItem, int, object[]> OverviewListView;
        internal readonly ListView<FieldValue, Guid, object[]> DetailListView;
        internal readonly ListView<PermissionValue, string, object[]> PermissionsListView;


        IDisposable adviseOverview;

        internal ItemsView()
        {
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

        readonly static DependencyProperty<FolderItems> folderItems = DependencyProperty<FolderItems>.Register(typeof(ItemsView));
        internal FolderItems FolderItems
        {
            get
            {
                return GetValue(folderItems);
            }

            set
            {
                SetValue(folderItems, value);
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if(e.Property == folderItems && e.NewValue != null)
            {
                SelectedList = FolderItems.ParentFolder.ParentList;
            }

            base.OnPropertyChanged(e);
        }

        readonly static DependencyProperty<ListItem> selectedItem = DependencyProperty<ListItem>.Register(typeof(ItemsView));

        internal ListItem SelectedItem
        {
            get
            {
                return GetValue(selectedItem);
            }

            set
            {
                SetValue(selectedItem, value);
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
