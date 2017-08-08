using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Common
{
    interface ITreeNode
    {
        string Image { get; }
        string Title { get; }
        string Description { get; }
    }
}
