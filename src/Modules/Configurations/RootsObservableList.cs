using JScriptSuite.JScriptLib.DataBinding.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.Explorer.Modules.Configurations
{
    [CollectionDataContractAttribute(ItemName = "Root")]
    public class RootsObservableList<TRootNode> : ObservableList<TRootNode>
    {
    }
}
