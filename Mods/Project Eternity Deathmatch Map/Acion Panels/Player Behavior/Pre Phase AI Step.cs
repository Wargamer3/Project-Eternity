using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelPlayerPrePhaseAIStep : ActionPanelDeathmatch
    {
        List<Squad> ListAISquad;

        public ActionPanelPlayerPrePhaseAIStep(DeathmatchMap Map)
            : base("ActionPanelPlayerPrePhaseAIStep", Map, false)
        {
            ListAISquad = new List<Squad>();

            if (Map.ListPlayer.Count > 0)
            {
                //Keep a list of Squad in memory so we won't use Squad that would be spawned in this phase
                for (int U = 0; U < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad.Count; U++)
                {
                    if (Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[U].CurrentLeader != null && Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[U].SquadAI != null && !Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[U].IsPlayerControlled)
                    {
                        ListAISquad.Add(Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[U]);
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

            for (int U = 0; U < ListAISquad.Count; U++)
            {
                Squad ActiveSquad = ListAISquad[U];
                Map.ActiveSquadIndex = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad.IndexOf(ActiveSquad);

                if (!ActiveSquad.CanMove || ActiveSquad.CurrentLeader == null)
                {
                    --UnitsNotUpdatedCount;
                    continue;
                }

                ActiveSquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                if (ActiveSquad.CurrentWingmanA != null)
                    ActiveSquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                if (ActiveSquad.CurrentWingmanB != null)
                    ActiveSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;

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
                RemoveAllSubActionPanels();
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
