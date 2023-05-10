using System;
using System.IO;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public struct MapRecord
    {
        public string Name;
        public uint TotalSecondsPlayed;
        public int TotalTimesPlayed;
        public int TotalVictories;
        public int TotalUnitsKilled;
        public uint TotalDamageGiven;

        public MapRecord(string Name)
        {
            this.Name = Name;
            TotalSecondsPlayed = 0;
            TotalTimesPlayed = 0;
            TotalVictories = 0;
            TotalUnitsKilled = 0;
            TotalDamageGiven = 0;
        }

        public MapRecord(BinaryReader BR)
        {
            Name = BR.ReadString();
            TotalSecondsPlayed = BR.ReadUInt32();
            TotalTimesPlayed = BR.ReadInt32();
            TotalVictories = BR.ReadInt32();
            TotalUnitsKilled = BR.ReadInt32();
            TotalDamageGiven = BR.ReadUInt32();
        }

        public MapRecord(MapRecord Clone)
        {
            Name = Clone.Name;
            TotalSecondsPlayed = Clone.TotalSecondsPlayed;
            TotalTimesPlayed = Clone.TotalTimesPlayed;
            TotalVictories = Clone.TotalVictories;
            TotalUnitsKilled = Clone.TotalUnitsKilled;
            TotalDamageGiven = Clone.TotalDamageGiven;
        }

        public MapRecord(ByteReader BR)
        {
            Name = BR.ReadString();
            TotalSecondsPlayed = BR.ReadUInt32();
            TotalTimesPlayed = BR.ReadInt32();
            TotalVictories = BR.ReadInt32();
            TotalUnitsKilled = BR.ReadInt32();
            TotalDamageGiven = BR.ReadUInt32();
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(Name);
            BW.Write(TotalSecondsPlayed);
            BW.Write(TotalTimesPlayed);
            BW.Write(TotalVictories);
            BW.Write(TotalUnitsKilled);
            BW.Write(TotalDamageGiven);
        }
    }
}
