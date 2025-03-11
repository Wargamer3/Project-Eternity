using System.Collections.Generic;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class UnlockableBookAlt : UnlockableItem
    {
        public const string BookAltType = "UnitAlt";

        public CardBook BookToBuy;
        public string AltTypeAndPath;

        public UnlockableBookAlt(string UnitPath, string AltPath)
            : base(BookAltType)
        {
            this.Path = UnitPath;
            this.AltTypeAndPath = AltPath;
            IsInShop = false;
        }

        public UnlockableBookAlt(string UnitPath, Dictionary<string, string> ActiveHeaderValues)
            : base(BookAltType)
        {
            this.Path = UnitPath;
            Load(ActiveHeaderValues);
            AltTypeAndPath = ActiveHeaderValues["AltPath"];
        }

        public UnlockableBookAlt(string UnitPath, byte UnlockQuantity, bool IsInShop)
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

            SorcererStreetPlayerUnlockInventory.DicBookDatabase[Path].ListLockedAlt.Remove(this);
            SorcererStreetPlayerUnlockInventory.DicBookDatabase[Path].ListUnlockedAlt.Add(this);

            BookToBuy = CardBook.GetCardBook(Path, ConditionsOwner.Inventory.GlobalBook, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);

            if (UnlockQuantity > 0)
            {
                if (ConditionsOwner.Inventory.DicOwnedBook.ContainsKey(Path))
                {
                    ConditionsOwner.Inventory.DicOwnedBook[Path].ListOwnedBookAlt.Add(new CardBookSkinInfo(Path, AltTypeAndPath, BookToBuy));
                }
                else
                {
                    ConditionsOwner.Inventory.DicOwnedBookAlt.Add(AltTypeAndPath, new CardBookSkinInfo(Path, AltTypeAndPath, BookToBuy));
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
            return AltTypeAndPath;
        }
    }
}
