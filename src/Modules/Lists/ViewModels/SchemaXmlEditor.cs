using SharePoint.Explorer.Modules.Lists.UI;
using JScriptSuite.DataBinding;
using JScriptSuite.Html;
using JScriptSuite.JScriptLib.Common;
using JScriptSuite.JScriptLib.DataBinding;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.Html;
using JScriptSuite.JScriptLib.UI.Components;
using JScriptSuite.JScriptLib.UI.Controls.DataWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharePoint.Explorer.Modules.Lists.ViewModels
{
    abstract class SchemaXmlEditor : Observable, ISavable
    {
        readonly static HtmlLazy<SchemaXmlEditorWindow> instance = new DefaultHtmlLazy<SchemaXmlEditorWindow>();

        internal readonly string Title;

        readonly ObservableProperty<string> schemaXml;
        
        string original;

        internal SchemaXmlEditor(string schemaXml, string title)
        {
            this.original = schemaXml;
            this.schemaXml = new ObservableProperty<string>(this);
            Title = title;
            SchemaXml = schemaXml;
        }

        internal string SchemaXml
        {
            get { return schemaXml.Value; }
            set { schemaXml.Value = value; }
        }

        protected virtual Task Save(CancellationToken cancellationToken)
        {
            return Tasks.Completed;
        }

        protected void Show()
        {
            instance.Value.Data = this;
            instance.Value.Show();
        }

        bool ISavable.IsDirty
        {
            get { 
                return false;
            }
        }


        async Task ISavable.Save(CancellationToken cancellationToken)
        {
            await Save(cancellationToken);
            original = SchemaXml;
        }

        public virtual void Close() { }
    }
}
