using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class StepEffect : TraitEffect
    {
        private float _Feet = 5f;
        private string _Trigger = "AllyMoved";

        public StepEffect()
            : base("Step")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll != null && Roll.Trigger == Trigger)
            {
                Roll.StepFeet = Feet;
                Roll.SpendReaction = true;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Feet);
            BW.Write(Trigger);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            StepEffect Copy = new StepEffect();
            Copy.Feet = BR.ReadSingle();
            Copy.Trigger = BR.ReadString();
            return Copy;
        }

        [CategoryAttribute("Move"), DefaultValueAttribute(5f)]
        public float Feet { get { return _Feet; } set { _Feet = value; } }

        [CategoryAttribute("Move"), DefaultValueAttribute("AllyMoved")]
        public string Trigger { get { return _Trigger; } set { _Trigger = value; } }
    }
}
