using System;
using System.Linq;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class DeathmatchFormulaParser : FormulaParser
    {
        private DeathmatchMap Map;

        public DeathmatchFormulaParser(DeathmatchMap Map)
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
            string ReturnExpression = null;

            if (Expression.Length >= 3)
            {
                List<Player> ListPlayer = new List<Player>();
                if (Expression[0] == "player")
                {
                    int PlayerIndex = Convert.ToInt32(Expression[1]) - 1;
                    if (PlayerIndex < Map.ListPlayer.Count)
                        ListPlayer.Add(Map.ListPlayer[PlayerIndex]);

                    Expression = Expression.Skip(2).ToArray();
                }
                else
                {
                    foreach (Player ActivePlayer in Map.ListPlayer)
                    {
                        ListPlayer.Add(ActivePlayer);
                    }
                }
                switch (Expression[0])
                {
                    case "squad":
                        ReturnExpression = SquadValueFromVariable(ListPlayer, Expression);
                        break;

                    case "unit":
                        ReturnExpression = UnitValueFromVariable(ListPlayer, Expression);
                        break;
                }
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
                            case "turn":
                            case "gameturn":
                            case "turngame":
                            case "battleturn":
                            case "turnbattle":
                            case "mapturn":
                            case "turnmap":
                            case "playerturn":
                                return Map.GameTurn.ToString();

                            case "phase":
                            case "currentphase":
                            case "activeplayer":
                            case "currentplayer":
                                return Map.ActivePlayerIndex.ToString();

                            default:
                                return Map.DicMapVariables[Expression[1]].ToString();
                        }

                    case "atk":
                        if (Map.GlobalDeathmatchContext.EffectOwnerSquad == null)
                            throw new Exception(Input + " is invalid");
                        return UnitStatFromUnit(Map.GlobalDeathmatchContext.EffectOwnerUnit, Map.GlobalDeathmatchContext.EffectOwnerSquad, Expression[1]);

                    case "def":
                    case "defender":
                        if (Map.GlobalDeathmatchContext.EffectTargetSquad == null)
                            throw new Exception(Input + " is invalid");
                        return UnitStatFromUnit(Map.GlobalDeathmatchContext.EffectTargetUnit, Map.GlobalDeathmatchContext.EffectTargetSquad, Expression[1]);

                    default:
                        if (Expression[0].Contains("player"))
                        {
                            string[] ArrayPlayerValue = Expression[0].Split(new[] { "player" }, StringSplitOptions.RemoveEmptyEntries);
                            if (ArrayPlayerValue.Length == 1)
                            {
                                int PlayerIndex = Convert.ToInt32(ArrayPlayerValue[0]) - 1;
                                if (Map.ListPlayer.Count <= PlayerIndex)
                                    return "0";
                                Player ActivePlayer = Map.ListPlayer[PlayerIndex];
                                switch (Expression[1])
                                {
                                    case "count":
                                    case "squadcount":
                                    case "squadcountalive":
                                    case "squad count":
                                    case "squad count alive":
                                        int SquadCountAlive = 0;
                                        for (int S = 0; S < ActivePlayer.ListSquad.Count; S++)
                                        {
                                            if (!ActivePlayer.ListSquad[S].IsDead && ActivePlayer.ListSquad[S].CurrentLeader != null && ActivePlayer.ListSquad[S].CurrentLeader.HP > 0)
                                                SquadCountAlive++;
                                        }
                                        return SquadCountAlive.ToString();
                                }
                            }
                        }
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

        public string UnitValueFromVariable(List<Player> ListPlayer, string[] Expression)
        {
            string ReturnExpression = null;
            Squad ActiveSquad = null;
            Unit ActiveUnit = null;

            int UnitNumber = 0;
            bool UnitCount = false;

            switch (Expression[1])
            {
                case "name":
                    ReturnExpression = "0";
                    for (int P = ListPlayer.Count - 1; P >= 0 && ActiveSquad == null; --P)
                    {
                        for (int S = ListPlayer[P].ListSquad.Count - 1; S >= 0 && ActiveSquad == null; --S)
                        {
                            for (int U = ListPlayer[P].ListSquad[S].UnitsAliveInSquad - 1; U >= 0; --U)
                            {
                                if (ListPlayer[P].ListSquad[S][U].UnitStat.Name.ToLower().Replace(" ", "") == Expression[2] && ListPlayer[P].ListSquad[S][U].HP > 0)
                                {
                                    if (UnitCount)
                                        ++UnitNumber;
                                    else
                                    {
                                        ActiveSquad = ListPlayer[P].ListSquad[S];
                                        ActiveUnit = ActiveSquad.CurrentLeader;
                                        ReturnExpression = "1";
                                    }
                                }
                            }
                        }
                    }
                    if (UnitCount)
                        ReturnExpression = UnitNumber.ToString();
                    break;

                case "originalname"://Transforming Unit have a different ItemName. ie Getter has Getter as ItemName but Getter-1 as UnitStat Name
                    ReturnExpression = "0";
                    for (int P = ListPlayer.Count - 1; P >= 0 && ActiveSquad == null; --P)
                    {
                        for (int S = ListPlayer[P].ListSquad.Count - 1; S >= 0 && ActiveSquad == null; --S)
                        {
                            for (int U = ListPlayer[P].ListSquad[S].UnitsAliveInSquad - 1; U >= 0; --U)
                            {
                                if (ListPlayer[P].ListSquad[S][U].ItemName.ToLower().Replace(" ", "") == Expression[2] && ListPlayer[P].ListSquad[S][U].HP > 0)
                                {
                                    if (UnitCount)
                                        ++UnitNumber;
                                    else
                                    {
                                        ActiveSquad = ListPlayer[P].ListSquad[S];
                                        ActiveUnit = ActiveSquad.CurrentLeader;
                                        ReturnExpression = "1";
                                    }
                                }
                            }
                        }
                    }
                    if (UnitCount)
                        ReturnExpression = UnitNumber.ToString();
                    break;
                case "at":
                case "atposition":
                    ReturnExpression = "0";
                    string[] MultipleEntries = Expression[2].Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] Position = MultipleEntries[0].Split(new string[] { ",", "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                    int StartX = Convert.ToInt32(Position[0]);
                    int StartY = Convert.ToInt32(Position[1]);
                    int EndX = StartX;
                    int EndY = StartY;

                    if (MultipleEntries.Length == 2)
                    {
                        Position = MultipleEntries[1].Split(new string[] { ",", "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                        EndX = Convert.ToInt32(Position[0]);
                        EndY = Convert.ToInt32(Position[1]);
                    }

                    for (int P = ListPlayer.Count - 1; P >= 0 && ActiveSquad == null; --P)
                    {
                        for (int S = ListPlayer[P].ListSquad.Count - 1; S >= 0 && ActiveSquad == null; --S)
                        {
                            for (int X = StartX; X < EndX; ++X)
                            {
                                for (int Y = StartY; Y < EndY; ++Y)
                                {
                                    if (ListPlayer[P].ListSquad[S].X == X &&
                                        ListPlayer[P].ListSquad[S].Y == Y)
                                    {
                                        for (int U = ListPlayer[P].ListSquad[S].UnitsAliveInSquad - 1; U >= 0; --U)
                                        {
                                            if (ListPlayer[P].ListSquad[S][U].HP > 0)
                                            {
                                                ActiveSquad = ListPlayer[P].ListSquad[S];
                                                ActiveUnit = ActiveSquad.CurrentLeader;
                                                ReturnExpression = "1";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (UnitCount)
                        ReturnExpression = "1";
                    break;
            }
            if (ActiveSquad == null)
                return ReturnExpression;

            if (Expression.Length >= 4)
            {
                ReturnExpression = UnitStatFromUnit(ActiveUnit, ActiveSquad, Expression[3]);
            }

            return ReturnExpression;
        }

        private string UnitStatFromUnit(Unit ActiveUnit, Squad ActiveSquad, string Expression)
        {
            string ReturnExpression = null;

            switch (Expression)
            {
                case "position":
                case "location":
                    ReturnExpression = "[" + ActiveSquad.X + "," + ActiveSquad.Y + "]";
                    break;

                case "name":
                case "leadername":
                case "currentleadername":
                    ReturnExpression = ActiveUnit.UnitStat.Name.ToLower().Replace(" ", "");
                    break;

                case "originalname"://Transforming Unit have a different ItemName. ie Getter has Getter as ItemName but Getter-1 as UnitStat Name
                    ReturnExpression = ActiveUnit.ItemName.ToLower().Replace(" ", "");
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

                case "will":
                    ReturnExpression = ActiveUnit.Pilot.Will.ToString();
                    break;
            }

            return ReturnExpression;
        }

        public string SquadValueFromVariable(List<Player> ListPlayer, string[] Expression)
        {
            string ReturnExpression = null;
            Squad ActiveSquad = null;
            Unit ActiveUnit = null;

            int SquadNumber = 0;
            bool SquadCount = false;
            if (Expression.Length >= 4 && Expression[3] == "count")
                SquadCount = true;

            switch (Expression[1])
            {
                case "id":
                    ReturnExpression = "0";
                    UInt32 SquadID = Convert.ToUInt32(Expression[2]);
                    for (int P = ListPlayer.Count - 1; P >= 0 && ActiveSquad == null; --P)
                    {
                        for (int S = ListPlayer[P].ListSquad.Count - 1; S >= 0 && ActiveSquad == null; --S)
                        {
                            if (ListPlayer[P].ListSquad[S].ID == SquadID && ListPlayer[P].ListSquad[S].CurrentLeader.HP > 0)
                            {
                                if (SquadCount)
                                    ++SquadNumber;
                                else
                                {
                                    ActiveSquad = ListPlayer[P].ListSquad[S];
                                    ActiveUnit = ActiveSquad.CurrentLeader;
                                    ReturnExpression = "1";
                                }
                            }
                        }
                    }
                    if (SquadCount)
                        ReturnExpression = SquadNumber.ToString();
                    break;

                case "name":
                    ReturnExpression = "0";
                    for (int P = ListPlayer.Count - 1; P >= 0 && ActiveSquad == null; --P)
                    {
                        for (int S = ListPlayer[P].ListSquad.Count - 1; S >= 0 && ActiveSquad == null; --S)
                        {
                            if (ListPlayer[P].ListSquad[S].SquadName == Expression[2] && ListPlayer[P].ListSquad[S].CurrentLeader.HP > 0)
                            {
                                if (SquadCount)
                                    ++SquadNumber;
                                else
                                {
                                    ActiveSquad = ListPlayer[P].ListSquad[S];
                                    ActiveUnit = ActiveSquad.CurrentLeader;
                                    ReturnExpression = "1";
                                }
                            }
                        }
                    }
                    if (SquadCount)
                        ReturnExpression = SquadNumber.ToString();
                    break;


                case "at":
                case "atposition":
                    ReturnExpression = "0";
                    string[] MultipleEntries = Expression[2].Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] Position = MultipleEntries[0].Split(new string[] { ",", "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                    int StartX = Convert.ToInt32(Position[0]);
                    int StartY = Convert.ToInt32(Position[1]);
                    int EndX = StartX;
                    int EndY = StartY;

                    if (MultipleEntries.Length == 2)
                    {
                        Position = MultipleEntries[0].Split(new string[] { ",", "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                        EndX = Convert.ToInt32(Position[0]);
                        EndY = Convert.ToInt32(Position[1]);
                    }

                    for (int P = ListPlayer.Count - 1; P >= 0 && ActiveSquad == null; --P)
                    {
                        for (int S = ListPlayer[P].ListSquad.Count - 1; S >= 0 && ActiveSquad == null; --S)
                        {
                            for (int X = StartX; X <= EndX; ++X)
                            {
                                for (int Y = StartY; Y <= EndY; ++Y)
                                {
                                    if (ListPlayer[P].ListSquad[S].X == X &&
                                        ListPlayer[P].ListSquad[S].Y == Y)
                                    {
                                        for (int U = ListPlayer[P].ListSquad[S].UnitsAliveInSquad - 1; U >= 0; --U)
                                        {
                                            if (ListPlayer[P].ListSquad[S][U].HP > 0)
                                            {
                                                ActiveSquad = ListPlayer[P].ListSquad[S];
                                                ActiveUnit = ActiveSquad.CurrentLeader;
                                                ReturnExpression = "1";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (SquadCount)
                        ReturnExpression = "1";
                    break;
            }
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
                        ReturnExpression = ActiveSquad.CurrentLeader.RelativePath;
                        break;

                    case "wingmananame":
                    case "currentwingmananame":
                    case "wingman1name":
                    case "currentwingman1name":
                        ReturnExpression = ActiveSquad.CurrentWingmanA.RelativePath;
                        break;

                    case "wingmanbname":
                    case "currentwingmanbname":
                    case "wingman2name":
                    case "currentwingman2name":
                        ReturnExpression = ActiveSquad.CurrentWingmanB.RelativePath;
                        break;

                    case "id":
                        ReturnExpression = ActiveSquad.ID.ToString();
                        break;

                    default:
                        return UnitStatFromUnit(ActiveUnit, ActiveSquad, Expression[4]);
                }

                #endregion
            }

            if (Expression.Length >= 5)
            {
                if (ActiveUnit == null)
                    return ReturnExpression;

                return UnitStatFromUnit(ActiveUnit, ActiveSquad, Expression[4]);
            }

            return ReturnExpression;
        }
    }
}
