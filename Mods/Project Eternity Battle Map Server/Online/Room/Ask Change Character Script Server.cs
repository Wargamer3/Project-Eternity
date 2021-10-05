using System;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class AskChangeCharacterScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Change Character";

        private readonly RoomInformations Owner;

        private string NewCharacterType;

        public AskChangeCharacterScriptServer(RoomInformations Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new AskChangeCharacterScriptServer(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            foreach (OnlinePlayer ActivePlayer in Owner.ListRoomPlayer)
            {
                if (ActivePlayer.ConnectionID == Sender.ID)
                {
                }
            }

            for (int P = 0; P < Owner.ListOnlinePlayer.Count; P++)
            {
                IOnlineConnection ActiveOnlinePlayer = Owner.ListOnlinePlayer[P];

                ActiveOnlinePlayer.Send(new ChangeCharacterScriptServer(Sender.ID, NewCharacterType));
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            NewCharacterType = Sender.ReadString();
        }
    }
}
