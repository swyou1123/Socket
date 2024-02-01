using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost =  Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Socket listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);


            try
            {
                listenSocket.Bind(endPoint);

                listenSocket.Listen(10);

                while (true)
                {
                    Console.WriteLine("Listening...");
                
                    Socket clientSocket = listenSocket.Accept(); // 입장

                    byte[] receBuff = new byte[1024];
                    int recvBytes = clientSocket.Receive(receBuff);
                    string recvData = Encoding.UTF8.GetString(receBuff, 0, recvBytes );
                    Console.WriteLine($"[From Client] {recvData}");                     /// 받는 부분

                    byte[] sendBuff = Encoding.UTF8.GetBytes("Welcom to MMORPG SERVER!");
                    clientSocket.Send(sendBuff);                                        /// 보내는 부분

                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();                                               /// 쫒아내는 부분
                }
            }
            catch (Exception e)
            {
                 Console.WriteLine(e.ToString());
            }

        }
    }
}