using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public sealed class AttackEffect : LifeSimEffect
    {
        public static string Name = "Attack";

        private string _FoodName;

        public AttackEffect()
            : base(Name, false)
        {
        }

        public AttackEffect(LifeSimParams Params)
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
            AttackEffect NewEffect = new AttackEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            AttackEffect NewEffect = (AttackEffect)Copy;

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
