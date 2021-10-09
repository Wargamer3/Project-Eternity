using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class ReceiveGameDataScriptClient : OnlineScript
    {
        public const string ScriptName = "Receive Game Data";

        private readonly BattleMapOnlineClient Owner;

        private byte[] ArrayGameData;

        public ReceiveGameDataScriptClient(BattleMapOnlineClient Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;

            ArrayGameData = null;
        }

        public override OnlineScript Copy()
        {
            return new ReceiveGameDataScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            Owner.BattleMapGame.Load(ArrayGameData);
        }

        protected override void Read(OnlineReader Sender)
        {
            ArrayGameData = Sender.ReadByteArray();
        }
    }
}
