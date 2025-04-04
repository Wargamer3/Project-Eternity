using ProjectEternity.Core.Units;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class UnlockableUnitSkin : UnlockableItem
    {
        public const string UnitSkinType = "UnitSkin";

        public Unit UnitSkinToBuy;//Used by the shop
        public string SkinTypeAndPath;

        public UnlockableUnitSkin(string UnitPath, string SkinPath)
            : base(UnitSkinType)
        {
            this.Path = UnitPath;
            this.SkinTypeAndPath = SkinPath;
            IsInShop = false;
        }

        public UnlockableUnitSkin(string UnitPath, Dictionary<string, string> ActiveHeaderValues)
            : base(UnitSkinType)
        {
            this.Path = UnitPath;
            Load(ActiveHeaderValues);
            SkinTypeAndPath = ActiveHeaderValues["SkinPath"];
        }

        public UnlockableUnitSkin(string UnitPath, byte UnlockQuantity, bool IsInShop)
            : base(UnitPath, UnlockQuantity, IsInShop)
        {
        }

        public override List<string> Unlock(BattleMapPlayer ConditionsOwner)
        {
            List<string> ListUnlockMessage = new List<string>();

            if (UnlockConditions != null)
            {
                UnlockConditions.IsUnlocked = true;
            }

            BattleMapPlayerShopUnlockInventory.DicUnitDatabase[Path].ListLockedSkin.Remove(this);
            BattleMapPlayerShopUnlockInventory.DicUnitDatabase[Path].ListUnlockedSkin.Add(this);

            Unit NewUnit = Unit.FromFullName(SkinTypeAndPath, GameScreen.ContentFallback, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
            NewUnit.ID = NewUnit.ItemName;

            if (UnlockQuantity > 0)
            {
                ConditionsOwner.Inventory.DicOwnedUnitSkin.Add(SkinTypeAndPath, new UnitSkinInfo(Path, SkinTypeAndPath, NewUnit));

                if (ConditionsOwner.Inventory.DicOwnedUnit.ContainsKey(Path))
                {
                    ConditionsOwner.Inventory.DicOwnedUnit[Path].ListOwnedUnitSkin.Add(new UnitSkinInfo(Path, SkinTypeAndPath, NewUnit));
                }

                ListUnlockMessage.Add("You just received " + UnlockQuantity + "x " + NewUnit.ItemName + "!");
            }

            if (IsInShop)
            {
                ListUnlockMessage.Add(NewUnit.ItemName + " is now available in the shop!");
            }

            return ListUnlockMessage;
        }

        public override string ToString()
        {
            return SkinTypeAndPath;
        }
    }
}
