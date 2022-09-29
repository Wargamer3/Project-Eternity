using System;
using System.IO;
using System.Collections.Generic;

namespace ProjectEternity.Core.Units
{
    public class TagRestriction : ITerrainRestriction
    {
        public const string RestrictionName = "Tag";

        private readonly UnitAndTerrainValues Parent;

        public List<string> ListAllowedTags;
        public List<string> ListAllowedTagsNegative;//Anyone but these Tags

        public TagRestriction(UnitAndTerrainValues Parent)
            : base(RestrictionName)
        {
            this.Parent = Parent;
            ListAllowedTags = new List<string>();
            ListAllowedTagsNegative = new List<string>();
        }

        public TagRestriction(BinaryReader BR, UnitAndTerrainValues Parent)
            : base(RestrictionName)
        {
            this.Parent = Parent;

            int ListAllowedTagsCount = BR.ReadInt32();
            for (int T = 0; T < ListAllowedTagsCount; ++T)
            {
                ListAllowedTags.Add(BR.ReadString());
            }

            int ListAllowedTagsNegativeCount = BR.ReadInt32();
            for (int T = 0; T < ListAllowedTagsNegativeCount; ++T)
            {
                ListAllowedTagsNegative.Add(BR.ReadString());
            }
        }

        public override void DoSave(BinaryWriter BW)
        {
            BW.Write(ListAllowedTags.Count);
            for (int T = 0; T < ListAllowedTags.Count; ++T)
            {
                BW.Write(ListAllowedTags[T]);
            }

            BW.Write(ListAllowedTagsNegative.Count);
            for (int T = 0; T < ListAllowedTagsNegative.Count; ++T)
            {
                BW.Write(ListAllowedTagsNegative[T]);
            }
        }

        public override bool IsValid(UnitMapComponent Unit, UnitStats Stats)
        {
            foreach (string ActiveTag in ListAllowedTags)
            {
                if (Unit.TeamTags.ContainsTag(ActiveTag))
                {
                    return true;
                }
            }

            foreach (string ActiveTag in ListAllowedTagsNegative)
            {
                if (Unit.TeamTags.ContainsTag(ActiveTag))
                {
                    return false;
                }
            }

            return false;
        }

        public override void SetValues(List<List<string>> ListValuePerRow)
        {
        }

        public override DataGridValue GetColumns()
        {
            DataGridValue GridValues = new DataGridValue();

            /* List<RowWithSelectedValue> ListChoiceByRow = new List<RowWithSelectedValue>();
             List<CellValue> ListRowCellValue;

             foreach (string ActiveTag in ListAllowedTags)
             {
                 ListRowCellValue = new List<CellValue>();
                 ListRowCellValue.Add(new CellValue(ListUnitType[ActiveUnitTypeIndex], ListUnitType));

                 ListChoiceByRow.Add(new RowWithSelectedValue(ListRowCellValue));
                 ListChoiceByColumn.Add(new RowWithSelectedValue("Tag", 100, null, ActiveTag));
             }

             foreach (string ActiveTag in ListAllowedTagsNegative)
             {
                 ListChoiceByColumn.Add(new RowWithSelectedValue("Tag", 100, null, ActiveTag));
             }

             ListChoiceByColumn.Add(new RowWithSelectedValue("Tag", 100, null, null));
            */
            return GridValues;
        }

        public override ITerrainRestriction DoCopy()
        {
            return new TagRestriction(Parent);
        }

        public override string ToString()
        {
            return "Tag Restriction";
        }
    }
}
