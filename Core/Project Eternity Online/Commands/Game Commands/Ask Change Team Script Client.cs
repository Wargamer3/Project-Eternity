using System;

namespace ProjectEternity.Core.Online
{
    public class AskChangeTeamScriptClient : OnlineScript
    {
        private readonly int Team;

        public AskChangeTeamScriptClient(int Team)
            : base("Ask Change Team")
        {
            this.Team = Team;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendInt32(Team);
        }

        protected internal override void Execute(IOnlineConnection Host)
        {
            throw new NotImplementedException();
        }

        protected internal override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
