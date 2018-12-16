using James.SharePoint.WebParts;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.JScriptLib.DataBinding.Providers.DependencyObjects;
using JScriptSuite.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Configurations
{
    public class Configuration<TRootNode> : DependencyObject
    {
        public ScreenConfiguration Screen;

        public RootsObservableList<TRootNode> Roots;

        readonly static DependencyProperty<DisplayCurrentMode?> displayCurrent = DependencyProperty<DisplayCurrentMode?>.Register(typeof(Configuration<TRootNode>));
        public DisplayCurrentMode? DisplayCurrent
        {
            get
            {
                return GetValue(displayCurrent);
            }

            set
            {
                SetValue(displayCurrent, value);
            }
        }

        internal DisplayCurrentMode EffectiveDisplayCurrent
        {
            get
            {
                return DisplayCurrent ?? DisplayCurrentMode.Site;
            }

            set
            {
                DisplayCurrent = value;
            }
        }
    }

    public enum DisplayCurrentMode
    {
        None,
        Web,
        Site
    }
}
