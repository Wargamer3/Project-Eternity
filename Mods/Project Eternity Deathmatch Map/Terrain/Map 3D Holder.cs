using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class Map3DDrawable : ILayerHolderDrawable
    {
        protected Point MapSize { get { return Map.MapSize; } }

        private DeathmatchMap Map;

        private Effect effect;
        private BasicEffect PolygonEffect;
        private DefaultCamera Camera;
        private Texture2D sprCursor;
        private Tile3D Cursor;
        private Dictionary<int, Tile3DHolder> DicTile3DByTileset;
        private Dictionary<Color, List<Tile3D>> DicDrawablePointPerColor;
        private List<Tile3D> ListDrawableArrowPerColor;

        public Map3DDrawable(DeathmatchMap Map, GraphicsDevice g)
        {
            this.Map = Map;
            sprCursor = Map.sprCursor;
            Camera = new DefaultCamera(g);

            effect = Map.Content.Load<Effect>("Shaders/Default Shader 3D");

            PolygonEffect = new BasicEffect(g);


            PolygonEffect.TextureEnabled = true;
            PolygonEffect.EnableDefaultLighting();

            float aspectRatio = g.Viewport.Width / (float)g.Viewport.Height;

            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    1, 10000);
            PolygonEffect.Projection = Projection;

            PolygonEffect.World = Matrix.Identity;
            PolygonEffect.View = Matrix.Identity;

            // Key light.
            effect.Parameters["DirLight0Direction"].SetValue(new Vector3(-0.5265408f, -0.5735765f, -0.6275069f));
            effect.Parameters["DirLight0DiffuseColor"].SetValue(new Vector3(1, 0.9607844f, 0.8078432f));
            effect.Parameters["DirLight0SpecularColor"].SetValue(new Vector3(1, 0.9607844f, 0.8078432f));

            // Fill light.
            effect.Parameters["DirLight1Direction"].SetValue(new Vector3(0.7198464f, 0.3420201f, 0.6040227f));
            effect.Parameters["DirLight1DiffuseColor"].SetValue(new Vector3(0.9647059f, 0.7607844f, 0.4078432f));
            effect.Parameters["DirLight1SpecularColor"].SetValue(Vector3.Zero);

            // Back light.
            effect.Parameters["DirLight2Direction"].SetValue(new Vector3(0.4545195f, -0.7660444f, 0.4545195f));
            effect.Parameters["DirLight2DiffuseColor"].SetValue(new Vector3(0.3231373f, 0.3607844f, 0.3937255f));
            effect.Parameters["DirLight2SpecularColor"].SetValue(new Vector3(0.3231373f, 0.3607844f, 0.3937255f));

            Vector3 diffuseColor = Vector3.One;
            Vector3 emissiveColor = Vector3.Zero;
            Vector3 ambientLightColor = new Vector3(0.05333332f, 0.09882354f, 0.1819608f);
            Vector4 diffuse = new Vector4();
            Vector3 emissive = new Vector3();
            float alpha = 1;
            diffuse.X = diffuseColor.X * alpha;
            diffuse.Y = diffuseColor.Y * alpha;
            diffuse.Z = diffuseColor.Z * alpha;
            diffuse.W = alpha;

            emissive.X = (emissiveColor.X + ambientLightColor.X * diffuseColor.X) * alpha;
            emissive.Y = (emissiveColor.Y + ambientLightColor.Y * diffuseColor.Y) * alpha;
            emissive.Z = (emissiveColor.Z + ambientLightColor.Z * diffuseColor.Z) * alpha;

            effect.Parameters["DiffuseColor"].SetValue(diffuse);
            effect.Parameters["EmissiveColor"].SetValue(emissive);
            effect.Parameters["SpecularColor"].SetValue(Vector3.One);
            effect.Parameters["SpecularPower"].SetValue(64);


            DicDrawablePointPerColor = new Dictionary<Color, List<Tile3D>>();
            DicTile3DByTileset = new Dictionary<int, Tile3DHolder>();
            ListDrawableArrowPerColor = new List<Tile3D>();

            for (int L = 0; L < Map.LayerManager.ListLayer.Count; L++)
            {
                CreateMap(Map, Map.LayerManager.ListLayer[L], L);
            }

            foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByTileset)
            {
                ActiveTileSet.Value.Finish(GameScreen.GraphicsDevice);
            }

            float Z = Map.LayerManager.ListLayer[0].ArrayTerrain[0, 0].Position.Z * 32;
            Map2D GroundLayer = Map.LayerManager.ListLayer[0].LayerGrid;
            DrawableTile ActiveTerrain = GroundLayer.GetTile(0, 0);
            Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;
            Cursor = ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                0, 0, Z, 0, Map.TileSize, new List<Texture2D>() { sprCursor }, Z, Z, Z, Z, 0)[0];
        }

        protected void CreateMap(DeathmatchMap Map, MapLayer Owner, int LayerIndex)
        {
            Map2D GroundLayer = Owner.LayerGrid;

            for (int X = Map.MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
                {
                    DrawableTile ActiveTerrain = GroundLayer.GetTile(X, Y);
                    Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;
                    if (ActiveTerrain3D.TerrainStyle == Terrain3D.TerrainStyles.Invisible)
                    {
                        continue;
                    }

                    float Z = Owner.ArrayTerrain[X, Y].Position.Z * 32 + (LayerIndex * 32);
                    float ZFront = Z;
                    float ZBack = Z;
                    float ZRight = Z;
                    float ZLeft = Z;
                    if (Y + 1 < Map.MapSize.Y)
                    {
                        ZFront = Owner.ArrayTerrain[X, Y + 1].Position.Z * 32 + (LayerIndex * 32);
                    }
                    if (Y - 1 >= 0)
                    {
                        ZBack = Owner.ArrayTerrain[X, Y - 1].Position.Z * 32 + (LayerIndex * 32);
                    }
                    if (X - 1 >= 0)
                    {
                        ZLeft = Owner.ArrayTerrain[X - 1, Y].Position.Z * 32 + (LayerIndex * 32);
                    }
                    if (X + 1 < Map.MapSize.X)
                    {
                        ZRight = Owner.ArrayTerrain[X + 1, Y].Position.Z * 32 + (LayerIndex * 32);
                    }

                    List<Tile3D> ListNew3DTile = ActiveTerrain3D.CreateTile3D(ActiveTerrain.TilesetIndex, ActiveTerrain.Origin.Location,
                                            X * Map.TileSize.X, Y * Map.TileSize.Y, Z, LayerIndex * 32, Map.TileSize, Map.ListTileSet, ZFront, ZBack, ZLeft, ZRight, 0);

                    foreach (Tile3D ActiveTile in ListNew3DTile)
                    {
                        if (!DicTile3DByTileset.ContainsKey(ActiveTile.TilesetIndex))
                        {
                            DicTile3DByTileset.Add(ActiveTile.TilesetIndex, new Tile3DHolder(effect, Map.ListTileSet[ActiveTile.TilesetIndex]));
                        }
                        DicTile3DByTileset[ActiveTile.TilesetIndex].AddTile(ActiveTile);
                    }
                }
            }

            for (int L = 0; L < Owner.ListSubLayer.Count; L++)
            {
                CreateMap(Map, Owner.ListSubLayer[L], LayerIndex);
            }
        }

        public void Update(GameTime gameTime)
        {
            UpdateCamera();

            if (Map.CursorPositionVisible.X < 0)
            {
                Camera.CameraHeight = 600;
                Camera.CameraDistance = 500;
                Camera.SetTarget(new Vector3(Map.TileSize.X * Map.MapSize.X / 2, Map.CursorPosition.Z * 32, Map.TileSize.Y * Map.MapSize.Y / 2));
                Camera.Update(gameTime);
                return;
            }

            Camera.CameraHeight = 400;
            Camera.CameraDistance = 300;
            int X = (int)Map.CursorPositionVisible.X;
            int Y = (int)Map.CursorPositionVisible.Y;
            float Z = Map.LayerManager.ListLayer[(int)Map.CursorPosition.Z].ArrayTerrain[X, Y].Position.Z * 32 + (Map.CursorPosition.Z * 32) + 0.3f;
            Map2D GroundLayer = Map.LayerManager.ListLayer[(int)Map.CursorPosition.Z].LayerGrid;
            DrawableTile ActiveTerrain = GroundLayer.GetTile(X, Y);
            Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;
            Cursor = ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                X * Map.TileSize.X, Y * Map.TileSize.Y, Z, Map.CursorPosition.Z * 32 + 0.3f, Map.TileSize, new List<Texture2D>() { sprCursor }, Z, Z, Z, Z, 0)[0];

            Camera.SetTarget(new Vector3(Map.TileSize.X * Map.CursorPositionVisible.X, Map.CursorPosition.Z * 32, Map.TileSize.Y * Map.CursorPositionVisible.Y));
            Camera.Update(gameTime);

            DicDrawablePointPerColor.Clear();
            ListDrawableArrowPerColor.Clear();
        }

        private void UpdateCamera()
        {
            if (KeyboardHelper.KeyPressed(Keys.Q))
            {
                float NextZ = Map.CursorPosition.Z;

                if (KeyboardHelper.KeyHold(Keys.LeftAlt))
                {
                    NextZ += 1f;
                }
                else
                {
                    do
                    {
                        NextZ += 1f;
                    }
                    while (NextZ < Map.LayerManager.ListLayer.Count
                        && Map.LayerManager.ListLayer[(int)NextZ].ArrayTerrain[(int)Map.CursorPosition.X, (int)Map.CursorPosition.Y].TerrainTypeIndex == UnitStats.TerrainVoidIndex);

                    if (NextZ >= Map.LayerManager.ListLayer.Count)
                    {
                        NextZ = Map.CursorPosition.Z + 1f;
                    }
                }

                if (NextZ >= Map.LayerManager.ListLayer.Count)
                {
                    NextZ = Map.LayerManager.ListLayer.Count - 1;
                }

                Map.CursorPosition.Z = NextZ;
            }

            if (KeyboardHelper.KeyPressed(Keys.E))
            {
                float NextZ = Map.CursorPosition.Z;

                if (KeyboardHelper.KeyHold(Keys.LeftAlt))
                {
                    NextZ -= 1f;
                }
                else
                {
                    do
                    {
                        NextZ -= 1f;
                    }
                    while (NextZ >= 0
                        && Map.LayerManager.ListLayer[(int)NextZ].ArrayTerrain[(int)Map.CursorPosition.X, (int)Map.CursorPosition.Y].TerrainTypeIndex == UnitStats.TerrainVoidIndex);

                    if (NextZ < 0)
                    {
                        NextZ = Map.CursorPosition.Z - 1f;
                    }
                }

                if (NextZ < 0)
                {
                    NextZ = 0;
                }

                Map.CursorPosition.Z = NextZ;
            }
        }

        public void AddDrawablePoints(List<MovementAlgorithmTile> ListPoint, Color PointColor)
        {
            List<Tile3D> ListDrawablePoint3D = new List<Tile3D>(ListPoint.Count);

            foreach (MovementAlgorithmTile ActivePoint in ListPoint)
            {
                int X = (int)ActivePoint.Position.X;
                int Y = (int)ActivePoint.Position.Y;
                float Z = Map.LayerManager.ListLayer[ActivePoint.LayerIndex].ArrayTerrain[X, Y].Position.Z * 32 + (ActivePoint.LayerIndex * 32) + 0.1f;
                Map2D GroundLayer = Map.LayerManager.ListLayer[ActivePoint.LayerIndex].LayerGrid;
                DrawableTile ActiveTerrain = GroundLayer.GetTile(X, Y);
                Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;

                ListDrawablePoint3D.Add(ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                X * Map.TileSize.X, Y * Map.TileSize.Y, Z, Z + 0.1f, Map.TileSize, new List<Texture2D>() { sprCursor }, Z, Z, Z, Z, 0)[0]);
            }

            DicDrawablePointPerColor.Add(PointColor, ListDrawablePoint3D);
        }

        public void AddDrawablePath(List<MovementAlgorithmTile> ListPoint)
        {
            for (int P = 1; P < ListPoint.Count; P++)
            {
                MovementAlgorithmTile ActivePoint = ListPoint[P];
                MovementAlgorithmTile Previous = ListPoint[P - 1];
                MovementAlgorithmTile Next = null;
                if (P + 1 < ListPoint.Count)
                {
                    Next = ListPoint[P + 1];
                }

                int X = (int)ActivePoint.Position.X;
                int Y = (int)ActivePoint.Position.Y;
                float Z = Map.LayerManager.ListLayer[ActivePoint.LayerIndex].ArrayTerrain[X, Y].Position.Z * 32 + (ActivePoint.LayerIndex * 32) + 0.1f;
                Map2D GroundLayer = Map.LayerManager.ListLayer[ActivePoint.LayerIndex].LayerGrid;
                DrawableTile ActiveTerrain = GroundLayer.GetTile(X, Y);
                Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;

                ListDrawableArrowPerColor.Add(ActiveTerrain3D.CreateTile3D(0, GetCursorTextureOffset(Previous, ActivePoint, Next),
                X * Map.TileSize.X, Y * Map.TileSize.Y, Z, Z + 0.15f, Map.TileSize, new List<Texture2D>() { Map.sprCursorPath }, Z, Z, Z, Z, 0)[0]);
            }
        }

        private void DrawDrawablePoints(CustomSpriteBatch g)
        {
            PolygonEffect.Texture = GameScreen.sprPixel;
            PolygonEffect.CurrentTechnique.Passes[0].Apply();

            foreach (KeyValuePair<Color, List<Tile3D>> DrawablePointPerColor in DicDrawablePointPerColor)
            {
                foreach (Tile3D DrawablePoint in DrawablePointPerColor.Value)
                {
                    DrawablePoint.Draw(g.GraphicsDevice);
                }
            }

            PolygonEffect.Texture = Map.sprCursorPath;
            PolygonEffect.CurrentTechnique.Passes[0].Apply();

            foreach (Tile3D DrawablePoint in ListDrawableArrowPerColor)
            {
                DrawablePoint.Draw(g.GraphicsDevice);
            }
        }

        private Point GetCursorTextureOffset(MovementAlgorithmTile Previous, MovementAlgorithmTile Current, MovementAlgorithmTile Next)
        {
            Point TextureOffset = Point.Zero;
            if (Next == null)
            {
                if (Previous != null && Previous.Position.X < Current.Position.X)//Right
                {
                    TextureOffset.X = 32 * 3;
                }
                else if (Previous != null && Previous.Position.X > Current.Position.X)//Left
                {
                    TextureOffset.X = 32 * 4;
                }
                else if (Previous != null && Previous.Position.Y < Current.Position.Y)//Down
                {
                    TextureOffset.X = 32 * 5;
                }
                else if (Previous != null && Previous.Position.Y > Current.Position.Y)//Up
                {
                    TextureOffset.X = 32 * 2;
                }
            }
            else if (Current.Position.X < Next.Position.X)//Going Right
            {
                if (Previous != null && Previous.Position.Y < Current.Position.Y)//Down Right
                {
                    TextureOffset.X = 32 * 9;
                }
                else if (Previous != null && Previous.Position.Y > Current.Position.Y)//Up Right
                {
                    TextureOffset.X = 32 * 8;
                }
                else
                {
                    TextureOffset.X = 32 * 1;
                }
            }
            else if (Current.Position.X > Next.Position.X)//Going Left
            {
                if (Previous != null && Previous.Position.Y < Current.Position.Y)//Down Left
                {
                    TextureOffset.X = 32 * 7;
                }
                else if (Previous != null && Previous.Position.Y > Current.Position.Y)//Up Left
                {
                    TextureOffset.X = 32 * 6;
                }
                else
                {
                    TextureOffset.X = 32 * 1;
                }
            }
            else if (Current.Position.Y < Next.Position.Y)//Going Down
            {
                if (Previous != null && Previous.Position.X < Current.Position.X)//Right Down
                {
                    TextureOffset.X = 32 * 6;
                }
                else if (Previous != null && Previous.Position.X > Current.Position.X)//Left Down
                {
                    TextureOffset.X = 32 * 8;
                }
                else
                {
                    TextureOffset.X = 32 * 0;
                }
            }
            else if (Current.Position.Y > Next.Position.Y)//Going Up
            {
                if (Previous != null && Previous.Position.X < Current.Position.X)//Right Up
                {
                    TextureOffset.X = 32 * 7;
                }
                else if (Previous != null && Previous.Position.X > Current.Position.X)//Left Up
                {
                    TextureOffset.X = 32 * 9;
                }
                else
                {
                    TextureOffset.X = 32 * 0;
                }
            }

            return TextureOffset;
        }

        public void Reset()
        {
            DicTile3DByTileset.Clear();

            for (int L = 0; L < Map.LayerManager.ListLayer.Count; L++)
            {
                CreateMap(Map, Map.LayerManager.ListLayer[L], L);
            }

            foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByTileset)
            {
                ActiveTileSet.Value.Finish(GameScreen.GraphicsDevice);
            }
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
        }

        public void Draw(CustomSpriteBatch g, MapLayer Owner, bool IsSubLayer)
        {
            if (!Owner.IsVisible)
            {
                return;
            }

            if (IsSubLayer)
            {
                return;
            }

            for (int P = 0; P < Owner.ListProp.Count; ++P)
            {
                Owner.ListProp[P].Unit3D.SetViewMatrix(Camera.View);
                float TerrainZ = Owner.ArrayTerrain[(int)Owner.ListProp[P].Position.X, (int)Owner.ListProp[P].Position.Y].Position.Z;

                Owner.ListProp[P].Unit3D.SetPosition(
                    Owner.ListProp[P].Position.X + 0.5f,
                    Owner.ListProp[P].Position.Z * 32 + TerrainZ * 32,
                    Owner.ListProp[P].Position.Y + 0.5f);

                Owner.ListProp[P].Draw3D(GameScreen.GraphicsDevice);
            }
        }

        public void Draw(CustomSpriteBatch g)
        {
            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            g.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            PolygonEffect.View = Camera.View;
            Matrix ViewProjection = Camera.View * PolygonEffect.Projection;

            foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByTileset)
            {
                ActiveTileSet.Value.SetViewMatrix(ViewProjection, Camera.CameraPosition3D);

                ActiveTileSet.Value.Draw(g.GraphicsDevice);
            }

            if (Map.ShowLayerIndex == -1)
            {
                for (int L = 0; L < Map.LayerManager.ListLayer.Count; L++)
                {
                    Draw(g, Map.LayerManager.ListLayer[L], false);
                }
            }
            else
            {
                Draw(g, Map.LayerManager.ListLayer[Map.ShowLayerIndex], false);
            }

            DrawDrawablePoints(g);

            g.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            PolygonEffect.Texture = sprCursor;
            PolygonEffect.CurrentTechnique.Passes[0].Apply();

            Cursor.Draw(g.GraphicsDevice);

            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                //If the selected unit have the order to move, draw the possible positions it can go to.
                for (int U = 0; U < Map.ListPlayer[P].ListSquad.Count; U++)
                {
                    //If it's dead, don't draw it unless it's an event unit.
                    if ((Map.ListPlayer[P].ListSquad[U].CurrentLeader == null && !Map.ListPlayer[P].ListSquad[U].IsEventSquad)
                        || Map.ListPlayer[P].ListSquad[U].IsDead)
                        continue;

                    Color UnitColor;
                    if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.Colored)
                        UnitColor = Map.ListPlayer[P].Color;
                    else
                        UnitColor = Color.White;

                    Map.ListPlayer[P].ListSquad[U].Unit3D.SetViewMatrix(Camera.View);
                    float TerrainZ = Map.LayerManager.ListLayer[(int)Map.ListPlayer[P].ListSquad[U].Z].ArrayTerrain[(int)Map.ListPlayer[P].ListSquad[U].Position.X, (int)Map.ListPlayer[P].ListSquad[U].Position.Y].Position.Z;

                    Map.ListPlayer[P].ListSquad[U].Unit3D.SetPosition(
                        Map.ListPlayer[P].ListSquad[U].Position.X + 0.5f,
                        (Map.ListPlayer[P].ListSquad[U].Position.Z * 32 + TerrainZ * 32),
                        Map.ListPlayer[P].ListSquad[U].Position.Y + 0.5f);

                    Map.ListPlayer[P].ListSquad[U].Unit3D.Draw(GameScreen.GraphicsDevice);
                }
            }

            DrawDelayedAttacks(g);

            DrawPERAttacks(g);

            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            g.End();
            g.Begin();
        }

        private void DrawDelayedAttacks(CustomSpriteBatch g)
        {
            /*int BorderX = (int)(TileSize.X * 0.1);
            int BorderY = (int)(TileSize.Y * 0.1);

            foreach (DelayedAttack ActiveAttack in Map.ListDelayedAttack)
            {
                foreach (Vector3 ActivePosition in ActiveAttack.ListAttackPosition)
                {
                    g.Draw(GameScreen.sprPixel,
                        new Rectangle(
                            (int)(ActivePosition.X - CameraPosition.X) * TileSize.X + BorderX,
                            (int)(ActivePosition.Y - CameraPosition.Y) * TileSize.Y + BorderY,
                            TileSize.X - BorderX * 2,
                            TileSize.Y - BorderY * 2), Color.FromNonPremultiplied(139, 0, 0, 190));
                }
            }*/
        }

        private void DrawPERAttacks(CustomSpriteBatch g)
        {
            foreach (PERAttack ActiveAttack in Map.ListPERAttack)
            {
                ActiveAttack.Map3DComponent.SetViewMatrix(Camera.View);
                ActiveAttack.Map3DComponent.Draw(g.GraphicsDevice);
            }
        }

        public void EndDraw(CustomSpriteBatch g)
        {
        }
    }
}
