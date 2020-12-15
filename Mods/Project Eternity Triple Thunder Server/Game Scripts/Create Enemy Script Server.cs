using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderServer
{
    public class CreateEnemyScriptServer : OnlineScript
    {
        public const string ScriptName = "Create Enemy";
        
        private uint PlayerID;
        private int LayerIndex;
        private int Team;
        private string AIPath;
        private string EnemyPath;
        private Vector2 Position;
        private List<string> ListWeapons;

        public CreateEnemyScriptServer(uint PlayerID, int LayerIndex, int Team, string AIPath, string EnemyPath, Vector2 Position, List<string> ListWeapons)
            : base(ScriptName)
        {
            this.PlayerID = PlayerID;
            this.LayerIndex = LayerIndex;
            this.Team = Team;
            this.AIPath = AIPath;
            this.EnemyPath = EnemyPath;
            this.Position = Position;
            this.ListWeapons = ListWeapons;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendUInt32(PlayerID);
            WriteBuffer.AppendInt32(LayerIndex);
            WriteBuffer.AppendInt32(Team);
            WriteBuffer.AppendString(AIPath);
            WriteBuffer.AppendString(EnemyPath);
            WriteBuffer.AppendFloat(Position.X);
            WriteBuffer.AppendFloat(Position.Y);

            WriteBuffer.AppendInt32(ListWeapons.Count);
            for (int W = 0; W < ListWeapons.Count; ++W)
            {
                WriteBuffer.AppendString(ListWeapons[W]);
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
