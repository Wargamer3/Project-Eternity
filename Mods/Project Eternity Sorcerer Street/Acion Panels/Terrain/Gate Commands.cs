using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelGateCommands : ActionPanelSorcererStreet
    {
        private enum Commands { Territory, Map, Info, Options, Help, End }

        private readonly Player ActivePlayer;

        private int CursorIndex;

        public ActionPanelGateCommands(SorcererStreetMap Map, Player ActivePlayer)
            : base("Gate Commands", Map, false)
        {
            this.ActivePlayer = ActivePlayer;
        }

        public override void OnSelect()
        {
            AddChoiceToCurrentPanel(new ActionPanelChooseTerritory(Map, ActivePlayer));
            AddChoiceToCurrentPanel(new ActionPanelViewMap(Map, ActivePlayer));
            AddChoiceToCurrentPanel(new ActionPanelInfo(Map, ActivePlayer));
            AddChoiceToCurrentPanel(new ActionPanelOptions(Map, ActivePlayer));
            AddChoiceToCurrentPanel(new ActionPanelHelp(Map, ActivePlayer));
            AddChoiceToCurrentPanel(new ActionPanelEndPlayerPhase(Map, ActivePlayer));
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        protected override void OnCancelPanel()
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            for (int P = 0; P < ListNextChoice.Count; ++P)
            {
                g.Draw(GameScreen.sprPixel, new Rectangle(P * 80 + 10, Constants.Height - 150, 70, 100), Color.Green);
            }
        }
    }
}
