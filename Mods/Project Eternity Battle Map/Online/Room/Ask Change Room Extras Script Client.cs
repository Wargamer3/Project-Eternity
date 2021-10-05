using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class AskChangeRoomExtrasBattleScriptClient : OnlineScript
    {
        private readonly int MaxKill;
        private readonly int MaxGameLengthInMinutes;

        public AskChangeRoomExtrasBattleScriptClient(int MaxKill, int MaxGameLengthInMinutes)
            : base("Ask Change Room Extras")
        {
            this.MaxKill = MaxKill;
            this.MaxGameLengthInMinutes = MaxGameLengthInMinutes;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendInt32(MaxKill);
            WriteBuffer.AppendInt32(MaxGameLengthInMinutes);
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
