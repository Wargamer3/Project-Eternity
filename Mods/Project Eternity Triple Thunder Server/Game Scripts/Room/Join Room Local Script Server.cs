using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class JoinRoomLocalScriptServer : OnlineScript
    {
        public const string ScriptName = "Join Room Local";

        private readonly string RoomID;
        private readonly string CurrentDifficulty;
        private readonly List<Player> ListJoiningPlayer;
        private readonly GameClientGroup ActiveGroup;

        public JoinRoomLocalScriptServer(string RoomID, string CurrentDifficulty, List<Player> ListJoiningPlayer, GameClientGroup ActiveGroup)
            : base(ScriptName)
        {
            this.RoomID = RoomID;
            this.CurrentDifficulty = CurrentDifficulty;
            this.ListJoiningPlayer = ListJoiningPlayer;
            this.ActiveGroup = ActiveGroup;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(RoomID);
            WriteBuffer.AppendString(ActiveGroup.Room.RoomName);
            WriteBuffer.AppendString(ActiveGroup.Room.MapPath);
            WriteBuffer.AppendString(ActiveGroup.Room.GameMode);
            WriteBuffer.AppendString(ActiveGroup.Room.RoomSubtype);
            WriteBuffer.AppendString(CurrentDifficulty);

            WriteBuffer.AppendInt32(ListJoiningPlayer.Count);
            for (int P = 0; P < ListJoiningPlayer.Count; ++P)
            {
                WriteBuffer.AppendString(ListJoiningPlayer[P].ConnectionID);
            }

            WriteBuffer.AppendByteArray(ActiveGroup.Room.GetRoomInfo());

            WriteBuffer.AppendBoolean(ActiveGroup.CurrentGame != null);
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            throw new NotImplementedException();
        }

        protected override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
