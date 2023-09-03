using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace DummyClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();                        // 현재 컴퓨터의 호스트 이름
            IPHostEntry ipHost = Dns.GetHostEntry(host);            // 매개변수로 넣은 호스트의 Dns 정보 반환.
            IPAddress ipAddr = ipHost.AddressList[0];               // ip주소얻어옴. AddressList가 배열인데 우리는 첫 번째거만 가져올 것이다.
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);     // IPEndPoint가 최종 주소 (ip : port)

            // 연결 소켓 생성
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // 이 소켓을 통해 연결을 시도한다.
                socket.Connect(endPoint);   // 상대방의 주소(서버)를 넣어준다.
                Console.WriteLine($"Connected To {socket.RemoteEndPoint.ToString()}");
                // RemoteEndPoint : 반대쪽 대상
                // 누구한테 연락을 했는지 알려준다. 

                // 송신
                byte[] sendBuff = Encoding.UTF8.GetBytes("Hello World!");
                int sendBytes = socket.Send(sendBuff);

                // 수신
                byte[] recvBuff = new byte[1024];
                int recvBytes = socket.Receive(recvBuff);
                string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                Console.WriteLine($"[From Server] {recvData}");

                // 나간다.
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
