using System.IO;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class ExpressionCondition : BattleCondition
    {
        private string _Expression;

        public ExpressionCondition()
            : this(null)
        {
        }

        public ExpressionCondition(BattleMap Map)
            : base(Map, 140, 70, "Expression", new string[] { "Check Expression" }, new string[] { "Expression is true", "Expression is false" })
        {
            _Expression = "";
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(Expression);
        }

        public override void Load(BinaryReader BR)
        {
            Expression = BR.ReadString();
        }

        public override void Update(int Index)
        {
            string ExpressionFinal = Map.Params.ActiveParser.Evaluate(Expression);
            if (ExpressionFinal == "1")
                Map.ExecuteFollowingScripts(this, 0);
            else
                Map.ExecuteFollowingScripts(this, 1);
        }

        public override MapScript CopyScript()
        {
            return new ExpressionCondition(Map);
        }

        #region Properties

        [CategoryAttribute("Condition requirement"),
        DescriptionAttribute("."),
        DefaultValueAttribute("")]
        public string Expression
        {
            get
            {
                return _Expression;
            }
            set
            {
                _Expression = value;
            }
        }

        #endregion
    }
}
