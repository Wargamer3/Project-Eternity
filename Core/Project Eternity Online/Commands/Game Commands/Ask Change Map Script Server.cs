using System;

namespace ProjectEternity.Core.Online
{
    public class AskChangeMapScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Change Map";

        private readonly IRoomInformations Owner;
        private readonly GameServer OnlineServer;

        private string CurrentDifficulty;
        private string MissionPath;

        public AskChangeMapScriptServer(IRoomInformations Owner, GameServer OnlineServer)
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

        protected internal override void Execute(IOnlineConnection Sender)
        {
            Owner.MapPath = MissionPath;
            for (int P = 0; P < Owner.ListOnlinePlayer.Count; P++)
            {
                IOnlineConnection ActiveOnlinePlayer = Owner.ListOnlinePlayer[P];

                ActiveOnlinePlayer.Send(new ChangeMapScriptServer(CurrentDifficulty, MissionPath));
            }
        }

        protected internal override void Read(OnlineReader Sender)
        {
            CurrentDifficulty = Sender.ReadString();
            MissionPath = Sender.ReadString();
        }
    }
}
