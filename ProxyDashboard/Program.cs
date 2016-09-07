using System;
using System.Collections.Generic;
using System.Linq;

namespace ProxyDashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var interProxy = new IntermediateProxy();

            Console.WriteLine("Enter a proxy IP if you just want to run the intermediate proxy alone:");
            var forceIP = Console.ReadLine();
            if (forceIP != "")
            {
                interProxy.SetProxy(forceIP);
                interProxy.Run();

                Console.WriteLine("Tunnel Running, press 'E' to Exit");
                while (true) if (char.ToUpper(Console.ReadKey(true).KeyChar) == 'E') return;
            }

            var cts = new System.Threading.CancellationTokenSource();

            var proxyListTxt = new ProxyProviders.TxtFile();

            var providers = new IProxyProvider[]
            {
                proxyListTxt,
                new ProxyProviders.SamAirRu(),
                new ProxyProviders.InCloakCom(),
                new ProxyProviders.ProxyNovaCom(),
                new ProxyProviders.ProxyListOrg(),
                new ProxyProviders.UsProxyOrg(),
            }
            .AsParallel().WithCancellation(cts.Token)
            .WithMergeOptions(ParallelMergeOptions.NotBuffered);

            var ignoreList = new ProxyValidators.IgnoreList();

            var validators = new IProxyValidator[]
            {
                ignoreList,
                new ProxyValidators.GoogleTest(cts.Token),
            };

            Console.WriteLine("Getting Proxy List ...");
            var proxyIPs = providers.SelectMany(p =>
            {
                try
                {
                    return p.EnumerateIPs();
                }
                catch
                {
                    Console.WriteLine("Failed to get list from: " + p.GetType().Name);
                    return Enumerable.Empty<string>();
                }
            }).Distinct();

            var validIPs = proxyIPs.AsParallel()
                .WithCancellation(cts.Token)
                .WithDegreeOfParallelism(8)
                .WithMergeOptions(ParallelMergeOptions.NotBuffered)
                .Where(ip =>
                {
                    var failure = validators.FirstOrDefault(v => !v.IsValid(ip));

                    if (!cts.IsCancellationRequested)
                        Console.WriteLine(ip + (ip.Length >= 16 ? "\t" : "\t\t")
                            + (failure == null ? "Success!" : "Fail: " + failure.Name));

                    return failure == null;
                });

            foreach (var ip in validIPs)
            {
                Console.WriteLine("\t\t\t\tSetting Proxy ...");
                Console.WriteLine("\t\t\t\tDone: " + ip);
                interProxy.SetProxy(ip);
                interProxy.Run();

                var keys = new[] { 'S', 'N', 'E' };
                var key = '\0';

                while (!keys.Contains(char.ToUpper(key)))
                {
                    key = '\0';
                    Console.WriteLine("\t\t\t\tSkip/Add/Next/Exit?");
                    while (key == '\0') key = Console.ReadKey(true).KeyChar;

                    if (char.ToUpper(key) == 'A')
                    {
                        Console.WriteLine("\t\t\t\tAdding ...");
                        proxyListTxt.AddIP(ip);
                    }
                }

                if (char.ToUpper(key) == 'S')
                {
                    Console.WriteLine("\t\t\t\tSkipping ...");
                    ignoreList.AddIgnore(ip);
                    proxyListTxt.RemoveIP(ip);
                }

                if (char.ToUpper(key) == 'N')
                    Console.WriteLine("\t\t\t\tNext ...");

                if (char.ToUpper(key) == 'E')
                {
                    Console.WriteLine("\t\t\t\tExiting ...");
                    cts.Cancel();
                    break;
                }
            }

            if (cts.IsCancellationRequested) return;

            Console.Write("\r\nReached the End of the List! ");
            while (Console.ReadKey(true).KeyChar == '\0') ;
        }
    }
}
