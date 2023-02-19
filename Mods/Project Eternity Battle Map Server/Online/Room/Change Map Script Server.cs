using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class ChangeMapScriptServer : OnlineScript
    {
        public const string ScriptName = "Change Map";

        private readonly string MapName;
        private readonly string MapType;
        private readonly string MapPath;
        private readonly string GameMode;
        private readonly byte MinNumberOfPlayer;
        private readonly byte MaxNumberOfPlayer;
        private readonly byte MaxSquadPerPlayer;
        private readonly GameModeInfo GameInfo;
        private readonly List<string> ListMandatoryMutator;

        public ChangeMapScriptServer(string MapName, string MapType, string MapPath, string GameMode, byte MinNumberOfPlayer, byte MaxNumberOfPlayer, byte MaxSquadPerPlayer, GameModeInfo GameInfo, List<string> ListMandatoryMutator)
            : base(ScriptName)
        {
            this.MapName = MapName;
            this.MapType = MapType;
            this.MapPath = MapPath;
            this.GameMode = GameMode;
            this.MinNumberOfPlayer = MinNumberOfPlayer;
            this.MaxNumberOfPlayer = MaxNumberOfPlayer;
            this.MaxSquadPerPlayer = MaxSquadPerPlayer;
            this.GameInfo = GameInfo;
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
            WriteBuffer.AppendByte(MaxSquadPerPlayer);

            GameInfo.Write(WriteBuffer);

            WriteBuffer.AppendInt32(ListMandatoryMutator.Count);
            for (int M = 0; M < ListMandatoryMutator.Count; M++)
            {
                WriteBuffer.AppendString(ListMandatoryMutator[M]);
            }
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
