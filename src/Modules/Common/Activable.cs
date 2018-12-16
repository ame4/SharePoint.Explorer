using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.DataBinding.Providers.DependencyObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Common
{
    class Activable : DependencyObject
    {
        readonly static DependencyProperty<bool> isActive = DependencyProperty<bool>.Register(typeof(Activable));
        internal bool IsActive
        {
            get
            {
                return GetValue(isActive);
            }

            set
            {
                SetValue(isActive, value);
            }
        }
    }
}
