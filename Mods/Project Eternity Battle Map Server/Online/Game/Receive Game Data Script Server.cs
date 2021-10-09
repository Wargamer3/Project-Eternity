using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class ReceiveGameDataScriptServer : OnlineScript
    {
        public const string ScriptName = "Receive Game Data";

        private readonly GameClientGroup ActiveGroup;
        private readonly IOnlineConnection Owner;

        public ReceiveGameDataScriptServer(GameClientGroup ActiveGroup, IOnlineConnection Owner)
            : base(ScriptName)
        {
            this.ActiveGroup = ActiveGroup;
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new ReceiveGameDataScriptServer(ActiveGroup, Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendByteArray(ActiveGroup.CurrentGame.GetSnapshotData());
        }

        protected override void Execute(IOnlineConnection Sender)
        {
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
