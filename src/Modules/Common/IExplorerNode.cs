using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JScriptSuite.JScriptLib.DataBinding;
using JScriptSuite.JScriptLib.UI.Controls.Menus;
using JScriptSuite.JScriptLib.UI.Controls.Trees;
using JScriptSuite.JScriptLib.DataBinding.Providers;

namespace SharePoint.Explorer.Modules.Common
{
    interface IExplorerNode : ITreeNode, IHierarchyItem
    {
        ContextMenu ContextMenu(IHierarchyLevel level, long index);
        DeepLink DeepLink { get; }
        NavigateDisposition Navigate(DeepLink deepLink);
    }
}
