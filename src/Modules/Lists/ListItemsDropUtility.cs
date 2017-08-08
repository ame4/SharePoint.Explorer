using James.SharePoint;
using JScriptSuite.JScriptLib.UI.DragDrops;
using dragDrop = JScriptSuite.JScriptLib.UI.DragDrops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JScriptSuite.JScriptLib.Html;
using SharePoint.Explorer.Modules.Common;
using JScriptSuite.JScriptLib.UI.Controls.Lazy;
using JScriptSuite.Html;
using System.Threading;

namespace SharePoint.Explorer.Modules.Lists
{
    class ListItemsDropUtility
    {
        internal static void Drop(IList<ListItem> source, Folder target)
        {
            if (target != null && source != null && source.Count != 0)
            {
                for (int i = 0; i < source.Count; i++)
                {
                    source[i].MoveTo(target);
                }

                LazyWindow.ShowWaiting(
                    delegate(CancellationToken cancellationToken) 
                    {
                        return ListItem.Update(source, cancellationToken);
                    });
            }
        }

        internal static DropEffects DropEffects(IList<ListItem> source, Folder target)
        {
            if (target.Item != null && (target.Item.EffectiveBasePermissions & BasePermissions.AddListItems) != BasePermissions.AddListItems)
            {
                return dragDrop.DropEffects.None;
            }

            dragDrop.DropEffects dropEffects = dragDrop.DropEffects.Copy | dragDrop.DropEffects.Move;
            for (int i = 0; i < source.Count; i++)
            {
                dropEffects &= DropEffects(source[i], target);
                if (dropEffects == dragDrop.DropEffects.None)
                {
                    break;
                }
            }

            return dropEffects;
        }

        static DropEffects DropEffects(ListItem source, Folder target)
        {
            if (source.ParentList != target.ParentList)
            {
                return dragDrop.DropEffects.None;
            }


            if (UriUtility.GetDirectoryUrl(source.ServerRelativeUrl) == target.ServerRelativeUrl)
            {
                return source.FileSystemObjectType == FileSystemObjectType.Folder ? dragDrop.DropEffects.None : dragDrop.DropEffects.Copy;
            }

            if (source.ServerRelativeUrl == target.ServerRelativeUrl) return dragDrop.DropEffects.None;

            if (source.FileSystemObjectType == FileSystemObjectType.Folder)
            {
                return target.ServerRelativeUrl.StartsWith(source.ServerRelativeUrl + '/') ? dragDrop.DropEffects.None : dragDrop.DropEffects.Move;
            }

            if (!source.IsNew && source.ParentList.BaseType != BaseType.DocumentLibrary)
            {
                return dragDrop.DropEffects.Copy | dragDrop.DropEffects.Move;
            }

            return UriUtility.GetDirectoryUrl(source.ServerRelativeUrl) == target.ServerRelativeUrl
               ? dragDrop.DropEffects.Copy
               : dragDrop.DropEffects.Copy | dragDrop.DropEffects.Move;
        }
    }
}
