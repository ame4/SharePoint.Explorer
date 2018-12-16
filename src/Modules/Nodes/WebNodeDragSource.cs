using JScriptSuite.Html;
using JScriptSuite.JScriptLib.UI.DragDrops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Nodes
{
    class WebNodeDragSource : IDragSource
    {
        readonly WebNode web;

        internal WebNodeDragSource(WebNode web)
        {
            this.web = web;
        }

        DropEffects IDragSource.DropEffects
        {
            get {
                return web.RootNode != null && !web.RootNode.IsCurrent ? DropEffects.Copy | DropEffects.Move : DropEffects.Copy;
            }
        }

        object IDragSource.DataObject
        {
            get { 
                return web; 
            }
        }

        void IDragSource.GiveFeedback(HtmlElement outer) {}

        void IDragSource.Complete(DropEffects dropEffect)
        {
            
        }

        void IDisposable.Dispose()
        {
        }
    }
}
