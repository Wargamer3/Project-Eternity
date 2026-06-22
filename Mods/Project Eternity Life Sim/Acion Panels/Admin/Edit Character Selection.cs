using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelEditCharacterSelection : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "EditCharacterSelection";

        private static List<PlayerCharacter> ListCharacter;

        public ActionPanelEditCharacterSelection(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, true)
        {
            ListCharacter = new List<PlayerCharacter>();
        }

        public override void OnSelect()
        {
            foreach (PlayerCharacter ActiveCharacter in Owner.InvisibleCharacterAsCursor.SharedMapContex.ActiveMap.ListCharacter)
            {
                ListCharacter.Add(ActiveCharacter);
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ActiveInputManager.InputConfirmPressed())
            {
                RemoveAllActionPanels();
                AddToPanelListAndSelect(new ActionPanelEditCharacter(Owner, ListCharacter[ActionMenuCursor], MapManager, ListActionMenuChoice));
            }
            else if (ActiveInputManager.InputUpPressed())
            {
                if (--ActionMenuCursor < 0)
                {
                    ActionMenuCursor = ListCharacter.Count - 1;
                }
            }
            if (ActiveInputManager.InputDownPressed())
            {
                if (++ActionMenuCursor >= ListCharacter.Count)
                {
                    ActionMenuCursor = 0;
                }
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
            return new ActionPanelEditCharacterSelection(Owner, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Draw the action panel.
            int X = FinalMenuX;
            int Y = FinalMenuY;

            X += 20;
            GameScreen.DrawBox(g, new Vector2(X, Y), ActionMenuWidth, MenuHeight, Color.White);
            X += 10;
            Y += 14;

            //Draw the choices.
            foreach (PlayerCharacter ActiveCharacter in ListCharacter)
            {
                TextHelper.DrawText(g, ActiveCharacter.Name.ToString(), new Vector2(X, Y), Color.White);
                Y += PannelHeight;
            }

            Y = BaseMenuY;
            if (Y + MenuHeight >= Constants.Height)
                Y = Constants.Height - MenuHeight;
            //Draw the menu cursor.
            g.Draw(GameScreen.sprPixel, new Rectangle(X, 9 + Y + ActionMenuCursor * PannelHeight, ActionMenuWidth - 20, PannelHeight - 5), Color.FromNonPremultiplied(255, 255, 255, 200));
        }
    }
}
