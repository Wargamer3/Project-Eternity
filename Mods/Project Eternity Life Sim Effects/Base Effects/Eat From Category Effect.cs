using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public sealed class EatFromCategoryEffect : LifeSimEffect
    {
        public static string Name = "Eat Category";

        private string _FoodCategory;

        public EatFromCategoryEffect()
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
            return Params.Owner.HasItemInCategory(_FoodCategory);
        }

        protected override string DoExecuteEffect()
        {
            return _FoodCategory;
        }

        protected override void ReactivateEffect()
        {
            //Don't change terrain on reactivation
        }

        protected override BaseEffect DoCopy()
        {
            EatFromCategoryEffect NewEffect = new EatFromCategoryEffect();

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            EatFromCategoryEffect NewEffect = (EatFromCategoryEffect)Copy;

        }

        #region Properties

        [CategoryAttribute(""),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public string FoodCategory
        {
            get
            {
                return _FoodCategory;
            }
            set
            {
                _FoodCategory = value;
            }
        }
        
        #endregion
    }
}
