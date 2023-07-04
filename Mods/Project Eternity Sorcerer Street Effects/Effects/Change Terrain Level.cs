using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ChangeTerrainLevelEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Change Terrain Level";

        private int _LevelIncrease;

        public ChangeTerrainLevelEffect()
            : base(Name, false)
        {
            _LevelIncrease = 1;
        }

        public ChangeTerrainLevelEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
            _LevelIncrease = BR.ReadByte();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_LevelIncrease);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            return "Terrain Level changed " + string.Join(",", _LevelIncrease);
        }

        protected override BaseEffect DoCopy()
        {
            ChangeTerrainLevelEffect NewEffect = new ChangeTerrainLevelEffect(Params);

            NewEffect._LevelIncrease = _LevelIncrease;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public int LevelIncrease
        {
            get
            {
                return _LevelIncrease;
            }
            set
            {
                _LevelIncrease = Math.Max(0, Math.Min(100, value));
            }
        }

        #endregion
    }
}
