using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;
using ProjectEternity.GameScreens.BattleMapScreen.Server;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelPlayerAIStep : ActionPanelDeathmatch
    {
        private int ActivePlayerIndex;

        public ActionPanelPlayerAIStep(DeathmatchMap Map)
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

            int UnitsNotUpdatedCount = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad.Count;

            for (int S = 0; S < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad.Count; S++)
            {
                Map.ActiveSquadIndex = S;
                Squad ActiveSquad = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[S];

                if (!Map.ActiveSquad.CanMove || Map.ActiveSquad.CurrentLeader == null)
                {
                    --UnitsNotUpdatedCount;
                    continue;
                }

                for (int U = ActiveSquad.UnitsAliveInSquad - 1; U >= 0; --U)
                {
                    ActiveSquad[U].BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                }

                if (ActiveSquad.X < Map.CameraPosition.X || ActiveSquad.Y < Map.CameraPosition.Y ||
                    ActiveSquad.X >= Map.CameraPosition.X + Map.ScreenSize.X || ActiveSquad.Y >= Map.CameraPosition.Y + Map.ScreenSize.Y)
                {
                    Map.PushScreen(new CenterOnSquadCutscene(Map.CenterCamera, Map, ActiveSquad.Position));
                }

                Map.TargetSquadIndex = -1;

                ActiveSquad.SquadAI.UpdateStep(gameTime);

                Map.TargetSquadIndex = -1;
                break;
            }

            if (UnitsNotUpdatedCount == 0)
            {
                FinishAIPlayerTurn(Map);
            }
        }

        public static void FinishAIPlayerTurn(DeathmatchMap Map)
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
                foreach (IOnlineConnection ActiveOnlinePlayer in Map.GameGroup.Room.ListOnlinePlayer)
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
