using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units.Normal;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Units.Transforming
{
    public class UnitTransforming : DeathmatchUnit
    {
        public override string UnitTypeName => "Transforming";

        public struct TransformationInformations
        {
            public Unit UnitTransformed;
            public string TransformingUnitName;
            public int WillRequirement;
            public int TurnLimit;
            public bool PermanentTransformation;
            // If Transformed and that skill is not active, cancel the transformation.
            private string RequiredActiveSkillToTransform;

            public TransformationInformations(Unit UnitTransformed, string TransformingUnitName)
            {
                this.UnitTransformed = UnitTransformed;
                this.TransformingUnitName = TransformingUnitName;
                this.WillRequirement = -1;
                this.TurnLimit = -1;
                this.PermanentTransformation = false;
                this.RequiredActiveSkillToTransform = string.Empty;
            }

            public override string ToString()
            {
                return TransformingUnitName;
            }
        }

        public TransformationInformations[] ArrayTransformingUnit;
        public int ActiveUnit;
        public int TimeRemaining;
        public bool PermanentTransformation;

        private double HPPercentage;
        private double ENPercentage;

        public UnitTransforming()
            : base(null)
        { }

        public UnitTransforming(DeathmatchMap Map)
            : base(Map)
        { }
        
        public UnitTransforming(string Name, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : this(Name, Content, null, DicRequirement, DicEffect, DicAutomaticSkillTarget)
        {
        }

        public UnitTransforming(string Name, ContentManager Content, DeathmatchMap Map, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : base(Name, Map)
        {
            this.ItemName = Name;
            PermanentTransformation = false;
            HPPercentage = 1;
            ENPercentage = 1;
            ActiveUnit = 0;
            TimeRemaining = -1;

            FileStream FS = new FileStream("Content/Units/Transforming/" + Name + ".peu", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            int ListTransformingUnitCount = BR.ReadInt32();

            ArrayTransformingUnit = new TransformationInformations[ListTransformingUnitCount];

            for (int U = 0; U < ListTransformingUnitCount; ++U)
            {
                ArrayTransformingUnit[U] = new TransformationInformations();

                string TransformingUnitPath = BR.ReadString();
                ArrayTransformingUnit[U].TransformingUnitName = TransformingUnitPath;

                ArrayTransformingUnit[U].UnitTransformed = new UnitNormal(TransformingUnitPath, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget);

                ArrayTransformingUnit[U].WillRequirement = BR.ReadInt32();
                ArrayTransformingUnit[U].TurnLimit = BR.ReadInt32();
                ArrayTransformingUnit[U].PermanentTransformation = BR.ReadBoolean();
            }

            FS.Close();
            BR.Close();

            if (ArrayTransformingUnit.Length > 0)
            {
                _HP = ArrayTransformingUnit[0].UnitTransformed.MaxHP;
                _EN = ArrayTransformingUnit[0].UnitTransformed.MaxEN;
                ChangeUnit(ActiveUnit);
            }
        }

        public override void ReinitializeMembers(Unit InitializedUnitBase)
        {
            UnitTransforming Other = (UnitTransforming)InitializedUnitBase;
            Map = Other.Map;
        }

        public void ChangeUnit(int ActiveUnit)
        {//Used to avoid updating HP, EN and PermanentTransformation on Init.
            if (this.ActiveUnit != ActiveUnit)
            {
                HPPercentage = HP / (double)MaxHP;
                ENPercentage = EN / (double)MaxEN;
                if (ArrayTransformingUnit[ActiveUnit].PermanentTransformation)
                    PermanentTransformation = true;

                this.ActiveUnit = ActiveUnit;
            }
            
            _UnitStat = ArrayTransformingUnit[ActiveUnit].UnitTransformed.UnitStat;
            
            _HP = (int)(MaxHP * HPPercentage);
            _EN = (int)(MaxEN * ENPercentage);
            SpriteMap = ArrayTransformingUnit[ActiveUnit].UnitTransformed.SpriteMap;
            SpriteUnit = ArrayTransformingUnit[ActiveUnit].UnitTransformed.SpriteUnit;
        }

        public bool CanTransform(int ActiveUnit, Unit CurrentWingmanA, Unit CurrentWingmanB)
        {
            if (ArrayTransformingUnit[ActiveUnit].WillRequirement >= 0 && PilotMorale < ArrayTransformingUnit[ActiveUnit].WillRequirement)
                return false;

            if (ArrayTransformingUnit[ActiveUnit].UnitTransformed.ListTerrainChoices.Contains(UnitStats.TerrainAir) &&
                !ArrayTransformingUnit[ActiveUnit].UnitTransformed.ListTerrainChoices.Contains(UnitStats.TerrainLand))
            {
                if (CurrentWingmanA != null)
                {
                    if (!CurrentWingmanA.ListTerrainChoices.Contains(UnitStats.TerrainAir))
                        return false;
                    else
                    {
                        if (CurrentWingmanB != null)
                        {
                            if (!CurrentWingmanB.ListTerrainChoices.Contains(UnitStats.TerrainAir))
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        public override void OnTurnEnd(int ActivePlayerIndex, Squad ActiveSquad)
        {
            if (TimeRemaining >= 1)
            {
                if (--TimeRemaining <= 0)
                    ChangeUnit(0);
            }
        }

        public override List<ActionPanel> OnMenuMovement(int ActivePlayerIndex, Squad ActiveSquad, ActionPanelHolder ListActionMenuChoice)
        {
            if (Boosts.PostMovementModifier.Transform)
            {
                return new List<ActionPanel>() { new ActionPanelTransform(Map, ActivePlayerIndex, ActiveSquad) };
            }

            return null;
        }

        public override List<ActionPanel> OnMenuSelect(int ActivePlayerIndex, Squad ActiveSquad, ActionPanelHolder ListActionMenuChoice)
        {
            int NumberOfTransformingUnitsInSquad = 0;
            for (int U = Map.ActiveSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                UnitTransforming ActiveUnit = (UnitTransforming)Map.ActiveSquad[U];
                if (ActiveUnit != null)
                {
                    if (!ActiveUnit.PermanentTransformation)
                        ++NumberOfTransformingUnitsInSquad;
                }
            }

            if (NumberOfTransformingUnitsInSquad >= 1)
            {
                return new List<ActionPanel>() { new ActionPanelTransform(Map, ActivePlayerIndex, ActiveSquad) };
            }

            return new List<ActionPanel>();
        }

        public override Unit FromFile(string Name, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            return new UnitTransforming(Name, Content, Map, DicRequirement, DicEffect, DicAutomaticSkillTarget);
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
            BW.Write(ActiveUnit);
            BW.Write(TimeRemaining);
            BW.Write(PermanentTransformation);
        }

        protected override void DoQuickLoad(BinaryReader BR, ContentManager Content)
        {
            ActiveUnit = BR.ReadInt32();
            TimeRemaining = BR.ReadInt32();
            PermanentTransformation = BR.ReadBoolean();

            ChangeUnit(ActiveUnit);
        }

        public override void DoInit()
        {
        }

        public override GameScreens.GameScreen GetCustomizeScreen()
        {
            return null;
        }
    }
}
