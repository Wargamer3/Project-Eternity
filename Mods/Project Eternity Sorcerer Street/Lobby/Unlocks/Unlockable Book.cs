using System;
using System.Collections.Generic;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class UnlockableBook : UnlockableItem
    {
        public const string UnitType = "Book";

        public CardBook BookToBuy;
        public List<UnlockableBookSkin> ListUnlockedSkin = new List<UnlockableBookSkin>();
        public List<UnlockableBookSkin> ListLockedSkin = new List<UnlockableBookSkin>();
        public List<UnlockableBookAlt> ListUnlockedAlt = new List<UnlockableBookAlt>();
        public List<UnlockableBookAlt> ListLockedAlt = new List<UnlockableBookAlt>();
        public bool ShowSkin;

        public UnlockableBook(string Path)
            : base(UnitType)
        {
            this.Path = Path;
            IsInShop = false;
        }

        public UnlockableBook(string Path, Dictionary<string, string> ActiveHeaderValues)
            : base(UnitType)
        {
            this.Path = Path;
            Load(ActiveHeaderValues);
        }

        public UnlockableBook(string Path, byte UnlockQuantity, bool IsInShop)
            : base(Path, UnlockQuantity, IsInShop)
        {
        }

        public override List<string> Unlock(Player ConditionsOwner)
        {
            List<string> ListUnlockMessage = new List<string>();

            if (UnlockConditions != null)
            {
                UnlockConditions.IsUnlocked = true;
            }

            ConditionsOwner.UnlockInventory.RootBookContainer.ListLockedBook.Remove(this);
            ConditionsOwner.UnlockInventory.RootBookContainer.ListUnlockedBook.Add(this);
            BookToBuy = CardBook.GetCardBook(Path, ConditionsOwner.Inventory.GlobalBook, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);

            if (UnlockQuantity > 0)
            {
                ConditionsOwner.Inventory.DicOwnedBook.Add(Path, new CardBookInfo(BookToBuy));
                ListUnlockMessage.Add("You just received " + UnlockQuantity + "x " + BookToBuy.BookName + "!");
            }

            if (IsInShop)
            {
                ListUnlockMessage.Add(BookToBuy.BookName + " is now available in the shop!");
            }

            return ListUnlockMessage;
        }
    }
}
