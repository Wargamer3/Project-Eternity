﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    /// <summary>
    /// Called every time a player has finished his actions.
    /// </summary>
    public class ActionPanelPhaseChange : ActionPanelDeathmatch
    {
        private int PhaseTime;
        public bool ActiveSelect;
        private int StartingPlayerIndex;

        public ActionPanelPhaseChange(DeathmatchMap Map)
            : base("PhaseEnd", Map, false)
        {
            SendBackToSender = true;
            PhaseTime = 120;
            ActiveSelect = false;
            StartingPlayerIndex = Map.ActivePlayerIndex;
        }

        public override void OnSelect()
        {
            ActiveSelect = true;
            if (Map.IsOnlineClient)
            {
                return;
            }

            ExecuteEndPhase();
        }

        private void ExecuteEndPhase()
        {
            List<BattleMap> ListActiveSubMaps = ActionPanelMapSwitch.GetActiveSubMaps(Map);
            if (ListActiveSubMaps.Count <= 1)
            {
                EndPlayerPhase(Map);
                if (Map.IsServer)
                {
                    StartPlayerPhase(Map, Map.ListPlayer[Map.ActivePlayerIndex]);
                }

                if (Map.ListPlayer.Count > 0)
                {
                    ActiveInputManager = Map.ListPlayer[Map.ActivePlayerIndex].InputManager;
                }
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

                if (Map.ListPlayer.Count > 0)
                {
                    ActiveInputManager = Map.ListPlayer[Map.ActivePlayerIndex].InputManager;
                }

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

            if (Map.IsClient && GameScreen.FMODSystem.sndActiveBGMName != Map.sndBattleThemeName && !string.IsNullOrEmpty(Map.sndBattleThemeName))
            {
                Map.sndBattleTheme.Stop();
                Map.sndBattleTheme.SetLoop(true);
                Map.sndBattleTheme.PlayAsBGM();
                GameScreen.FMODSystem.sndActiveBGMName = Map.sndBattleThemeName;
            }

            do
            {
                Map.ActivePlayerIndex++;

                if (Map.ActivePlayerIndex >= Map.ListPlayer.Count)
                {
                    OnNewTurn(Map);
                }

                Map.GameRule.OnNewTurn(Map.ActivePlayerIndex);

                UpdateProps(Map, Map.ActivePlayerIndex);
                UpdateDelayedAttacks(Map, Map.ActivePlayerIndex);
                UpdatePERAttacks(Map, Map.ActivePlayerIndex);
                UpdateSquadMovement(Map, Map.ActivePlayerIndex);

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

            Map.MapEnvironment.OnNewPlayerPhase();

            for (int S = 0; S < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad.Count; S++)
            {
                Squad ActiveSquad = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[S];

                if (ActiveSquad.ItemHeld != null)
                {
                    ActiveSquad.ItemHeld.OnTurnEnd(ActiveSquad, Map.ActivePlayerIndex);
                }

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

        /// <summary>
        /// Called every time every players has finished their actions.
        /// </summary>
        public static void OnNewTurn(DeathmatchMap Map)
        {
            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                for (int S = 0; S < Map.ListPlayer[P].ListSquad.Count; S++)
                {
                    Squad ActiveSquad = Map.ListPlayer[P].ListSquad[S];
                    if (ActiveSquad.CurrentLeader == null)
                        continue;

                    //Remove 5 EN each time the Squad spend a turn in the air.
                    int ENUsedPerTurn = Map.TerrainRestrictions.GetENUsedPerTurnCost(ActiveSquad, ActiveSquad.CurrentLeader.UnitStat, ActiveSquad.CurrentTerrainIndex);
                    if (ENUsedPerTurn > 0)
                        ActiveSquad.CurrentLeader.ConsumeEN(ENUsedPerTurn);

                    for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; U++)
                    {
                        foreach (Terrain ActiveTerrain in Map.GetAllTerrain(ActiveSquad, Map))
                        {
                            //Terrain passive bonus.
                            for (int i = 0; i < ActiveTerrain.BonusInfo.ListActivation.Length; i++)
                                switch (ActiveTerrain.BonusInfo.ListActivation[i])
                                {
                                    case TerrainActivation.OnEveryTurns:
                                        switch (ActiveTerrain.BonusInfo.ListBonus[i])
                                        {
                                            case TerrainBonus.HPRegen:
                                                ActiveSquad[U].HealUnit((int)(ActiveTerrain.BonusInfo.ListBonusValue[i] / 100.0f * ActiveSquad[U].MaxHP));
                                                break;

                                            case TerrainBonus.ENRegen:
                                                ActiveSquad[U].RefillEN((int)(ActiveTerrain.BonusInfo.ListBonusValue[i] / 100.0f * ActiveSquad[U].MaxEN));
                                                break;
                                            case TerrainBonus.HPRestore:
                                                ActiveSquad[U].HealUnit(ActiveTerrain.BonusInfo.ListBonusValue[i]);
                                                break;

                                            case TerrainBonus.ENRestore:
                                                ActiveSquad[U].RefillEN(ActiveTerrain.BonusInfo.ListBonusValue[i]);
                                                break;
                                        }
                                        break;
                                }
                        }
                    }
                }
            }

            Map.ActivePlayerIndex = 0;
            Map.GameTurn++;

            Map.UpdateMapEvent(DeathmatchMap.EventTypeTurn, 0);

            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                for (int S = 0; S < Map.ListPlayer[P].ListSquad.Count; S++)
                {
                    Squad ActiveSquad = Map.ListPlayer[P].ListSquad[S];
                    if (ActiveSquad.CurrentLeader == null)
                        continue;

                    ActiveSquad.StartTurn();

                    //Update Effect based on Turns.
                    for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; U++)
                    {
                        ActiveSquad[U].OnTurnEnd(Map.ActivePlayerIndex, ActiveSquad);
                    }
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

        public static void UpdatePERAttacks(DeathmatchMap Map, int ActivePlayerIndex)
        {
            for (int A = Map.ListPERAttack.Count - 1; A >= 0; --A)
            {
                PERAttack ActivePERAttack = Map.ListPERAttack[A];

                if (ActivePERAttack.PlayerIndex == ActivePlayerIndex)
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelUpdatePERAttacks(Map));
                    return;
                }
            }
        }

        public static void UpdateSquadMovement(DeathmatchMap Map, int ActivePlayerIndex)
        {
            for (int S = Map.ListPlayer[ActivePlayerIndex].ListSquad.Count - 1; S >= 0; --S)
            {
                if (Map.ListPlayer[ActivePlayerIndex].ListSquad[S].Speed != Vector3.Zero)
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelAutoMove(Map));
                    return;
                }
            }
        }

        public static void UpdateProps(DeathmatchMap Map, int ActivePlayerIndex)
        {
            foreach (MapLayer ActiveLayer in Map.LayerManager.ListLayer)
            {
                foreach (InteractiveProp ActiveProp in ActiveLayer.ListProp)
                {
                    ActiveProp.OnTurnEnd(ActivePlayerIndex);
                }
            }
        }

        public static void UpdateHoldableItems(DeathmatchMap Map, int ActivePlayerIndex)
        {
            foreach (MapLayer ActiveLayer in Map.LayerManager.ListLayer)
            {
                foreach (HoldableItem ActiveProp in ActiveLayer.ListHoldableItem)
                {
                    ActiveProp.OnTurnEnd(null, ActivePlayerIndex);
                }
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (PhaseTime >= 0)
            {
                if (ActiveInputManager != null && (ActiveInputManager.InputConfirmPressed() || ActiveInputManager.InputCancelPressed() || ActiveInputManager.InputSkipPressed()))
                {
                    PhaseTime = 0;
                }
                else
                {
                    PhaseTime--;
                }

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

        public override void UpdatePassive(GameTime gameTime)
        {
            DoUpdate(gameTime);
        }

        public override void DoRead(ByteReader BR)
        {
            PhaseTime = 120;
            ActiveSelect = BR.ReadBoolean();
            int NewActivePlayerIndex = BR.ReadInt32();
            if (Map.IsOnlineClient)
            {
                Map.ActivePlayerIndex = NewActivePlayerIndex;
            }

            if (ActiveSelect)
            {
                ExecuteEndPhase();
            }
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendBoolean(ActiveSelect);
            BW.AppendInt32(StartingPlayerIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPhaseChange(Map);
        }

        public static void StartPlayerPhase(DeathmatchMap Map, Player ActivePlayer)
        {
            Map.UpdateMapEvent(BattleMap.EventTypePhase, 0);
            Map.ListActionMenuChoice.RemoveAllActionPanels();//Will also remove this panel

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
            else
            {
                if (!ActivePlayer.IsPlayerControlled)
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelPlayerAIStep(Map));
                }
                else
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelPlayerHumanStep(Map));
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawPhaseStartOverlay(g, Map);
        }

        public static void DrawPhaseStartOverlay(CustomSpriteBatch g, DeathmatchMap Map)
        {
            g.Draw(Map.sprPhaseBackground, new Rectangle(0, 0, Constants.Width, Constants.Height), Color.White);

            float PosX = (Constants.Width - Map.sprPhaseBackground.Width) / 2;

            if (Map.ListPlayer[Map.ActivePlayerIndex].IsPlayerControlled)
                g.Draw(Map.sprPhasePlayer, new Vector2(PosX, 0), Color.White);
            else
                g.Draw(Map.sprPhaseEnemy, new Vector2(PosX, 0), Color.White);

            g.Draw(Map.sprPhaseTurn, new Vector2(PosX, 0), Color.White);
            g.DrawStringMiddleAligned(Map.fntPhaseNumber, Map.GameTurn.ToString(), new Vector2(PosX + 308, 292), Color.White);
        }
    }
}
