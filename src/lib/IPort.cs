using System.Net;

namespace Flux.Client.Datagram
{
    public interface IPort
    {
        void Connect(IPAddress address, int port);
        void Send(byte[] data);
        (byte[], IPEndPoint) Receive();
        void Close();
    }
}
