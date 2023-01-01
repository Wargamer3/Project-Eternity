using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class ShopItemMission
    {
        public string Path;
        public int Price;
        public int OrderNumber;
        public bool HiddenUntilUnlocked;//Not visible in the shop until unlocked
        public ItemUnlockConditions UnlockConditions;

        public MissionInfo MissionToBuy;

        public ShopItemMission(string Path)
        {
            this.Path = Path;
        }

        public ShopItemMission(string Path, Dictionary<string, string> ActiveHeaderValues)
        {
            this.Path = Path;
            Load(ActiveHeaderValues);
        }

        public void Load(Dictionary<string, string> ActiveHeaderValues)
        {
            Price = int.Parse(ActiveHeaderValues["Price"]);
            OrderNumber = int.Parse(ActiveHeaderValues["Order"]);

            UnlockConditions = new ItemUnlockConditions(ActiveHeaderValues);

            if (UnlockConditions.ListUnlockConditions.Count > 0)
            {
                HiddenUntilUnlocked = bool.Parse(ActiveHeaderValues["Hidden"]);
            }
        }
    }
}
