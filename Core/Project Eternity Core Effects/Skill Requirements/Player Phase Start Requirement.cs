using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.Core.Effects
{
    public sealed class PlayerPhaseStartRequirement : ActiveSkillRequirement
    {
        public static string Name = "Player Phase Start Requirement";

        public PlayerPhaseStartRequirement()
            : this(null)
        {
        }

        public PlayerPhaseStartRequirement(UnitEffectContext GlobalContext)
            : base(Name, GlobalContext)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        public override BaseSkillRequirement Copy()
        {
            PlayerPhaseStartRequirement NewSkillEffect = new PlayerPhaseStartRequirement(GlobalContext);

            return NewSkillEffect;
        }
    }
}
