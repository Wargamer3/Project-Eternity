using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class BattleFormEffect : TraitEffect
    {
        private string _Form = "Wolf";

        public BattleFormEffect()
            : base("Battle form")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll != null && Roll.Trigger != "List")
            {
                Roll.FormName = Form;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Form);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            BattleFormEffect Copy = new BattleFormEffect();
            Copy.Form = BR.ReadString();
            return Copy;
        }

        [CategoryAttribute("Form"), DefaultValueAttribute("Wolf")]
        public string Form { get { return _Form; } set { _Form = value; } }
    }
}
