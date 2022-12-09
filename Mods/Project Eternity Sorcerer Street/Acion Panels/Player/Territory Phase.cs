﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelTerritoryMenuPhase : ActionPanelSorcererStreet
    {
        private const string PanelName = "TerritoryPhase";

        private int ActivePlayerIndex;

        private double AnimationTime;

        public ActionPanelTerritoryMenuPhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelTerritoryMenuPhase(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
        }

        public override void OnSelect()
        {
            AddChoiceToCurrentPanel(new ActionPanelChooseTerritory(Map, ActivePlayerIndex));
            AddChoiceToCurrentPanel(new ActionPanelViewMap(Map));
            AddChoiceToCurrentPanel(new ActionPanelInfo(Map, ActivePlayerIndex));
            AddChoiceToCurrentPanel(new ActionPanelOptions(Map));
            AddChoiceToCurrentPanel(new ActionPanelHelp(Map));
            AddChoiceToCurrentPanel(new ActionPanelEndPlayerPhase(Map));
        }

        public override void DoUpdate(GameTime gameTime)
        {
            AnimationTime += gameTime.ElapsedGameTime.TotalSeconds;

            if (AnimationTime > Math.PI)
            {
                AnimationTime -= Math.PI;
            }

            if (InputHelper.InputLeftPressed() && --ActionMenuCursor < 0)
            {
                ActionMenuCursor = 5;
            }
            else if (InputHelper.InputRightPressed() && ++ActionMenuCursor > 5)
            {
                ActionMenuCursor = 0;
            }
            else if (InputHelper.InputUpPressed())
            {
                RemoveFromPanelList(this);
                AddToPanelListAndSelect(new ActionPanelSpellCardSelectionPhase(Map, ActivePlayerIndex));
            }
            else if (InputHelper.InputConfirmPressed())
            {
                AddToPanelListAndSelect(ListNextChoice[ActionMenuCursor]);
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
            return new ActionPanelTerritoryMenuPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int ActionInfoBoxX = Constants.Width / 16;
            int ActionInfoBoxY = Constants.Height / 3;
            int ActionInfoBoxWidth = Constants.Width / 5;
            int ActionInfoBoxHeight = Constants.Height / 14;
            ActionPanelPlayerDefault.DrawPlayerInformation(g, Map, Map.ListAllPlayer[ActivePlayerIndex], Constants.Width / 16, Constants.Height / 10);
            MenuHelper.DrawBorderlessBox(g, new Vector2(ActionInfoBoxX, ActionInfoBoxY), ActionInfoBoxWidth, ActionInfoBoxHeight);
            g.DrawStringCentered(Map.fntArial12, "Command Selection", new Vector2(ActionInfoBoxX + ActionInfoBoxWidth / 2, ActionInfoBoxY + ActionInfoBoxHeight / 2), Color.White);

            float Scale = 0.52f;
            int DistanceBetweenCard = Constants.Width / 8;
            float CardsHeight = Constants.Height - 100;

            g.Draw(Map.sprArrowUp, new Vector2(Constants.Width / 2, CardsHeight - 95 + (float)Math.Sin(AnimationTime * 10) * 3f), Color.White);

            float X = DistanceBetweenCard;
            Color CardColor = Color.FromNonPremultiplied(255, 255, 255, 200);
            DrawCardMiniature(g, Map.sprTerritory, CardColor, ActionMenuCursor == 0, X, Scale, (float)AnimationTime, 0.05f);
            DrawCardMiniature(g, Map.sprMap, CardColor, ActionMenuCursor == 1, X += DistanceBetweenCard, Scale, (float)AnimationTime, 0.05f);
            DrawCardMiniature(g, Map.sprInfo, CardColor, ActionMenuCursor == 2, X += DistanceBetweenCard, Scale, (float)AnimationTime, 0.05f);
            DrawCardMiniature(g, Map.sprOptions, CardColor, ActionMenuCursor == 3, X += DistanceBetweenCard, Scale, (float)AnimationTime, 0.05f);
            DrawCardMiniature(g, Map.sprHelp, CardColor, ActionMenuCursor == 4, X += DistanceBetweenCard, Scale, (float)AnimationTime, 0.05f);
            DrawCardMiniature(g, Map.sprEndTurn, CardColor, ActionMenuCursor == 5, X += DistanceBetweenCard, Scale, (float)AnimationTime, 0.05f);

            int BoxWidth = (int)(Constants.Width / 2.8);
            int BoxHeight = (int)(Constants.Height / 2);
            float InfoBoxX = Constants.Width - Constants.Width / 12 - BoxWidth;
            float InfoBoxY = Constants.Height / 10;
            int TextWidth = BoxWidth - 60;

            MenuHelper.DrawNamedBox(g, "Command", new Vector2(InfoBoxX, InfoBoxY), BoxWidth, BoxHeight);
            TextHelper.DrawTextMultiline(g, Map.fntArial12, TextHelper.FitToWidth(Map.fntArial12, ListNextChoice[ActionMenuCursor].ToString(), TextWidth), TextHelper.TextAligns.Left, InfoBoxX + BoxWidth / 2, InfoBoxY + 5, TextWidth);
        }

        private static void DrawCardMiniature(CustomSpriteBatch g, Texture2D sprCard, Color CardFrontColor, bool Selected, float X, float MaxScale, float AnimationTime, float ExtraAnimationScale)
        {
            float Y = Constants.Height - Constants.Height / 6;

            float Scale = ExtraAnimationScale;
            if (Selected)
            {
                if (AnimationTime < MathHelper.PiOver2)
                {
                    Scale = AnimationTime / MathHelper.PiOver2 * ExtraAnimationScale;
                }
                else
                {
                    Scale = (MathHelper.Pi - AnimationTime) / MathHelper.PiOver2 * ExtraAnimationScale;
                }
            }

            float FinalScale = MaxScale + Scale;
            Card.DrawCardMiniatureCentered(g, sprCard, GameScreen.sprPixel, CardFrontColor, X, Y, -FinalScale, FinalScale, false);
        }
    }
}
