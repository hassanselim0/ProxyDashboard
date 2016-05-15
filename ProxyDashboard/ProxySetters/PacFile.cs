using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProxyDashboard.ProxySetters
{
    /// <summary>
    /// Creates a PAC file that can be used for proxy auto-configuration.
    /// You'll need an add-on like "Reload PAC Button" to force the browser to read the new value.
    /// </summary>
    public class PacFile : IProxySetter
    {
        private const string PAC_TEMPLATE =
            "function FindProxyForURL(url, host) {{ return \"PROXY {0}\"; }}";

        public void SetProxy(string ip)
        {
            File.WriteAllText("proxy.pac", string.Format(PAC_TEMPLATE, ip));
        }
    }
}
