using System;
using System.IO;
using System.Collections.Generic;

namespace ProjectEternity.Core.Units
{
    public class UnitSizeRestriction : ITerrainRestriction
    {
        public const string RestrictionName = "Unit Size";

        private readonly UnitAndTerrainValues Parent;

        public List<byte> ListUnitSize;
        public List<byte> ListUnitSizeNegative;//Anyone but these sizes

        public UnitSizeRestriction(UnitAndTerrainValues Parent)
            : base(RestrictionName)
        {
            this.Parent = Parent;

            ListUnitSize = new List<byte>();
            ListUnitSizeNegative = new List<byte>();
        }

        public UnitSizeRestriction(BinaryReader BR, UnitAndTerrainValues Parent)
            : base(RestrictionName)
        {
            this.Parent = Parent;

            int ListUnitSizeCount = BR.ReadInt32();
            for (int T = 0; T < ListUnitSizeCount; ++T)
            {
                ListUnitSize.Add(BR.ReadByte());
            }

            int ListUnitSizeNegativeCount = BR.ReadInt32();
            for (int T = 0; T < ListUnitSizeNegativeCount; ++T)
            {
                ListUnitSizeNegative.Add(BR.ReadByte());
            }
        }

        public override void DoSave(BinaryWriter BW)
        {
            BW.Write(ListUnitSize.Count);
            for (int T = 0; T < ListUnitSize.Count; ++T)
            {
                BW.Write(ListUnitSize[T]);
            }

            BW.Write(ListUnitSizeNegative.Count);
            for (int T = 0; T < ListUnitSizeNegative.Count; ++T)
            {
                BW.Write(ListUnitSizeNegative[T]);
            }
        }

        public override bool IsValid(UnitMapComponent Unit, UnitStats Stats)
        {
            return ListUnitSize.Contains(Stats.SizeIndex) && !ListUnitSizeNegative.Contains(Stats.SizeIndex);
        }

        public override void SetValues(List<List<string>> ListValuePerRow)
        {
            ListUnitSize.Clear();
            ListUnitSizeNegative.Clear();

            foreach (List<string> NewValue in ListValuePerRow)
            {
                if (NewValue.Count == 0)
                {
                    continue;
                }

                if (NewValue[0].StartsWith("!"))
                {
                    ListUnitSize.Add((byte)UnitStats.ListUnitSize.IndexOf(NewValue[0].Substring(1)));
                }
                else
                {
                    ListUnitSizeNegative.Add((byte)UnitStats.ListUnitSize.IndexOf(NewValue[0]));
                }
            }
        }

        public override DataGridValue GetColumns()
        {
            DataGridValue GridValues = new DataGridValue();

            List<RowWithSelectedValue> ListChoiceByRow = new List<RowWithSelectedValue>();
            List<CellValue> ListRowCellValue;

            foreach (byte ActiveSize in ListUnitSize)
            {
                ListRowCellValue = new List<CellValue>();
                ListRowCellValue.Add(new CellValue(UnitStats.ListUnitSize[ActiveSize], UnitStats.ListUnitSize));

                ListChoiceByRow.Add(new RowWithSelectedValue("Value", ListRowCellValue));
            }

            foreach (byte ActiveTag in ListUnitSizeNegative)
            {
                ListRowCellValue = new List<CellValue>();
                ListRowCellValue.Add(new CellValue("!" + UnitStats.ListUnitSize[ActiveTag], UnitStats.ListUnitSize));

                ListChoiceByRow.Add(new RowWithSelectedValue("Value", ListRowCellValue));
            }

            ListRowCellValue = new List<CellValue>();
            ListRowCellValue.Add(new CellValue(null, UnitStats.ListUnitSize));

            ListChoiceByRow.Add(new RowWithSelectedValue("Value", ListRowCellValue));

            GridValues.ListRow = ListChoiceByRow;

            return GridValues;
        }

        public override ITerrainRestriction DoCopy()
        {
            return new UnitSizeRestriction(Parent);
        }

        public override string ToString()
        {
            return "Unit Size Restriction";
        }
    }
}
