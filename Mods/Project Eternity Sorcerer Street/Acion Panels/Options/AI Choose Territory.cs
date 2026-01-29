using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelAIChooseTerritory : ActionPanelViewMap
    {
        private const string PanelName = "ChooseAITerritory";

        public enum AITerritoryActions { LevelUp, CreatureMovement, CreatureExchange, TerritoryAbility }

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        protected TerrainSorcererStreet AISelectedTerrain;
        private readonly AITerritoryActions AITerritoryAction;
        private double AITimer;

        public ActionPanelAIChooseTerritory(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelAIChooseTerritory(SorcererStreetMap Map, int ActivePlayerIndex, TerrainSorcererStreet AISelectedTerrain, AITerritoryActions AITerritoryAction)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.AISelectedTerrain = AISelectedTerrain;
            this.AITerritoryAction = AITerritoryAction;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void DoUpdate(GameTime gameTime)
        {
            UpdateCamera();
            SetTarget();

            bool TerrainSelected = true;
            Vector3 TilePosition = new Vector3(AISelectedTerrain.WorldPosition.X, AISelectedTerrain.WorldPosition.Z, AISelectedTerrain.WorldPosition.Y);

            Vector3 Tile3DPositionIn2DCenter = GameScreen.GraphicsDevice.Viewport.Project(TilePosition + new Vector3(Map.TileSize.X / 2f, 0, Map.TileSize.Y / 2f), Camera.Projection, Camera.View, Matrix.Identity);
            Vector3 Tile3DPositionIn2DTopLeft = GameScreen.GraphicsDevice.Viewport.Project(TilePosition, Camera.Projection, Camera.View, Matrix.Identity);
            Vector3 Tile3DPositionIn2DBottomRight = GameScreen.GraphicsDevice.Viewport.Project(TilePosition + new Vector3(Map.TileSize.X, 0, Map.TileSize.Y), Camera.Projection, Camera.View, Matrix.Identity);
            Vector3 Tile3DPositionIn2DTopRight = GameScreen.GraphicsDevice.Viewport.Project(TilePosition + new Vector3(Map.TileSize.X, 0, 0), Camera.Projection, Camera.View, Matrix.Identity);
            Vector3 Tile3DPositionIn2DBottomLeft = GameScreen.GraphicsDevice.Viewport.Project(TilePosition + new Vector3(0, 0, Map.TileSize.Y), Camera.Projection, Camera.View, Matrix.Identity);

            float MinDistanceLeft = Tile3DPositionIn2DCenter.X - Tile3DPositionIn2DTopLeft.X;
            float MinDistanceRight = Tile3DPositionIn2DBottomRight.X - Tile3DPositionIn2DCenter.X;
            float MinDistanceUp = Tile3DPositionIn2DCenter.Y - Tile3DPositionIn2DTopLeft.Y;
            float MinDistanceDown = Tile3DPositionIn2DBottomRight.Y - Tile3DPositionIn2DCenter.Y;

            float CursorSpeed = 0.3f;
            if (CursorPosition.X < Tile3DPositionIn2DCenter.X - MinDistanceLeft * 0.4f)
            {
                CursorPosition.X += (float)(CursorSpeed * gameTime.ElapsedGameTime.TotalMilliseconds);
                TerrainSelected = false;
            }
            else if (CursorPosition.X > Tile3DPositionIn2DCenter.X + MinDistanceRight * 0.4f)
            {
                CursorPosition.X -= (float)(CursorSpeed * gameTime.ElapsedGameTime.TotalMilliseconds);
                TerrainSelected = false;
            }
            if (CursorPosition.Y < Tile3DPositionIn2DCenter.Y - MinDistanceUp * 0.4f)
            {
                CursorPosition.Y += (float)(CursorSpeed * gameTime.ElapsedGameTime.TotalMilliseconds);
                TerrainSelected = false;
            }
            else if (CursorPosition.Y > Tile3DPositionIn2DCenter.Y + MinDistanceDown * 0.4f)
            {
                CursorPosition.Y -= (float)(CursorSpeed * gameTime.ElapsedGameTime.TotalMilliseconds);
                TerrainSelected = false;
            }

            if (TerrainSelected)
            {
                AITimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (AITimer > 0.6f)
                {
                    AddToPanelListAndSelect(new ActionPanelAITerritoryActions(Map, ActivePlayerIndex, AISelectedTerrain, AITerritoryAction));
                }
            }
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAIChooseTerritory(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);
            foreach (TerrainSorcererStreet ActiveTerrain in Map.ListPassedTerrain)
            {
            }
        }

        public override string ToString()
        {
            return "You can level up your land you've passed through and change terrain. When you use this command, your turn will end.";
        }
    }
}
