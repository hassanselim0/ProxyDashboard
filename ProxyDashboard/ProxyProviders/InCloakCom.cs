using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace ProxyDashboard.ProxyProviders
{
    /// <summary>
    /// A proxy provider for incloak.com
    /// </summary>
    public class InCloakCom : IProxyProvider
    {
        private HttpClient http;

        public InCloakCom()
        {
            http = new HttpClient(new HttpClientHandler()
            {
                UseProxy = false,
            });

            // To avoid getting detected as a bot
            http.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
        }

        public IEnumerable<string> EnumerateIPs()
        {
            var html = http.GetStringAsync("https://incloak.com/proxy-list/?country=US").Result;

            var tbodyIdx = html.IndexOf("<tbody>");
            var tbody = html.Substring(tbodyIdx, html.IndexOf("</tbody>") - tbodyIdx + 8);

            while (true)
            {
                var classIdx = tbody.IndexOf(" class=");
                if (classIdx == -1) break;
                var bracketIdx = tbody.IndexOf('>', classIdx);
                tbody = tbody.Remove(classIdx, bracketIdx - classIdx);
            }

            tbody = tbody.Replace("&nbsp;", " ");

            var xml = XElement.Parse(tbody);

            return xml.Elements("tr").Select(row =>
            {
                var cells = row.Elements("td");
                return cells.ElementAt(0).Value + ':' + cells.ElementAt(1).Value;
            });
        }
    }
}
