using System;
using System.Globalization;
using System.ComponentModel;

namespace ProjectEternity.Core.Effects
{
    public class CsvConverter : TypeConverter
    {
        // Overrides the ConvertTo method of TypeConverter.
        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return string.Join(",", (string[])value);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
