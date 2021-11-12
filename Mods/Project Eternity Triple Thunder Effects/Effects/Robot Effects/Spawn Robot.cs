using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Drawing.Design;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class SpawnRobotEffect : TripleThunderRobotEffect
    {
        public static string Name = "Spawn Robot";

        private Vector2 _Offset;
        private string _AIPath;
        private string _RobotPath;
        private int _Team;
        private List<string> _ListWeapons;

        public SpawnRobotEffect()
            : base(Name, false)
        {
            _Team = 0;
            _AIPath = string.Empty;
            _RobotPath = string.Empty;
            _ListWeapons = new List<string>();
        }

        public SpawnRobotEffect(TripleThunderRobotParams Params)
            : base(Name, false, Params)
        {
            _Team = 0;
            _AIPath = string.Empty;
            _RobotPath = string.Empty;
            _ListWeapons = new List<string>();
        }

        protected override void Load(BinaryReader BR)
        {
            _Team = BR.ReadInt32();
            _AIPath = BR.ReadString();
            _RobotPath = BR.ReadString();
            _Offset = new Vector2(BR.ReadSingle(), BR.ReadSingle());

            int ListWeaponsCount = BR.ReadInt32();
            _ListWeapons = new List<string>(ListWeaponsCount);
            for (int W = 0; W < ListWeaponsCount; ++W)
                _ListWeapons.Add(BR.ReadString());
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_Team);
            BW.Write(_AIPath);
            BW.Write(_RobotPath);
            BW.Write(_Offset.X);
            BW.Write(_Offset.Y);

            BW.Write(_ListWeapons.Count);
            for (int W = 0; W < _ListWeapons.Count; ++W)
                BW.Write(_ListWeapons[W]);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            if (string.IsNullOrEmpty(_RobotPath))
                return null;

            Layer Owner = Params.LocalContext.ActiveLayer;

            List<ComboWeapon> ListExtraWeapon = new List<ComboWeapon>();
            for (int W = 0; W < _ListWeapons.Count; ++W)
            {
                ListExtraWeapon.Add(new ComboWeapon(_RobotPath, _ListWeapons[W], true, Owner.DicRequirement, Owner.DicEffect, Owner.DicAutomaticSkillTarget));
            }

            //TODO: Create the robot on load to avoid loading assets when this effect is called.
            RobotAnimation NewRobot = new RobotAnimation(_RobotPath, Owner, Params.LocalContext.Target.Position + _Offset, _Team, new PlayerInventory(), Owner.PlayerSFXGenerator, ListExtraWeapon);

            if (!string.IsNullOrEmpty(AIPath))
            {
                NewRobot.RobotAI = new TripleThunderScripAIContainer(new TripleThunderAIInfo(NewRobot, Owner, Params.LocalContext.Map));
                NewRobot.RobotAI.Load(AIPath);
            }

            Owner.ListRobotToAdd.Add(NewRobot);

            return null;
        }

        protected override BaseEffect DoCopy()
        {
            SpawnRobotEffect NewEffect = new SpawnRobotEffect(Params);

            NewEffect._Offset = _Offset;
            NewEffect._AIPath = _AIPath;
            NewEffect._RobotPath = _RobotPath;
            NewEffect._Team = _Team;
            NewEffect._ListWeapons = new List<string>(_ListWeapons);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            SpawnRobotEffect NewEffect = (SpawnRobotEffect)Copy;

            _Offset = NewEffect._Offset;
            _AIPath = NewEffect._AIPath;
            _RobotPath = NewEffect._RobotPath;
            _Team = NewEffect._Team;
            _ListWeapons = new List<string>(NewEffect._ListWeapons);
        }

        #region Properties

        [CategoryAttribute("Prop attributes"),
        DescriptionAttribute("The spawning position of the Robot"),
        DefaultValueAttribute("")]
        public Vector2 Offset { get { return _Offset; } set { _Offset = value; } }

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
