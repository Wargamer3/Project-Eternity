using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using static ProjectEternity.GameScreens.SorcererStreetScreen.Player;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelMovementPhase : ActionPanelSorcererStreet
    {
        private Player ActivePlayer;
        private int Movement;
        private ActionPanelChooseDirection DirectionPicker;

        public ActionPanelMovementPhase(SorcererStreetMap Map, Player ActivePlayer, int Movement)
            : base("Move Player", Map, false)
        {
            this.ActivePlayer = ActivePlayer;
            this.Movement = Movement;
        }

        public override void OnSelect()
        {
            PrepareToMoveToNextTerrain(ActivePlayer.GamePiece.Position, ActivePlayer.CurrentDirection == Directions.None);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (DirectionPicker != null && DirectionPicker.ChosenTerrain != null)
            {
                MoveToNextTerrain(DirectionPicker.ChosenTerrain);
                DirectionPicker = null;
            }

            if (Map.MovementAnimation.Count == 0)
            {
                if (Movement > 0)
                {
                    PrepareToMoveToNextTerrain(ActivePlayer.GamePiece.Position, true);
                }
                else
                {
                    RemoveFromPanelList(this);
                    TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(ActivePlayer.GamePiece);

                    ActiveTerrain.OnSelect(Map, ActivePlayer);
                }
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Draw remaining movement count in the upper left corner
            GameScreen.DrawBox(g, new Vector2(30, 30), 50, 50, Color.Black);
            g.DrawString(Map.fntArial12, Movement.ToString(), new Vector2(37, 35), Color.White);
        }

        private void MoveToNextTerrain(TerrainSorcererStreet NextTerrain)
        {
            Map.MovementAnimation.Add(ActivePlayer.GamePiece.X, ActivePlayer.GamePiece.Y, ActivePlayer.GamePiece);
            Vector3 FinalPosition = NextTerrain.Position;
            ActivePlayer.GamePiece.SetPosition(FinalPosition);

            --Movement;
        }

        private void PrepareToMoveToNextTerrain(Vector3 PlayerPosition, bool AllowDirectionChange)
        {
            Dictionary<Directions, TerrainSorcererStreet> DicNextTerrain = GetNextTerrains(Map, (int)PlayerPosition.X, (int)PlayerPosition.Y, ActivePlayer.CurrentDirection);

            if (!AllowDirectionChange)
            {
                MoveToNextTerrain(DicNextTerrain[ActivePlayer.CurrentDirection]);
            }
            else if (DicNextTerrain.Count > 1)
            {
                DirectionPicker = new ActionPanelChooseDirection(Map, ActivePlayer, DicNextTerrain);
                AddToPanelListAndSelect(DirectionPicker);
            }
            else
            {
                MoveToNextTerrain(DicNextTerrain.First().Value);
            }
        }

        private static Dictionary<Directions, TerrainSorcererStreet> GetNextTerrains(SorcererStreetMap Map, int ActiveTerrainX, int ActiveTerrainY, Directions PlayerDirection)
        {
            Dictionary<Directions, TerrainSorcererStreet> DicNextTerrain = new Dictionary<Directions, TerrainSorcererStreet>();

            if (ActiveTerrainY - 1 >= 0)
            {
                DicNextTerrain.Add(Directions.Up, Map.GetTerrain(ActiveTerrainX, ActiveTerrainY - 1, Map.ActiveLayerIndex));
            }
            if (ActiveTerrainY + 1 < Map.MapSize.Y)
            {
                DicNextTerrain.Add(Directions.Down, Map.GetTerrain(ActiveTerrainX, ActiveTerrainY + 1, Map.ActiveLayerIndex));
            }
            if (ActiveTerrainX - 1 >= 0)
            {
                DicNextTerrain.Add(Directions.Left, Map.GetTerrain(ActiveTerrainX - 1, ActiveTerrainY, Map.ActiveLayerIndex));
            }
            if (ActiveTerrainX + 1 < Map.MapSize.X)
            {
                DicNextTerrain.Add(Directions.Right, Map.GetTerrain(ActiveTerrainX + 1, ActiveTerrainY, Map.ActiveLayerIndex));
            }

            Directions[] TerrainDirections = DicNextTerrain.Keys.ToArray();
            foreach (Directions ActiveDirection in TerrainDirections)
            {
                if (PlayerDirection == Directions.Left && ActiveDirection == Directions.Right)
                {
                    DicNextTerrain.Remove(ActiveDirection);
                }
                else if (PlayerDirection == Directions.Right && ActiveDirection == Directions.Left)
                {
                    DicNextTerrain.Remove(ActiveDirection);
                }
                else if (PlayerDirection == Directions.Up && ActiveDirection == Directions.Down)
                {
                    DicNextTerrain.Remove(ActiveDirection);
                }
                else if (PlayerDirection == Directions.Down && ActiveDirection == Directions.Up)
                {
                    DicNextTerrain.Remove(ActiveDirection);
                }
                else if (DicNextTerrain[ActiveDirection].TerrainTypeIndex == 0)
                {
                    DicNextTerrain.Remove(ActiveDirection);
                }
            }

            return DicNextTerrain;
        }
    }

    public class ActionPanelChooseDirection : ActionPanelSorcererStreet
    {
        public TerrainSorcererStreet ChosenTerrain { get { return ActivePlayer.CurrentDirection == Directions.None ? null : DicNextTerrain[ActivePlayer.CurrentDirection]; } }

        private readonly Player ActivePlayer;
        private readonly Dictionary<Directions, TerrainSorcererStreet> DicNextTerrain;

        public ActionPanelChooseDirection(SorcererStreetMap Map, Player ActivePlayer, Dictionary<Directions, TerrainSorcererStreet> DicNextTerrain)
            : base("Choose Direction", Map, false)
        {
            this.ActivePlayer = ActivePlayer;
            this.DicNextTerrain = DicNextTerrain;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputUpPressed() && DicNextTerrain.ContainsKey(Directions.Up))
            {
                ActivePlayer.CurrentDirection = Directions.Up;
            }
            else if (InputHelper.InputDownPressed() && DicNextTerrain.ContainsKey(Directions.Down))
            {
                ActivePlayer.CurrentDirection = Directions.Down;
            }
            else if (InputHelper.InputLeftPressed() && DicNextTerrain.ContainsKey(Directions.Left))
            {
                ActivePlayer.CurrentDirection = Directions.Left;
            }
            else if (InputHelper.InputRightPressed() && DicNextTerrain.ContainsKey(Directions.Right))
            {
                ActivePlayer.CurrentDirection = Directions.Right;
            }
            else if (InputHelper.InputConfirmPressed() && ActivePlayer.CurrentDirection != Directions.None)
            {
                FinalizeChoice();
            }
        }

        public void FinalizeChoice()
        {
            RemoveFromPanelList(this);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (DicNextTerrain.Keys.Contains(Directions.Right))
            {
                if (ActivePlayer.CurrentDirection == Directions.Right)
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle((int)ActivePlayer.GamePiece.X + 32, (int)ActivePlayer.GamePiece.Y, 20, 20), Color.Red);
                }
                else
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle((int)ActivePlayer.GamePiece.X + 32, (int)ActivePlayer.GamePiece.Y, 20, 20), Color.AliceBlue);
                }
            }
            if (DicNextTerrain.Keys.Contains(Directions.Down))
            {
                if (ActivePlayer.CurrentDirection == Directions.Down)
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle((int)ActivePlayer.GamePiece.X, (int)ActivePlayer.GamePiece.Y + 32, 20, 20), Color.Red);
                }
                else
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle((int)ActivePlayer.GamePiece.X, (int)ActivePlayer.GamePiece.Y + 32, 20, 20), Color.AliceBlue);
                }
            }
        }
    }

    public class ActionPanelPayTollPhase : ActionPanelCardSelectionPhase
    {
        private readonly Player ActivePlayer;
        private readonly TerrainSorcererStreet ActiveTerrain;
        
        public ActionPanelPayTollPhase(SorcererStreetMap Map, Player ActivePlayer, TerrainSorcererStreet ActiveTerrain)
            : base(Map.ListActionMenuChoice, Map, ActivePlayer, CreatureCard.CreatureCardType)
        {
            this.ActivePlayer = ActivePlayer;
            this.ActiveTerrain = ActiveTerrain;
        }

        public override void OnCardSelected(Card CardSelected)
        {
            RemoveAllActionPanels();
            Map.PushScreen(new ActionPanelBattleStartPhase(Map, ActivePlayer, (CreatureCard)CardSelected));
        }

        public override void OnEndCardSelected()
        {
            ActivePlayer.Magic -= 10;
            RemoveAllActionPanels();
            Map.EndPlayerPhase();
        }
    }

    public class ActionPanelDiscardCardPhase : ActionPanelSorcererStreet
    {
        private readonly Player ActivePlayer;
        private readonly int MaximumCardsAllowed;

        private int CardCursorIndex;

        public ActionPanelDiscardCardPhase(SorcererStreetMap Map, Player ActivePlayer, int MaximumCardsAllowed)
            : base("Discard Card", Map, false)
        {
            this.ActivePlayer = ActivePlayer;
            this.MaximumCardsAllowed = MaximumCardsAllowed;
        }

        public override void OnSelect()
        {
            CardCursorIndex = 0;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputLeftPressed() && --CardCursorIndex < 0)
            {
                CardCursorIndex = ActivePlayer.ListCardInHand.Count - 1;
            }
            else if (InputHelper.InputRightPressed() && ++CardCursorIndex >= ActivePlayer.ListCardInHand.Count)
            {
                CardCursorIndex = 0;
            }
            else if (InputHelper.InputConfirmPressed())
            {
                ActivePlayer.ListCardInHand.RemoveAt(CardCursorIndex);

                if (ActivePlayer.ListCardInHand.Count <= MaximumCardsAllowed)
                {
                    RemoveFromPanelList(this);
                }
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
