using System;

namespace ProjectEternity.Core.Online
{
    public class ChangeCharacterScriptServer : OnlineScript
    {
        public const string ScriptName = "Change Character";

        private readonly string PlayerID;
        private readonly string CharacterType;

        public ChangeCharacterScriptServer(string PlayerID, string CharacterType)
            : base(ScriptName)
        {
            this.PlayerID = PlayerID;
            this.CharacterType = CharacterType;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(PlayerID);
            WriteBuffer.AppendString(CharacterType);
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
