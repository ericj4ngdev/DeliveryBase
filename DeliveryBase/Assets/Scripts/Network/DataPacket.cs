using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct DataPacket
{
    public Int32 len;
    public Int32 protocol;
    public byte bcc;
}