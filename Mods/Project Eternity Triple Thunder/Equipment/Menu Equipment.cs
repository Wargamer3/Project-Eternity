using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class MenuEquipment
    {
        public readonly string Name;
        public readonly EquipmentTypes EquipmentType;
        public readonly int Price;
        public readonly Texture2D sprIcon;
        public readonly Texture2D sprFull;

        public MenuEquipment(string Name, EquipmentTypes EquipmentType, int Price, Texture2D sprIcon, Texture2D sprFull)
        {
            this.Name = Name;
            this.EquipmentType = EquipmentType;
            this.Price = Price;
            this.sprIcon = sprIcon;
            this.sprFull = sprFull;
        }
    }

    public class CharacterMenuEquipment : MenuEquipment
    {
        public CharacterMenuEquipment(string Name, int Price, Texture2D sprIcon, Texture2D sprPortrait)
            : base(Name, EquipmentTypes.Character, Price, sprIcon, sprPortrait)
        {
        }
    }
}
