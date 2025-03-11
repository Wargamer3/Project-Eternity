using System.Collections.Generic;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class UnlockableCardAlt : UnlockableItem
    {
        public const string UnitAltType = "UnitAlt";

        public Card CardToBuy;
        public string AltTypeAndPath;

        public UnlockableCardAlt(string UnitPath, string AltPath)
            : base(UnitAltType)
        {
            this.Path = UnitPath;
            this.AltTypeAndPath = AltPath;
            IsInShop = false;
        }

        public UnlockableCardAlt(string UnitPath, Dictionary<string, string> ActiveHeaderValues)
            : base(UnitAltType)
        {
            this.Path = UnitPath;
            Load(ActiveHeaderValues);
            AltTypeAndPath = ActiveHeaderValues["AltPath"];
        }

        public UnlockableCardAlt(string UnitPath, byte UnlockQuantity, bool IsInShop)
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

            SorcererStreetPlayerUnlockInventory.DicCardDatabase[Path].ListLockedAlt.Remove(this);
            SorcererStreetPlayerUnlockInventory.DicCardDatabase[Path].ListUnlockedAlt.Add(this);

            CardToBuy = Card.LoadCard(Path, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);

            if (UnlockQuantity > 0)
            {
                ConditionsOwner.Inventory.DicOwnedCardAlt.Add(AltTypeAndPath, new CardSkinInfo(Path, AltTypeAndPath, CardToBuy));

                ListUnlockMessage.Add("You just received " + UnlockQuantity + "x " + CardToBuy.Name + "!");
            }

            if (IsInShop)
            {
                ListUnlockMessage.Add(CardToBuy.Name + " is now available in the shop!");
            }

            return ListUnlockMessage;
        }

        public override string ToString()
        {
            return AltTypeAndPath;
        }
    }
}
