using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class AskChangeRoomExtrasMissionScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Change Room Extras";

        private readonly BattleMapRoomInformations Owner;

        public AskChangeRoomExtrasMissionScriptServer(BattleMapRoomInformations Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new AskChangeRoomExtrasMissionScriptServer(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            //TODO: Update database

            for (int P = 0; P < Owner.ListUniqueOnlineConnection.Count; P++)
            {
                IOnlineConnection ActiveOnlinePlayer = Owner.ListUniqueOnlineConnection[P];

                ActiveOnlinePlayer.Send(new ChangeRoomExtrasMissionScriptServer());
            }
        }

        protected override void Read(OnlineReader Sender)
        {
        }
    }
}
