using ProjectEternity.Core.Units;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class UnlockableUnit : UnlockableItem
    {
        public const string UnitType = "Unit";

        public Unit UnitToBuy;

        public UnlockableUnit(string Path)
            : base(UnitType)
        {
            this.Path = Path;
            IsInShop = false;
        }

        public UnlockableUnit(string Path, Dictionary<string, string> ActiveHeaderValues)
            : base(UnitType)
        {
            this.Path = Path;
            Load(ActiveHeaderValues);
        }

        public UnlockableUnit(string Path, byte UnlockQuantity, bool IsInShop)
            : base(Path, UnlockQuantity, IsInShop)
        {
        }

        public override List<string> Unlock(BattleMapPlayer ConditionsOwner)
        {
            List<string> ListUnlockMessage = new List<string>();

            if (UnlockConditions != null)
            {
                UnlockConditions.IsUnlocked = true;
            }

            ConditionsOwner.UnlockInventory.RootUnitContainer.ListLockedUnit.Remove(this);
            ConditionsOwner.UnlockInventory.RootUnitContainer.ListUnlockedUnit.Add(this);
            Unit NewUnit = Unit.FromFullName(Path, GameScreen.ContentFallback, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
            NewUnit.ID = NewUnit.ItemName;

            if (UnlockQuantity > 0)
            {
                ConditionsOwner.Inventory.DicOwnedSquad.Add(Path, new UnitInfo(NewUnit, UnlockQuantity));
                ListUnlockMessage.Add("You just received " + UnlockQuantity + "x " + NewUnit.ItemName + "!");
            }

            if (IsInShop)
            {
                ListUnlockMessage.Add(NewUnit.ItemName + " is now available in the shop!");
            }

            return ListUnlockMessage;
        }
    }
}
