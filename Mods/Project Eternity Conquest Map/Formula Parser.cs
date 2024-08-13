using System;
using System.Linq;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ConquestFormulaParser : FormulaParser
    {
        private ConquestMap Map;

        public ConquestFormulaParser(ConquestMap Map)
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
                        if (Map.Params.GlobalContext.EffectOwnerSquad == null)
                            throw new Exception(Input + " is invalid");
                        return UnitStatFromUnit(Map.ListPlayer[Map.ActivePlayerIndex], (UnitConquest)Map.Params.GlobalContext.EffectOwnerUnit, Expression[1]);

                    case "def":
                    case "defender":
                        if (Map.Params.GlobalContext.EffectTargetSquad == null)
                            throw new Exception(Input + " is invalid");
                        return UnitStatFromUnit(Map.ListPlayer[Map.TargetPlayerIndex], (UnitConquest)Map.Params.GlobalContext.EffectTargetUnit, Expression[1]);

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
                                    case "exist":
                                    case "exists":
                                        if (ActivePlayer.TeamIndex >= 0)
                                        {
                                            return "1";
                                        }
                                        else
                                        {
                                            return "0";
                                        }
                                    case "count":
                                    case "squadcount":
                                    case "squadcountalive":
                                    case "squad count":
                                    case "squad count alive":
                                        int SquadCountAlive = 0;
                                        for (int S = 0; S < ActivePlayer.ListUnit.Count; S++)
                                        {
                                            if (ActivePlayer.ListUnit[S].HP > 0)
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
            Player ActivePlayer = null;
            UnitConquest ActiveUnit = null;

            int UnitNumber = 0;
            bool UnitCount = false;

            switch (Expression[1])
            {
                case "name":
                    ReturnExpression = "0";
                    for (int P = ListPlayer.Count - 1; P >= 0 && ActiveUnit == null; --P)
                    {
                        for (int S = ListPlayer[P].ListUnit.Count - 1; S >= 0 && ActiveUnit == null; --S)
                        {
                            if (ListPlayer[P].ListUnit[S].UnitStat.Name.ToLower().Replace(" ", "") == Expression[2] && ListPlayer[P].ListUnit[S].HP > 0)
                            {
                                if (UnitCount)
                                    ++UnitNumber;
                                else
                                {
                                    ActivePlayer = ListPlayer[P];
                                    ActiveUnit = ListPlayer[P].ListUnit[S];
                                    ReturnExpression = "1";
                                }
                            }
                        }
                    }
                    if (UnitCount)
                        ReturnExpression = UnitNumber.ToString();
                    break;

                case "originalname"://Transforming Unit have a different ItemName. ie Getter has Getter as ItemName but Getter-1 as UnitStat Name
                    ReturnExpression = "0";
                    for (int P = ListPlayer.Count - 1; P >= 0 && ActiveUnit == null; --P)
                    {
                        for (int S = ListPlayer[P].ListUnit.Count - 1; S >= 0 && ActiveUnit == null; --S)
                        {
                            if (ListPlayer[P].ListUnit[S].ItemName.ToLower().Replace(" ", "") == Expression[2] && ListPlayer[P].ListUnit[S].HP > 0)
                            {
                                if (UnitCount)
                                    ++UnitNumber;
                                else
                                {
                                    ActivePlayer = ListPlayer[P];
                                    ActiveUnit = ListPlayer[P].ListUnit[S];
                                    ReturnExpression = "1";
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

                    for (int P = ListPlayer.Count - 1; P >= 0 && ActiveUnit == null; --P)
                    {
                        for (int S = ListPlayer[P].ListUnit.Count - 1; S >= 0 && ActiveUnit == null; --S)
                        {
                            for (int X = StartX; X < EndX; ++X)
                            {
                                for (int Y = StartY; Y < EndY; ++Y)
                                {
                                    if (ListPlayer[P].ListUnit[S].X == X &&
                                        ListPlayer[P].ListUnit[S].Y == Y)
                                    {
                                        if (ListPlayer[P].ListUnit[S].HP > 0)
                                        {
                                            ActivePlayer = ListPlayer[P];
                                            ActiveUnit = ListPlayer[P].ListUnit[S];
                                            ReturnExpression = "1";
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
            if (ActiveUnit == null)
                return ReturnExpression;

            if (Expression.Length >= 4)
            {
                ReturnExpression = UnitStatFromUnit(ActivePlayer, ActiveUnit, Expression[3]);
            }

            return ReturnExpression;
        }

        private string UnitStatFromUnit(Player ActivePlayer, UnitConquest ActiveUnit, string Expression)
        {
            string ReturnExpression = null;

            switch (Expression)
            {
                case "position":
                case "location":
                    ReturnExpression = "[" + ActiveUnit.X + "," + ActiveUnit.Y + "]";
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

                case "kills":
                    return ActivePlayer.Records.PlayerUnitRecords.DicUnitIDByNumberOfKills[ActiveUnit.ID].ToString();
            }

            return ReturnExpression;
        }

        public string SquadValueFromVariable(List<Player> ListPlayer, string[] Expression)
        {
            string ReturnExpression = null;
            Player ActivePlayer = null;
            UnitConquest ActiveUnit = null;

            int SquadNumber = 0;
            bool SquadCount = false;
            if (Expression.Length >= 4 && Expression[3] == "count")
                SquadCount = true;

            switch (Expression[1])
            {
                case "id":
                    ReturnExpression = "0";
                    UInt32 SquadID = Convert.ToUInt32(Expression[2]);
                    for (int P = ListPlayer.Count - 1; P >= 0 && ActiveUnit == null; --P)
                    {
                        for (int S = ListPlayer[P].ListUnit.Count - 1; S >= 0 && ActiveUnit == null; --S)
                        {
                            if (ListPlayer[P].ListUnit[S].SpawnID == SquadID && ListPlayer[P].ListUnit[S].HP > 0)
                            {
                                if (SquadCount)
                                    ++SquadNumber;
                                else
                                {
                                    ActivePlayer = ListPlayer[P];
                                    ActiveUnit = ListPlayer[P].ListUnit[S];
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
                    for (int P = ListPlayer.Count - 1; P >= 0 && ActiveUnit == null; --P)
                    {
                        for (int S = ListPlayer[P].ListUnit.Count - 1; S >= 0 && ActiveUnit == null; --S)
                        {
                            if (ListPlayer[P].ListUnit[S].RelativePath == Expression[2] && ListPlayer[P].ListUnit[S].HP > 0)
                            {
                                if (SquadCount)
                                    ++SquadNumber;
                                else
                                {
                                    ActivePlayer = ListPlayer[P];
                                    ActiveUnit = ListPlayer[P].ListUnit[S];
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

                    for (int P = ListPlayer.Count - 1; P >= 0 && ActiveUnit == null; --P)
                    {
                        for (int S = ListPlayer[P].ListUnit.Count - 1; S >= 0 && ActiveUnit == null; --S)
                        {
                            for (int X = StartX; X <= EndX; ++X)
                            {
                                for (int Y = StartY; Y <= EndY; ++Y)
                                {
                                    if (ListPlayer[P].ListUnit[S].X == X &&
                                        ListPlayer[P].ListUnit[S].Y == Y)
                                    {
                                        if (ListPlayer[P].ListUnit[S].HP > 0)
                                        {
                                            ActivePlayer = ListPlayer[P];
                                            ActiveUnit = ListPlayer[P].ListUnit[S];
                                            ReturnExpression = "1";
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

            if (ActiveUnit == null)
                return ReturnExpression;

            if (Expression.Length >= 4)
            {
                #region Squad variables

                switch (Expression[3])
                {
                    case "position":
                    case "location":
                        ReturnExpression = "[" + ActiveUnit.X + "," + ActiveUnit.Y + "]";
                        break;

                    case "name":
                        ReturnExpression = ActiveUnit.RelativePath;
                        break;

                    case "id":
                        ReturnExpression = ActiveUnit.SpawnID.ToString();
                        break;

                    default:
                        return UnitStatFromUnit(ActivePlayer, ActiveUnit, Expression[4]);
                }

                #endregion
            }

            if (Expression.Length >= 5)
            {
                if (ActiveUnit == null)
                    return ReturnExpression;

                return UnitStatFromUnit(ActivePlayer, ActiveUnit, Expression[4]);
            }

            return ReturnExpression;
        }

        public string ValueFromRecords(string[] Expression)
        {
            string ReturnExpression = null;
            switch (Expression[1])
            {
                case "unit":
                    return UnitStatFromUnitRecords(Map.Params.GlobalContext.EffectTargetUnit, Expression);
                case "pilot":
                    return CharacterRecords(Map.Params.GlobalContext.EffectTargetCharacter, Expression);
            }
            return ReturnExpression;
        }

        public string UnitStatFromUnitRecords(Unit ActiveUnit, string[] Expression)
        {
            BattleMapScreen.PlayerRecords ActivePlayerRecords = Map.ListPlayer[Map.ActivePlayerIndex].Records;

            string ReturnExpression = null;
            switch (Expression[2])
            {
                case "kills":
                    return ActivePlayerRecords.PlayerUnitRecords.DicUnitIDByNumberOfKills[ActiveUnit.ID].ToString();
            }
            return ReturnExpression;
        }

        public string CharacterRecords(Character ActiveUnit,string[] Expression)
        {
            BattleMapScreen.PlayerRecords ActivePlayerRecords = Map.ListPlayer[Map.ActivePlayerIndex].Records;

            string ReturnExpression = null;
            switch (Expression[2])
            {
            }
            return ReturnExpression;
        }
    }
}
