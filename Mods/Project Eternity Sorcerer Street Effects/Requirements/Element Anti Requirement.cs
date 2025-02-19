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

        public SorcererStreetElementAntiRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Element Anti", GlobalContext)
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
                    if ((ActiveElement == ElementChoices.Air && GlobalContext.TerrainRestrictions.ListTerrainType[GlobalContext.DefenderTerrain.TerrainTypeIndex].Name != TerrainSorcererStreet.AirElement)
                        || (ActiveElement == ElementChoices.Fire && GlobalContext.TerrainRestrictions.ListTerrainType[GlobalContext.DefenderTerrain.TerrainTypeIndex].Name != TerrainSorcererStreet.FireElement)
                        || (ActiveElement == ElementChoices.Earth && GlobalContext.TerrainRestrictions.ListTerrainType[GlobalContext.DefenderTerrain.TerrainTypeIndex].Name != TerrainSorcererStreet.EarthElement)
                        || (ActiveElement == ElementChoices.Water && GlobalContext.TerrainRestrictions.ListTerrainType[GlobalContext.DefenderTerrain.TerrainTypeIndex].Name != TerrainSorcererStreet.WaterElement)
                        || (ActiveElement == ElementChoices.Neutral && GlobalContext.TerrainRestrictions.ListTerrainType[GlobalContext.DefenderTerrain.TerrainTypeIndex].Name != TerrainSorcererStreet.NeutralElement))
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
                CardAbilities Abilities = TargetCreature.GetCurrentAbilities(GlobalContext.EffectActivationPhase);

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
            SorcererStreetElementAntiRequirement NewRequirement = new SorcererStreetElementAntiRequirement(GlobalContext);

            NewRequirement.ArrayElement = ArrayElement;
            NewRequirement._Target = _Target;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetElementAntiRequirement CopyRequirement = (SorcererStreetElementAntiRequirement)Copy;

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
