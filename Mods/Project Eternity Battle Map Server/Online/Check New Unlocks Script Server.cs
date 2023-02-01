using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class CheckNewUnlocksScriptServer : OnlineScript
    {
        public const string ScriptName = "Check New Unlocks";

        public CheckNewUnlocksScriptServer()
            : base(ScriptName)
        {
        }

        public override OnlineScript Copy()
        {
            return new CheckNewUnlocksScriptServer();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            BattleMapPlayer ConditionsOwner = (BattleMapPlayer)Sender.ExtraInformation;

            List<UnlockableItem> ListUnlockedItem = new List<UnlockableItem>();

            for (int C = ConditionsOwner.UnlockInventory.ListLockedCharacter.Count - 1; C >= 0; C--)
            {
                UnlockableCharacter ActiveCharacter = ConditionsOwner.UnlockInventory.ListLockedCharacter[C];
                if (BattleMapItemUnlockConditionsEvaluator.IsValid(ActiveCharacter.UnlockConditions, ConditionsOwner))
                {
                    ActiveCharacter.UnlockConditions.IsUnlocked = true;
                    ConditionsOwner.UnlockInventory.ListLockedCharacter.Remove(ActiveCharacter);
                    ConditionsOwner.UnlockInventory.ListUnlockedCharacter.Add(ActiveCharacter);
                    ListUnlockedItem.Add(ActiveCharacter);

                    for (int Q = 0; Q < ActiveCharacter.UnlockQuantity; ++Q)
                    {
                        Character NewCharacter = new Character();
                        NewCharacter.FullName = ActiveCharacter.Path;
                        ConditionsOwner.Inventory.ListOwnedCharacter.Add(NewCharacter);
                    }
                }
            }

            Sender.Send(new NewUnlocksScriptServer(ListUnlockedItem));
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
