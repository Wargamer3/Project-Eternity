using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class VSSquadNameCondition : DeathmatchCondition
    {
        private string _SquadName;

        public VSSquadNameCondition()
            : base(140, 70, "VS Squad Name", new string[] { "Check Condition" }, new string[] { "Squad found", "Attacker found", "Defender found", "Nothing found" })
        {
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
            bool VSSquadNameFound = false;
            if (Map.ActiveSquad != null && Map.ActiveSquad.SquadName == SquadName)
            {
                Map.ExecuteFollowingScripts(this, 0);
                Map.ExecuteFollowingScripts(this, 1);

                VSSquadNameFound = true;
            }
            else if (Map.TargetSquad != null && Map.TargetSquad.SquadName == SquadName)
            {
                if (!VSSquadNameFound)
                    Map.ExecuteFollowingScripts(this, 0);

                Map.ExecuteFollowingScripts(this, 1);

                VSSquadNameFound = true;
            }
            else if (!VSSquadNameFound)
                Map.ExecuteFollowingScripts(this, 3);
        }

        public override MapScript CopyScript()
        {
            return new VSSquadNameCondition();
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
