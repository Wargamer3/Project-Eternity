using System;
using System.IO;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public struct MultiplayerRecord
    {
        public string Name;
        public DateTimeOffset FirstCompletionDate;
        public uint MaxScore;

        public MultiplayerRecord(BinaryReader BR)
        {
            Name = BR.ReadString();
            FirstCompletionDate = new DateTimeOffset(BR.ReadInt64(), new TimeSpan(BR.ReadInt64()));
            MaxScore = BR.ReadUInt32();
        }

        public MultiplayerRecord(MultiplayerRecord Clone)
        {
            Name = Clone.Name;
            FirstCompletionDate = Clone.FirstCompletionDate;
            MaxScore = Clone.MaxScore;
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
