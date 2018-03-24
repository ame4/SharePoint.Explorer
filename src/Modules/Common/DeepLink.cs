using SharePoint.Explorer.Modules.Common;
using JScriptSuite.JScriptLib.DataBinding.Providers.DependencyObjects;
using JScriptSuite.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Common
{
    public class DeepLink : DependencyObject, IEquatable<DeepLink>
    {
        readonly static DependencyProperty<NodeType> nodeType = DependencyProperty<NodeType>.Register(typeof(DeepLink));
        public NodeType NodeType
        {
            get
            {
                return GetValue(nodeType);
            }

            set
            {
                SetValue(nodeType, value);
            }
        }



        readonly static DependencyProperty<string> webUrl = DependencyProperty<string>.Register(typeof(DeepLink));
        public string WebUrl
        {
            get
            {
                return GetValue(webUrl);
            }

            set
            {
                SetValue(webUrl, value);
            }
        }

        readonly static DependencyProperty<Guid?> listId = DependencyProperty<Guid?>.Register(typeof(DeepLink));
        public Guid? ListId
        {
            get
            {
                return GetValue(listId);
            }

            set
            {
                SetValue(listId, value);
            }
        }

        readonly static DependencyProperty<string> folderUrl = DependencyProperty<string>.Register(typeof(DeepLink));
        public string FolderUrl
        {
            get
            {
                return GetValue(folderUrl);
            }

            set
            {
                SetValue(folderUrl, value);
            }
        }


        readonly static DependencyProperty<int> tabItemIndex = DependencyProperty<int>.Register(typeof(DeepLink));
        public int TabItemIndex
        {
            get
            {
                return GetValue(tabItemIndex);
            }

            set
            {
                SetValue(tabItemIndex, value);
            }
        }

        readonly static DependencyProperty<string> contentTypeId = DependencyProperty<string>.Register(typeof(DeepLink));
        public string ContentTypeId
        {
            get
            {
                return GetValue(contentTypeId);
            }

            set
            {
                SetValue(contentTypeId, value);
            }
        }

        public DeepLink()
        {
        }

        internal DeepLink(DeepLink source)
        {
            NodeType = source.NodeType;
            WebUrl = source.WebUrl;
            ListId = source.ListId;
            FolderUrl = source.FolderUrl;
            TabItemIndex = source.TabItemIndex;
            ContentTypeId = source.ContentTypeId;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DeepLink);
        }

        public bool Equals(DeepLink other)
        {
            return other != null
                && NodeType == other.NodeType
                && WebUrl == other.WebUrl
                && ListId == other.ListId
                && FolderUrl == other.FolderUrl
                && TabItemIndex == other.TabItemIndex
                && ContentTypeId == other.ContentTypeId;
        }
    }

    public enum NodeType
    {
        Info,
        Web,
        Webs,
        Lists,
        List,
        Folder
    }

}
