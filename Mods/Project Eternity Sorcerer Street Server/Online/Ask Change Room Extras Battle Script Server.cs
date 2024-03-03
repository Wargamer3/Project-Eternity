using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen.Server
{
    public class AskChangeRoomExtrasBattleScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Change Room Extras";

        private readonly SorcererStreetRoomInformations Owner;

        public int MaxKill;
        public int MaxGameLengthInMinutes;

        public AskChangeRoomExtrasBattleScriptServer(SorcererStreetRoomInformations Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new AskChangeRoomExtrasBattleScriptServer(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            //TODO: Update database
            Owner.MaxKill = MaxKill;
            Owner.MaxGameLengthInMinutes = MaxGameLengthInMinutes;

            foreach (IOnlineConnection ActiveOnlinePlayer in Owner.ListUniqueOnlineConnection)
            {
                ActiveOnlinePlayer.Send(new ChangeRoomExtrasBattleScriptServer(MaxKill, MaxGameLengthInMinutes));
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            MaxKill = Sender.ReadInt32();
            MaxGameLengthInMinutes = Sender.ReadInt32();
        }
    }
}
