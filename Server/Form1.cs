using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        StreamReader streamReader;  // 데이타 읽기 위한 StreamReader(Receive)
        StreamWriter streamWriter;  // 데이타 쓰기 위한 StreamWriter(Send)
        TcpClient tcpClient;
        NetworkStream stream;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
                

        private void button1_Click(object sender, EventArgs e)
        {
            Thread thread1 = new Thread(connect);      // thread1 생성. 시작은 connect()함수
            thread1.IsBackground = true;        // Form 종료 시, thread1도 종료
            thread1.Start();                    // thread1 시작
        }

        private void connect()
        {            
            try
            {
                // IP와 port번호 바인딩
                TcpListener tcpListener = new TcpListener(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text));
                tcpListener.Start();        // Listen 시작
                
                byte[] buffer = new byte[256];

                writeRichTextbox("Sever Open");
                while (true)
                {
                    // 클라이언트 접속 확인
                    tcpClient = tcpListener.AcceptTcpClient();
                    writeRichTextbox("Client Connected");

                    stream = tcpClient.GetStream();     // 클라와 연결

                    streamReader = new StreamReader(tcpClient.GetStream());
                    streamWriter = new StreamWriter(tcpClient.GetStream());
                    streamWriter.AutoFlush = true;

                    while (tcpClient.Connected)
                    {
                        // 읽을 게 있으면 수신
                        if (stream.Read(buffer, 0, buffer.Length) != 0)
                        {
                            Recv(buffer);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }            
        }

        // ===================================== 수신 (버퍼 -> 구조체 ) =================================
        // 수신
        /*void Recv(byte[] buffer)
        {
            Heartbeat packet = GetStream(buffer);
            Int32 len = packet.len;
            Int32 protocol = packet.protocol;            
        }*/

        // 역직렬화(버퍼 -> 구조체)
        private Heartbeat Recv(byte[] btfuffer)
        {
            Heartbeat packet = new Heartbeat();

            MemoryStream ms = new MemoryStream(btfuffer, false);
            BinaryReader br = new BinaryReader(ms);

            // 직렬화된 데이터에서 멤버변수들을 가져온다. 
            int len = IPAddress.NetworkToHostOrder(br.ReadInt32());
            int protocol = IPAddress.NetworkToHostOrder(br.ReadInt32());
            int bcc = IPAddress.NetworkToHostOrder(br.ReadInt32());

            // 디버깅
            writeRichTextbox("======= Recv Heartbeat =======");
            writeRichTextbox($"len : {len}");
            writeRichTextbox($"protocol : {protocol}");
            writeRichTextbox($"bcc : {bcc}");

            br.Close();
            ms.Close();

            return packet;
        }

        // ===================================== 송신 (구조체 -> 버퍼) =================================
        #region HeartBeat 전송
        public const int BODY_BIND_SIZE = 20 + 20 + 4 + 100;

        // 직렬화
        byte[] GetBytes_Bind(Heartbeat pHeartbeat)
        {
            byte[] btBuffer = new byte[BODY_BIND_SIZE];

            MemoryStream ms = new MemoryStream(btBuffer, true);
            BinaryWriter bw = new BinaryWriter(ms);

            // packet을 byte화 함
            bw.Write(IPAddress.HostToNetworkOrder(pHeartbeat.len));
            bw.Write(IPAddress.HostToNetworkOrder(pHeartbeat.protocol));
            bw.Write(IPAddress.HostToNetworkOrder(pHeartbeat.bcc));

            // 디버깅
            writeRichTextbox("======= Send Heartbeat =======");
            writeRichTextbox($"len : {pHeartbeat.len}");
            writeRichTextbox($"protocol : {pHeartbeat.protocol}");
            writeRichTextbox($"bcc : {pHeartbeat.bcc}");

            bw.Close();
            ms.Close();

            return btBuffer;
        }

        void SendHB(byte[] buffer)
        {
            if (!tcpClient.Connected) return;
            
            stream.Write(buffer, 0, buffer.Length);     // 보내기 write
            /*// 디버깅
            Heartbeat pHeartbeat = new Heartbeat();

            Int32 len = pHeartbeat.len;
            Int32 protocol = pHeartbeat.protocol;
            byte bcc = pHeartbeat.bcc;

            writeRichTextbox($"len : {len}");
            writeRichTextbox($"protocol : {protocol}");
            writeRichTextbox($"bcc : {bcc}");*/

        }
        #endregion

        // =================== Button ===================

        // 문자열 뒤쪽에 위치한 null 을 제거한 후에 공백문자를 제거한다.
        private static string ExtendedTrim(string source)
        {
            string dest = source;
            int index = dest.IndexOf('\0');
            if (index > -1)
            {
                dest = source.Substring(0, index + 1);
            }

            return dest.TrimEnd('\0').Trim();
        }

        // richTextbox1 에 쓰기 함수
        private void writeRichTextbox(string str)
        {
            // 데이타를 수신창에 표시, 반드시 invoke 사용. 충돌피함.
            richTextBox1.Invoke((MethodInvoker)delegate { richTextBox1.AppendText(str + "\r\n"); });
            // 스크롤을 젤 밑으로.
            richTextBox1.Invoke((MethodInvoker)delegate { richTextBox1.ScrollToCaret(); });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string sendData1 = "Heart beat";
            streamWriter.WriteLine(sendData1); // 문자열 데이터 전송
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Heartbeat pHeartbeat = new Heartbeat();
            pHeartbeat.len = 2;
            pHeartbeat.protocol = 2000;
            pHeartbeat.bcc = 2;
            byte[] buffer = GetBytes_Bind(pHeartbeat);
            SendHB(buffer);
        }
    }
}
