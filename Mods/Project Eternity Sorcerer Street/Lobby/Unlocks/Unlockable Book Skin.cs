using System.Collections.Generic;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class UnlockableBookSkin : UnlockableItem
    {
        public const string BookSkinType = "UnitSkin";

        public CardBook BookToBuy;
        public string SkinTypeAndPath;

        public UnlockableBookSkin(string UnitPath, string SkinPath)
            : base(BookSkinType)
        {
            this.Path = UnitPath;
            this.SkinTypeAndPath = SkinPath;
            IsInShop = false;
        }

        public UnlockableBookSkin(string UnitPath, Dictionary<string, string> ActiveHeaderValues)
            : base(BookSkinType)
        {
            this.Path = UnitPath;
            Load(ActiveHeaderValues);
            SkinTypeAndPath = ActiveHeaderValues["SkinPath"];
        }

        public UnlockableBookSkin(string UnitPath, byte UnlockQuantity, bool IsInShop)
            : base(UnitPath, UnlockQuantity, IsInShop)
        {
        }

        public override List<string> Unlock(Player ConditionsOwner)
        {
            List<string> ListUnlockMessage = new List<string>();

            if (UnlockConditions != null)
            {
                UnlockConditions.IsUnlocked = true;
            }

            SorcererStreetPlayerUnlockInventory.DicBookDatabase[Path].ListLockedSkin.Remove(this);
            SorcererStreetPlayerUnlockInventory.DicBookDatabase[Path].ListUnlockedSkin.Add(this);

            BookToBuy = CardBook.GetCardBook(Path, ConditionsOwner.Inventory.GlobalBook, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);

            if (UnlockQuantity > 0)
            {
                if (ConditionsOwner.Inventory.DicOwnedBook.ContainsKey(Path))
                {
                    ConditionsOwner.Inventory.DicOwnedBook[Path].ListOwnedBookSkin.Add(new CardBookSkinInfo(Path, SkinTypeAndPath, BookToBuy));
                }
                else
                {
                    ConditionsOwner.Inventory.DicOwnedBookSkin.Add(SkinTypeAndPath, new CardBookSkinInfo(Path, SkinTypeAndPath, BookToBuy));
                }

                ListUnlockMessage.Add("You just received " + UnlockQuantity + "x " + BookToBuy.BookName + "!");
            }

            if (IsInShop)
            {
                ListUnlockMessage.Add(BookToBuy.BookName + " is now available in the shop!");
            }

            return ListUnlockMessage;
        }

        public override string ToString()
        {
            return SkinTypeAndPath;
        }
    }
}
