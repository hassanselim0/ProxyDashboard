using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProxyDashboard.ProxyProviders
{
    /// <summary>
    /// A proxy provider based on proxy-list.txt
    /// </summary>
    public class TxtFile : IProxyProvider
    {
        private const string FILE_NAME = "proxy-list.txt";

        private List<string> lines;

        public TxtFile()
        {
            if (File.Exists(FILE_NAME))
                lines = File.ReadAllLines(FILE_NAME).ToList();
            else
                lines = new List<string>();
        }

        public IEnumerable<string> EnumerateIPs()
        {
            var list = lines.Select(l => l.Trim())
                .Where(l => l != "" && !l.StartsWith("#"));

            return list;
        }

        public void AddIP(string ip)
        {
            if (lines.Contains(ip)) return;

            lines.Add(ip);

            File.WriteAllLines(FILE_NAME, lines);
        }

        public void RemoveIP(string ip)
        {
            if (!lines.Remove(ip)) return;

            File.WriteAllLines(FILE_NAME, lines);
        }
    }
}
