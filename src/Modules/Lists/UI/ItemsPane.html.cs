using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePoint.Explorer.Modules.Common;
using SharePoint.Explorer.Modules.Nodes;
using James.SharePoint;
using James.SharePoint.UI;
using spui = James.SharePoint.UI;
using JScriptSuite.DataBinding;
using JScriptSuite.Html;
using JScriptSuite.InteropServices;
using JScriptSuite.JScriptLib.Common;
using JScriptSuite.JScriptLib.DataBinding;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.DataBinding.Synchronizers;
using JScriptSuite.JScriptLib.UI.Controls;
using JScriptSuite.JScriptLib.UI.Controls.GridLayout;
using JScriptSuite.JScriptLib.UI.Controls.Grids;
using JScriptSuite.JScriptLib.UI.Controls.Lazy;
using JScriptSuite.JScriptLib.UI.Controls.Menus;
using JScriptSuite.JScriptLib.UI.Controls.Tabs;
using JScriptSuite.JScriptLib.UI.Controls.Windows;
using ui = SharePoint.Explorer.Modules.Lists.UI;
using JScriptSuite.JScriptLib.UI.DragDrops;
using JScriptSuite.JScriptLib.UI.Controls.Grids.Filters;
using SharePoint.Explorer.Modules.Lists.ViewModels;
using JScriptSuite.JScriptLib.UI.Controls.DataBinding;
using JScriptSuite.JScriptLib.UI.Controls.Grids.ColumnHeaders;
using JScriptSuite.Html5;
using System.Threading;
using JScriptSuite.JScriptLib.UI.Components.Selections;
using JScriptSuite.JScriptLib.DataBinding.Providers.DependencyObjects;

namespace SharePoint.Explorer.Modules.Lists.UI
{
    class ItemsPane : UserLazyPane<ItemsView>
    {
        internal readonly Grid<ListItem> Overview;
        internal readonly Grid<FieldValue> DetailGrid;
        internal readonly Grid PermissionsGrid;
        GridClientFilterer<ListItem> overviewFilterer;
        IEnumerable<Field> fields;

        internal ItemsPane()
        {
            Data = new ItemsView();

            Overview = new Grid<ListItem>();

            Overview.FilterColumns.AllowFilter = true;

            Overview.SortColumns.SortMode = GridSortMode.MultipleColumn;
            Overview.RowsSelection.SelectionMode = SelectionMode.Multiple;

            new GridClientSorter<ListItem, int>(Overview, Data.OverviewListView);

            DetailGrid = new Grid<FieldValue>();

            new GridClientSorter<FieldValue, Guid>(DetailGrid, Data.DetailListView);

            DetailGrid.Columns.Add(new TextColumn<FieldValue>()
            {
                Width = 100,
                AllowResize = true,
                Accessor = delegate(FieldValue data) { 
                    return data.Field.Title; 
                },
                SortMode = GridColumnSortMode.Allow,
                Header = new SortableTextColumnHeader() { Text = "Field" }
            });

            DetailGrid.Columns.Add(new GridColumn<FieldValue>()
            {
                Width = 100,
                AllowResize = true,
                Header = new SortableTextColumnHeader() { Text = "Value" },
                Renderer = delegate(HtmlElement element, FieldValue data, object state)
                {
                    HtmlElement child = (HtmlElement)element.FirstChild;
                    if(child == null)
                    {
                        element.InnerHtml = "<div style='height:100%;overflow:hidden'><div style='height:100%;overflow:hidden'></div></div>";
                        child = (HtmlElement)element.FirstChild;
                    }

                    child.ClassName = data.Column.ClassName;
                    child = (HtmlElement)child.FirstChild;
                    return data.Column.Renderer(child, data.ListItem, state);
                }
            });

            DetailGrid.SortColumns.SortMode = GridSortMode.MultipleColumn;
            DetailGrid.FixedLeftColumns = 1;

            PermissionsGrid = new Grid<PermissionValue>();
            PermissionsGrid.SortColumns.SortMode = GridSortMode.MultipleColumn;
            PermissionsGrid.Columns.Add(new TextColumn<PermissionValue>()
            {
                Width = 100,
                AllowResize = true,
                Accessor = delegate(PermissionValue data) { return data.Key; },
                Header = new SortableTextColumnHeader() { Text = "Permission" }
            });

            PermissionsGrid.Columns.Add(new BoolColumn<PermissionValue>()
            {
                Width = 80,
                Accessor = delegate(PermissionValue data) { return data.Value; },
                ClassName = GridSkin.Instance.CheckImageGridClass
            });

            PermissionsGrid.FixedLeftColumns = 1;

            new GridClientSorter<PermissionValue, string>(PermissionsGrid, Data.PermissionsListView);

            Overview.Body.AdviseContextMenu(Overview_ContextMenu);
            Overview.Body.AdviseDblClick(delegate(long row, int column)
            {
                if (row >= 0 && column >= 0)
                {
                    Data.EditItem();
                }
            });

            Overview.Body.AdviseDrag(CreateDragSource);
            Overview.Body.AdviseDrop(CreateDropTarget);
            if(SupportedHtml5Features.File)
            {
                new ItemsOverviewFileDropTarget(this);
            }

            Overview.FilterColumns.AllowFilter = true;
            Overview.AdviseKeyDown(
                delegate(HtmlKeyboardEvent e)
                {
                    if(e.KeyCode == KeyCode.Delete)
                    {
                        DeleteSelectedItems();
                    }
                });
        }

        IDragSource CreateDragSource(HtmlMouseEvent e)
        {
            GridBodyHit hit = Overview.Body.Hit(e);
            if (hit.Row < 0)
            {
                return null;
            }

            ListItem[] listItems;

            if (Overview.RowsSelection.Contains(hit.Row))
            {
                listItems = new ListItem[Overview.RowsSelection.Count];
                int index = 0;
                Overview.RowsSelection.Enumerate(0, delegate(long row)
                {
                    listItems[index++] = (ListItem)Overview.DataSource[row];
                    return true;
                });
            }
            else
            {
                listItems = new ListItem[] { (ListItem)Overview.DataSource[hit.Row] };
            }

            return new ListItemsDragSource(listItems);
        }

        IDropTarget CreateDropTarget(object dataObject)
        {
            IList<ListItem> source = dataObject as IList<ListItem>;
            return source != null && source.Count != 0 && Overview.DataSource != null ? new ItemsOverviewDropTarget(this, source) : null;
        }

        async void DeleteSelectedItems()
        {
            if (Overview.RowsSelection.Count != 0)
            {
                JArray<ListItem> listItems = new JArray<ListItem>();
                Overview.RowsSelection.Enumerate(0,
                    delegate(ListItem li)
                    {
                        if ((li.EffectiveBasePermissions & BasePermissions.DeleteListItems) == BasePermissions.DeleteListItems)
                        {
                            listItems.Push(li);
                        }

                        return true;
                    });

                if (listItems.Length != 0)
                {
                    if(await MessageBox.Show(string.Format("Are you sure to delete {0} selected item(s)", listItems.Length),
                        "Deleting...",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        await LazyWindow.ShowWaiting(
                            delegate (CancellationToken cancellationToken) {
                                return ListItem.Delete(listItems, cancellationToken);
                            });
                    }
                }
                else
                {
                    await MessageBox.Show("No list items can be deleted.");
                }
            }
            else
            {
                await MessageBox.Show("No list items has been selected to delete.");
            }
        }

        ContextMenu Overview_ContextMenu(long rowIndex, int colIndex)
        {
            ListItem listItem = rowIndex >= 0 && rowIndex < Overview.DataSource.Count ? (ListItem)Overview.DataSource[rowIndex] : null;
            return NodeUtil.FolderItemsContextMenu(Data.FolderItems, 
                listItem,
                delegate()
                {
                    if (listItem != null && !Overview.RowsSelection.Contains(rowIndex))
                    {
                        Data.DeleteItem(listItem);
                    }
                    else
                    {
                        DeleteSelectedItems();
                    }
                });
        }

        internal IEnumerable<Field> Fields
        {
            get
            {
                return fields;
            }

            set
            {
                if(fields != value)
                {
                    fields = value;
                    overviewFilterer = overviewFilterer.EnsureDispose();
                    Overview.Columns.Clear();
                    if (value != null)
                    {
                        overviewFilterer = new GridClientFilterer<ListItem>(Overview, Data.OverviewListView);
                        foreach (Field field in value)
                        {
                            GridColumn column = UIUtility.CreateGridColumn(field);
                            if (column is IClientSortableColumn)
                            {
                                column.SortMode = GridColumnSortMode.Allow;
                            }

                            Overview.Columns.Add(column);
                            IFilterEditor filterEditor = UIUtility.CreateGridColumnFilterEditor(field);
                            if (filterEditor != null)
                            {
                                overviewFilterer.AddFilterColumn(column, filterEditor);
                            }
                        }
                    }
                }
            }
        }
    }
}
