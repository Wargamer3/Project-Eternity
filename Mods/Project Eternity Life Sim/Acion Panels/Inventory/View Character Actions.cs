using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelViewCharacterActions : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "ViewCharacterActions";

        private readonly PlayerCharacter ActiveCharacter;

        public ActionPanelViewCharacterActions(PlayerOverseer Owner, PlayerCharacter ActiveCharacter, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, true)
        {
            this.ActiveCharacter = ActiveCharacter;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ActiveInputManager.InputConfirmPressed())
            {
                ActiveCharacter.ArrayCharacterAction[ActionMenuCursor].ActivateFromMenu();
            }
            else if (ActiveInputManager.InputLeftPressed())
            {
                AddToPanelListAndSelect(new ActionPanelViewCharacter(Owner, ActiveCharacter, MapManager, ListActionMenuChoice));
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
            return new ActionPanelViewCharacterActions(Owner, ActiveCharacter, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            ActionPanelViewCharacter.DrawHeader(g, ActiveCharacter);
            ActionPanelViewCharacter.DrawLeftPart(g, ActiveCharacter);

            int BaseMenuHeight = 40;
            int MenuWidth = 600;
            int MenuHeight = BaseMenuHeight + ActiveCharacter.ArrayCharacterAction.Length * 30;
            int MenuX = 200;
            int MenuY = 30;
            int TextX = MenuX + 5;
            int TextY = MenuY + 5;

            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, MenuHeight, Color.White);
            TextHelper.DrawText(g, "Attacks", new Vector2(TextX, TextY), Color.White);
            TextX = 250;
            TextY += 30;
            foreach (CharacterAction ActiveAction in ActiveCharacter.ArrayCharacterAction)
            {
                TextHelper.DrawText(g, ActiveAction.Name, new Vector2(TextX, TextY), Color.White);
                TextY += 30;
            }

            MenuY += MenuHeight;
            MenuHeight = BaseMenuHeight + 3 * 30;
            TextX = MenuX + 5;
            TextY = MenuY + 5;
            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, MenuHeight, Color.White);
            TextHelper.DrawText(g, "Actions", new Vector2(TextX, TextY), Color.White);
            TextX = 250;
            TextY += 30;
            foreach (CharacterAction ActiveAction in ActiveCharacter.ArrayCharacterAction)
            {
                TextHelper.DrawText(g, ActiveAction.Name, new Vector2(TextX, TextY), Color.White);
                TextY += 30;
                TextHelper.DrawText(g, ActiveAction.Name, new Vector2(TextX, TextY), Color.White);
                TextY += 30;
                TextHelper.DrawText(g, ActiveAction.Name, new Vector2(TextX, TextY), Color.White);
                TextY += 30;
            }

            MenuY += MenuHeight;
            MenuHeight = BaseMenuHeight + ActiveCharacter.ArrayCharacterAction.Length * 30;
            TextX = MenuX + 5;
            TextY = MenuY + 5;
            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, MenuHeight, Color.White);
            TextHelper.DrawText(g, "Reactions", new Vector2(TextX, TextY), Color.White);
            TextX = 250;
            TextY += 30;
            foreach (CharacterAction ActiveAction in ActiveCharacter.ArrayCharacterAction)
            {
                TextHelper.DrawText(g, ActiveAction.Name, new Vector2(TextX, TextY), Color.White);
                TextY += 30;
            }

            MenuY += MenuHeight;
            MenuHeight = BaseMenuHeight + ActiveCharacter.ArrayCharacterAction.Length * 30;
            TextX = MenuX + 5;
            TextY = MenuY + 5;
            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, MenuHeight, Color.White);
            TextHelper.DrawText(g, "Free Actions", new Vector2(TextX, TextY), Color.White);
            TextX = 250;
            TextY += 30;
            foreach (CharacterAction ActiveAction in ActiveCharacter.ArrayCharacterAction)
            {
                TextHelper.DrawText(g, ActiveAction.Name, new Vector2(TextX, TextY), Color.White);
                TextY += 30;
            }
        }
    }
}
