using JScriptSuite.JScriptLib.DataBinding.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Common
{
    class Activable : Observable
    {
        readonly ObservableProperty<bool> isActive;

        internal Activable()
        {
            isActive = new ObservableProperty<bool>(this);
        }

        internal bool IsActive
        {
            get
            {
                return isActive.Value;
            }

            set
            {
                isActive.Value = value;
            }
        }
    }
}
