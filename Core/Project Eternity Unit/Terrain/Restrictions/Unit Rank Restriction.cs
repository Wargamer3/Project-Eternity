using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ProjectEternity.Core.Units
{
    public class UnitRankRestriction : ITerrainRestriction
    {
        public const string RestrictionName = "Unit Rank";

        private readonly UnitAndTerrainValues Parent;

        private readonly List<string> ListChoice = new List<string>() { "S", "A", "B", "C", "D", "-" };

        public Dictionary<byte, List<byte>> DicAllowedMovementRankPositive;
        public Dictionary<byte, List<byte>> DicAllowedMovementNegativeRankPositive;//Anyone but these movement tier

        public Dictionary<byte, List<byte>> DicAllowedMovementRankNegative;
        public Dictionary<byte, List<byte>> DicAllowedMovementNegativeRankNegative;//Anyone but these movement and rank tier

        public UnitRankRestriction(UnitAndTerrainValues Parent)
            : base(RestrictionName)
        {
            this.Parent = Parent;

            DicAllowedMovementRankPositive = new Dictionary<byte, List<byte>>();
            DicAllowedMovementRankNegative = new Dictionary<byte, List<byte>>();

            DicAllowedMovementNegativeRankPositive = new Dictionary<byte, List<byte>>();
            DicAllowedMovementNegativeRankNegative = new Dictionary<byte, List<byte>>();
        }

        public UnitRankRestriction(BinaryReader BR, UnitAndTerrainValues Parent)
            : base(RestrictionName)
        {
            this.Parent = Parent;

            int DicAllowedMovementRankPositiveCount = BR.ReadInt32();
            DicAllowedMovementRankPositive = new Dictionary<byte, List<byte>>(DicAllowedMovementRankPositiveCount);
            for (int M = 0; M < DicAllowedMovementRankPositiveCount; ++M)
            {
                byte MovementIndex = BR.ReadByte();
                int ListValueCount = BR.ReadInt32();

                List<byte> ListValue = new List<byte>();
                for (int V = 0; V < ListValueCount; ++V)
                {
                    ListValue.Add(BR.ReadByte());
                }

                DicAllowedMovementRankPositive.Add(MovementIndex, ListValue);
            }

            int DicAllowedMovementNegativeRankPositiveCount = BR.ReadInt32();
            DicAllowedMovementRankNegative = new Dictionary<byte, List<byte>>(DicAllowedMovementNegativeRankPositiveCount);
            for (int M = 0; M < DicAllowedMovementNegativeRankPositiveCount; ++M)
            {
                byte MovementIndex = BR.ReadByte();
                int ListValueCount = BR.ReadInt32();

                List<byte> ListValue = new List<byte>();
                for (int V = 0; V < ListValueCount; ++V)
                {
                    ListValue.Add(BR.ReadByte());
                }

                DicAllowedMovementNegativeRankPositive.Add(MovementIndex, ListValue);
            }

            int DicAllowedMovementRankNegativeCount = BR.ReadInt32();
            DicAllowedMovementNegativeRankPositive = new Dictionary<byte, List<byte>>(DicAllowedMovementRankNegativeCount);
            for (int M = 0; M < DicAllowedMovementRankNegativeCount; ++M)
            {
                byte MovementIndex = BR.ReadByte();
                int ListValueCount = BR.ReadInt32();

                List<byte> ListValue = new List<byte>();
                for (int V = 0; V < ListValueCount; ++V)
                {
                    ListValue.Add(BR.ReadByte());
                }

                DicAllowedMovementRankNegative.Add(MovementIndex, ListValue);
            }

            int DicAllowedMovementNegativeRankNegativeCount = BR.ReadInt32();
            DicAllowedMovementNegativeRankNegative = new Dictionary<byte, List<byte>>(DicAllowedMovementNegativeRankNegativeCount);
            for (int M = 0; M < DicAllowedMovementNegativeRankNegativeCount; ++M)
            {
                byte MovementIndex = BR.ReadByte();
                int ListValueCount = BR.ReadInt32();

                List<byte> ListValue = new List<byte>();
                for (int V = 0; V < ListValueCount; ++V)
                {
                    ListValue.Add(BR.ReadByte());
                }

                DicAllowedMovementNegativeRankNegative.Add(MovementIndex, ListValue);
            }
        }

        public override void DoSave(BinaryWriter BW)
        {
            BW.Write(DicAllowedMovementRankPositive.Count);
            foreach (KeyValuePair<byte, List<byte>> RanksByMovement in DicAllowedMovementRankPositive)
            {
                BW.Write(RanksByMovement.Key);
                BW.Write(RanksByMovement.Value.Count);
                for (int R = 0; R < RanksByMovement.Value.Count; ++R)
                {
                    BW.Write(RanksByMovement.Value[R]);
                }
            }
            BW.Write(DicAllowedMovementNegativeRankPositive.Count);
            foreach (KeyValuePair<byte, List<byte>> RanksByMovement in DicAllowedMovementNegativeRankPositive)
            {
                BW.Write(RanksByMovement.Key);
                BW.Write(RanksByMovement.Value.Count);
                for (int R = 0; R < RanksByMovement.Value.Count; ++R)
                {
                    BW.Write(RanksByMovement.Value[R]);
                }
            }

            BW.Write(DicAllowedMovementRankNegative.Count);
            foreach (KeyValuePair<byte, List<byte>> RanksByMovement in DicAllowedMovementRankNegative)
            {
                BW.Write(RanksByMovement.Key);
                BW.Write(RanksByMovement.Value.Count);
                for (int R = 0; R < RanksByMovement.Value.Count; ++R)
                {
                    BW.Write(RanksByMovement.Value[R]);
                }
            }
            BW.Write(DicAllowedMovementNegativeRankNegative.Count);
            foreach (KeyValuePair<byte, List<byte>> RanksByMovement in DicAllowedMovementNegativeRankNegative)
            {
                BW.Write(RanksByMovement.Key);
                BW.Write(RanksByMovement.Value.Count);
                for (int R = 0; R < RanksByMovement.Value.Count; ++R)
                {
                    BW.Write(RanksByMovement.Value[R]);
                }
            }
        }

        public override bool IsValid(UnitMapComponent Unit, UnitStats Stats)
        {
            byte UnitRank;

            foreach (KeyValuePair<byte, List<byte>> ActiveRanksByMovement in DicAllowedMovementRankPositive)
            {
                if (Stats.DicRankByMovement.TryGetValue(ActiveRanksByMovement.Key, out UnitRank))
                {
                    if (ActiveRanksByMovement.Value.Contains(UnitRank))
                    {
                        return true;
                    }
                }
            }

            foreach (KeyValuePair<byte, List<byte>> ActiveRanksByMovement in DicAllowedMovementNegativeRankPositive)
            {
                if (!Stats.DicRankByMovement.TryGetValue(ActiveRanksByMovement.Key, out UnitRank))
                {
                    if (ActiveRanksByMovement.Value.Contains(UnitRank))
                    {
                        return true;
                    }
                }
            }

            foreach (KeyValuePair<byte, List<byte>> ActiveRanksByMovement in DicAllowedMovementRankNegative)
            {
                if (Stats.DicRankByMovement.TryGetValue(ActiveRanksByMovement.Key, out UnitRank))
                {
                    if (!ActiveRanksByMovement.Value.Contains(UnitRank))
                    {
                        return true;
                    }
                }
            }

            foreach (KeyValuePair<byte, List<byte>> ActiveRanksByMovement in DicAllowedMovementNegativeRankNegative)
            {
                if (!Stats.DicRankByMovement.TryGetValue(ActiveRanksByMovement.Key, out UnitRank))
                {
                    if (!ActiveRanksByMovement.Value.Contains(UnitRank))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override void SetValues(List<List<string>> ListValuePerRow)
        {
            DicAllowedMovementRankPositive.Clear();
            DicAllowedMovementRankNegative.Clear();

            DicAllowedMovementNegativeRankPositive.Clear();
            DicAllowedMovementNegativeRankNegative.Clear();

            foreach (List<string> NewValue in ListValuePerRow)
            {
                if (NewValue.Count == 0)
                {
                    continue;
                }

                if (NewValue[0].StartsWith("!"))
                {
                    byte MovementIndex = (byte)Parent.ListTerrainType.IndexOf(Parent.ListTerrainType.Find(x => x.Name == NewValue[0].Substring(1)));

                    if (NewValue.Count == 2)
                    {

                    }
                }
                else
                {
                    byte MovementIndex = (byte)Parent.ListTerrainType.IndexOf(Parent.ListTerrainType.Find(x => x.Name == NewValue[0]));

                    if (NewValue.Count >= 2)
                    {
                        List<byte> ListNewRankPositive = new List<byte>();
                        List<byte> ListNewRankNegative = new List<byte>();
                        for (int V = 1; V < NewValue.Count; ++V)
                        {
                            if (NewValue[V].StartsWith("!"))
                            {
                                ListNewRankNegative.Add((byte)ListChoice.IndexOf(NewValue[V].Substring(1)));
                            }
                            else
                            {
                                ListNewRankPositive.Add((byte)ListChoice.IndexOf(NewValue[V]));
                            }
                        }

                        if (!DicAllowedMovementRankPositive.ContainsKey(MovementIndex) && ListNewRankPositive.Count > 0)
                        {
                            DicAllowedMovementRankPositive.Add(MovementIndex, ListNewRankPositive);
                        }

                        if (!DicAllowedMovementRankNegative.ContainsKey(MovementIndex) && ListNewRankNegative.Count > 0)
                        {
                            DicAllowedMovementRankNegative.Add(MovementIndex, ListNewRankNegative);
                        }
                    }
                    else
                    {
                        if (!DicAllowedMovementRankPositive.ContainsKey(MovementIndex))
                        {
                            DicAllowedMovementRankPositive.Add(MovementIndex, new List<byte>());
                        }
                    }
                }
            }
        }

        public override DataGridValue GetColumns()
        {
            DataGridValue GridValues = new DataGridValue();
            List<string> ListMovementName = Parent.ListTerrainType.Select(x => x.Name).ToList();

            List<RowWithSelectedValue> ListChoiceByColumn = new List<RowWithSelectedValue>();
            List<CellValue> ListRowCellValue;

            foreach (KeyValuePair<byte, List<byte>> ActiveUnitTypeIndex in DicAllowedMovementRankPositive)
            {
                ListRowCellValue = new List<CellValue>();
                ListRowCellValue.Add(new CellValue(Parent.ListTerrainType[ActiveUnitTypeIndex.Key].Name, ListMovementName));

                foreach (byte ActiveUnitRankIndex in ActiveUnitTypeIndex.Value)
                {
                    ListRowCellValue.Add(new CellValue(ListChoice[ActiveUnitRankIndex], ListChoice));
                }

                ListRowCellValue.Add(new CellValue(null, ListChoice));

                ListChoiceByColumn.Add(new RowWithSelectedValue("Value", ListRowCellValue));
            }

            foreach (KeyValuePair<byte, List<byte>> ActiveUnitTypeIndex in DicAllowedMovementRankNegative)
            {
                ListRowCellValue = new List<CellValue>();
                ListRowCellValue.Add(new CellValue(Parent.ListTerrainType[ActiveUnitTypeIndex.Key].Name, ListMovementName));

                foreach (byte ActiveUnitRankIndex in ActiveUnitTypeIndex.Value)
                {
                    ListRowCellValue.Add(new CellValue("!" + ListChoice[ActiveUnitRankIndex], ListChoice));
                }

                ListRowCellValue.Add(new CellValue(null, ListChoice));

                ListChoiceByColumn.Add(new RowWithSelectedValue("Value", ListRowCellValue));
            }
            foreach (KeyValuePair<byte, List<byte>> ActiveUnitTypeIndex in DicAllowedMovementNegativeRankPositive)
            {
                ListRowCellValue = new List<CellValue>();
                ListRowCellValue.Add(new CellValue("!" + Parent.ListTerrainType[ActiveUnitTypeIndex.Key].Name, ListMovementName));

                foreach (byte ActiveUnitRankIndex in ActiveUnitTypeIndex.Value)
                {
                    ListRowCellValue.Add(new CellValue(ListChoice[ActiveUnitRankIndex], ListChoice));
                }

                ListChoiceByColumn.Add(new RowWithSelectedValue("Value", ListRowCellValue));
            }
            foreach (KeyValuePair<byte, List<byte>> ActiveUnitTypeIndex in DicAllowedMovementNegativeRankNegative)
            {
                ListRowCellValue = new List<CellValue>();
                ListRowCellValue.Add(new CellValue("!" + Parent.ListTerrainType[ActiveUnitTypeIndex.Key].Name, ListMovementName));

                foreach (byte ActiveUnitRankIndex in ActiveUnitTypeIndex.Value)
                {
                    ListRowCellValue.Add(new CellValue("!" + ListChoice[ActiveUnitRankIndex], ListChoice));
                }

                ListChoiceByColumn.Add(new RowWithSelectedValue("Value", ListRowCellValue));
            }


            ListRowCellValue = new List<CellValue>();
            ListRowCellValue.Add(new CellValue(null, ListMovementName));
            ListRowCellValue.Add(new CellValue(null, ListChoice));
            ListChoiceByColumn.Add(new RowWithSelectedValue("Value", ListRowCellValue));

            GridValues.ListRow = ListChoiceByColumn;

            return GridValues;
        }

        public override ITerrainRestriction DoCopy()
        {
            return new UnitRankRestriction(Parent);
        }

        public override string ToString()
        {
            return "Unit Rank Restriction";
        }
    }
}
