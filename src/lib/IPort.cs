using System.Net;

namespace Flux.Client.Datagram
{
    public interface IPort
    {
        void Connect(string address);
        void Send(IPEndPoint endPoint, byte[] data);
        (byte[], IPEndPoint) Receive();
    }
}
