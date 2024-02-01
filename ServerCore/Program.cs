using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static Listener _listener = new Listener();
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost =  Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            try
            {
                _listener.init(endPoint);

                while (true)
                {
                    Console.WriteLine("Listening...");
                
                    Socket clientSocket = _listener.Accept();

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