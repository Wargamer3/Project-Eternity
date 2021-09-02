using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class CreateVFXScriptServer : OnlineScript
    {
        public const string ScriptName = "Create VFX";

        private Vector2 Position;
        private Vector2 Speed;
        private byte VFXType;

        public CreateVFXScriptServer()
            : base(ScriptName)
        {
        }

        public override OnlineScript Copy()
        {
            return new CreateVFXScriptServer();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendFloat(Position.X);
            WriteBuffer.AppendFloat(Position.Y);
            WriteBuffer.AppendFloat(Speed.X);
            WriteBuffer.AppendFloat(Speed.Y);
            WriteBuffer.AppendByte(VFXType);
        }

        protected override void Execute(IOnlineConnection Sender)
        {
        }

        protected override void Read(OnlineReader Sender)
        {
            Position = new Vector2(Sender.ReadFloat(), Sender.ReadFloat());
            Speed = new Vector2(Sender.ReadFloat(), Sender.ReadFloat());
            VFXType = Sender.ReadByte();
        }
    }
}
