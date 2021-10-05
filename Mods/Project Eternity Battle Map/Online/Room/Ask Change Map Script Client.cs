using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class AskChangeMapScriptClient : OnlineScript
    {
        private readonly string MapType;
        private readonly string MapPath;

        public AskChangeMapScriptClient(string MapType, string MapPath)
            : base("Ask Change Map")
        {
            this.MapType = MapType;
            this.MapPath = MapPath;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(MapType);
            WriteBuffer.AppendString(MapPath);
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
