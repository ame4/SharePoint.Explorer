using JScriptSuite.Html;
using JScriptSuite.Html5;
using JScriptSuite.JScriptLib.UI.Controls.Trees;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.UI.Controls.Trees.DragDrops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using James.SharePoint;
using JScriptSuite.JScriptLib.UI.DragDrops;
using JScriptSuite.Html5.IO;
using JScriptSuite.JScriptLib.UI.Controls.Lazy;

namespace SharePoint.Explorer.Modules.Nodes
{
    class TreeViewFileDropTarget : TreeViewDropTarget
    {
        IDisposable disposable;
        internal TreeViewFileDropTarget(DataTreeView treeView) : base(treeView)
        {
            disposable = treeView.AdviseDrop(DragOver, Drop);
        }


        new void DragOver(HtmlMouseEvent e)
        {
            base.DragOver(e);
            ShowFeedback(Folder != null ? FeedbackMode.Over : FeedbackMode.None);
        }

        Folder Folder
        {
            get
            {
                object node = HitNode;
                Folder folder = node as Folder;
                List list = folder != null ? folder.ParentList : node as List;
                return list?.BaseType == BaseType.DocumentLibrary ? folder??list.RootFolder : null;
            }
        }


        void Drop(HtmlDropEvent e)
        {
            base.Drop(e);
            Folder folder = Folder;
            if(folder != null)
            {
                Uploader.Upload(folder, e.Files);
            }
        }


        public override void Dispose()
        {
            disposable = disposable.EnsureDispose();
            base.Dispose();
        }
    }
}
