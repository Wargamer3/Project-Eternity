using System;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class AskChangeMapScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Change Map";

        private readonly RoomInformations Owner;
        private readonly GameServer OnlineServer;

        private string CurrentDifficulty;
        private string MissionPath;

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
            Owner.MapPath = MissionPath;
            for (int P = 0; P < Owner.ListOnlinePlayer.Count; P++)
            {
                IOnlineConnection ActiveOnlinePlayer = Owner.ListOnlinePlayer[P];

                ActiveOnlinePlayer.Send(new ChangeMapScriptServer(CurrentDifficulty, MissionPath));
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            CurrentDifficulty = Sender.ReadString();
            MissionPath = Sender.ReadString();
        }
    }
}
