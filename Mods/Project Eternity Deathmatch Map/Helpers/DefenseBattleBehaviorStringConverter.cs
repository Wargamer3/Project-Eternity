using System.ComponentModel;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class DefenseBattleBehaviorStringConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            List<string> list = new List<string>();
            list.Add("Smart Counterattack");
            list.Add("Always Counterattack");
            list.Add("Simple Counterattack");
            list.Add("Always Block");
            list.Add("Always Dodge");
            return new StandardValuesCollection(list);
        }
    }
}
