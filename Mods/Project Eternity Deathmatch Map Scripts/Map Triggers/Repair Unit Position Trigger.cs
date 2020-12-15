using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class RepairUnitPositionTrigger : DeathmatchTrigger
    {
        private System.Drawing.Point _Position;
        private int _RepairValue;

        public RepairUnitPositionTrigger()
            : base(140, 70, "Repair Unit At Position Event", new string[] { "Repair Unit" }, new string[] { "Unit repaired", "Unit not found" })
        {
            Position = new System.Drawing.Point();
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(Position.X);
            BW.Write(Position.Y);
            BW.Write(RepairValue);
        }

        public override void Load(BinaryReader BR)
        {
            Position = new System.Drawing.Point(BR.ReadInt32(), BR.ReadInt32());
            RepairValue = BR.ReadInt32();
        }

        public override void Update(int Index)
        {
            for (int P = Map.ListPlayer.Count - 1; P >= 0; --P)
            {
                for (int S = Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                {
                    if (Map.ListPlayer[P].ListSquad[S].X == Position.X && Map.ListPlayer[P].ListSquad[S].Y == Position.Y)
                    {
                        Map.ListPlayer[P].ListSquad[S].CurrentLeader.HealUnit(RepairValue);
                        Map.ExecuteFollowingScripts(this, 0);
                        return;
                    }
                }
            }
            Map.ExecuteFollowingScripts(this, 1);
        }

        public override MapScript CopyScript()
        {
            return new RepairUnitPositionTrigger();
        }

        #region Properties

        [CategoryAttribute("Trigger values"),
        DescriptionAttribute("."),
        DefaultValueAttribute("")]
        public System.Drawing.Point Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
            }
        }

        [CategoryAttribute("Trigger values"),
        DescriptionAttribute("."),
        DefaultValueAttribute("")]
        public int RepairValue
        {
            get
            {
                return _RepairValue;
            }
            set
            {
                _RepairValue = value;
            }
        }

        #endregion
    }
}
