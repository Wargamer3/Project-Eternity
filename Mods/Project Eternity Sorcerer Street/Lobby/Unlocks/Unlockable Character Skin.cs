using System.Collections.Generic;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class UnlockableCharacterSkin : UnlockableItem
    {
        public const string UnitSkinType = "UnitSkin";

        public PlayerCharacter CharacterSkinToBuy;
        public string SkinTypeAndPath;

        public UnlockableCharacterSkin(string UnitPath, string SkinPath)
            : base(UnitSkinType)
        {
            this.Path = UnitPath;
            this.SkinTypeAndPath = SkinPath;
            IsInShop = false;
        }

        public UnlockableCharacterSkin(string UnitPath, Dictionary<string, string> ActiveHeaderValues)
            : base(UnitSkinType)
        {
            this.Path = UnitPath;
            Load(ActiveHeaderValues);
            SkinTypeAndPath = ActiveHeaderValues["SkinPath"];
        }

        public UnlockableCharacterSkin(string UnitPath, byte UnlockQuantity, bool IsInShop)
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

            SorcererStreetPlayerUnlockInventory.DicCharacterDatabase[Path].ListLockedSkin.Remove(this);
            SorcererStreetPlayerUnlockInventory.DicCharacterDatabase[Path].ListUnlockedSkin.Add(this);

            PlayerCharacter NewCharacter = new PlayerCharacter(SkinTypeAndPath, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);

            if (UnlockQuantity > 0)
            {
                if (ConditionsOwner.Inventory.DicOwnedCharacter.ContainsKey(Path))
                {
                    ConditionsOwner.Inventory.DicOwnedCharacter[Path].ListOwnedUnitSkin.Add(new PlayerCharacterSkin(Path, SkinTypeAndPath, NewCharacter));
                }
                else
                {
                    ConditionsOwner.Inventory.DicOwnedCharacterSkin.Add(SkinTypeAndPath, new PlayerCharacterSkin(Path, SkinTypeAndPath, NewCharacter));
                }

                ListUnlockMessage.Add("You just received " + UnlockQuantity + "x " + NewCharacter.Name + "!");
            }

            if (IsInShop)
            {
                ListUnlockMessage.Add(NewCharacter.Name + " is now available in the shop!");
            }

            return ListUnlockMessage;
        }

        public override string ToString()
        {
            return SkinTypeAndPath;
        }
    }
}
