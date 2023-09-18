using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ChangeCreatureMovementEffect : SorcererStreetEffect
    {// Able to move to any vacant land in the area.
        public enum ElementalAffinity { Any, Neutral, Fire, Water, Earth, Air }

        public static string Name = "Sorcerer Street Change Creature Movement";

        private ElementalAffinity _SymbolTypeType;

        public ChangeCreatureMovementEffect()
            : base(Name, false)
        {
        }

        public ChangeCreatureMovementEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
            _SymbolTypeType = (ElementalAffinity)BR.ReadByte();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_SymbolTypeType);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            ChangeCreatureMovementEffect NewEffect = new ChangeCreatureMovementEffect(Params);

            NewEffect._SymbolTypeType = _SymbolTypeType;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ChangeCreatureMovementEffect NewEffect = (ChangeCreatureMovementEffect)Copy;

            _SymbolTypeType = NewEffect._SymbolTypeType;
        }

        #region Properties

        [CategoryAttribute(""),
        DescriptionAttribute("How to destroy cards."),
        DefaultValueAttribute(0)]
        public ElementalAffinity SymbolTypeType
        {
            get
            {
                return _SymbolTypeType;
            }
            set
            {
                _SymbolTypeType = value;
            }
        }

        #endregion
    }
}
