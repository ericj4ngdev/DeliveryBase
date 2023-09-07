using System;

[Serializable]
struct DataPacket
{
    public Int32 len;
    public Int32 protocol;
    public byte bcc;
}