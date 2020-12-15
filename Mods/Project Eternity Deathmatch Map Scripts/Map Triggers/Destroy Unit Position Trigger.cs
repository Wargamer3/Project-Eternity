using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class DestroyUnitPositionTrigger : DeathmatchTrigger
    {
        private System.Drawing.Point _Position;

        public DestroyUnitPositionTrigger()
            : base(140, 70, "Destroy Unit At Position Event", new string[] { "Destroy Unit" }, new string[] { "Unit destroyed", "Unit not found" })
        {
            Position = new System.Drawing.Point();
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(Position.X);
            BW.Write(Position.Y);
        }

        public override void Load(BinaryReader BR)
        {
            Position = new System.Drawing.Point(BR.ReadInt32(), BR.ReadInt32());
        }

        public override void Update(int Index)
        {
            for (int P = Map.ListPlayer.Count - 1; P >= 0; --P)
            {
                for (int S = Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                {
                    if (Map.ListPlayer[P].ListSquad[S].X == Position.X && Map.ListPlayer[P].ListSquad[S].Y == Position.Y)
                    {
                        Map.ListPlayer[P].ListSquad[S].CurrentLeader.KillUnit();
                        Map.ExecuteFollowingScripts(this, 0);
                        return;
                    }
                }
            }
            Map.ExecuteFollowingScripts(this, 1);
        }

        public override MapScript CopyScript()
        {
            return new DestroyUnitPositionTrigger();
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

        #endregion
    }
}
