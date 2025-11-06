using System;
using System.Linq;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetElementAntiRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent, Land }
        public enum ElementChoices { Neutral, Fire, Water, Earth, Air }

        private Targets _Target;

        public ElementChoices[] ArrayElement;

        public SorcererStreetElementAntiRequirement()
            : this(null)
        {
        }

        public SorcererStreetElementAntiRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street Element Anti", Params)
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
                    if ((ActiveElement == ElementChoices.Air && Params.GlobalContext.TerrainHolder.ListTerrainType[Params.GlobalContext.ActiveTerrain.TerrainTypeIndex] != TerrainSorcererStreet.AirElement)
                        || (ActiveElement == ElementChoices.Fire && Params.GlobalContext.TerrainHolder.ListTerrainType[Params.GlobalContext.ActiveTerrain.TerrainTypeIndex] != TerrainSorcererStreet.FireElement)
                        || (ActiveElement == ElementChoices.Earth && Params.GlobalContext.TerrainHolder.ListTerrainType[Params.GlobalContext.ActiveTerrain.TerrainTypeIndex] != TerrainSorcererStreet.EarthElement)
                        || (ActiveElement == ElementChoices.Water && Params.GlobalContext.TerrainHolder.ListTerrainType[Params.GlobalContext.ActiveTerrain.TerrainTypeIndex] != TerrainSorcererStreet.WaterElement)
                        || (ActiveElement == ElementChoices.Neutral && Params.GlobalContext.TerrainHolder.ListTerrainType[Params.GlobalContext.ActiveTerrain.TerrainTypeIndex] != TerrainSorcererStreet.NeutralElement))
                    {
                        return true;
                    }
                }

                return false;
            }
            else if (_Target == Targets.Self)
            {
                TargetCreature = Params.GlobalContext.SelfCreature.Creature;
            }
            else
            {
                TargetCreature = Params.GlobalContext.OpponentCreature.Creature;
            }

            foreach (ElementChoices ActiveElement in ArrayElement)
            {
                CardAbilities Abilities = TargetCreature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase);

                if ((ActiveElement == ElementChoices.Air && !Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Air))
                    || (ActiveElement == ElementChoices.Fire && !Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Fire))
                    || (ActiveElement == ElementChoices.Earth && !Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Earth))
                    || (ActiveElement == ElementChoices.Water && !Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Water))
                    || (ActiveElement == ElementChoices.Neutral && !Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Neutral)))
                {
                    return true;
                }
            }

            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetElementAntiRequirement NewRequirement = new SorcererStreetElementAntiRequirement(Params);

            NewRequirement.ArrayElement = ArrayElement;
            NewRequirement._Target = _Target;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetElementAntiRequirement CopyRequirement = Copy as SorcererStreetElementAntiRequirement;

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
