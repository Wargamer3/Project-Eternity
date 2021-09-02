using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class RoomListScriptClient : OnlineScript
    {
        public const string ScriptName = "Room List";

        private readonly Loby Owner;

        private List<RoomInformations> ListRoomUpdates;

        public RoomListScriptClient(Loby Owner)
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
            Owner.UpdateRooms(ListRoomUpdates);
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
                    ListRoomUpdates.Add(new ServerRoomInformations(RoomID, IsDead));
                }
                else
                {
                    string Name = Sender.ReadString();
                    string RoomType = Sender.ReadString();
                    string RoomSubtype = Sender.ReadString();
                    bool IsPlaying = Sender.ReadBoolean();
                    int MaxPlayer = Sender.ReadInt32();
                    int CurrentClientCount = Sender.ReadInt32();

                    ListRoomUpdates.Add(new ServerRoomInformations(RoomID, Name, RoomType, RoomSubtype, IsPlaying, MaxPlayer, CurrentClientCount));
                }
            }
        }
    }
}
