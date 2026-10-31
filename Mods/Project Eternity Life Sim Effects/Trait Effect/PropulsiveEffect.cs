using System.IO;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PropulsiveEffect : TraitEffect
    {
        public PropulsiveEffect()
            : base("Propulsive")
        {
        }

        public override void AlterDamage(StrikeRoll Roll)
        {
            if (Roll == null || Roll.Who == null || Roll.Weapon == null)
            {
                return;
            }

            if (Roll.Weapon.WeaponType != Weapon.WeaponTypes.Range)
            {
                return;
            }

            int Strength = Roll.Who.STR;
            Roll.DamageAbility = Strength < 0 ? Strength : Strength / 2;
        }

        protected override void DoWrite(BinaryWriter BW)
        {
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            return new PropulsiveEffect();
        }
    }
}
