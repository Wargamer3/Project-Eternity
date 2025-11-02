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
        public enum ElementChoices { DifferentFromOpponent, Neutral, Fire, Water, Earth, Air, DifferentFromTerritory }

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
                    if ((ActiveElement == ElementChoices.Air && GlobalContext.TerrainHolder.ListTerrainType[GlobalContext.ActiveTerrain.TerrainTypeIndex] == TerrainSorcererStreet.AirElement)
                        || (ActiveElement == ElementChoices.Fire && GlobalContext.TerrainHolder.ListTerrainType[GlobalContext.ActiveTerrain.TerrainTypeIndex] == TerrainSorcererStreet.FireElement)
                        || (ActiveElement == ElementChoices.Earth && GlobalContext.TerrainHolder.ListTerrainType[GlobalContext.ActiveTerrain.TerrainTypeIndex] == TerrainSorcererStreet.EarthElement)
                        || (ActiveElement == ElementChoices.Water && GlobalContext.TerrainHolder.ListTerrainType[GlobalContext.ActiveTerrain.TerrainTypeIndex] == TerrainSorcererStreet.WaterElement)
                        || (ActiveElement == ElementChoices.Neutral && GlobalContext.TerrainHolder.ListTerrainType[GlobalContext.ActiveTerrain.TerrainTypeIndex] == TerrainSorcererStreet.NeutralElement))
                    {
                        return true;
                    }
                    else if (ActiveElement == ElementChoices.DifferentFromTerritory)
                    {
                        CardAbilities Abilities = GlobalContext.ActiveTerrain.DefendingCreature.GetCurrentAbilities(GlobalContext.EffectActivationPhase);
                        bool ElementMatch = false;

                        if ((Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Air) && GlobalContext.TerrainHolder.ListTerrainType[GlobalContext.ActiveTerrain.TerrainTypeIndex] == TerrainSorcererStreet.AirElement)
                            || (Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Fire) && GlobalContext.TerrainHolder.ListTerrainType[GlobalContext.ActiveTerrain.TerrainTypeIndex] == TerrainSorcererStreet.FireElement)
                            || (Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Earth) && GlobalContext.TerrainHolder.ListTerrainType[GlobalContext.ActiveTerrain.TerrainTypeIndex] == TerrainSorcererStreet.EarthElement)
                            || (Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Water) && GlobalContext.TerrainHolder.ListTerrainType[GlobalContext.ActiveTerrain.TerrainTypeIndex] == TerrainSorcererStreet.WaterElement)
                            || (Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Neutral) && GlobalContext.TerrainHolder.ListTerrainType[GlobalContext.ActiveTerrain.TerrainTypeIndex] == TerrainSorcererStreet.NeutralElement))
                        {
                            ElementMatch = true;
                        }

                        return !ElementMatch;
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
                CardAbilities Abilities = TargetCreature.GetCurrentAbilities(GlobalContext.EffectActivationPhase);
                if ((ActiveElement == ElementChoices.Air && Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Air))
                    || (ActiveElement == ElementChoices.Fire && Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Fire))
                    || (ActiveElement == ElementChoices.Earth && Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Earth))
                    || (ActiveElement == ElementChoices.Water && Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Water))
                    || (ActiveElement == ElementChoices.Neutral && Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Neutral)))
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

                    foreach (CreatureCard.ElementalAffinity ActiveAffinity in Abilities.ArrayElementAffinity)
                    {
                        if (!OtherCreature.GetCurrentAbilities(GlobalContext.EffectActivationPhase).ArrayElementAffinity.Contains(ActiveAffinity))
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
            SorcererStreetElementRequirement CopyRequirement = Copy as SorcererStreetElementRequirement;

            if (CopyRequirement != null)
            {
                ArrayElement = CopyRequirement.ArrayElement;
                _Target = CopyRequirement._Target;
            }
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
