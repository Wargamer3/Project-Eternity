using System.IO;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class RemainingRespawnCondition : BattleCondition
    {
        private Operators.LogicOperators _LogicOperator;
        private byte _PlayerIndex;
        private int _RemainingSpawn;

        public RemainingRespawnCondition()
            : this(null)
        {
        }

        public RemainingRespawnCondition(BattleMap Map)
            : base(Map, 140, 70, "Current Turn", new string[] { "Check Condition" }, new string[] { "Condition is true", "Condition is false" })
        {
            _PlayerIndex = 1;
            _RemainingSpawn = 0;
            _LogicOperator = Operators.LogicOperators.Equal;
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write((byte)LogicOperator);
            BW.Write(_PlayerIndex);
            BW.Write(_RemainingSpawn);
        }

        public override void Load(BinaryReader BR)
        {
            LogicOperator = (Operators.LogicOperators)BR.ReadByte();
            _PlayerIndex = BR.ReadByte();
            _RemainingSpawn = BR.ReadInt32();
        }

        public override void Update(int Index)
        {
            bool CurrentTurnIsValid = Operators.CompareValue(LogicOperator, Map.GameRule.GetRemainingResapwn(_PlayerIndex - 1), _RemainingSpawn);

            if (CurrentTurnIsValid)
                Map.ExecuteFollowingScripts(this, 0);
            else
                Map.ExecuteFollowingScripts(this, 1);
        }

        public override MapScript CopyScript()
        {
            return new RemainingRespawnCondition(Map);
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
        DescriptionAttribute("Starts at 1."),
        DefaultValueAttribute(1)]
        public byte PlayerIndex
        {
            get
            {
                return _PlayerIndex;
            }
            set
            {
                _PlayerIndex = value;
            }
        }

        [CategoryAttribute("Condition requirement"),
        DescriptionAttribute("."),
        DefaultValueAttribute(1)]
        public int RemainingSpawn
        {
            get
            {
                return _RemainingSpawn;
            }
            set
            {
                _RemainingSpawn = value;
            }
        }

        #endregion
    }
}
