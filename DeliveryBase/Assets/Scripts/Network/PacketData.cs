using System;

[Serializable]
public struct Heartbeat
{
    public Int32 len;
    public Int32 protocol;
    public char[] id;               // = tray ID
    public byte bcc;
}

[Serializable]
public struct stAddTrayReq
{
    public Int32 len;
    public Int32 protocol;
    public char[] id;               // = tray ID
    public Int32 column;            // = tray 열 번호
    public Int32 row;               // tray 행 번호
    public Int32 height;           // 점유량 (기본 1 : 물건이 있을 경우 2 이상의 값)
    public byte bcc;
}

[Serializable]
public struct stAddTrayRes
{
    public Int32 len;
    public Int32 protocol;
    public Int32 ret;               //  0 : 성공 , 그외 데이터 실패 코드 (ErrorCode)
    public char[] id;
    public Int32 column;            // = tray 열 번호
    public Int32 row;               // tray 행 번호
    public Int32 height;           // 점유량 (기본 1 : 물건이 있을 경우 2 이상의 값)
    public byte bcc;
}