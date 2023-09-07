using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

                    // 읽을게 있다면
                    // while (stream.Read(buffer, 0, buffer.Length) != 0)                   


                    streamReader = new StreamReader(tcpClient.GetStream());
                    streamWriter = new StreamWriter(tcpClient.GetStream());
                    streamWriter.AutoFlush = true;

                    while (tcpClient.Connected)
                    {
                        if (stream.Read(buffer, 0, buffer.Length) != 0)
                        {
                            DataPacket packet = GetBindAck(buffer);
                            Int32 len = packet.len;
                            Int32 protocol = packet.protocol;
                            // byte bcc = packet.bcc;

                            writeRichTextbox($"len : {len}");
                            writeRichTextbox($"protocol : {protocol}");
                            // writeRichTextbox($"bcc : {bcc}\n");
                            writeRichTextbox("==============================");
                        }
                        // string receiveData1 = streamReader.ReadLine();  // 수신 데이터를 읽어서 receiveData1 변수에 저장
                        // writeRichTextbox(receiveData1);                 // receiveData1 출력
                    }
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }            
        }


        private static DataPacket GetBindAck(byte[] btfuffer)
        {
            DataPacket packet = new DataPacket();

            MemoryStream ms = new MemoryStream(btfuffer, false);
            BinaryReader br = new BinaryReader(ms);

            packet.len = IPAddress.NetworkToHostOrder(br.ReadInt32());
            packet.protocol = IPAddress.NetworkToHostOrder(br.ReadInt32());
            // packet.bcc = br.ReadBytes(10);


            br.Close();
            ms.Close();

            return packet;
        }

        public void Recv(byte[] buff)
        {

        }


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

    }
}
