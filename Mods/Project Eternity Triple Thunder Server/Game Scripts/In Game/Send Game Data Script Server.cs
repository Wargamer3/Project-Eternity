using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    /// <summary>
    /// Receive sent data from client
    /// </summary>
    public class SendGameDataScriptServer : OnlineScript
    {
        public const string ScriptName = "Send Game Data";

        Server Owner;
        string RoomID;
        byte[] ArrayGameData;

        public SendGameDataScriptServer(Server Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return null;
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            //Owner.DicTransferingRoom[RoomID].CurrentGame.RestoreSnapshot(ArrayGameData);
        }

        protected override void Read(OnlineReader Sender)
        {
            RoomID = Sender.ReadString();
            ArrayGameData = Sender.ReadByteArray();
        }
    }
}
