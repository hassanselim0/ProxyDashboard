using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace ProxyDashboard.ProxyProviders
{
    /// <summary>
    /// A proxy provider for proxy-list.org
    /// </summary>
    public class ProxyListOrg : IProxyProvider
    {
        private HttpClient http;

        public ProxyListOrg()
        {
            http = new HttpClient(new HttpClientHandler()
            {
                UseProxy = false,
            });
        }

        public IEnumerable<string> EnumerateIPs()
        {
            var html = http.GetStringAsync("https://proxy-list.org/english/search.php"
                + "?search=US&country=US&type=any&port=any&ssl=any").Result;

            var tbodyIdx = html.IndexOf("<div id=\"proxy-table");
            var tbody = html.Substring(tbodyIdx, html.IndexOf("<div class=\"table-menu") - tbodyIdx);

            tbody = tbody.Replace("&nbsp;", " ");

            var xml = XElement.Parse(tbody);
            var table = xml.Elements("div").ElementAt(1).Element("div");

            return table.Elements("ul").Select(ul =>
            {
                var ip = ul.Elements("li").ElementAt(0).Value;
                var idx = ip.IndexOf('\'') + 1;
                ip = ip.Substring(idx, ip.LastIndexOf('\'') - idx);
                ip = Encoding.UTF8.GetString(Convert.FromBase64String(ip));

                return ip;
            });
        }
    }
}
