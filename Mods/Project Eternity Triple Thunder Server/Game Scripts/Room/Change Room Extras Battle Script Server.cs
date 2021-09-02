using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class ChangeRoomExtrasBattleScriptServer : OnlineScript
    {
        public const string ScriptName = "Change Room Extras";

        public readonly int MaxKill;
        public readonly int MaxGameLengthInMinutes;

        public ChangeRoomExtrasBattleScriptServer(int MaxKill, int MaxGameLengthInMinutes)
            : base(ScriptName)
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

        protected override void Execute(IOnlineConnection Sender)
        {
            throw new NotImplementedException();
        }

        protected override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
