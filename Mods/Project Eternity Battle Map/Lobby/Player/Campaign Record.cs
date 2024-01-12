using System;
using System.IO;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public struct CampaignRecord
    {
        public string Name;
        public DateTimeOffset FirstCompletionDate;
        public uint MaxScore;

        public CampaignRecord(string Name, uint MaxScore)
        {
            this.Name = Name;
            this.MaxScore = MaxScore;
            FirstCompletionDate = DateTimeOffset.UtcNow;
        }

        public CampaignRecord(BinaryReader BR)
        {
            Name = BR.ReadString();
            FirstCompletionDate = new DateTimeOffset(BR.ReadInt64(), new TimeSpan(BR.ReadInt64()));
            MaxScore = BR.ReadUInt32();
        }

        public CampaignRecord(CampaignRecord Clone)
        {
            Name = Clone.Name;
            FirstCompletionDate = Clone.FirstCompletionDate;
            MaxScore = Clone.MaxScore;
        }

        public CampaignRecord(ByteReader BR)
        {
            Name = BR.ReadString();
            long Ticks = BR.ReadInt64();
            long Ticks2 = BR.ReadInt64();
            FirstCompletionDate = new DateTimeOffset(Ticks, new TimeSpan(Ticks2));
            MaxScore = BR.ReadUInt32();
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(Name);
            BW.Write(FirstCompletionDate.Ticks);
            BW.Write(FirstCompletionDate.Offset.Ticks);
            BW.Write(MaxScore);
        }
    }
}
