using System;
using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Text;

// 
public class Client : MonoBehaviour
{
    public TMP_InputField IpInput;
    public TMP_InputField PortInput;

    public Simulate controller;
    public bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    IPAddress[] addr;

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
        if (socketReady && stream.DataAvailable)
        {
            RecvServerStruct();            
        }
    }
    private string GetAddress()
    {
        IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
        addr = ipEntry.AddressList;
        // for (int i = 0; i < addr.Length; i++)
        // {
        //     // 디버깅 해보면 1번이 활성화된 IP이다.
        //     writeRichTextbox($"IP Address {i}: {addr[i].ToString()} ");
        // }
        return addr[1].ToString();
    }

    // 서버 접속
    public void ConnectToServer()
    {
        if (socketReady) return;

        Debug.Log("현재 IP :" + GetAddress());
        string ip = IpInput.text == "" ? GetAddress() : IpInput.text;
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
        Heartbeat pHeartbeat = new Heartbeat();
        byte[] buffer = pHeartbeat.Send();
        stream.Write(buffer, 0, buffer.Length);    // 직렬화된 버퍼를 송신
    }

    public void SendAddTrayRes(stAddTrayRes pAddTrayRes)
    {
        byte[] buffer = pAddTrayRes.Send();      // 직렬화된 버퍼
        stream.Write(buffer, 0, buffer.Length);
    }

    // ================================== 수신 (버퍼 -> 구조체 ) ===============================
    void RecvServerStruct()
    {
        byte[] buffer = new byte[256];
        // 버퍼가 비어있다면
        if (stream.Read(buffer, 0, buffer.Length) != 0)
        {
            Int32 len = BitConverter.ToInt32(buffer, 0);
            Int32 protocol = BitConverter.ToInt32(buffer, sizeof(Int32));
            switch (protocol)
            {
                case (Int32)protocolNum.stHeartbeat:
                    {
                        Heartbeat pHeartbeat = new Heartbeat();
                        pHeartbeat.Read(buffer);

                        Debug.Log("======= Recv stAddTrayReq =======");
                        Debug.Log($"len : {pHeartbeat.len}");
                        Debug.Log($"protocol : {pHeartbeat.protocol}");
                        Debug.Log($"bcc : {pHeartbeat.bcc}");
                    }
                    break;

                case (Int32)protocolNum.stAddTrayReq:
                    {
                        try
                        {
                            stAddTrayReq pAddTrayReq = new stAddTrayReq();
                            stAddTrayRes pAddTrayRes = new stAddTrayRes();
                            pAddTrayReq.Read(buffer);

                            Debug.Log("======= Recv pHeartbeat =======");
                            Debug.Log($"len : {pAddTrayReq.len}");
                            Debug.Log($"protocol : {pAddTrayReq.protocol}");
                            Debug.Log($"bcc : {pAddTrayReq.bcc}");
                            Debug.Log($"id : {new string(pAddTrayReq.id)}");
                            Debug.Log($"row : {pAddTrayReq.row}");
                            Debug.Log($"column : {pAddTrayReq.column}");
                            Debug.Log($"height : {pAddTrayReq.height}");

                            // 동기화
                            pAddTrayRes.ret = 0;
                            pAddTrayRes.id = pAddTrayReq.id;        // ?
                            pAddTrayRes.column = pAddTrayReq.column;
                            pAddTrayRes.row = pAddTrayReq.row;
                            pAddTrayRes.height = pAddTrayReq.height;

                            SendAddTrayRes(pAddTrayRes);     // 응답해서 보내기
                        }
                        catch (Exception exception)
                        {
                            // 에러 출력
                            Debug.Log($"소켓에러 : {exception.Message}");
                            pAddTrayRes.ret = 0;
                        }
                        
                    }
                    break;
                case (Int32)protocolNum.stMoveHandlerReq:
                    {
                        try
                        {
                            stMoveHandlerReq pMoveHandlerReq = new stMoveHandlerReq();
                            stMoveHandlerRes pMoveHandlerRes = new stMoveHandlerRes();
                            pMoveHandlerReq.Read(buffer);

                            Debug.Log("======= Recv pHeartbeat =======");
                            Debug.Log($"len : {pMoveHandlerReq.len}");
                            Debug.Log($"protocol : {pMoveHandlerReq.protocol}");
                            Debug.Log($"bcc : {pMoveHandlerReq.bcc}");
                            Debug.Log($"handler : {pMoveHandlerReq.handler}");
                            Debug.Log($"column : {pMoveHandlerReq.column}");
                            Debug.Log($"row : {pMoveHandlerReq.row}");

                            // 동기화
                            pMoveHandlerRes.handler = pMoveHandlerReq.handler;
                            pMoveHandlerRes.column  = pMoveHandlerReq.column;
                            pMoveHandlerRes.row     = pMoveHandlerReq.row;

                            controller.Handler = pMoveHandlerReq.handler;
                            controller.Column = pMoveHandlerReq.column;
                            controller.Row = pMoveHandlerReq.row;
                            controller.MoveHandler();

                            byte[] temp = pMoveHandlerRes.Send();      // 직렬화된 버퍼
                            stream.Write(temp, 0, temp.Length);

                        }                   
                        catch (Exception exception)
                        {
                            // 에러 출력
                            Debug.Log($"소켓에러 : {exception.Message}");
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        
    }


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
