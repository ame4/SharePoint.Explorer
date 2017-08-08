using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharePoint.Explorer.Modules.Common;
using JScriptSuite.JScriptLib.DataBinding;
using JScriptSuite.JScriptLib.UI.Controls.Menus;
using JScriptSuite.JScriptLib.UI.Controls.Trees;

namespace SharePoint.Explorer.Modules.Nodes
{
    class InformationNode : IExplorerNode
    {
        readonly DeepLink deepLink;

        internal InformationNode ()
        {
            deepLink = new DeepLink() { NodeType = NodeType.Info };
        }

        string ITreeNode.Image
        {
            get { return "/_layouts/images/helpicon.gif"; }
        }

        string ITreeNode.Title
        {
            get { return "Info"; }
        }

        string ITreeNode.Description
        {
            get { return "Information about SharePoint Explorer"; }
        }

        NavigateDisposition IExplorerNode.Navigate(DeepLink deepLink)
        {
            return deepLink.NodeType == NodeType.Info ? NavigateDisposition.Complete : NavigateDisposition.Next;
        }

        ContextMenu IExplorerNode.ContextMenu(IHierarchyLevel level, long index)
        {
            return null;
        }

        public IObservableList Children
        {
            get
            {
                return null;
            }
        }


        DeepLink IExplorerNode.DeepLink
        {
            get
            {
                return deepLink;
            }
        }

    }
}
