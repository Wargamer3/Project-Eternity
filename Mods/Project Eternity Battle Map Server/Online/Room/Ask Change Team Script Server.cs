using System;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class AskChangeTeamScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Change Team";

        private readonly RoomInformations Owner;

        private int NewTeam;

        public AskChangeTeamScriptServer(RoomInformations Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new AskChangeTeamScriptServer(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            foreach (OnlinePlayerBase ActivePlayer in Owner.ListRoomPlayer)
            {
                if (ActivePlayer.ConnectionID == Sender.ID)
                {
                    ActivePlayer.TeamIndex = NewTeam;
                }
            }

            for (int P = 0; P < Owner.ListUniqueOnlineConnection.Count; P++)
            {
                IOnlineConnection ActiveOnlinePlayer = Owner.ListUniqueOnlineConnection[P];

                ActiveOnlinePlayer.Send(new ChangeTeamScriptServer(Sender.ID, NewTeam));
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            NewTeam = Sender.ReadInt32();
        }
    }
}
