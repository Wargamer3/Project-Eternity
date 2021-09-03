using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class CreateSFXScriptClient : OnlineScript
    {
        public enum SFXTypes { JetpackStart, JetpackLoop, JetpackEnd }

        public const string ScriptName = "Create SFX";

        private readonly TripleThunderOnlineClient Owner;

        private Vector2 Position;
        private SFXTypes SFXType;

        public CreateSFXScriptClient(TripleThunderOnlineClient Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public CreateSFXScriptClient(Vector2 Position, SFXTypes SFXType)
            : base(ScriptName)
        {
            this.Position = Position;
            this.SFXType = SFXType;
        }

        public override OnlineScript Copy()
        {
            return new CreateSFXScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendFloat(Position.X);
            WriteBuffer.AppendFloat(Position.Y);
            WriteBuffer.AppendByte((byte)SFXType);
        }

        protected override void Execute(IOnlineConnection Host)
        {
            switch (SFXType)
            {
                case SFXTypes.JetpackStart:
                    Owner.TripleThunderGame.PlayerSFXGenerator.PlayJetpackStartSound(UnitSounds.JetpackStartSounds.Default);
                    break;
                case SFXTypes.JetpackLoop:
                    Owner.TripleThunderGame.PlayerSFXGenerator.PlayJetpackLoopSound(UnitSounds.JetpackUseSounds.Default);
                    break;
                case SFXTypes.JetpackEnd:
                    Owner.TripleThunderGame.PlayerSFXGenerator.PlayJetpackEndSound(UnitSounds.JetpackEndSounds.Default);
                    break;
            }
        }

        protected override void Read(OnlineReader Host)
        {
            Position = new Vector2(Host.ReadFloat(), Host.ReadFloat());
            SFXType = (SFXTypes)Host.ReadByte();
        }
    }
}
