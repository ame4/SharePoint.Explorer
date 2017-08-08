using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePoint.Explorer.Modules.Common;
using JScriptSuite.JScriptLib.DataBinding;
using JScriptSuite.JScriptLib.DataBinding.Providers;

namespace SharePoint.Explorer.Modules.Nodes
{
    class ExplorerNodeList : ReferenceObservableList, IObservableHierarchyList
    {
        public ExplorerNodeList()
        {
        }


        IObservableList IObservableHierarchyList.this[long index]
        {
            get {
                IExplorerNode explorerNode = base[(int)index] as IExplorerNode;
                IObservableList list = explorerNode != null ? explorerNode.Children : null;
                return list != null && list.Count != 0 ? list : null;
            }
        }
    }
}
