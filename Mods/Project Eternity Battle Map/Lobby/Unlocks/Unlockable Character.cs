using System.Collections.Generic;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class UnlockableCharacter : UnlockableItem
    {
        public const string CharacterType = "Character";

        public Character CharacterToBuy;

        public UnlockableCharacter(string Path)
            : base(CharacterType)
        {
            this.Path = Path;
            UnlockQuantity = 0;
        }

        public UnlockableCharacter(string Path, Dictionary<string, string> ActiveHeaderValues)
            : base(CharacterType)
        {
            this.Path = Path;
            Load(ActiveHeaderValues);
        }

        public UnlockableCharacter(string Path, byte UnlockQuantity, bool IsInShop)
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

            ConditionsOwner.UnlockInventory.ListLockedCharacter.Remove(this);
            ConditionsOwner.UnlockInventory.ListUnlockedCharacter.Add(this);
            Character NewCharacter = new Character(Path, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);

            if (UnlockQuantity > 0)
            {
                ConditionsOwner.Inventory.DicOwnedCharacter.Add(Path, new CharacterInfo(NewCharacter, UnlockQuantity));
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
