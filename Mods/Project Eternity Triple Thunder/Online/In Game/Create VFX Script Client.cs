using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class CreateVFXScriptClient : OnlineScript
    {
        public enum VFXTypes { Jetpack }

        public const string ScriptName = "Create VFX";

        private readonly TripleThunderOnlineClient Owner;

        private Vector2 Position;
        private Vector2 Speed;
        private VFXTypes VFXType;

        public CreateVFXScriptClient(TripleThunderOnlineClient Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public CreateVFXScriptClient(Vector2 Position, Vector2 Speed, VFXTypes VFXType)
            : base(ScriptName)
        {
            this.Position = Position;
            this.Speed = Speed;
            this.VFXType = VFXType;
        }

        public override OnlineScript Copy()
        {
            return new CreateVFXScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendFloat(Position.X);
            WriteBuffer.AppendFloat(Position.Y);
            WriteBuffer.AppendFloat(Speed.X);
            WriteBuffer.AppendFloat(Speed.Y);
            WriteBuffer.AppendByte((byte)VFXType);
        }

        protected override void Execute(IOnlineConnection Host)
        {
            Propulsor.ParticleSystem.AddParticle(Position, Speed);
        }

        protected override void Read(OnlineReader Host)
        {
            Position = new Vector2(Host.ReadFloat(), Host.ReadFloat());
            Speed = new Vector2(Host.ReadFloat(), Host.ReadFloat());
            VFXType = (VFXTypes)Host.ReadByte();
        }
    }
}
