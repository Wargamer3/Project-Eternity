using System.IO;
using System.ComponentModel;
using System.Globalization;
using System;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class UnlockableStatBoost : UnlcokableItemType
    {
        public class ArraySummaryConverter : TypeConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string) && value is Array arr)
                {
                    return $"{string.Join(", ", (PlayerCharacter.CharacterStats[])arr)}";
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public const string UnlockableTypeName = "Stat Boost";

        private StatsToAsign StatBoosts;

        public UnlockableStatBoost()
            : base(UnlockableTypeName)
        {
            StatBoosts = new StatsToAsign();
        }

        protected UnlockableStatBoost(BinaryReader BR)
            : this()
        {
            StatBoosts.PointsRemaning = BR.ReadByte();
            byte ArrayStatToChooseLength = BR.ReadByte();

            StatBoosts.ArrayStatToChoose = new PlayerCharacter.CharacterStats[ArrayStatToChooseLength];
            for (int i = 0; i < ArrayStatToChooseLength; ++i)
            {
                StatBoosts.ArrayStatToChoose[i] = (PlayerCharacter.CharacterStats)BR.ReadByte();
            }
        }

        public override void DoWrite(BinaryWriter BW)
        {
            BW.Write((byte)StatBoosts.PointsRemaning);
            BW.Write((byte)StatBoosts.ArrayStatToChoose.Length);
            foreach (PlayerCharacter.CharacterStats ActiveStat in StatBoosts.ArrayStatToChoose)
            {
                BW.Write((byte)ActiveStat);
            }
        }

        public override void Unlock()
        {
            if (Parent.GetType() == typeof(CharacterAncestry))
            {
                Params.Owner.Ancestry.ListStatBoosts.Add(StatBoosts);
            }
            else if (Parent.GetType() == typeof(CharacterBackground))
            {
                Params.Owner.Background.ListStatBoosts.Add(StatBoosts);
            }
            else if (Parent.GetType() == typeof(CharacterClass))
            {
                Params.Owner.Class.ListStatBoosts.Add(StatBoosts);
            }
        }

        public override UnlcokableItemType Copy()
        {
            return new UnlockableStatBoost();
        }

        public override UnlcokableItemType LoadCopy(BinaryReader BR)
        {
            return new UnlockableStatBoost(BR);
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("Used if the boost is a free type.")]
        public int PointsRemaning
        {
            get { return StatBoosts.PointsRemaning; }
            set { StatBoosts.PointsRemaning = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("Used if the boost is a free type."),
        TypeConverter(typeof(ArraySummaryConverter))]
        public PlayerCharacter.CharacterStats[] NumberType
        {
            get { return StatBoosts.ArrayStatToChoose; }
            set { StatBoosts.ArrayStatToChoose = value; }
        }

        #endregion
    }
}
