using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ProxyDashboard.ProxyProviders
{
    /// <summary>
    /// A proxy provider for us-proxy.org
    /// </summary>
    public class UsProxyOrg : IProxyProvider
    {
        private HttpClient http;

        public UsProxyOrg()
        {
            http = new HttpClient(new HttpClientHandler()
            {
                UseProxy = false,
            });
        }

        public IEnumerable<string> EnumerateIPs()
        {
            var html = http.GetStringAsync("https://www.us-proxy.org/").Result;

            return useXml(html); // Faster (1.9-2.0 ms)
            //return useRegex(html); // Slower (4.2-4.3 ms) (3.5-2.6 ms compiled)
        }

        private IEnumerable<string> useXml(string html)
        {
            var tbodyIdx = html.IndexOf("<tbody>");
            var tbody = html.Substring(tbodyIdx, html.IndexOf("</tbody>") - tbodyIdx + 8);

            var xml = XElement.Parse(tbody);

            return xml.Elements("tr").Select(row =>
            {
                var cells = row.Elements("td");
                return cells.ElementAt(0).Value + ':' + cells.ElementAt(1).Value;
            });
        }

        private IEnumerable<string> useRegex(string html)
        {
            var tbody = Regex.Match(html, @"<tbody><tr>([\s\S]*)<\/tr>\n<\/tbody>").Groups[1].Value;
            var rows = Regex.Matches(tbody, @"([\d.]{7,15})<\/td><td>(\d{1,5})");

            return rows.Cast<Match>().Select(row => row.Result("$1:$2"));
        }
    }
}
