using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class MoveUnitScriptServer : OnlineScript
    {
        public const string ScriptName = "Move Unit";

        private readonly BattleMapClientGroup ActiveGroup;

        private Vector3 StartPosition;
        private Vector3 FinalPosition;

        public MoveUnitScriptServer(BattleMapClientGroup ActiveGroup)
            : base(ScriptName)
        {
            this.ActiveGroup = ActiveGroup;
        }

        public MoveUnitScriptServer(Vector3 StartPosition, Vector3 FinalPosition)
            : base(ScriptName)
        {
            this.StartPosition = StartPosition;
            this.FinalPosition = FinalPosition;
        }

        public override OnlineScript Copy()
        {
            return new MoveUnitScriptServer(ActiveGroup);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendInt32((int)StartPosition.X);
            WriteBuffer.AppendInt32((int)StartPosition.Y);
            WriteBuffer.AppendInt32((int)StartPosition.Z);

            WriteBuffer.AppendInt32((int)FinalPosition.X);
            WriteBuffer.AppendInt32((int)FinalPosition.Y);
            WriteBuffer.AppendInt32((int)FinalPosition.Z);
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            for (int P = 0; P < ActiveGroup.Room.ListOnlinePlayer.Count; P++)
            {
                if (ActiveGroup.Room.ListOnlinePlayer[P] != Sender)
                {
                    ActiveGroup.Room.ListOnlinePlayer[P].Send(new MoveUnitScriptServer(StartPosition, FinalPosition));
                }
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            StartPosition = new Vector3(Sender.ReadInt32(), Sender.ReadInt32(), Sender.ReadInt32());
            FinalPosition = new Vector3(Sender.ReadInt32(), Sender.ReadInt32(), Sender.ReadInt32());
        }
    }
}
