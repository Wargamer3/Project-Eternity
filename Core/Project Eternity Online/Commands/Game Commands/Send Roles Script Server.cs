using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public class SendRolesScriptServer : OnlineScript
    {
        public const string ScriptName = "Send Roles";

        private readonly List<string> ListRole;

        public SendRolesScriptServer()
            : base(ScriptName)
        {
            ListRole = new List<string>();
        }

        public SendRolesScriptServer(List<string> ListRole)
            : base(ScriptName)
        {
            this.ListRole = ListRole;
        }

        public override OnlineScript Copy()
        {
            return new SendRolesScriptServer();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendByte((byte)ListRole.Count);

            for (int R = 0; R < ListRole.Count; ++R)
            {
                WriteBuffer.AppendString(ListRole[R]);
            }
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
