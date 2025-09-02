using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelPlayerPrePhaseAIStep : ActionPanelConquest
    {
        List<UnitConquest> ListAISquad;
        List<int> ListAISquadIndex;

        public ActionPanelPlayerPrePhaseAIStep(ConquestMap Map)
            : base("ActionPanelPlayerPrePhaseAIStep", Map, false)
        {
            ListAISquad = new List<UnitConquest>();
            ListAISquadIndex = new List<int>();

            if (Map.ListPlayer.Count > 0)
            {
                //Keep a list of Squad in memory so we won't use Squad that would be spawned in this phase
                for (int U = 0; U < Map.ListPlayer[Map.ActivePlayerIndex].ListUnit.Count; U++)
                {
                    if (Map.ListPlayer[Map.ActivePlayerIndex].ListUnit[U].HP > 0 && Map.ListPlayer[Map.ActivePlayerIndex].ListUnit[U].SquadAI != null && !Map.ListPlayer[Map.ActivePlayerIndex].ListUnit[U].IsPlayerControlled)
                    {
                        ListAISquad.Add(Map.ListPlayer[Map.ActivePlayerIndex].ListUnit[U]);
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
                UnitConquest ActiveUnit = ListAISquad[S];
                Map.ActiveUnitIndex = ListAISquadIndex[S];

                if (!ActiveUnit.CanMove || ActiveUnit.HP == 0)
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
                RemoveAllActionPanels();

                if (!Map.ListPlayer[Map.ActivePlayerIndex].IsPlayerControlled)
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelPlayerAIStep(Map));
                }
                else
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelPlayerHumanStep(Map, Map.ActivePlayerIndex));
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
