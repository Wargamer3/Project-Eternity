using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class ShootRequirement : TripleThunderRobotRequirement
    {
        public static string Name = "Shoot";

        public ShootRequirement()
            : this(null)
        {
        }

        public ShootRequirement(TripleThunderRobotContext GlobalContext)
            : base(Name, GlobalContext)
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
            return new ShootRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
