using ProjectEternity.Core.Item;
using System;

namespace ProjectEternity.Core.Online
{
    public class ConnectionSuccessScriptClient : OnlineScript
    {
        public const string ScriptName = "Connection Success";

        public ConnectionSuccessScriptClient()
            : base(ScriptName)
        {
        }

        public override OnlineScript Copy()
        {
            return new ConnectionSuccessScriptClient();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Execute(IOnlineConnection Host)
        {
            IniFile ConnectionInfo = IniFile.ReadFromFile("Connection Info.ini");
            string Username = ConnectionInfo.ReadField("Player Info", "Username");
            string Password = ConnectionInfo.ReadField("Player Info", "Password");
            Host.Send(new AskRoomListScriptClient());
            Host.Send(new AskLoginScriptClient(Username, Password));
        }

        protected internal override void Read(OnlineReader Sender)
        {
        }
    }
}
