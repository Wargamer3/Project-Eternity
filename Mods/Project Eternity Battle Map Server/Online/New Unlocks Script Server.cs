using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class NewUnlocksScriptServer : OnlineScript
    {
        public const string ScriptName = "New Unlocks";

        private readonly List<UnlockableItem> ListUnlockedItem;

        public NewUnlocksScriptServer(List<UnlockableItem> ListUnlockedItem)
            : base(ScriptName)
        {
            this.ListUnlockedItem = ListUnlockedItem;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendInt32(ListUnlockedItem.Count);

            for (int I = 0; I < ListUnlockedItem.Count; ++I)
            {
                WriteBuffer.AppendString(ListUnlockedItem[I].ItemType);
                WriteBuffer.AppendString(ListUnlockedItem[I].Path);
                WriteBuffer.AppendByte(ListUnlockedItem[I].UnlockQuantity);
                WriteBuffer.AppendBoolean(ListUnlockedItem[I].IsInShop);
            }
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            throw new NotImplementedException();
        }

        protected override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
