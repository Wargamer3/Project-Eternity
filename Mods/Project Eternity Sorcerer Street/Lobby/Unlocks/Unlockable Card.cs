using System;
using System.Collections.Generic;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class UnlockableCard : UnlockableItem
    {
        public const string UnitType = "Card";

        public Card CardToBuy;
        public List<UnlockableCardSkin> ListUnlockedSkin = new List<UnlockableCardSkin>();
        public List<UnlockableCardSkin> ListLockedSkin = new List<UnlockableCardSkin>();
        public List<UnlockableCardAlt> ListUnlockedAlt = new List<UnlockableCardAlt>();
        public List<UnlockableCardAlt> ListLockedAlt = new List<UnlockableCardAlt>();

        public UnlockableCard(string Path)
            : base(UnitType)
        {
            this.Path = Path;
            IsInShop = false;
        }

        public UnlockableCard(string Path, Dictionary<string, string> ActiveHeaderValues)
            : base(UnitType)
        {
            this.Path = Path;
            Load(ActiveHeaderValues);
        }

        public UnlockableCard(string Path, byte UnlockQuantity, bool IsInShop)
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

            ConditionsOwner.UnlockInventory.RootCardContainer.ListLockedCard.Remove(this);
            ConditionsOwner.UnlockInventory.RootCardContainer.ListUnlockedCard.Add(this);
            CardToBuy = Card.LoadCard(Path, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);

            if (UnlockQuantity > 0)
            {
                ConditionsOwner.Inventory.GlobalBook.AddCard(new CardInfo(CardToBuy, UnlockQuantity));
                ListUnlockMessage.Add("You just received " + UnlockQuantity + "x " + CardToBuy.Name + "!");
            }

            if (IsInShop)
            {
                ListUnlockMessage.Add(CardToBuy.Name + " is now available in the shop!");
            }

            return ListUnlockMessage;
        }
    }
}
