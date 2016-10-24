﻿namespace TESUnity.ESM
{
    public class BYDTSubRecord : SubRecord
    {
        public enum BodyPart
        {
            Head = 0,
            Hair = 1,
            Neck = 2,
            Chest = 3,
            Groin = 4,
            Hand = 5,
            Wrist = 6,
            Forearm = 7,
            Upperarm = 8,
            Foot = 9,
            Ankle = 10,
            Knee = 11,
            Upperleg = 12,
            Clavicle = 13,
            Tail = 14
        }

        public enum Flag
        {
            Female = 1,
            Playabe = 2
        }

        public enum BodyPartType
        {
            Skin = 0,
            Clothing = 1,
            Armor = 2
        }

        public byte part;
        public byte vampire;
        public byte flags;
        public byte partType;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            part = reader.ReadByte();
            vampire = reader.ReadByte();
            flags = reader.ReadByte();
            partType = reader.ReadByte();
        }
    }

    public class BODYRecord : Record
    {
        public class DELESubRecord : INTVSubRecord { }

        public BYDTSubRecord BYDT;
        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "BYDT":
                    BYDT = new BYDTSubRecord();
                    return BYDT;
                case "NAME":
                    NAME = new NAMESubRecord();
                    return NAME;
                case "MODL":
                    MODL = new MODLSubRecord();
                    return MODL;
                case "FNAM":
                    FNAM = new FNAMSubRecord();
                    return FNAM;
            }

            return null;
        }
    }
}
