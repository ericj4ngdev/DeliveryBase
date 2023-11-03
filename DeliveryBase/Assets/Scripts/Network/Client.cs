using System;
using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Text;
using System.Collections.Generic;
using static UnityEditor.Progress;

public class Client : MonoBehaviour
{
    public TMP_InputField IpInput;
    public TMP_InputField PortInput;

    [SerializeField] Simulate simulate;
    [SerializeField] PlacementManager placementManager;
    [SerializeField] bool socketReady;
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

        simulate = GetComponent<Simulate>();
        placementManager = GetComponent<PlacementManager>();
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


    private void SendPacket(Packet packet)
    {
        byte[] buffer = packet.Send();                  // 버퍼 직렬화
        stream.Write(buffer, 0, buffer.Length);         // 직렬화된 버퍼를 송신
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

                        Debug.Log("======= Recv pHeartbeat =======");
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

                            Debug.Log("======= Recv pAddTrayReq =======");
                            Debug.Log($"len :       {pAddTrayReq.len}");
                            Debug.Log($"protocol :  {pAddTrayReq.protocol}");
                            Debug.Log($"bcc :       {pAddTrayReq.bcc}");
                            Debug.Log($"id :        {new string(pAddTrayReq.id)}");
                            Debug.Log($"row :       {pAddTrayReq.row}");
                            Debug.Log($"column :    {pAddTrayReq.column}");
                            Debug.Log($"height :    {pAddTrayReq.height}");

                            // 동기화
                            pAddTrayRes.ret = 0;
                            pAddTrayRes.id = pAddTrayReq.id;        // ?
                            pAddTrayRes.column = pAddTrayReq.column;
                            pAddTrayRes.row = pAddTrayReq.row;
                            pAddTrayRes.height = pAddTrayReq.height;

                            // 동작
                            placementManager.rect_num = pAddTrayReq.column;
                            placementManager.rect_height = pAddTrayReq.row;
                            placementManager.percel_Size = pAddTrayReq.height;
                            
                            placementManager.AddTray();
                            placementManager.SetPercelSizebyServer();

                            SendPacket(pAddTrayRes);     // 응답해서 보내기
                        }
                        catch (Exception exception)
                        {
                            // 에러 출력
                            pAddTrayRes.ret = 1;
                            SendPacket(pAddTrayRes);     // 응답해서 보내기
                            Debug.Log($"소켓에러 : {exception.Message}");
                        }

                    }
                    break;
                case (Int32)protocolNum.stAddTrayRes:
                    break;
                case (Int32)protocolNum.stDeleteTrayReq:
                    {
                        try
                        {
                            stDeleteTrayReq pDeleteTrayReq = new stDeleteTrayReq();
                            stDeleteTrayRes pDeleteTrayRes = new stDeleteTrayRes();
                            pDeleteTrayReq.Read(buffer);

                            Debug.Log("======= Recv pDeleteTrayReq =======");
                            Debug.Log($"len :       {pDeleteTrayReq.len}");
                            Debug.Log($"protocol :  {pDeleteTrayReq.protocol}");
                            Debug.Log($"bcc :       {pDeleteTrayReq.bcc}");
                            Debug.Log($"id :        {new string(pDeleteTrayReq.id)}");
                            Debug.Log($"row :       {pDeleteTrayReq.row}");
                            Debug.Log($"column :    {pDeleteTrayReq.column}");
                            Debug.Log($"height :    {pDeleteTrayReq.height}");

                            // 동기화
                            pDeleteTrayRes.id = pDeleteTrayReq.id;        // ?
                            pDeleteTrayRes.column = pDeleteTrayReq.column;
                            pDeleteTrayRes.row = pDeleteTrayReq.row;
                            pDeleteTrayRes.height = pDeleteTrayReq.height;

                            // 동작
                            placementManager.rect_num = pDeleteTrayReq.column;
                            placementManager.rect_height = pDeleteTrayReq.row;
                            placementManager.percel_Size = pDeleteTrayReq.height;

                            placementManager.DeleteTray();

                            SendPacket(pDeleteTrayRes);     // 응답해서 보내기
                        }
                        catch (Exception exception)
                        {
                            // 에러 출력
                            Debug.Log($"소켓에러 : {exception.Message}");
                            pAddTrayRes.ret = 0;
                        }
                    }
                    break;
                case (Int32)protocolNum.stDeleteTrayRes:
                    break;
                case (Int32)protocolNum.stDeleteAllTrayReq:
                    {
                        try
                        {
                            stDeleteAllTrayReq pDeleteAllTrayReq = new stDeleteAllTrayReq();
                            stDeleteAllTrayRes pDeleteAllTrayRes = new stDeleteAllTrayRes();
                            pDeleteAllTrayReq.Read(buffer);

                            Debug.Log("======= Recv pDeleteAllTrayReq =======");
                            Debug.Log($"len :       {pDeleteAllTrayReq.len}");
                            Debug.Log($"protocol :  {pDeleteAllTrayReq.protocol}");
                            Debug.Log($"bcc :       {pDeleteAllTrayReq.bcc}");

                            placementManager.DeleteAllTray();

                            SendPacket(pDeleteAllTrayRes);     // 응답해서 보내기
                        }
                        catch (Exception exception)
                        {
                            // 에러 출력
                            Debug.Log($"소켓에러 : {exception.Message}");
                            pAddTrayRes.ret = 0;
                        }
                    }
                    break;
                case (Int32)protocolNum.stDeleteAllTrayRes:
                    break;
                case (Int32)protocolNum.stMoveHandlerReq:
                    {
                        try
                        {
                            stMoveHandlerReq pMoveHandlerReq = new stMoveHandlerReq();
                            stMoveHandlerRes pMoveHandlerRes = new stMoveHandlerRes();
                            pMoveHandlerReq.Read(buffer);

                            Debug.Log("======= Recv pMoveHandlerReq =======");
                            Debug.Log($"len : {pMoveHandlerReq.len}");
                            Debug.Log($"protocol : {pMoveHandlerReq.protocol}");
                            Debug.Log($"bcc : {pMoveHandlerReq.bcc}");
                            Debug.Log($"handler : {pMoveHandlerReq.handler}");
                            Debug.Log($"column : {pMoveHandlerReq.column}");
                            Debug.Log($"row : {pMoveHandlerReq.row}");

                            // 동기화
                            // pMoveHandlerRes.handler = pMoveHandlerReq.handler;
                            // pMoveHandlerRes.column = pMoveHandlerReq.column;
                            // pMoveHandlerRes.row = pMoveHandlerReq.row;

                            simulate.Handler = pMoveHandlerReq.handler;
                            simulate.Column = pMoveHandlerReq.column;
                            simulate.Row = pMoveHandlerReq.row;
                            simulate.MoveHandler();                     // 구동

                            SendPacket(pMoveHandlerRes);
                        }
                        catch (Exception exception)
                        {
                            // 에러 출력
                            Debug.Log($"소켓에러 : {exception.Message}");
                        }
                    }                    
                    break;
                case (Int32)protocolNum.stMoveHandlerRes:
                    break;
                case (Int32)protocolNum.stMoveHandlerCompleteNotify:
                    break;
                case (Int32)protocolNum.stMoveHandlerCompleteRes:
                    break;
                case (Int32)protocolNum.stLoadTrayReq:
                    {
                        try
                        {
                            stLoadTrayReq pLoadTrayReq = new stLoadTrayReq();
                            stLoadTrayRes pLoadTrayRes = new stLoadTrayRes();
                            pLoadTrayReq.Read(buffer);

                            Debug.Log("======= Recv pLoadTrayReq =======");
                            Debug.Log($"len :       {pLoadTrayReq.len}");
                            Debug.Log($"protocol :  {pLoadTrayReq.protocol}");
                            Debug.Log($"bcc :       {pLoadTrayReq.bcc}");
                            Debug.Log($"handler :   {pLoadTrayReq.handler}");
                            Debug.Log($"column :    {pLoadTrayReq.column}");
                            Debug.Log($"row :       {pLoadTrayReq.row}");

                            simulate.Handler = pLoadTrayReq.handler;
                            simulate.Column = pLoadTrayReq.column;
                            simulate.Row = pLoadTrayReq.row;
                            simulate.MoveAndLoadTray();                   // 구동

                            SendPacket(pLoadTrayRes);
                        }
                        catch (Exception exception)
                        {
                            // 에러 출력
                            Debug.Log($"소켓에러 : {exception.Message}");
                        }
                    }                    
                    break;
                case (Int32)protocolNum.stLoadTrayRes:
                    break;
                case (Int32)protocolNum.stLoadTrayCompleteNotify:
                    break;
                case (Int32)protocolNum.stLoadTrayCompleteRes:
                    break;
                case (Int32)protocolNum.stUnloadTrayReq:
                    {
                        try
                        {
                            stUnloadTrayReq pUnloadTrayReq = new stUnloadTrayReq();
                            stUnloadTrayRes pUnloadTrayRes = new stUnloadTrayRes();
                            pUnloadTrayReq.Read(buffer);

                            Debug.Log("======= Recv pUnloadTrayReq =======");
                            Debug.Log($"len :       {pUnloadTrayReq.len}");
                            Debug.Log($"protocol :  {pUnloadTrayReq.protocol}");
                            Debug.Log($"bcc :       {pUnloadTrayReq.bcc}");
                            Debug.Log($"handler :   {pUnloadTrayReq.handler}");
                            Debug.Log($"column :    {pUnloadTrayReq.column}");
                            Debug.Log($"row :       {pUnloadTrayReq.row}");

                            simulate.Handler = pUnloadTrayReq.handler;
                            simulate.Column = pUnloadTrayReq.column;
                            simulate.Row = pUnloadTrayReq.row;
                            simulate.MoveAndUnloadTray();                   // 구동

                            SendPacket(pUnloadTrayRes);
                        }
                        catch (Exception exception)
                        {
                            // 에러 출력
                            Debug.Log($"소켓에러 : {exception.Message}");
                        }
                    }
                    break;
                case (Int32)protocolNum.stUnloadTrayRes:
                    break;
                case (Int32)protocolNum.stUnloadTrayCompleteNotify:
                    break;
                case (Int32)protocolNum.stUnloadTrayCompleteRes:
                    break;
                case (Int32)protocolNum.stEnteranceLoadTrayReq:
                    {
                        stEnteranceLoadTrayReq pEnteranceLoadTrayReq = new stEnteranceLoadTrayReq();
                        stEnteranceLoadTrayRes pEnteranceLoadTrayRes = new stEnteranceLoadTrayRes();
                        pEnteranceLoadTrayReq.Read(buffer);

                        Debug.Log("======= Recv pEnteranceLoadTrayReq =======");
                        Debug.Log($"len :       {pEnteranceLoadTrayReq.len}");
                        Debug.Log($"protocol :  {pEnteranceLoadTrayReq.protocol}");
                        Debug.Log($"bcc :       {pEnteranceLoadTrayReq.bcc}");
                        Debug.Log($"handler :   {pEnteranceLoadTrayReq.handler}");
                        Debug.Log($"column :    {pEnteranceLoadTrayReq.column}");
                        Debug.Log($"row :       {pEnteranceLoadTrayReq.row}");

                        simulate.Handler = pEnteranceLoadTrayReq.handler;
                        simulate.Column = pEnteranceLoadTrayReq.column;
                        simulate.Row = pEnteranceLoadTrayReq.row;
                        simulate.EntranceLoadTray();                   // 구동

                        SendPacket(pEnteranceLoadTrayRes);
                    }
                    break;
                case (Int32)protocolNum.stEnteranceLoadTrayRes:
                    break;
                case (Int32)protocolNum.stEnteranceLoadTrayCompleteNotify:
                    {
                        // 핸들의 상태에 따라 전송하기
                        // 핸들이 특정상태가 되면 




                    }
                    break;
                case (Int32)protocolNum.stEnteranceLoadTrayCompleteRes:
                    break;
                case (Int32)protocolNum.stEnteranceUnloadTrayReq:
                    {
                        stEnteranceUnloadTrayReq pEnteranceUnloadTrayReq = new stEnteranceUnloadTrayReq();
                        stEnteranceUnloadTrayRes pEnteranceUnloadTrayRes = new stEnteranceUnloadTrayRes();
                        pEnteranceUnloadTrayReq.Read(buffer);

                        Debug.Log("======= Recv pEnteranceUnloadTrayReq =======");
                        Debug.Log($"len :       {pEnteranceUnloadTrayReq.len}");
                        Debug.Log($"protocol :  {pEnteranceUnloadTrayReq.protocol}");
                        Debug.Log($"bcc :       {pEnteranceUnloadTrayReq.bcc}");
                        Debug.Log($"handler :   {pEnteranceUnloadTrayReq.handler}");

                        simulate.Handler = pEnteranceUnloadTrayReq.handler;
                        simulate.EntranceUnloadTray();                   // 구동

                        SendPacket(pEnteranceUnloadTrayRes);
                    }
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
                    {
                        stAddEnteranceParcelReq pAddEnteranceParcelReq = new stAddEnteranceParcelReq();
                        stAddEnteranceParcelRes pAddEnteranceParcelRes = new stAddEnteranceParcelRes();
                        pAddEnteranceParcelReq.Read(buffer);

                        Debug.Log("======= Recv pEnteranceUnloadTrayReq =======");
                        Debug.Log($"len :       {pAddEnteranceParcelReq.len}");
                        Debug.Log($"protocol :  {pAddEnteranceParcelReq.protocol}");
                        Debug.Log($"bcc :       {pAddEnteranceParcelReq.bcc}");
                        Debug.Log($"trackingNum :{new string(pAddEnteranceParcelReq.trackingNum)}");
                        Debug.Log($"column :    {pAddEnteranceParcelReq.column}");
                        Debug.Log($"row :       {pAddEnteranceParcelReq.row}");
                        Debug.Log($"height :    {pAddEnteranceParcelReq.height}");

                        placementManager.rect_num = pAddEnteranceParcelReq.column;
                        placementManager.rect_height = pAddEnteranceParcelReq.row;
                        placementManager.percel_Size = pAddEnteranceParcelReq.height;

                        placementManager.AddPercelOnEntrance();
                        placementManager.SetPercelSizebyServer();

                        SendPacket(pAddEnteranceParcelRes);
                    }
                    break;
                case (Int32)protocolNum.stAddEnteranceParcelRes:
                    break;
                case (Int32)protocolNum.stDeleteEnteranceParcelReq:
                    {
                        stDeleteEnteranceParcelReq pDeleteEnteranceParcelReq = new stDeleteEnteranceParcelReq();
                        stDeleteEnteranceParcelRes pDeleteEnteranceParcelRes = new stDeleteEnteranceParcelRes();
                        pDeleteEnteranceParcelReq.Read(buffer);

                        Debug.Log("======= Recv pEnteranceUnloadTrayReq =======");
                        Debug.Log($"len :       {pDeleteEnteranceParcelReq.len}");
                        Debug.Log($"protocol :  {pDeleteEnteranceParcelReq.protocol}");
                        Debug.Log($"bcc :       {pDeleteEnteranceParcelReq.bcc}");
                        Debug.Log($"column :    {pDeleteEnteranceParcelReq.column}");
                        Debug.Log($"row :       {pDeleteEnteranceParcelReq.row}");

                        placementManager.DeletePercelOnEntrance();

                        SendPacket(pDeleteEnteranceParcelRes);
                    }
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
                    {
                        stAllParcelCheckReq pAllParcelCheckReq = new stAllParcelCheckReq();
                        
                        pAllParcelCheckReq.Read(buffer);

                        Debug.Log("======= Recv AllParcelCheckReq =======");
                        Debug.Log($"len :       {pAllParcelCheckReq.len}");
                        Debug.Log($"protocol :  {pAllParcelCheckReq.protocol}");
                        Debug.Log($"bcc :       {pAllParcelCheckReq.bcc}");

                        Tray[] trays = FindObjectsOfType<Tray>();
                        stAllParcelCheckRes[] packets = new stAllParcelCheckRes[trays.Length];

                        for (int i = 0; i < trays.Length; i++)
                        {
                            packets[i] = trays[i].GetTrayInfo();
                            SendPacket(packets[i]);
                        }                   
                    }
                    break;
                case (Int32)protocolNum.stAllParcelCheckRes:
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
    // publicl void fnuc() { }
}
