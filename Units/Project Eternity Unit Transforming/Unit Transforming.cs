﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.ControlHelper;
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
        public int ActiveUnitIndex;
        public int TimeRemaining;
        public bool PermanentTransformation;

        private double HPPercentage;
        private double ENPercentage;

        private Character[] ArrayOriginalCharacter;

        public UnitTransforming()
            : base(null)
        { }

        public UnitTransforming(DeathmatchParams Params)
            : base(Params)
        { }
        
        public UnitTransforming(string Name, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : this(Name, Content, null, DicRequirement, DicEffect, DicAutomaticSkillTarget)
        {
        }

        public UnitTransforming(string Name, ContentManager Content, DeathmatchParams Params, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : base(Name, Params)
        {
            this.ItemName = Name;
            PermanentTransformation = false;
            HPPercentage = 1;
            ENPercentage = 1;
            ActiveUnitIndex = 0;
            TimeRemaining = -1;
            ArrayOriginalCharacter = new Character[0];

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
                ChangeUnit(ActiveUnitIndex);
            }
        }

        public override void ReinitializeMembers(Unit InitializedUnitBase)
        {
            UnitTransforming Other = (UnitTransforming)InitializedUnitBase;
        }

        public void ChangeUnit(int NewActiveUnitIndex)
        {//Used to avoid updating HP, EN and PermanentTransformation on Init.
            if (this.ActiveUnitIndex != NewActiveUnitIndex)
            {
                HPPercentage = HP / (double)MaxHP;
                ENPercentage = EN / (double)MaxEN;
                if (ArrayTransformingUnit[NewActiveUnitIndex].PermanentTransformation)
                    PermanentTransformation = true;

                this.ActiveUnitIndex = NewActiveUnitIndex;
            }

            if (ArrayOriginalCharacter.Length != ArrayCharacterActive.Length)
            {
                ArrayOriginalCharacter = new Character[ArrayCharacterActive.Length];
                for (int C = 0; C < ArrayCharacterActive.Length; ++C)
                {
                    ArrayOriginalCharacter[C] = ArrayCharacterActive[C];
                }
            }

            _UnitStat = ArrayTransformingUnit[NewActiveUnitIndex].UnitTransformed.UnitStat;

            if (ArrayCharacterActive.Length > 0)
            {
                ArrayCharacterActive[0] = ArrayOriginalCharacter[NewActiveUnitIndex];
                int NextCharacterChangeIndex = 1;
                for (int C = 0; C < ArrayOriginalCharacter.Length; ++C)
                {
                    if (C == NewActiveUnitIndex)
                        continue;

                    ArrayCharacterActive[NextCharacterChangeIndex] = ArrayOriginalCharacter[C];
                    NextCharacterChangeIndex++;
                }
            }

            _HP = (int)(MaxHP * HPPercentage);
            _EN = (int)(MaxEN * ENPercentage);
            SpriteMap = ArrayTransformingUnit[NewActiveUnitIndex].UnitTransformed.SpriteMap;
            SpriteUnit = ArrayTransformingUnit[NewActiveUnitIndex].UnitTransformed.SpriteUnit;
        }

        public bool CanTransform(int ActiveUnit, List<Unit> ListWingman)
        {
            if (ArrayTransformingUnit[ActiveUnit].WillRequirement >= 0 && PilotMorale < ArrayTransformingUnit[ActiveUnit].WillRequirement)
                return false;

            if (ArrayTransformingUnit[ActiveUnit].UnitTransformed.DicRankByMovement.ContainsKey(UnitStats.TerrainAirIndex) &&
                !ArrayTransformingUnit[ActiveUnit].UnitTransformed.DicRankByMovement.ContainsKey(UnitStats.TerrainLandIndex))
            {
                foreach (Unit ActiveWingman in ListWingman)
                {
                    if (!ActiveWingman.DicRankByMovement.ContainsKey(UnitStats.TerrainAirIndex))
                        return false;
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

        public override List<ActionPanel> OnInputPressed(int ActivePlayerIndex, Squad ActiveSquad, ActionPanelHolder ListActionMenuChoice)
        {
            if (InputHelper.InputCommand1Pressed())
            {
                int NextIndex = ActiveUnitIndex + 1;
                if (NextIndex >= ArrayTransformingUnit.Length)
                {
                    NextIndex = 0;
                }
                ChangeUnit(NextIndex);
            }

            return new List<ActionPanel>();
        }

        public override List<ActionPanel> OnMenuMovement(int ActivePlayerIndex, Squad ActiveSquad, ActionPanelHolder ListActionMenuChoice)
        {
            if (Boosts.PostMovementModifier.Transform)
            {
                return new List<ActionPanel>() { new ActionPanelTransform(Params.Map, ActivePlayerIndex, ActiveSquad) };
            }

            return new List<ActionPanel>();
        }

        public override List<ActionPanel> OnMenuSelect(int ActivePlayerIndex, Squad ActiveSquad, ActionPanelHolder ListActionMenuChoice)
        {
            int NumberOfTransformingUnitsInSquad = 0;
            for (int U = Params.Map.ActiveSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                UnitTransforming ActiveUnit = (UnitTransforming)Params.Map.ActiveSquad[U];
                if (ActiveUnit != null)
                {
                    if (!ActiveUnit.PermanentTransformation)
                        ++NumberOfTransformingUnitsInSquad;
                }
            }

            if (NumberOfTransformingUnitsInSquad >= 1)
            {
                return new List<ActionPanel>() { new ActionPanelTransform(Params.Map, ActivePlayerIndex, ActiveSquad) };
            }

            return new List<ActionPanel>();
        }

        public override Unit FromFile(string Name, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            return new UnitTransforming(Name, Content, Params, DicRequirement, DicEffect, DicAutomaticSkillTarget);
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
            BW.Write(ActiveUnitIndex);
            BW.Write(TimeRemaining);
            BW.Write(PermanentTransformation);
            for (int C = 0; C < ArrayOriginalCharacter.Length; ++C)
            {
                BW.Write(ArrayOriginalCharacter[C].Name);
            }
        }

        protected override void DoQuickLoad(BinaryReader BR, ContentManager Content)
        {
            ActiveUnitIndex = BR.ReadInt32();
            TimeRemaining = BR.ReadInt32();
            PermanentTransformation = BR.ReadBoolean();

            ArrayOriginalCharacter = new Character[ArrayCharacterActive.Length];
            for (int C = 0; C < ArrayOriginalCharacter.Length; ++C)
            {
                string CharacterName = BR.ReadString();
                for (int C2 = 0; C2 < ArrayCharacterActive.Length; ++C2)
                {
                    if (ArrayCharacterActive[C2].Name == CharacterName)
                    {
                        ArrayOriginalCharacter[C] = ArrayCharacterActive[C2];
                        break;
                    }
                }
            }

            ChangeUnit(ActiveUnitIndex);
        }

        public override void DoInit()
        {
            ChangeUnit(0);
        }

        public override GameScreens.GameScreen GetCustomizeScreen(List<Unit> ListPresentUnit, int SelectedUnitIndex, FormulaParser ActiveParser)
        {
            return null;
        }
    }
}
