using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class StoreMinHeightEffect : TripleThunderRobotEffect
    {
        public static string Name = "Store Min Height";
        
        public StoreMinHeightEffect()
            : base(Name, false)
        {
        }

        public StoreMinHeightEffect(TripleThunderRobotParams Params)
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
            string Height;

            if (Params.LocalContext.Target.DicStoredVariable.TryGetValue("MinHeight", out Height))
            {
                float RealHeight = float.Parse(Height);
                if (Params.LocalContext.Target.Position.Y < RealHeight)
                {
                    Params.LocalContext.Target.DicStoredVariable["MinHeight"] = Params.LocalContext.Target.Position.Y.ToString();
                }
            }
            else
            {

                Params.LocalContext.Target.DicStoredVariable.Add("MinHeight", Params.LocalContext.Target.Position.Y.ToString());
            }

            return null;
        }

        protected override BaseEffect DoCopy()
        {
            StoreMinHeightEffect NewEffect = new StoreMinHeightEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        #region Properties

        #endregion
    }
}
