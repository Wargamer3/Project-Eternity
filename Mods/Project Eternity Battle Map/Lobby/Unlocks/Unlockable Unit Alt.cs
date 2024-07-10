using ProjectEternity.Core.Units;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class UnlockableUnitAlt : UnlockableItem
    {
        public const string UnitAltType = "UnitAlt";

        public Unit UnitAltToBuy;
        public string AltTypeAndPath;

        public UnlockableUnitAlt(string UnitPath, string AltPath)
            : base(UnitAltType)
        {
            this.Path = UnitPath;
            this.AltTypeAndPath = AltPath;
            IsInShop = false;
        }

        public UnlockableUnitAlt(string UnitPath, Dictionary<string, string> ActiveHeaderValues)
            : base(UnitAltType)
        {
            this.Path = UnitPath;
            Load(ActiveHeaderValues);
            AltTypeAndPath = ActiveHeaderValues["AltPath"];
        }

        public UnlockableUnitAlt(string UnitPath, byte UnlockQuantity, bool IsInShop)
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

            BattleMapPlayerShopUnlockInventory.DicUnitDatabase[Path].ListLockedAlt.Remove(this);
            BattleMapPlayerShopUnlockInventory.DicUnitDatabase[Path].ListUnlockedAlt.Add(this);

            Unit NewUnit = Unit.FromFullName(AltTypeAndPath, GameScreen.ContentFallback, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
            NewUnit.ID = NewUnit.ItemName;

            if (UnlockQuantity > 0)
            {
                if (ConditionsOwner.Inventory.DicOwnedUnit.ContainsKey(Path))
                {
                    ConditionsOwner.Inventory.DicOwnedUnit[Path].ListOwnedUnitAlt.Add(new UnitSkinInfo(Path, AltTypeAndPath, NewUnit));
                }
                else
                {
                    ConditionsOwner.Inventory.DicOwnedUnitAlt.Add(AltTypeAndPath, new UnitSkinInfo(Path, AltTypeAndPath, NewUnit));
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
            return AltTypeAndPath;
        }
    }
}
