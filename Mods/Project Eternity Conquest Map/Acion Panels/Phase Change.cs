using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    /// <summary>
    /// Called every time a player has finished his actions.
    /// </summary>
    public class ActionPanelPhaseChange : ActionPanelConquest
    {
        private int PhaseTime;
        public bool ActiveSelect;
        private int StartingPlayerIndex;

        public ActionPanelPhaseChange(ConquestMap Map)
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
                        ConquestMap ActiveDeathmatchMap = (ConquestMap)ActiveMap;
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
                                ConquestMap ActiveDeathmatchMap = (ConquestMap)ActiveMap;

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

        public static void EndPlayerPhase(ConquestMap Map)
        {
            //Reset the cursor.
            Map.ActiveUnitIndex = -1;

            if (Map.IsClient && GameScreen.FMODSystem.sndActiveBGMName != Map.sndBattleThemePath && !string.IsNullOrEmpty(Map.sndBattleThemePath))
            {
                Map.sndBattleTheme.Stop();
                Map.sndBattleTheme.SetLoop(true);
                Map.sndBattleTheme.PlayAsBGM();
                GameScreen.FMODSystem.sndActiveBGMName = Map.sndBattleThemePath;
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
                    for (int S = 0; S < ActivePlayer.ListUnit.Count; S++)
                    {
                        UnitConquest ActiveUnit = ActivePlayer.ListUnit[S];

                        ActiveUnit.UpdateSkillsLifetime(SkillEffect.LifetimeTypeTurns + Map.ActivePlayerIndex);
                        Map.ActivateAutomaticSkills(null, ActiveUnit, null, null, ActiveUnit);
                    }
                }
            }
            while (!Map.ListPlayer[Map.ActivePlayerIndex].IsAlive);

            Map.MapEnvironment.OnNewPlayerPhase();

            for (int S = 0; S < Map.ListPlayer[Map.ActivePlayerIndex].ListUnit.Count; S++)
            {
                UnitConquest ActiveUnit = Map.ListPlayer[Map.ActivePlayerIndex].ListUnit[S];

                if (ActiveUnit.Components.ItemHeld != null)
                {
                    ActiveUnit.Components.ItemHeld.OnTurnEnd(null, Map.ActivePlayerIndex);
                }

                Map.ActivateAutomaticSkills(null, ActiveUnit, "Player Phase Start Requirement", null, ActiveUnit);

                //Repair passive bonus.
                if (ActiveUnit.Boosts.RepairModifier)
                {
                    ActiveUnit.HealUnit((int)(ActiveUnit.MaxHP * 0.05));
                }

                //Resupply passive bonus.
                if (ActiveUnit.Boosts.ResupplyModifier)
                {
                    ActiveUnit.RefillEN((int)(ActiveUnit.MaxEN * 0.05));
                }

                ActiveUnit.OnPlayerPhaseStart(Map.ActivePlayerIndex, null);
            }

            for (int B = 0; B < Map.ListBuilding.Count; ++B)
            {
                //Loop through the players to find a Unit to control.
                for (int P = 0; P < Map.ListPlayer.Count; P++)
                {
                    //Find if a Unit is under the cursor.
                    int CursorSelect = Map.CheckForUnitAtPosition(P, Map.CursorPosition, Vector3.Zero);

                    //If one was found.
                    if (CursorSelect < 0)
                    {
                        Map.ListBuilding[B].CurrentHP = Map.ListBuilding[B].MaxHP;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Called every time every players has finished their actions.
        /// </summary>
        public static void OnNewTurn(ConquestMap Map)
        {
            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                for (int S = 0; S < Map.ListPlayer[P].ListUnit.Count; S++)
                {
                    UnitConquest ActiveUnit = Map.ListPlayer[P].ListUnit[S];
                    if (ActiveUnit.HP == 0)
                        continue;

                    //Remove 5 EN each time the Squad spend a turn in the air.
                    int ENUsedPerTurn = 0;
                    if (ENUsedPerTurn > 0)
                        ActiveUnit.ConsumeEN(ENUsedPerTurn);

                    foreach (Terrain ActiveTerrain in Map.GetAllTerrain(ActiveUnit.Components, Map))
                    {
                        //Terrain passive bonus.
                        for (int i = 0; i < ActiveTerrain.BonusInfo.ListActivation.Length; i++)
                            switch (ActiveTerrain.BonusInfo.ListActivation[i])
                            {
                                case TerrainActivation.OnEveryTurns:
                                    switch (ActiveTerrain.BonusInfo.ListBonus[i])
                                    {
                                        case TerrainBonus.HPRegen:
                                            ActiveUnit.HealUnit((int)(ActiveTerrain.BonusInfo.ListBonusValue[i] / 100.0f * ActiveUnit.MaxHP));
                                            break;

                                        case TerrainBonus.ENRegen:
                                            ActiveUnit.RefillEN((int)(ActiveTerrain.BonusInfo.ListBonusValue[i] / 100.0f * ActiveUnit.MaxEN));
                                            break;
                                        case TerrainBonus.HPRestore:
                                            ActiveUnit.HealUnit(ActiveTerrain.BonusInfo.ListBonusValue[i]);
                                            break;

                                        case TerrainBonus.ENRestore:
                                            ActiveUnit.RefillEN(ActiveTerrain.BonusInfo.ListBonusValue[i]);
                                            break;
                                    }
                                    break;
                            }
                    }
                }
            }

            Map.ActivePlayerIndex = 0;
            Map.GameTurn++;

            Map.UpdateMapEvent(ConquestMap.EventTypeTurn, 0);

            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                for (int S = 0; S < Map.ListPlayer[P].ListUnit.Count; S++)
                {
                    UnitConquest ActiveUnit = Map.ListPlayer[P].ListUnit[S];
                    if (ActiveUnit.HP == 0)
                        continue;

                    ActiveUnit.StartTurn();

                    //Update Effect based on Turns.
                    ActiveUnit.OnTurnEnd(Map.ActivePlayerIndex, null);
                }
            }
        }

        public static void UpdateDelayedAttacks(ConquestMap Map, int ActivePlayerIndex)
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

        public static void UpdatePERAttacks(ConquestMap Map, int ActivePlayerIndex)
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

        public static void UpdateSquadMovement(ConquestMap Map, int ActivePlayerIndex)
        {
            for (int S = Map.ListPlayer[ActivePlayerIndex].ListUnit.Count - 1; S >= 0; --S)
            {
                if (Map.ListPlayer[ActivePlayerIndex].ListUnit[S].Components.Speed != Vector3.Zero)
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelAutoMove(Map));
                    return;
                }
            }
        }

        public static void UpdateProps(ConquestMap Map, int ActivePlayerIndex)
        {
            foreach (MapLayer ActiveLayer in Map.LayerManager.ListLayer)
            {
                foreach (InteractiveProp ActiveProp in ActiveLayer.ListProp)
                {
                    ActiveProp.OnTurnEnd(ActivePlayerIndex);
                }
            }
        }

        public static void UpdateHoldableItems(ConquestMap Map, int ActivePlayerIndex)
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

        public static void StartPlayerPhase(ConquestMap Map, Player ActivePlayer)
        {
            Map.UpdateMapEvent(BattleMap.EventTypePhase, 0);
            Map.ListActionMenuChoice.RemoveAllActionPanels();//Will also remove this panel

            bool AIControlledSquadFound = false;
            for (int U = 0; U < ActivePlayer.ListUnit.Count; U++)
            {
                if (ActivePlayer.ListUnit[U].HP > 0 && ActivePlayer.ListUnit[U].SquadAI != null
                     && !ActivePlayer.ListUnit[U].IsPlayerControlled)
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
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelPlayerHumanStep(Map, Map.ActivePlayerIndex));
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawPhaseStartOverlay(g, Map);
        }

        public static void DrawPhaseStartOverlay(CustomSpriteBatch g, ConquestMap Map)
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
