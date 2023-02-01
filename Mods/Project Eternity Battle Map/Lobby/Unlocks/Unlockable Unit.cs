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

        public UnlockableUnit(string Path, int UnlockQuantity, bool IsInShop)
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

            ConditionsOwner.UnlockInventory.ListLockedUnit.Remove(this);
            ConditionsOwner.UnlockInventory.ListUnlockedUnit.Add(this);
            Unit NewUnit = null;
            //ConditionsOwner.Inventory.ListOwnedSquad.Add(NewUnit);
            for (int Q = 1; Q < UnlockQuantity; ++Q)
            {
                //ConditionsOwner.Inventory.ListOwnedSquad.Add();
            }

            //ListUnlockMessage.Add("You just received " + UnlockQuantity + "x " + NewUnit.Name + "!");

            if (IsInShop)
            {
                //ListUnlockMessage.Add(NewUnit.Name + " is now available in the shop!");
            }

            return ListUnlockMessage;
        }
    }
}
