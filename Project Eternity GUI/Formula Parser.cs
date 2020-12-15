using System;
using System.Linq;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GUI
{
    public class GUIFormulaParser : FormulaParser
    {
        public Unit ActiveUnit;
        public Squad ActiveSquad;

        public GUIFormulaParser()
        {
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
                switch (Expression[0])
                {
                    case "squad":
                        ReturnExpression = SquadValueFromVariable(Expression);
                        break;

                    case "unit":
                        ReturnExpression = UnitValueFromVariable(Expression);
                        break;
                }

                if (ReturnExpression != null)
                    return ReturnExpression;
                else
                    throw new Exception(Input + " is invalid");
            }
            else if (Expression.Length == 2)
            {
                return "0";
            }
            else if (Expression.Length == 1)
            {
                return Expression[0];
            }
            else
                throw new Exception(Input + " is invalid");
        }

        public string UnitValueFromVariable(string[] Expression)
        {
            string ReturnExpression = null;
            Squad ActiveSquad = null;
            Unit ActiveUnit = null;

            if (ActiveSquad == null)
                return ReturnExpression;

            if (Expression.Length >= 4)
            {
                ReturnExpression = UnitStatFromUnit(ActiveUnit, Expression[3]);
            }

            return ReturnExpression;
        }

        private string UnitStatFromUnit(Unit ActiveUnit, string Expression)
        {
            string ReturnExpression = null;

            switch (Expression)
            {
                case "name":
                case "leadername":
                case "currentleadername":
                    ReturnExpression = ActiveUnit.FullName;
                    break;

                case "hp":
                case "hitpoint":
                case "hitpoints":
                    ReturnExpression = ActiveUnit.HP.ToString();
                    break;

                case "maxhp":
                case "hpmax":
                case "maxhitpoint":
                case "maxhitpoints":
                case "hitpointmax":
                case "hitpointsmax":
                    ReturnExpression = ActiveUnit.MaxHP.ToString();
                    break;

                case "en":
                case "energy":
                    ReturnExpression = ActiveUnit.EN.ToString();
                    break;

                case "maxen":
                case "enmax":
                case "maxenergy":
                case "energymax":
                    ReturnExpression = ActiveUnit.MaxEN.ToString();
                    break;

                case "armor":
                    ReturnExpression = ActiveUnit.Armor.ToString();
                    break;

                case "mobility":
                    ReturnExpression = ActiveUnit.Mobility.ToString();
                    break;

                case "maxmv":
                case "mv":
                case "maxmovement":
                case "movement":
                    ReturnExpression = ActiveUnit.MaxMovement.ToString();
                    break;

                case "lv":
                case "lvl":
                case "level":
                    ReturnExpression = ActiveUnit.Pilot.Level.ToString();
                    break;

                case "mel":
                case "melee":
                    ReturnExpression = ActiveUnit.Pilot.MEL.ToString();
                    break;

                case "rng":
                case "range":
                    ReturnExpression = ActiveUnit.Pilot.RNG.ToString();
                    break;

                case "def":
                case "defense":
                    ReturnExpression = ActiveUnit.Pilot.DEF.ToString();
                    break;

                case "skl":
                case "skill":
                    ReturnExpression = ActiveUnit.Pilot.SKL.ToString();
                    break;

                case "eva":
                case "evasion":
                    ReturnExpression = ActiveUnit.Pilot.EVA.ToString();
                    break;

                case "hit":
                    ReturnExpression = ActiveUnit.Pilot.HIT.ToString();
                    break;
            }

            return ReturnExpression;
        }

        public string SquadValueFromVariable(string[] Expression)
        {
            string ReturnExpression = null;

            if (ActiveSquad == null)
                return ReturnExpression;

            if (Expression.Length >= 4)
            {
                #region Squad variables

                switch (Expression[3])
                {
                    case "leader":
                    case "currentleader":
                        ActiveUnit = ActiveSquad.CurrentLeader;
                        break;

                    case "wingmana":
                    case "currentwingmana":
                    case "wingman1":
                    case "currentwingman1":
                        ActiveUnit = ActiveSquad.CurrentWingmanA;
                        break;

                    case "wingmanb":
                    case "currentwingmanb":
                    case "wingman2":
                    case "currentwingman2":
                        ActiveUnit = ActiveSquad.CurrentWingmanA;
                        break;

                    case "position":
                    case "location":
                        ReturnExpression = "[" + ActiveSquad.X + "," + ActiveSquad.Y + "]";
                        break;

                    case "name":
                    case "squadname":
                        ReturnExpression = ActiveSquad.SquadName;
                        break;

                    case "leadername":
                    case "currentleadername":
                        ReturnExpression = ActiveSquad.CurrentLeader.FullName;
                        break;

                    case "wingmananame":
                    case "currentwingmananame":
                    case "wingman1name":
                    case "currentwingman1name":
                        ReturnExpression = ActiveSquad.CurrentWingmanA.FullName;
                        break;

                    case "wingmanbname":
                    case "currentwingmanbname":
                    case "wingman2name":
                    case "currentwingman2name":
                        ReturnExpression = ActiveSquad.CurrentWingmanB.FullName;
                        break;

                    case "id":
                        ReturnExpression = ActiveSquad.ID.ToString();
                        break;

                    case "hp":
                    case "hitpoint":
                    case "hitpoints":
                        ReturnExpression = ActiveUnit.HP.ToString();
                        break;

                    case "maxhp":
                    case "hpmax":
                    case "maxhitpoint":
                    case "maxhitpoints":
                    case "hitpointmax":
                    case "hitpointsmax":
                        ReturnExpression = ActiveUnit.MaxHP.ToString();
                        break;

                    case "en":
                    case "energy":
                        ReturnExpression = ActiveUnit.EN.ToString();
                        break;

                    case "maxen":
                    case "enmax":
                    case "maxenergy":
                    case "energymax":
                        ReturnExpression = ActiveUnit.MaxEN.ToString();
                        break;
                }

                #endregion
            }

            if (Expression.Length >= 5)
            {
                if (ActiveUnit == null)
                    return ReturnExpression;

                #region Unit variables

                switch (Expression[4])
                {
                    case "hp":
                    case "hitpoint":
                    case "hitpoints":
                        ReturnExpression = ActiveUnit.HP.ToString();
                        break;

                    case "maxhp":
                    case "hpmax":
                    case "maxhitpoint":
                    case "maxhitpoints":
                    case "hitpointmax":
                    case "hitpointsmax":
                        ReturnExpression = ActiveUnit.MaxHP.ToString();
                        break;

                    case "en":
                    case "energy":
                        ReturnExpression = ActiveUnit.EN.ToString();
                        break;

                    case "maxen":
                    case "enmax":
                    case "maxenergy":
                    case "energymax":
                        ReturnExpression = ActiveUnit.MaxEN.ToString();
                        break;

                    case "name":
                        ReturnExpression = ActiveUnit.FullName;
                        break;
                }

                #endregion
            }

            return ReturnExpression;
        }
    }
}
