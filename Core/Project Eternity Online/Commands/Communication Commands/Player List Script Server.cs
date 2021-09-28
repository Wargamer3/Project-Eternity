using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public class PlayerListScriptServer : OnlineScript
    {
        public const string ScriptName = "Player List";

        private readonly Dictionary<string, byte[]> ListPlayerInfo;

        public PlayerListScriptServer(Dictionary<string, byte[]> ListPlayerInfo)
            : base(ScriptName)
        {
            this.ListPlayerInfo = ListPlayerInfo;
        }

        public override OnlineScript Copy()
        {
            return new PlayerListScriptServer(ListPlayerInfo);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendInt32(ListPlayerInfo.Count);
            foreach (KeyValuePair<string, byte[]> ActivePlayerInfo in ListPlayerInfo)
            {
                WriteBuffer.AppendString(ActivePlayerInfo.Key);
                WriteBuffer.AppendByteArray(ActivePlayerInfo.Value);
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
