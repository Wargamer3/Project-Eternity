using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class MoveCursorScriptClient : OnlineScript
    {
        public const string ScriptName = "Move Cursor";

        private readonly BattleMapOnlineClient Owner;

        private float CursorPositionX;
        private float CursorPositionY;

        public MoveCursorScriptClient(float CursorPositionX, float CursorPositionY)
            : base(ScriptName)
        {
            this.CursorPositionX = CursorPositionX;
            this.CursorPositionY = CursorPositionY;
        }

        public MoveCursorScriptClient(BattleMapOnlineClient Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new MoveCursorScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendFloat(CursorPositionX);
            WriteBuffer.AppendFloat(CursorPositionY);
        }

        protected override void Execute(IOnlineConnection Host)
        {
            Owner.BattleMapGame.CursorPosition.X = CursorPositionX;
            Owner.BattleMapGame.CursorPosition.Y = CursorPositionY;
        }

        protected override void Read(OnlineReader Sender)
        {
            CursorPositionX = Sender.ReadFloat();
            CursorPositionY = Sender.ReadFloat();
        }
    }
}
