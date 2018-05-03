### flux-dotnet

Datagram (UDP) send and receive.

For a c99 version, check out [flux-c](https://github.com/Piot/flux-c).

##### Usage

```csharp
var receiver = new Receiver();
var client = new Client("example.com:32001", receiver);

client.Send(new byte[] {0x20, 0x30, 0x40});

// The receiver object must implement the `ReceivePacket` method.
class Receiver : IPacketReceiver
{
	void ReceivePacket(byte[] octets, IPEndPoint fromEndpoint)
	{
	}
}
```
