using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class AskChangeMapScriptClient : OnlineScript
    {
        private readonly RoomInformations Room;

        public AskChangeMapScriptClient(RoomInformations Room)
            : base("Ask Change Map")
        {
            this.Room = Room;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(Room.MapName);
            WriteBuffer.AppendString(Room.MapModName);
            WriteBuffer.AppendString(Room.MapPath);
            WriteBuffer.AppendString(Room.GameMode);
            WriteBuffer.AppendByte(Room.MinNumberOfPlayer);
            WriteBuffer.AppendByte(Room.MaxNumberOfPlayer);
            WriteBuffer.AppendByte(Room.MaxSquadPerPlayer);

            Room.GameInfo.Write(WriteBuffer);

            WriteBuffer.AppendInt32(Room.ListMandatoryMutator.Count);
            for (int M = 0; M < Room.ListMandatoryMutator.Count; M++)
            {
                WriteBuffer.AppendString(Room.ListMandatoryMutator[M]);
            }

            WriteBuffer.AppendInt32(Room.ListMapTeam.Count);
            for (int T = 0; T < Room.ListMapTeam.Count; T++)
            {
                WriteBuffer.AppendByte(Room.ListMapTeam[T].R);
                WriteBuffer.AppendByte(Room.ListMapTeam[T].G);
                WriteBuffer.AppendByte(Room.ListMapTeam[T].B);
            }
        }

        protected override void Execute(IOnlineConnection Host)
        {
            throw new NotImplementedException();
        }

        protected override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
