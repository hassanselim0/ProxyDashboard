using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace ProxyDashboard.ProxyValidators
{
    /// <summary>
    /// A Validator that test a proxy by doing a HTTPS HEAD request to google.com through that proxy
    /// </summary>
    public class GoogleTest : IProxyValidator
    {
        public string Name { get; } = "Google Test";

        private CancellationToken ct;

        public GoogleTest(CancellationToken ct)
        {
            this.ct = ct;
        }

        public bool IsValid(string ip)
        {
            var http = new HttpClient(new HttpClientHandler()
            {
                Proxy = new WebProxy(ip),
                UseProxy = true,
            });
            http.Timeout = TimeSpan.FromSeconds(8);

            try
            {
                http.SendAsync(new HttpRequestMessage(
                    HttpMethod.Head, "https://google.com/")).Wait(ct);
            }
            catch { return false; }

            return true;
        }

        private class WebProxy : IWebProxy
        {
            private Uri uri;

            public WebProxy(string ip)
            {
                uri = new Uri("http://" + ip);
            }

            public ICredentials Credentials { get; set; } = null;

            public Uri GetProxy(Uri destination)
            {
                return uri;
            }

            public bool IsBypassed(Uri host)
            {
                return false;
            }
        }
    }
}
