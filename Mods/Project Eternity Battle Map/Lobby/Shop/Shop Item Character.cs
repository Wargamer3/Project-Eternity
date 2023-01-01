using System.Collections.Generic;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class ShopItemCharacter
    {
        public string Path;
        public int Price;
        public int OrderNumber;
        public int UnlockQuantity;
        public bool HiddenUntilUnlocked;//Not visible in the shop until unlocked
        public ItemUnlockConditions UnlockConditions;

        public Character CharacterToBuy;

        public ShopItemCharacter(string Path)
        {
            this.Path = Path;
            HiddenUntilUnlocked = false;
            UnlockQuantity = 0;
        }

        public ShopItemCharacter(string Path, Dictionary<string, string> ActiveHeaderValues)
        {
            this.Path = Path;
            Load(ActiveHeaderValues);
        }

        public void Load(Dictionary<string, string> ActiveHeaderValues)
        {
            Price = int.Parse(ActiveHeaderValues["Price"]);
            OrderNumber = int.Parse(ActiveHeaderValues["Order"]);

            string UnlockQuantityValue;
            if (ActiveHeaderValues.TryGetValue("UnlockQuantity", out UnlockQuantityValue))
            {
                UnlockQuantity = int.Parse(UnlockQuantityValue);
            }

            UnlockConditions = new ItemUnlockConditions(ActiveHeaderValues);

            if (UnlockConditions.ListUnlockConditions.Count > 0)
            {
                HiddenUntilUnlocked = bool.Parse(ActiveHeaderValues["Hidden"]);
            }
        }
    }
}
