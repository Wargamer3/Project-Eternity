using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public class SendRolesScriptClient : OnlineScript
    {
        public const string ScriptName = "Send Roles";

        private readonly List<string> ListRole;

        public SendRolesScriptClient()
            : base(ScriptName)
        {
            ListRole = new List<string>();
        }

        public override OnlineScript Copy()
        {
            return new SendRolesScriptClient();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Sender)
        {
            Sender.Roles.Reset();

            for (int R = 0; R < ListRole.Count; ++R)
            {
                Sender.Roles.AddRole(ListRole[R]);
            }
        }

        protected internal override void Read(OnlineReader Sender)
        {
            int ListRoleCount = Sender.ReadByte();

            for (int R = 0; R < ListRoleCount; ++R)
            {
                ListRole.Add(Sender.ReadString());
            }
        }
    }
}
