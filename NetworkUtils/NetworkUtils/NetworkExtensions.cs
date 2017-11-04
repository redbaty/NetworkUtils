using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetworkUtils
{
    public static class NetworkExtensions
    {
        public static Task<bool> PingHostAsync(this string hostUri, int portNumber)
        {
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    var client = new TcpClient(hostUri, portNumber);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        public static Task<bool> PingHostAsync(this IPAddress hostUri, int portNumber)
        {
            return PingHostAsync(hostUri.ToString(), portNumber);
        }
    }
}