using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class NewUnlocksScriptClient : OnlineScript
    {
        public const string ScriptName = "New Unlocks";

        private readonly GameScreen Owner;

        private List<UnlockableItem> ListUnlockedItem;

        public NewUnlocksScriptClient(GameScreen Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new NewUnlocksScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            List<PendingUnlockScreen> ListPendingUnlocks = new List<PendingUnlockScreen>();

            BattleMapPlayer ConditionsOwner = (BattleMapPlayer)PlayerManager.ListLocalPlayer[0];

            foreach (UnlockableItem NewUnlock in ListUnlockedItem)
            {
                foreach (string UnlockMessage in NewUnlock.Unlock(ConditionsOwner))
                {
                    ListPendingUnlocks.Add(new PendingUnlockScreen(UnlockMessage));
                }
            }
            if (ListPendingUnlocks.Count > 0)
            {
                PendingUnlockScreen.ListPendingUnlocks.AddRange(ListPendingUnlocks);
                Owner.PushScreen(ListPendingUnlocks[0]);
                ListPendingUnlocks.RemoveAt(0);
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            int ListUnlockedItemCount = Sender.ReadInt32();
            ListUnlockedItem = new List<UnlockableItem>(ListUnlockedItemCount);

            for (int I = 0; I < ListUnlockedItemCount; ++I)
            {
                switch (Sender.ReadString())
                {
                    case UnlockableCharacter.CharacterType:
                        ListUnlockedItem.Add(new UnlockableCharacter(Sender.ReadString(), Sender.ReadInt32(), Sender.ReadBoolean()));
                        break;

                    case UnlockableUnit.UnitType:
                        ListUnlockedItem.Add(new UnlockableUnit(Sender.ReadString(), Sender.ReadInt32(), Sender.ReadBoolean()));
                        break;

                    case UnlockableMission.MissionType:
                        ListUnlockedItem.Add(new UnlockableMission(Sender.ReadString(), Sender.ReadInt32(), Sender.ReadBoolean()));
                        break;
                }
            }
        }
    }
}
