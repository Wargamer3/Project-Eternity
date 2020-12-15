using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class VSUnitNameCondition : DeathmatchCondition
    {
        private string _UnitName;

        public VSUnitNameCondition()
            : base(140, 70, "VS Unit Name", new string[] { "Check Condition" }, new string[] { "Unit found", "Attacker found", "Defender found", "Nothing found" })
        {
            _UnitName = "";
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(UnitName);
        }

        public override void Load(BinaryReader BR)
        {
            UnitName = BR.ReadString();
        }

        public override void Update(int Index)
        {
            bool VSUnitNameFound = false;
            if (Map.ActiveSquad != null)
            {
                for (int U = Map.ActiveSquad.UnitsAliveInSquad - 1; U >= 0; --U)
                {
                    if (Map.ActiveSquad[U].FullName == UnitName)
                    {
                        Map.ExecuteFollowingScripts(this, 0);
                        Map.ExecuteFollowingScripts(this, 1);

                        VSUnitNameFound = true;
                        break;
                    }
                }
            }
            if (Map.TargetSquad != null)
            {
                for (int U = Map.TargetSquad.UnitsAliveInSquad - 1; U >= 0; --U)
                {
                    if (Map.TargetSquad[U].FullName == UnitName)
                    {
                        if (!VSUnitNameFound)
                            Map.ExecuteFollowingScripts(this, 0);

                        Map.ExecuteFollowingScripts(this, 2);

                        VSUnitNameFound = true;
                        break;
                    }
                }
            }
            if (!VSUnitNameFound)
                Map.ExecuteFollowingScripts(this, 3);
        }

        public override MapScript CopyScript()
        {
            return new VSUnitNameCondition();
        }

        #region Properties

        [CategoryAttribute("Condition requirement"),
        DescriptionAttribute("."),
        DefaultValueAttribute("")]
        public string UnitName
        {
            get
            {
                return _UnitName;
            }
            set
            {
                _UnitName = value;
            }
        }

        #endregion
    }
}
