using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class ReceiveGameDataScriptClient : OnlineScript
    {
        public const string ScriptName = "Receive Game Data";

        private readonly TripleThunderOnlineClient Owner;

        private List<uint> ListLocalCharacterID;
        private Dictionary<uint, Player> DicAllPlayer;
        private byte[] ArrayGameData;

        public ReceiveGameDataScriptClient(TripleThunderOnlineClient Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;

            ListLocalCharacterID = new List<uint>();
            DicAllPlayer = new Dictionary<uint, Player>();
            ArrayGameData = null;
        }

        public override OnlineScript Copy()
        {
            return new ReceiveGameDataScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            Owner.TripleThunderGame.Load(ArrayGameData, ListLocalCharacterID, DicAllPlayer);
        }

        protected override void Read(OnlineReader Sender)
        {
            int ListLocalCharacterIDCount = Sender.ReadInt32();
            for (int i = 0; i < ListLocalCharacterIDCount; ++i)
            {
                ListLocalCharacterID.Add(Sender.ReadUInt32());
            }

            int ListAllCharacterIDCount = Sender.ReadInt32();
            for (int i = 0; i < ListAllCharacterIDCount; ++i)
            {
                uint RobotID = Sender.ReadUInt32();
                Player NewPlayer = new Player(Sender.ReadString(), Sender.ReadString(), Player.PlayerTypes.Online, true, Sender.ReadInt32());
                DicAllPlayer.Add(RobotID, NewPlayer);
            }

            ArrayGameData = Sender.ReadByteArray();
        }
    }
}
