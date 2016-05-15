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
    public class SamAirRu : IProxyProvider
    {
        private HttpClient http;

        public SamAirRu()
        {
            http = new HttpClient(new HttpClientHandler()
            {
                UseProxy = false,
            });
        }

        public IEnumerable<string> EnumerateIPs()
        {
            var list = new string[0].AsEnumerable();

            for (int i = 1; i <= 3; i++)
            {
                var html1 = http.GetStringAsync(
                    "http://www.samair.ru/proxy-by-country/United-States-0" + i + ".htm").Result;

                var urlIdx = html1.IndexOf("<a class=\"ipport\"href") + 23;
                var url = html1.Substring(urlIdx, html1.IndexOf('"', urlIdx) - urlIdx);

                var html2 = "";
                try { html2 = http.GetStringAsync("http://www.samair.ru/" + url).Result; }
                catch { continue; }

                var preIdx = html2.IndexOf("<pre>") + 5;
                var ips = html2.Substring(preIdx, html2.IndexOf("</pre>") - preIdx - 1);

                list = list.Union(ips.Split('\n'));
            }

            return list;
        }
    }
}
