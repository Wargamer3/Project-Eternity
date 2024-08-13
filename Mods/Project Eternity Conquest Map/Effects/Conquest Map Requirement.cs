using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public abstract class ConquestSkillRequirement : UnitSkillRequirement
    {
        public static string BattleStartRequirementName = "Battle Start Requirement";
        public static string BeforeAttackRequirementName = "Before Attack Requirement";
        public static string BeforeGettingAttackedRequirementName = "Before Getting Attacked Requirement";
        public static string BeforeHitRequirementName = "Before Hit Requirement";
        public static string BeforeGettingHitRequirementName = "Before Getting Hit Requirement";
        public static string BeforeMissRequirementName = "Before Miss Requirement";
        public static string BeforeGettingMissedRequirementName = "Before Getting Missed Requirement";
        public static string BeforeGettingDestroyedRequirementName = "Before Getting Destroyed Requirement";
        public static string AfterAttackRequirementName = "After Attack Requirement";
        public static string WithinAttackRangeRequirementName = "Within Attack Range Requirement";
        public static string AfterGettingAttackedRequirementName = "After Getting Attacked Requirement";
        public static string AfterHitRequirementName = "After Hit Requirement";
        public static string AfterGettingHitRequirementName = "After Getting Hit Requirement";
        public static string AfterMissRequirementName = "After Miss Requirement";
        public static string AfterGettingMissedRequirementName = "After Getting Missed Requirement";
        public static string AfterGettingDestroyRequirementName = "After Getting Destroy Requirement";
        public static string VSPilotRequirementName = "VS Pilot Requirement";
        public static string VSUnitRequirementName = "VS Unit Requirement";
        public static string VSFlyingUnitRequirementName = "VS Flying Unit Requirement";
        public static string VSLandUnitRequirementName = "VS Land Unit Requirement";
        public static string VSWaterUnitRequirementName = "VS Water Unit Requirement";
        public static string VSUnderwaterUnitRequirementName = "VS Underwater Unit Requirement";
        public static string AttackUsedRequirementName = "Attack Used Requirement";
        public static string AttackDefendedRequirementName = "Attack Defended Requirement";
        public static string SupportAttackRequirementName = "Support Attack Requirement";
        public static string SupportDefendRequirementName = "Support Defend Requirement";

        private ConquestParams _Params;
        protected ConquestParams Params { get { return _Params; } }

        public ConquestSkillRequirement(string SkillRequirementName, ConquestParams Params)
            : base(SkillRequirementName)
        {
            this._Params = Params;
        }

        public override bool CanActivatePassive()
        {
            return false;
        }

        protected override void DoReload(string ParamsID)
        {
            this._Params = ConquestParams.DicParams[ParamsID];
        }
    }
}
