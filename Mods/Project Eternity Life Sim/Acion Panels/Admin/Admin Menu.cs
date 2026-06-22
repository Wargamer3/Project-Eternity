using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelAdminMenu : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "AdminMenu";

        public ActionPanelAdminMenu(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, false)
        {
        }

        public override void OnSelect()
        {
            ListNextChoice.Clear();
            AddChoiceToCurrentPanel(new ActionPanelSpawnCharacter(Owner, MapManager, ListActionMenuChoice));
            AddChoiceToCurrentPanel(new ActionPanelEditCharacterSelection(Owner, MapManager, ListActionMenuChoice));
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (NavigateThroughNextChoices(MapManager.sndSelection))
            {
            }
            else if (ConfirmNextChoices(MapManager.sndConfirm))
            {
            }
            else if (ActiveInputManager.InputCancelPressed())
            {
                RemoveAllSubActionPanels();
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAdminMenu(Owner, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            TextHelper.DrawText(g, "Admin Menu", new Vector2(), Color.Red);
            DrawNextChoice(g);
        }
    }
}
