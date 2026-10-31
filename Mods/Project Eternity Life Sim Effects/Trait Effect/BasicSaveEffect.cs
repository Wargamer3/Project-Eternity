using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class BasicSaveEffect : TraitEffect
    {
        private string _Save = "Reflex";
        private bool _Basic = true;
        private float _AreaFeet = 15f;

        public BasicSaveEffect()
            : base("Basic save")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null || Roll.Trigger == "List")
            {
                return;
            }

            Roll.Save = Save;
            Roll.Basic = Basic;
            Roll.AreaFeet = AreaFeet;
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Save);
            BW.Write(Basic);
            BW.Write(AreaFeet);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            BasicSaveEffect Copy = new BasicSaveEffect();
            Copy.Save = BR.ReadString();
            Copy.Basic = BR.ReadBoolean();
            Copy.AreaFeet = BR.ReadSingle();
            return Copy;
        }

        [CategoryAttribute("Save"), DescriptionAttribute("Fortitude, Reflex, or Will."), DefaultValueAttribute("Reflex")]
        public string Save { get { return _Save; } set { _Save = value; } }

        [CategoryAttribute("Save"), DescriptionAttribute("Success deals half damage."), DefaultValueAttribute(true)]
        public bool Basic { get { return _Basic; } set { _Basic = value; } }

        [CategoryAttribute("Save"), DescriptionAttribute("Area in feet."), DefaultValueAttribute(15f)]
        public float AreaFeet { get { return _AreaFeet; } set { _AreaFeet = value; } }
    }
}
