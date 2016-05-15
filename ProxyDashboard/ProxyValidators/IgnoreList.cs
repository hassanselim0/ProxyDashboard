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
        private const string FILE_NAME = "ignored.txt";

        private List<string> list;

        public string Name { get; } = "Ignore List";

        public bool IsValid(string ip)
        {
            if (list == null)
                if (File.Exists(FILE_NAME))
                    list = File.ReadAllLines(FILE_NAME).ToList();
                else
                    list = new List<string>();

            return !list.Contains(ip);
        }

        public void AddIgnore(string ip)
        {
            if (list.Contains(ip)) return;

            list.Add(ip);

            File.WriteAllLines(FILE_NAME, list);
        }
    }
}
