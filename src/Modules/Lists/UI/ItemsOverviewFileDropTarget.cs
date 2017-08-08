using JScriptSuite.JScriptLib.UI.Controls.Grids.DragDrops;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JScriptSuite.Html;
using James.SharePoint;
using JScriptSuite.JScriptLib.UI.DragDrops;
using JScriptSuite.Html5;
using JScriptSuite.Html5.IO;
using JScriptSuite.JScriptLib.UI.Controls.Lazy;
using SharePoint.Explorer.Modules.Nodes;

namespace SharePoint.Explorer.Modules.Lists.UI
{
    class ItemsOverviewFileDropTarget : GridRowDropTarget
    {
        IDisposable disposable;
        readonly ItemsPane itemsPane;

        internal ItemsOverviewFileDropTarget(ItemsPane itemsPane) : base(itemsPane.Overview)
        {
            this.itemsPane = itemsPane;
            disposable = itemsPane.Overview.Body.AdviseDrop(DragOver, Drop);
        }

        new void DragOver(HtmlMouseEvent e)
        {
            base.DragOver(e);
            if(e != null && itemsPane.Data?.FolderItems?.ParentFolder?.ParentList?.BaseType == BaseType.DocumentLibrary)
            {
                ListItem listItem = HitItem as ListItem;
                ShowFeedback(listItem != null && listItem.FileSystemObjectType == FileSystemObjectType.Folder 
                    ? FeedbackMode.Over : FeedbackMode.After);
            }
            else
            {
                HideFeedback();
            }
        }

        void Drop(HtmlDropEvent e)
        {
            base.Drop(e);
            if (e != null && itemsPane.Data?.FolderItems?.ParentFolder?.ParentList?.BaseType == BaseType.DocumentLibrary)
            {
                ListItem listItem = HitItem as ListItem;
                Folder folder = listItem != null && listItem.FileSystemObjectType == FileSystemObjectType.Folder
                    ? listItem.Folder : itemsPane.Data.FolderItems.ParentFolder;

                Uploader.Upload(folder, e.Files);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            disposable = disposable.EnsureDispose();
        }
    }
}
