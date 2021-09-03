using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class CreateSFXScriptServer : OnlineScript
    {
        public const string ScriptName = "Create SFX";

        private readonly TripleThunderClientGroup ActiveGroup;

        private Vector2 Position;
        private byte SFXType;

        public CreateSFXScriptServer(TripleThunderClientGroup ActiveGroup)
            : base(ScriptName)
        {
            this.ActiveGroup = ActiveGroup;
        }

        public CreateSFXScriptServer(Vector2 Position, byte SFXType)
            : base(ScriptName)
        {
            this.Position = Position;
            this.SFXType = SFXType;
        }

        public override OnlineScript Copy()
        {
            return new CreateSFXScriptServer(ActiveGroup);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendFloat(Position.X);
            WriteBuffer.AppendFloat(Position.Y);
            WriteBuffer.AppendByte(SFXType);
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            foreach (IOnlineConnection ActiveOnlinePlayer in ActiveGroup.Room.ListOnlinePlayer)
            {
                if (ActiveOnlinePlayer != Sender)
                {
                    ActiveOnlinePlayer.Send(new CreateSFXScriptServer(Position, SFXType));
                }
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            Position = new Vector2(Sender.ReadFloat(), Sender.ReadFloat());
            SFXType = Sender.ReadByte();
        }
    }
}
