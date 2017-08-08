using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using James.SharePoint;
using JScriptSuite.JScriptLib.DataBinding.Providers;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class PermissionsDataSource : ObservableList<PermissionValue>
    {
        readonly BasePermissions permissions;

        internal PermissionsDataSource(BasePermissions permissions)
        {
            this.permissions = permissions;
            Add("AddAndCustomizePages", BasePermissions.AddAndCustomizePages);
            Add("AddDelPrivateWebParts", BasePermissions.AddDelPrivateWebParts);
            Add("AddListItems", BasePermissions.AddListItems);
            Add("ApplyStyleSheets", BasePermissions.ApplyStyleSheets);
            Add("ApplyThemeAndBorder", BasePermissions.ApplyThemeAndBorder);
            Add("ApproveItems", BasePermissions.ApproveItems);
            Add("BrowseDirectories", BasePermissions.BrowseDirectories);
            Add("BrowseUserInfo", BasePermissions.BrowseUserInfo);
            Add("CancelCheckout", BasePermissions.CancelCheckout);
            Add("CreateAlerts", BasePermissions.CreateAlerts);

            
            Add("CreateGroups", BasePermissions.CreateGroups);
            Add("CreateSSCSite", BasePermissions.CreateSSCSite);
            Add("DeleteListItems", BasePermissions.DeleteListItems);
            Add("DeleteVersions", BasePermissions.DeleteVersions);
            Add("EditListItems", BasePermissions.EditListItems);
            Add("EditMyUserInfo", BasePermissions.EditMyUserInfo);
            Add("EnumeratePermissions", BasePermissions.EnumeratePermissions);
            Add("FullMask", BasePermissions.FullMask);
            Add("ManageAlerts", BasePermissions.ManageAlerts);
            Add("ManageLists", BasePermissions.ManageLists);
            Add("ManagePermissions", BasePermissions.ManagePermissions);
            Add("ManagePersonalViews", BasePermissions.ManagePersonalViews);
            Add("ManageSubwebs", BasePermissions.ManageSubwebs);
            
            Add("ManageWeb", BasePermissions.ManageWeb);
            Add("Open", BasePermissions.Open);
            Add("OpenItems", BasePermissions.OpenItems);
            Add("UpdatePersonalWebParts", BasePermissions.UpdatePersonalWebParts);
            Add("UseClientIntegration", BasePermissions.UseClientIntegration);
            Add("UseRemoteAPIs", BasePermissions.UseRemoteAPIs);
            Add("ViewListItems", BasePermissions.ViewListItems);
            Add("ViewPages", BasePermissions.ViewPages);
            Add("ViewUsageData", BasePermissions.ViewUsageData);
            Add("ViewVersions", BasePermissions.ViewVersions);
        }

        void Add(string name, BasePermissions value)
        {
            Add(new PermissionValue() { Key = name, Value = (permissions & value) == value });
        }


        public override bool Equals(object obj)
        {
            PermissionsDataSource other = obj as PermissionsDataSource;
            return other != null && other.permissions == permissions;
        }
    }

    class PermissionValue
    {
        internal string Key;
        internal bool Value;
    }
}
