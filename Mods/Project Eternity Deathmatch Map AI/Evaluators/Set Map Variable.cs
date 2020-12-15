using System;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class SetMapVariable : DeathmatchScript, ScriptEvaluator
        {
            private string _VariableName;
            private Operators.SignOperators _SignOperator;

            public SetMapVariable()
                : base(100, 50, "Set Map Variable", new string[0], new string[0])
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                string VariableNameLower = VariableName.ToLower();
                double Value = (double)Input;

                if (!BattleMap.DicMapVariables.ContainsKey(VariableNameLower))
                {
                    if (SignOperator == Operators.SignOperators.Equal)
                        BattleMap.DicMapVariables.Add(VariableNameLower, Value);
                    else
                        throw new Exception(SignOperator.ToString() + " can't be used on " + VariableNameLower + " as this variable is unassigned.");
                }
                else
                {
                    switch (SignOperator)
                    {
                        case Operators.SignOperators.Equal:
                            BattleMap.DicMapVariables[VariableNameLower] = Value;
                            break;

                        case Operators.SignOperators.PlusEqual:
                            BattleMap.DicMapVariables[VariableNameLower] += Value;
                            break;

                        case Operators.SignOperators.MinusEqual:
                            BattleMap.DicMapVariables[VariableNameLower] -= Value;
                            break;

                        case Operators.SignOperators.MultiplicatedEqual:
                            BattleMap.DicMapVariables[VariableNameLower] *= Value;
                            break;

                        case Operators.SignOperators.DividedEqual:
                            BattleMap.DicMapVariables[VariableNameLower] /= Value;
                            break;

                        case Operators.SignOperators.ModuloEqual:
                            BattleMap.DicMapVariables[VariableNameLower] %= Value;
                            break;
                    }
                }

                Result = new List<object>();
                IsCompleted = true;
            }
            
            public override AIScript CopyScript()
            {
                return new SetMapVariable();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public string VariableName
            {
                get
                {
                    return _VariableName;
                }
                set
                {
                    _VariableName = value;
                }
            }

            [TypeConverter(typeof(Core.Item.SignOperatorConverter)),
            CategoryAttribute(""),
            DescriptionAttribute(".")]
            public Operators.SignOperators SignOperator
            {
                get { return _SignOperator; }
                set { _SignOperator = value; }
            }
        }
    }
}
