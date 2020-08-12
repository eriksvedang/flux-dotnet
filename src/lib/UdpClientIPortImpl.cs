using System.Net;
using System.Net.Sockets;

namespace Flux.Client.Datagram
{
    public class UdpClient : IPort
    {
        System.Net.Sockets.UdpClient udp;

        public UdpClient()
        {
            udp = null;
        }

        public void Close()
        {
            udp.Close();
        }

        public void Connect(IPAddress address, int port)
        {
            var localBindPoint = new IPEndPoint(address, port);
            udp = new System.Net.Sockets.UdpClient(localBindPoint);
        }

        public (byte[], IPEndPoint) Receive()
        {
            IPEndPoint remote = null;
            var data = udp.Receive(ref remote);
            return (data, remote);
        }

        public void Send(byte[] data)
        {
            udp.Send(data, data.Length);
        }
    }
}
