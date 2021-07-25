using System;
using System.Globalization;
using System.ComponentModel;

namespace ProjectEternity.Core.Effects
{
    public class UnitStatsConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(
                          ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection
                 GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] { "Max HP", "Max EN", "Armor", "Mobility", "Max MV" });
        }

        public override bool CanConvertTo(ITypeDescriptorContext context,
                              Type destinationType)
        {
            if (destinationType == typeof(UnitStats))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
                           CultureInfo culture,
                           object value,
                           Type destinationType)
        {
            if (value.GetType() == typeof(string))
            {
                return value;
            }
            else if (value.GetType() == typeof(UnitStats))
            {
                return value.ToString();
            }
            return null;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context,
                          Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
                          CultureInfo culture, object value)
        {
            string StatusType = (string)value;

            switch (StatusType)
            {
                case "Max HP":
                    return UnitStats.MaxHP;

                case "Max EN":
                    return UnitStats.MaxEN;

                case "Armor":
                    return UnitStats.Armor;

                case "Mobility":
                    return UnitStats.Mobility;

                case "Max MV":
                    return UnitStats.MaxMV;
            }
            return UnitStats.MaxHP;
        }
    }
}
