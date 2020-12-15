using ProjectEternity.Core.Magic;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Magic
{
    public abstract class TripleThunderMagicCore : MagicCore
    {
        protected TripleThunderAttackParams AttackParams;

        protected TripleThunderMagicCore(string Name, int BasePower, int RequiredMana, int Radius,
            MagicUserParams Params, TripleThunderAttackParams AttackParams)
            : base(Name, BasePower, RequiredMana, Radius, Params)
        {
            this.AttackParams = new TripleThunderAttackParams(AttackParams);
        }
    }
}
