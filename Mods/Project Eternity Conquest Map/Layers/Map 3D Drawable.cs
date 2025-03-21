﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Vehicle;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class Map3DDrawable : ILayerHolderDrawable
    {
        protected Point MapSize { get { return Map.MapSize; } }

        const float CameraHeight = 200;
        const float CameraDistance = 300;
        const float CameraYaw = 0.2f;
        const float RealDistance = 255;

        private ConquestMap Map;

        private Effect MapEffect;
        private Effect ColorEffect;
        private BasicEffect PolygonEffect;
        private Camera3D Camera => Map.Camera3D;
        private Texture2D sprCursor;
        private Tile3D Cursor;
        private List<Tile3D> ListEditorCursorFace;

        private Dictionary<int, Tile3DHolder> DicTileBorderPerPlayer;
        private Dictionary<int, Tile3DHolder> DicTileShadowPerPlayer;
        private Dictionary<int, Tile3DHolder> DicTile3DByTileset;
        private Dictionary<int, Dictionary<int, Tile3DHolder>> DicTile3DByLayerByTileset;
        private Dictionary<int, Tile3DHolder> DicHiddenTile3DByTileset = new Dictionary<int, Tile3DHolder>();
        private List<DrawableTile> ListIgnoredTerrain = new List<DrawableTile>();

        private Dictionary<Vector4, List<Tile3D>> DicDrawablePointPerColor;
        private List<Tile3D> ListDrawableArrowPerColor;
        private Dictionary<string, Vector3> DicDamageNumberByPosition;

        public Map3DDrawable(ConquestMap Map, LayerHolderConquest LayerManager, GraphicsDevice g)
        {
            this.Map = Map;
            sprCursor = Map.sprCursor;
            ListEditorCursorFace = new List<Tile3D>();

            if (!Map.IsServer)
            {
                MapEffect = Map.Content.Load<Effect>("Shaders/Default Shader 3D 2");
                ColorEffect = Map.Content.Load<Effect>("Shaders/Color Only");
                ColorEffect.Parameters["t0"].SetValue(GameScreen.sprPixel);

                PolygonEffect = new BasicEffect(g);

                PolygonEffect.TextureEnabled = true;
                PolygonEffect.EnableDefaultLighting();

                float AspectRatio = g.Viewport.Width / (float)g.Viewport.Height;

                Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                        AspectRatio,
                                                                        1, 10000);
                PolygonEffect.Projection = Projection;

                PolygonEffect.World = Matrix.Identity;
                PolygonEffect.View = Matrix.Identity;

                MapEffect.Parameters["TextureAlpha"].SetValue(1);
                MapEffect.Parameters["World"].SetValue(Matrix.Transpose(PolygonEffect.World));

                Matrix worldInverse = Matrix.Invert(PolygonEffect.World);

                MapEffect.Parameters["WorldInverseTranspose"].SetValue(worldInverse);

                // Key light.
                MapEffect.Parameters["DirLight0Direction"].SetValue(new Vector3(-0.5265408f, -0.5735765f, -0.6275069f));
                MapEffect.Parameters["DirLight0DiffuseColor"].SetValue(new Vector3(1, 0.9607844f, 0.8078432f));
                MapEffect.Parameters["DirLight0SpecularColor"].SetValue(new Vector3(1, 0.9607844f, 0.8078432f));

                // Fill light.
                MapEffect.Parameters["DirLight1Direction"].SetValue(new Vector3(0.7198464f, 0.3420201f, 0.6040227f));
                MapEffect.Parameters["DirLight1DiffuseColor"].SetValue(new Vector3(0.9647059f, 0.7607844f, 0.4078432f));
                MapEffect.Parameters["DirLight1SpecularColor"].SetValue(Vector3.Zero);

                // Back light.
                MapEffect.Parameters["DirLight2Direction"].SetValue(new Vector3(0.4545195f, -0.7660444f, 0.4545195f));
                MapEffect.Parameters["DirLight2DiffuseColor"].SetValue(new Vector3(0.3231373f, 0.3607844f, 0.3937255f));
                MapEffect.Parameters["DirLight2SpecularColor"].SetValue(new Vector3(0.3231373f, 0.3607844f, 0.3937255f));

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

                MapEffect.Parameters["DiffuseColor"].SetValue(diffuse);
                MapEffect.Parameters["EmissiveColor"].SetValue(emissive);
                MapEffect.Parameters["SpecularColor"].SetValue(Vector3.One);
                MapEffect.Parameters["SpecularPower"].SetValue(64);

                MapEffect.Parameters["FogLimits"].SetValue(new Vector2(1200, 2000));
                MapEffect.Parameters["FogColor"].SetValue(new Vector3(0.0f, 0.0f, 0.0f));

                CreateMap(Map, LayerManager);

                float Z = LayerManager.ListLayer[0].ArrayTerrain[0, 0].WorldPosition.Z;
                DrawableTile ActiveTerrain = LayerManager.ListLayer[0].ArrayTile[0, 0];
                Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;
                Cursor = ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                    0, 0, Z, 0, Map.TileSize, Map.TileSize, new List<Texture2D>() { sprCursor }, Z, Z, Z, Z, 0)[0];
            }

            DicDrawablePointPerColor = new Dictionary<Vector4, List<Tile3D>>();
            ListDrawableArrowPerColor = new List<Tile3D>();
            DicDamageNumberByPosition = new Dictionary<string, Vector3>();

            if (Map.IsEditor)
            {
                Map.CursorPosition.X = Map.MapSize.X / 2;
                Map.CursorPosition.Y = Map.MapSize.Y / 2;
                Map.CursorPosition.Z = (LayerManager.ListLayer.Count - 1) / 2;
            }
        }

        private void CreateMap(ConquestMap Map, LayerHolderConquest LayerManager)
        {
            DicTileBorderPerPlayer = new Dictionary<int, Tile3DHolder>();
            DicTileShadowPerPlayer = new Dictionary<int, Tile3DHolder>();
            DicTile3DByTileset = new Dictionary<int, Tile3DHolder>();
            DicHiddenTile3DByTileset = new Dictionary<int, Tile3DHolder>();
            DicTile3DByLayerByTileset = new Dictionary<int, Dictionary<int, Tile3DHolder>>();

            for (int L = 0; L < LayerManager.ListLayer.Count; L++)
            {
                CreateMap(Map, LayerManager.ListLayer[L], L);
            }

            foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByTileset)
            {
                ActiveTileSet.Value.Finish(GameScreen.GraphicsDevice);
            }

            foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicHiddenTile3DByTileset)
            {
                ActiveTileSet.Value.Finish(GameScreen.GraphicsDevice);
            }

            foreach (KeyValuePair<int, Dictionary<int, Tile3DHolder>> ActiveLayer in DicTile3DByLayerByTileset)
            {
                foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in ActiveLayer.Value)
                {
                    ActiveTileSet.Value.Finish(GameScreen.GraphicsDevice);
                }
            }

            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                if (Map.ListPlayer[P].ListUnit.Count > 0)
                {
                    CreateUnitBorderAndShadow(P);
                }
            }
        }

        protected void CreateMap(ConquestMap Map, MapLayer Owner, int LayerIndex)
        {
            foreach (SubMapLayer ActiveSubLayer in Owner.ListSubLayer)
            {
                CreateMap(Map, ActiveSubLayer, LayerIndex);
            }

            if (!DicTile3DByLayerByTileset.ContainsKey(LayerIndex))
            {
                DicTile3DByLayerByTileset.Add(LayerIndex, new Dictionary<int, Tile3DHolder>());
            }

            for (int X = Map.MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
                {
                    DrawableTile ActiveTerrain = Owner.ArrayTile[X, Y];
                    Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;
                    if (ActiveTerrain3D.TerrainStyle == Terrain3D.TerrainStyles.Invisible)
                    {
                        continue;
                    }

                    float Z = Owner.ArrayTerrain[X, Y].WorldPosition.Z * Map.LayerHeight;
                    float MinZ = Z - Owner.ArrayTerrain[X, Y].Height * Map.LayerHeight;
                    float ZFront = MinZ;
                    float ZBack = MinZ;
                    float ZRight = MinZ;
                    float ZLeft = MinZ;
                    if (Y + 1 < Map.MapSize.Y && ConsiderTerrain(Owner.ArrayTile[X, Y + 1].Terrain3DInfo.TerrainStyle))
                    {
                        ZFront = Owner.ArrayTerrain[X, Y + 1].WorldPosition.Z * Map.LayerHeight;
                    }
                    if (Y - 1 >= 0 && ConsiderTerrain(Owner.ArrayTile[X, Y - 1].Terrain3DInfo.TerrainStyle))
                    {
                        ZBack = Owner.ArrayTerrain[X, Y - 1].WorldPosition.Z * Map.LayerHeight;
                    }
                    if (X - 1 >= 0 && ConsiderTerrain(Owner.ArrayTile[X - 1, Y].Terrain3DInfo.TerrainStyle))
                    {
                        ZLeft = Owner.ArrayTerrain[X - 1, Y].WorldPosition.Z * Map.LayerHeight;
                    }
                    if (X + 1 < Map.MapSize.X && ConsiderTerrain(Owner.ArrayTile[X + 1, Y].Terrain3DInfo.TerrainStyle))
                    {
                        ZRight = Owner.ArrayTerrain[X + 1, Y].WorldPosition.Z * Map.LayerHeight;
                    }

                    List<Tile3D> ListNew3DTile = ActiveTerrain3D.CreateTile3D(ActiveTerrain.TilesetIndex, ActiveTerrain.Origin.Location,
                                            Owner.ArrayTerrain[X, Y].WorldPosition.X, Owner.ArrayTerrain[X, Y].WorldPosition.Y, Z, MinZ,
                                            Map.TileSize, Map.TileSize, Map.ListTileSet, ZFront, ZBack, ZLeft, ZRight, 0);

                    if (ListIgnoredTerrain.Contains(ActiveTerrain))
                    {
                        foreach (Tile3D ActiveTile in ListNew3DTile)
                        {
                            if (!DicHiddenTile3DByTileset.ContainsKey(ActiveTile.TilesetIndex))
                            {
                                Tile3DHolder NewTile3DHolder = new Tile3DHolder(MapEffect, Map.ListTileSet[ActiveTile.TilesetIndex]);
                                NewTile3DHolder.Effect3D.Parameters["TextureAlpha"].SetValue(0.5f);
                                DicHiddenTile3DByTileset.Add(ActiveTile.TilesetIndex, NewTile3DHolder);
                            }

                            if (!DicTile3DByLayerByTileset[LayerIndex].ContainsKey(ActiveTile.TilesetIndex))
                            {
                                DicTile3DByLayerByTileset[LayerIndex].Add(ActiveTile.TilesetIndex, new Tile3DHolder(MapEffect, Map.ListTileSet[ActiveTile.TilesetIndex]));
                            }

                            DicHiddenTile3DByTileset[ActiveTile.TilesetIndex].AddTile(ActiveTile);
                            DicTile3DByLayerByTileset[LayerIndex][ActiveTile.TilesetIndex].AddTile(ActiveTile);
                        }
                    }
                    else
                    {
                        foreach (Tile3D ActiveTile in ListNew3DTile)
                        {
                            if (!DicTile3DByTileset.ContainsKey(ActiveTile.TilesetIndex))
                            {
                                DicTile3DByTileset.Add(ActiveTile.TilesetIndex, new Tile3DHolder(MapEffect, Map.ListTileSet[ActiveTile.TilesetIndex]));
                            }

                            if (!DicTile3DByLayerByTileset[LayerIndex].ContainsKey(ActiveTile.TilesetIndex))
                            {
                                DicTile3DByLayerByTileset[LayerIndex].Add(ActiveTile.TilesetIndex, new Tile3DHolder(MapEffect, Map.ListTileSet[ActiveTile.TilesetIndex]));
                            }

                            DicTile3DByTileset[ActiveTile.TilesetIndex].AddTile(ActiveTile);
                            DicTile3DByLayerByTileset[LayerIndex][ActiveTile.TilesetIndex].AddTile(ActiveTile);
                        }
                    }
                }
            }

            for (int L = 0; L < Owner.ListSubLayer.Count; L++)
            {
                CreateMap(Map, Owner.ListSubLayer[L], LayerIndex);
            }
        }

        private bool ConsiderTerrain(Terrain3D.TerrainStyles TerrainInfo)
        {
            return TerrainInfo != Terrain3D.TerrainStyles.Invisible
                && TerrainInfo != Terrain3D.TerrainStyles.SlopeBackToFront
                && TerrainInfo != Terrain3D.TerrainStyles.SlopeFrontToBack
                && TerrainInfo != Terrain3D.TerrainStyles.SlopeLeftToRight
                && TerrainInfo != Terrain3D.TerrainStyles.SlopeRightToLeft;
        }

        private void Create3DCursor()
        {
            ListEditorCursorFace.Clear();

            int X = (int)Map.CursorPositionVisible.X;
            int Y = (int)Map.CursorPositionVisible.Y;
            float ZTop = (Map.CursorPosition.Z + 1) * Map.LayerHeight + 0.3f;
            float ZBottom = Map.CursorPosition.Z * Map.LayerHeight + 0.3f;
            Terrain3D Cursor = new Terrain3D();
            Cursor.TerrainStyle = Terrain3D.TerrainStyles.Cube;
            Cursor.FrontFace = new DrawableTile();
            Cursor.FrontFace.TilesetIndex = 0;
            Cursor.FrontFace.Origin = new Rectangle(0, 0, Map.TileSize.X, Map.TileSize.Y);
            Cursor.BackFace = new DrawableTile();
            Cursor.BackFace.TilesetIndex = 0;
            Cursor.BackFace.Origin = new Rectangle(0, 0, Map.TileSize.X, Map.TileSize.Y);
            Cursor.LeftFace = new DrawableTile();
            Cursor.LeftFace.TilesetIndex = 0;
            Cursor.LeftFace.Origin = new Rectangle(0, 0, Map.TileSize.X, Map.TileSize.Y);
            Cursor.RightFace = new DrawableTile();
            Cursor.RightFace.TilesetIndex = 0;
            Cursor.RightFace.Origin = new Rectangle(0, 0, Map.TileSize.X, Map.TileSize.Y);

            ListEditorCursorFace = Cursor.CreateTile3D(0, Point.Zero,
                X * Map.TileSize.X, Y * Map.TileSize.Y, ZTop, ZBottom, Map.TileSize, Map.TileSize, new List<Texture2D>() { sprCursor }, ZBottom, ZBottom, ZBottom, ZBottom, 0);
            ListEditorCursorFace.Add(Cursor.CreateTile3D(0, Point.Zero,
                X * Map.TileSize.X, Y * Map.TileSize.Y, ZBottom, ZBottom, Map.TileSize, Map.TileSize, new List<Texture2D>() { sprCursor }, ZBottom, ZBottom, ZBottom, ZBottom, 0)[0]);
        }

        public void Update(GameTime gameTime)
        {
            if (Map.ActivePlatform == null && (!Map.IsAPlatform || Map.IsPlatformActive) && !Map.IsServer)
            {
                Cursor = Map.CreateTile3D(0, Map.CursorPosition, Point.Zero, Map.TileSize, Map.TileSize, 0f);
            }

            if (!Map.IsAPlatform && !Map.IsServer)
            {
                if (Map.ActivePlatform != null)
                {
                    SetTarget(new Vector3(Map.TileSize.X * Map.ActivePlatform.Map.CursorTerrain.WorldPosition.X,
                        Map.ActivePlatform.Map.CursorTerrain.WorldPosition.Z * Map.LayerHeight,
                        Map.TileSize.Y * Map.ActivePlatform.Map.CursorTerrain.WorldPosition.Y));
                }
                else
                {
                    SetTarget(new Vector3(Map.TileSize.X * Map.CursorPositionVisible.X, Map.CursorPosition.Z * Map.LayerHeight, Map.TileSize.Y * Map.CursorPositionVisible.Y));
                }

                Camera.Update(gameTime);
            }

            DicDrawablePointPerColor.Clear();
            ListDrawableArrowPerColor.Clear();
            DicDamageNumberByPosition.Clear();
            if (Map.IsEditor)
            {
                Create3DCursor();
            }
            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                //If the selected unit have the order to move, draw the possible positions it can go to.
                foreach (UnitConquest ActiveUnit in Map.ListPlayer[P].ListUnit)
                {
                    //If it's dead, don't draw it unless it's an event unit.
                    if ((!ActiveUnit.IsEventSquad)
                        || ActiveUnit.HP == 0 || ActiveUnit.Unit3DModel == null)
                        continue;

                    ActiveUnit.Unit3DModel.Update(gameTime);
                }
            }
        }

        public void SetTarget(Vector3 Target)
        {
            float YawRotation = MathHelper.ToRadians(Map.Camera3DYawAngle);
            float PitchRotation = MathHelper.ToRadians(Map.Camera3DPitchAngle);
            float RollRotation = 0;

            Matrix FinalMatrix = Matrix.CreateTranslation(0, Map.Camera3DDistance, 0) * Matrix.CreateFromYawPitchRoll(YawRotation, PitchRotation, RollRotation);
            Camera.CameraPosition3D = FinalMatrix.Translation + Target;

            Camera.View = Matrix.CreateLookAt(Camera.CameraPosition3D, Target, Vector3.Up);
        }

        public void SetWorld(Matrix NewWorld)
        {
            PolygonEffect.World = NewWorld;

            Matrix worldInverse = Matrix.Invert(NewWorld);

            MapEffect.Parameters["World"].SetValue(Matrix.Transpose(NewWorld));

            MapEffect.Parameters["WorldInverseTranspose"].SetValue(worldInverse);

            foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByTileset)
            {
                Matrix World;

                if (Map.Camera3DOverride != null && !Map.IsEditor)
                {
                    World = NewWorld * Map.Camera3DOverride.View;
                }
                else
                {
                    World = NewWorld * Map.Camera3D.View;
                }

                worldInverse = Matrix.Invert(World);

                ActiveTileSet.Value.Effect3D.Parameters["World"].SetValue(Matrix.Transpose(NewWorld));
                ActiveTileSet.Value.Effect3D.Parameters["WorldInverseTranspose"].SetValue(worldInverse);
            }
        }

        private bool IsCursorHiddenByWall()
        {
            int CursorX = (int)Map.CursorPosition.X;
            int CursorLayer = (int)Map.CursorPosition.Z;
            for (int Y = (int)Map.CursorPosition.Y; Y < Map.MapSize.Y; ++Y)
            {
                if (CursorLayer < Map.LayerManager.ListLayer.Count && Map.LayerManager.ListLayer[CursorLayer].ArrayTerrain[CursorX, Y].TerrainTypeIndex == UnitStats.TerrainWallIndex)
                {
                    return true;
                }
                ++CursorLayer;
            }

            return false;
        }

        public Point GetVisiblePosition(Vector3 Position)
        {
            Point BaseMenuPosition;

            Vector3 Visible3DPosition = new Vector3(Position.X * Map.TileSize.X, Position.Z * Map.LayerHeight, Position.Y * Map.TileSize.Y);

            if (Map.ActivePlatform !=null)
            {
                Visible3DPosition = new Vector3(Map.ActivePlatform.Map.CursorTerrain.WorldPosition.X * Map.ActivePlatform.Map.TileSize.X,
                    Map.ActivePlatform.Map.CursorTerrain.WorldPosition.Z * Map.LayerHeight,
                    Map.ActivePlatform.Map.CursorTerrain.WorldPosition.Y * Map.ActivePlatform.Map.TileSize.Y);
            }

            Vector3 Position2D = GameScreen.GraphicsDevice.Viewport.Project(Visible3DPosition, PolygonEffect.Projection, PolygonEffect.View, Matrix.Identity);

            BaseMenuPosition.X = (int)Position2D.X;
            BaseMenuPosition.Y = (int)Position2D.Y;

            return BaseMenuPosition;
        }

        public void AddDrawablePoints(List<MovementAlgorithmTile> ListPoint, Color PointColor)
        {
            List<Tile3D> ListDrawablePoint3D = new List<Tile3D>(ListPoint.Count);

            foreach (MovementAlgorithmTile ActivePoint in ListPoint)
            {
                ListDrawablePoint3D.Add(Map.CreateTile3D(0, ActivePoint.WorldPosition, Point.Zero, ActivePoint.Owner.TileSize, ActivePoint.Owner.TileSize, 0));
            }

            DicDrawablePointPerColor.Add(new Vector4(PointColor.R / 255f, PointColor.G / 255f, PointColor.B / 255f, PointColor.A / 255f), ListDrawablePoint3D);
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

                ListDrawableArrowPerColor.Add(Map.CreateTile3D(0, ActivePoint.WorldPosition, GetCursorTextureOffset(Previous, ActivePoint, Next), ActivePoint.Owner.TileSize, ActivePoint.Owner.TileSize, 0));
            }
        }

        public void AddDamageNumber(string Damage, Vector3 Position)
        {
            DicDamageNumberByPosition.Add(Damage, Position);
        }

        private Point GetCursorTextureOffset(MovementAlgorithmTile Previous, MovementAlgorithmTile Current, MovementAlgorithmTile Next)
        {
            const int TextureSize = 32;

            Point TextureOffset = Point.Zero;
            if (Next == null)
            {
                if (Previous != null && Previous.WorldPosition.X < Current.WorldPosition.X)//Right
                {
                    TextureOffset.X = TextureSize * 3;
                }
                else if (Previous != null && Previous.WorldPosition.X > Current.WorldPosition.X)//Left
                {
                    TextureOffset.X = TextureSize * 4;
                }
                else if (Previous != null && Previous.WorldPosition.Y < Current.WorldPosition.Y)//Down
                {
                    TextureOffset.X = TextureSize * 5;
                }
                else if (Previous != null && Previous.WorldPosition.Y > Current.WorldPosition.Y)//Up
                {
                    TextureOffset.X = TextureSize * 2;
                }
            }
            else if (Current.WorldPosition.X < Next.WorldPosition.X)//Going Right
            {
                if (Previous != null && Previous.WorldPosition.Y < Current.WorldPosition.Y)//Down Right
                {
                    TextureOffset.X = TextureSize * 9;
                }
                else if (Previous != null && Previous.WorldPosition.Y > Current.WorldPosition.Y)//Up Right
                {
                    TextureOffset.X = TextureSize * 8;
                }
                else
                {
                    TextureOffset.X = TextureSize * 1;
                }
            }
            else if (Current.WorldPosition.X > Next.WorldPosition.X)//Going Left
            {
                if (Previous != null && Previous.WorldPosition.Y < Current.WorldPosition.Y)//Down Left
                {
                    TextureOffset.X = TextureSize * 7;
                }
                else if (Previous != null && Previous.WorldPosition.Y > Current.WorldPosition.Y)//Up Left
                {
                    TextureOffset.X = TextureSize * 6;
                }
                else
                {
                    TextureOffset.X = TextureSize * 1;
                }
            }
            else if (Current.WorldPosition.Y < Next.WorldPosition.Y)//Going Down
            {
                if (Previous != null && Previous.WorldPosition.X < Current.WorldPosition.X)//Right Down
                {
                    TextureOffset.X = TextureSize * 6;
                }
                else if (Previous != null && Previous.WorldPosition.X > Current.WorldPosition.X)//Left Down
                {
                    TextureOffset.X = TextureSize * 8;
                }
                else
                {
                    TextureOffset.X = TextureSize * 0;
                }
            }
            else if (Current.WorldPosition.Y > Next.WorldPosition.Y)//Going Up
            {
                if (Previous != null && Previous.WorldPosition.X < Current.WorldPosition.X)//Right Up
                {
                    TextureOffset.X = TextureSize * 7;
                }
                else if (Previous != null && Previous.WorldPosition.X > Current.WorldPosition.X)//Left Up
                {
                    TextureOffset.X = TextureSize * 9;
                }
                else
                {
                    TextureOffset.X = TextureSize * 0;
                }
            }

            return TextureOffset;
        }

        public void Reset()
        {
            CreateMap(Map, Map.LayerManager);
        }

        public void CursorMoved()
        {
            FilterTerrainObscuringUnits();
        }

        public void UnitMoved(int PlayerIndex)
        {
            CreateUnitBorderAndShadow(PlayerIndex);
        }

        public void UnitKilled(int PlayerIndex)
        {
            CreateUnitBorderAndShadow(PlayerIndex);
        }

        private void CreateUnitBorderAndShadow(int PlayerIndex)
        {
            if (!DicTileBorderPerPlayer.ContainsKey(PlayerIndex))
            {
                Color PlayerColor = Map.ListPlayer[PlayerIndex].Color;

                if (PlayerColor == Color.Red)
                {
                    Tile3DHolder NewTile3DHolder = new Tile3DHolder(MapEffect, Map.sprTileBorderRed);
                    NewTile3DHolder.Effect3D.Parameters["TextureAlpha"].SetValue(0.5f);
                    DicTileBorderPerPlayer.Add(PlayerIndex, NewTile3DHolder);
                }
                else
                {
                    Tile3DHolder NewTile3DHolder = new Tile3DHolder(MapEffect, Map.sprTileBorderBlue);
                    NewTile3DHolder.Effect3D.Parameters["TextureAlpha"].SetValue(0.5f);
                    DicTileBorderPerPlayer.Add(PlayerIndex, NewTile3DHolder);
                }
            }

            DicTileBorderPerPlayer[PlayerIndex].Clear();

            bool BorderCreated = false;
            foreach (UnitConquest ActiveUnit in Map.ListPlayer[PlayerIndex].ListUnit)
            {
                Tile3D BorderTile = Map.CreateTile3D(0, ActiveUnit.Position, Map.TileSize, Point.Zero, new Point(Map.sprTileBorderRed.Width, Map.sprTileBorderRed.Height), 0f);

                DicTileBorderPerPlayer[PlayerIndex].AddTile(BorderTile);
                BorderCreated = true;
            }

            if (BorderCreated)
            {
                foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTileBorderPerPlayer)
                {
                    ActiveTileSet.Value.Finish(GameScreen.GraphicsDevice);
                }
            }
            else
            {
                DicTileBorderPerPlayer.Remove(PlayerIndex);
            }

            if (!DicTileShadowPerPlayer.ContainsKey(PlayerIndex))
            {
                Tile3DHolder NewTile3DHolder = new Tile3DHolder(MapEffect, Map.sprUnitHover);
                NewTile3DHolder.Effect3D.Parameters["TextureAlpha"].SetValue(0.5f);

                DicTileShadowPerPlayer.Add(PlayerIndex, NewTile3DHolder);
            }

            DicTileShadowPerPlayer[PlayerIndex].Clear();

            bool ShadowCreated = false;
            foreach (UnitConquest ActiveUnit in Map.ListPlayer[PlayerIndex].ListUnit)
            {
                if (ActiveUnit.Components.IsOnGround)
                {
                    continue;
                }

                Tile3D BorderTile = Map.CreateTile3D(0, ActiveUnit.Position, Point.Zero, Map.TileSize, new Point(Map.sprTileBorderRed.Width, Map.sprTileBorderRed.Height), 0f);

                DicTileShadowPerPlayer[PlayerIndex].AddTile(BorderTile);
                ShadowCreated = true;
            }

            if (ShadowCreated)
            {
                foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTileShadowPerPlayer)
                {
                    ActiveTileSet.Value.Finish(GameScreen.GraphicsDevice);
                }
            }
            else
            {
                DicTileShadowPerPlayer.Remove(PlayerIndex);
            }
        }

        private void FilterTerrainObscuringUnits()
        {
            ListIgnoredTerrain.Clear();

            foreach (Player ActivePlayer in Map.ListPlayer)
            {
                foreach (UnitConquest ActiveUnit in ActivePlayer.ListUnit)
                {
                    if (ActiveUnit.Z + 1 < Map.LayerManager.ListLayer.Count)
                    {
                        int X = (int)ActiveUnit.X;
                        int Y = (int)ActiveUnit.Y;
                        int LayerIndex = (int)ActiveUnit.Z;
                        Terrain UpperTerrain = Map.LayerManager.ListLayer[LayerIndex + 1].ArrayTerrain[X, Y];
                        DrawableTile ActiveTile = Map.LayerManager.ListLayer[LayerIndex].ArrayTile[X, Y];
                        TerrainType UpperTerrainType = Map.TerrainRestrictions.ListTerrainType[UpperTerrain.TerrainTypeIndex];

                        if (UpperTerrainType.ListRestriction.Count > 0 || ActiveTile.Terrain3DInfo.TerrainStyle != Terrain3D.TerrainStyles.Invisible
                            && !ListIgnoredTerrain.Contains(ActiveTile))
                        {
                            ListIgnoredTerrain.Add(ActiveTile);
                            FloodFill(new Vector3(UpperTerrain.GridPosition.X, UpperTerrain.GridPosition.Y, UpperTerrain.LayerIndex), UpperTerrain.TerrainTypeIndex);
                        }
                    }
                }
            }

            CreateMap(Map, Map.LayerManager);
        }

        private void FloodFill(Vector3 StartingPosition, byte TargetTerrainIndex)
        {
            int Z = (int)StartingPosition.Z;

            Stack<Vector3> ListTerrainPositionToHide = new Stack<Vector3>();

            ListTerrainPositionToHide.Push(StartingPosition);
            while (ListTerrainPositionToHide.Count != 0)
            {
                Vector3 ActivePosition = ListTerrainPositionToHide.Pop();
                int X = (int)ActivePosition.X;
                int Y = (int)ActivePosition.Y;
                int ActiveY = Y;
                Terrain ActiveTerrain = Map.LayerManager.ListLayer[Z].ArrayTerrain[X, ActiveY];
                DrawableTile ActiveTile = Map.LayerManager.ListLayer[Z].ArrayTile[X, ActiveY];

                while (ActiveY >= 0 && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex && !ListIgnoredTerrain.Contains(ActiveTile))
                {
                    if (--ActiveY >= 0)
                    {
                        ActiveTerrain = Map.GetTerrain(new Vector3(X, ActiveY, Z));
                    }
                }

                ActiveY++;
                bool spanLeft = false;
                bool spanRight = false;

                while (ActiveY < Map.MapSize.Y && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex)
                {
                    ListIgnoredTerrain.Add(Map.LayerManager.ListLayer[Z].ArrayTile[X, ActiveY]);

                    if (X - 1 >= 0)
                    {
                        ActiveTerrain = Map.LayerManager.ListLayer[Z].ArrayTerrain[X - 1, ActiveY];
                        ActiveTile = Map.LayerManager.ListLayer[Z].ArrayTile[X - Map.TileSize.X, ActiveY];

                        if (!spanLeft && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex && !ListIgnoredTerrain.Contains(ActiveTile))
                        {
                            ListTerrainPositionToHide.Push(new Vector3(X - 1, ActiveY, Z));
                            spanLeft = true;
                        }
                        else if (spanLeft && X - 1 == 0 && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex && !ListIgnoredTerrain.Contains(ActiveTile))
                        {
                            spanLeft = false;

                        }
                    }

                    if (X + 1 < Map.MapSize.X)
                    {
                        ActiveTerrain = Map.LayerManager.ListLayer[Z].ArrayTerrain[X + 1, ActiveY];
                        ActiveTile = Map.LayerManager.ListLayer[Z].ArrayTile[X + Map.TileSize.X, ActiveY];

                        if (!spanRight && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex && !ListIgnoredTerrain.Contains(ActiveTile))
                        {
                            ListTerrainPositionToHide.Push(new Vector3(X + 1, ActiveY, Z));
                            spanRight = true;
                        }
                        else if (spanRight && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex && !ListIgnoredTerrain.Contains(ActiveTile))
                        {
                            spanRight = false;
                        }
                    }

                    ActiveY++;
                }
            }
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
        }

        public void Draw(CustomSpriteBatch g)
        {
            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            g.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            Matrix View = Map.Camera3D.View;
            Matrix World = PolygonEffect.World;
            Vector3 CameraPosition = Map.Camera3D.CameraPosition3D;

            if (Map.Camera3DOverride != null && !Map.IsEditor)
            {
                View = Map.Camera3DOverride.View;
                CameraPosition = Map.Camera3DOverride.CameraPosition3D;
            }
            PolygonEffect.View = View;
            Matrix ViewProjection = View * PolygonEffect.Projection;
            Matrix WorldViewProjection = World * ViewProjection;
            ColorEffect.Parameters["ViewProjection"].SetValue(ViewProjection);

            bool DrawUpperLayers = !IsCursorHiddenByWall();

            DrawMap(g, View, WorldViewProjection, DrawUpperLayers);

            DrawVehicles(g, View);

            DrawDrawablePoints(g);

            if ((Map.ActivePlatform == null && !Map.IsAPlatform) || Map.IsPlatformActive)
            {
                PolygonEffect.Texture = sprCursor;
                PolygonEffect.CurrentTechnique.Passes[0].Apply();

                if (Map.IsEditor)
                {
                    g.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                    foreach (Tile3D CursorFace in ListEditorCursorFace)
                    {
                        CursorFace.Draw(g.GraphicsDevice);
                    }
                    g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                }
                else
                {
                    Cursor.Draw(g.GraphicsDevice);
                }
            }

            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            DrawSquads(g, View, DrawUpperLayers);

            DrawDelayedAttacks(g, ViewProjection);

            DrawPERAttacks(g, View);

            foreach (Tile3DHolder ActivePlayerBorder in DicTileBorderPerPlayer.Values)
            {
                ActivePlayerBorder.Effect3D.Parameters["WorldViewProj"].SetValue(ViewProjection);
                ActivePlayerBorder.Effect3D.Parameters["CameraPosition"].SetValue(CameraPosition);
                ActivePlayerBorder.Draw(g.GraphicsDevice);
            }

            foreach (Tile3DHolder ActivePlayerBorder in DicTileShadowPerPlayer.Values)
            {
                ActivePlayerBorder.Effect3D.Parameters["WorldViewProj"].SetValue(ViewProjection);
                ActivePlayerBorder.Effect3D.Parameters["CameraPosition"].SetValue(CameraPosition);
                ActivePlayerBorder.Draw(g.GraphicsDevice);
            }

            foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicHiddenTile3DByTileset)
            {
                ActiveTileSet.Value.Effect3D.Parameters["WorldViewProj"].SetValue(ViewProjection);
                ActiveTileSet.Value.Effect3D.Parameters["CameraPosition"].SetValue(CameraPosition);

                ActiveTileSet.Value.Draw(g.GraphicsDevice);
            }

            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            g.End();
            g.Begin();

            DrawDamageNumbers(g, View);
        }

        private void DrawMap(CustomSpriteBatch g, Matrix View, Matrix WorldViewProjection, bool DrawUpperLayers)
        {
            Vector3 CameraPosition = Map.Camera3D.CameraPosition3D;
            if (Map.Camera3DOverride != null && !Map.IsEditor)
            {
                CameraPosition = Map.Camera3DOverride.CameraPosition3D;
            }

            if (Map.ShowLayerIndex == -1)
            {
                if (DrawUpperLayers || Map.IsEditor)
                {
                    foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByTileset)
                    {
                        ActiveTileSet.Value.Effect3D.Parameters["WorldViewProj"].SetValue(WorldViewProjection);
                        ActiveTileSet.Value.Effect3D.Parameters["CameraPosition"].SetValue(CameraPosition);

                        ActiveTileSet.Value.Draw(g.GraphicsDevice);
                    }

                    for (int L = 0; L < Map.LayerManager.ListLayer.Count; L++)
                    {
                        DrawItems(g, View, Map.LayerManager.ListLayer[L], false);
                    }
                }
                else
                {
                    int MaxLayerIndex = Map.LayerManager.ListLayer.Count;
                    if (!DrawUpperLayers)
                    {
                        MaxLayerIndex = (int)Map.CursorPosition.Z + 1;
                    }

                    for (int L = 0; L < MaxLayerIndex; L++)
                    {
                        foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByLayerByTileset[L])
                        {
                            ActiveTileSet.Value.Effect3D.Parameters["WorldViewProj"].SetValue(WorldViewProjection);
                            ActiveTileSet.Value.Effect3D.Parameters["CameraPosition"].SetValue(CameraPosition);

                            ActiveTileSet.Value.Draw(g.GraphicsDevice);
                        }

                        DrawItems(g, View, Map.LayerManager.ListLayer[L], false);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByLayerByTileset[Map.ShowLayerIndex])
                {
                    ActiveTileSet.Value.Effect3D.Parameters["WorldViewProj"].SetValue(WorldViewProjection);
                    ActiveTileSet.Value.Effect3D.Parameters["CameraPosition"].SetValue(CameraPosition);

                    ActiveTileSet.Value.Draw(g.GraphicsDevice);
                }

                DrawItems(g, View, Map.LayerManager.ListLayer[Map.ShowLayerIndex], false);
            }
        }

        public void DrawItems(CustomSpriteBatch g, Matrix View, MapLayer Owner, bool IsSubLayer)
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
                float TerrainZ = Owner.ArrayTerrain[(int)Owner.ListProp[P].Position.X, (int)Owner.ListProp[P].Position.Y].WorldPosition.Z;

                if (Owner.ListProp[P].Unit3D != null)
                {
                    Owner.ListProp[P].Unit3D.SetPosition(
                        (Owner.ListProp[P].Position.X + 0.5f) * Map.TileSize.X,
                        (TerrainZ + 0.5f) * Map.LayerHeight,
                        (Owner.ListProp[P].Position.Y + 0.5f) * Map.TileSize.Y);
                }
                Owner.ListProp[P].Draw3D(GameScreen.GraphicsDevice,View, g);
            }

            for (int P = 0; P < Owner.ListAttackPickup.Count; ++P)
            {
                Owner.ListAttackPickup[P].Attack3D.SetViewMatrix(View);
                float TerrainZ = Owner.ArrayTerrain[(int)Owner.ListAttackPickup[P].Position.X, (int)Owner.ListAttackPickup[P].Position.Y].WorldPosition.Z;

                Owner.ListAttackPickup[P].Attack3D.SetPosition(
                    (Owner.ListAttackPickup[P].Position.X + 0.5f) * Map.TileSize.X,
                    TerrainZ * Map.LayerHeight,
                    (Owner.ListAttackPickup[P].Position.Y + 0.5f) * Map.TileSize.Y);
            }

            for (int P = 0; P < Owner.ListAttackPickup.Count; ++P)
            {
                Owner.ListAttackPickup[P].Attack3D.Draw(GameScreen.GraphicsDevice);
            }

            for (int I = 0; I < Owner.ListHoldableItem.Count; ++I)
            {
                Owner.ListHoldableItem[I].Item3D.SetViewMatrix(View);
                float TerrainZ = Owner.ArrayTerrain[(int)Owner.ListHoldableItem[I].Position.X, (int)Owner.ListHoldableItem[I].Position.Y].WorldPosition.Z;

                Owner.ListHoldableItem[I].Item3D.SetPosition(
                    (Owner.ListHoldableItem[I].Position.X + 0.5f) * Map.TileSize.X,
                    TerrainZ * Map.LayerHeight,
                    (Owner.ListHoldableItem[I].Position.Y + 0.5f) * Map.TileSize.Y);
            }

            for (int I = 0; I < Owner.ListHoldableItem.Count; ++I)
            {
                Owner.ListHoldableItem[I].Item3D.Draw(GameScreen.GraphicsDevice);
            }
        }

        private void DrawSquads(CustomSpriteBatch g, Matrix View, bool DrawUpperLayers)
        {
            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                //If the selected unit have the order to move, draw the possible positions it can go to.
                foreach (UnitConquest ActiveUnit in Map.ListPlayer[P].ListUnit)
                {
                    //If it's dead, don't draw it unless it's an event unit.
                    if ((!ActiveUnit.IsEventSquad)
                        || ActiveUnit.HP == 0
                        || (!DrawUpperLayers && ActiveUnit.Z > Map.CursorPosition.Z))
                        continue;

                    if (Map.MovementAnimation.Contains(ActiveUnit.Components))
                    {
                        Vector3 CurrentPosition = Map.MovementAnimation.GetPosition(ActiveUnit.Components);

                        if (ActiveUnit.Components.ItemHeld != null)
                        {
                            ActiveUnit.Components.ItemHeld.Item3D.SetViewMatrix(View);

                            ActiveUnit.Components.ItemHeld.Item3D.SetPosition(
                                CurrentPosition.X * Map.TileSize.X,
                                (CurrentPosition.Z + 1f) * Map.LayerHeight,
                                (CurrentPosition.Y - 0.5f) * Map.TileSize.Y);

                            ActiveUnit.Components.ItemHeld.Item3D.Draw(GameScreen.GraphicsDevice);
                        }

                        if (ActiveUnit.Unit3DModel == null)
                        {
                            ActiveUnit.Unit3DSprite.SetViewMatrix(View);

                            ActiveUnit.Unit3DSprite.SetPosition(
                                CurrentPosition.X * Map.TileSize.X,
                                (CurrentPosition.Z + 0.5f) * Map.LayerHeight,
                                CurrentPosition.Y * Map.TileSize.Y);

                            ActiveUnit.Unit3DSprite.UnitEffect3D.Parameters["Greyscale"].SetValue(true);

                            ActiveUnit.Unit3DSprite.Draw(GameScreen.GraphicsDevice);
                        }
                        else
                        {
                            ActiveUnit.Unit3DModel.PlayAnimation("Walking");
                            Matrix RotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(ActiveUnit.Components.Direction));

                            Vector3 FinalPosition = new Vector3(CurrentPosition.X * Map.TileSize.X, CurrentPosition.Y * Map.TileSize.Y, CurrentPosition.Z * Map.LayerHeight);

                            if (ActiveUnit.UnitStat.ArrayMapSize.GetLength(0) > 1)
                            {
                                FinalPosition.X += (Map.TileSize.X / 2f) * ActiveUnit.UnitStat.ArrayMapSize.GetLength(0) - Map.TileSize.X / 2;
                                FinalPosition.Y += (Map.TileSize.Y / 2f) * ActiveUnit.UnitStat.ArrayMapSize.GetLength(1) - Map.TileSize.Y / 2;
                            }

                            ActiveUnit.Unit3DModel.Draw(View, PolygonEffect.Projection, RotationMatrix
                                * Matrix.CreateTranslation(FinalPosition.X, FinalPosition.Z, FinalPosition.Y));
                        }
                    }
                    else
                    {
                        Color UnitColor;
                        if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.Colored)
                            UnitColor = Map.ListPlayer[P].Color;
                        else
                            UnitColor = Color.White;

                        Vector3 CurrentPosition = ActiveUnit.Position;

                        if (ActiveUnit.Components.ItemHeld != null)
                        {
                            ActiveUnit.Components.ItemHeld.Item3D.SetViewMatrix(View);

                            ActiveUnit.Components.ItemHeld.Item3D.SetPosition(
                                CurrentPosition.X  * Map.TileSize.X,
                                (CurrentPosition.Z + 1f) * Map.LayerHeight,
                                (CurrentPosition.Y - 0.5f) * Map.TileSize.Y);

                            ActiveUnit.Components.ItemHeld.Item3D.Draw(GameScreen.GraphicsDevice);
                        }

                        if (ActiveUnit.Unit3DModel == null)
                        {
                            ActiveUnit.Unit3DSprite.SetViewMatrix(View);

                            ActiveUnit.Unit3DSprite.SetPosition(
                                CurrentPosition.X * Map.TileSize.X,
                                (CurrentPosition.Z + 0.5f) * Map.LayerHeight,
                                CurrentPosition.Y * Map.TileSize.Y);

                            ActiveUnit.Unit3DSprite.UnitEffect3D.Parameters["Greyscale"].SetValue(!ActiveUnit.CanMove && P == Map.ActivePlayerIndex);

                            ActiveUnit.Unit3DSprite.Draw(GameScreen.GraphicsDevice);
                        }
                        else
                        {
                            ActiveUnit.Unit3DModel.PlayAnimation("Idle");
                            Matrix RotationMatrix = RotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(ActiveUnit.Components.Direction));

                            Vector3 FinalPosition = new Vector3(CurrentPosition.X * Map.TileSize.X, CurrentPosition.Y * Map.TileSize.Y, CurrentPosition.Z * Map.LayerHeight);

                            if (ActiveUnit.UnitStat.ArrayMapSize.GetLength(0) > 1)
                            {
                                FinalPosition.X += (Map.TileSize.X / 2f) * ActiveUnit.UnitStat.ArrayMapSize.GetLength(0) - Map.TileSize.X / 2;
                                FinalPosition.Y += (Map.TileSize.Y / 2f) * ActiveUnit.UnitStat.ArrayMapSize.GetLength(1) - Map.TileSize.Y / 2;
                            }

                            ActiveUnit.Unit3DModel.Draw(View, PolygonEffect.Projection, RotationMatrix
                                * Matrix.CreateTranslation(FinalPosition.X, FinalPosition.Z, FinalPosition.Y));
                        }
                    }
                }
            }
        }

        private void DrawDrawablePoints(CustomSpriteBatch g)
        {
            ColorEffect.CurrentTechnique.Passes[0].Apply();

            foreach (KeyValuePair<Vector4, List<Tile3D>> DrawablePointPerColor in DicDrawablePointPerColor)
            {
                ColorEffect.Parameters["Color"].SetValue(DrawablePointPerColor.Key);

                ColorEffect.CurrentTechnique.Passes[0].Apply();

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

        private void DrawDelayedAttacks(CustomSpriteBatch g, Matrix ViewProjection)
        {
            int BorderX = (int)(Map.TileSize.X * 0.1);
            int BorderY = (int)(Map.TileSize.Y * 0.1);
            Point BorderSize = new Point((int)(Map.TileSize.X * 0.8), (int)(Map.TileSize.Y * 0.8));

            foreach (DelayedAttack ActiveAttack in Map.ListDelayedAttack)
            {
                if (ActiveAttack.DrawableAttackPosition3D == null)
                {
                    ActiveAttack.DrawableAttackPosition3D = new Tile3DHolder(ColorEffect, GameScreen.sprPixel);
                    ActiveAttack.DrawableAttackPosition3D.Effect3D.Parameters["Color"].SetValue(Color.FromNonPremultiplied(139, 0, 0, 190).ToVector4());
                    foreach (MovementAlgorithmTile ActivePosition in ActiveAttack.ListAttackPosition)
                    {
                        ActiveAttack.DrawableAttackPosition3D.AddTile(Map.CreateTile3D(0, ActivePosition.WorldPosition, Point.Zero, BorderSize, Map.TileSize, 0f));
                    }

                    ActiveAttack.DrawableAttackPosition3D.Finish(GameScreen.GraphicsDevice);

                    if (ActiveAttack.ActiveAttack.ExplosionOption.ExplosionRadius > 0)
                    {
                        ActiveAttack.DrawableAttackExplosionPosition3D = new Tile3DHolder(ColorEffect, GameScreen.sprPixel);
                        ActiveAttack.DrawableAttackExplosionPosition3D.Effect3D.Parameters["Color"].SetValue(Color.FromNonPremultiplied(139, 0, 0, 140).ToVector4());
                        foreach (MovementAlgorithmTile ActivePosition in ActiveAttack.ListAttackPosition)
                        {
                            float X = ActivePosition.WorldPosition.X;
                            float Y = ActivePosition.WorldPosition.Y;
                            float Z = ActivePosition.WorldPosition.Z * Map.LayerHeight + 0.3f;

                            for (float OffsetX = -ActiveAttack.ActiveAttack.ExplosionOption.ExplosionRadius; OffsetX < ActiveAttack.ActiveAttack.ExplosionOption.ExplosionRadius; ++OffsetX)
                            {
                                for (float OffsetY = -ActiveAttack.ActiveAttack.ExplosionOption.ExplosionRadius; OffsetY < ActiveAttack.ActiveAttack.ExplosionOption.ExplosionRadius; ++OffsetY)
                                {
                                    if (Math.Abs(OffsetX) + Math.Abs(OffsetY) < ActiveAttack.ActiveAttack.ExplosionOption.ExplosionRadius)
                                    {
                                        ActiveAttack.DrawableAttackPosition3D.AddTile(Map.CreateTile3D(0, ActivePosition.WorldPosition, Point.Zero, BorderSize, Map.TileSize, 0f));
                                    }
                                }
                            }
                        }
                        ActiveAttack.DrawableAttackExplosionPosition3D.Finish(GameScreen.GraphicsDevice);
                    }
                }

                ActiveAttack.DrawableAttackPosition3D.Effect3D.Parameters["ViewProjection"].SetValue(ViewProjection);

                ActiveAttack.DrawableAttackPosition3D.Draw(g.GraphicsDevice);

                if (ActiveAttack.DrawableAttackExplosionPosition3D != null)
                {
                    ActiveAttack.DrawableAttackExplosionPosition3D.Effect3D.Parameters["ViewProjection"].SetValue(ViewProjection);

                    ActiveAttack.DrawableAttackExplosionPosition3D.Draw(g.GraphicsDevice);
                }
            }
        }

        private void DrawPERAttacks(CustomSpriteBatch g, Matrix View)
        {
            foreach (PERAttack ActiveAttack in Map.ListPERAttack)
            {
                Vector3 CurrentPosition = ActiveAttack.Position;

                if (ActiveAttack.IsOnGround)
                {
                    CurrentPosition = Map.GetFinalPosition(CurrentPosition);
                }

                if (ActiveAttack.Map3DComponent != null)
                {
                    ActiveAttack.Map3DComponent.SetPosition(CurrentPosition.X * Map.TileSize.X, CurrentPosition.Z * Map.LayerHeight + 16, CurrentPosition.Y * Map.TileSize.Y);
                    ActiveAttack.Map3DComponent.SetViewMatrix(View);
                    ActiveAttack.Map3DComponent.Draw(g.GraphicsDevice);
                }
                else if (ActiveAttack.Unit3DModel != null)
                {
                    ActiveAttack.Unit3DModel.Draw(View, PolygonEffect.Projection, Matrix.CreateTranslation(CurrentPosition.X * Map.TileSize.X, CurrentPosition.Z * Map.LayerHeight, CurrentPosition.Y * Map.TileSize.Y));
                }
            }
        }

        private void DrawDamageNumbers(CustomSpriteBatch g, Matrix View)
        {
            foreach (KeyValuePair<string, Vector3> ActiveAttack in DicDamageNumberByPosition)
            {
                Vector3 Visible3DPosition = new Vector3(ActiveAttack.Value.X, ActiveAttack.Value.Z * Map.LayerHeight, ActiveAttack.Value.Y);
                Vector3 Position = new Vector3(Visible3DPosition.X * Map.TileSize.X, Visible3DPosition.Y + 16, Visible3DPosition.Z * Map.TileSize.Y);

                Vector3 Position2D = g.GraphicsDevice.Viewport.Project(Position, PolygonEffect.Projection, View, Matrix.Identity);
                g.DrawString(Map.fntNonDemoDamage, ActiveAttack.Key, new Vector2(Position2D.X, Position2D.Y), Color.White);
            }
        }

        private void DrawVehicles(CustomSpriteBatch g, Matrix View)
        {
            foreach (Vehicle ActiveVehicle in Map.ListVehicle)
            {
                PolygonEffect.World = ActiveVehicle.World;
                PolygonEffect.Texture = ActiveVehicle.sprVehicle;
                PolygonEffect.CurrentTechnique.Passes[0].Apply();

                ActiveVehicle.DrawVehicle(g.GraphicsDevice, View, PolygonEffect.Projection);
            }

            PolygonEffect.World = Map.World;
        }

        public void EndDraw(CustomSpriteBatch g)
        {
        }
    }
}
