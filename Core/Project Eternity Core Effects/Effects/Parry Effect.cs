using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Effects
{
    public sealed class ParryEffect : SkillEffect
    {
        public static string Name = "Parry Effect";

        private List<string> ListAttack;

        public ParryEffect()
            : base(Name, true)
        {
            ListAttack = new List<string>();
        }

        public ParryEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
            ListAttack = new List<string>();
        }
        
        protected override void Load(BinaryReader BR)
        {
            int ParryListAttackCount = BR.ReadInt32();
            for (int A = ParryListAttackCount - 1; A >= 0; --A)
                ListAttack.Add(BR.ReadString());
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(ListAttack.Count);
            for (int A = 0; A < ListAttack.Count; A++)
                BW.Write(ListAttack[A]);
        }

        protected override string DoExecuteEffect()
        {
            for (int A = ListAttack.Count - 1; A >= 0; --A)
                Params.LocalContext.EffectTargetUnit.Boosts.ParryModifier.Add(ListAttack[A]);

            return string.Join(System.Environment.NewLine, ListAttack);
        }

        protected override void ReactivateEffect()
        {
            for (int A = ListAttack.Count - 1; A >= 0; --A)
                Params.LocalContext.EffectTargetUnit.Boosts.ParryModifier.Add(ListAttack[A]);
        }

        protected override BaseEffect DoCopy()
        {
            ParryEffect NewEffect = new ParryEffect(Params);

            NewEffect.ListAttack = new List<string>(ListAttack.Count);
            for (int i = ListAttack.Count - 1; i >= 0; --i)
                NewEffect.ListAttack.Add(ListAttack[i]);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ParryEffect NewEffect = (ParryEffect)Copy;

            ListAttack = new List<string>(NewEffect.ListAttack.Count);
            for (int i = NewEffect.ListAttack.Count - 1; i >= 0; --i)
                ListAttack.Add(NewEffect.ListAttack[i]);
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("."),
        Editor(@"System.Windows.Forms.Design.StringCollectionEditor," +
        "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
        typeof(System.Drawing.Design.UITypeEditor)),
        TypeConverter(typeof(CsvConverter))]
        public List<string> Attacks
        {
            get { return ListAttack; }
            set { ListAttack = value; }
        }

        #endregion
    }
}
