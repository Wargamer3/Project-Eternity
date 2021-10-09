using System;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class MoveCursorScriptServer : OnlineScript
    {
        public const string ScriptName = "Move Cursor";

        private readonly BattleMapClientGroup ActiveGroup;

        private float CursorPositionX;
        private float CursorPositionY;

        public MoveCursorScriptServer(BattleMapClientGroup ActiveGroup)
            : base(ScriptName)
        {
            this.ActiveGroup = ActiveGroup;
        }

        public MoveCursorScriptServer(float CursorPositionX, float CursorPositionY)
            : base(ScriptName)
        {
            this.CursorPositionX = CursorPositionX;
            this.CursorPositionY = CursorPositionY;
        }

        public override OnlineScript Copy()
        {
            return new MoveCursorScriptServer(ActiveGroup);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendFloat(CursorPositionX);
            WriteBuffer.AppendFloat(CursorPositionY);
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            for (int P = 0; P < ActiveGroup.Room.ListOnlinePlayer.Count; P++)
            {
                ActiveGroup.Room.ListOnlinePlayer[P].Send(new MoveCursorScriptServer(CursorPositionX, CursorPositionY));
            }
        }
        
        protected override void Read(OnlineReader Sender)
        {
            CursorPositionX = Sender.ReadFloat();
            CursorPositionY = Sender.ReadFloat();
        }
    }
}
