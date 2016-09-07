using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;

namespace ProxyDashboard.ProxyProviders
{
    /// <summary>
    /// A proxy provider for proxynova.com
    /// </summary>
    public class ProxyNovaCom : IProxyProvider
    {
        private HttpClient http;

        public ProxyNovaCom()
        {
            http = new HttpClient(new HttpClientHandler()
            {
                UseProxy = false,
            });
        }

        public IEnumerable<string> EnumerateIPs()
        {
            var html = http.GetStringAsync(
                "http://www.proxynova.com/proxy-server-list/country-us/").Result;

            var tbodyIdx = html.IndexOf("<tbody>");
            var tbody = html.Substring(tbodyIdx, html.IndexOf("</tbody>") - tbodyIdx + 8);

            tbody = tbody.Replace(" colspan=10", "").Replace(" async", "");

            var xml = XElement.Parse(tbody);

            return xml.Elements("tr").Select(row =>
            {
                var cells = row.Elements("td");

                var ip = cells.ElementAt(0).Value;
                if (ip.Contains("adsbygoogle")) return null;

                if (ip.Contains('\''))
                {
                    var idx = ip.IndexOf('\'') + 1;
                    ip = ip.Substring(idx, ip.LastIndexOf('\'') - idx);
                }

                var port = cells.ElementAt(1).Value;
                port = port.Trim();

                return ip + ':' + port;
            })
            .Where(ip => ip != null);
        }
    }
}
