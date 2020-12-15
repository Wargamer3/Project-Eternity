using System.IO;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class CurrentTurnCondition : BattleCondition
    {
        private Operators.LogicOperators _LogicOperator;
        private int _Turn;

        public CurrentTurnCondition()
            : this(null)
        {
        }

        public CurrentTurnCondition(BattleMap Map)
            : base(Map, 140, 70, "Current Turn", new string[] { "Check Condition" }, new string[] { "Condition is true", "Condition is false" })
        {
            _Turn = 0;
            _LogicOperator = Operators.LogicOperators.Equal;
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write((byte)LogicOperator);
            BW.Write(Turn);
        }

        public override void Load(BinaryReader BR)
        {
            LogicOperator = (Operators.LogicOperators)BR.ReadByte();
            Turn = BR.ReadInt32();
        }

        public override void Update(int Index)
        {
            bool CurrentTurnIsValid = Operators.CompareValue(LogicOperator, Map.GameTurn, Turn);

            if (CurrentTurnIsValid)
                Map.ExecuteFollowingScripts(this, 0);
            else
                Map.ExecuteFollowingScripts(this, 1);
        }

        public override MapScript CopyScript()
        {
            return new CurrentTurnCondition(Map);
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
        public int Turn
        {
            get
            {
                return _Turn;
            }
            set
            {
                _Turn = value;
            }
        }

        #endregion
    }
}
