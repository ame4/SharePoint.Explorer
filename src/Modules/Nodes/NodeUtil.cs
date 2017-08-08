using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePoint.Explorer.Modules.Lists;
using SharePoint.Explorer.Modules.Lists.UI;
using James.SharePoint;
using JScriptSuite.Html;
using JScriptSuite.Common;
using JScriptSuite.JScriptLib.DataBinding;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.UI.Controls.Menus;
using JScriptSuite.JScriptLib.UI.Controls.Windows;
using JScriptSuite.JScriptLib.UI.Controls.Lazy;
using SharePoint.Explorer.Modules.Lists.ViewModels;

namespace SharePoint.Explorer.Modules.Nodes
{
    class NodeUtil
    {
        readonly static ContextMenuCommand open = new ContextMenuCommand() { EditMode = EditMode.Open, Name = "Open", Image = "Download" };
        readonly static ContextMenuCommand edit = new ContextMenuCommand() { EditMode = EditMode.Edit, Name = "Edit...", Image = "Edit" };
        readonly static ContextMenuCommand copy = new ContextMenuCommand() { EditMode = EditMode.Copy, Name = "Copy...", Image = "Copy" };
        readonly static ContextMenuCommand addFolder = new ContextMenuCommand() { EditMode = EditMode.AddFolder, Name = "Add folder...", Image = "AddFolder" };
        readonly static ContextMenuCommand addItem = new ContextMenuCommand() { EditMode = EditMode.AddItem, Name = "Add item...", Image = "AddItem" };
        readonly static ContextMenuCommand upload = new ContextMenuCommand() { EditMode = EditMode.Upload, Name = "Upoad...", Image = "Upload" };
        readonly static ContextMenuCommand delete = new ContextMenuCommand() { EditMode = EditMode.Delete, Name = "Delete...", Image = "Delete" };
        readonly static ContextMenuCommand refreshCommand = new ContextMenuCommand() { EditMode = EditMode.Refresh, Name = "Refresh", Image = "Refresh" };
        readonly static ContextMenuCommands
            listCommands = new ContextMenuCommands()
            {
                List = new ContextMenuCommand[] { addItem, addFolder, refreshCommand },
                Folder = new ContextMenuCommand[] { edit, delete, addItem, addFolder, refreshCommand },
                Item = new ContextMenuCommand[] { edit, delete, copy, addItem, addFolder, refreshCommand },
                None = new ContextMenuCommand[] { addItem, addFolder, refreshCommand }
            },
            libraryCommands = new ContextMenuCommands()
            {
                List = new ContextMenuCommand[] { upload, addFolder, refreshCommand },
                Folder = new ContextMenuCommand[] { edit, delete, upload, addFolder, refreshCommand },
                Item = new ContextMenuCommand[] { open, edit, delete, copy, upload, addFolder, refreshCommand },
                None = new ContextMenuCommand[] { upload, addFolder, refreshCommand }
            };

        readonly static ContextMenuCommand[] rootsCommands = new ContextMenuCommand[]
        {
            new ContextMenuCommand() { EditMode = EditMode.AddItem, Name = "Add root...", Image = "AddLink" },
            refreshCommand
        };

        readonly static ContextMenuCommand[] rootCommands = new ContextMenuCommand[] { delete, refreshCommand };

        internal static ContextMenu RefeshContextMenu(Action refresh)
        {
            return new ContextMenu()
            {
                List = new ObservableList<string>() { null },
                Execute = delegate(IHierarchyLevel menuLevel, long menuIndex)
                {
                    refresh();
                },
                Renderer = delegate(IMenuItem menuItem, IHierarchyLevel menuLevel, long menuIndex)
                {
                    menuItem.Text = "Refresh";
                    menuItem.ImageClass = "RefreshIcon";
                    menuItem.Enabled = true;
                }
            };
        }

        internal static async void DeleteFolder(FolderNode folder)
        {
            if(await MessageBox.Show(string.Format("Are you sure to delete folder '{0}'", folder.Name),
                "Deleting...",
                MessageBoxButtons.YesNoCancel) == DialogResult.Yes) {
                await LazyWindow.ShowWaiting(folder.Item.Delete);
            }
        }

        internal static ContextMenu FolderContextMenu(FolderNode folder)
        {
            ContextMenuCommands commands = folder.ParentList.BaseType == BaseType.DocumentLibrary
                ? libraryCommands : listCommands;

            return new ContextMenu()
            {
                List = new ObservableList<ContextMenuCommand>(folder.IsRootFolder ? commands.List : commands.Folder),
                Execute = delegate(IHierarchyLevel menuLevel, long menuIndex)
                {
                    ContextMenuCommand command = (ContextMenuCommand)menuLevel.List[menuIndex];
                    switch (command.EditMode)
                    {
                        case EditMode.Refresh:
                            if (!folder.IsRootFolder)
                            {
                                folder.Refresh();
                            }
                            else
                            {
                                ((ListNode)folder.ParentList).Refresh();
                            }
                            break;

                        case EditMode.Edit:
                            ListItemEditor.Show(new EditAction() { EditMode = command.EditMode, ListItem = folder.Item, TargetList = folder.ParentList });
                            break;

                        case EditMode.AddItem:
                            ListItemEditor.Show(new EditAction() { EditMode = command.EditMode, TargetFolder = folder, TargetList = folder.ParentList });
                            break;

                        case EditMode.Upload:
                            Uploader.Upload(folder);
                            break;

                        case EditMode.AddFolder:
                            ListItemEditor.Show(new EditAction() { EditMode = command.EditMode, TargetFolder = folder, TargetList = folder.ParentList });
                            break;

                        default:
                            if(folder.IsRootFolder)
                            {
                                throw new NotImplementedException("List deletion is not implemented yet.");
                            }

                            DeleteFolder(folder);
                            break;
                    }
                },
                Renderer = delegate(IMenuItem menuItem, IHierarchyLevel menuLevel, long menuIndex)
                {
                    ContextMenuCommand command = (ContextMenuCommand)menuLevel.List[menuIndex];
                    switch (command.EditMode)
                    {
                        case EditMode.AddFolder:
                        case EditMode.AddItem:
                            menuItem.Enabled = folder.IsRootFolder || (folder.Item.EffectiveBasePermissions & BasePermissions.AddListItems) == BasePermissions.AddListItems;
                            break;

                        case EditMode.Delete:
                            menuItem.Enabled = folder.IsRootFolder || (folder.Item.EffectiveBasePermissions & BasePermissions.DeleteListItems) == BasePermissions.DeleteListItems;
                            break;

                        case EditMode.Edit:
                            menuItem.Enabled = !folder.IsRootFolder || BasePermissions.EditListItems == (BasePermissions.EditListItems & folder.Item.EffectiveBasePermissions);
                            break;

                        case EditMode.Refresh:
                            menuItem.Enabled = true;
                            break;
                    }


                    menuItem.Text = command.Name;
                    menuItem.ImageClass = command.Image + "Icon";
                    menuItem.SeparatorBefore = (command.EditMode == EditMode.Refresh || command.EditMode == EditMode.Delete || command.EditMode == EditMode.AddItem);
                }
            };
        }

        internal static ContextMenu FolderItemsContextMenu(FolderItems folderItems, ListItem listItem, Action delete)
        {
            ContextMenuCommands commands = folderItems.ParentFolder.ParentList.BaseType == BaseType.DocumentLibrary
                ? libraryCommands : listCommands;

            ContextMenuCommand[] menuItems = commands.None;
            if(listItem != null)
            {
                menuItems = listItem.FileSystemObjectType == FileSystemObjectType.Folder ? commands.Folder : commands.Item;
            }

            return new ContextMenu()
            {
                List = new ObservableList<ContextMenuCommand>(menuItems),
                Execute = delegate(IHierarchyLevel menuLevel, long menuIndex)
                {
                    ContextMenuCommand command = (ContextMenuCommand)menuLevel.List[menuIndex];
                    switch (command.EditMode)
                    {
                        case EditMode.Refresh:
                            folderItems.Refresh();
                            break;

                        case EditMode.Edit:
                            ListItemEditor.Show(new EditAction() { EditMode = command.EditMode, ListItem = listItem, TargetList = listItem.ParentList });
                            break;

                        case EditMode.Open:
                            HtmlWindow.Current.Open(listItem.AbsoluteUrl);
                            break;

                        case EditMode.Upload:
                            Uploader.Upload(folderItems.ParentFolder);
                            break;

                        case EditMode.AddItem:
                        case EditMode.AddFolder:
                            ListItemEditor.Show(new EditAction() { EditMode = command.EditMode,
                                TargetFolder = folderItems.ParentFolder,
                                TargetList = folderItems.ParentFolder.ParentList });
                            break;


                        case EditMode.Copy:
                            ListItemEditor.Show(new EditAction() { EditMode = command.EditMode,
                                ListItem = listItem,
                                TargetFolder = folderItems.ParentFolder,
                                TargetList = folderItems.ParentFolder.ParentList
                            });
                            break;

                        default:
                            delete();
                            break;
                    }
                },
                Renderer = delegate(IMenuItem menuItem, IHierarchyLevel menuLevel, long menuIndex)
                {
                    ContextMenuCommand command = (ContextMenuCommand)menuLevel.List[menuIndex];
                    menuItem.Text = command.Name;
                    menuItem.ImageClass = command.Image + "Icon";
                    menuItem.SeparatorBefore = (command.EditMode == EditMode.Refresh || command.EditMode == EditMode.Delete || command.EditMode == EditMode.Copy);
                    switch (command.EditMode)
                    {
                        case EditMode.AddFolder:
                            menuItem.Enabled = folderItems.ParentFolder.IsRootFolder 
                                || (folderItems.ParentFolder.Item.EffectiveBasePermissions & BasePermissions.AddListItems) 
                                    == BasePermissions.AddListItems;
                            break;

                        case EditMode.AddItem:
                        case EditMode.Copy:
                            menuItem.Enabled = folderItems.ParentFolder.ParentList.BaseType != BaseType.DocumentLibrary
                                && (folderItems.ParentFolder.IsRootFolder 
                                    || (folderItems.ParentFolder.Item.EffectiveBasePermissions & BasePermissions.AddListItems) 
                                        == BasePermissions.AddListItems);
                            break;

                        case EditMode.Delete:
                            menuItem.Enabled = (listItem.EffectiveBasePermissions & BasePermissions.DeleteListItems) == BasePermissions.DeleteListItems;
                            break;

                        case EditMode.Edit:
                            menuItem.Enabled = BasePermissions.EditListItems == (BasePermissions.EditListItems & listItem.EffectiveBasePermissions);
                            break;

                        case EditMode.Refresh:
                            menuItem.Enabled = folderItems.CanRefresh;
                            break;
                    }

                }
            };
        }

        internal static ContextMenu RootsContextMenu(Action addRoot, Action refresh)
        {
            return AnyAndRefreshContextMenu(rootsCommands, addRoot, refresh);
        }

        internal static ContextMenu RootContextMenu(Action deleteRoot, Action refresh)
        {
            return AnyAndRefreshContextMenu(rootCommands, deleteRoot, refresh);
        }

        static ContextMenu AnyAndRefreshContextMenu(ContextMenuCommand[] commands, Action any, Action refresh)
        {
            return new ContextMenu()
            {
                List = new ObservableList<ContextMenuCommand>(commands),
                Renderer = delegate(IMenuItem menuItem, IHierarchyLevel menuLevel, long menuIndex)
                {
                    ContextMenuCommand command = (ContextMenuCommand)menuLevel.List[menuIndex];
                    menuItem.Text = command.Name;
                    menuItem.ImageClass = command.Image + "Icon";
                    if (command == refreshCommand)
                    {
                        menuItem.SeparatorBefore = true;
                    }
                },

                Execute = delegate(IHierarchyLevel menuLevel, long menuIndex)
                {
                    ContextMenuCommand command = (ContextMenuCommand)menuLevel.List[menuIndex];
                    if (command == refreshCommand)
                    {
                        refresh();
                    }
                    else
                    {
                        any();
                    }
                }
            };
        }


        class ContextMenuCommand
        {
            internal string Name;
            internal EditMode EditMode;
            internal string Image;
        }

        class ContextMenuCommands
        {
            internal ContextMenuCommand[] List;
            internal ContextMenuCommand[] Folder;
            internal ContextMenuCommand[] Item;
            internal ContextMenuCommand[] None;
        }
    }
}
