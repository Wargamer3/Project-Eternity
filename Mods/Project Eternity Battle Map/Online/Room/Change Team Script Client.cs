using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class ChangeTeamScriptClient : OnlineScript
    {
        public const string ScriptName = "Change Team";

        private readonly RoomInformations Owner;

        private string PlayerID;
        private int Team;

        public ChangeTeamScriptClient(RoomInformations Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new ChangeTeamScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            foreach (OnlinePlayer ActivePlayer in Owner.ListRoomPlayer)
            {
                if (ActivePlayer.ConnectionID == PlayerID)
                {
                    ActivePlayer.Team = Team;
                }
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            PlayerID = Sender.ReadString();
            Team = Sender.ReadInt32();
        }
    }
}
