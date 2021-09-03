using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class CreateVFXScriptServer : OnlineScript
    {
        public const string ScriptName = "Create VFX";

        private readonly TripleThunderClientGroup ActiveGroup;

        private Vector2 Position;
        private Vector2 Speed;
        private byte VFXType;

        public CreateVFXScriptServer(TripleThunderClientGroup ActiveGroup)
            : base(ScriptName)
        {
            this.ActiveGroup = ActiveGroup;
        }

        public CreateVFXScriptServer(Vector2 Position, Vector2 Speed, byte VFXType)
            : base(ScriptName)
        {
            this.Position = Position;
            this.Speed = Speed;
            this.VFXType = VFXType;
        }

        public override OnlineScript Copy()
        {
            return new CreateVFXScriptServer(ActiveGroup);
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
            foreach (IOnlineConnection ActiveOnlinePlayer in ActiveGroup.Room.ListOnlinePlayer)
            {
                if (ActiveOnlinePlayer != Sender)
                {
                    ActiveOnlinePlayer.Send(new CreateVFXScriptServer(Position, Speed, VFXType));
                }
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            Position = new Vector2(Sender.ReadFloat(), Sender.ReadFloat());
            Speed = new Vector2(Sender.ReadFloat(), Sender.ReadFloat());
            VFXType = Sender.ReadByte();
        }
    }
}
