using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class AskChangeMapScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Change Map";

        private readonly RoomInformations Owner;
        private readonly GameServer OnlineServer;

        public AskChangeMapScriptServer(RoomInformations Owner, GameServer OnlineServer)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.OnlineServer = OnlineServer;
        }

        public override OnlineScript Copy()
        {
            return new AskChangeMapScriptServer(Owner, OnlineServer);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            for (int P = 0; P < Owner.ListUniqueOnlineConnection.Count; P++)
            {
                IOnlineConnection ActiveOnlinePlayer = Owner.ListUniqueOnlineConnection[P];

                ActiveOnlinePlayer.Send(new ChangeMapScriptServer(Owner));
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            Owner.ReadSelectedMap(Sender);
        }
    }
}
