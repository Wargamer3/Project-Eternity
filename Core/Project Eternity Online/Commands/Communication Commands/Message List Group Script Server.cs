using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public class MessageListGroupScriptServer : OnlineScript
    {
        public const string ScriptName = "Message List Group";
        private readonly string GroupID;
        private readonly Dictionary<string, ChatManager.MessageColors> OldMessages;

        public MessageListGroupScriptServer(string GroupID, Dictionary<string, ChatManager.MessageColors> OldMessages)
            : base(ScriptName)
        {
            this.GroupID = GroupID;
            this.OldMessages = OldMessages;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(GroupID);
            WriteBuffer.AppendInt32(OldMessages.Count);
            foreach (KeyValuePair<string, ChatManager.MessageColors> ActivePlayerInfo in OldMessages)
            {
                WriteBuffer.AppendString(ActivePlayerInfo.Key);
                WriteBuffer.AppendByte((byte)ActivePlayerInfo.Value);
            }
        }

        protected internal override void Execute(IOnlineConnection ActivePlayer)
        {
            throw new NotImplementedException();
        }

        protected internal override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
