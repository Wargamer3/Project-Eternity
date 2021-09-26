using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public class PlayerListScriptServer : OnlineScript
    {
        public const string ScriptName = "Player List";

        private readonly ICollection<string> ListPlayerName;

        public PlayerListScriptServer(ICollection<string> ListPlayerName)
            : base(ScriptName)
        {
            this.ListPlayerName = ListPlayerName;
        }

        public override OnlineScript Copy()
        {
            return new PlayerListScriptServer(ListPlayerName);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendInt32(ListPlayerName.Count);
            foreach (string ActiveRoom in ListPlayerName)
            {
                WriteBuffer.AppendString(ActiveRoom);
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
