using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public sealed class EatSpecificFoodEffect : LifeSimEffect
    {
        public static string Name = "Eat";

        private string _FoodName;

        public EatSpecificFoodEffect()
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
            EatSpecificFoodEffect NewEffect = new EatSpecificFoodEffect();

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            EatSpecificFoodEffect NewEffect = (EatSpecificFoodEffect)Copy;

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
