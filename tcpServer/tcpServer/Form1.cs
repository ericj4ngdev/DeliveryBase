using System;
using System.Text;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace tcpServer
{
    public partial class Form1 : Form
    {
        String strIp = String.Empty;
        int intPort = 25001;
        bool bServer = false;
        Thread mainTh = null;
        TcpClient socket;
        TcpListener listen;
        NetworkStream stream;
        
        byte[] buff = new byte[1024];

        private StringBuilder _Strings;
        private String Logs
        {
            set
            {
                if (_Strings == null)
                    _Strings = new StringBuilder(1500);
                if (_Strings.Length >= (1500 - value.Length))
                    _Strings.Clear();
                _Strings.AppendLine(value);
                textBox3.Text = _Strings.ToString();
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!bServer)
            {
                serverOpen();
            }
            else
            {
                serverClose();
            }
        }



        void serverOpen()
        {
            try
            {
                mainTh = new Thread(new ThreadStart(Running));
                mainTh.Start();
                bServer = true;
            }
            catch(Exception exe)
            {
                Logs = String.Format("System Error = {0}", exe.Message);

                bServer = false;
                return;
            }
        }

        void Running()
        {
            try
            {
                // IP, port 입력
                strIp = textBox1.Text;
                intPort = Int32.Parse(textBox2.Text);
                listen = new TcpListener(IPAddress.Parse(strIp), intPort);
                listen.Start();

                this.Invoke(new MethodInvoker(delegate ()
                {
                    Logs = String.Format("Server Open");
                    button1.Text = "Stop";
                }));
            }
            catch(Exception exe)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    Logs = String.Format("Server Error = {0}", exe.Message);
                    button1.Text = "Start";
                }));

                bServer = false;
                return;
            }

            try
            {
                // listen 시작
                socket = listen.AcceptTcpClient();
                stream = socket.GetStream();
            }
            catch
            {

            }

            int intLength = 0;
            while((intLength = stream.Read(buff, 0, buff.Length)) > 0)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    Logs = String.Format("Receive Data = {0} Byte", buff.Length);
                    Logs = String.Format("Content");
                }));

                String strData = byteToString(buff);

                this.Invoke(new MethodInvoker(delegate ()
                {
                    Logs = String.Format("{0}", strData);
                    //Logs = String.Format("\n");
                }));
            }
        }


        void serverClose()
        {
            try
            {
                if(socket != null) socket.Close();
                if (stream != null) stream.Close();

                listen.Stop();
                mainTh.Abort();

                this.Invoke(new MethodInvoker(delegate ()
                {
                    Logs = String.Format("Server Closed");
                    button1.Text = "Start";
                    bServer = false;
                }));
            }
            catch(Exception exe)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    Logs = String.Format("Server Closed Error = {0}", exe.Message);
                }));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _Strings.Clear();
            textBox3.Text = _Strings.ToString();
        }

        String byteToString(byte[] strByte)
        {
            string str = Encoding.Default.GetString(strByte);
            return str;
        }

        // 서버에서 클라이언트로 데이터 보내는 함수
        void SendToClient(TcpClient client, string data)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine(data);     // send
                writer.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending data to client: " + ex.Message);
            }
        }

        // 클라이언트와 통신하는 부분에서 SendToClient 함수 호출
        private void button3_Click(object sender, EventArgs e)
        {
            // 클라이언트로 데이터 전송
            SendToClient(socket, "CallFuncA");
        }
    }//form
}
