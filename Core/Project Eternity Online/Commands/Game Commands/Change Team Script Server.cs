using System;

namespace ProjectEternity.Core.Online
{
    public class ChangeTeamScriptServer : OnlineScript
    {
        public const string ScriptName = "Change Team";

        private readonly string PlayerID;
        private readonly int Team;

        public ChangeTeamScriptServer(string PlayerID, int Team)
            : base(ScriptName)
        {
            this.PlayerID = PlayerID;
            this.Team = Team;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(PlayerID);
            WriteBuffer.AppendInt32(Team);
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            throw new NotImplementedException();
        }

        protected internal override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
