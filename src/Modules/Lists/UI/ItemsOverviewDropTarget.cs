using SharePoint.Explorer.Modules.Common;
using James.SharePoint;
using JScriptSuite.Html;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.UI.Components;
using JScriptSuite.JScriptLib.UI.Controls.Grids;
using JScriptSuite.JScriptLib.UI.Controls.Grids.DragDrops;
using JScriptSuite.JScriptLib.UI.Controls.Lazy;
using JScriptSuite.JScriptLib.UI.DragDrops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Lists.UI
{
    class ItemsOverviewDropTarget : GridRowDropTarget, IDropTarget
    {
        readonly ItemsPane itemsPane;
        readonly IList<ListItem> source;
        readonly DropEffects dropEffects;
        Folder targetFolder;
        
        internal ItemsOverviewDropTarget(ItemsPane itemsPane, IList<ListItem> source) : base(itemsPane.Overview)
        {
            this.itemsPane = itemsPane;
            this.source = source;
            dropEffects = ListItemsDropUtility.DropEffects(source, itemsPane.Data.FolderItems.ParentFolder);
        }

        DropEffects IDropTarget.DragOver(HtmlMouseEvent e)
        {
            base.DragOver(e);
            ListItem listItem = HitItem as ListItem;
            targetFolder = listItem != null ? listItem.Folder : null;
            return targetFolder != null ? ListItemsDropUtility.DropEffects(source, targetFolder) : dropEffects;
        }

        void IDropTarget.Drop(HtmlMouseEvent e, DropEffects dropEffect)
        {
            try
            {
                base.Drop(e);
                ListItemsDropUtility.Drop(source, targetFolder != null ? targetFolder : itemsPane.Data.FolderItems.ParentFolder);
            }
            catch(Exception ex)
            {
                LazyWindow.ShowError(ex);
            }
        }

        void IDropTarget.ShowFeedback()
        {
            ShowFeedback(targetFolder != null ? FeedbackMode.Over : FeedbackMode.After);
        }
    }
}
