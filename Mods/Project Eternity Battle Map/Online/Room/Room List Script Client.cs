using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class RoomListScriptClient : OnlineScript
    {
        public const string ScriptName = "Room List";

        private readonly Lobby Owner;

        private List<RoomInformations> ListRoomUpdates;

        public RoomListScriptClient(Lobby Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;

            ListRoomUpdates = new List<RoomInformations>();
        }

        public override OnlineScript Copy()
        {
            return new RoomListScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            Owner.PopulateRooms(ListRoomUpdates);
        }

        protected override void Read(OnlineReader Sender)
        {
            int ListRoomUpdatesCount = Sender.ReadInt32();

            for (int i = 0; i < ListRoomUpdatesCount; ++i)
            {
                string RoomID = Sender.ReadString();
                bool IsDead = Sender.ReadBoolean();

                if (IsDead)
                {
                    ListRoomUpdates.Add(new BattleMapRoomInformations(RoomID, IsDead));
                }
                else
                {
                    string Name = Sender.ReadString();
                    string RoomType = Sender.ReadString();
                    string RoomSubtype = Sender.ReadString();
                    bool IsPlaying = Sender.ReadBoolean();
                    byte MinPlayer = Sender.ReadByte();
                    byte MaxPlayer = Sender.ReadByte();
                    byte CurrentClientCount = Sender.ReadByte();

                    ListRoomUpdates.Add(new BattleMapRoomInformations(RoomID, Name, RoomType, RoomSubtype, IsPlaying, MinPlayer, MaxPlayer, CurrentClientCount));
                }
            }
        }
    }
}
