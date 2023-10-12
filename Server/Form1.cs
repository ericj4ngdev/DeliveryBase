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

                WriteRichTextbox("Sever Open");
                while (true)
                {
                    // 클라이언트 접속 확인
                    tcpClient = tcpListener.AcceptTcpClient();
                    WriteRichTextbox("Client Connected");

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
                        WriteRichTextbox("===== Recv Heartbeat =====");
                        WriteRichTextbox($"len : {pHeartbeat.len}");
                        WriteRichTextbox($"protocol : {protocol}");
                        WriteRichTextbox($"bcc : {pHeartbeat.bcc}");
                    }
                    break;
                case (Int32)protocolNum.stAddTrayReq:                   
                    break;
                case (Int32)protocolNum.stAddTrayRes:
                    {
                        stAddTrayRes pAddTrayRes = new stAddTrayRes();
                        pAddTrayRes.Read(btfuffer);
                        WriteRichTextbox("===== Recv AddTrayRes =====");
                        WriteRichTextbox($"len : {pAddTrayRes.len}");
                        WriteRichTextbox($"protocol : {pAddTrayRes.protocol}");
                        WriteRichTextbox($"bcc : {pAddTrayRes.bcc}");
                        WriteRichTextbox($"ret : {pAddTrayRes.ret}");
                        WriteRichTextbox($"id : {pAddTrayRes.id[0]}");
                        WriteRichTextbox($"column : {pAddTrayRes.column}");
                        WriteRichTextbox($"row : {pAddTrayRes.row}");
                        WriteRichTextbox($"height : {pAddTrayRes.height}");
                    }
                    break;
                case (Int32)protocolNum.stDeleteTrayReq:                
                    break;
                case (Int32)protocolNum.stDeleteTrayRes:                
                    break;
                case (Int32)protocolNum.stDeleteAllTrayReq:             
                    break;
                case (Int32)protocolNum.stDeleteAllTrayRes:             
                    break;
                case (Int32)protocolNum.stMoveHandlerReq:               
                    break;
                case (Int32)protocolNum.stMoveHandlerRes:
                    {
                        stMoveHandlerReq stMoveHandlerRes = new stMoveHandlerReq();
                        stMoveHandlerRes.Read(btfuffer);
                        WriteRichTextbox("===== Recv MoveHandlerRes =====");
                        WriteRichTextbox($"len : {stMoveHandlerRes.len}");
                        WriteRichTextbox($"protocol : {stMoveHandlerRes.protocol}");
                        WriteRichTextbox($"bcc : {stMoveHandlerRes.bcc}");
                        WriteRichTextbox($"height : {stMoveHandlerRes.handler}");
                        WriteRichTextbox($"column : {stMoveHandlerRes.column}");
                        WriteRichTextbox($"row : {stMoveHandlerRes.row}");
                    }
                    break;
                case (Int32)protocolNum.stMoveHandlerCompleteNotify:    
                    break;
                case (Int32)protocolNum.stMoveHandlerCompleteRes:       
                    break;
                case (Int32)protocolNum.stLoadTrayReq:                  
                    break;
                case (Int32)protocolNum.stLoadTrayRes:                  
                    break;
                case (Int32)protocolNum.stLoadTrayCompleteNotify:       
                    break;
                case (Int32)protocolNum.stLoadTrayCompleteRes:          
                    break;
                case (Int32)protocolNum.stUnloadTrayReq:                
                    break;
                case (Int32)protocolNum.stUnloadTrayRes:                
                    break;
                case (Int32)protocolNum.stUnloadTrayCompleteNotify:     
                    break;
                case (Int32)protocolNum.stUnloadTrayCompleteRes:        
                    break;
                case (Int32)protocolNum.stEnteranceLoadTrayReq:         
                    break;
                case (Int32)protocolNum.stEnteranceLoadTrayRes:         
                    break;
                case (Int32)protocolNum.stEnteranceLoadTrayCompleteNotify:   
                    break;
                case (Int32)protocolNum.stEnteranceLoadTrayCompleteRes:      
                    break;
                case (Int32)protocolNum.stEnteranceUnloadTrayReq:            
                    break;
                case (Int32)protocolNum.stEnteranceUnloadTrayRes:            
                    break;
                case (Int32)protocolNum.stEnteranceUnloadTrayCompleteNotify: 
                    break;
                case (Int32)protocolNum.stEnteranceUnloadTrayCompleteRes:    
                    break;
                case (Int32)protocolNum.stGateLoadTrayReq:                   
                    break;
                case (Int32)protocolNum.stGateLoadTrayRes:                   
                    break;
                case (Int32)protocolNum.stGateLoadTrayCompleteNotify:        
                    break;
                case (Int32)protocolNum.stGateLoadTrayCompleteRes:           
                    break;
                case (Int32)protocolNum.stGateUnloadTrayReq:                 
                    break;
                case (Int32)protocolNum.stGateUnloadTrayRes:                 
                    break;
                case (Int32)protocolNum.stGateUnloadTrayCompleteNotify:      
                    break;
                case (Int32)protocolNum.stGateUnloadTrayCompleteRes:         
                    break;
                case (Int32)protocolNum.stAddEnteranceParcelReq:             
                    break;
                case (Int32)protocolNum.stAddEnteranceParcelRes:             
                    break;
                case (Int32)protocolNum.stDeleteEnteranceParcelReq:          
                    break;
                case (Int32)protocolNum.stDeleteEnteranceParcelRes:          
                    break;
                case (Int32)protocolNum.stAddGateParcelReq:                  
                    break;
                case (Int32)protocolNum.stAddGateParcelRes:                  
                    break;
                case (Int32)protocolNum.stDeleteGateParcelReq:               
                    break;
                case (Int32)protocolNum.stDeleteGateParcelRes:               
                    break;
                case (Int32)protocolNum.stAllParcelCheckReq:                 
                    break;
                case (Int32)protocolNum.stAllParcelCheckRes:                 
                    break;
                default:
                    break;
            }

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
        private void WriteRichTextbox(string str)
        {
            // 데이타를 수신창에 표시, 반드시 invoke 사용. 충돌피함.
            richTextBox1.Invoke((MethodInvoker)delegate { richTextBox1.AppendText(str + "\r\n"); });
            // 스크롤을 젤 밑으로.
            richTextBox1.Invoke((MethodInvoker)delegate     { richTextBox1.ScrollToCaret(); });
        }

        // =================== Button (송신부) ===================
        // Heartbeat Button
        private void button3_Click(object sender, EventArgs e)
        {
            Heartbeat pHeartbeat = new Heartbeat();
            byte[] buffer = pHeartbeat.Send();
            stream.Write(buffer, 0, buffer.Length);    // 직렬화된 버퍼를 송신

            WriteRichTextbox("===== Send Heartbeat =====");
            WriteRichTextbox($"len :        {pHeartbeat.len}");
            WriteRichTextbox($"protocol :   {pHeartbeat.protocol}");
            WriteRichTextbox($"bcc :        {pHeartbeat.bcc}");
        }

        // Add Tray Button
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "") 
            {
                WriteRichTextbox("column 또는 Row 또는 Height 를 선택하시오.");
                return; 
            }
            stAddTrayReq pAddTrayReq = new stAddTrayReq();
            pAddTrayReq.id[0] = 'a';

            pAddTrayReq.column = Int32.Parse(comboBox1.Text);
            pAddTrayReq.row = Int32.Parse(comboBox2.Text);
            pAddTrayReq.height = Int32.Parse(comboBox3.Text);

            byte[] buffer = pAddTrayReq.Send();
            stream.Write(buffer, 0, buffer.Length);    // 직렬화된 버퍼를 송신

            WriteRichTextbox("===== Send AddTrayReq =====");
            WriteRichTextbox($"len :        {pAddTrayReq.len}");
            WriteRichTextbox($"protocol :   {pAddTrayReq.protocol}");
            WriteRichTextbox($"bcc :        {pAddTrayReq.bcc}");
            WriteRichTextbox($"id :         {pAddTrayReq.id[0]}");
            WriteRichTextbox($"column :     {pAddTrayReq.column}");
            WriteRichTextbox($"row :        {pAddTrayReq.row}");
            WriteRichTextbox($"height :     {pAddTrayReq.height}");
        }

        // Delete Tray
        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "") return;
            stDeleteTrayReq pDeleteTrayReq = new stDeleteTrayReq();
            pDeleteTrayReq.id[0] = 'a';

            pDeleteTrayReq.column = Int32.Parse(comboBox1.Text);
            pDeleteTrayReq.row = Int32.Parse(comboBox2.Text);
            pDeleteTrayReq.height = Int32.Parse(comboBox3.Text);

            byte[] buffer = pDeleteTrayReq.Send();
            stream.Write(buffer, 0, buffer.Length);    // 직렬화된 버퍼를 송신

            WriteRichTextbox("===== Send AddTrayReq =====");
            WriteRichTextbox($"len :        {pDeleteTrayReq.len}");
            WriteRichTextbox($"protocol :   {pDeleteTrayReq.protocol}");
            WriteRichTextbox($"bcc :        {pDeleteTrayReq.bcc}");
            WriteRichTextbox($"id :         {pDeleteTrayReq.id[0]}");
            WriteRichTextbox($"column :     {pDeleteTrayReq.column}");
            WriteRichTextbox($"row :        {pDeleteTrayReq.row}");
            WriteRichTextbox($"height :     {pDeleteTrayReq.height}");
        }

        // Delete All Tray
        private void button5_Click(object sender, EventArgs e)
        {            
            stDeleteAllTrayReq pDeleteAllTrayReq = new stDeleteAllTrayReq();

            byte[] buffer = pDeleteAllTrayReq.Send();
            stream.Write(buffer, 0, buffer.Length);    // 직렬화된 버퍼를 송신

            WriteRichTextbox("===== Send AddTrayReq =====");
            WriteRichTextbox($"len :        {pDeleteAllTrayReq.len}");
            WriteRichTextbox($"protocol :   {pDeleteAllTrayReq.protocol}");
            WriteRichTextbox($"bcc :        {pDeleteAllTrayReq.bcc}");
        }

        // HandlerMove
        private void button6_Click(object sender, EventArgs e)
        {
            // 아무 입력 없으면 아무것도 안함
            if (comboBox1.Text == "" || comboBox2.Text == "" || comboBox4.Text == "") return;       
            stMoveHandlerReq pMoveHandlerReq = new stMoveHandlerReq();

            pMoveHandlerReq.handler = Int32.Parse(comboBox4.Text);      // 핸들 종류
            pMoveHandlerReq.column = Int32.Parse(comboBox1.Text);       // 랙함 번호
            pMoveHandlerReq.row = Int32.Parse(comboBox2.Text);          // 높이

            byte[] buffer = pMoveHandlerReq.Send();    // 버퍼 직렬화
            stream.Write(buffer, 0, buffer.Length);    // 직렬화된 버퍼를 송신

            WriteRichTextbox("===== Send MoveHandlerReq =====");
            WriteRichTextbox($"len :        {pMoveHandlerReq.len}");
            WriteRichTextbox($"protocol :   {pMoveHandlerReq.protocol}");
            WriteRichTextbox($"bcc :        {pMoveHandlerReq.bcc}");
            WriteRichTextbox($"handler :    {pMoveHandlerReq.handler}");
            WriteRichTextbox($"column :     {pMoveHandlerReq.column}");
            WriteRichTextbox($"row :        {pMoveHandlerReq.row}");
        }

        // Load Tray
        private void button8_Click(object sender, EventArgs e)
        {
            // 아무 입력 없으면 아무것도 안함
            if (comboBox1.Text == "" || comboBox2.Text == "" || comboBox4.Text == "") return;
            stLoadTrayReq pLoadTrayReq = new stLoadTrayReq();

            pLoadTrayReq.handler = Int32.Parse(comboBox4.Text);      // 핸들 종류
            pLoadTrayReq.column = Int32.Parse(comboBox1.Text);       // 랙함 번호
            pLoadTrayReq.row = Int32.Parse(comboBox2.Text);          // 높이

            byte[] buffer = pLoadTrayReq.Send();    // 버퍼 직렬화
            stream.Write(buffer, 0, buffer.Length);    // 직렬화된 버퍼를 송신

            WriteRichTextbox("===== Send LoadTrayReq =====");
            WriteRichTextbox($"len :        {pLoadTrayReq.len}");
            WriteRichTextbox($"protocol :   {pLoadTrayReq.protocol}");
            WriteRichTextbox($"bcc :        {pLoadTrayReq.bcc}");
            WriteRichTextbox($"handler :    {pLoadTrayReq.handler}");
            WriteRichTextbox($"column :     {pLoadTrayReq.column}");
            WriteRichTextbox($"row :        {pLoadTrayReq.row}");
        }

        // UnLoad Tray
        private void button7_Click(object sender, EventArgs e)
        {
            // 아무 입력 없으면 아무것도 안함
            if (comboBox1.Text == "" || comboBox2.Text == "" || comboBox4.Text == "") return;
            stUnloadTrayReq pUnloadTrayReq = new stUnloadTrayReq();

            pUnloadTrayReq.handler = Int32.Parse(comboBox4.Text);      // 핸들 종류
            pUnloadTrayReq.column = Int32.Parse(comboBox1.Text);       // 랙함 번호
            pUnloadTrayReq.row = Int32.Parse(comboBox2.Text);          // 높이

            byte[] buffer = pUnloadTrayReq.Send();    // 버퍼 직렬화
            stream.Write(buffer, 0, buffer.Length);    // 직렬화된 버퍼를 송신

            WriteRichTextbox("===== Send LoadTrayReq =====");
            WriteRichTextbox($"len :        {pUnloadTrayReq.len}");
            WriteRichTextbox($"protocol :   {pUnloadTrayReq.protocol}");
            WriteRichTextbox($"bcc :        {pUnloadTrayReq.bcc}");
            WriteRichTextbox($"handler :    {pUnloadTrayReq.handler}");
            WriteRichTextbox($"column :     {pUnloadTrayReq.column}");
            WriteRichTextbox($"row :        {pUnloadTrayReq.row}");
        }

        // Enterance Load Tray
        private void button9_Click(object sender, EventArgs e)
        {

        }

        // Enterance Unload Tray
        private void button10_Click(object sender, EventArgs e)
        {

        }        

        // All Tray Check
        private void button14_Click(object sender, EventArgs e)
        {

        }
    }
}
