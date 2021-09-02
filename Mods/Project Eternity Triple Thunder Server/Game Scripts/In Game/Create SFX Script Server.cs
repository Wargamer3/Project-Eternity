using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class CreateSFXScriptServer : OnlineScript
    {
        public const string ScriptName = "Create SFX";

        private Vector2 Position;
        private byte SFXType;

        public CreateSFXScriptServer()
            : base(ScriptName)
        {
        }

        public override OnlineScript Copy()
        {
            return new CreateSFXScriptServer();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendFloat(Position.X);
            WriteBuffer.AppendFloat(Position.Y);
            WriteBuffer.AppendByte(SFXType);
        }

        protected override void Execute(IOnlineConnection Sender)
        {
        }

        protected override void Read(OnlineReader Sender)
        {
            Position = new Vector2(Sender.ReadFloat(), Sender.ReadFloat());
            SFXType = Sender.ReadByte();
        }
    }
}
