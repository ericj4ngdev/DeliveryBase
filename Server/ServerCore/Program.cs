using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace ServerCore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Dns(Domain Name System)
            // www.naver.com(Dns) -> 123.456.78.9(IP)
            string host = Dns.GetHostName();                        // 현재 컴퓨터의 호스트 이름
            IPHostEntry ipHost = Dns.GetHostEntry(host);            // 매개변수로 넣은 호스트의 Dns 정보 반환.
            IPAddress ipAddr = ipHost.AddressList[0];               // ip주소얻어옴. AddressList가 배열인데 우리는 첫 번째거만 가져올 것이다.
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);     // IPEndPoint가 최종 주소 (ip : port)
            // Console.WriteLine(ipAddr);
            Console.WriteLine(endPoint);
            // 여기까지 DNS를 통해 자기 컴퓨터의 주소를 얻어온 것이다.

            try
            {
                // 문지기
                Socket listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // endPoint.AddressFamily = Ipv4, Ipv6 인지 프로토콜 설정
                // 2,3번째 인자 : 프로토콜 설정(TCP, UDP)

                // 주소 바인딩
                listenSocket.Bind(endPoint);

                // Listen
                // backlog 최대 대기수
                listenSocket.Listen(10);

                while (true)
                {
                    Console.WriteLine("Listening...");

                    // 클라 입장
                    // 소켓을 반환하는데 이는 바로 클라와 소통할 수 있는 통신 소켓이다. 
                    // 해서 이를 clientSocket으로 받아준다. 
                    Socket clientSocket = listenSocket.Accept();

                    // 송수신 

                    // 수신
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = clientSocket.Receive(recvBuff);
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"[From Client] {recvData}");
                    // recvBuff : 문자열 내용
                    // 0: 시작 인덱스
                    // recvBytes : 문자열 크기

                    // 송신
                    // 보낼 문자열을 버퍼에 저장
                    byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Sever!!");
                    clientSocket.Send(sendBuff);                    // 클라에게 보내기

                    // 쫓아낸다.
                    clientSocket.Shutdown(SocketShutdown.Both);      // 둘다 끊어버린다고 예고
                    clientSocket.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
        }
    }
}
