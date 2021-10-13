using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    /// <summary>
    /// Called every time a player has finished his actions.
    /// </summary>
    public class ActionPanelPhaseChange : ActionPanelDeathmatch
    {
        private int PhaseTime;

        public ActionPanelPhaseChange(DeathmatchMap Map)
            : base("PhaseEnd", Map, false)
        {
            PhaseTime = 120;
        }

        public override void OnSelect()
        {
            Map.ListActionMenuChoice.RemoveAllActionPanels();//Will also remove this panel
            Map.ListActionMenuChoice.Add(this);
            List<BattleMap> ListActiveSubMaps = ActionPanelMapSwitch.GetActiveSubMaps(Map);
            if (ListActiveSubMaps.Count <= 1)
            {
                EndPlayerPhase(Map);
            }
            else
            {
                int InitialPlayerIndex = Map.ActivePlayerIndex;

                //Immediately update the sub maps, the player or AI will cycle through them if needed.
                foreach (BattleMap ActiveMap in ListActiveSubMaps)
                {
                    if (ActiveMap != Map && ActiveMap.ActivePlayerIndex == Map.ActivePlayerIndex)
                    {
                        DeathmatchMap ActiveDeathmatchMap = (DeathmatchMap)ActiveMap;
                        EndPlayerPhase(ActiveDeathmatchMap);
                        StartPlayerPhase(ActiveDeathmatchMap, ActiveDeathmatchMap.ListPlayer[ActiveDeathmatchMap.ActivePlayerIndex]);
                    }
                }

                EndPlayerPhase(Map);

                //If the current Map has no other Players it will skip the other Players in other sub maps. If that happen switch to a Map with the proper player.
                if (InitialPlayerIndex == Map.ActivePlayerIndex)
                {
                    bool HasFoundPlayer = false;
                    int NextPlayerIndex = InitialPlayerIndex + 1;

                    while (NextPlayerIndex != InitialPlayerIndex)
                    {
                        foreach (BattleMap ActiveMap in ListActiveSubMaps)
                        {
                            if (ActiveMap != Map && ActiveMap.ActivePlayerIndex == NextPlayerIndex)
                            {
                                DeathmatchMap ActiveDeathmatchMap = (DeathmatchMap)ActiveMap;

                                if (ActiveDeathmatchMap.ListPlayer.Count >= NextPlayerIndex)
                                {
                                    Map.ListGameScreen.Remove(Map);
                                    Map.ListGameScreen.Insert(0, ActiveMap);
                                    StartPlayerPhase(Map, Map.ListPlayer[Map.ActivePlayerIndex]);
                                    HasFoundPlayer = true;
                                    return;
                                }
                            }
                        }

                        if (HasFoundPlayer)
                        {
                            ++NextPlayerIndex;
                        }
                        else
                        {
                            NextPlayerIndex = 0;
                        }
                    }
                }
            }
        }

        public static void EndPlayerPhase(DeathmatchMap Map)
        {
            //Reset the cursor.
            Map.ActiveSquadIndex = -1;

            if (GameScreen.FMODSystem.sndActiveBGMName != Map.sndBattleThemeName && !string.IsNullOrEmpty(Map.sndBattleThemeName))
            {
                Map.sndBattleTheme.Stop();
                Map.sndBattleTheme.SetLoop(true);
                Map.sndBattleTheme.PlayAsBGM();
                GameScreen.FMODSystem.sndActiveBGMName = Map.sndBattleThemeName;
            }

            do
            {
                Map.ActivePlayerIndex++;
                UpdateDelayedAttacks(Map, Map.ActivePlayerIndex);

                if (Map.ActivePlayerIndex >= Map.ListPlayer.Count)
                {
                    Map.OnNewTurn();
                    UpdateDelayedAttacks(Map, Map.ActivePlayerIndex);
                }

                foreach (Player ActivePlayer in Map.ListPlayer)
                {
                    for (int S = 0; S < ActivePlayer.ListSquad.Count; S++)
                    {
                        Squad ActiveSquad = ActivePlayer.ListSquad[S];

                        for (int U = ActiveSquad.UnitsAliveInSquad - 1; U >= 0; --U)
                        {
                            ActiveSquad[U].UpdateSkillsLifetime(SkillEffect.LifetimeTypeTurns + Map.ActivePlayerIndex);
                            Map.ActivateAutomaticSkills(ActiveSquad, ActiveSquad[U], null, ActiveSquad, ActiveSquad[U]);
                        }
                    }
                }
            }
            while (!Map.ListPlayer[Map.ActivePlayerIndex].IsAlive);

            for (int S = 0; S < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad.Count; S++)
            {
                Squad ActiveSquad = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[S];

                for (int U = ActiveSquad.UnitsAliveInSquad - 1; U >= 0; --U)
                {
                    Map.ActivateAutomaticSkills(ActiveSquad, ActiveSquad[U], "Player Phase Start Requirement", ActiveSquad, ActiveSquad[U]);

                    //Repair passive bonus.
                    if (ActiveSquad[U].Boosts.RepairModifier)
                    {
                        for (int U2 = ActiveSquad.UnitsAliveInSquad - 1; U2 >= 0; --U2)
                        {
                            ActiveSquad[U2].HealUnit((int)(ActiveSquad[U2].MaxHP * 0.05));
                        }
                    }

                    //Resupply passive bonus.
                    if (ActiveSquad[U].Boosts.ResupplyModifier)
                    {
                        for (int U2 = ActiveSquad.UnitsAliveInSquad - 1; U2 >= 0; --U2)
                        {
                            ActiveSquad[U2].RefillEN((int)(ActiveSquad[U2].MaxEN * 0.05));
                        }
                    }

                    ActiveSquad[U].OnPlayerPhaseStart(Map.ActivePlayerIndex, ActiveSquad);
                }
            }
        }

        public static void UpdateDelayedAttacks(DeathmatchMap Map, int ActivePlayerIndex)
        {
            for (int A = Map.ListDelayedAttack.Count - 1; A >= 0; --A)
            {
                DelayedAttack ActiveDelayedAttack = Map.ListDelayedAttack[A];

                if (ActiveDelayedAttack.PlayerIndex == ActivePlayerIndex)
                {
                    if (--ActiveDelayedAttack.TurnsRemaining == 0)
                    {
                        Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelInitDelayedAttackMAP(Map, ActivePlayerIndex, ActiveDelayedAttack));

                        Map.ListDelayedAttack.RemoveAt(A);
                    }
                }
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (PhaseTime >= 0)
            {
                if (InputHelper.InputConfirmPressed() || InputHelper.InputCancelPressed() || InputHelper.InputSkipPressed())
                    PhaseTime = 0;
                else
                    PhaseTime--;

                if (PhaseTime > 0)
                {
                    return;
                }
                else if (PhaseTime == 0)//Phase start.
                {
                    StartPlayerPhase(Map, Map.ListPlayer[Map.ActivePlayerIndex]);
                }
            }
        }

        public override void DoRead(ByteReader BR)
        {
            PhaseTime = 120;
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPhaseChange(Map);
        }

        public static void StartPlayerPhase(DeathmatchMap Map, Player ActivePlayer)
        {
            Map.UpdateMapEvent(BattleMap.EventTypePhase, 0);
            Map.ListActionMenuChoice.RemoveAllActionPanels();//Will also remove this panel

            if (!ActivePlayer.IsPlayerControlled)
            {
                Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelPlayerAIStep(Map));
            }
            else
            {
                Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelPlayerHumanStep(Map));
            }

            bool AIControlledSquadFound = false;
            for (int U = 0; U < ActivePlayer.ListSquad.Count; U++)
            {
                if (ActivePlayer.ListSquad[U].CurrentLeader != null && ActivePlayer.ListSquad[U].SquadAI != null
                     && !ActivePlayer.ListSquad[U].IsPlayerControlled)
                {
                    AIControlledSquadFound = true;
                }
            }

            if (AIControlledSquadFound)
            {
                Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelPlayerPrePhaseAIStep(Map));
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawPhaseStartOverlay(g, Map);
        }

        public static void DrawPhaseStartOverlay(CustomSpriteBatch g, DeathmatchMap Map)
        {
            g.Draw(Map.sprPhaseBackground, Vector2.Zero, Color.White);

            if (Map.ListPlayer[Map.ActivePlayerIndex].IsPlayerControlled)
                g.Draw(Map.sprPhasePlayer, Vector2.Zero, Color.White);
            else
                g.Draw(Map.sprPhaseEnemy, Vector2.Zero, Color.White);

            g.Draw(Map.sprPhaseTurn, Vector2.Zero, Color.White);
            g.DrawStringMiddleAligned(Map.fntPhaseNumber, Map.GameTurn.ToString(), new Vector2(308, 292), Color.White);
        }
    }
}
