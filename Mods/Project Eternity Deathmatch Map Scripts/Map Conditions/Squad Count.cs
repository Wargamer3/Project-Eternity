using System.IO;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class SquadCountCondition : DeathmatchCondition
    {
        private Operators.LogicOperators _LogicOperator;
        private int _Count;
        private string _SquadName;

        public SquadCountCondition()
            : base(140, 70, "Squad Count", new string[] { "Check Condition" }, new string[] { "Condition is true", "Condition is false" })
        {
            _SquadName = "";
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write((byte)LogicOperator);
            BW.Write(Count);
            BW.Write(SquadName);
        }

        public override void Load(BinaryReader BR)
        {
            LogicOperator = (Operators.LogicOperators)BR.ReadByte();
            Count = BR.ReadInt32();
            SquadName = BR.ReadString();
        }

        public override void Update(int Index)
        {
            int SquadCount = 0;
            for (int P = Map.ListPlayer.Count - 1; P >= 0; --P)
            {
                for (int S = Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                {
                    if (Map.ListPlayer[P].ListSquad[S].SquadName == SquadName && Map.ListPlayer[P].ListSquad[S].CurrentLeader.HP > 0)
                        ++SquadCount;
                }
            }

            bool SquadCountIsValid = Operators.CompareValue(LogicOperator, SquadCount, Count);

            if (SquadCountIsValid)
                Map.ExecuteFollowingScripts(this, 0);
            else
                Map.ExecuteFollowingScripts(this, 1);
        }

        public override MapScript CopyScript()
        {
            return new SquadCountCondition();
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
