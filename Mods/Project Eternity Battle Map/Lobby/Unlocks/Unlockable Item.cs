using System;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class UnlockableItem
    {
        public string ItemType;
        public string Path;
        public byte UnlockQuantity;
        public bool IsInShop;
        public int ShopOrder;
        public bool HiddenUntilUnlocked;//Not visible in the shop until unlocked
        public ItemUnlockConditions UnlockConditions;

        protected UnlockableItem(string ItemType)
        {
            this.ItemType = ItemType;
        }

        public UnlockableItem(string Path, byte UnlockQuantity, bool IsInShop)
        {
            this.Path = Path;
            this.UnlockQuantity = UnlockQuantity;
            this.IsInShop = IsInShop;
        }

        public void Load(Dictionary<string, string> ActiveHeaderValues)
        {
            string ShopOrderValue;
            if (ActiveHeaderValues.TryGetValue("Order", out ShopOrderValue))
            {
                ShopOrder = int.Parse(ShopOrderValue);
                IsInShop = true;
            }

            string UnlockQuantityValue;
            if (ActiveHeaderValues.TryGetValue("UnlockQuantity", out UnlockQuantityValue))
            {
                UnlockQuantity = byte.Parse(UnlockQuantityValue);
            }

            ReadHeaders(ActiveHeaderValues);
        }

        public void ReadHeaders(Dictionary<string, string> ActiveHeaderValues)
        {
            UnlockConditions = new ItemUnlockConditions(ActiveHeaderValues);

            if (UnlockConditions.ListUnlockConditions.Count > 0)
            {
                string HiddenValue;
                if (ActiveHeaderValues.TryGetValue("Hidden", out HiddenValue))
                {
                    HiddenUntilUnlocked = bool.Parse(HiddenValue);
                }
            }
        }

        public abstract List<string> Unlock(BattleMapPlayer ConditionsOwner);
    }
}