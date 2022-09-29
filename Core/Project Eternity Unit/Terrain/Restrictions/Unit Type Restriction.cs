using System;
using System.IO;
using System.Collections.Generic;

namespace ProjectEternity.Core.Units
{
    public class UnitTypeRestriction : ITerrainRestriction
    {
        public const string RestrictionName = "Unit Type";

        private readonly UnitAndTerrainValues Parent;
        public List<byte> ListUnitTypeIndex;
        public List<byte> ListUnitTypeIndexNegative;//Anyone but these types

        public UnitTypeRestriction(UnitAndTerrainValues Parent)
            : base(RestrictionName)
        {
            this.Parent = Parent;

            ListUnitTypeIndex = new List<byte>();
            ListUnitTypeIndexNegative = new List<byte>();
        }

        public UnitTypeRestriction(BinaryReader BR, UnitAndTerrainValues Parent)
            : base(RestrictionName)
        {
            this.Parent = Parent;

            int ListUnitTypeIndexCount = BR.ReadInt32();
            ListUnitTypeIndex = new List<byte>(ListUnitTypeIndexCount);
            for (int T = 0; T < ListUnitTypeIndexCount; ++T)
            {
                ListUnitTypeIndex.Add(BR.ReadByte());
            }

            int ListUnitTypeIndexNegativeCount = BR.ReadInt32();
            ListUnitTypeIndexNegative = new List<byte>(ListUnitTypeIndexNegativeCount);
            for (int T = 0; T < ListUnitTypeIndexNegativeCount; ++T)
            {
                ListUnitTypeIndexNegative.Add(BR.ReadByte());
            }
        }

        public override void DoSave(BinaryWriter BW)
        {
            BW.Write(ListUnitTypeIndex.Count);
            for (int T = 0; T < ListUnitTypeIndex.Count; ++T)
            {
                BW.Write(ListUnitTypeIndex[T]);
            }

            BW.Write(ListUnitTypeIndexNegative.Count);
            for (int T = 0; T < ListUnitTypeIndexNegative.Count; ++T)
            {
                BW.Write(ListUnitTypeIndexNegative[T]);
            }
        }

        public override bool IsValid(UnitMapComponent Unit, UnitStats Stats)
        {
            return ListUnitTypeIndex.Contains(Stats.UnitTypeIndex) && !ListUnitTypeIndexNegative.Contains(Stats.UnitTypeIndex);
        }

        public override void SetValues(List<List<string>> ListValuePerRow)
        {
            ListUnitTypeIndex.Clear();
            ListUnitTypeIndexNegative.Clear();

            foreach (List<string> NewValue in ListValuePerRow)
            {
                if (NewValue.Count == 0)
                {
                    continue;
                }

                if (NewValue[0].StartsWith("!"))
                {
                    ListUnitTypeIndex.Add((byte)Parent.ListUnitType.IndexOf(NewValue[0].Substring(1)));
                }
                else
                {
                    ListUnitTypeIndex.Add((byte)Parent.ListUnitType.IndexOf(NewValue[0]));
                }
            }
        }

        public override DataGridValue GetColumns()
        {
            DataGridValue GridValues = new DataGridValue();

            List<RowWithSelectedValue> ListChoiceByRow = new List<RowWithSelectedValue>();
            List<CellValue> ListRowCellValue;

            foreach (byte ActiveUnitTypeIndex in ListUnitTypeIndex)
            {
                ListRowCellValue = new List<CellValue>();
                ListRowCellValue.Add(new CellValue(Parent.ListUnitType[ActiveUnitTypeIndex], Parent.ListUnitType));

                ListChoiceByRow.Add(new RowWithSelectedValue("Value", ListRowCellValue));
            }

            foreach (byte ActiveUnitTypeIndex in ListUnitTypeIndexNegative)
            {
                ListRowCellValue = new List<CellValue>();
                ListRowCellValue.Add(new CellValue("!" + Parent.ListUnitType[ActiveUnitTypeIndex], Parent.ListUnitType));

                ListChoiceByRow.Add(new RowWithSelectedValue("Value", ListRowCellValue));
            }

            ListRowCellValue = new List<CellValue>();
            ListRowCellValue.Add(new CellValue(null, Parent.ListUnitType));

            ListChoiceByRow.Add(new RowWithSelectedValue("Value", ListRowCellValue));

            GridValues.ListRow = ListChoiceByRow;

            return GridValues;
        }

        public override ITerrainRestriction DoCopy()
        {
            return new UnitTypeRestriction(Parent);
        }

        public override string ToString()
        {
            return "Unit Type Restriction";
        }
    }
}
