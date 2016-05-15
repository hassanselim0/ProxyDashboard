using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ProxyDashboard.ProxyValidators
{
    /// <summary>
    /// A Validator that tests if an ip doesn't exist on the ignore list.
    /// It also allows ips to be added to the ignore list.
    /// </summary>
    public class IgnoreList : IProxyValidator
    {
        public string Name { get; } = "Ignore List";

        private const string FILE_NAME = "ignored.txt";

        private List<string> lines;

        public IgnoreList()
        {
            if (File.Exists(FILE_NAME))
                lines = File.ReadAllLines(FILE_NAME).ToList();
            else
                lines = new List<string>();
        }

        public bool IsValid(string ip)
        {
            return !lines.Contains(ip);
        }

        public void AddIgnore(string ip)
        {
            if (lines.Contains(ip)) return;

            lines.Add(ip);

            File.WriteAllLines(FILE_NAME, lines);
        }
    }
}
