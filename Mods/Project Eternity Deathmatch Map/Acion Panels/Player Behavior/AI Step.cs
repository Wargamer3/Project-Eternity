using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelPlayerAIStep : ActionPanelDeathmatch
    {
        public ActionPanelPlayerAIStep(DeathmatchMap Map)
            : base("PlayerAIStep", Map, false)
        {
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            int UnitsNotUpdatedCount = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad.Count;

            for (int U = 0; U < Map.ListPlayer[Map.ActivePlayerIndex].ListSquad.Count; U++)
            {
                Map.ActiveSquadIndex = U;
                Squad ActiveSquad = Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[U];

                if (!Map.ActiveSquad.CanMove || Map.ActiveSquad.CurrentLeader == null)
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

                Map.TargetSquadIndex = -1;

                ActiveSquad.SquadAI.UpdateStep(gameTime);

                Map.TargetSquadIndex = -1;
                break;
            }

            if (UnitsNotUpdatedCount == 0)
            {
                ActionPanelPhaseChange.EndPlayerPhase(Map);
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
            return new ActionPanelPlayerAIStep(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
