using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyDashboard
{
    /// <summary>
    /// This class is responsible for running an intermediate proxy that relays everything to another
    /// proxy server.
    /// Basically it's a reverse proxy for proxy servers ... yeah :D
    /// </summary>
    public class IntermediateProxy : IProxySetter
    {
        private IPAddress ip;
        private int port;

        private CancellationTokenSource cts;

        private bool isRunning;

        /// <summary>
        /// Starts the Intermediate Proxy Server to start accepting incoming connections
        /// </summary>
        public async void Run()
        {
            if (isRunning) return;
            isRunning = true;

            var server = new TcpListener(IPAddress.Any, 8080);
            server.Start();

            int cCount = 0; // Number of currently open connections

            while (true)
            {
                var cId = "[" + ++cCount + "] "; // use that count as an Id used for debugging

                var client1 = await server.AcceptTcpClientAsync();
                Console.WriteLine(cId + "Accepted Browser Connection");

                var client2 = new TcpClient();
                try
                {
                    client2.Connect(ip, port);
                    Console.WriteLine(cId + "Connected to Proxy");
                }
                catch
                {
                    Console.WriteLine(cId + "Failed to Connect to Proxy");
                    client1.Close();
                    continue;
                }

                handleClients(cId, client1, client2); // Fire-and-Forget
            }
        }

        /// <summary>
        /// Set the IP and Port of the Proxy Server that this intermediate server connects to
        /// </summary>
        /// <param name="IpPort"></param>
        public void SetProxy(string IpPort)
        {
            var parts = IpPort.Split(':');

            ip = IPAddress.Parse(parts[0]);
            port = int.Parse(parts[1]);

            if (cts != null) cts.Cancel();
            cts = new CancellationTokenSource();
        }

        // This handles the state of clients (one for each end of this "intermediate" proxy)
        private async void handleClients(string cId, TcpClient client1, TcpClient client2)
        {
            client1.ReceiveTimeout = client2.ReceiveTimeout = 2000;

            cts.Token.Register(client1.Close);
            cts.Token.Register(client2.Close);

            await Task.WhenAll(
                handleBrowserToProxy(cId, client1, client2),
                handleProxyToBrowser(cId, client1, client2));

            Console.WriteLine("\t" + cId + "Closing Connections");
            client1.Close();
            client2.Close();
        }

        // Handles data going from the Browser to the Proxy
        private async Task handleBrowserToProxy(string cId, TcpClient client1, TcpClient client2)
        {
            var stream1 = client1.GetStream();
            var stream2 = client2.GetStream();

            var buffer = new byte[4 * 1024];

            while (client1.Connected && client2.Connected)
                try
                {
                    // Browser to Buffer
                    var count = await stream1.ReadAsync(buffer, 0, buffer.Length);
                    logTraffic(cId, false, "Browser", count);

                    if (count == 0)
                    {
                        await Task.Delay(20);
                        continue;
                    }

                    // Buffer to Proxy
                    await stream2.WriteAsync(buffer, 0, count);
                    logTraffic(cId, true, "Proxy", count);
                }
                catch
                {
                    break;
                }
        }

        // Handles Data going from the Proxy to the Browser
        private async Task handleProxyToBrowser(string cId, TcpClient client1, TcpClient client2)
        {
            var stream1 = client1.GetStream();
            var stream2 = client2.GetStream();

            var buffer = new byte[64 * 1024];

            while (client1.Connected && client2.Connected)
                try
                {
                    // Proxy to Buffer
                    var count = await stream2.ReadAsync(buffer, 0, buffer.Length);
                    logTraffic(cId, false, "Proxy", count);

                    if (count == 0)
                    {
                        await Task.Delay(20);
                        continue;
                    }

                    // Buffer to Browser
                    await stream1.WriteAsync(buffer, 0, count);
                    logTraffic(cId, true, "Browser", count);
                }
                catch
                {
                    break;
                }
        }

        private void logTraffic(string cId, bool dir, string other, int size)
        {
            //System.Diagnostics.Debug.WriteLine("{0}Buffer {1} {2}\t{3} bytes",
            //    cId, dir ? "->" : "<-", other, size);
        }
    }
}
