using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public class DeathmatchMap2DHolder : ILayerHolderDrawable
    {
        protected Point MapSize { get { return Map.MapSize; } }

        protected Point TileSize { get { return Map.TileSize; } }

        protected Vector3 CameraPosition { get { return Map.CameraPosition; } }

        private readonly DeathmatchMap Map;

        private Dictionary<Color, List<MovementAlgorithmTile>> DicDrawablePointPerColor;
        private Dictionary<string, Vector3> DicDamageNumberByPosition;

        Texture2D RippleTexture;
        Texture2D Droplet;
        Texture2D sprSkyTexture;
        Effect RippleEffect;
        RenderTarget2D RippleRenderTarget;
        RenderTargetCube RefCubeMap;
        Model SkyModel;
        Matrix[] SkyBones;
        double Time;
        private Dictionary<int, Tile2DHolder> DicTile3DByTileset;

        public DeathmatchMap2DHolder(DeathmatchMap Map, LayerHolderDeathmatch LayerManager)
        {
            this.Map = Map;
            DicTile3DByTileset = new Dictionary<int, Tile2DHolder>();
            DicDrawablePointPerColor = new Dictionary<Color, List<MovementAlgorithmTile>>();
            DicDamageNumberByPosition = new Dictionary<string, Vector3>();
            RippleTexture = Map.Content.Load<Texture2D>("RippleNormal");
            sprSkyTexture = Map.Content.Load<Texture2D>("3D/Textures/Sky");
            Droplet = Map.Content.Load<Texture2D>("3D/Textures/Glass Droplets");

            RefCubeMap = new RenderTargetCube(GameScreen.GraphicsDevice, 256, true, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            SkyModel = Map.Content.Load<Model>("3D/Models/Sphere");
            SkyBones = new Matrix[this.SkyModel.Bones.Count];
            SkyModel.CopyAbsoluteBoneTransformsTo(SkyBones);

            RippleRenderTarget = new RenderTarget2D(GameScreen.GraphicsDevice, Constants.Width, Constants.Height, false, SurfaceFormat.Color, DepthFormat.None);

            Effect WetEffect = Map.Content.Load<Effect>("Shaders/Water");
            RippleEffect = Map.Content.Load<Effect>("Shaders/Ripple");

            Matrix View = Matrix.Identity;

            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 0, -1f);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            Projection = HalfPixelOffset * Projection;

            RippleEffect.Parameters["View"].SetValue(View);
            RippleEffect.Parameters["Projection"].SetValue(Projection);
            RippleEffect.Parameters["World"].SetValue(Matrix.Identity);
            RippleEffect.Parameters["RippleTexture"].SetValue(RippleTexture);
            RippleEffect.Parameters["Time"].SetValue(81.139168f);
            RippleEffect.Parameters["TextureSize"].SetValue(256f);
            RippleEffect.Parameters["RainIntensity"].SetValue(1f);

            WetEffect.Parameters["DropletsTexture"].SetValue(Droplet);
            WetEffect.Parameters["View"].SetValue(View);
            WetEffect.Parameters["Projection"].SetValue(Projection);
            WetEffect.Parameters["World"].SetValue(Matrix.Identity);
            WetEffect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(Matrix.Identity));

            WetEffect.Parameters["MaxPuddleDepth"].SetValue(1f);
            WetEffect.Parameters["WaterLevel"].SetValue(0.7f);
            WetEffect.Parameters["MinWaterLevel"].SetValue(0.5f);
            WetEffect.Parameters["WetLevel"].SetValue(1f);
            WetEffect.Parameters["RainIntensity"].SetValue(1f);

            WetEffect.Parameters["CameraPosition"].SetValue(new Vector4(700, 600, 2000, 1f));
            WetEffect.Parameters["LightPosition"].SetValue(new Vector3(1000, 3500, 3500));
            WetEffect.Parameters["LightIntensity"].SetValue(1f);
            WetEffect.Parameters["LightDirection"].SetValue(Vector3.Normalize(new Vector3(80, 15, 8)));

            for (int L = 0; L < LayerManager.ListLayer.Count; L++)
            {
                CreateMap(Map, LayerManager.ListLayer[L], WetEffect);
            }
        }

        protected void CreateMap(DeathmatchMap Map, MapLayer Owner, Effect WetEffect)
        {
            for (int X = Map.MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
                {
                    Terrain ActiveTerrain = Owner.ArrayTerrain[X, Y];
                    DrawableTile ActiveTile = Owner.ArrayTerrain[X, Y].DrawableTile;

                    if (!DicTile3DByTileset.ContainsKey(ActiveTile.TilesetIndex))
                    {
                        DicTile3DByTileset.Add(ActiveTile.TilesetIndex, new Tile2DHolder(Map.ListTilesetPreset[ActiveTile.TilesetIndex].TilesetName, Map.Content, WetEffect));
                    }

                    DicTile3DByTileset[ActiveTile.TilesetIndex].AddTile(ActiveTerrain);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            Time += gameTime.ElapsedGameTime.TotalSeconds;
            RippleEffect.Parameters["Time"].SetValue((float)Time);
            foreach (var a in DicTile3DByTileset.Values)
            {
                a.WetEffect.Parameters["Time"].SetValue((float)Time);
            }
            RippleEffect.Parameters["RainIntensity"].SetValue(1f);
            DicDrawablePointPerColor.Clear();
            DicDamageNumberByPosition.Clear();
        }

        public Point GetMenuPosition()
        {
            Point BaseMenuPosition;

            BaseMenuPosition.X = (int)((Map.CursorPosition.X - Map.CameraPosition.X + 1) * Map.TileSize.X);
            BaseMenuPosition.Y = (int)((Map.CursorPosition.Y - Map.CameraPosition.Y) * Map.TileSize.Y);

            return BaseMenuPosition;
        }

        public void AddDrawablePoints(List<MovementAlgorithmTile> ListPoint, Color PointColor)
        {
            DicDrawablePointPerColor.Add(PointColor, ListPoint);
        }

        public void AddDrawablePath(List<MovementAlgorithmTile> ListPoint)
        {
        }

        public void AddDamageNumber(string Damage, Vector3 Position)
        {
            DicDamageNumberByPosition.Add(Damage, Position);
        }

        public void SetWorld(Matrix NewWorld)
        {
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
            DrawRipples(g);
            DrawSkyCube();
        }

        void DrawSky(Matrix ViewMatrix)
        {
            foreach (ModelMesh mesh in SkyModel.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.  
                foreach (BasicEffect effectB in mesh.Effects)
                {
                    effectB.TextureEnabled = true;
                    effectB.EnableDefaultLighting();
                    effectB.PreferPerPixelLighting = true;


                    effectB.AmbientLightColor = new Vector3(1.0f, 1.0f, 1.0f);
                    effectB.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                    effectB.SpecularColor = new Vector3(1.0f, 0.0f, 0.0f);
                    effectB.SpecularPower = 32;
                    effectB.Texture = sprSkyTexture;
                    effectB.World = SkyBones[mesh.ParentBone.Index] * Matrix.CreateRotationX((float)Time / 10) * Matrix.CreateRotationY((float)Time / 7) * Matrix.CreateScale(30);
                    effectB.View = ViewMatrix;
                    effectB.Projection = Matrix.CreateOrthographicOffCenter(-Constants.Width / 2, Constants.Width / 2, Constants.Height / 2, -Constants.Height / 2, 5000, -5000f);
                }
                mesh.Draw();
            }
        }

        private void DrawSkyCube()
        {
            GameScreen.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            //Sky cube
            for (int i = 0; i < 6; i++)
            {
                // render the scene to all cubemap faces
                CubeMapFace cubeMapFace = (CubeMapFace)i;
                Matrix CubeViewMatrix = Matrix.Identity;

                switch (cubeMapFace)
                {
                    case CubeMapFace.NegativeX:
                        {
                            CubeViewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Left, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.NegativeY:
                        {
                            CubeViewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Down, Vector3.Forward);
                            break;
                        }
                    case CubeMapFace.NegativeZ:
                        {
                            CubeViewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Backward, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.PositiveX:
                        {
                            CubeViewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Right, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.PositiveY:
                        {
                            CubeViewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Up, Vector3.Backward);
                            break;
                        }
                    case CubeMapFace.PositiveZ:
                        {
                            CubeViewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Forward, Vector3.Up);
                            break;
                        }
                }

                // Set the cubemap render target, using the selected face
                GameScreen.GraphicsDevice.SetRenderTarget(RefCubeMap, cubeMapFace);
                GameScreen.GraphicsDevice.Clear(Color.White);
                DrawSky(CubeViewMatrix);
            }

            GameScreen.GraphicsDevice.SetRenderTarget(null);

            GameScreen.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        private void DrawRipples(CustomSpriteBatch g)
        {
            g.End();
            g.GraphicsDevice.SetRenderTarget(RippleRenderTarget);
            g.GraphicsDevice.Clear(Color.Transparent);
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, RippleEffect);
            g.Draw(RippleTexture, new Rectangle(0, 0, Constants.Width, Constants.Height), Color.White);
            g.End();
            g.GraphicsDevice.SetRenderTarget(null);
            g.Begin();
        }

        public void Draw(CustomSpriteBatch g, MapLayer Owner, int LayerIndex, bool IsSubLayer)
        {
            if (!Owner.IsVisible)
            {
                return;
            }

            g.End();

            foreach (var a in DicTile3DByTileset.Values)
            {
                a.WetEffect.Parameters["RippleTexture"].SetValue(RippleRenderTarget);
                a.WetEffect.Parameters["ReflectionCubeMap"].SetValue(RefCubeMap);
                a.Draw(g);
            }

            /*g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, WetEffect);

            for (int X = Owner.LayerGrid.ArrayTile.GetLength(0) - 1; X >= 0; --X)
            {
                for (int Y = Owner.LayerGrid.ArrayTile.GetLength(1) - 1; Y >= 0; --Y)
                {
                    Color FinalColor = Color.White;
                    float FinalHeight = Owner.ArrayTerrain[X, Y].WorldPosition.Z;

                    if (FinalHeight > CameraPosition.Z)
                    {
                        FinalColor.A = (byte)Math.Min(255, 255 - (FinalHeight - CameraPosition.Z) * 255);
                    }

                    g.Draw(Map.ListTileSet[Owner.LayerGrid.ArrayTile[X, Y].TilesetIndex],
                        new Vector2((X - CameraPosition.X) * TileSize.X, (Y - CameraPosition.Y) * TileSize.Y),
                        Owner.LayerGrid.ArrayTile[X, Y].Origin, FinalColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, Owner.LayerGrid.Depth);
                }
            }

            g.End();*/
            g.Begin();
            for (int P = 0; P < Owner.ListProp.Count; ++P)
            {
                Owner.ListProp[P].Draw(g);
            }

            if (Map.ShowUnits && !IsSubLayer)
            {
                DrawDrawablePoints(g);

                DrawCursor(g);
            }

            for (int L = 0; L < Owner.ListSubLayer.Count; L++)
            {
                Draw(g, Owner.ListSubLayer[L], LayerIndex, true);
            }
        }

        public void DrawUnitMap(CustomSpriteBatch g, Color PlayerColor, UnitMapComponent ActiveSquad, bool IsGreyed)
        {
            //If it's dead, don't draw it.
            if (!ActiveSquad.IsActive)
                return;

            float PosZ = ActiveSquad.Z;

            if (Map.MovementAnimation.Contains(ActiveSquad))
            {
                Vector3 CurrentPosition = Map.MovementAnimation.GetPosition(ActiveSquad);
                float PosX = (CurrentPosition.X - CameraPosition.X) * TileSize.X;
                float PosY = (CurrentPosition.Y - CameraPosition.Y) * TileSize.Y;

                if (ActiveSquad.IsFlying)
                {
                    g.Draw(Map.sprUnitHover, new Vector2(PosX, PosY), Color.White);
                    PosY -= 7;
                }

                ActiveSquad.Draw2DOnMap(g, new Vector3(PosX, PosY, PosZ), Color.White);
                g.End();
                g.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                ActiveSquad.Draw2DOnMap(g, new Vector3(PosX, PosY, PosZ), Color.White);
                g.End();
                g.Begin();
            }
            else
            {
                Color UnitColor;
                if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.Colored)
                    UnitColor = PlayerColor;
                else
                    UnitColor = Color.White;

                float PosX = (ActiveSquad.X - CameraPosition.X) * TileSize.X;
                float PosY = (ActiveSquad.Y - CameraPosition.Y) * TileSize.Y;

                if (ActiveSquad.IsFlying)
                {
                    g.Draw(Map.sprUnitHover, new Vector2(PosX, PosY), Color.White);
                    PosY -= 7;
                }
                if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.NonColoredWithBorder)
                {
                    Vector2 TextureRealSize = new Vector2(ActiveSquad.Width, ActiveSquad.Height);
                    Vector2 TextureOuputSize = new Vector2(TextureRealSize.X + 2, TextureRealSize.Y + 2);

                    Vector2 PixelSize = new Vector2(1 / TextureOuputSize.X, 1 / TextureOuputSize.Y);
                    Vector2 TextureScale = TextureOuputSize / TextureRealSize;

                    Map.fxOutline.Parameters["TextureScale"].SetValue(TextureScale);
                    Map.fxOutline.Parameters["OffsetScale"].SetValue(PixelSize * TextureScale);

                    g.End();
                    g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, Map.fxOutline);

                    ActiveSquad.Draw2DOnMap(g, new Vector3(PosX - 1, PosY - 1, PosZ), (int)TextureOuputSize.X, (int)TextureOuputSize.Y, PlayerColor);
                    g.End();
                    g.Begin();
                }
                //Unit can't move, grayed.
                if (IsGreyed)
                {
                    g.End();
                    g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, Map.fxGrayscale);

                    ActiveSquad.Draw2DOnMap(g, new Vector3(PosX, PosY, PosZ), Color.White);

                    g.End();
                    g.Begin();

                    if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.Colored)
                        ActiveSquad.Draw2DOnMap(g, new Vector3(PosX, PosY, PosZ), Color.FromNonPremultiplied(UnitColor.R, UnitColor.G, UnitColor.B, 140));
                }
                else
                {
                    ActiveSquad.Draw2DOnMap(g, new Vector3(PosX, PosY, PosZ), UnitColor);
                }

                ActiveSquad.DrawExtraOnMap(g, new Vector3(PosX, PosY, PosZ), Color.White);
            }
        }

        private void DrawCursor(CustomSpriteBatch g)
        {
            //Draw cursor.
            g.Draw(Map.sprCursor, new Vector2((Map.CursorPositionVisible.X - CameraPosition.X) * TileSize.X, (Map.CursorPositionVisible.Y - CameraPosition.Y) * TileSize.Y), Color.White);
        }

        private void DrawDrawablePoints(CustomSpriteBatch g)
        {
            foreach (KeyValuePair<Color, List<MovementAlgorithmTile>> DrawablePointPerColor in DicDrawablePointPerColor)
            {
                foreach (MovementAlgorithmTile DrawablePoint in DrawablePointPerColor.Value)
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle((int)(DrawablePoint.WorldPosition.X - CameraPosition.X) * TileSize.X, (int)(DrawablePoint.WorldPosition.Y - CameraPosition.Y) * TileSize.Y, TileSize.X, TileSize.Y), DrawablePointPerColor.Key);
                }
            }
        }

        public void Reset()
        {
            //Nothing to do.
        }

        public void Draw(CustomSpriteBatch g)
        {
            if (Map.ShowLayerIndex == -1)
            {
                for (int L = 0; L < 1; L++)
                {
                    Draw(g, Map.LayerManager.ListLayer[L], L, false);
                    DrawEditorOverlay(g, Map.LayerManager.ListLayer[L], L, false);
                }
            }
            else
            {
                Draw(g, Map.LayerManager.ListLayer[Map.ShowLayerIndex], Map.ShowLayerIndex, false);
                DrawEditorOverlay(g, Map.LayerManager.ListLayer[Map.ShowLayerIndex], Map.ShowLayerIndex, false);
            }

            DrawDelayedAttacks(g);

            DrawPERAttacks(g);

            DrawPlayers(g);

            DrawDamageNumbers(g);
        }

        public void DrawPlayers(CustomSpriteBatch g)
        {
            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                for (int S = 0; S < Map.ListPlayer[P].ListSquad.Count; S++)
                {
                    DrawUnitMap(g, Map.ListPlayer[P].Color, Map.ListPlayer[P].ListSquad[S], !Map.ListPlayer[P].ListSquad[S].CanMove && P == Map.ActivePlayerIndex);
                }
            }
        }

        private void DrawDelayedAttacks(CustomSpriteBatch g)
        {
            int BorderX = (int)(TileSize.X * 0.1);
            int BorderY = (int)(TileSize.Y * 0.1);

            foreach (DelayedAttack ActiveAttack in Map.ListDelayedAttack)
            {
                foreach (MovementAlgorithmTile ActivePosition in ActiveAttack.ListAttackPosition)
                {
                    g.Draw(GameScreen.sprPixel,
                        new Rectangle(
                            (int)(ActivePosition.WorldPosition.X - CameraPosition.X) * TileSize.X + BorderX,
                            (int)(ActivePosition.WorldPosition.Y - CameraPosition.Y) * TileSize.Y + BorderY,
                            TileSize.X - BorderX * 2,
                            TileSize.Y - BorderY * 2), Color.FromNonPremultiplied(139, 0, 0, 190));
                }
            }
        }

        private void DrawPERAttacks(CustomSpriteBatch g)
        {
            foreach (PERAttack ActiveAttack in Map.ListPERAttack)
            {
                ActiveAttack.ActiveAttack.PERAttributes.ProjectileAnimation.Draw(g, new Vector2(ActiveAttack.Position.X, ActiveAttack.Position.Y));
            }
        }

        private void DrawDamageNumbers(CustomSpriteBatch g)
        {
            foreach (KeyValuePair<string, Vector3> ActiveAttack in DicDamageNumberByPosition)
            {
                g.DrawString(Map.fntNonDemoDamage, ActiveAttack.Key, new Vector2(ActiveAttack.Value.X, ActiveAttack.Value.Y), Color.White);
            }
        }

        private void DrawEditorOverlay(CustomSpriteBatch g, MapLayer Owner, int LayerIndex, bool IsSubLayer)
        {
            if (Owner.IsVisible)
            {
                if (Map.ShowTerrainType)
                {
                    int IndexOfLayer = LayerIndex;
                    if (Map.ShowLayerIndex >= 0 && IndexOfLayer != -1)
                    {
                        IndexOfLayer = 0;
                        if (IsSubLayer)
                        {
                            IndexOfLayer = 1;
                        }
                    }
                    else if (IsSubLayer)
                    {
                        IndexOfLayer = 3;
                    }
                    float XOffset = (IndexOfLayer % 3) * Map.TileSize.X / 3;
                    float YOffset = (IndexOfLayer / 3) * Map.TileSize.Y / 3;
                    for (int Y = 0; Y < Map.MapSize.Y; Y++)
                    {
                        for (int X = 0; X < Map.MapSize.X; X++)
                        {
                            Color TextColor = Color.White;
                            switch (Owner.ArrayTerrain[X, Y].TerrainTypeIndex)
                            {
                                case 0:
                                    TextColor = Color.DeepSkyBlue;
                                    break;
                                case 1:
                                    TextColor = Color.White;
                                    break;
                                case 2:
                                    TextColor = Color.Navy;
                                    break;
                                case 3:
                                    TextColor = Color.DarkGray;
                                    break;
                                case 4:
                                    TextColor = Color.Red;
                                    break;
                                case 5:
                                    TextColor = Color.Yellow;
                                    break;
                            }
                            TextHelper.DrawText(g, Owner.ArrayTerrain[X, Y].TerrainTypeIndex.ToString(),
                                new Vector2((X - Map.CameraPosition.X) * Map.TileSize.X + XOffset,
                                (Y - Map.CameraPosition.Y) * Map.TileSize.Y + YOffset), TextColor);
                        }
                    }
                }

                if (Map.ShowTerrainHeight)
                {
                    int IndexOfLayer = LayerIndex;
                    if (IsSubLayer)
                    {
                        IndexOfLayer = 0;
                    }
                    float XOffset = (IndexOfLayer % 3) * Map.TileSize.X / 3;
                    float YOffset = (IndexOfLayer / 3) * Map.TileSize.Y / 3;
                    for (int Y = 0; Y < Map.MapSize.Y; Y++)
                    {
                        for (int X = 0; X < Map.MapSize.X; X++)
                        {
                            Color TextColor = Color.White;
                            if (Owner.ArrayTerrain[X, Y].Height >= 2)
                            {
                                TextColor = Color.Red;
                            }
                            else if (Owner.ArrayTerrain[X, Y].Height >= 1)
                            {
                                TextColor = Color.Orange;
                            }
                            else if (Owner.ArrayTerrain[X, Y].Height >= 0.75)
                            {
                                TextColor = Color.Yellow;
                            }
                            else if (Owner.ArrayTerrain[X, Y].Height >= 0.5)
                            {
                                TextColor = Color.Green;
                            }
                            else if (Owner.ArrayTerrain[X, Y].Height > 0)
                            {
                                TextColor = Color.SkyBlue;
                            }

                            TextHelper.DrawText(g, Owner.ArrayTerrain[X, Y].Height.ToString(),
                                new Vector2((X - Map.CameraPosition.X) * Map.TileSize.X + XOffset,
                                (Y - Map.CameraPosition.Y) * Map.TileSize.Y + YOffset), TextColor);
                        }
                    }
                }

                foreach (SubMapLayer ActiveSubLayer in Owner.ListSubLayer)
                {
                    DrawEditorOverlay(g, ActiveSubLayer, LayerIndex, true);
                }

                if (!Map.ShowUnits)
                {
                    Color BrushPlayer = Color.FromNonPremultiplied(30, 144, 255, 180);
                    Color BrushEnemy = Color.FromNonPremultiplied(255, 0, 0, 180);
                    Color BrushNeutral = Color.FromNonPremultiplied(255, 255, 0, 180);
                    Color BrushAlly = Color.FromNonPremultiplied(191, 255, 0, 180);
                    Color BrushMapSwitchEventPoint = Color.FromNonPremultiplied(191, 255, 0, 180);
                    Color BrushTeleportPoint = Color.FromNonPremultiplied(70, 13, 13, 180);

                    for (int i = 0; i < Owner.ListSingleplayerSpawns.Count; i++)
                    {
                        g.Draw(GameScreen.sprPixel, new Rectangle((int)(Owner.ListSingleplayerSpawns[i].Position.X - Map.CameraPosition.X) * Map.TileSize.X,
                                                      (int)(Owner.ListSingleplayerSpawns[i].Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y,
                                                       Map.TileSize.X, Map.TileSize.Y),
                                                      null,
                                        BrushPlayer, 0f, Vector2.Zero, SpriteEffects.None, 0.001f);

                        g.DrawString(Map.fntArial9, Owner.ListSingleplayerSpawns[i].Tag,
                            new Vector2((Owner.ListSingleplayerSpawns[i].Position.X - Map.CameraPosition.X) * Map.TileSize.X + 10,
                                        (Owner.ListSingleplayerSpawns[i].Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y + 10),
                            Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }

                    for (int i = 0; i < Owner.ListMultiplayerSpawns.Count; i++)
                    {
                        g.Draw(GameScreen.sprPixel, new Rectangle((int)(Owner.ListMultiplayerSpawns[i].Position.X - Map.CameraPosition.X) * Map.TileSize.X,
                                                      (int)(Owner.ListMultiplayerSpawns[i].Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y,
                                                       Map.TileSize.X, Map.TileSize.Y), null,
                                        BrushPlayer, 0f, Vector2.Zero, SpriteEffects.None, 0.001f);

                        g.DrawString(Map.fntArial9, Owner.ListMultiplayerSpawns[i].Tag,
                            new Vector2((Owner.ListMultiplayerSpawns[i].Position.X - Map.CameraPosition.X) * Map.TileSize.X + 10,
                                        (Owner.ListMultiplayerSpawns[i].Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y + 10),
                            Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }

                    for (int S = 0; S < Owner.ListMapSwitchPoint.Count; S++)
                    {
                        g.Draw(GameScreen.sprPixel, new Rectangle((int)(Owner.ListMapSwitchPoint[S].Position.X - Map.CameraPosition.X) * Map.TileSize.X,
                                                       (int)(Owner.ListMapSwitchPoint[S].Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y,
                                                       Map.TileSize.X, Map.TileSize.Y), null,
                                        BrushMapSwitchEventPoint, 0f, Vector2.Zero, SpriteEffects.None, 0.001f);

                        g.DrawString(Map.fntArial9, Owner.ListMapSwitchPoint[S].Tag,
                            new Vector2((Owner.ListMapSwitchPoint[S].Position.X - Map.CameraPosition.X) * Map.TileSize.X + 10,
                                        (Owner.ListMapSwitchPoint[S].Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y + 10),
                            Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }

                    for (int T = 0; T < Owner.ListTeleportPoint.Count; T++)
                    {
                        g.Draw(GameScreen.sprPixel, new Rectangle((int)(Owner.ListTeleportPoint[T].Position.X - Map.CameraPosition.X) * Map.TileSize.X,
                                                       (int)(Owner.ListTeleportPoint[T].Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y,
                                                       Map.TileSize.X, Map.TileSize.Y), null,
                                        BrushTeleportPoint, 0f, Vector2.Zero, SpriteEffects.None, 0.001f);

                        g.DrawString(Map.fntArial9, Owner.ListTeleportPoint[T].Tag,
                            new Vector2((Owner.ListTeleportPoint[T].Position.X - Map.CameraPosition.X) * Map.TileSize.X + 10,
                                        (Owner.ListTeleportPoint[T].Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y + 10),
                            Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                }

                if (Map.ShowGrid)
                {
                    //Draw the vertical lines for the grid.
                    for (int X = 0; X < Map.MapSize.X; X++)
                        g.Draw(GameScreen.sprPixel, new Rectangle(X * Map.TileSize.X, 0,
                                                       1, Map.MapSize.Y * Map.TileSize.Y), Color.Black);
                    //Draw the horizontal lines for the grid.
                    for (int Y = 0; Y < Map.MapSize.Y; Y++)
                        g.Draw(GameScreen.sprPixel, new Rectangle(0, Y * Map.TileSize.Y,
                                                   Map.MapSize.X * Map.TileSize.X, 1), Color.Black);
                }
            }
        }

        public void EndDraw(CustomSpriteBatch g)
        {
        }
    }
}
