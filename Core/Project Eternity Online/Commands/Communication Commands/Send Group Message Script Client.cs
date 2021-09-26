using System;

namespace ProjectEternity.Core.Online
{
    public class SendGroupMessageScriptClient : OnlineScript
    {
        private readonly string Message;
        private readonly byte ColorR;
        private readonly byte ColorG;
        private readonly byte ColorB;

        public SendGroupMessageScriptClient(string Message, byte ColorR, byte ColorG, byte ColorB)
            : base("Send Group Message")
        {
            this.Message = Message;
            this.ColorR = ColorR;
            this.ColorG = ColorG;
            this.ColorB = ColorB;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(Message);
            WriteBuffer.AppendByte(ColorR);
            WriteBuffer.AppendByte(ColorG);
            WriteBuffer.AppendByte(ColorB);
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
