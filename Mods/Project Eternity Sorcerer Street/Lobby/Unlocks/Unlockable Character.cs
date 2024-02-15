using System.Collections.Generic;
using ProjectEternity.Core.Characters;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class UnlockablePlayerCharacter : UnlockableItem
    {
        public const string CharacterType = "Character";

        public Character CharacterToBuy;

        public UnlockablePlayerCharacter(string Path)
            : base(CharacterType)
        {
            this.Path = Path;
            UnlockQuantity = 0;
        }

        public UnlockablePlayerCharacter(string Path, Dictionary<string, string> ActiveHeaderValues)
            : base(CharacterType)
        {
            this.Path = Path;
            Load(ActiveHeaderValues);
        }

        public UnlockablePlayerCharacter(string Path, byte UnlockQuantity, bool IsInShop)
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

            ConditionsOwner.UnlockInventory.RootCharacterContainer.ListLockedCharacter.Remove(this);
            ConditionsOwner.UnlockInventory.RootCharacterContainer.ListUnlockedCharacter.Add(this);
            PlayerCharacter NewCharacter = new PlayerCharacter(Path, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);

            if (UnlockQuantity > 0)
            {
                ConditionsOwner.Inventory.DicOwnedCharacter.Add(Path, NewCharacter);
                ListUnlockMessage.Add("You just received " + UnlockQuantity + "x " + NewCharacter.Name + "!");
            }

            if (IsInShop)
            {
                ListUnlockMessage.Add(NewCharacter.Name + " is now available in the shop!");
            }

            return ListUnlockMessage;
        }
    }
}
