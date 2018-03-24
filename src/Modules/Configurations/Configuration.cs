using James.SharePoint.WebParts;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharePoint.Explorer.Modules.Configurations
{
    public class Configuration<TRootNode> : Observable
    {
        readonly ObservableProperty<DisplayCurrentMode?> displayCurrentMode;

        public Configuration()
        {
            displayCurrentMode = new ObservableProperty<DisplayCurrentMode?>(this);
        }

        public ScreenConfiguration Screen;

        public RootsObservableList<TRootNode> Roots;

        public DisplayCurrentMode? DisplayCurrent
        {
            get
            {
                return displayCurrentMode.Value;
            }

            set
            {
                displayCurrentMode.Value = value;
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
