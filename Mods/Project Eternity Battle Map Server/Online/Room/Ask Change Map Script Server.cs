using ProjectEternity.Core.Online;
using System;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class AskChangeMapScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Change Map";

        private readonly PVPRoomInformations Owner;
        private readonly GameServer OnlineServer;

        private string MapType;
        private string MapPath;

        public AskChangeMapScriptServer(PVPRoomInformations Owner, GameServer OnlineServer)
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
            Owner.MapType = MapType;
            Owner.MapPath = MapPath;
            for (int P = 0; P < Owner.ListOnlinePlayer.Count; P++)
            {
                IOnlineConnection ActiveOnlinePlayer = Owner.ListOnlinePlayer[P];

                ActiveOnlinePlayer.Send(new ChangeMapScriptServer(MapType, MapPath));
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            MapType = Sender.ReadString();
            MapPath = Sender.ReadString();
        }
    }
}
