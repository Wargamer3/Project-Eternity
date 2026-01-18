using System.Collections.Generic;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class UnlockableCardSkin : UnlockableItem
    {
        public const string UnitSkinType = "UnitSkin";

        public Card CardToBuy;
        public string SkinTypeAndPath;
        public string ModelPath;

        public UnlockableCardSkin(string UnitPath, string SkinPath)
            : base(UnitSkinType)
        {
            this.Path = UnitPath;
            this.SkinTypeAndPath = SkinPath;
            IsInShop = false;
        }

        public UnlockableCardSkin(string UnitPath, Dictionary<string, string> ActiveHeaderValues)
            : base(UnitSkinType)
        {
            this.Path = UnitPath;
            Load(ActiveHeaderValues);
            SkinTypeAndPath = ActiveHeaderValues["SkinPath"];
            ModelPath = ActiveHeaderValues["ModelPath"];
        }

        public UnlockableCardSkin(string UnitPath, byte UnlockQuantity, bool IsInShop)
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

            SorcererStreetPlayerUnlockInventory.DicCardDatabase[Path].ListLockedSkin.Remove(this);
            SorcererStreetPlayerUnlockInventory.DicCardDatabase[Path].ListUnlockedSkin.Add(this);

            CardToBuy = Card.LoadCard(Path, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);

            if (UnlockQuantity > 0)
            {
                ConditionsOwner.Inventory.DicOwnedCardSkin.Add(SkinTypeAndPath, new CardSkinInfo(Path, SkinTypeAndPath, ModelPath, CardToBuy, false));

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
            return SkinTypeAndPath;
        }
    }
}
