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
        private readonly byte MinNumberOfPlayer;
        private readonly byte MaxNumberOfPlayer;
        private readonly List<string> ListMandatoryMutator;

        public ChangeMapScriptServer(string MapName, string MapType, string MapPath, byte MinNumberOfPlayer, byte MaxNumberOfPlayer, List<string> ListMandatoryMutator)
            : base(ScriptName)
        {
            this.MapName = MapName;
            this.MapType = MapType;
            this.MapPath = MapPath;
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
            WriteBuffer.AppendByte(MinNumberOfPlayer);
            WriteBuffer.AppendByte(MaxNumberOfPlayer);

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
