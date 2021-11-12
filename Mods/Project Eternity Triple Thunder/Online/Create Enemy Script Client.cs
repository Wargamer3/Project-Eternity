using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class CreateEnemyScriptClient : OnlineScript, DelayedExecutableOnlineScript
    {
        public const string ScriptName = "Create Enemy";

        private readonly TripleThunderOnlineClient Owner;

        private uint PlayerID;
        private int LayerIndex;
        private int Team;
        private string AIPath;
        private string EnemyPath;
        private Vector2 Position;
        private List<string> ListWeapons;

        public CreateEnemyScriptClient(TripleThunderOnlineClient Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new CreateEnemyScriptClient(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Host)
        {
            Layer ActiveLayer = Owner.TripleThunderGame.ListLayer[LayerIndex];
            ActiveLayer.DelayOnlineScript(this);
        }

        public void ExecuteOnMainThread()
        {
            Layer ActiveLayer = Owner.TripleThunderGame.ListLayer[LayerIndex];

            if (string.IsNullOrEmpty(EnemyPath))
                return;

            List<WeaponBase> ListExtraWeapon = new List<WeaponBase>();
            for (int W = 0; W < ListWeapons.Count; ++W)
            {
                ListExtraWeapon.Add(new ComboWeapon(EnemyPath, ListWeapons[W], true, ActiveLayer.DicRequirement, ActiveLayer.DicEffect, ActiveLayer.DicAutomaticSkillTarget));
            }

            RobotAnimation EnemyRobot = new RobotAnimation(EnemyPath, ActiveLayer, Position, Team, new PlayerInventory(), ActiveLayer.PlayerSFXGenerator, ListExtraWeapon);
            EnemyRobot.ID = PlayerID;

            if (EnemyRobot.Content != null)
            {
                if (!string.IsNullOrEmpty(AIPath))
                {
                    EnemyRobot.RobotAI = new TripleThunderScripAIContainer(new TripleThunderAIInfo(EnemyRobot, ActiveLayer, Owner.TripleThunderGame));
                    EnemyRobot.RobotAI.Load(AIPath);
                }

                ActiveLayer.SpawnRobot(EnemyRobot);
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            PlayerID = Sender.ReadUInt32();
            LayerIndex = Sender.ReadInt32();

            Team = Sender.ReadInt32();
            AIPath = Sender.ReadString();
            EnemyPath = Sender.ReadString();
            Position = new Vector2(Sender.ReadFloat(), Sender.ReadFloat());

            int ListWeaponsCount = Sender.ReadInt32();
            ListWeapons = new List<string>(ListWeaponsCount);
            for (int W = 0; W < ListWeaponsCount; ++W)
            {
                ListWeapons.Add(Sender.ReadString());
            }
        }
    }
}
