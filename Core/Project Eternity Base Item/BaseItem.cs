using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace ProjectEternity.Core.Item
{
    public class LogicOperatorConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(
                          ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection
                 GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] { "Equal", "Not equal", "Greater", "Greater or equal", "Lower", "Lower or equal" });
        }

        public override bool CanConvertTo(ITypeDescriptorContext context,
                              Type destinationType)
        {
            if (destinationType == typeof(Operators.LogicOperators))
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
            else if (value.GetType() == typeof(Operators.LogicOperators))
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
                case "Equal":
                    return Operators.LogicOperators.Equal;

                case "Not equal":
                    return Operators.LogicOperators.NotEqual;

                case "Greater":
                    return Operators.LogicOperators.Greater;

                case "Greater or equal":
                    return Operators.LogicOperators.GreaterOrEqual;

                case "Lower":
                    return Operators.LogicOperators.Lower;

                case "Lower or equal":
                    return Operators.LogicOperators.LowerOrEqual;
            }
            return Operators.LogicOperators.Equal;
        }
    }

    public class SignOperatorConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(
                          ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection
                 GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] { "=", "+=", "-=", "*=", "/=", "%=" });
        }

        public override bool CanConvertTo(ITypeDescriptorContext context,
                              Type destinationType)
        {
            if (destinationType == typeof(Operators.SignOperators))
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
            else if (value.GetType() == typeof(Operators.SignOperators))
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
                case "=":
                    return Operators.SignOperators.Equal;

                case "+=":
                    return Operators.SignOperators.PlusEqual;

                case "-=":
                    return Operators.SignOperators.MinusEqual;

                case "/=":
                    return Operators.SignOperators.DividedEqual;

                case "*=":
                    return Operators.SignOperators.MultiplicatedEqual;

                case "%=":
                    return Operators.SignOperators.ModuloEqual;
            }
            return Operators.SignOperators.Equal;
        }
    }

    public class SharableInt32
    {
        private bool IsActive;
        public SharableInt32 Pointer;
        private int BaseValue;
        public int Value { get { return UsePointer ? Pointer.Value : BaseValue; } set { if (UsePointer) Pointer.Value = value; else BaseValue = value; } }

        private bool UsePointer { get { return Pointer != null && Pointer.IsActive; } }

        public SharableInt32()
        {
            IsActive = false;
            Pointer = null;
            BaseValue = -1;
        }
    }

    public class ItemReference
    {
        public string ItemPath;
        public Type ItemType;
        public int Quantity;

        public ItemReference(string ItemPath, Type ItemType)
        {
            this.ItemPath = ItemPath;
            this.ItemType = ItemType;
            this.Quantity = 0;
        }
    }

    /// <summary>
    /// A FilterItem is used to contain a list of other FilterItem and of ShopItem to make sorting easier.
    /// </summary>
    public class FilterItem
    {
        public List<FilterItem> ListFilter;//List of filters
        public List<ShopItem> ListItem;
        /// <summary>
        /// Name of the Filter, mostly used for drawing and for better understanding of the filter.
        /// </summary>
        public string Name;
        /// <summary>
        /// Used to show the content of the FilterItem.
        /// </summary>
        public bool IsOpen;
        /// <summary>
        /// Index of the selected item in the FilterItem.
        /// </summary>
        public int CursorIndex;
        /// <summary>
        /// Create a new FilterItem with a name, a list of FiterItem and a list of ShopItem.
        /// </summary>
        /// <param name="Name">Name of the filter.</param>
        /// <param name="ListItem">List of ShopItem contained in the FilterItem.</param>
        /// <param name="ListFilter">List of FilterItem used for hierarchical Sorting.</param>
        public FilterItem(string Name, List<ShopItem> ListItem, List<FilterItem> ListFilter)
        {
            this.Name = Name;
            this.ListItem = ListItem;
            this.ListFilter = ListFilter;
            this.IsOpen = false;//Start the filter closed.
            this.CursorIndex = -1;
        }
    }

    public abstract class ShopItem
    {
        public string ItemName;
        public string FullName;
        public string Description;
        public int Price;
        public int Quantity;
        public int QuantityToBuy;

        protected ShopItem()
        {
        }

        protected ShopItem(string FullName)
            : this(FullName, "", 0)
        {
        }

        public ShopItem(string FullName, string Description, int Price)
        {
            this.FullName = FullName;
            this.Description = Description;
            this.Price = Price;
            this.Quantity = 1;
            this.QuantityToBuy = 0;
        }

        public override bool Equals(object obj)
        {
            ShopItem OtherShopItem = obj as ShopItem;
            if (OtherShopItem == null)
                return base.Equals(obj);
            else
                return FullName == OtherShopItem.FullName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return FullName;
        }
    }

    public class Blueprint : ShopItem
    {
        public static List<Blueprint> ListBlueprints = new List<Blueprint>();

        public struct ItemRequirement
        {
            public string ItemName;
            public int Quantity;
        }

        public List<ItemRequirement> ListRequirement;
        public ShopItem Creation;
        public bool CanCreate;

        public Blueprint(string Name, string Description, int Price, List<ItemRequirement> ListRequirement, ShopItem Creation)
            : base(Name, Description, Price)
        {
            this.ListRequirement = ListRequirement;
            this.Creation = Creation;
            CanCreate = false;
        }
    }

    public class ScrapParts : ShopItem
    {
        public ScrapParts(string Name, string Description, int Price)
            : base(Name, Description, Price)
        {
        }
    }
}
