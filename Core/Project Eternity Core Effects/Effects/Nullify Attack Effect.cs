using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Effects
{
    public sealed class NullifyAttackEffect : SkillEffect
    {
        public static string Name = "Nullify Attack Effect";

        private string[] ArrayAttack;

        public NullifyAttackEffect()
            : base(Name, true)
        {
            ArrayAttack = new string[0];
        }

        public NullifyAttackEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
            ArrayAttack = new string[0];
        }
        
        protected override void Load(BinaryReader BR)
        {
            int NullifyAttackListAttackCount = BR.ReadInt32();
            ArrayAttack = new string[NullifyAttackListAttackCount];
            for (int A = 0; A < NullifyAttackListAttackCount; A++)
                ArrayAttack[A] = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(ArrayAttack.Length);
            for (int A = 0; A < ArrayAttack.Length; A++)
                BW.Write(ArrayAttack[A]);
        }

        protected override string DoExecuteEffect()
        {
            for (int A = ArrayAttack.Length - 1; A >= 0; --A)
                Params.LocalContext.EffectTargetUnit.Boosts.NullifyAttackModifier.Add(ArrayAttack[A]);

            return string.Join(System.Environment.NewLine, ArrayAttack);
        }

        protected override BaseEffect DoCopy()
        {
            NullifyAttackEffect NewEffect = new NullifyAttackEffect(Params);

            NewEffect.ArrayAttack = new string[ArrayAttack.Length];
            for (int A = ArrayAttack.Length - 1; A >= 0; --A)
                NewEffect.ArrayAttack[A] = ArrayAttack[A];

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            NullifyAttackEffect NewEffect = (NullifyAttackEffect)Copy;

            ArrayAttack = new string[NewEffect.ArrayAttack.Length];
            for (int A = NewEffect.ArrayAttack.Length - 1; A >= 0; --A)
                ArrayAttack[A] = NewEffect.ArrayAttack[A];
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("."),
        Editor(@"System.Windows.Forms.Design.StringCollectionEditor," +
        "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
        typeof(System.Drawing.Design.UITypeEditor)),
        TypeConverter(typeof(CsvConverter))]
        public string[] Attacks
        {
            get { return ArrayAttack; }
            set { ArrayAttack = value; }
        }

        #endregion
    }
}
