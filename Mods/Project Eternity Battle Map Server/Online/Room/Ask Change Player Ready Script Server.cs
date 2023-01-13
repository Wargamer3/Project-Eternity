using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class AskChangePlayerTypeScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Change Player Ready";

        private readonly RoomInformations Owner;

        public AskChangePlayerTypeScriptServer(RoomInformations Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new AskChangePlayerTypeScriptServer(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            if (Sender.Roles.IsRoomReady)
            {
                Sender.Roles.RemoveRole(RoleManager.Ready);
            }
            else
            {
                Sender.Roles.AddRole(RoleManager.Ready);
            }

            for (int P = 0; P < Owner.ListOnlinePlayer.Count; P++)
            {
                Owner.ListOnlinePlayer[P].Send(new ChangePlayerRolesScriptServer(Sender.ID, Sender.Roles.ListActiveRole));
            }
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
