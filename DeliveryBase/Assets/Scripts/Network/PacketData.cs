using System;
using System.IO;

public enum protocolNum
{
    stHeartbeat                         = 2001,
    stAddTrayReq                        = 1101,
    stAddTrayRes                        = 1102,
    stDeleteTrayReq                     = 1103,
    stDeleteTrayRes                     = 1104,
    stDeleteAllTrayReq                  = 1105,
    stDeleteAllTrayRes                  = 1106,
    stMoveHandlerReq                    = 1107,
    stMoveHandlerRes                    = 1108,
    stMoveHandlerCompleteNotify         = 2109,
    stMoveHandlerCompleteRes            = 2110,
    stLoadTrayReq                       = 1111,
    stLoadTrayRes                       = 1112,
    stLoadTrayCompleteNotify            = 2113,
    stLoadTrayCompleteRes               = 2114,
    stUnloadTrayReq                     = 1115,      // simul
    stUnloadTrayRes                     = 1116,      // simul
    stUnloadTrayCompleteNotify          = 2117,
    stUnloadTrayCompleteRes             = 2118,
    stEnteranceLoadTrayReq              = 1119,      // simul
    stEnteranceLoadTrayRes              = 1120,      // simul
    stEnteranceLoadTrayCompleteNotify   = 2121,
    stEnteranceLoadTrayCompleteRes      = 2122,
    stEnteranceUnloadTrayReq            = 1123,
    stEnteranceUnloadTrayRes            = 1124,
    stEnteranceUnloadTrayCompleteNotify = 2125,
    stEnteranceUnloadTrayCompleteRes    = 2126,
    stGateLoadTrayReq                   = 1127,        // simul
    stGateLoadTrayRes                   = 1128,        // simul, 원래 stEnteranceLoadTrayRes
    stGateLoadTrayCompleteNotify        = 2129,
    stGateLoadTrayCompleteRes           = 2130,
    stGateUnloadTrayReq                 = 1131,
    stGateUnloadTrayRes                 = 1132,
    stGateUnloadTrayCompleteNotify      = 2133,
    stGateUnloadTrayCompleteRes         = 2134,
    stAddEnteranceParcelReq             = 2135,     // simul
    stAddEnteranceParcelRes             = 2136,     // simul
    stDeleteEnteranceParcelReq          = 2137,     // simul
    stDeleteEnteranceParcelRes          = 2138,     // simul
    stAddGateParcelReq                  = 2139,
    stAddGateParcelRes                  = 2140,
    stDeleteGateParcelReq               = 2141,
    stDeleteGateParcelRes               = 2142,     // 숫자 오타
    stAllParcelCheckReq                 = 1141,
    stAllParcelCheckRes                 = 1142
}

[Serializable]
public abstract class Packet
{
    public Int32 len;
    public Int32 protocol;
    public byte bcc;
    public abstract void Read(byte[] btfuffer);      // 수신
    public abstract byte[] Send();     // 송신
    protected void CalculateLen()
    {
        len = sizeof(Int32) * 2 + sizeof(byte); // 기본 필드 길이 (len, protocol, bcc)

        // 추가 멤버 변수의 크기를 더합니다.
        len += CalculateAdditionalLen();
    }
    // 파생 클래스에서 추가 멤버 변수의 길이를 계산하도록 추상 메서드 정의
    protected abstract int CalculateAdditionalLen();
}

[Serializable]
public class Heartbeat : Packet
{
    public Heartbeat()
    {
        this.protocol = (Int32)protocolNum.stHeartbeat;
        this.bcc = 1;
    }

    public override void Read(byte[] btfuffer)
    {
        Int32 len = BitConverter.ToInt32(btfuffer, 0);
        Int32 protocol = BitConverter.ToInt32(btfuffer, sizeof(Int32));
        byte bcc = btfuffer[sizeof(Int32) * 2];
    }

    public override byte[] Send()
    { 
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream(sizeof(Int32) * 2 + sizeof(byte)))
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }
    
    // Heartbeat 클래스에 추가된 멤버 변수의 크기를 여기에서 더합니다.
    protected override int CalculateAdditionalLen()
    {
        return 0; // 추가 멤버 변수가 없으므로 0 반환
    }
}

[Serializable]
public class stAddTrayReq : Packet
{
    public char[] id;               // = tray ID
    public char[] trackingNum;      // parcel tracking Num
    public Int32 column;            // = tray 열 번호
    public Int32 row;               // tray 행 번호
    public Int32 height;            // 점유량 (기본 1 : 물건이 있을 경우 2 이상의 값)

    public stAddTrayReq()
    {
        this.protocol = (Int32)protocolNum.stAddTrayReq;
        this.bcc = 1;
        this.id = new char[32];
        this.trackingNum = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // id 배열 읽기
        int idLength = sizeof(char) * 32;
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // trackingNum 배열 읽기
        int trackingNumLength = sizeof(char) * 32;
        this.trackingNum = new char[trackingNumLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.trackingNum, 0, trackingNumLength);
        offset += trackingNumLength;

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // height 필드 읽기
        this.height = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);

        byte[] trakcingBytes = new byte[this.trackingNum.Length * sizeof(char)];
        Buffer.BlockCopy(this.trackingNum, 0, trakcingBytes, 0, trakcingBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(idBytes);
            writer.Write(trakcingBytes);
            writer.Write(this.column);
            writer.Write(this.row);
            writer.Write(this.height);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char) + this.trackingNum.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 3; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stAddTrayRes : Packet
{
    public Int32 ret;               //  0 : 성공 , 그외 데이터 실패 코드 (ErrorCode)
    public char[] id;
    public char[] trackingNum;      // parcel tracking Num (Default = "")
    public Int32 column;            // = tray 열 번호
    public Int32 row;               // tray 행 번호
    public Int32 height;           // 점유량 (기본 1 : 물건이 있을 경우 2 이상의 값)

    public stAddTrayRes()
    {
        this.protocol = (Int32)protocolNum.stAddTrayRes;
        this.bcc = 1;
        this.id = new char[32];
        this.trackingNum = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // ret 필드 읽기
        this.ret = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // id 배열 읽기
        int idLength = sizeof(char) * 32; //this.len - (sizeof(Int32) * 3);
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // trackingNum 배열 읽기
        int trackingNumLength = sizeof(char) * 32;
        this.trackingNum = new char[trackingNumLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.trackingNum, 0, trackingNumLength);
        offset += trackingNumLength;

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // height 필드 읽기
        this.height = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);

        byte[] trakcingBytes = new byte[this.trackingNum.Length * sizeof(char)];
        Buffer.BlockCopy(this.trackingNum, 0, trakcingBytes, 0, trakcingBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(this.ret);
            writer.Write(idBytes);
            writer.Write(trakcingBytes);
            writer.Write(this.column);
            writer.Write(this.row);
            writer.Write(this.height);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char) + this.trackingNum.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 4; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stDeleteTrayReq : Packet
{
    public char[] id;               // = tray ID
    public Int32 column;            // = tray 열 번호
    public Int32 row;               // tray 행 번호
    public Int32 height;            // 점유량 (기본 1 : 물건이 있을 경우 2 이상의 값)

    public stDeleteTrayReq()
    {
        this.protocol = (Int32)protocolNum.stDeleteTrayReq;
        this.bcc = 1;
        this.id = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // id 배열 읽기
        int idLength = sizeof(char) * 32;
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // height 필드 읽기
        this.height = BitConverter.ToInt32(buffer, offset);
    }


    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(idBytes);
            writer.Write(this.column);
            writer.Write(this.row);
            writer.Write(this.height);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 3; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stDeleteTrayRes : Packet
{
    public char[] id;               // = tray ID
    public char[] trackingNum;      // parcel tracking Num (Default = "")
    public Int32 column;            // = tray 열 번호
    public Int32 row;               // tray 행 번호
    public Int32 height;            // 점유량 (기본 1 : 물건이 있을 경우 2 이상의 값)

    public stDeleteTrayRes()
    {
        this.protocol = (Int32)protocolNum.stDeleteTrayRes;
        this.bcc = 1;
        this.id = new char[32];
        this.trackingNum = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // id 배열 읽기
        int idLength = sizeof(char) * 32;
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // trackingNum 배열 읽기
        int trackingNumLength = sizeof(char) * 32;
        this.trackingNum = new char[trackingNumLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.trackingNum, 0, trackingNumLength);
        offset += trackingNumLength;

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // height 필드 읽기
        this.height = BitConverter.ToInt32(buffer, offset);
    }


    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);

        byte[] trakcingBytes = new byte[this.trackingNum.Length * sizeof(char)];
        Buffer.BlockCopy(this.trackingNum, 0, trakcingBytes, 0, trakcingBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(idBytes);
            writer.Write(trakcingBytes);
            writer.Write(this.column);
            writer.Write(this.row);
            writer.Write(this.height);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char) + this.trackingNum.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 3; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stDeleteAllTrayReq : Packet
{
    public stDeleteAllTrayReq()
    {
        this.protocol = (Int32)protocolNum.stDeleteAllTrayReq;
        this.bcc = 1;
    }

    public override void Read(byte[] btfuffer)
    {
        Int32 len = BitConverter.ToInt32(btfuffer, 0);
        Int32 protocol = BitConverter.ToInt32(btfuffer, sizeof(Int32));
        byte bcc = btfuffer[sizeof(Int32) * 2];
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream(sizeof(Int32) * 2 + sizeof(byte)))
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }

    // Heartbeat 클래스에 추가된 멤버 변수의 크기를 여기에서 더합니다.
    protected override int CalculateAdditionalLen()
    {
        return 0; // 추가 멤버 변수가 없으므로 0 반환
    }
}

[Serializable]
public class stDeleteAllTrayRes : Packet
{
    public stDeleteAllTrayRes()
    {
        this.protocol = (Int32)protocolNum.stDeleteAllTrayRes;
        this.bcc = 1;
    }

    public override void Read(byte[] btfuffer)
    {
        Int32 len = BitConverter.ToInt32(btfuffer, 0);
        Int32 protocol = BitConverter.ToInt32(btfuffer, sizeof(Int32));
        byte bcc = btfuffer[sizeof(Int32) * 2];
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream(sizeof(Int32) * 2 + sizeof(byte)))
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }

    // Heartbeat 클래스에 추가된 멤버 변수의 크기를 여기에서 더합니다.
    protected override int CalculateAdditionalLen()
    {
        return 0; // 추가 멤버 변수가 없으므로 0 반환
    }
}

[Serializable]
public class stMoveHandlerReq : Packet
{
    public Int32 handler;           // 점유량 (기본 1 : 물건이 있을 경우 2 이상의 값)
    public Int32 column;            // tray 열 번호(랙함)
    public Int32 row;               // tray 행 번호

    public stMoveHandlerReq()
    {
        this.protocol = (Int32)protocolNum.stMoveHandlerReq;
        this.bcc = 1;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // handler 필드 읽기
        this.handler = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(this.handler);
            writer.Write(this.column);
            writer.Write(this.row);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = sizeof(Int32) * 3;
        return additionalLen;
    }
}
//
[Serializable]
public class stMoveHandlerRes : Packet
{
    public stMoveHandlerRes()
    {
        this.protocol = (Int32)protocolNum.stMoveHandlerRes;
        this.bcc = 1;
    }

    public override void Read(byte[] btfuffer)
    {
        Int32 len = BitConverter.ToInt32(btfuffer, 0);
        Int32 protocol = BitConverter.ToInt32(btfuffer, sizeof(Int32));
        byte bcc = btfuffer[sizeof(Int32) * 2];
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream(sizeof(Int32) * 2 + sizeof(byte)))
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        return 0;
    }
}

[Serializable]
public class stMoveHandlerCompleteNotify : Packet
{
    public Int32 result;
    public Int32 handler;           
    public Int32 column;            
    public Int32 row;               

    public stMoveHandlerCompleteNotify()
    {
        this.protocol = (Int32)protocolNum.stMoveHandlerCompleteNotify;
        this.bcc = 1;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // result 필드 읽기
        this.result = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // handler 필드 읽기
        this.handler = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(this.result);
            writer.Write(this.handler);
            writer.Write(this.column);
            writer.Write(this.row);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = sizeof(Int32) * 4;
        return additionalLen;
    }
}

[Serializable]
public class stMoveHandlerCompleteRes : Packet
{
    public stMoveHandlerCompleteRes()
    {
        this.protocol = (Int32)protocolNum.stMoveHandlerCompleteRes;
        this.bcc = 1;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        return 0;
    }
}

[Serializable]
public class stLoadTrayReq : Packet
{
    public char[] id;               // = tray ID
    public char[] trackingNum;      // parcel tracking Num
    public Int32 handler;           // 점유량 (기본 1 : 물건이 있을 경우 2 이상의 값)
    public Int32 column;            // = tray 열 번호
    public Int32 row;               // tray 행 번호

    public stLoadTrayReq()
    {
        this.protocol = (Int32)protocolNum.stLoadTrayReq;
        this.bcc = 1;
        this.id = new char[32];
        this.trackingNum = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // id 배열 읽기
        int idLength = sizeof(char) * 32; //this.len - (sizeof(Int32) * 3);
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // trackingNum 배열 읽기
        int trackingNumLength = sizeof(char) * 32;
        this.trackingNum = new char[trackingNumLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.trackingNum, 0, trackingNumLength);
        offset += trackingNumLength;

        // handler 필드 읽기
        this.handler = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);

        byte[] trakcingBytes = new byte[this.trackingNum.Length * sizeof(char)];
        Buffer.BlockCopy(this.trackingNum, 0, trakcingBytes, 0, trakcingBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(idBytes);
            writer.Write(trakcingBytes);
            writer.Write(this.handler);
            writer.Write(this.column);
            writer.Write(this.row);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char) + this.trackingNum.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 3; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stLoadTrayRes : Packet
{ 
    public stLoadTrayRes()
    {
        this.protocol = (Int32)protocolNum.stLoadTrayRes;
        this.bcc = 1;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream(len))
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        return 0;
    }
}

[Serializable]
public class stLoadTrayCompleteNotify : Packet
{
    public Int32 result;           // 점유량 (기본 1 : 물건이 있을 경우 2 이상의 값)
    public char[] id;               // = tray ID
    public char[] trackingNum;      // parcel tracking Num
    public Int32 column;            // = tray 열 번호
    public Int32 row;               // tray 행 번호

    public stLoadTrayCompleteNotify()
    {
        this.protocol = (Int32)protocolNum.stLoadTrayCompleteNotify;
        this.bcc = 1;
        this.id = new char[32];
        this.trackingNum = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // result 필드 읽기
        this.result = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // id 배열 읽기
        int idLength = sizeof(char) * 32;
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;
        
        // trackingNum 배열 읽기
        int trackingNumLength = sizeof(char) * 32;
        this.trackingNum = new char[trackingNumLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.trackingNum, 0, trackingNumLength);
        offset += trackingNumLength;

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);       
    }


    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);
        
        byte[] trakcingBytes = new byte[this.trackingNum.Length * sizeof(char)];
        Buffer.BlockCopy(this.trackingNum, 0, trakcingBytes, 0, trakcingBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(this.result);
            writer.Write(idBytes);
            writer.Write(trakcingBytes);
            writer.Write(this.column);
            writer.Write(this.row);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char) + this.trackingNum.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 3; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stLoadTrayCompleteRes : Packet
{
    public stLoadTrayCompleteRes()
    {
        this.protocol = (Int32)protocolNum.stLoadTrayCompleteRes;
        this.bcc = 1;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        return 0;
    }
}

[Serializable]
public class stUnloadTrayReq : Packet
{
    public char[] id;               // = tray ID
    public char[] trackingNum;      // parcel tracking Num
    public Int32 handler;           // 점유량 (기본 1 : 물건이 있을 경우 2 이상의 값)
    public Int32 column;            // = tray 열 번호
    public Int32 row;               // tray 행 번호

    public stUnloadTrayReq()
    {
        this.protocol = (Int32)protocolNum.stUnloadTrayReq;
        this.bcc = 1;
        this.id = new char[32];
        this.trackingNum = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // id 배열 읽기
        int idLength = sizeof(char) * 32; //this.len - (sizeof(Int32) * 3);
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;
        
        // trackingNum 배열 읽기
        int trackingNumLength = sizeof(char) * 32;
        this.trackingNum = new char[trackingNumLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.trackingNum, 0, trackingNumLength);
        offset += trackingNumLength;

        // handler 필드 읽기
        this.handler = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);
        byte[] trakcingBytes = new byte[this.trackingNum.Length * sizeof(char)];
        Buffer.BlockCopy(this.trackingNum, 0, trakcingBytes, 0, trakcingBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(idBytes);
            writer.Write(trakcingBytes);
            writer.Write(this.handler);
            writer.Write(this.column);
            writer.Write(this.row);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char) + this.trackingNum.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 3; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stUnloadTrayRes : Packet
{
    public stUnloadTrayRes()
    {
        this.protocol = (Int32)protocolNum.stUnloadTrayRes;
        this.bcc = 1;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        return 0;
    }
}

[Serializable]
public class stUnloadTrayCompleteNotify : Packet
{
    public Int32 result;           // 점유량 (기본 1 : 물건이 있을 경우 2 이상의 값)
    public char[] id;               // = tray ID
    public char[] trackingNum;      // parcel tracking Num
    public Int32 column;            // = tray 열 번호
    public Int32 row;               // tray 행 번호

    public stUnloadTrayCompleteNotify()
    {
        this.protocol = (Int32)protocolNum.stUnloadTrayCompleteNotify;
        this.bcc = 1;
        this.id = new char[32];
        this.trackingNum = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // result 필드 읽기
        this.result = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // id 배열 읽기
        int idLength = sizeof(char) * 32; 
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // trackingNum 배열 읽기
        int trackingNumLength = sizeof(char) * 32;
        this.trackingNum = new char[trackingNumLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.trackingNum, 0, trackingNumLength);
        offset += trackingNumLength;

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
    }


    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);
        byte[] trakcingBytes = new byte[this.trackingNum.Length * sizeof(char)];
        Buffer.BlockCopy(this.trackingNum, 0, trakcingBytes, 0, trakcingBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(this.result);
            writer.Write(idBytes);
            writer.Write(trakcingBytes);
            writer.Write(this.column);
            writer.Write(this.row);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char) + this.trackingNum.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 3; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stUnloadTrayCompleteRes : Packet
{
    public stUnloadTrayCompleteRes()
    {
        this.protocol = (Int32)protocolNum.stUnloadTrayCompleteRes;
        this.bcc = 1;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        return 0;
    }
}

[Serializable]
public class stEnteranceLoadTrayReq : Packet
{
    public char[] id;               // tray ID
    public char[] trackingNum;      // parcel tracking Num
    public Int32 handler;           // 점유량 (기본 1 : 물건이 있을 경우 2 이상의 값)
    public Int32 column;            // tray 열 번호
    public Int32 row;               // tray 행 번호

    public stEnteranceLoadTrayReq()
    {
        this.protocol = (Int32)protocolNum.stEnteranceLoadTrayReq;
        this.bcc = 1;
        this.id = new char[32];
        this.trackingNum = new char[32];
        this.column = 10;
        this.row = 9;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // id 배열 읽기
        int idLength = sizeof(char) * 32; //this.len - (sizeof(Int32) * 3);
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // trackingNum 배열 읽기
        int trackingNumLength = sizeof(char) * 32;
        this.trackingNum = new char[trackingNumLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.trackingNum, 0, trackingNumLength);
        offset += trackingNumLength;

        // handler 필드 읽기
        this.handler = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);
        byte[] trakcingBytes = new byte[this.trackingNum.Length * sizeof(char)];
        Buffer.BlockCopy(this.trackingNum, 0, trakcingBytes, 0, trakcingBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(idBytes);
            writer.Write(trakcingBytes);
            writer.Write(this.handler);
            writer.Write(this.column);
            writer.Write(this.row);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char) + this.trackingNum.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 3; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stEnteranceLoadTrayRes : Packet
{
    public stEnteranceLoadTrayRes()
    {
        this.protocol = (Int32)protocolNum.stEnteranceLoadTrayRes;
        this.bcc = 1;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        return 0;
    }
}

[Serializable]
public class stEnteranceLoadTrayCompleteNotify : Packet
{
    public Int32 result;            // 0:성공, 그외 ErrorCode
    public char[] id;               // = tray ID
    public char[] trackingNum;      // parcel tracking Num
    public Int32 handler;           // 점유량 (기본 1 : 물건이 있을 경우 2 이상의 값)

    public stEnteranceLoadTrayCompleteNotify()
    {
        this.protocol = (Int32)protocolNum.stEnteranceLoadTrayCompleteNotify;
        this.bcc = 1;
        this.id = new char[32];
        this.trackingNum = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // result 필드 읽기
        this.result = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // id 배열 읽기
        int idLength = sizeof(char) * 32; //this.len - (sizeof(Int32) * 3);
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // trackingNum 배열 읽기
        int trackingNumLength = sizeof(char) * 32;
        this.trackingNum = new char[trackingNumLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.trackingNum, 0, trackingNumLength);
        offset += trackingNumLength;

        // handler 필드 읽기
        this.handler = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(this.result);
            writer.Write(idBytes);
            writer.Write(this.handler);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 2; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stEnteranceLoadTrayCompleteRes : Packet
{
    public stEnteranceLoadTrayCompleteRes()
    {
        this.protocol = (Int32)protocolNum.stEnteranceLoadTrayCompleteRes;
        this.bcc = 1;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        return 0;
    }
}

[Serializable]
public class stEnteranceUnloadTrayReq : Packet
{
    public Int32 handler;           // 점유량 (기본 1 : 물건이 있을 경우 2 이상의 값)
    public char[] id;               // = tray ID
    public char[] trackingNum;      // parcel tracking Num

    public stEnteranceUnloadTrayReq()
    {
        this.protocol = (Int32)protocolNum.stEnteranceUnloadTrayReq;
        this.bcc = 1;
        this.id = new char[32];
        this.trackingNum = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);        

        // id 배열 읽기
        int idLength = sizeof(char) * 32; //this.len - (sizeof(Int32) * 3);
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // trackingNum 배열 읽기
        int trackingNumLength = sizeof(char) * 32;
        this.trackingNum = new char[trackingNumLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.trackingNum, 0, trackingNumLength);
        offset += trackingNumLength;

        // handler 필드 읽기
        this.handler = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);
        byte[] trakcingBytes = new byte[this.trackingNum.Length * sizeof(char)];
        Buffer.BlockCopy(this.trackingNum, 0, trakcingBytes, 0, trakcingBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(idBytes);
            writer.Write(trakcingBytes);
            writer.Write(this.handler);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char) + this.trackingNum.Length * sizeof(char);
        additionalLen += sizeof(Int32); // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stEnteranceUnloadTrayRes : Packet
{
    public stEnteranceUnloadTrayRes()
    {
        this.protocol = (Int32)protocolNum.stEnteranceUnloadTrayRes;
        this.bcc = 1;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        return 0;
    }
}

[Serializable]
public class stEnteranceUnloadTrayCompleteNotify : Packet
{
    public Int32 result;            // 0:성공, 그외 ErrorCode
    public char[] id;               // = tray ID
    public char[] trackingNum;      // parcel tracking Num
    public Int32 handler;           // 점유량 (기본 1 : 물건이 있을 경우 2 이상의 값)

    public stEnteranceUnloadTrayCompleteNotify()
    {
        this.protocol = (Int32)protocolNum.stEnteranceUnloadTrayCompleteNotify;
        this.bcc = 1;
        this.id = new char[32];
        this.trackingNum = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // result 필드 읽기
        this.result = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // id 배열 읽기
        int idLength = sizeof(char) * 32; //this.len - (sizeof(Int32) * 3);
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // trackingNum 배열 읽기
        int trackingNumLength = sizeof(char) * 32;
        this.trackingNum = new char[trackingNumLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.trackingNum, 0, trackingNumLength);
        offset += trackingNumLength;

        // handler 필드 읽기
        this.handler = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);
        byte[] trakcingBytes = new byte[this.trackingNum.Length * sizeof(char)];
        Buffer.BlockCopy(this.trackingNum, 0, trakcingBytes, 0, trakcingBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(this.result);
            writer.Write(idBytes);
            writer.Write(trakcingBytes);
            writer.Write(this.handler);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char) + this.trackingNum.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 2; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stEnteranceUnloadTrayCompleteRes : Packet
{
    public stEnteranceUnloadTrayCompleteRes()
    {
        this.protocol = (Int32)protocolNum.stEnteranceUnloadTrayCompleteRes;
        this.bcc = 1;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        return 0;
    }
}

// ===================== 삭제 =========================
[Serializable]
public class stGateLoadTrayReq : Packet
{
    public char[] id;               // Tray ID
    public Int32 handler;           // 1:Left, 2:Right
    public Int32 column;            // 15 (Gate Index : 고정)
    public Int32 row;               // 0~9 Gate의 row 값

    public stGateLoadTrayReq()
    {
        this.protocol = (Int32)protocolNum.stGateLoadTrayReq;
        this.bcc = 1;
        this.id = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // id 배열 읽기
        int idLength = sizeof(char) * 32; //this.len - (sizeof(Int32) * 3);
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // handler 필드 읽기
        this.handler = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(idBytes);
            writer.Write(this.handler);
            writer.Write(this.column);
            writer.Write(this.row);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 3; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stGateLoadTrayRes : Packet
{
    public stGateLoadTrayRes()
    {
        this.protocol = (Int32)protocolNum.stGateLoadTrayRes;
        this.bcc = 1;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        return 0;
    }
}

[Serializable]
public class stGateLoadTrayCompleteNotify : Packet
{
    public Int32 result;            // 0:성공, 그외 ErrorCode
    public char[] id;               // = tray ID
    public Int32 handler;           // 1:Left, 2:Right
    public Int32 column;            // 15 (Gate Index : 고정)
    public Int32 row;               // 0 ~ 9

    public stGateLoadTrayCompleteNotify()
    {
        this.protocol = (Int32)protocolNum.stGateLoadTrayCompleteNotify;
        this.bcc = 1;
        this.id = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // result 필드 읽기
        this.result = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // id 배열 읽기
        int idLength = sizeof(char) * 32; //this.len - (sizeof(Int32) * 3);
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // handler 필드 읽기
        this.handler = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(this.result);
            writer.Write(idBytes);
            writer.Write(this.handler);
            writer.Write(this.column);
            writer.Write(this.row);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 4; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stGateLoadTrayCompleteRes : Packet
{
    public stGateLoadTrayCompleteRes()
    {
        this.protocol = (Int32)protocolNum.stGateLoadTrayCompleteRes;
        this.bcc = 1;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        return 0;
    }
}

[Serializable]
public class stGateUnloadTrayReq : Packet
{
    public char[] id;               // = tray ID
    public Int32 handler;           // 0:Client 판단, 1:Left, 2:Right
    public Int32 column;            // 15 (Gate Index : 고정)
    public Int32 row;               // 0 ~ 9

    public stGateUnloadTrayReq()
    {
        this.protocol = (Int32)protocolNum.stGateUnloadTrayReq;
        this.bcc = 1;
        this.id = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // id 배열 읽기
        int idLength = sizeof(char) * 32; //this.len - (sizeof(Int32) * 3);
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // handler 필드 읽기
        this.handler = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(idBytes);
            writer.Write(this.handler);
            writer.Write(this.column);
            writer.Write(this.row);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 3; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stGateUnloadTrayRes : Packet
{
    public stGateUnloadTrayRes()
    {
        this.protocol = (Int32)protocolNum.stGateUnloadTrayRes;
        this.bcc = 1;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        return 0;
    }
}

[Serializable]
public class stGateUnloadTrayCompleteNotify : Packet
{
    public Int32 result;            // 0:성공, 그외 ErrorCode
    public char[] id;               // = tray ID
    public Int32 handler;           // 1:Left, 2:Right
    public Int32 column;            // 15 (Gate Index : 고정)
    public Int32 row;               // 0 ~ 9

    public stGateUnloadTrayCompleteNotify()
    {
        this.protocol = (Int32)protocolNum.stGateUnloadTrayCompleteNotify;
        this.bcc = 1;
        this.id = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // result 필드 읽기
        this.result = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // id 배열 읽기
        int idLength = sizeof(char) * 32; //this.len - (sizeof(Int32) * 3);
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // handler 필드 읽기
        this.handler = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(this.result);
            writer.Write(idBytes);
            writer.Write(this.handler);
            writer.Write(this.column);
            writer.Write(this.row);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 4; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stGateUnloadTrayCompleteRes : Packet
{
    public stGateUnloadTrayCompleteRes()
    {
        this.protocol = (Int32)protocolNum.stGateUnloadTrayCompleteRes;
        this.bcc = 1;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        return 0;
    }
}

// ===========================================================

[Serializable]
public class stAddEnteranceParcelReq : Packet
{
    public Int32 column;            // 14(고정)
    public Int32 row;               // 0 (고정)
    public Int32 height;            // row 점유량
    public char[] trackingNum;      // parcel tracking Num

    public stAddEnteranceParcelReq()
    {
        this.protocol = (Int32)protocolNum.stAddEnteranceParcelReq;
        this.bcc = 1;
        this.trackingNum = new char[32];
        this.column = 10;
        this.row = 9;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);
        
        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // height 필드 읽기
        this.height = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // trackingNum 배열 읽기
        int trackingNumLength = sizeof(char) * 32;
        this.trackingNum = new char[trackingNumLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.trackingNum, 0, trackingNumLength);        
    }

    public override byte[] Send()
    {
        byte[] trakcingBytes = new byte[this.trackingNum.Length * sizeof(char)];
        Buffer.BlockCopy(this.trackingNum, 0, trakcingBytes, 0, trakcingBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(this.column);
            writer.Write(this.row);
            writer.Write(this.height);
            writer.Write(trakcingBytes);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.trackingNum.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 3;
        return additionalLen;
    }
}

[Serializable]
public class stAddEnteranceParcelRes : Packet
{
    public Int32 result;            // 0:성공, 그외 ErrorCode
    public char[] id;               // = tray ID
    public char[] trackingNum;      // parcel tracking Num
    public Int32 column;            // 14 (Gate Index : 고정)
    public Int32 row;               // 0 고정
    public Int32 height;           // row 점유량

    public stAddEnteranceParcelRes()
    {
        this.protocol = (Int32)protocolNum.stAddEnteranceParcelRes;
        this.bcc = 1;
        this.id = new char[32];
        this.trackingNum = new char[32];
        this.column = 10;
        this.row = 9;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // result 필드 읽기
        this.result = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // id 배열 읽기
        int idLength = sizeof(char) * 32;
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // trackingNum 배열 읽기
        int trackingNumLength = sizeof(char) * 32;
        this.trackingNum = new char[trackingNumLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.trackingNum, 0, trackingNumLength);
        offset += trackingNumLength;

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);        

        // height 필드 읽기
        this.height = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);
        byte[] trakcingBytes = new byte[this.trackingNum.Length * sizeof(char)];
        Buffer.BlockCopy(this.trackingNum, 0, trakcingBytes, 0, trakcingBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(this.result);
            writer.Write(idBytes);
            writer.Write(trakcingBytes);
            writer.Write(this.column);
            writer.Write(this.row);
            writer.Write(this.height);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char) + this.trackingNum.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 4; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stDeleteEnteranceParcelReq : Packet
{
    public Int32 column;            // 14(고정)
    public Int32 row;               // 0 (고정)
    public char[] trackingNum;      // parcel tracking Num
    public Int32 height;            // row 점유량

    public stDeleteEnteranceParcelReq()
    {
        this.protocol = (Int32)protocolNum.stDeleteEnteranceParcelReq;
        this.bcc = 1;
        this.trackingNum = new char[32];
        this.column = 10;
        this.row = 9;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // trackingNum 배열 읽기
        int trackingNumLength = sizeof(char) * 32;
        this.trackingNum = new char[trackingNumLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.trackingNum, 0, trackingNumLength);
        offset += trackingNumLength;

        // height 필드 읽기
        this.height = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        byte[] trakcingBytes = new byte[this.trackingNum.Length * sizeof(char)];
        Buffer.BlockCopy(this.trackingNum, 0, trakcingBytes, 0, trakcingBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(this.column);
            writer.Write(this.row);
            writer.Write(trakcingBytes);
            writer.Write(this.height);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        int additionalLen = this.trackingNum.Length * sizeof(char);
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        additionalLen += sizeof(Int32) * 3;
        return additionalLen;
    }
}

[Serializable]
public class stDeleteEnteranceParcelRes : Packet
{
    public Int32 result;            // 0:성공, 그외 ErrorCode
    public char[] id;               // = tray ID
    public char[] trackingNum;      // parcel tracking Num
    public Int32 column;            // 14 (Gate Index : 고정)
    public Int32 row;               // 0 고정
    public Int32 height;           // row 점유량

    public stDeleteEnteranceParcelRes()
    {
        this.protocol = (Int32)protocolNum.stDeleteEnteranceParcelRes;
        this.bcc = 1;
        this.id = new char[32];
        this.trackingNum = new char[32];
        this.column = 10;
        this.row = 9;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // result 필드 읽기
        this.result = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // id 배열 읽기
        int idLength = sizeof(char) * 32;
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // trackingNum 배열 읽기
        int trackingNumLength = sizeof(char) * 32;
        this.trackingNum = new char[trackingNumLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.trackingNum, 0, trackingNumLength);
        offset += trackingNumLength;

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // height 필드 읽기
        this.height = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);
        byte[] trakcingBytes = new byte[this.trackingNum.Length * sizeof(char)];
        Buffer.BlockCopy(this.trackingNum, 0, trakcingBytes, 0, trakcingBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(this.result);
            writer.Write(idBytes);
            writer.Write(trakcingBytes);
            writer.Write(this.column);
            writer.Write(this.row);
            writer.Write(this.height);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char) + this.trackingNum.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 4; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}


// ===========================================================

[Serializable]
public class stAddGateParcelReq : Packet
{
    public Int32 column;            // 15(고정)
    public Int32 row;               // 0 ~ 9
    public Int32 height;            // row 점유량

    public stAddGateParcelReq()
    {
        this.protocol = (Int32)protocolNum.stAddGateParcelReq;
        this.bcc = 1;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // height 필드 읽기
        this.height = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(this.column);
            writer.Write(this.row);
            writer.Write(this.height);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = sizeof(Int32) * 3;
        return additionalLen;
    }
}

[Serializable]
public class stAddGateParcelRes : Packet
{
    public Int32 result;            // 0:성공, 그외 ErrorCode
    public char[] id;               // = tray ID
    public Int32 column;            // 15 (Gate Index : 고정)
    public Int32 row;               // 0 ~ 9 
    public Int32 height;           // row 점유량

    public stAddGateParcelRes()
    {
        this.protocol = (Int32)protocolNum.stAddGateParcelRes;
        this.bcc = 1;
        this.id = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // result 필드 읽기
        this.result = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // id 배열 읽기
        int idLength = sizeof(char) * 32;
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // height 필드 읽기
        this.height = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(this.result);
            writer.Write(idBytes);
            writer.Write(this.column);
            writer.Write(this.row);
            writer.Write(this.height);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 4; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

[Serializable]
public class stDeleteGateParcelReq : Packet
{
    public Int32 column;            // 15(고정)
    public Int32 row;               // 0 ~ 9
    public Int32 height;            // row 점유량

    public stDeleteGateParcelReq()
    {
        this.protocol = (Int32)protocolNum.stDeleteGateParcelReq;
        this.bcc = 1;
        this.column = 15;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // height 필드 읽기
        this.height = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(this.column);
            writer.Write(this.row);
            writer.Write(this.height);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = sizeof(Int32) * 3;
        return additionalLen;
    }
}

[Serializable]
public class stDeleteGateParcelRes : Packet
{
    public Int32 result;            // 0:성공, 그외 ErrorCode
    public char[] id;               // = tray ID
    public Int32 column;            // 15 (Gate Index : 고정)
    public Int32 row;               // 0 ~ 9 
    public Int32 height;            // row 점유량

    public stDeleteGateParcelRes()
    {
        this.protocol = (Int32)protocolNum.stDeleteGateParcelRes;
        this.column = 15;
        this.bcc = 1;
        this.id = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // result 필드 읽기
        this.result = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // id 배열 읽기
        int idLength = sizeof(char) * 32;
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // height 필드 읽기
        this.height = BitConverter.ToInt32(buffer, offset);
    }

    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(this.result);
            writer.Write(idBytes);
            writer.Write(this.column);
            writer.Write(this.row);
            writer.Write(this.height);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 4; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}

// ===========================================================

[Serializable]
public class stAllParcelCheckReq : Packet
{
    public stAllParcelCheckReq()
    {
        this.protocol = (Int32)protocolNum.stAllParcelCheckReq;
        this.bcc = 1;
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
    }

    public override byte[] Send()
    {
        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);

            return ms.ToArray();
        }
    }

    protected override int CalculateAdditionalLen()
    {
        return 0;
    }
}

// Tray의 갯수 만큼 아래 패킷이 전송되어야 합니다.
[Serializable]
public class stAllParcelCheckRes : Packet
{
    public char[] id;               // = tray ID
    public char[] trackingNum;      // parcel tracking Num
    public Int32 column;            // = tray 열 번호
    public Int32 row;               // tray 행 번호
    public Int32 height;            // 점유량 (기본 1 : 물건이 있을 경우 2 이상의 값)

    public stAllParcelCheckRes()
    {
        this.protocol = (Int32)protocolNum.stAllParcelCheckRes;
        this.bcc = 1;
        this.id = new char[32];
        this.trackingNum = new char[32];
    }

    public override void Read(byte[] buffer)
    {
        int offset = 0;

        // 길이 필드 읽기
        this.len = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // 프로토콜 필드 읽기
        this.protocol = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // BCC 필드 읽기
        this.bcc = buffer[offset];
        offset += sizeof(byte);

        // id 배열 읽기
        int idLength = sizeof(char) * 32; //this.len - (sizeof(Int32) * 3);
        this.id = new char[idLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.id, 0, idLength);
        offset += idLength;

        // trackingNum 배열 읽기
        int trackingNumLength = sizeof(char) * 32;
        this.trackingNum = new char[trackingNumLength / sizeof(char)];
        Buffer.BlockCopy(buffer, offset, this.trackingNum, 0, trackingNumLength);
        offset += trackingNumLength;

        // column 필드 읽기
        this.column = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // row 필드 읽기
        this.row = BitConverter.ToInt32(buffer, offset);
        offset += sizeof(Int32);

        // height 필드 읽기
        this.height = BitConverter.ToInt32(buffer, offset);
    }


    public override byte[] Send()
    {
        // char[]을 바이트 배열로 변환
        byte[] idBytes = new byte[this.id.Length * sizeof(char)];
        Buffer.BlockCopy(this.id, 0, idBytes, 0, idBytes.Length);
        byte[] trakcingBytes = new byte[this.trackingNum.Length * sizeof(char)];
        Buffer.BlockCopy(this.trackingNum, 0, trakcingBytes, 0, trakcingBytes.Length);

        // 패킷을 구성할 때 임시 MemoryStream 사용
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            CalculateLen();

            writer.Write(this.len);
            writer.Write(this.protocol);
            writer.Write(this.bcc);
            writer.Write(idBytes);
            writer.Write(trakcingBytes);
            writer.Write(this.column);
            writer.Write(this.row);
            writer.Write(this.height);

            return ms.ToArray();
        }
    }
    protected override int CalculateAdditionalLen()
    {
        // 추가 멤버 변수의 크기를 여기에서 더합니다.
        int additionalLen = this.id.Length * sizeof(char) + this.trackingNum.Length * sizeof(char);
        additionalLen += sizeof(Int32) * 3; // 다른 Int32 멤버 변수의 크기도 더합니다.
        return additionalLen;
    }
}