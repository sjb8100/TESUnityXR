using UnityEngine;
using UnityEditor;
using System;

namespace TESUnity.ESM
{
    public abstract class SubRecord
    {
        public SubRecordHeader header;

        public abstract void DeserializeData(UnityBinaryReader reader, uint dataSize);
    }

    // Common sub-records.
    public class STRVSubRecord : SubRecord
    {
        public string value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            value = reader.ReadPossiblyNullTerminatedASCIIString((int)header.dataSize);
        }
    }

    // variable size
    public class INTVSubRecord : SubRecord
    {
        public long value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            switch (header.dataSize)
            {
                case 1:
                    value = reader.ReadByte();
                    break;
                case 2:
                    value = reader.ReadLEInt16();
                    break;
                case 4:
                    value = reader.ReadLEInt32();
                    break;
                case 8:
                    value = reader.ReadLEInt64();
                    break;
                default:
                    throw new NotImplementedException("Tried to read an INTV subrecord with an unsupported size (" + header.dataSize.ToString() + ").");
            }
        }
    }

    public class INTVTwoI32SubRecord : SubRecord
    {
        public int value0, value1;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            Debug.Assert(header.dataSize == 8);

            value0 = reader.ReadLEInt32();
            value1 = reader.ReadLEInt32();
        }
    }

    public class INDXSubRecord : INTVSubRecord { }

    public class FLTVSubRecord : SubRecord
    {
        public float value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            value = reader.ReadLESingle();
        }
    }

    public class ByteSubRecord : SubRecord
    {
        public byte value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            value = reader.ReadByte();
        }
    }

    public class Int32SubRecord : SubRecord
    {
        public int value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            value = reader.ReadLEInt32();
        }
    }

    public class UInt32SubRecord : SubRecord
    {
        public uint value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            value = reader.ReadLEUInt32();
        }
    }

    public class NAMESubRecord : STRVSubRecord { }
    public class FNAMSubRecord : STRVSubRecord { }
    public class SNAMSubRecord : STRVSubRecord { }

    public class ANAMSubRecord : SubRecord
    {
        public string value;

        public override void DeserializeData(UnityBinaryReader reader, UInt32 dataSize)
        {
            value = reader.ReadASCIIString((int)dataSize);
        }
    }

    public class ITEXSubRecord : STRVSubRecord { }
    public class ENAMSubRecord : STRVSubRecord { }
    public class BNAMSubRecord : STRVSubRecord { }
    public class CNAMSubRecord : STRVSubRecord { }
    public class SCRISubRecord : STRVSubRecord { }
    public class SCPTSubRecord : STRVSubRecord { }
    public class MODLSubRecord : STRVSubRecord { }
    public class TEXTSubRecord : STRVSubRecord { }
}