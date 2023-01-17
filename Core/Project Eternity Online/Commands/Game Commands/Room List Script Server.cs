using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Online
{
    public class RoomListScriptServer : OnlineScript
    {
        public const string ScriptName = "Room List";

        private readonly GameServer Owner;
        private readonly ICollection<IRoomInformations> ListRoomUpdates;

        public RoomListScriptServer(GameServer Owner, ICollection<IRoomInformations> ListRoomUpdates)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.ListRoomUpdates = ListRoomUpdates;
        }

        public override OnlineScript Copy()
        {
            return new RoomListScriptServer(Owner, ListRoomUpdates);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendInt32(ListRoomUpdates.Count);
            foreach (IRoomInformations ActiveRoom in ListRoomUpdates)
            {
                WriteBuffer.AppendString(ActiveRoom.RoomID);
                WriteBuffer.AppendBoolean(ActiveRoom.IsDead);

                if (!ActiveRoom.IsDead)
                {
                    WriteBuffer.AppendString(ActiveRoom.RoomName);
                    WriteBuffer.AppendString(ActiveRoom.GameMode);
                    WriteBuffer.AppendString(ActiveRoom.RoomSubtype);
                    WriteBuffer.AppendBoolean(ActiveRoom.IsPlaying);
                    WriteBuffer.AppendByte(ActiveRoom.MinNumberOfPlayer);
                    WriteBuffer.AppendByte(ActiveRoom.MaxNumberOfPlayer);
                    WriteBuffer.AppendByte(ActiveRoom.CurrentPlayerCount);
                }
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
