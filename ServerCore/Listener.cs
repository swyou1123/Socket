using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    internal class Listener
    {
        Socket _listenSocket;

        public void init(IPEndPoint endPoint)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _listenSocket.Bind(endPoint);

            _listenSocket.Listen(10);
        }

        public Socket Accept()
        {
            _listenSocket.AcceptAsync();
            return _listenSocket.Accept();
        }
    }
}
