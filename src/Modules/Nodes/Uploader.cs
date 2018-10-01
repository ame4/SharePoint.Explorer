using SharePoint.Explorer.Modules.Lists.ViewModels;
using James.SharePoint;
using JScriptSuite.Html;
using JScriptSuite.Html5;
using JScriptSuite.Html5.IO;
using JScriptSuite.JScriptLib.UI.Components;
using JScriptSuite.JScriptLib.UI.Controls.Lazy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SharePoint.Explorer.Modules.Nodes
{
    class Uploader
    {
        readonly static HtmlLazy<Uploader> instance = new HtmlLazy<Uploader>(delegate () { return new Uploader(); });

        readonly HtmlInputFileElement input;
        Folder folder;

        Uploader()
        {
            if (SupportedHtml5Features.File)
            {
                input = (HtmlInputFileElement)ElementParking.CreateElements("<input type='file' multiply='1'/>")[0];
                input.AdviseChange(Change);
            }
        }

        void Change()
        {
            Upload(folder, input.Files);
        }

        internal static void Upload(Folder folder)
        {
            if (instance.Value.input != null)
            {
                instance.Value.folder = folder;
                instance.Value.input.Click();
            }
        }

        internal static async void Upload(Folder folder, HtmlCollection<File> files)
        {
            if(files.Length == 1)
            {
                File file = files[0];
                ListItem listItem = null;
                if (await LazyWindow.ShowWaiting(string.Format("{0}: uploading...", file.Name),
                    async delegate(CancellationToken cancellationToken)
                    {
                        listItem = await folder.Upload(file, cancellationToken);
                    }))
                {
                    ListItemEditor.Show(new EditAction() { EditMode = EditMode.Edit,
                        ListItem = listItem,
                        TargetList = listItem.ParentList });
                }
            }
            else
            {
                for (int i = 0; i < files.Length; i++)
                {
                    if (!await LazyWindow.ShowWaiting(string.Format("{0}: uploading...", files[i].Name),
                        delegate (CancellationToken cancellation)
                        {
                            return folder.Upload(files[i], cancellation);
                        }))
                        break;
                }
            }
        }

        internal static async void Upload(ListItem listItem, HtmlCollection<File> files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                if (!await LazyWindow.ShowWaiting(string.Format("{0}: uploading...", files[i].Name),
                    delegate (CancellationToken cancellationToken)
                    {
                        return listItem.AddAttachment(UriUtility.GetFileName(files[i].Name), files[i], cancellationToken);
                    }))
                    break;
            }
        }
    }
}