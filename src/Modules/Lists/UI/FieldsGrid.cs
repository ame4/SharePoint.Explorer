using SharePoint.Explorer.Modules.Lists.ViewModels;
using James.SharePoint;
using JScriptSuite.JScriptLib.UI.Controls.Grids;
using JScriptSuite.JScriptLib.UI.Controls.Grids.ColumnHeaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using spui = James.SharePoint.UI;

namespace SharePoint.Explorer.Modules.Lists.UI
{
    class FieldsGrid : Grid<Field>
    {
        readonly FieldsEditor editor;

        internal FieldsGrid(FieldsEditor editor)
        {
            this.editor = editor;

            SortColumns.SortMode = GridSortMode.MultipleColumn;
            FixedLeftColumns = 1;
            Columns.Add(new TextColumn<Field>()
            {
                Header = CreateColumnHeader("Title"),
                Width = 100,
                AllowResize = true,
                SortMode = GridColumnSortMode.Allow,
                Accessor = delegate(Field field) { return field.Title; }
            });
            Columns.Add(new TextColumn<Field>()
            {
                Header = CreateColumnHeader("InternalName"),
                Width = 100,
                AllowResize = true,
                SortMode = GridColumnSortMode.Allow,
                Accessor = delegate(Field field) { return field.InternalName; }
            });
            Columns.Add(new TextColumn<Field>()
            {
                Header = CreateColumnHeader("Type"),
                Width = 100,
                AllowResize = true,
                SortMode = GridColumnSortMode.Allow,
                Accessor = delegate(Field field) { return field.TypeName; }
            });
            Columns.Add(new TextColumn<Field>()
            {
                Header = CreateColumnHeader("Group"),
                Width = 100,
                AllowResize = true,
                SortMode = GridColumnSortMode.Allow,
                Accessor = delegate(Field field) { return field.Group; }
            });
            Columns.Add(new TextColumn<Field>()
            {
                Header = CreateColumnHeader("ID"),
                Width = 100,
                AllowResize = true,
                SortMode = GridColumnSortMode.Allow,
                Accessor = delegate(Field field) { return field.ID.ToString(); }
            });
            Columns.Add(new BoolColumn<Field>()
            {
                Header = CreateColumnHeader("Required"),
                Width = 80,
                Accessor = delegate(Field field) { return field.Required; },
                SortMode = GridColumnSortMode.Allow,
                ClassName = GridSkin.Instance.CheckImageGridClass
            });
            Columns.Add(new BoolColumn<Field>()
            {
                Header = CreateColumnHeader("Sortable"),
                Width = 80,
                Accessor = delegate(Field field) { return field.Sortable; },
                SortMode = GridColumnSortMode.Allow,
                ClassName = GridSkin.Instance.CheckImageGridClass
            });
            Columns.Add(new BoolColumn<Field>()
            {
                Header = CreateColumnHeader("FromBaseType"),
                Width = 80,
                Accessor = delegate(Field field) { return field.FromBaseType; },
                SortMode = GridColumnSortMode.Allow,
                ClassName = GridSkin.Instance.CheckImageGridClass
            });
            Columns.Add(new BoolColumn<Field>()
            {
                Header = CreateColumnHeader("ReadOnly"),
                Width = 80,
                Accessor = delegate(Field field) { return field.ReadOnly; },
                SortMode = GridColumnSortMode.Allow,
                ClassName = GridSkin.Instance.CheckImageGridClass
            });
            Columns.Add(new BoolColumn<Field>()
            {
                Header = CreateColumnHeader("ShowInDisplayForm"),
                Width = 80,
                Accessor = delegate(Field field) { return field.ShowInDisplayForm; },
                SortMode = GridColumnSortMode.Allow,
                ClassName = GridSkin.Instance.CheckImageGridClass
            });
            Columns.Add(new BoolColumn<Field>()
            {
                Header = CreateColumnHeader("ShowInNewForm"),
                Width = 80,
                Accessor = delegate(Field field) { return field.ShowInNewForm; },
                SortMode = GridColumnSortMode.Allow,
                ClassName = GridSkin.Instance.CheckImageGridClass
            });
            Columns.Add(new BoolColumn<Field>()
            {
                Header = CreateColumnHeader("ShowInEditForm"),
                Width = 80,
                Accessor = delegate(Field field) { return field.ShowInEditForm; },
                SortMode = GridColumnSortMode.Allow,
                ClassName = GridSkin.Instance.CheckImageGridClass
            });
            Columns.Add(new BoolColumn<Field>()
            {
                Header = CreateColumnHeader("Hidden"),
                Width = 80,
                Accessor = delegate(Field field) { return field.Hidden; },
                SortMode = GridColumnSortMode.Allow,
                ClassName = GridSkin.Instance.CheckImageGridClass
            });

            Columns.Add(new BoolColumn<Field>()
            {
                Header = CreateColumnHeader("Sortable"),
                Width = 80,
                Accessor = delegate(Field field) { return field.Sortable; },
                SortMode = GridColumnSortMode.Allow,
                ClassName = GridSkin.Instance.CheckImageGridClass
            });

            Columns.Add(new BoolColumn<Field>()
            {
                Header = CreateColumnHeader("Filterable"),
                Width = 80,
                Accessor = delegate(Field field) { return field.Filterable; },
                SortMode = GridColumnSortMode.Allow,
                ClassName = GridSkin.Instance.CheckImageGridClass
            });


            new GridClientSorter<Field, string>(this, editor.FieldsListView);
            SortColumns.SortMode = GridSortMode.MultipleColumn;

            Body.AdviseDblClick(delegate(long row, int column)
            {
                if (row >= 0 && column >= 0)
                {
                    editor.EditField((Field)RowsSelection.Data);
                }
            });
        }

        static ColumnHeader CreateColumnHeader(string title)
        {
            return new SortableTextColumnHeader() { Text = title };
        }
    }
}
