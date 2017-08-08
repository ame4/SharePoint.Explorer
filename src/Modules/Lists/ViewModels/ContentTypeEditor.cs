using James.SharePoint;
using JScriptSuite.DataBinding;
using JScriptSuite.Html;
using JScriptSuite.JScriptLib.DataBinding;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.DataBinding.Providers.Lazy;
using JScriptSuite.JScriptLib.UI.Components;
using JScriptSuite.JScriptLib.UI.Controls;
using JScriptSuite.JScriptLib.UI.Controls.GridLayout;
using JScriptSuite.JScriptLib.UI.Controls.Lazy;
using JScriptSuite.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    class ContentTypeEditor : SchemaXmlEditor
    {
        readonly ListContentType contentType;

        ContentTypeEditor(ListContentType contentType)
            : base(contentType.Detail.Value.SchemaXml, "View content type")
        {
            this.contentType = contentType;
        }

        internal static void Show(ListContentType contentType)
        {
            LazyWindow.ShowWaiting();
            contentType.Detail.Get(delegate(ContentTypeDetail detail)
            {
                LazyWindow.HideWaiting();
                new ContentTypeEditor(contentType).Show();
            },
            LazyWindow.ShowError);
        }
    }
}
