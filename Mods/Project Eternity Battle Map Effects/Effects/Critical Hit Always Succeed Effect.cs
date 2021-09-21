using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class CriticalHitAlwaysSucceedEffect : SkillEffect
    {
        public static string Name = "Critical Hit Always Succeed Effect";

        public CriticalHitAlwaysSucceedEffect()
            : base(Name, true)
        {
        }

        public CriticalHitAlwaysSucceedEffect(UnitEffectParams Params)
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
            Params.LocalContext.EffectTargetUnit.Boosts.CriticalAlwaysSucceed = true;

            return string.Empty;
        }

        protected override void ReactivateEffect()
        {
            Params.LocalContext.EffectTargetUnit.Boosts.CriticalAlwaysSucceed = true;
        }

        protected override BaseEffect DoCopy()
        {
            CriticalHitAlwaysSucceedEffect NewEffect = new CriticalHitAlwaysSucceedEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
