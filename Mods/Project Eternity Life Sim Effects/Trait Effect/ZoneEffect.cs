using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ZoneEffect : TraitEffect
    {
        private float _Feet = 10f;

        public ZoneEffect()
            : base("Zone")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null || Roll.Trigger == "List")
            {
                return;
            }

            Roll.IsZone = true;
            Roll.ZoneFeet = Feet;
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Feet);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            ZoneEffect Copy = new ZoneEffect();
            Copy.Feet = BR.ReadSingle();
            return Copy;
        }

        [CategoryAttribute("Area"), DefaultValueAttribute(10f)]
        public float Feet { get { return _Feet; } set { _Feet = value; } }
    }
}
