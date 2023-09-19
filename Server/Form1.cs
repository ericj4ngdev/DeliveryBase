using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;


namespace Server
{
    public partial class Form1 : Form
    {
        StreamReader streamReader;  // 데이타 읽기 위한 StreamReader(Receive)
        StreamWriter streamWriter;  // 데이타 쓰기 위한 StreamWriter(Send)
        TcpClient tcpClient;
        NetworkStream stream;
        IPAddress[] addr;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GetAddress();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread thread1 = new Thread(connect);      // thread1 생성. 시작은 connect()함수
            thread1.IsBackground = true;        // Form 종료 시, thread1도 종료
            thread1.Start();                    // thread1 시작
        }

        private void GetAddress()
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            addr = ipEntry.AddressList;
            // for (int i = 0; i < addr.Length; i++)
            // {
            //     // 디버깅 해보면 1번이 활성화된 IP이다.
            //     writeRichTextbox($"IP Address {i}: {addr[i].ToString()} ");
            // }
            textBox1.Text = addr[1].ToString();
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

        void Recv(byte[] btfuffer)
        {
            // 직렬화된 데이터에서 멤버변수들을 가져온다. 
            Int32 len = BitConverter.ToInt32(btfuffer, 0);
            Int32 protocol = BitConverter.ToInt32(btfuffer, sizeof(Int32));
            switch (protocol)
            {
                case (Int32)protocolNum.stHeartbeat:
                    {
                        Heartbeat pHeartbeat = new Heartbeat();
                        pHeartbeat.Read(btfuffer);
                        writeRichTextbox("===== Recv Heartbeat =====");
                        writeRichTextbox($"len : {pHeartbeat.len}");
                        writeRichTextbox($"protocol : {protocol}");
                        writeRichTextbox($"bcc : {pHeartbeat.bcc}");
                    }
                    break;

                case (Int32)protocolNum.stAddTrayRes:
                    {
                        stAddTrayRes pAddTrayRes = new stAddTrayRes();
                        pAddTrayRes.Read(btfuffer);
                        writeRichTextbox("===== Recv AddTrayRes =====");
                        writeRichTextbox($"len : {pAddTrayRes.len}");
                        writeRichTextbox($"protocol : {pAddTrayRes.protocol}");
                        writeRichTextbox($"bcc : {pAddTrayRes.bcc}");
                        writeRichTextbox($"ret : {pAddTrayRes.ret}");
                        writeRichTextbox($"id : {pAddTrayRes.id[0]}");
                        writeRichTextbox($"column : {pAddTrayRes.column}");
                        writeRichTextbox($"row : {pAddTrayRes.row}");
                        writeRichTextbox($"height : {pAddTrayRes.height}");
                    }
                    break;

                default:
                    break;
            }
        }

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
            richTextBox1.Invoke((MethodInvoker)delegate     { richTextBox1.ScrollToCaret(); });
        }        

        private void button3_Click(object sender, EventArgs e)
        {
            Heartbeat pHeartbeat = new Heartbeat();
            writeRichTextbox("===== Send Heartbeat =====");
            writeRichTextbox($"len : {pHeartbeat.len}");
            writeRichTextbox($"protocol : {pHeartbeat.protocol}");
            writeRichTextbox($"bcc : {pHeartbeat.bcc}");
            byte[] buffer = pHeartbeat.Send();
            stream.Write(buffer, 0, buffer.Length);    // 직렬화된 버퍼를 송신
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(comboBox1.Text == null || comboBox2.Text == null || comboBox3.Text == null) return;
            stAddTrayReq pAddTrayReq = new stAddTrayReq();
            pAddTrayReq.id[0] = 'a';
            
            pAddTrayReq.column = Int32.Parse(comboBox1.Text);
            pAddTrayReq.row = Int32.Parse(comboBox2.Text);
            pAddTrayReq.height = Int32.Parse(comboBox3.Text);

            writeRichTextbox("===== Send AddTrayReq =====");
            writeRichTextbox($"len : {pAddTrayReq.len}");
            writeRichTextbox($"protocol : {pAddTrayReq.protocol}");
            writeRichTextbox($"bcc : {pAddTrayReq.bcc}");
            writeRichTextbox($"id : {pAddTrayReq.id[0]}");
            writeRichTextbox($"column : {pAddTrayReq.column}");
            writeRichTextbox($"row : {pAddTrayReq.row}");
            writeRichTextbox($"height : {pAddTrayReq.height}");

            byte[] buffer = pAddTrayReq.Send();
            stream.Write(buffer, 0, buffer.Length);    // 직렬화된 버퍼를 송신
        }
    }
}
