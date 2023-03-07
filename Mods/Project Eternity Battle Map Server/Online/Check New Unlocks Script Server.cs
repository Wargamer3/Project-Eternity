using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class CheckNewUnlocksScriptServer : OnlineScript
    {
        public const string ScriptName = "Check New Unlocks";

        private readonly GameServer Owner;
        private string ID;

        public CheckNewUnlocksScriptServer(GameServer Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new CheckNewUnlocksScriptServer(Owner);
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

            if (ListUnlockedItem.Count > 0)
            {
                Owner.Database.SavePlayerInventory(ID, ConditionsOwner);
            }

            Sender.Send(new NewUnlocksScriptServer(ListUnlockedItem));
        }

        protected override void Read(OnlineReader Sender)
        {
            ID = Sender.ReadString();
        }
    }
}
