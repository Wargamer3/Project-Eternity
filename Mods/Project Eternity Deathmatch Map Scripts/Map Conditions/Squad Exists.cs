using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class SquadExistsCondition : DeathmatchCondition
    {
        private string _SquadName;

        public SquadExistsCondition()
            : base(140, 70, "Squad Exists", new string[] { "Check Condition" }, new string[] { "Condition is true", "Condition is false" })
        {
            _SquadName = "";
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(SquadName);
        }

        public override void Load(BinaryReader BR)
        {
            SquadName = BR.ReadString();
        }

        public override void Update(int Index)
        {
            string SquadExistsName = SquadName;
            for (int P = Map.ListPlayer.Count - 1; P >= 0; --P)
            {
                for (int S = Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                {
                    if (Map.ListPlayer[P].ListSquad[S].SquadName == SquadExistsName && Map.ListPlayer[P].ListSquad[S].CurrentLeader.HP > 0)
                    {
                        Map.ExecuteFollowingScripts(this, 0);
                        return;
                    }
                }
            }
            Map.ExecuteFollowingScripts(this, 1);
        }

        public override MapScript CopyScript()
        {
            return new SquadExistsCondition();
        }

        #region Properties

        [CategoryAttribute("Condition requirement"),
        DescriptionAttribute("."),
        DefaultValueAttribute("")]
        public string SquadName
        {
            get
            {
                return _SquadName;
            }
            set
            {
                _SquadName = value;
            }
        }

        #endregion
    }
}
