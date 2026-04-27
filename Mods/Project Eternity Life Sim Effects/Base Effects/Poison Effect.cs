using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public sealed class PoisonEffect : LifeSimEffect
    {
        public static string Name = "Poison";

        private string _FoodName;

        public PoisonEffect()
            : base(Name, false)
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
            return Params.Owner.HasItem(_FoodName);
        }

        protected override string DoExecuteEffect()
        {

            return _FoodName;
        }

        protected override void ReactivateEffect()
        {
            //Don't change terrain on reactivation
        }

        protected override BaseEffect DoCopy()
        {
            PoisonEffect NewEffect = new PoisonEffect();

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            PoisonEffect NewEffect = (PoisonEffect)Copy;

        }

        #region Properties

        [CategoryAttribute(""),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public string FoodName
        {
            get
            {
                return _FoodName;
            }
            set
            {
                _FoodName = value;
            }
        }
        
        #endregion
    }
}
