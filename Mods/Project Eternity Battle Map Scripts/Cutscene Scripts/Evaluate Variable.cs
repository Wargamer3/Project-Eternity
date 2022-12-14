using System;
using System.ComponentModel;
using System.IO;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptEvaluateVariable : BattleMapScript
        {
            private string _Expression;

            public ScriptEvaluateVariable()
                : this(null)
            {
            }

            public ScriptEvaluateVariable(BattleMap Map)
                : base(Map, 100, 50, "Evaluate Variable", new string[] { "Evaluate" }, new string[] { "true", "false"})
            {
            }

            public override void ExecuteTrigger(int Index)
            {
                string ExpressionFinal = Map.ActiveParser.Evaluate(Expression);
                if (ExpressionFinal == "1")
                    ExecuteEvent(this, 0);
                else
                    ExecuteEvent(this, 1);
                IsEnded = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                throw new NotImplementedException();
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                Expression = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(Expression);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptEvaluateVariable(Map);
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
}
