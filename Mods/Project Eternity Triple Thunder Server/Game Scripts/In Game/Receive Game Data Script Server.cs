using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class ReceiveGameDataScriptServer : OnlineScript
    {
        public const string ScriptName = "Receive Game Data";

        private readonly TripleThunderClientGroup ActiveGroup;
        private readonly IOnlineConnection Owner;
        private FightingZone ActiveGame { get { return ActiveGroup.TripleThunderGame; } }

        public ReceiveGameDataScriptServer(TripleThunderClientGroup ActiveGroup, IOnlineConnection Owner)
            : base(ScriptName)
        {
            this.ActiveGroup = ActiveGroup;
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new ReceiveGameDataScriptServer(ActiveGroup, Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            List<uint> ListLocalPlayerID = new List<uint>();
            foreach (Player ActivePlayer in ActiveGame.ListLocalPlayer)
            {
                if (ActivePlayer.OnlineClient == Owner)
                {
                    ListLocalPlayerID.Add(ActivePlayer.InGameRobot.ID);
                }
            }

            WriteBuffer.AppendInt32(ListLocalPlayerID.Count);
            foreach (uint ActivePlayerID in ListLocalPlayerID)
            {
                WriteBuffer.AppendUInt32(ActivePlayerID);
            }

            WriteBuffer.AppendInt32(ActiveGame.ListAllPlayer.Count);
            foreach (Player ActivePlayer in ActiveGame.ListAllPlayer)
            {
                WriteBuffer.AppendUInt32(ActivePlayer.InGameRobot.ID);
                WriteBuffer.AppendString(ActivePlayer.ConnectionID);
                WriteBuffer.AppendString(ActivePlayer.Name);
                WriteBuffer.AppendInt32(ActivePlayer.Team);
            }

            WriteBuffer.AppendByteArray(ActiveGroup.CurrentGame.GetSnapshotData());
        }

        protected override void Execute(IOnlineConnection Sender)
        {
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
