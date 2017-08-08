using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using James.SharePoint;
using spui = James.SharePoint.UI;
using JScriptSuite.Html;
using JScriptSuite.InteropServices;
using JScriptSuite.JScriptLib.DataBinding;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.DataBinding.Synchronizers;
using JScriptSuite.JScriptLib.UI.Controls.Grids;
using JScriptSuite.JScriptLib.UI.Controls.Splitters;
using JScriptSuite.JScriptLib.UI.Controls.Lazy;
using JScriptSuite.JScriptLib.UI.Controls.DataBinding;
using SharePoint.Explorer.Modules.Lists.ViewModels;
using JScriptSuite.JScriptLib.UI.Controls.Grids.ColumnHeaders;

namespace SharePoint.Explorer.Modules.Lists.UI
{
    class ContentTypesPane : UserLazyPane<ContentTypesViewModel>
    {
        internal readonly Grid<ListContentType> Overview;
        
        internal ContentTypesPane()
        {
            Data = new ContentTypesViewModel();
            Overview = new Grid<ListContentType>();
            Overview.FixedLeftColumns = 1;
            Overview.Columns.Add(new TextColumn<ListContentType>()
            {
                Header = new SortableTextColumnHeader() { Text = "Name" },
                Width = 150,
                AllowResize = true,
                SortMode = GridColumnSortMode.Allow,
                Accessor = delegate(ListContentType data) { return data.Name; }
            });
            Overview.Columns.Add(new TextColumn<ListContentType>()
            {
                Header = new SortableTextColumnHeader() { Text = "Description" },
                Width = 150,
                AllowResize = true,
                SortMode = GridColumnSortMode.Allow,
                Accessor = delegate(ListContentType data) { return data.Description; }
            });
            Overview.Columns.Add(new TextColumn<ListContentType>()
            {
                Header = new SortableTextColumnHeader() { Text = "ID" },
                Width = 150,
                AllowResize = true,
                SortMode = GridColumnSortMode.Allow,
                Accessor = delegate(ListContentType data) { return data.ID; }
            });

            new GridClientSorter<ListContentType, string>(Overview, Data.OverviewListView);

            Overview.SortColumns.SortMode = GridSortMode.MultipleColumn;

            Overview.Body.AdviseDblClick(delegate(long row, int column)
            {
                if (row >= 0 && column >= 0)
                {
                    Data.EditContentType(Overview.RowsSelection.Data);
                }
            });
        }
    }
}
