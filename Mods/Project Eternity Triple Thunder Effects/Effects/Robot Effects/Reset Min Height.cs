using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class ResetMinHeightEffect : TripleThunderRobotEffect
    {
        public static string Name = "Reset Min Height";

        public ResetMinHeightEffect()
            : base(Name, false)
        {
        }

        public ResetMinHeightEffect(TripleThunderRobotParams Params)
            : base(Name, false, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }

        protected override void Save(BinaryWriter BW)
        {
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            if (Params.LocalContext.Target.DicStoredVariable.ContainsKey("MinHeight"))
            {
                Params.LocalContext.Target.DicStoredVariable["MinHeight"] = Params.LocalContext.Target.Position.Y.ToString();
            }
            else
            {

                Params.LocalContext.Target.DicStoredVariable.Add("MinHeight", Params.LocalContext.Target.Position.Y.ToString());
            }

            return null;
        }

        protected override BaseEffect DoCopy()
        {
            ResetMinHeightEffect NewEffect = new ResetMinHeightEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        #region Properties

        #endregion
    }
}
