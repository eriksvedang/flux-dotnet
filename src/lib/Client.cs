/*

MIT License

Copyright (c) 2017 Peter Bjorklund

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/
using System;
using System.Net;
using System.Threading;

namespace Flux.Client.Datagram
{
    public class Client
    {
        IPort port;
        IPEndPoint endpoint;
        public IPacketReceiver receiver;
        bool isListening;
        Thread listeningThread;
        bool useThreads;

        public Client(IPacketReceiver receiver, IPort port, bool useThreads)
        {
            this.port = port;
            this.receiver = receiver;
            this.useThreads = useThreads;

            if(useThreads) {
                StartListener();
            }
        }

        public void Close()
        {
            listeningThread.Join();
            listeningThread = null;
        }

        public void SetDefaultSendEndpoint(string hostAndPort)
        {
            var hostLength = hostAndPort.LastIndexOf(':');

            var hostname = hostAndPort.Substring(0, hostLength);
            var port = Convert.ToInt32(hostAndPort.Substring(hostLength + 1));

            var addresses = Dns.GetHostAddresses(hostname);

            if (addresses.Length < 1)
            {
                throw new Exception($"Couldn't find the dns lookup for {hostname}");
            }
            endpoint = new IPEndPoint(addresses[0], port);
        }

        public void StartListener()
        {
            if (!isListening)
            {
                listeningThread = new Thread(ListenForUDPPackages);
                isListening = true;
                listeningThread.Start();
            }
        }

        public void ListenForUDPPackages()
        {
            while (isListening)
            {
                try
                {
                    Relay();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Flux: Error {e}");
                    receiver.HandleException(e);
                }
            }
        }

        public void Send(byte[] data)
        {
            Send(data, endpoint);
        }

        public void Send(byte[] data, IPEndPoint receivedEndpoint)
        {
            const int maxRecommendedSize = 1280;
            if (data.Length > maxRecommendedSize)
            {
                throw new Exception($"UDP Send: Packet too big {data.Length}");
            }

            port.Send(receivedEndpoint, data);
        }

        public byte[] Receive(out IPEndPoint receivedEndpoint)
        {
            var (data, hostEndpoint) = port.Receive();
            receivedEndpoint = hostEndpoint;
            return data;
        }

        private void Relay()
        {
            System.Console.WriteLine("Client.Relay()");

            var receivedEndpoint = new IPEndPoint(IPAddress.Any, 32001);
            var octets = Receive(out receivedEndpoint);
            receiver.ReceivePacket(octets, receivedEndpoint);
        }

        public void Update()
        {
            System.Console.WriteLine("Client.Update()");

            if(!useThreads)
            {
                Relay();
            }
        }
    }
}
