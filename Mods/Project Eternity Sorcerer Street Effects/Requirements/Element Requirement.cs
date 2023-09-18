using System;
using System.Linq;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetElementRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent, Land }
        public enum ElementChoices { DifferentFromOpponent, Neutral, Fire, Water, Earth, Air }

        private Targets _Target;

        public ElementChoices[] ArrayElement;

        public SorcererStreetElementRequirement()
            : this(null)
        {
        }

        public SorcererStreetElementRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Element", GlobalContext)
        {
            _Target = Targets.Self;
            ArrayElement = new ElementChoices[0];
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)ArrayElement.Length);
            for (int A = 0; A < ArrayElement.Length; ++A)
            {
                BW.Write((byte)ArrayElement[A]);
            }
        }

        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            int ArrayAffinityLength = BR.ReadByte();
            ArrayElement = new ElementChoices[ArrayAffinityLength];
            for (int A = 0; A < ArrayAffinityLength; ++A)
            {
                ArrayElement[A] = (ElementChoices)BR.ReadByte();
            }
        }

        public override bool CanActivatePassive()
        {
            CreatureCard TargetCreature;

            if (_Target == Targets.Land)
            {
                foreach (ElementChoices ActiveElement in ArrayElement)
                {
                    if ((ActiveElement == ElementChoices.Air && GlobalContext.TerrainRestrictions.ListTerrainType[GlobalContext.DefenderTerrain.TerrainTypeIndex].Name == TerrainSorcererStreet.AirElement)
                        || (ActiveElement == ElementChoices.Fire && GlobalContext.TerrainRestrictions.ListTerrainType[GlobalContext.DefenderTerrain.TerrainTypeIndex].Name == TerrainSorcererStreet.AirElement)
                        || (ActiveElement == ElementChoices.Earth && GlobalContext.TerrainRestrictions.ListTerrainType[GlobalContext.DefenderTerrain.TerrainTypeIndex].Name == TerrainSorcererStreet.AirElement)
                        || (ActiveElement == ElementChoices.Water && GlobalContext.TerrainRestrictions.ListTerrainType[GlobalContext.DefenderTerrain.TerrainTypeIndex].Name == TerrainSorcererStreet.AirElement)
                        || (ActiveElement == ElementChoices.Neutral && GlobalContext.TerrainRestrictions.ListTerrainType[GlobalContext.DefenderTerrain.TerrainTypeIndex].Name == TerrainSorcererStreet.AirElement))
                    {
                        return true;
                    }
                }

                return false;
            }
            else if (_Target == Targets.Self)
            {
                TargetCreature = GlobalContext.SelfCreature.Creature;
            }
            else
            {
                TargetCreature = GlobalContext.OpponentCreature.Creature;
            }

            foreach (ElementChoices ActiveElement in ArrayElement)
            {
                if ((ActiveElement == ElementChoices.Air && TargetCreature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Air))
                    || (ActiveElement == ElementChoices.Fire && TargetCreature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Fire))
                    || (ActiveElement == ElementChoices.Earth && TargetCreature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Earth))
                    || (ActiveElement == ElementChoices.Water && TargetCreature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Water))
                    || (ActiveElement == ElementChoices.Neutral && TargetCreature.BattleAbilities.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Neutral)))
                {
                    return true;
                }

                if (ActiveElement == ElementChoices.DifferentFromOpponent)
                {
                    CreatureCard OtherCreature;

                    if (_Target == Targets.Self)
                    {
                        OtherCreature = GlobalContext.OpponentCreature.Creature;
                    }
                    else
                    {
                        OtherCreature = GlobalContext.SelfCreature.Creature;
                    }

                    foreach (CreatureCard.ElementalAffinity ActiveAffinity in TargetCreature.BattleAbilities.ArrayAffinity)
                    {
                        if (!OtherCreature.BattleAbilities.ArrayAffinity.Contains(ActiveAffinity))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetElementRequirement NewRequirement = new SorcererStreetElementRequirement(GlobalContext);

            NewRequirement.ArrayElement = ArrayElement;
            NewRequirement._Target = _Target;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetElementRequirement CopyRequirement = (SorcererStreetElementRequirement)Copy;

            ArrayElement = CopyRequirement.ArrayElement;
            _Target = CopyRequirement._Target;
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public Targets Target
        {
            get
            {
                return _Target;
            }
            set
            {
                _Target = value;
            }
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public ElementChoices[] Elements
        {
            get
            {
                return ArrayElement;
            }
            set
            {
                ArrayElement = value;
            }
        }

        #endregion
    }
}
