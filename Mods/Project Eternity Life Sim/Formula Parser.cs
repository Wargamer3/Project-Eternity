using System;
using System.Linq;
using System.Collections.Generic;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class LifeSimFormulaParser : FormulaParser
    {
        private LifeSimMap Map;
        private LifeSimBattleContext BattleContext;

        public LifeSimFormulaParser(LifeSimMap Map, LifeSimBattleContext BattleContext)
        {
            this.Map = Map;
            this.BattleContext = BattleContext;
        }

        protected override string ValueFromVariable(string Input)
        {
            while (Input.Last() == ' ')//Remove the spaces at the end of the buffer.
                Input = Input.Remove(Input.Length - 1, 1);
            while (Input.First() == ' ')//Remove the spaces at the start of the buffer.
                Input = Input.Remove(0, 1);

            string[] Expression = Input.Split('.');
            string ReturnExpression = null;

            if (Expression.Length >= 3)
            {
                if (ReturnExpression != null)
                    return ReturnExpression;
                else
                    throw new Exception(Input + " is invalid");
            }
            else if (Expression.Length == 2)
            {
                switch (Expression[0])
                {
                    case "map":
                    case "game":
                        switch (Expression[1])
                        {

                            default:
                                return Map.DicMapVariables[Expression[1]].ToString();
                        }

                    default:
                        if (Expression[0].Contains("player"))
                        {
                            string[] ArrayPlayerValue = Expression[0].Split(new[] { "player" }, StringSplitOptions.RemoveEmptyEntries);
                            if (ArrayPlayerValue.Length == 1)
                            {
                            }
                        }
                        throw new Exception(Input + " is invalid");
                }
            }
            else if (Expression.Length == 1)
            {
                switch (Expression[0])
                {
                }

                return Expression[0];
            }
            else
                throw new Exception(Input + " is invalid");
        }
    }
}
