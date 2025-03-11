using System.Collections.Generic;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class UnlockableCharacterAlt : UnlockableItem
    {
        public const string UnitAltType = "UnitAlt";

        public PlayerCharacter UnitAltToBuy;
        public string AltTypeAndPath;

        public UnlockableCharacterAlt(string UnitPath, string AltPath)
            : base(UnitAltType)
        {
            this.Path = UnitPath;
            this.AltTypeAndPath = AltPath;
            IsInShop = false;
        }

        public UnlockableCharacterAlt(string UnitPath, Dictionary<string, string> ActiveHeaderValues)
            : base(UnitAltType)
        {
            this.Path = UnitPath;
            Load(ActiveHeaderValues);
            AltTypeAndPath = ActiveHeaderValues["AltPath"];
        }

        public UnlockableCharacterAlt(string UnitPath, byte UnlockQuantity, bool IsInShop)
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

            SorcererStreetPlayerUnlockInventory.DicCharacterDatabase[Path].ListLockedAlt.Remove(this);
            SorcererStreetPlayerUnlockInventory.DicCharacterDatabase[Path].ListUnlockedAlt.Add(this);

            PlayerCharacter NewCharacter = new PlayerCharacter(AltTypeAndPath, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);

            if (UnlockQuantity > 0)
            {
                if (ConditionsOwner.Inventory.DicOwnedCharacter.ContainsKey(Path))
                {
                    ConditionsOwner.Inventory.DicOwnedCharacter[Path].ListOwnedUnitAlt.Add(new PlayerCharacterSkin(Path, AltTypeAndPath, NewCharacter));
                }
                else
                {
                    ConditionsOwner.Inventory.DicOwnedCharacterAlt.Add(AltTypeAndPath, new PlayerCharacterSkin(Path, AltTypeAndPath, NewCharacter));
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
            return AltTypeAndPath;
        }
    }
}
