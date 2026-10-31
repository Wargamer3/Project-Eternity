using System.IO;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class FinesseEffect : TraitEffect
    {
        public FinesseEffect()
            : base("Finesse")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null || Roll.Who == null || Roll.Weapon == null)
            {
                return;
            }

            if (Roll.Weapon.WeaponType == Weapon.WeaponTypes.Range)
            {
                return;
            }

            if (Roll.Who.DEX > Roll.AttackAbility)
            {
                Roll.AttackAbility = Roll.Who.DEX;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            return new FinesseEffect();
        }
    }
}
