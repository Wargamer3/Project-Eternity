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

                switch (Expression[0])
                {
                    case "unit":
                        ReturnExpression = CreatureValueFromVariable(ListPlayer, Expression);
                        break;

                    case "self":
                        switch (Expression[1])
                        {
                            case "item":
                                switch (Expression[2])
                                {
                                    case "mhp":
                                        if (Params.GlobalContext.SelfCreature.Item != null && Params.GlobalContext.SelfCreature.Item is CreatureCard)
                                        {
                                            return ((CreatureCard)Params.GlobalContext.SelfCreature.Item).MaxHP.ToString();
                                        }
                                        else
                                        {
                                            return "0";
                                        }
                                }
                                break;
                        }
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
                            case "turns":
                            case "gameturn":
                            case "gameturns":
                            case "turngame":
                            case "battleturn":
                            case "turnbattle":
                            case "mapturn":
                            case "turnmap":
                            case "playerturn":
                                return Params.GlobalContext.CurrentTurn.ToString();

                            default:
                                return Params.Map.DicMapVariables[Expression[1]].ToString();
                        }

                    case "creatures":
                        return StatsFromCreatures(Expression[1]);

                    case "atk":
                    case "invader":
                        if (Params.GlobalContext.SelfCreature == null)
                            throw new Exception(Input + " is invalid");
                        return StatsFromCreature(Params.GlobalContext.SelfCreature, Expression[1]);

                    case "def":
                    case "defender":
                        if (Params.GlobalContext.OpponentCreature == null)
                            throw new Exception(Input + " is invalid");
                        return StatsFromCreature(Params.GlobalContext.OpponentCreature,  Expression[1]);

                    case "owner":
                    case "ownercreature":
                    case "selfcreature":
                        return StatsFromCreature(Params.GlobalContext.SelfCreature, Expression[1]);

                    case "opponent":
                    case "opponentcreature":
                        return StatsFromCreature(Params.GlobalContext.OpponentCreature, Expression[1]);

                    case "self":
                    case "selfplayer":
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
                                        if (ActivePlayer.TeamIndex >= 0)
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
                switch (Expression[0])
                {
                    case "terrainlevel":
                        return Params.GlobalContext.ActiveTerrain.LandLevel.ToString();

                    case "creaturesdestroyed":
                        return Params.GlobalContext.TotalCreaturesDestroyed.ToString();
                }

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
                ReturnExpression = StatsFromCreature(new SorcererStreetBattleContext.BattleCreatureInfo(ActiveCreature, ActivePlayer, Params.Map.DicTeam[ActivePlayer.TeamIndex]), Expression[3]);
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

                case "damageneutralized":
                case "damageneutralizedbyopponent":
                    ReturnExpression = ActiveCreature.DamageNeutralizedByOpponent.ToString();
                    break;

                case "damagereflected":
                    ReturnExpression = ActiveCreature.DamageReflectedByOpponent.ToString();
                    break;

                case "damagereceived":
                    SorcererStreetBattleContext.BattleCreatureInfo Opponent = Params.GlobalContext.OpponentCreature;
                    if (ActiveCreature == Opponent)
                    {
                        Opponent = Params.GlobalContext.SelfCreature;
                    }
                    if (Opponent.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).ScrollAttack)//Ignore scroll attacks
                    {
                        ReturnExpression = "0";
                    }
                    else
                    {
                        ReturnExpression = ActiveCreature.DamageReceived.ToString();
                    }
                    break;

                case "terrainlevel":
                    ReturnExpression = Params.GlobalContext.ActiveTerrain.LandLevel.ToString();
                    break;

                case "cardsinhand":
                    ReturnExpression = ActiveCreature.Owner.ListCardInHand.Count.ToString();
                    break;

                case "spellanditemcardsinhand":
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

                case "neutral":
                    ReturnExpression = Params.GlobalContext.DicCreatureCountByElementType[CreatureCard.ElementalAffinity.Neutral].ToString();
                    break;

                default://Count By Name
                    ReturnExpression = Params.GlobalContext.CountCreaturesByName(Expression).ToString();
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
                    ReturnExpression = Params.Map.DicTeam[ActivePlayer.TeamIndex].Rank.ToString();
                    break;

                case "territories":
                    int TerritoryCount = 0;
                    foreach (byte ActiveElement in Params.Map.DicTeam[ActivePlayer.TeamIndex].DicCreatureCountByElementType.Values)
                    {
                        TerritoryCount += ActiveElement;
                    }
                    ReturnExpression = TerritoryCount.ToString();
                    break;

                case "airterritories":
                    ReturnExpression = Params.Map.DicTeam[ActivePlayer.TeamIndex].DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Air].ToString();
                    break;

                case "earthterritories":
                    ReturnExpression = Params.Map.DicTeam[ActivePlayer.TeamIndex].DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Earth].ToString();
                    break;

                case "fireterritories":
                    ReturnExpression = Params.Map.DicTeam[ActivePlayer.TeamIndex].DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Fire].ToString();
                    break;

                case "waterterritories":
                    ReturnExpression = Params.Map.DicTeam[ActivePlayer.TeamIndex].DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Water].ToString();
                    break;

                case "neutralterritories":
                    ReturnExpression = Params.Map.DicTeam[ActivePlayer.TeamIndex].DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Neutral].ToString();
                    break;

                case "magic":
                case "gold":
                    ReturnExpression = ActivePlayer.Gold.ToString();
                    break;

                case "lap":
                    ReturnExpression = ActivePlayer.CompletedLaps.ToString();
                    break;

                case "hand":
                case "cardsinhand":
                    ReturnExpression = ActivePlayer.ListCardInHand.Count.ToString();
                    break;

                case "deck":
                    ReturnExpression = ActivePlayer.ListCardInDeck.Count.ToString();
                    break;

                case "airsymbols":
                    ReturnExpression = ActivePlayer.DicOwnedSymbols[CreatureCard.ElementalAffinity.Air].ToString();
                    break;

                case "earthsymbols":
                    ReturnExpression = ActivePlayer.DicOwnedSymbols[CreatureCard.ElementalAffinity.Earth].ToString();
                    break;

                case "firesymbols":
                    ReturnExpression = ActivePlayer.DicOwnedSymbols[CreatureCard.ElementalAffinity.Fire].ToString();
                    break;

                case "watersymbols":
                    ReturnExpression = ActivePlayer.DicOwnedSymbols[CreatureCard.ElementalAffinity.Water].ToString();
                    break;

                case "neutralsymbols":
                    ReturnExpression = ActivePlayer.DicOwnedSymbols[CreatureCard.ElementalAffinity.Neutral].ToString();
                    break;

                case "symbols":
                    int Total = 0;
                    foreach (int TotalSymbol in ActivePlayer.DicOwnedSymbols.Values)
                    {
                        Total += TotalSymbol;
                    }
                    ReturnExpression = Total.ToString();
                    break;
            }

            return ReturnExpression;
        }
    }
}
