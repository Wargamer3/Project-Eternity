using System;
using System.IO;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionUnlockSkillRequirement : UnlockRequirementEvaluator
    {
        public const string UnlockTypeName = "Skill Level";

        public string SkillName;
        public byte UnlockLevel;

        public ActionUnlockSkillRequirement()
            : base(UnlockTypeName)
        {
        }

        private ActionUnlockSkillRequirement(BinaryReader BR)
            : this()
        {
            SkillName = BR.ReadString();
            UnlockLevel = BR.ReadByte();
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            throw new NotImplementedException();
        }

        public override bool CanBeUnlocked()
        {
            CharacterSkill NeededSkill;
            if (Params.Owner.DicSkillByName.TryGetValue(SkillName, out NeededSkill))
            {
                return NeededSkill.Level >= UnlockLevel;
            }

            return false;
        }

        public override UnlockRequirementEvaluator Copy()
        {
            return new ActionUnlockSkillRequirement();
        }

        public override UnlockRequirementEvaluator LoadCopy(BinaryReader BR)
        {
            return new ActionUnlockSkillRequirement(BR);
        }
    }
}
