using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    /// <summary>
    /// Called every time a player has finished his actions.
    /// </summary>
    public class ActionPanelPhaseEnd : ActionPanelConquest
    {
        private int PhaseTime;

        public ActionPanelPhaseEnd(ConquestMap Map)
            : base("PhaseEnd", Map)
        {
            PhaseTime = 120;
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
        public override void DoRead(ByteReader BR)
        {
            PhaseTime = 120;
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPhaseEnd(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            TextHelper.DrawText(g, "New Phase", new Vector2(100, 100), Color.White);
        }

    }
}
