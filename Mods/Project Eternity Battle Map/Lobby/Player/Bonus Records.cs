using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BonusRecords
    {
        public Dictionary<string, uint> DicNumberOfBonusObtainedByName;

        public BonusRecords()
        {
            DicNumberOfBonusObtainedByName = new Dictionary<string, uint>();
        }

        public BonusRecords(BinaryReader BR)
        {
            int DicNumberOfBonusObtainedByNameCount = BR.ReadInt32();
            DicNumberOfBonusObtainedByName = new Dictionary<string, uint>(DicNumberOfBonusObtainedByNameCount);
            for (int i = 0; i < DicNumberOfBonusObtainedByNameCount; ++i)
            {
                DicNumberOfBonusObtainedByName.Add(BR.ReadString(), BR.ReadUInt32());
            }
        }

        public BonusRecords(ByteReader BR)
        {
            int DicNumberOfBonusObtainedByNameCount = BR.ReadInt32();
            DicNumberOfBonusObtainedByName = new Dictionary<string, uint>(DicNumberOfBonusObtainedByNameCount);
            for (int i = 0; i < DicNumberOfBonusObtainedByNameCount; ++i)
            {
                DicNumberOfBonusObtainedByName.Add(BR.ReadString(), BR.ReadUInt32());
            }
        }

        public BonusRecords(BonusRecords Clone)
        {
            DicNumberOfBonusObtainedByName = new Dictionary<string, uint>(Clone.DicNumberOfBonusObtainedByName);
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(DicNumberOfBonusObtainedByName.Count);
            foreach (KeyValuePair<string, uint> ActiveEntry in DicNumberOfBonusObtainedByName)
            {
                BW.Write(ActiveEntry.Key);
                BW.Write(ActiveEntry.Value);
            }
        }
    }
}
