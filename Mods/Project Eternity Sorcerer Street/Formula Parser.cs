using System;
using System.Linq;
using System.Collections.Generic;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetFormulaParser : FormulaParser
    {
        private SorcererStreetBattleParams Params;

        public SorcererStreetFormulaParser(SorcererStreetBattleParams Params)
        {
            this.Params = Params;
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
                    if (PlayerIndex < Params.Map.ListPlayer.Count)
                        ListPlayer.Add(Params.Map.ListPlayer[PlayerIndex]);

                    Expression = Expression.Skip(2).ToArray();
                }
                else
                {
                    foreach (Player ActivePlayer in Params.Map.ListPlayer)
                    {
                        ListPlayer.Add(ActivePlayer);
                    }
                }
                switch (Expression[0])
                {
                    case "unit":
                        ReturnExpression = CreatureValueFromVariable(ListPlayer, Expression);
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
                                return Params.Map.GameTurn.ToString();

                            case "phase":
                            case "currentphase":
                            case "activeplayer":
                            case "currentplayer":
                                return Params.Map.ActivePlayerIndex.ToString();

                            default:
                                return Params.Map.DicMapVariables[Expression[1]].ToString();
                        }

                    case "atk":
                    case "invader":
                        if (Params.GlobalContext.Invader == null)
                            throw new Exception(Input + " is invalid");
                        return StatsFromCreature(Params.GlobalContext.Invader, Expression[1]);

                    case "def":
                    case "defender":
                        if (Params.GlobalContext.Defender == null)
                            throw new Exception(Input + " is invalid");
                        return StatsFromCreature(Params.GlobalContext.Defender,  Expression[1]);

                    case "self":
                    case "owner":
                    case "ownercreature":
                        return StatsFromCreature(Params.GlobalContext.SelfCreature, Expression[1]);

                    case "opponent":
                    case "opponentcreature":
                        return StatsFromCreature(Params.GlobalContext.OpponentCreature, Expression[1]);

                    case "creatures":
                        return StatsFromCreatures(Expression[1]);

                    case "ownerplayer":
                        return PlayerStatsFromPlayer(Params.GlobalContext.SelfCreature.Owner, Expression[1]);

                    case "opponentplayer":
                        return PlayerStatsFromPlayer(Params.GlobalContext.OpponentCreature.Owner, Expression[1]);

                    default:
                        if (Expression[0].Contains("player"))
                        {
                            string[] ArrayPlayerValue = Expression[0].Split(new[] { "player" }, StringSplitOptions.RemoveEmptyEntries);
                            if (ArrayPlayerValue.Length == 1)
                            {
                                int PlayerIndex = Convert.ToInt32(ArrayPlayerValue[0]) - 1;
                                if (Params.Map.ListPlayer.Count <= PlayerIndex)
                                    return "0";
                                Player ActivePlayer = Params.Map.ListPlayer[PlayerIndex];
                                switch (Expression[1])
                                {
                                    case "exist":
                                    case "exists":
                                        if (ActivePlayer.Team >= 0)
                                        {
                                            return "1";
                                        }
                                        else
                                        {
                                            return "0";
                                        }
                                    case "count":
                                    case "creaturecount":
                                    case "creaturecountalive":
                                    case "creature count":
                                    case "creature count alive":
                                        int CreatureCountAlive = 0;
                                        for (int X = 0; X < Params.Map.MapSize.X; ++X)
                                        {
                                            for (int Y = 0; Y < Params.Map.MapSize.Y; ++Y)
                                            {
                                                CreatureCard DefendingCreature = Params.Map.GetTerrain(new Microsoft.Xna.Framework.Vector3(X, Y, 0)).DefendingCreature;

                                                if (DefendingCreature != null && DefendingCreature.Name.ToLower().Replace(" ", "") == Expression[2])
                                                {
                                                    CreatureCountAlive++;
                                                }
                                            }
                                        }
                                        return CreatureCountAlive.ToString();
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

        public string CreatureValueFromVariable(List<Player> ListPlayer, string[] Expression)
        {
            string ReturnExpression = null;
            Player ActivePlayer = null;
            CreatureCard ActiveCreature = null;

            int UnitNumber = 0;
            bool UnitCount = false;

            switch (Expression[1])
            {
                case "name":
                    ReturnExpression = "0";
                    for (int X = 0; X < Params.Map.MapSize.X; ++X)
                    {
                        for (int Y = 0; Y < Params.Map.MapSize.Y; ++Y)
                        {
                            CreatureCard DefendingCreature = Params.Map.GetTerrain(new Microsoft.Xna.Framework.Vector3(X, Y, 0)).DefendingCreature;

                            if (DefendingCreature != null && DefendingCreature.Name.ToLower().Replace(" ", "") == Expression[2])
                            {
                                if (UnitCount)
                                    ++UnitNumber;
                                else
                                {
                                    ActiveCreature = DefendingCreature;
                                    ReturnExpression = "1";
                                }
                            }
                        }
                    }
                    if (UnitCount)
                        ReturnExpression = UnitNumber.ToString();
                    break;

                case "standing":
                    break;

            }
            if (ActiveCreature == null)
                return ReturnExpression;

            if (Expression.Length >= 4)
            {
                ReturnExpression = StatsFromCreature(new SorcererStreetBattleContext.BattleCreatureInfo(ActiveCreature, ActivePlayer), Expression[3]);
            }

            return ReturnExpression;
        }

        private string StatsFromCreature(SorcererStreetBattleContext.BattleCreatureInfo ActiveCreature, string Expression)
        {
            string ReturnExpression = null;

            switch (Expression)
            {
                case "name":
                case "leadername":
                case "currentleadername":
                    ReturnExpression = ActiveCreature.Creature.Name.ToLower().Replace(" ", "");
                    break;

                case "hp":
                case "hitpoint":
                case "hitpoints":
                    ReturnExpression = ActiveCreature.Creature.CurrentHP.ToString();
                    break;

                case "mhp":
                case "maxhp":
                case "hpmax":
                case "basehp":
                case "maxhitpoint":
                case "maxhitpoints":
                case "hitpointmax":
                case "hitpointsmax":
                    ReturnExpression = ActiveCreature.Creature.MaxHP.ToString();
                    break;

                case "st":
                case "strengt":
                    ReturnExpression = ActiveCreature.Creature.CurrentST.ToString();
                    break;

                case "damagereceived":
                    ReturnExpression = ActiveCreature.DamageReceived.ToString();
                    break;

                case "terrainlevel":
                    ReturnExpression = Params.GlobalContext.DefenderTerrain.LandLevel.ToString();
                    break;

                case "cardsinhand":
                    ReturnExpression = ActiveCreature.Owner.ListCardInHand.Count.ToString();
                    break;
            }

            return ReturnExpression;
        }

        private string StatsFromCreatures(string Expression)
        {
            string ReturnExpression = null;

            switch (Expression)
            {
                case "air":
                    ReturnExpression = Params.GlobalContext.DicCreatureCountByElementType[CreatureCard.ElementalAffinity.Air].ToString();
                    break;

                case "earth":
                    ReturnExpression = Params.GlobalContext.DicCreatureCountByElementType[CreatureCard.ElementalAffinity.Earth].ToString();
                    break;

                case "fire":
                    ReturnExpression = Params.GlobalContext.DicCreatureCountByElementType[CreatureCard.ElementalAffinity.Fire].ToString();
                    break;

                case "water":
                    ReturnExpression = Params.GlobalContext.DicCreatureCountByElementType[CreatureCard.ElementalAffinity.Water].ToString();
                    break;
            }

            return ReturnExpression;
        }

        private string PlayerStatsFromPlayer(Player ActivePlayer, string Expression)
        {
            string ReturnExpression = null;

            switch (Expression)
            {
                case "standing":
                case "currentstanding":
                case "currentleadername":
                    ReturnExpression = ActivePlayer.Rank.ToString();
                    break;
            }

            return ReturnExpression;
        }
    }
}
