using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetOnLoadRequirement : SorcererStreetRequirement
    {
        public SorcererStreetOnLoadRequirement()
            : this(null)
        {
        }

        public SorcererStreetOnLoadRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street On Load", Params)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }
        
        public override bool CanActivatePassive()
        {
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            return new SorcererStreetOnLoadRequirement(Params);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
