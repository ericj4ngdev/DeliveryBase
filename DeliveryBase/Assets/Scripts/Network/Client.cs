using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Text;
using UnityEditor.Sprites;


public class Client : MonoBehaviour
{
    public TMP_InputField IpInput;
    public TMP_InputField PortInput;

    public bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    
    Heartbeat pHeartbeat;
    stAddTrayRes pAddTrayRes;
    
    
    private void Start()
    {
        pHeartbeat = new Heartbeat();
        pAddTrayRes = new stAddTrayRes();
        pAddTrayRes.id = new char[32];
    }

    // 업데이트 함수에서 메시지 확인
    void Update()
    {
        RecvServerStruct();
    }


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

    public void OnSendButton()
    {
        SetHB();
        byte[] buffer = GetBytes_Bind(pHeartbeat);      // 직렬화된 버퍼
        SendHB(buffer);     // 직렬화된 버퍼를 송신
    }

    // ================================== 수신 (버퍼 -> 구조체 ) ===============================
    void RecvServerStruct()
    {
        byte[] buffer = new byte[256];
        if (socketReady && stream.DataAvailable)
        {
            // 버퍼가 비어있다면
            if (stream.Read(buffer, 0, buffer.Length) != 0)
            {
                Recv(buffer);
            }
        }
    }

    // 수신, 역직렬화(버퍼 -> 구조체)
    private Heartbeat Recv(byte[] btfuffer)
    {
        Heartbeat packet = new Heartbeat();

        MemoryStream ms = new MemoryStream(btfuffer, false);    // btfuffer -> stream 변환
        BinaryReader br = new BinaryReader(ms);                 // stream -> 직렬화

        // 직렬화된 데이터에서 멤버변수들을 가져온다. 
        int len = IPAddress.NetworkToHostOrder(br.ReadInt32());
        int protocol = IPAddress.NetworkToHostOrder(br.ReadInt32());
        int bcc = IPAddress.NetworkToHostOrder(br.ReadInt32());

        // 디버깅
        Debug.Log("======= Recv Heartbeat =======");
        Debug.Log($"len : {len}");
        Debug.Log($"protocol : {protocol}");
        Debug.Log($"bcc : {bcc}");

        br.Close();
        ms.Close();

        return packet;
    }

    // ===================================== 송신 (구조체 -> 버퍼) =================================
    #region HeartBeat 전송
    public const int BODY_BIND_SIZE = 20 + 20 + 4 + 100;        // 패킷 사이즈 

    // 직렬화
    public byte[] GetBytes_Bind(Heartbeat pHeartbeat)
    {
        byte[] btBuffer = new byte[BODY_BIND_SIZE];

        MemoryStream ms = new MemoryStream(btBuffer, true);
        BinaryWriter bw = new BinaryWriter(ms);

        // packet을 byte화 함
        bw.Write(IPAddress.HostToNetworkOrder(pHeartbeat.len));
        bw.Write(IPAddress.HostToNetworkOrder(pHeartbeat.protocol));
        bw.Write(IPAddress.HostToNetworkOrder(pHeartbeat.bcc));

        // 보낼 구조체 멤버변수 디버깅
        Debug.Log("======= Send Heartbeat =======");
        Debug.Log($"len : {pHeartbeat.len}");
        Debug.Log($"protocol : {pHeartbeat.protocol}");
        Debug.Log($"bcc : {pHeartbeat.bcc}");

        bw.Close();
        ms.Close();

        return btBuffer;
    }
    void SetHB()
    {
        pHeartbeat.len = 1;
        pHeartbeat.protocol = 1000;
        pHeartbeat.bcc = 1;
    }

    void SendHB(byte[] buffer)
    {
        if (!socketReady) return;

        stream.Write(buffer, 0, buffer.Length);
    }    
    #endregion


    #region stAddTrayRes

    public void OnSendButton_AddTrayRes()
    {
        byte[] buffer = GetBytes_Bind(pAddTrayRes);
        SendAddTrayRes(buffer);
    }
    
    void SendAddTrayRes(byte[] buffer)
    {
        if (!socketReady) return;

        stream.Write(buffer, 0, buffer.Length);
        Int32 len = pHeartbeat.len;
        Int32 protocol = pHeartbeat.protocol;
        byte bcc = pHeartbeat.bcc;
        
        Debug.Log($"len : {len}");
        Debug.Log($"protocol : {protocol}");
        Debug.Log($"bcc : {bcc}");
        
        // writer.Write(buffer);
        // writer.Flush();
    }
    
    public byte[] GetBytes_Bind(stAddTrayRes packet)
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
        
        packet.len = 7;
        packet.protocol = 1001;
        packet.id[0] = 'a';
        packet.id[1] = 'b';
        packet.id[2] = 'c';
        packet.bcc = 1;
        
        // Grade - long
        bw.Write(IPAddress.HostToNetworkOrder(packet.len));
        bw.Write(IPAddress.HostToNetworkOrder(packet.protocol));
        bw.Write(IPAddress.HostToNetworkOrder(packet.bcc));
        
        
        bw.Close();
        ms.Close();

        return btBuffer;
    }

    #endregion
        

    // ====================== 문자열 보내는 함수 =============================
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
}
