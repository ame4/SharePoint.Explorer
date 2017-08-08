using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePoint.Explorer.Modules.Common;
using James.SharePoint;
using spui = James.SharePoint.UI;
using JScriptSuite.Html;
using JScriptSuite.InteropServices;
using JScriptSuite.JScriptLib.DataBinding;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.DataBinding.Synchronizers;
using JScriptSuite.JScriptLib.UI.Controls.Grids;
using JScriptSuite.JScriptLib.UI.Controls.Splitters;
using SharePoint.Explorer.Modules.Nodes;
using SharePoint.Explorer.Modules.Lists.ViewModels;
using JScriptSuite.JScriptLib.UI.Controls.DataBinding;
using JScriptSuite.JScriptLib.UI.Controls.Lazy;
using JScriptSuite.JScriptLib.UI.Controls.Grids.ColumnHeaders;

namespace SharePoint.Explorer.Modules.Lists.UI
{
    class ViewsPane : UserLazyPane<ViewsViewModel>
    {
        internal readonly Grid<View> Overview;
        internal readonly Grid<Field> Fields;

        internal ViewsPane()
        {
            Data = new ViewsViewModel();
            Overview = new Grid<View>() { FixedLeftColumns = 2 };
            new GridClientSorter<View, Guid>(Overview, Data.OverviewListView);

            Overview.Columns.Add(new ImageColumn<View>()
            {
                Width = 40,
                ClassName = GridSkin.Instance.ImageGridClass,
                SortMode = GridColumnSortMode.Allow,
                Accessor = delegate(View data) { return data.Image; }
            });

            Overview.Columns.Add(new BoolColumn<View>()
            {
                Header = CreateColumnHeader("Default"),
                Width = 80,
                Accessor = delegate(View data) { return data.DefaultView; },
                SortMode = GridColumnSortMode.Allow,
                ClassName = GridSkin.Instance.CheckImageGridClass
            });

            Overview.Columns.Add(new TextColumn<View>()
            {
                Header = CreateColumnHeader("Title"),
                Width = 150,
                AllowResize = true,
                SortMode = GridColumnSortMode.Allow,
                Accessor = delegate(View data) { return data.Title; }
            });

            Overview.Columns.Add(new TextColumn<View>()
            {
                Header = CreateColumnHeader("ID"),
                Width = 200,
                AllowResize = true,
                SortMode = GridColumnSortMode.Allow,
                Accessor = delegate(View data) { return data.ID.ToString(); }
            });

            Overview.SortColumns.SortMode = GridSortMode.MultipleColumn;

            Fields = new Grid<Field>();
            Fields.Columns.Add(new TextColumn<string>()
            {
                Header = CreateColumnHeader("Field"),
                Width = 150,
                AllowResize = true,
                Accessor = delegate(string text) { return text; }
            });           
        }

        static ColumnHeader CreateColumnHeader(string title)
        {
            return new SortableTextColumnHeader() { Text = title };
        }

    }
}
