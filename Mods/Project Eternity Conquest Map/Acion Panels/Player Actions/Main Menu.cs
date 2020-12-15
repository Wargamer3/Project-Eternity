using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelPlayerMainMenu : ActionPanelConquest
    {
        public ActionPanelPlayerMainMenu(ConquestMap Map)
            : base("Player Unit Main Menu", Map)
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
            NavigateThroughNextChoices(Map.sndSelection, Map.sndConfirm);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }
}
