using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelPlayerMainMenu : ActionPanelConquest
    {
        public ActionPanelPlayerMainMenu(ConquestMap Map)
            : base("Player Unit Main Menu", Map, false)
        {
        }

        public override void OnSelect()
        {
            AddChoiceToCurrentPanel(new ActionPanelMainMenuUnit(Map));
            AddChoiceToCurrentPanel(new ActionPanelMainMenuIntel(Map));
            AddChoiceToCurrentPanel(new ActionPanelMainMenuPower(Map));
            AddChoiceToCurrentPanel(new ActionPanelMainMenuSave(Map));
            AddChoiceToCurrentPanel(new ActionPanelMainMenuOptions(Map));
            AddChoiceToCurrentPanel(new ActionPanelMainMenuEnd(Map));
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (NavigateThroughNextChoices(Map.sndSelection))
            {
            }
            else if (ConfirmNextChoices(Map.sndConfirm))
            {
            }
            else if (ActiveInputManager.InputCancelPressed())
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
            return new ActionPanelPlayerMainMenu(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }
}
