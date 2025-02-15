using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetTerriotryRequirement : SorcererStreetBattleRequirement
    {
        private string _Price;

        public SorcererStreetTerriotryRequirement()
            : this(null)
        {
        }

        public SorcererStreetTerriotryRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Territory", GlobalContext)
        {
            _Price = string.Empty;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_Price);
        }

        protected override void Load(BinaryReader BR)
        {
            _Price = BR.ReadString();
        }

        public override bool CanActivatePassive()
        {
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetTerriotryRequirement NewRequirement = new SorcererStreetTerriotryRequirement(GlobalContext);

            NewRequirement._Price = _Price;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetTerriotryRequirement CopyRequirement = (SorcererStreetTerriotryRequirement)Copy;

            _Price = CopyRequirement._Price;
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public string Price
        {
            get
            {
                return _Price;
            }
            set
            {
                _Price = value;
            }
        }

        #endregion
    }
}
