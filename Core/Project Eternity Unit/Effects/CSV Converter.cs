using System;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;

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
                if (value.GetType() == typeof(string))
                {
                    return value;
                }
                else if (value.GetType() == typeof(List<string>))
                {
                    return string.Join(",", (List<string>)value);
                }
                else
                {
                    return string.Join(",", (string[])value);
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
