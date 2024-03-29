﻿using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class MaxAmmoEffect : SkillEffect
    {
        public static string Name = "Max Ammo Effect";

        private string _MaxAmmoValue;
        private int LastEvaluationResult;


        public MaxAmmoEffect()
            : base(Name, true)
        {
        }

        public MaxAmmoEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _MaxAmmoValue = BR.ReadString();
        }

        protected override void DoQuickLoad(BinaryReader BR, FormulaParser ActiveParser)
        {
            base.DoQuickLoad(BR, ActiveParser);

            LastEvaluationResult = BR.ReadInt32();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_MaxAmmoValue);
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
            base.DoQuickSave(BW);

            BW.Write(LastEvaluationResult);
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.GlobalContext.ActiveParser.Evaluate(_MaxAmmoValue);
            byte FinalMaxAmmoValue = (byte)double.Parse(EvaluationResult, CultureInfo.InvariantCulture);
            LastEvaluationResult = FinalMaxAmmoValue;

            Params.LocalContext.EffectTargetUnit.Boosts.AmmoMaxModifier = FinalMaxAmmoValue;

            foreach (Core.Attacks.Attack ActiveAttack in Params.LocalContext.EffectTargetUnit.ListAttack)
            {
                ActiveAttack.IncreaseAmmo(FinalMaxAmmoValue);
            }

            if (EvaluationResult != _MaxAmmoValue)
            {
                return "Max ammo increased by " + FinalMaxAmmoValue + " (" + _MaxAmmoValue + ")";
            }
            return "Max ammo increased by " + _MaxAmmoValue;
        }

        protected override void ReactivateEffect()
        {
            Params.LocalContext.EffectTargetUnit.Boosts.AmmoMaxModifier = LastEvaluationResult;
            //Don't give ammo on reactivation
        }

        protected override BaseEffect DoCopy()
        {
            MaxAmmoEffect NewEffect = new MaxAmmoEffect(Params);

            NewEffect._MaxAmmoValue = _MaxAmmoValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            MaxAmmoEffect NewEffect = (MaxAmmoEffect)Copy;

            _MaxAmmoValue = NewEffect._MaxAmmoValue;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string Value
        {
            get { return _MaxAmmoValue; }
            set { _MaxAmmoValue = value; }
        }

        #endregion
    }
}
