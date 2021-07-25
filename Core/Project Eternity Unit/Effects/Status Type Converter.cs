using System;
using System.Globalization;
using System.ComponentModel;

namespace ProjectEternity.Core.Effects
{
    public class StatusTypeConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(
                          ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection
                 GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] { "MEL", "RNG", "DEF", "SKL", "EVA", "HIT" });
        }

        public override bool CanConvertTo(ITypeDescriptorContext context,
                              Type destinationType)
        {
            if (destinationType == typeof(StatusTypes))
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
            else if (value.GetType() == typeof(StatusTypes))
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
                case "MEL":
                    return StatusTypes.MEL;

                case "RNG":
                    return StatusTypes.RNG;

                case "DEF":
                    return StatusTypes.DEF;

                case "SKL":
                    return StatusTypes.SKL;

                case "EVA":
                    return StatusTypes.EVA;

                case "HIT":
                    return StatusTypes.HIT;
            }
            return StatusTypes.MEL;
        }
    }
}
