using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class CriticalHitAlwaysFailEffect : SkillEffect
    {
        public static string Name = "Critical Hit Fail Effect";

        public CriticalHitAlwaysFailEffect()
            : base(Name, true)
        {
        }

        public CriticalHitAlwaysFailEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
        }

        protected override void Save(BinaryWriter BW)
        {
        }

        protected override string DoExecuteEffect()
        {
            Params.LocalContext.EffectTargetUnit.Boosts.CriticalAlwaysFail = true;
            return string.Empty;
        }

        protected override void ReactivateEffect()
        {
            Params.LocalContext.EffectTargetUnit.Boosts.CriticalAlwaysFail = true;
        }

        protected override BaseEffect DoCopy()
        {
            CriticalHitAlwaysFailEffect NewEffect = new CriticalHitAlwaysFailEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
