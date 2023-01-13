using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class ChangePlayerRolesScriptClient : OnlineScript
    {
        public const string ScriptName = "Change Player Roles";

        private readonly RoomInformations Owner;
        private readonly GamePreparationScreen MissionSelectScreen;

        private string PlayerID;
        private readonly List<string> ListRole;

        public ChangePlayerRolesScriptClient(RoomInformations Owner, GamePreparationScreen MissionSelectScreen)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.MissionSelectScreen = MissionSelectScreen;
            ListRole = new List<string>();
        }

        public override OnlineScript Copy()
        {
            return new ChangePlayerRolesScriptClient(Owner, MissionSelectScreen);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            foreach (OnlinePlayerBase ActivePlayer in Owner.ListRoomPlayer)
            {
                if (ActivePlayer.ConnectionID == PlayerID)
                {
                    ActivePlayer.OnlineClient.Roles.Reset();

                    for (int R = 0; R < ListRole.Count; ++R)
                    {
                        ActivePlayer.OnlineClient.Roles.AddRole(ListRole[R]);
                    }
                }
            }

            MissionSelectScreen.UpdateReadyOrHost();
        }

        protected override void Read(OnlineReader Sender)
        {
            PlayerID = Sender.ReadString();
            int ListRoleCount = Sender.ReadByte();

            for (int R = 0; R < ListRoleCount; ++R)
            {
                ListRole.Add(Sender.ReadString());
            }
        }
    }
}
