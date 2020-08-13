using System.Net;

namespace Flux.Client.Datagram
{
    public interface IPort
    {
        void Send(IPEndPoint endPoint, byte[] data);
        (byte[], IPEndPoint) Receive();
    }
}
