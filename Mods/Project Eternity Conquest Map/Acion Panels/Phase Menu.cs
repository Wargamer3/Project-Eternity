using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    /// <summary>
    /// Called every time a player has finished his actions.
    /// </summary>
    public class ActionPanelPhaseEnd : ActionPanelConquest
    {
        private int PhaseTime;
        private bool SendToHost;

        public ActionPanelPhaseEnd(ConquestMap Map, bool SendToHost)
            : base("PhaseEnd", Map)
        {
            PhaseTime = 120;
            this.SendToHost = SendToHost;
        }

        public override void OnSelect()
        {
            PhaseTime = 120;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (PhaseTime >= 0)
            {
                if (InputHelper.InputConfirmPressed() || InputHelper.InputCancelPressed() || InputHelper.InputSkipPressed())
                    PhaseTime = 0;
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
                    Map.UpdateMapEvent(BattleMapScreen.BattleMap.EventTypePhase, 0);
                    RemoveAllSubActionPanels();
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            GameScreen.DrawText(g, "New Phase", new Vector2(100, 100), Color.White);
        }
    }
}
