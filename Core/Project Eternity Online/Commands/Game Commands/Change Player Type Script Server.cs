using System;

namespace ProjectEternity.Core.Online
{
    public class ChangePlayerTypeScriptServer : OnlineScript
    {
        public const string ScriptName = "Change Player Type";

        private readonly string PlayerID;
        private readonly string PlayerType;

        public ChangePlayerTypeScriptServer(string PlayerID, string PlayerType)
            : base(ScriptName)
        {
            this.PlayerID = PlayerID;
            this.PlayerType = PlayerType;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(PlayerID);
            WriteBuffer.AppendString(PlayerType);
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
