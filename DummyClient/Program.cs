﻿using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Connect(endPoint);
                Console.WriteLine($"Connected To {socket.RemoteEndPoint.ToString()  }");

                byte[] sendBuff = Encoding.UTF8.GetBytes("Hello World!");    
                int sendBytes = socket.Send(sendBuff);                              // 보내는 코드

                byte[] receBuff = new byte[1024];
                int recvBytes = socket.Receive(receBuff);
                string recvData = Encoding.UTF8.GetString(receBuff, 0, recvBytes);
                Console.WriteLine($"[From Server] {recvData}");                     // 받는 코드

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();

            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
