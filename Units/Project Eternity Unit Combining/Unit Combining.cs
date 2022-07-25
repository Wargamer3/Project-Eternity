using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens;
using ProjectEternity.GameScreens.DeathmatchMapScreen;
using Microsoft.Xna.Framework;

namespace ProjectEternity.Core.Units.Combining
{
    public class UnitCombining : DeathmatchUnit
    {
        public override string UnitTypeName => "Combining";

        private Unit OriginalUnit;
        public readonly string OriginalUnitName;
        private Unit CombinedUnit;
        public readonly string CombinedUnitName;
        //First Unit in the array is the leader.
        public readonly string[] ArrayCombiningUnitName;
        public List<Squad> ListFoundCombiningUnit;
        private bool Combined;

        public UnitCombining()
            : this(null)
        {
        }

        public UnitCombining(DeathmatchParams Params)
            : base(Params)
        {
            OriginalUnitName = string.Empty;
            CombinedUnitName = string.Empty;
            ArrayCombiningUnitName = new string[0];
        }

        public UnitCombining(string Name, ContentManager Content, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : this(Name, Content, null, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget)
        {
        }

        public UnitCombining(string Name, ContentManager Content, DeathmatchParams Params, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffect, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : base(Name, Params)
        {
            ItemName = Name;
            Combined = false;

            FileStream FS = new FileStream("Content/Units/Combining/" + Name + ".peu", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            OriginalUnitName = BR.ReadString();
            if (!string.IsNullOrEmpty(OriginalUnitName) && DicUnitType!=null)
            {
                OriginalUnit = FromFullName(OriginalUnitName, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);

                _UnitStat = OriginalUnit.UnitStat;
                _HP = OriginalUnit.MaxHP;
                _EN = OriginalUnit.MaxEN;

                SpriteMap = OriginalUnit.SpriteMap;
                SpriteUnit = OriginalUnit.SpriteUnit;
            }

            CombinedUnitName = BR.ReadString();
            if (!string.IsNullOrEmpty(CombinedUnitName) && DicUnitType != null)
            {
                CombinedUnit = FromFullName(CombinedUnitName, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }

            int ArrayCombiningUnitLength = BR.ReadInt32();
            ArrayCombiningUnitName = new string[ArrayCombiningUnitLength];

            for (int C = 0; C < ArrayCombiningUnitLength; ++C)
            {
                ArrayCombiningUnitName[C] = BR.ReadString();
            }

            FS.Close();
            BR.Close();
        }

        public override void ReinitializeMembers(Unit InitializedUnitBase)
        {
            UnitCombining Other = (UnitCombining)InitializedUnitBase;

            if (OriginalUnit == null)
            {
                OriginalUnit = FromFullName(OriginalUnitName, Params.Map.Content, Params.DicUnitType, Params.DicRequirement, Params.DicEffect, Params.DicAutomaticSkillTarget);
                CombinedUnit = FromFullName(CombinedUnitName, Params.Map.Content, Params.DicUnitType, Params.DicRequirement, Params.DicEffect, Params.DicAutomaticSkillTarget  );

                _UnitStat = OriginalUnit.UnitStat;
                _HP = OriginalUnit.MaxHP;
                _EN = OriginalUnit.MaxEN;

                SpriteMap = OriginalUnit.SpriteMap;
                SpriteUnit = OriginalUnit.SpriteUnit;
            }
        }

        public void Combine(Squad FoundCombiningUnit)
        {
            Combined = true;
            ListFoundCombiningUnit.Add(FoundCombiningUnit);
            Params.Map.ListPlayer[Params.Map.ActivePlayerIndex].ListSquad.Remove(FoundCombiningUnit);

            //Used to avoid updating HP, EN and PermanentTransformation on Init.
            double HPPercentage = HP / (double)MaxHP;
            double ENPercentage = EN / (double)MaxEN;

            _UnitStat = CombinedUnit.UnitStat;

            _HP = (int)(MaxHP * HPPercentage);
            _EN = (int)(MaxEN * ENPercentage);
            SpriteMap = CombinedUnit.SpriteMap;
            SpriteUnit = CombinedUnit.SpriteUnit;
        }

        public void Uncombine()
        {
            Combined = false;

            for (int S = ListFoundCombiningUnit.Count - 1; S >= 0; --S)
            {
                Squad ActiveSquad = ListFoundCombiningUnit[S];

                if (ActiveSquad.CurrentLeader.RelativePath != RelativePath)
                {
                    Vector3 FinalPosition;
                    Params.Map.GetEmptyPosition(ActiveSquad.Position, out FinalPosition);

                    ActiveSquad.SetPosition(FinalPosition);
                    Params.Map.ListPlayer[Params.Map.ActivePlayerIndex].ListSquad.Add(ActiveSquad);
                }
            }

            //Used to avoid updating HP, EN and PermanentTransformation on Init.
            double HPPercentage = HP / (double)MaxHP;
            double ENPercentage = EN / (double)MaxEN;

            _UnitStat = OriginalUnit.UnitStat;

            _HP = (int)(MaxHP * HPPercentage);
            _EN = (int)(MaxEN * ENPercentage);
            SpriteMap = OriginalUnit.SpriteMap;
            SpriteUnit = OriginalUnit.SpriteUnit;
        }

        public override List<ActionPanel> OnMenuSelect(int ActivePlayerIndex, Squad ActiveSquad, ActionPanelHolder ListActionMenuChoice)
        {
            if (!Combined)
            {
                int RemainingUnitsToFind = ArrayCombiningUnitName.Length;
                ListFoundCombiningUnit = new List<Squad>(ArrayCombiningUnitName.Length);

                if (ActiveSquad.UnitsAliveInSquad == 1)
                {
                    foreach (Squad OtherSquad in Params.Map.ListPlayer[Params.Map.ActivePlayerIndex].ListSquad)
                    {
                        if (ActiveSquad.UnitsAliveInSquad == 1)
                        {
                            foreach (string LeaderName in ArrayCombiningUnitName)
                            {
                                if (LeaderName.Contains(OtherSquad.CurrentLeader.RelativePath))
                                {
                                    ListFoundCombiningUnit.Add(OtherSquad);
                                }
                            }
                        }
                    }

                    if (ListFoundCombiningUnit.Count == ArrayCombiningUnitName.Length)
                    {
                        return new List<ActionPanel>() { new ActionPanelCombine(Params.Map, this) };
                    }
                }
            }
            else
            {
                return new List<ActionPanel>() { new ActionPanelSplit(Params.Map, this) };
            }

            return new List<ActionPanel>();
        }

        public override Unit FromFile(string Name, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            if (Params.Map == null)
            {
                return new UnitCombining(Name, Content, null, DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }
            else
            {
                return new UnitCombining(Name, Content, Params, Params.DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
        }

        protected override void DoQuickLoad(BinaryReader BR, ContentManager Content)
        {
        }

        public override void DoInit()
        {
        }

        public override GameScreen GetCustomizeScreen(List<Unit> ListPresentUnit, int SelectedUnitIndex, FormulaParser ActiveParser)
        {
            return null;
        }
    }
}
