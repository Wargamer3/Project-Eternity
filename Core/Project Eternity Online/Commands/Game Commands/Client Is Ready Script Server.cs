using System;

namespace ProjectEternity.Core.Online
{
    public class ClientIsReadyScriptServer : OnlineScript
    {
        public const string ScriptName = "Client Is Ready";

        private readonly GameClientGroup ActiveGroup;

        public ClientIsReadyScriptServer(GameClientGroup ActiveGroup)
            : base(ScriptName)
        {
            this.ActiveGroup = ActiveGroup;
        }

        public override OnlineScript Copy()
        {
            return new ClientIsReadyScriptServer(ActiveGroup);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection ActivePlayer)
        {
            ActivePlayer.IsGameReady = true;

            foreach (IOnlineConnection ActiveRoomPlayer in ActiveGroup.Room.ListOnlinePlayer)
            {
                if (!ActiveRoomPlayer.IsGameReady)
                {
                    return;
                }
            }

            ActiveGroup.IsGameReady = true;
            foreach (IOnlineConnection ActiveRoomPlayer in ActiveGroup.Room.ListOnlinePlayer)
            {
                ActiveRoomPlayer.Send(new ServerIsReadyScriptServer());
            }
        }

        protected internal override void Read(OnlineReader Sender)
        {
        }
    }
}
