using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class KickbackEffect : TraitEffect
    {
        private int _Required = 1;
        private int _Penalty = 2;

        public KickbackEffect()
            : base("Kickback")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null || Roll.Who == null)
            {
                return;
            }

            if (Roll.Who.STR < Required)
            {
                Roll.RaiseCircumstancePenalty(Penalty);
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Required);
            BW.Write(Penalty);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            KickbackEffect Copy = new KickbackEffect();
            Copy.Required = BR.ReadInt32();
            Copy.Penalty = BR.ReadInt32();
            return Copy;
        }

        #region Properties

        [CategoryAttribute("Attack"),
        DescriptionAttribute("Strength modifier required to fire without the penalty."),
        DefaultValueAttribute(1)]
        public int Required
        {
            get { return _Required; }
            set { _Required = value; }
        }

        [CategoryAttribute("Attack"),
        DescriptionAttribute("Circumstance penalty when Strength is below that modifier."),
        DefaultValueAttribute(2)]
        public int Penalty
        {
            get { return _Penalty; }
            set { _Penalty = value; }
        }

        #endregion
    }
}
