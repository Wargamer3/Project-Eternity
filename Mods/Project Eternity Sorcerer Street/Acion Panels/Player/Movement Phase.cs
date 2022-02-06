using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using static ProjectEternity.GameScreens.SorcererStreetScreen.Player;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelMovementPhase : ActionPanelSorcererStreet
    {
        private const string PanelName = "MovePlayer";

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private int Movement;
        private ActionPanelChooseDirection DirectionPicker;

        public ActionPanelMovementPhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelMovementPhase(SorcererStreetMap Map, int ActivePlayerIndex, int Movement)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            this.Movement = Movement;
        }

        public override void OnSelect()
        {
            PrepareToMoveToNextTerrain(ActivePlayer.GamePiece.Position, (int)ActivePlayer.GamePiece.Position.Z, ActivePlayer.CurrentDirection == Directions.None);
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
                    PrepareToMoveToNextTerrain(ActivePlayer.GamePiece.Position, (int)ActivePlayer.GamePiece.Position.Z, true);
                }
                else
                {
                    RemoveFromPanelList(this);
                    TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(ActivePlayer.GamePiece);

                    ActiveTerrain.OnSelect(Map, ActivePlayerIndex);
                }
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            Movement = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(Movement);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelMovementPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Draw remaining movement count in the upper left corner
            GameScreen.DrawBox(g, new Vector2(30, 30), 50, 50, Color.Black);
            g.DrawString(Map.fntArial12, Movement.ToString(), new Vector2(37, 35), Color.White);
        }

        private void MoveToNextTerrain(TerrainSorcererStreet NextTerrain)
        {
            Vector3 FinalPosition = NextTerrain.Position;
            Map.MovementAnimation.Add(ActivePlayer.GamePiece, ActivePlayer.GamePiece.Position, FinalPosition);
            ActivePlayer.GamePiece.SetPosition(FinalPosition);

            --Movement;
        }

        private void PrepareToMoveToNextTerrain(Vector3 PlayerPosition, int LayerIndex, bool AllowDirectionChange)
        {
            Dictionary<Directions, TerrainSorcererStreet> DicNextTerrain = GetNextTerrains(Map, (int)PlayerPosition.X, (int)PlayerPosition.Y, LayerIndex, ActivePlayer.CurrentDirection);

            if (!AllowDirectionChange)
            {
                MoveToNextTerrain(DicNextTerrain[ActivePlayer.CurrentDirection]);
            }
            else if (DicNextTerrain.Count > 1)
            {
                DirectionPicker = new ActionPanelChooseDirection(Map, ActivePlayerIndex, DicNextTerrain);
                AddToPanelListAndSelect(DirectionPicker);
            }
            else
            {
                MoveToNextTerrain(DicNextTerrain.First().Value);
            }
        }

        private static Dictionary<Directions, TerrainSorcererStreet> GetNextTerrains(SorcererStreetMap Map, int ActiveTerrainX, int ActiveTerrainY, int LayerIndex, Directions PlayerDirection)
        {
            Dictionary<Directions, TerrainSorcererStreet> DicNextTerrain = new Dictionary<Directions, TerrainSorcererStreet>();

            if (ActiveTerrainY - 1 >= 0)
            {
                DicNextTerrain.Add(Directions.Up, Map.GetTerrain(ActiveTerrainX, ActiveTerrainY - 1, LayerIndex));
            }
            if (ActiveTerrainY + 1 < Map.MapSize.Y)
            {
                DicNextTerrain.Add(Directions.Down, Map.GetTerrain(ActiveTerrainX, ActiveTerrainY + 1, LayerIndex));
            }
            if (ActiveTerrainX - 1 >= 0)
            {
                DicNextTerrain.Add(Directions.Left, Map.GetTerrain(ActiveTerrainX - 1, ActiveTerrainY, LayerIndex));
            }
            if (ActiveTerrainX + 1 < Map.MapSize.X)
            {
                DicNextTerrain.Add(Directions.Right, Map.GetTerrain(ActiveTerrainX + 1, ActiveTerrainY, LayerIndex));
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
}
