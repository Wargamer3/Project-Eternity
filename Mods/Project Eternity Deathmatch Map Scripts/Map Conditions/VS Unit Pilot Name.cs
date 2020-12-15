using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class VSUnitPilotNameCondition : DeathmatchCondition
    {
        private string _PilotName;

        public VSUnitPilotNameCondition()
            : base(140, 70, "VS Unit Pilot Name", new string[] { "Check Condition" }, new string[] { "Unit found", "Attacker found", "Defender found", "Nothing found" })
        {
            _PilotName = "";
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(PilotName);
        }

        public override void Load(BinaryReader BR)
        {
            PilotName = BR.ReadString();
        }

        public override void Update(int Index)
        {
            bool VSUnitPilotNameFound = false;
            if (Map.ActiveSquad != null)
            {
                for (int U = Map.ActiveSquad.UnitsAliveInSquad - 1; U >= 0; --U)
                {
                    if (Map.ActiveSquad[U].PilotName == PilotName)
                    {
                        Map.ExecuteFollowingScripts(this, 0);
                        Map.ExecuteFollowingScripts(this, 1);

                        VSUnitPilotNameFound = true;
                        break;
                    }
                }
            }
            if (Map.TargetSquad != null)
            {
                for (int U = Map.TargetSquad.UnitsAliveInSquad - 1; U >= 0; --U)
                {
                    if (Map.TargetSquad[U].PilotName == PilotName)
                    {
                        if (!VSUnitPilotNameFound)
                            Map.ExecuteFollowingScripts(this, 0);

                        Map.ExecuteFollowingScripts(this, 2);

                        VSUnitPilotNameFound = true;
                        break;
                    }
                }
            }
            if (!VSUnitPilotNameFound)
                Map.ExecuteFollowingScripts(this, 3);
        }

        public override MapScript CopyScript()
        {
            return new VSUnitPilotNameCondition();
        }

        #region Properties

        [CategoryAttribute("Condition requirement"),
        DescriptionAttribute("."),
        DefaultValueAttribute("")]
        public string PilotName
        {
            get
            {
                return _PilotName;
            }
            set
            {
                _PilotName = value;
            }
        }

        #endregion
    }
}
