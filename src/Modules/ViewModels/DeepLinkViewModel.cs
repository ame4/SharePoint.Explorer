using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JScriptSuite.JScriptLib.DataBinding.Providers;
using JScriptSuite.Logging;
using JScriptSuite.Html;
using JScriptSuite.JScriptLib.Serializing;
using SharePoint.Explorer.Modules.Common;
using JScriptSuite.JScriptLib.Html;

namespace SharePoint.Explorer.ViewModels
{
    class DeepLinkViewModel
    {
        readonly string localStorageKey;
        readonly Action<DeepLink> navigate;

        DeepLink deepLink;
        DeepLink lastReceived;

        IDisposable disposable;

        internal DeepLinkViewModel(Action<DeepLink> navigate)
        {
            this.navigate = navigate;
            HtmlLocation location = HtmlWindow.Current.Location;
            string hash = location.Hash;
            localStorageKey = UriUtility.GetServerRelativeUrl(UriUtility.GetAbsolutePath(location.Href)) + "?deepLink";
            if(!string.IsNullOrEmpty(hash))
            {
                try
                {
                    lastReceived = JsonDeserializer.Deserialize<DeepLink>(hash.Substring(1)); // ignore #
                }
                catch
                {

                }
            }

            if (lastReceived == null && HtmlWindow.Current.LocalStorage != null)
            {
                hash = HtmlWindow.Current.LocalStorage[localStorageKey];
                if (!string.IsNullOrEmpty(hash))
                {
                    try
                    {
                        lastReceived = JsonDeserializer.Deserialize<DeepLink>(hash); // ignore #
                    }
                    catch
                    {

                    }
                }
            }

            HtmlWindow.Current.AdviseHash(HashChanged);
        }

        void HashChanged()
        {
            string hash = HtmlWindow.Current.Location.Hash;
            if (!string.IsNullOrEmpty(hash) && hash.Length > 1)
            {
                DeepLink deepLink;
                try
                {
                    deepLink = JsonDeserializer.Deserialize<DeepLink>(hash.Substring(1));
                }
                catch
                {
                    return;
                }

                if (deepLink != null && !this.deepLink.Equals(deepLink))
                {
                    navigate(deepLink);
                }
            }
        }

        internal DeepLink DeepLink
        {
            get
            {
                return lastReceived;
            }

            set
            {
                if(lastReceived != value)
                {
                    disposable = disposable.EnsureDispose();
                    lastReceived = value;
                    if(value != null)
                    {
                        disposable = lastReceived.Advise(Apply);
                    }
                }

                if (!object.Equals(deepLink, value))
                {
                    Apply();
                }
            }
        }

        void Apply()
        {
            string hash = null;
            deepLink = null;
            if(lastReceived != null)
            {
                deepLink = new DeepLink(lastReceived);
                hash = JsonSerializer.Serialize<DeepLink>(deepLink);
            }

            HtmlWindow.Current.Location.Hash = hash;
            if (HtmlWindow.Current.LocalStorage != null)
            {
                HtmlWindow.Current.LocalStorage[localStorageKey] = hash;
            }
        }
    }
}
