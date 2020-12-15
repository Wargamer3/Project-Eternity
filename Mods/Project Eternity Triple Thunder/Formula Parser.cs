using System;
using System.Linq;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class FightingZoneFormulaParser : FormulaParser
    {
        private FightingZone Map;

        public FightingZoneFormulaParser(FightingZone Map)
        {
            this.Map = Map;
        }

        protected override string ValueFromVariable(string Input)
        {
            while (Input.Last() == ' ')//Remove the spaces at the end of the buffer.
                Input = Input.Remove(Input.Length - 1, 1);
            while (Input.First() == ' ')//Remove the spaces at the start of the buffer.
                Input = Input.Remove(0, 1);

            string[] Expression = Input.Split('.');

            if (Expression.Length >= 3)
            {
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
                        throw new Exception(Input + " is invalid");
                }
            }
            else if (Expression.Length == 1)
            {
                return Expression[0];
            }
            else
                throw new Exception(Input + " is invalid");
        }
    }
}
