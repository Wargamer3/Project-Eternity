using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public class FriendListScriptServer : OnlineScript
    {
        public const string ScriptName = "Friend List";

        private readonly List<PlayerPOCO> ListFriend;

        public FriendListScriptServer(List<PlayerPOCO> ListFriend)
            : base(ScriptName)
        {
            this.ListFriend = ListFriend;
        }

        public override OnlineScript Copy()
        {
            return new FriendListScriptServer(ListFriend);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendInt32(ListFriend.Count);
            foreach (PlayerPOCO ActivePlayerInfo in ListFriend)
            {
                WriteBuffer.AppendString(ActivePlayerInfo.ID);
                WriteBuffer.AppendByteArray(ActivePlayerInfo.Info);
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
