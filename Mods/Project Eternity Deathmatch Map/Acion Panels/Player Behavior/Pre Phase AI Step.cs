using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelPlayerPrePhaseAIStep : ActionPanelDeathmatch
    {
        List<Squad> ListAISquad;
        List<int> ListAISquadIndex;

        public ActionPanelPlayerPrePhaseAIStep(DeathmatchMap Map)
            : base("ActionPanelPlayerPrePhaseAIStep", Map, false)
        {
            ListAISquad = new List<Squad>();
            ListAISquadIndex = new List<int>();

            if (Map.ListPlayer.Count > 0)
            {
                //Keep a list of Squad in memory so we won't use Squad that would be spawned in this phase
                for (int U = 0; U < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad.Count; U++)
                {
                    if (Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[U].CurrentLeader != null && Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[U].SquadAI != null && !Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[U].IsPlayerControlled)
                    {
                        ListAISquad.Add(Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[U]);
                        ListAISquadIndex.Add(U);
                    }
                }
            }
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            int UnitsNotUpdatedCount = ListAISquad.Count;

            Map.MovementAnimation.Skip();

            for (int S = 0; S < ListAISquad.Count; S++)
            {
                Squad ActiveSquad = ListAISquad[S];
                Map.ActiveSquadIndex = ListAISquadIndex[S];

                if (!ActiveSquad.CanMove || ActiveSquad.CurrentLeader == null)
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

                Map.UpdateAllAttacks(ActiveSquad.CurrentLeader, ActiveSquad.Position, Map.ListPlayer[Map.ActivePlayerIndex].Team, ActiveSquad.CanMove);

                Map.TargetSquadIndex = -1;

                ActiveSquad.SquadAI.UpdateStep(gameTime);

                Map.TargetSquadIndex = -1;
                break;
            }

            if (UnitsNotUpdatedCount == 0)
            {
                RemoveAllActionPanels();

                if (!Map.ListPlayer[Map.ActivePlayerIndex].IsPlayerControlled)
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelPlayerAIStep(Map));
                }
                else
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelPlayerHumanStep(Map));
                }
            }
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPlayerPrePhaseAIStep(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
