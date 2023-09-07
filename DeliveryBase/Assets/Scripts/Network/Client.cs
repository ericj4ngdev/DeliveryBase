using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using System.Text;


public class Client : MonoBehaviour
{
    public TMP_InputField IpInput;
    public TMP_InputField PortInput;

    public bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    
    DataPacket packet = new DataPacket();
    
    // 서버 접속
    public void ConnectToServer()
    {
        if (socketReady) return;

        string ip = IpInput.text == "" ? "172.30.1.11": IpInput.text;
        int port = PortInput.text == "" ? 7777 : Int32.Parse(PortInput.text);

        // 소켓 생성
        try
        {
            socket = new TcpClient(ip, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;
            Debug.Log("연결 완료");
        }
        catch (Exception exception)
        {
            // 에러 출력
            Debug.Log($"소켓에러 : {exception.Message}");
        }
    }
    
    // 서버에게 보내는 함수
    void Send(string data)
    {
        if (!socketReady) return;
        
        writer.WriteLine(data);
        writer.Flush();
    }
    
    
    // SendInput의 OnSubmit 이벤트에 등록
    public void OnSendButton(InputField SendInput) 
    {
        // 모바일 고려
#if (UNITY_EDITOR || UNITY_STANDALONE)
        if (!Input.GetButtonDown("Submit")) return;
        SendInput.ActivateInputField();
#endif
        if (SendInput.text.Trim() == "") return;

        string message = SendInput.text;
        SendInput.text = "";
        Send(message);
    }
    
    // 종료하면 소켓 닫기
    void OnApplicationQuit()
    {
        CloseSocket();
    }
    void CloseSocket()
    {
        if (!socketReady) return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }
    // ===========================

    private void Start()
    {
        

    }

    
    
    void SendHB(byte[] buffer)
    {
        if (!socketReady) return;
        
        packet.len = 7;
        packet.protocol = 1001;
        packet.bcc = 1;
        
        stream.Write(buffer, 0, buffer.Length);
        Int32 len = packet.len;
        Int32 protocol = packet.protocol;
        byte bcc = packet.bcc;
        
        Debug.Log($"len : {len}");
        Debug.Log($"protocol : {protocol}");
        Debug.Log($"bcc : {bcc}");
        
        // writer.Write(buffer);
        // writer.Flush();
    }

    public void OnSendButton()
    {
        byte[] buffer = GetBytes_Bind(packet);
        SendHB(buffer);
    }
    // 패킷 사이즈 
    public const int BODY_BIND_SIZE = 20 + 20 + 4 + 100;
    public byte[] GetBytes_Bind(DataPacket packet)
    {
        byte[] btBuffer = new byte[BODY_BIND_SIZE];

        MemoryStream ms = new MemoryStream(btBuffer, true);
        BinaryWriter bw = new BinaryWriter(ms);

        /*// Name - string
        try
        {
            byte[] btName = new byte[20];
            Encoding.UTF8.GetBytes(packet.Name, 0, packet.Name.Length, btName, 0);
            bw.Write(btName);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error : {0}", ex.Message.ToString());
        }*/

        // Grade - long
        bw.Write(IPAddress.HostToNetworkOrder(packet.len));
        bw.Write(IPAddress.HostToNetworkOrder(packet.protocol));
        bw.Write(IPAddress.HostToNetworkOrder(packet.bcc));
        
        
        bw.Close();
        ms.Close();

        return btBuffer;
    }
    
    // =====================================================================================
    // 서버로부터 받은 메시지 처리
    void ReadServerMessage()
    {
        if (socketReady && stream.DataAvailable)
        {
            string data = reader.ReadLine();
            Debug.Log(data);
            // 서버로부터 받은 데이터를 처리하는 로직 추가
            // 예: "CallFuncA" 메시지를 받으면 클라이언트의 FuncA() 함수 호출
            if (data == "CallFuncA")
            {
                FuncA();
            }
        }
    }
    

    // 업데이트 함수에서 메시지 확인
    void Update()
    {
        ReadServerMessage();
    }

    // FuncA() 함수 구현
    public void FuncA()
    {
        Debug.Log("FuncA() 호출됨");
    }
    
}
