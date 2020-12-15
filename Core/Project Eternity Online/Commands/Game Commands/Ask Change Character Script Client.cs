using System;

namespace ProjectEternity.Core.Online
{
    public class AskChangeCharacterScriptClient : OnlineScript
    {
        private readonly string CharacterType;

        public AskChangeCharacterScriptClient(string CharacterType)
            : base("Ask Change Character")
        {
            this.CharacterType = CharacterType;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(CharacterType);
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
