using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace NetworkUtils
{
    public class NetworkDiscovery
    {
        //BASE CODE FROM https://stackoverflow.com/questions/13492134/find-all-ip-address-in-a-network

        private List<Ping> Pingers { get; set; } = new List<Ping>();
        private int Instances { get; set; }

        private readonly object _lock = new object();
        public string BaseIp { get; set; } = "192.168.1.";
        private int TimeOut { get; set; } = 250;

        private int Ttl { get; set; } = 5;

        /// <summary>
        /// Creates a new NetworkDiscovery instance
        /// </summary>
        /// <param name="baseIp">The base ip address mask. Ex: <b>192.168.1.</b> **MUST END IN A DOT**</param>
        public NetworkDiscovery(string baseIp = null)
        {
            if (baseIp != null)
                BaseIp = baseIp;
        }

        public async Task<List<IPAddress>> SearchAsync()
        {
            CreatePingers(255);

            var po = new PingOptions(Ttl, true);
            var enc = new System.Text.ASCIIEncoding();
            var data = enc.GetBytes("abababababababababababababababab");

            var found = new List<IPAddress>();
            var tasks = Pingers.Select((p, o) =>
                {
                    return Task.Factory.StartNew(async () =>
                    {
                        lock (_lock)
                        {
                            Instances += 1;
                        }
                        var pingReply = await p.SendPingAsync($"{BaseIp}{o++}", TimeOut, data, po);

                        if (pingReply.Status == IPStatus.Success)
                            found.Add(pingReply.Address);
                    });
                })
                .Cast<Task>()
                .ToList();

            await Task.WhenAll(tasks);

            DestroyPingers();

            return found;
        }


        private void CreatePingers(int cnt)
        {
            for (var i = 1; i <= cnt; i++)
                Pingers.Add(new Ping());
        }

        private void DestroyPingers()
        {
            foreach (var p in Pingers)
                p.Dispose();

            Pingers.Clear();
        }
    }
}