using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class AskChangeMapScriptClient : OnlineScript
    {
        private readonly string MapName;
        private readonly string MapType;
        private readonly string MapPath;
        private readonly string GameMode;
        private readonly byte MinNumberOfPlayer;
        private readonly byte MaxNumberOfPlayer;
        private readonly List<string> ListMandatoryMutator;

        public AskChangeMapScriptClient(string MapName, string MapType, string MapPath, string GameMode, byte MinNumberOfPlayer, byte MaxNumberOfPlayer, List<string> ListMandatoryMutator)
            : base("Ask Change Map")
        {
            this.MapName = MapName;
            this.MapType = MapType;
            this.MapPath = MapPath;
            this.GameMode = GameMode;
            this.MinNumberOfPlayer = MinNumberOfPlayer;
            this.MaxNumberOfPlayer = MaxNumberOfPlayer;
            this.ListMandatoryMutator = ListMandatoryMutator;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(MapName);
            WriteBuffer.AppendString(MapType);
            WriteBuffer.AppendString(MapPath);
            WriteBuffer.AppendString(GameMode);
            WriteBuffer.AppendByte(MinNumberOfPlayer);
            WriteBuffer.AppendByte(MaxNumberOfPlayer);

            WriteBuffer.AppendInt32(ListMandatoryMutator.Count);
            for (int M = 0; M < ListMandatoryMutator.Count; M++)
            {
                WriteBuffer.AppendString(ListMandatoryMutator[M]);
            }
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
