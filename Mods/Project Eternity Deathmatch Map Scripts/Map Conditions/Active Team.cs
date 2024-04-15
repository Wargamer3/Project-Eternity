using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActiveTeamCondition : DeathmatchCondition
    {
        private int _ActiveTeam;

        public ActiveTeamCondition()
            : base(140, 70, "Active Team", new string[] { "Check Condition" }, new string[] { "Condition is true", "Condition is false" })
        {
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(ActiveTeam);
        }

        public override void Load(BinaryReader BR)
        {
            ActiveTeam = BR.ReadInt32();
        }

        public override void Update(int Index)
        {
            if (Map.ListPlayer[Map.ActivePlayerIndex].TeamIndex == ActiveTeam)
                Map.ExecuteFollowingScripts(this, 0);
            else
                Map.ExecuteFollowingScripts(this, 1);
        }

        public override MapScript CopyScript()
        {
            return new ActiveTeamCondition();
        }

        #region Properties

        [CategoryAttribute("Condition requirement"),
        DescriptionAttribute("."),
        DefaultValueAttribute(0)]
        public int ActiveTeam
        {
            get
            {
                return _ActiveTeam;
            }
            set
            {
                _ActiveTeam = value;
            }
        }

        #endregion
    }
}
