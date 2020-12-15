using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class UnitExistsCondition : DeathmatchCondition
    {
        private string _UnitName;

        public UnitExistsCondition()
            : base(140, 70, "Unit Exists", new string[] { "Check Condition" }, new string[] { "Condition is true", "Condition is false" })
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
            string UnitExistsName = UnitName;
            for (int P = Map.ListPlayer.Count - 1; P >= 0; --P)
            {
                for (int S = Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                {
                    for (int U = Map.ListPlayer[P].ListSquad[S].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        if (Map.ListPlayer[P].ListSquad[S][U].FullName == UnitExistsName && Map.ListPlayer[P].ListSquad[S][U].HP > 0)
                        {
                            Map.ExecuteFollowingScripts(this, 0);
                            return;
                        }
                    }
                }
            }
            Map.ExecuteFollowingScripts(this, 1);
        }

        public override MapScript CopyScript()
        {
            return new UnitExistsCondition();
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
