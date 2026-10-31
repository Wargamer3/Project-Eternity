using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PenaltyOverrideEffect : TraitEffect
    {
        private int _Second = 4;
        private int _Later = 8;

        public PenaltyOverrideEffect()
            : base("Penalty override")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null || Roll.StrikesAlreadyMade <= 0)
            {
                return;
            }

            int Next = Roll.StrikesAlreadyMade == 1 ? Second : Later;
            if (Next < Roll.UntypedPenalty)
            {
                Roll.UntypedPenalty = Next;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Second);
            BW.Write(Later);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            PenaltyOverrideEffect Copy = new PenaltyOverrideEffect();
            Copy.Second = BR.ReadInt32();
            Copy.Later = BR.ReadInt32();
            return Copy;
        }

        #region Properties

        [CategoryAttribute("Penalty"),
        DescriptionAttribute("Penalty on the second attack this turn. Agile uses 4."),
        DefaultValueAttribute(4)]
        public int Second
        {
            get { return _Second; }
            set { _Second = value; }
        }

        [CategoryAttribute("Penalty"),
        DescriptionAttribute("Penalty on the third and later attacks. Agile uses 8."),
        DefaultValueAttribute(8)]
        public int Later
        {
            get { return _Later; }
            set { _Later = value; }
        }

        #endregion
    }
}
