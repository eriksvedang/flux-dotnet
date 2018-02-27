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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Flux.Client.Datagram
{
	public class Client
	{
		UdpClient udp;
		IPEndPoint endpoint;
		IPacketReceiver receiver;
		bool isListening;
		Thread listeningThread;

		public Client(string host, int port, IPacketReceiver receiver)
		{
			endpoint = new IPEndPoint(IPAddress.Parse(host), port);
			var localBindPoint = new IPEndPoint (IPAddress.Any, 0);
			udp = new UdpClient(localBindPoint);
			this.receiver = receiver;
			StartListener();
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
				var receivedEndpoint = new IPEndPoint (IPAddress.Any, 32001);

				var octets = Receive(out receivedEndpoint);
				receiver.ReceivePacket(octets, receivedEndpoint);
			}
		}

		public void Send(byte[] data)
		{
			udp.Send(data, data.Length, endpoint);
		}

		private byte[] Receive(out IPEndPoint receivedEndpoint)
		{
			IPEndPoint hostEndpoint = null;
			var data = udp.Receive(ref hostEndpoint);

			receivedEndpoint = hostEndpoint;
			return data;
		}
	}
}
