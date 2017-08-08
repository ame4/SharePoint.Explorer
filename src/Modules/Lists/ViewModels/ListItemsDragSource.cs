using James.SharePoint;
using JScriptSuite.Html;
using JScriptSuite.JScriptLib.UI.Controls.Grids;
using JScriptSuite.JScriptLib.UI.DragDrops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class ListItemsDragSource : IDragSource
    {
        readonly IList<ListItem> listItems;
        readonly DropEffects dropEffects;

        internal ListItemsDragSource(IList<ListItem> listItems)
        {
            this.listItems = listItems;
            dropEffects = DropEffects.Copy | DropEffects.Link;
            int i = 0;
            for (; i < listItems.Count; i++)
            {
                ListItem listItem = listItems[i];
                if ((listItem.EffectiveBasePermissions & BasePermissions.DeleteListItems) != BasePermissions.DeleteListItems
                    || (listItem.EffectiveBasePermissions & BasePermissions.EditListItems) != BasePermissions.EditListItems)
                {
                    break;
                }
            }

            if (i == listItems.Count)
            {
                dropEffects |= DropEffects.Move;
            }
        }

        DropEffects IDragSource.DropEffects
        {
            get { return dropEffects; }
        }

        object IDragSource.DataObject
        {
            get { return listItems; }
        }

        void IDragSource.GiveFeedback(HtmlElement outer)
        {
            outer.InnerText = listItems.Count > 1 ? listItems.Count + " list items" : "1 list item";
        }

        void IDragSource.Complete(DropEffects dropEffects)
        {
        }

        void IDisposable.Dispose()
        {
        }
    }
}
