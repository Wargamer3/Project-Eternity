using System;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using System.IO;

namespace ProjectEternity.Core.Effects
{
    public sealed class BarrierEffect : SkillEffect
    {
        public static string Name = "Barrier Effect";

        public enum BarrierTypes { Defend = 0, Dodge = 1 };

        private BarrierTypes _BarrierType;
        private string _ENCost;
        private Operators.NumberTypes _NumberType;
        private string _DamageReduction;
        private string _BreakingDamage;//Damage limit for the BarrierEffect to break.
        private List<string> ListEffectiveAttack;//List of Attacks the BarrierEffect is usefull against.
        private List<string> ListBreakingAttack;//List of Attacks the BarrierEffect is destroyed against.
        private List<string> ListBreakingSkill;//List of Skills the BarrierEffect is destroyed against.

        public BarrierEffect()
            : base(Name, true)
        {
            _BarrierType = BarrierTypes.Defend;
            _ENCost = "0";
            _NumberType = Operators.NumberTypes.Absolute;
            _DamageReduction = "1000";
            BreakingDamage = "1000";
            ListEffectiveAttack = new List<string>();
            ListBreakingAttack = new List<string>();
            ListBreakingSkill = new List<string>();
        }

        public BarrierEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _BarrierType = (BarrierTypes)BR.ReadByte();
            _ENCost = BR.ReadString();
            _NumberType = (Operators.NumberTypes)BR.ReadByte();
            _DamageReduction = BR.ReadString();
            _BreakingDamage = BR.ReadString();

            int BarrierEffectEffectiveAttacksCount = BR.ReadInt32();
            for (int A = BarrierEffectEffectiveAttacksCount - 1; A >= 0; --A)
                ListEffectiveAttack.Add(BR.ReadString());

            int BarrierEffectBreakingAttacksCount = BR.ReadInt32();
            for (int A = BarrierEffectBreakingAttacksCount - 1; A >= 0; --A)
                ListBreakingAttack.Add(BR.ReadString());

            int BarrierEffectBreakingSkillsCount = BR.ReadInt32();
            for (int A = BarrierEffectBreakingSkillsCount - 1; A >= 0; --A)
                ListBreakingSkill.Add(BR.ReadString());
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_BarrierType);
            BW.Write(_ENCost);
            BW.Write((byte)_NumberType);
            BW.Write(_DamageReduction);
            BW.Write(_BreakingDamage);

            BW.Write(ListEffectiveAttack.Count);
            for (int A = 0; A < ListEffectiveAttack.Count; A++)
                BW.Write(ListEffectiveAttack[A]);

            BW.Write(ListBreakingAttack.Count);
            for (int A = 0; A < ListBreakingAttack.Count; A++)
                BW.Write(ListBreakingAttack[A]);

            BW.Write(ListBreakingSkill.Count);
            for (int A = 0; A < ListBreakingSkill.Count; A++)
                BW.Write(ListBreakingSkill[A]);
        }

        protected override string DoExecuteEffect()
        {
            string Output = string.Empty;

            Output += "Barrier type: " + _BarrierType + Environment.NewLine;
            Output += "EN cost: " + _ENCost + Environment.NewLine;
            Output += "Number type: " + _NumberType + Environment.NewLine;
            Output += "Damage reduction: " + _DamageReduction + Environment.NewLine;
            Output += "Breaking damage: " + _BreakingDamage + Environment.NewLine;

            Output += "Effective Attacks: " + _BreakingDamage + Environment.NewLine;
            Output += string.Join(Environment.NewLine, ListEffectiveAttack);
            Output += "Breaking Attacks: " + _BreakingDamage + Environment.NewLine;
            Output += string.Join(Environment.NewLine, ListBreakingAttack);
            Output += "Breaking Skills: " + _BreakingDamage + Environment.NewLine;
            Output += string.Join(Environment.NewLine, ListBreakingSkill);

            return Output;
        }
        
        protected override BaseEffect DoCopy()
        {
            BarrierEffect NewEffect = new BarrierEffect(Params);

            NewEffect._BarrierType = _BarrierType;
            NewEffect._ENCost = _ENCost;
            NewEffect._NumberType = _NumberType;
            NewEffect._DamageReduction = _DamageReduction;
            NewEffect._BreakingDamage = _BreakingDamage;

            NewEffect.ListEffectiveAttack = new List<string>(ListEffectiveAttack.Count);
            for (int i = ListEffectiveAttack.Count - 1; i >= 0; --i)
                NewEffect.ListEffectiveAttack.Add(ListEffectiveAttack[i]);

            NewEffect.ListBreakingAttack = new List<string>(ListBreakingAttack.Count);
            for (int i = ListBreakingAttack.Count - 1; i >= 0; --i)
                NewEffect.ListBreakingAttack.Add(ListBreakingAttack[i]);

            NewEffect.ListBreakingSkill = new List<string>(ListBreakingSkill.Count);
            for (int i = ListBreakingSkill.Count - 1; i >= 0; --i)
                NewEffect.ListBreakingSkill.Add(ListBreakingSkill[i]);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            BarrierEffect NewEffect = (BarrierEffect)Copy;

            _BarrierType = NewEffect._BarrierType;
            _ENCost = NewEffect._ENCost;
            _NumberType = NewEffect._NumberType;
            _DamageReduction = NewEffect._DamageReduction;
            _BreakingDamage = NewEffect._BreakingDamage;

            ListEffectiveAttack = new List<string>(NewEffect.ListEffectiveAttack.Count);
            for (int i = NewEffect.ListEffectiveAttack.Count - 1; i >= 0; --i)
                ListEffectiveAttack.Add(NewEffect.ListEffectiveAttack[i]);

            ListBreakingAttack = new List<string>(NewEffect.ListBreakingAttack.Count);
            for (int i = NewEffect.ListBreakingAttack.Count - 1; i >= 0; --i)
                ListBreakingAttack.Add(NewEffect.ListBreakingAttack[i]);

            ListBreakingSkill = new List<string>(NewEffect.ListBreakingSkill.Count);
            for (int i = NewEffect.ListBreakingSkill.Count - 1; i >= 0; --i)
                ListBreakingSkill.Add(NewEffect.ListBreakingSkill[i]);
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public BarrierTypes BarrierType
        {
            get { return _BarrierType; }
            set { _BarrierType = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string ENCost
        {
            get { return _ENCost; }
            set { _ENCost = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public Operators.NumberTypes NumberType
        {
            get { return _NumberType; }
            set { _NumberType = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string DamageReduction
        {
            get { return _DamageReduction; }
            set { _DamageReduction = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string BreakingDamage
        {
            get { return _BreakingDamage; }
            set { _BreakingDamage = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("."),
        Editor(@"System.Windows.Forms.Design.StringCollectionEditor," +
        "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
        typeof(System.Drawing.Design.UITypeEditor)),
        TypeConverter(typeof(CsvConverter))]
        public List<string> EffectiveAttacks
        {
            get { return ListEffectiveAttack; }
            set { ListEffectiveAttack = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("."),
        Editor(@"System.Windows.Forms.Design.StringCollectionEditor," +
        "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
        typeof(System.Drawing.Design.UITypeEditor)),
        TypeConverter(typeof(CsvConverter))]
        public List<string> BreakingAttacks
        {
            get { return ListBreakingAttack; }
            set { ListBreakingAttack = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("."),
        Editor(@"System.Windows.Forms.Design.StringCollectionEditor," +
        "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
        typeof(System.Drawing.Design.UITypeEditor)),
        TypeConverter(typeof(CsvConverter))]
        public List<string> BreakingSkills
        {
            get { return ListBreakingSkill; }
            set { ListBreakingSkill = value; }
        }

        #endregion
    }
}
