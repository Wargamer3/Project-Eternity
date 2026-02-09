using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using static ProjectEternity.GameScreens.SorcererStreetScreen.CreatureCard;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ChangeTerrainElementEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Change Terrain Element";

        private ElementalAffinity _Element;

        public ChangeTerrainElementEffect()
            : base(Name, false)
        {
            _Element = ElementalAffinity.Neutral;
        }

        public ChangeTerrainElementEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _Element = ElementalAffinity.Neutral;
        }
        
        protected override void Load(BinaryReader BR)
        {
            _Element = (ElementalAffinity)BR.ReadByte();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_Element);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            switch (_Element)
            {
                case ElementalAffinity.Air:
                    Params.GlobalContext.ActiveTerrain.TerrainTypeIndex = (byte)Params.Map.TerrainHolder.ListTerrainType.IndexOf(TerrainSorcererStreet.AirElement);
                    break;
                case ElementalAffinity.Earth:
                    Params.GlobalContext.ActiveTerrain.TerrainTypeIndex = (byte)Params.Map.TerrainHolder.ListTerrainType.IndexOf(TerrainSorcererStreet.EarthElement);
                    break;
                case ElementalAffinity.Fire:
                    Params.GlobalContext.ActiveTerrain.TerrainTypeIndex = (byte)Params.Map.TerrainHolder.ListTerrainType.IndexOf(TerrainSorcererStreet.FireElement);
                    break;
                case ElementalAffinity.Water:
                    Params.GlobalContext.ActiveTerrain.TerrainTypeIndex = (byte)Params.Map.TerrainHolder.ListTerrainType.IndexOf(TerrainSorcererStreet.WaterElement);
                    break;
                case ElementalAffinity.Neutral:
                    Params.GlobalContext.ActiveTerrain.TerrainTypeIndex = (byte)Params.Map.TerrainHolder.ListTerrainType.IndexOf(TerrainSorcererStreet.NeutralElement);
                    break;
            }

            Params.Map.Reset();
            Params.Map.UpdateTotalMagic();
            return "Terrain element changed to " + Element;
        }

        protected override BaseEffect DoCopy()
        {
            ChangeTerrainElementEffect NewEffect = new ChangeTerrainElementEffect(Params);

            NewEffect._Element = _Element;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ChangeTerrainElementEffect NewEffect = (ChangeTerrainElementEffect)Copy;

            _Element = NewEffect._Element;
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public ElementalAffinity Element
        {
            get
            {
                return _Element;
            }
            set
            {
                _Element = value;
            }
        }

        #endregion
    }
}
