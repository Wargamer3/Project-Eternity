using ProjectEternity.Core.Online;
using System.IO;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public struct BattleRecords
    {
        public uint NumberOfGamesPlayed;
        public uint NumberOfGamesWon;
        public uint NumberOfGamesLost;
        public uint NumberOfKills;
        public uint NumberOfUnitsLost;

        public uint TotalDamageGiven;
        public uint TotalDamageReceived;
        public uint TotalDamageRecovered;

        public BattleRecords(BinaryReader BR)
        {
            NumberOfGamesPlayed = BR.ReadUInt32();
            NumberOfGamesWon = BR.ReadUInt32();
            NumberOfGamesLost = BR.ReadUInt32();
            NumberOfKills = BR.ReadUInt32();
            NumberOfUnitsLost = BR.ReadUInt32();

            TotalDamageGiven = BR.ReadUInt32();
            TotalDamageReceived = BR.ReadUInt32();
            TotalDamageRecovered = BR.ReadUInt32();
        }

        public BattleRecords(BattleRecords Clone)
        {
            NumberOfGamesPlayed = Clone.NumberOfGamesPlayed;
            NumberOfGamesWon = Clone.NumberOfGamesWon;
            NumberOfGamesLost = Clone.NumberOfGamesLost;
            NumberOfKills = Clone.NumberOfKills;
            NumberOfUnitsLost = Clone.NumberOfUnitsLost;

            TotalDamageGiven = Clone.TotalDamageGiven;
            TotalDamageReceived = Clone.TotalDamageReceived;
            TotalDamageRecovered = Clone.TotalDamageRecovered;
        }

        public BattleRecords(ByteReader BR)
        {
            NumberOfGamesPlayed = BR.ReadUInt32();
            NumberOfGamesWon = BR.ReadUInt32();
            NumberOfGamesLost = BR.ReadUInt32();
            NumberOfKills = BR.ReadUInt32();
            NumberOfUnitsLost = BR.ReadUInt32();

            TotalDamageGiven = BR.ReadUInt32();
            TotalDamageReceived = BR.ReadUInt32();
            TotalDamageRecovered = BR.ReadUInt32();
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(NumberOfGamesPlayed);
            BW.Write(NumberOfGamesWon);
            BW.Write(NumberOfGamesLost);
            BW.Write(NumberOfKills);
            BW.Write(NumberOfUnitsLost);

            BW.Write(TotalDamageGiven);
            BW.Write(TotalDamageReceived);
            BW.Write(TotalDamageRecovered);
        }
    }
}
