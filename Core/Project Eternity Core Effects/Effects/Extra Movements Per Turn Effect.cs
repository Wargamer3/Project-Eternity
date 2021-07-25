using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Effects
{
    public sealed class ExtraMovementsPerTurnEffect : SkillEffect
    {
        public static string Name = "Extra Movements Per Turn Effect";

        private int _ExtraMovementsPerTurn;
        private int _MaxExtraMovementsPerTurn;

        public ExtraMovementsPerTurnEffect()
            : base(Name, true)
        {
        }

        public ExtraMovementsPerTurnEffect(UnitEffectParams Params)
            : base(Name, true, Params)
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
            return Params.GlobalContext.EffectTargetUnit.Boosts.ExtraActionsPerTurn < _MaxExtraMovementsPerTurn;
        }

        protected override string DoExecuteEffect()
        {
            Params.LocalContext.EffectTargetUnit.Boosts.ExtraActionsPerTurn = _ExtraMovementsPerTurn;

            return _ExtraMovementsPerTurn + " - max " + _MaxExtraMovementsPerTurn;
        }

        protected override BaseEffect DoCopy()
        {
            ExtraMovementsPerTurnEffect NewEffect = new ExtraMovementsPerTurnEffect(Params);

            NewEffect._ExtraMovementsPerTurn = _ExtraMovementsPerTurn;
            NewEffect._MaxExtraMovementsPerTurn = _MaxExtraMovementsPerTurn;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ExtraMovementsPerTurnEffect NewEffect = (ExtraMovementsPerTurnEffect)Copy;

            _ExtraMovementsPerTurn = NewEffect._ExtraMovementsPerTurn;
            _MaxExtraMovementsPerTurn = NewEffect._MaxExtraMovementsPerTurn;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public int ExtraMovementsPerTurn
        {
            get { return _ExtraMovementsPerTurn; }
            set { _ExtraMovementsPerTurn = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public int MaxExtraMovementsPerTurn
        {
            get { return _MaxExtraMovementsPerTurn; }
            set { _MaxExtraMovementsPerTurn = value; }
        }

        #endregion
    }
}
