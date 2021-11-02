using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using ProjectEternity.Core.Scripts;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class SpawnRobotTrigger : FightingZoneTrigger
    {
        private int _LayerIndex;
        private int _Team;
        private Vector2 _Position;
        private string _AIPath;
        private string _RobotPath;
        private List<string> _ListWeapons;

        public SpawnRobotTrigger()
            : this(null)
        {
        }

        public SpawnRobotTrigger(FightingZone Map)
            : base(Map, 140, 70, "Spawn Robot", new string[] { "Spawn Robot" }, new string[] { })
        {
            _LayerIndex = 0;
            _Team = 0;
            _AIPath = string.Empty;
            _RobotPath = string.Empty;
            _ListWeapons = new List<string>();
        }

        public override void Load(BinaryReader BR)
        {
            _LayerIndex = BR.ReadInt32();
            _Team = BR.ReadInt32();
            _AIPath = BR.ReadString();
            _RobotPath = BR.ReadString();
            _Position = new Vector2(BR.ReadSingle(), BR.ReadSingle());

            int ListWeaponsCount = BR.ReadInt32();
            _ListWeapons = new List<string>(ListWeaponsCount);
            for (int W = 0; W < ListWeaponsCount; ++W)
                _ListWeapons.Add(BR.ReadString());
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(_LayerIndex);
            BW.Write(_Team);
            BW.Write(_AIPath);
            BW.Write(_RobotPath);
            BW.Write(_Position.X);
            BW.Write(_Position.Y);

            BW.Write(_ListWeapons.Count);
            for (int W = 0; W < _ListWeapons.Count; ++W)
                BW.Write(_ListWeapons[W]);
        }

        public override void Update(int Index)
        {
            if (string.IsNullOrEmpty(_RobotPath))
                return;

            Layer Owner = Map.ListLayer[_LayerIndex];

            List<Weapon> ListExtraWeapon = new List<Weapon>();
            for (int W = 0; W < _ListWeapons.Count; ++W)
            {
                ListExtraWeapon.Add(new Weapon(_ListWeapons[W], Owner.DicRequirement, Owner.DicEffect, Owner.DicAutomaticSkillTarget));
            }

            RobotAnimation NewRobot = new RobotAnimation(_RobotPath, Owner, _Position, _Team, new PlayerInventory(), Owner.PlayerSFXGenerator, ListExtraWeapon);

            if (!string.IsNullOrEmpty(AIPath))
            {
                NewRobot.RobotAI = new TripleThunderScripAIContainer(new TripleThunderAIInfo(NewRobot, Owner, Map));
                NewRobot.RobotAI.Load(AIPath);
            }

            Owner.ListRobotToAdd.Add(NewRobot);
        }

        public override MapScript CopyScript()
        {
            SpawnRobotTrigger NewEffect = new SpawnRobotTrigger(Map);

            NewEffect._LayerIndex = _LayerIndex;
            NewEffect._Position = _Position;
            NewEffect._Team = _Team;
            NewEffect._AIPath = _AIPath;
            NewEffect._RobotPath = _RobotPath;
            NewEffect._ListWeapons = new List<string>(_ListWeapons);

            return NewEffect;
        }

        #region Properties

        [CategoryAttribute("Prop attributes"),
        DescriptionAttribute("Layer Index"),
        DefaultValueAttribute("")]
        public int LayerIndex { get { return _LayerIndex; } set { _LayerIndex = value; } }

        [CategoryAttribute("Prop attributes"),
        DescriptionAttribute("The spawning position of the Robot"),
        DefaultValueAttribute("")]
        public Vector2 Position { get { return _Position; } set { _Position = value; } }

        [CategoryAttribute("Prop attributes"),
        DescriptionAttribute("The Team of the Robot"),
        DefaultValueAttribute("")]
        public int Team { get { return _Team; } set { _Team = value; } }

        [Editor(typeof(Core.AI.Selectors.AISelector), typeof(UITypeEditor)),
        CategoryAttribute("Prop attributes"),
        DescriptionAttribute("The AI path"),
        DefaultValueAttribute("")]
        public string AIPath { get { return _AIPath; } set { _AIPath = value; } }

        [Editor(typeof(RobotSelector), typeof(UITypeEditor)),
        CategoryAttribute("Prop attributes"),
        DescriptionAttribute("The Robot path"),
        DefaultValueAttribute("")]
        public string RobotPath { get { return _RobotPath; } set { _RobotPath = value; } }

        [Editor(typeof(WeaponSelector), typeof(UITypeEditor)),
        CategoryAttribute("Prop attributes"),
        DescriptionAttribute("All the Weapons used by the enemy"),
        DefaultValueAttribute("")]
        public List<string> ListWeapons { get { return _ListWeapons; } set { _ListWeapons = value; } }


        #endregion
    }
}
