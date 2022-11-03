using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelTerritoryMenuPhase : ActionPanelSorcererStreet
    {
        private const string PanelName = "TerritoryPhase";

        private int ActivePlayerIndex;
        private Player ActivePlayer;

        private double AnimationTime;

        public ActionPanelTerritoryMenuPhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelTerritoryMenuPhase(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void OnSelect()
        {
            AddChoiceToCurrentPanel(new ActionPanelChooseTerritory(Map, ActivePlayerIndex));
            AddChoiceToCurrentPanel(new ActionPanelViewMap(Map));
            AddChoiceToCurrentPanel(new ActionPanelInfo(Map));
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
            float CardsHeight = Constants.Height - 100;
            g.Draw(Map.sprArrowUp, new Vector2(Constants.Width / 2, CardsHeight - 95 + (float)Math.Sin(AnimationTime * 10) * 3f), Color.White);

            float X = 80;
            Color CardColor = Color.FromNonPremultiplied(255, 255, 255, 200);
            DrawCardMiniature(g, Map.sprTerritory, CardColor, ActionMenuCursor == 0, X, 0.30f, (float)AnimationTime, 0.05f);
            DrawCardMiniature(g, Map.sprMap, CardColor, ActionMenuCursor == 1, X += 80, 0.30f, (float)AnimationTime, 0.05f);
            DrawCardMiniature(g, Map.sprInfo, CardColor, ActionMenuCursor == 2, X += 80, 0.30f, (float)AnimationTime, 0.05f);
            DrawCardMiniature(g, Map.sprOptions, CardColor, ActionMenuCursor == 3, X += 80, 0.30f, (float)AnimationTime, 0.05f);
            DrawCardMiniature(g, Map.sprHelp, CardColor, ActionMenuCursor == 4, X += 80, 0.30f, (float)AnimationTime, 0.05f);
            DrawCardMiniature(g, Map.sprEndTurn, CardColor, ActionMenuCursor == 5, X += 80, 0.30f, (float)AnimationTime, 0.05f);
        }

        private static void DrawCardMiniature(CustomSpriteBatch g, Texture2D sprCard, Color CardFrontColor, bool Selected, float X, float MaxScale, float AnimationTime, float ExtraAnimationScale)
        {
            float Y = Constants.Height - 100;

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
            Card.DrawCardMiniatureCentered(g, sprCard, GameScreen.sprPixel, CardFrontColor, X, Y, -FinalScale, FinalScale, MathHelper.Pi + MathHelper.PiOver2);
        }
    }
}
