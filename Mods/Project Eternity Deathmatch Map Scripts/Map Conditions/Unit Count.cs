using System.IO;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class UnitCountCondition : DeathmatchCondition
    {
        private Operators.LogicOperators _LogicOperator;
        private int _Count;
        private string _UnitName;

        public UnitCountCondition()
            : base(140, 70, "Unit Count", new string[] { "Check Condition" }, new string[] { "Condition is true", "Condition is false" })
        {
            _UnitName = "";
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write((byte)LogicOperator);
            BW.Write(Count);
            BW.Write(UnitName);
        }

        public override void Load(BinaryReader BR)
        {
            LogicOperator = (Operators.LogicOperators)BR.ReadByte();
            Count = BR.ReadInt32();
            UnitName = BR.ReadString();
        }

        public override void Update(int Index)
        {
            int UnitCount = 0;
            for (int P = Map.ListPlayer.Count - 1; P >= 0; --P)
            {
                for (int S = Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                {
                    for (int U = Map.ListPlayer[P].ListSquad[S].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        if (Map.ListPlayer[P].ListSquad[S][U].RelativePath == UnitName && Map.ListPlayer[P].ListSquad[S][U].HP > 0)
                            ++UnitCount;
                    }
                }
            }

            bool UnitCountIsValid = Operators.CompareValue(LogicOperator, UnitCount, Count);

            if (UnitCountIsValid)
                Map.ExecuteFollowingScripts(this, 0);
            else
                Map.ExecuteFollowingScripts(this, 1);
        }

        public override MapScript CopyScript()
        {
            return new UnitCountCondition();
        }

        #region Properties

        [TypeConverter(typeof(LogicOperatorConverter)),
        CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public Operators.LogicOperators LogicOperator
        {
            get { return _LogicOperator; }
            set { _LogicOperator = value; }
        }

        [CategoryAttribute("Condition requirement"),
        DescriptionAttribute("."),
        DefaultValueAttribute(1)]
        public int Count
        {
            get
            {
                return _Count;
            }
            set
            {
                _Count = value;
            }
        }

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
