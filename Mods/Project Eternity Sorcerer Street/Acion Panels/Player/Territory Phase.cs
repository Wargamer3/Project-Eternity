using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelTerritoryMenuPhase : ActionPanelSorcererStreet
    {
        private const string PanelName = "TerritoryPhase";

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private TerrainSorcererStreet ActiveTerrain;

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
            ActiveTerrain = Map.GetTerrain(ActivePlayer.GamePiece);
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

                switch (Map.ListTerrainType[ActiveTerrain.TerrainTypeIndex])
                {
                    case TerrainSorcererStreet.Castle:
                    case TerrainSorcererStreet.NorthTower:
                    case TerrainSorcererStreet.SouthTower:
                    case TerrainSorcererStreet.EastTower:
                    case TerrainSorcererStreet.WestTower:
                        AddToPanelListAndSelect(new ActionPanelLandInfoPhase(Map, ActivePlayerIndex));
                        break;

                    default:
                        AddToPanelListAndSelect(new ActionPanelCreatureCardSelectionPhase(Map, ActivePlayerIndex));
                        break;
                }
            }
            else if (InputHelper.InputConfirmPressed())
            {
                if (ActionMenuCursor > 0 || (ActionMenuCursor == 0 && Map.ListPassedTerrein.Count > 0))
                {
                    AddToPanelListAndSelect(ListNextChoice[ActionMenuCursor]);
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
            return new ActionPanelTerritoryMenuPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            ActionPanelPlayerDefault.DrawPlayerInformation(g, Map, Map.ListAllPlayer[ActivePlayerIndex]);
            ActionPanelPlayerDefault.DrawPhase(g, Map, "Command Selection");

            float Scale = 0.52f;
            int DistanceBetweenCard = Constants.Width / 8;
            float CardsHeight = Constants.Height - 100;

            MenuHelper.DrawUpArrow(g);

            float X = DistanceBetweenCard;
            Color CardColor = Color.FromNonPremultiplied(255, 255, 255, 200);
            Color UnavailableColor = Color.FromNonPremultiplied(127, 127, 127, 200);
            if (Map.ListPassedTerrein.Count == 0)
            {
                DrawCardMiniature(g, Map.sprTerritory, UnavailableColor, ActionMenuCursor == 0, X, Scale, (float)AnimationTime, 0.05f);
            }
            else
            {
                DrawCardMiniature(g, Map.sprTerritory, CardColor, ActionMenuCursor == 0, X, Scale, (float)AnimationTime, 0.05f);
            }
            DrawCardMiniature(g, Map.sprMap, CardColor, ActionMenuCursor == 1, X += DistanceBetweenCard, Scale, (float)AnimationTime, 0.05f);
            DrawCardMiniature(g, Map.sprInfo, CardColor, ActionMenuCursor == 2, X += DistanceBetweenCard, Scale, (float)AnimationTime, 0.05f);
            DrawCardMiniature(g, Map.sprOptions, CardColor, ActionMenuCursor == 3, X += DistanceBetweenCard, Scale, (float)AnimationTime, 0.05f);
            DrawCardMiniature(g, Map.sprHelp, CardColor, ActionMenuCursor == 4, X += DistanceBetweenCard, Scale, (float)AnimationTime, 0.05f);
            DrawCardMiniature(g, Map.sprEndTurn, CardColor, ActionMenuCursor == 5, X += DistanceBetweenCard, Scale, (float)AnimationTime, 0.05f);

            MenuHelper.DrawFingerIcon(g, new Vector2(DistanceBetweenCard / 2 + DistanceBetweenCard * ActionMenuCursor, Constants.Height - Constants.Height / 6));

            int BoxWidth = (int)(Constants.Width / 2.8);
            int BoxHeight = (int)(Constants.Height / 2);
            float InfoBoxX = Constants.Width - Constants.Width / 12 - BoxWidth;
            float InfoBoxY = Constants.Height / 10;
            int TextWidth = BoxWidth - 60;

            MenuHelper.DrawNamedBox(g, "Command", new Vector2(InfoBoxX, InfoBoxY), BoxWidth, BoxHeight);
            TextHelper.DrawTextMultiline(g, Map.fntMenuText, TextHelper.FitToWidth(Map.fntMenuText, ListNextChoice[ActionMenuCursor].ToString(), TextWidth), TextHelper.TextAligns.Left, InfoBoxX + BoxWidth / 2, InfoBoxY + 5, TextWidth, SorcererStreetMap.TextColor);
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
