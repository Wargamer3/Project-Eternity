using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Server;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelPlayerAIStep : ActionPanelConquest
    {
        private int ActivePlayerIndex;

        public ActionPanelPlayerAIStep(ConquestMap Map)
            : base("PlayerAIStep", Map, false)
        {
            ActivePlayerIndex = Map.ActivePlayerIndex;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (Map.IsOnlineClient)
            {
                return;
            }

            int UnitsNotUpdatedCount = Map.ListPlayer[Map.ActivePlayerIndex].ListUnit.Count;

            for (int S = 0; S < Map.ListPlayer[Map.ActivePlayerIndex].ListUnit.Count; S++)
            {
                Map.ActiveUnitIndex = S;
                UnitConquest ActiveUnit = Map.ListPlayer[Map.ActivePlayerIndex].ListUnit[S];

                if (!Map.ActiveUnit.CanMove || Map.ActiveUnit.HP == 0)
                {
                    --UnitsNotUpdatedCount;
                    continue;
                }

                if (ActiveUnit.X < Map.Camera2DPosition.X || ActiveUnit.Y < Map.Camera2DPosition.Y ||
                    ActiveUnit.X >= Map.Camera2DPosition.X + Map.ScreenSize.X || ActiveUnit.Y >= Map.Camera2DPosition.Y + Map.ScreenSize.Y)
                {
                    Map.PushScreen(new CenterOnSquadCutscene(Map.CenterCamera, Map, ActiveUnit.Position));
                }

                Map.TargetSquadIndex = -1;

                ActiveUnit.SquadAI.UpdateStep(gameTime);

                Map.TargetSquadIndex = -1;
                break;
            }

            if (UnitsNotUpdatedCount == 0)
            {
                FinishAIPlayerTurn(Map);
            }
        }

        public static void FinishAIPlayerTurn(ConquestMap Map)
        {
            List<BattleMap> ListActiveSubMaps = ActionPanelMapSwitch.GetActiveSubMaps(Map);

            if (ListActiveSubMaps.Count > 1)//Look for sub maps to update before ending turn.
            {
                foreach (BattleMap ActiveMap in ListActiveSubMaps)
                {
                    if (ActiveMap != Map && ActiveMap.ActivePlayerIndex == Map.ActivePlayerIndex)
                    {
                        ActionPanelPhaseChange.EndPlayerPhase(Map);
                        Map.ListGameScreen.Remove(Map);
                        Map.ListGameScreen.Insert(0, ActiveMap);
                        return;
                    }
                }
            }

            Map.ListActionMenuChoice.RemoveAllActionPanels();
            ActionPanelPhaseChange EndPhase = new ActionPanelPhaseChange(Map);
            if (Map.IsServer)
            {
                EndPhase.ActiveSelect = true;
                foreach (IOnlineConnection ActiveOnlinePlayer in Map.GameGroup.Room.ListUniqueOnlineConnection)
                {
                    ActiveOnlinePlayer.Send(new OpenMenuScriptServer(new ActionPanel[] { EndPhase } ));
                }
            }
            Map.ListActionMenuChoice.AddToPanelListAndSelect(EndPhase);
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPlayerAIStep(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
