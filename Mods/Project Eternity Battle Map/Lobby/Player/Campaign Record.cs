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
            FirstCompletionDate = new DateTimeOffset(BR.ReadInt64(), new TimeSpan(BR.ReadInt64()));
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
