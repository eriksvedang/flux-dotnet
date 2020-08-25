using System.Net;
using System.Net.Sockets;

namespace Flux.Client.Datagram
{
    public class UdpClient : IPort
    {
        System.Net.Sockets.UdpClient udp;

        public UdpClient()
        {
            var localBindPoint = new IPEndPoint(IPAddress.Any, 0);
            udp = new System.Net.Sockets.UdpClient(localBindPoint);
        }

        public void Connect(string address)
        {
            // Not necessary for UDP protocol.
        }

        public (byte[], IPEndPoint) Receive()
        {
            IPEndPoint remote = null;
            var data = udp.Receive(ref remote);
            return (data, remote);
        }

        public void Send(IPEndPoint endPoint, byte[] data)
        {
            udp.Send(data, data.Length, endPoint);
        }
    }
}
