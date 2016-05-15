using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyDashboard.ProxySetters
{
    /// <summary>
    /// Creates a user.js file to update Firefox's proxy prefences the next time it starts
    /// </summary>
    public class Firefox : IProxySetter
    {
        private const string PREFS_TEMPLATE =
            "user_pref(\"network.proxy.type\", 1);\r\n" +
            "user_pref(\"network.proxy.share_proxy_settings\", true);\r\n" +
            "user_pref(\"network.proxy.http\", \"{0}\");\r\n" +
            "user_pref(\"network.proxy.http_port\", {1});\r\n" +
            "user_pref(\"network.proxy.ssl\", \"{0}\");\r\n" +
            "user_pref(\"network.proxy.ssl_port\", {1});\r\n" +
            "user_pref(\"network.proxy.ftp\", \"{0}\");\r\n" +
            "user_pref(\"network.proxy.ftp_port\", {1});\r\n" +
            "user_pref(\"network.proxy.socks\", \"{0}\");\r\n" +
            "user_pref(\"network.proxy.socks_port\", {1});\r\n";

        private string prefsPath;

        public Firefox()
        {
            // Example Path:
            // C:\Users\Derp\AppData\Roaming\Mozilla\Firefox\Profiles\30jd6mb5.default\user.js

            var profilesDir = Environment.ExpandEnvironmentVariables(
                @"%APPDATA%\Mozilla\Firefox\Profiles");
            var defProfDir = Directory.GetDirectories(profilesDir, "*default")[0];

            prefsPath = Path.Combine(defProfDir, "user.js");
        }

        public void SetProxy(string ip)
        {
            var prefs = string.Format(PREFS_TEMPLATE, ip.Split(':')[0], ip.Split(':')[1]);

            File.WriteAllText(prefsPath, prefs);
        }
    }
}
