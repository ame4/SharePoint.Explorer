using JScriptSuite.Html;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.UI.Components;
using JScriptSuite.JScriptLib.UI.Controls.Trees;
using JScriptSuite.JScriptLib.UI.DragDrops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Nodes
{
    class WebNodeDragTarget : IDropTarget
    {
        readonly DataTreeView treeView;
        readonly WebNode webNode;
        bool canMove;
        IDropFeedback feedback;
        TreeViewHit hit;
        int clientY;


        internal WebNodeDragTarget(DataTreeView treeView, WebNode webNode)
        {
            this.treeView = treeView;
            this.webNode = webNode;
            canMove = webNode.RootNode != null && !webNode.RootNode.IsCurrent;
        }


        DropEffects IDropTarget.DragOver(HtmlMouseEvent e)
        {
            hit = treeView.Hit(e);
            clientY = e.ClientY;
            if(hit != null)
            {
                RootNode target = hit.Nodes.List[hit.Index] as RootNode;
                if (target == null) return DropEffects.None;
                if (target == webNode.RootNode) return DropEffects.Copy;
                if (2 * (clientY - hit.Rect.Top) < hit.Rect.Height)
                {
                    if (hit.Index > 0 && hit.Nodes.List[hit.Index - 1] == webNode) return DropEffects.Copy;
                }
                else if(hit.Index < hit.Nodes.List.Count - 1 && hit.Nodes.List[hit.Index + 1] == webNode)
                {
                    return DropEffects.Copy;
                }
            }

            return canMove ? DropEffects.Move | DropEffects.Copy : DropEffects.Copy;
        }

        void IDropTarget.Drop(HtmlMouseEvent e, DropEffects dropEffect)
        {
            RootNodes rootNodes = (RootNodes)treeView.NodeSource;
            RootNode rootNode = webNode.RootNode;
            long index = hit != null ? hit.Index : -1;
            if (index >= 0 && 2 * (clientY - hit.Rect.Top) >= hit.Rect.Height)
            {
                index++;
            }

            if(dropEffect == DropEffects.Move)
            {
                int pos = 0;
                while(rootNodes[pos] != rootNode)
                {
                    pos++;
                }

                rootNodes.RemoveRootNode(pos);
                if(index > pos) index--;
            }
            else
            {
                rootNode = new RootNode() { Url = webNode.Url, Title = webNode.Title, IsSite = rootNode != null && rootNode.IsSite, RootNodes = rootNodes  };
            }

            if (index >= 0)
            {
                rootNodes.InsertRootNode(index, rootNode);
            }
            else
            {
                rootNodes.Configuration.Roots.Add(rootNode);
            }
        }

        public void HideFeedback()
        {
            if (feedback != null)
            {
                feedback.Hide();
            }
        }

        void IDropTarget.ShowFeedback()
        {
            if(feedback == null)
            {
                feedback = DragDropUtility.DropHorizontalFeedback;
            }

            HtmlRect outer = treeView.Element.GetBoundingClientRect();
            Rect rc = null;
            if (hit != null)
            {
                rc = hit.Rect.Intersect(outer);
                if (2 * (clientY - hit.Rect.Top) >= hit.Rect.Height)
                {
                    rc.Top = rc.Bottom;
                }
            }
            else
            {
                rc = new Rect() { Left = outer.Left, Top = outer.Bottom - 1, Width = outer.Right - outer.Top, Height = 1 };
                if (outer.Height / treeView.RowHeight < treeView.NodeSource.Count)
                {
                    rc.Top = treeView.RowHeight * (int)treeView.NodeSource.Count;
                }
            }

            feedback.Show(rc);
        }

        void IDisposable.Dispose()
        {
            feedback = feedback.EnsureDispose();
        }
    }
}
