﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using static ProjectEternity.GameScreens.BattleMapScreen.Terrain;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class Map2DDrawable : ILayerHolderDrawable
    {
        protected Point MapSize { get { return Map.MapSize; } }

        protected Point TileSize { get { return Map.TileSize; } }

        protected Vector3 CameraPosition { get { return Map.Camera2DPosition; } }

        private readonly ConquestMap Map;

        private Dictionary<int, Tile2DHolder> DicTile2DByTileset;
        private Dictionary<int, Dictionary<int, Tile2DHolder>> DicTile2DByLayerByTileset;

        private Dictionary<Color, List<MovementAlgorithmTile>> DicDrawablePointPerColor;
        private Dictionary<string, Vector3> DicDamageNumberByPosition;

        public Map2DDrawable(ConquestMap Map, LayerHolderConquest LayerManager)
        {
            this.Map = Map;
            DicDrawablePointPerColor = new Dictionary<Color, List<MovementAlgorithmTile>>();
            DicDamageNumberByPosition = new Dictionary<string, Vector3>();

            CreateMap(Map, LayerManager);
        }

        private void CreateMap(ConquestMap Map, LayerHolderConquest LayerManager)
        {
            DicTile2DByTileset = new Dictionary<int, Tile2DHolder>();
            DicTile2DByLayerByTileset = new Dictionary<int, Dictionary<int, Tile2DHolder>>();

            for (int L = 0; L < LayerManager.ListLayer.Count; L++)
            {
                CreateMap(Map, LayerManager.ListLayer[L], L);
            }
        }

        protected void CreateMap(ConquestMap Map, MapLayer Owner, int LayerIndex)
        {
            foreach (SubMapLayer ActiveSubLayer in Owner.ListSubLayer)
            {
                CreateMap(Map, ActiveSubLayer, LayerIndex);
            }

            if (!DicTile2DByLayerByTileset.ContainsKey(LayerIndex))
            {
                DicTile2DByLayerByTileset.Add(LayerIndex, new Dictionary<int, Tile2DHolder>());
            }

            if (Map.ListTileSet.Count > 0)
            {
                for (int X = Map.MapSize.X - 1; X >= 0; --X)
                {
                    for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
                    {
                        Terrain ActiveTerrain = Owner.ArrayTerrain[X, Y];
                        DrawableTile ActiveTile = Owner.ArrayTile[X, Y];

                        if (!DicTile2DByLayerByTileset[LayerIndex].ContainsKey(ActiveTile.TilesetIndex))
                        {
                            TilesetPreset.TilesetTypes TilesetType = Map.ListTilesetPreset[ActiveTile.TilesetIndex].TilesetType;
                            DicTile2DByLayerByTileset[LayerIndex].Add(ActiveTile.TilesetIndex, new Tile2DHolder(Map.ListTileSet[ActiveTile.TilesetIndex], TilesetType));
                        }

                        if (!DicTile2DByTileset.ContainsKey(ActiveTile.TilesetIndex))
                        {
                            DicTile2DByTileset.Add(ActiveTile.TilesetIndex, DicTile2DByLayerByTileset[LayerIndex][ActiveTile.TilesetIndex]);
                        }

                        if (ActiveTile.ArraySubTile.Length == 0)
                        {
                            DicTile2DByLayerByTileset[LayerIndex][ActiveTile.TilesetIndex].AddTile(ActiveTile.Origin, ActiveTerrain.WorldPosition);
                        }
                        else
                        {
                            float OffsetSize = 1f / (ActiveTile.ArraySubTile.Length / 2f);
                            for (int T = 0; T < ActiveTile.ArraySubTile.Length; T++)
                            {
                                Vector3 TilePosition = ActiveTerrain.WorldPosition;
                                TilePosition.X += OffsetSize * (T % (ActiveTile.ArraySubTile.Length / 2));
                                TilePosition.Y += OffsetSize * (T / (ActiveTile.ArraySubTile.Length / 2));
                                DicTile2DByLayerByTileset[LayerIndex][ActiveTile.TilesetIndex].AddTile(ActiveTile.ArraySubTile[T], TilePosition);
                            }
                        }
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
            DicDrawablePointPerColor.Clear();
            DicDamageNumberByPosition.Clear();
            foreach (KeyValuePair<int, Tile2DHolder> ActiveTileSet in DicTile2DByTileset)
            {
                ActiveTileSet.Value.Update(gameTime);
            }
        }

        public Point GetVisiblePosition(Vector3 Position)
        {
            Point BaseMenuPosition;

            BaseMenuPosition.X = (int)((Position.X - Map.Camera2DPosition.X) * Map.TileSize.X);
            BaseMenuPosition.Y = (int)((Position.Y - Map.Camera2DPosition.Y) * Map.TileSize.Y);

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

        public void Reset()
        {
            CreateMap(Map, Map.LayerManager);
        }

        public void CursorMoved()
        {
        }

        public void UnitMoved(int PlayerIndex)
        {
        }

        public void UnitKilled(int PlayerIndex)
        {
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
            Map.MapEnvironment.BeginDraw(g);
        }

        public void Draw(CustomSpriteBatch g, MapLayer Owner)
        {
            if (!Owner.IsVisible)
            {
                return;
            }

            for (int P = 0; P < Owner.ListProp.Count; ++P)
            {
                Owner.ListProp[P].Draw(g);
            }
        }

        public void DrawUnitMap(CustomSpriteBatch g, Color PlayerColor, UnitMapComponent ActiveUnit, bool IsGreyed)
        {
            //If it's dead, don't draw it.
            if (!ActiveUnit.IsActive)
                return;

            float PosZ = ActiveUnit.Z;

            if (Map.MovementAnimation.Contains(ActiveUnit))
            {
                Vector3 CurrentPosition = Map.MovementAnimation.GetPosition(ActiveUnit);
                float PosX = (CurrentPosition.X - CameraPosition.X) * TileSize.X;
                float PosY = (CurrentPosition.Y - CameraPosition.Y) * TileSize.Y;

                if (ActiveUnit.CurrentTerrainIndex == UnitStats.TerrainAirIndex)
                {
                    g.Draw(Map.sprUnitHover, new Vector2(PosX, PosY), Color.White);
                    PosY -= 7;
                }

                ActiveUnit.Draw2DOnMap(g, new Vector3(PosX, PosY, PosZ), Color.White);
                g.End();
                g.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                ActiveUnit.Draw2DOnMap(g, new Vector3(PosX, PosY, PosZ), Color.White);
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

                float PosX = (ActiveUnit.X - CameraPosition.X) * TileSize.X;
                float PosY = (ActiveUnit.Y - CameraPosition.Y) * TileSize.Y;

                if (ActiveUnit.CurrentTerrainIndex == UnitStats.TerrainAirIndex)
                {
                    g.Draw(Map.sprUnitHover, new Vector2(PosX, PosY), Color.White);
                    PosY -= 7;
                }
                if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.NonColoredWithBorder)
                {
                    Vector2 TextureRealSize = new Vector2(ActiveUnit.Width, ActiveUnit.Height);
                    Vector2 TextureOuputSize = new Vector2(TextureRealSize.X + 2, TextureRealSize.Y + 2);

                    Vector2 PixelSize = new Vector2(1 / TextureOuputSize.X, 1 / TextureOuputSize.Y);
                    Vector2 TextureScale = TextureOuputSize / TextureRealSize;

                    Map.fxOutline.Parameters["TextureScale"].SetValue(TextureScale);
                    Map.fxOutline.Parameters["OffsetScale"].SetValue(PixelSize * TextureScale);

                    g.End();
                    g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, Map.fxOutline);

                    ActiveUnit.Draw2DOnMap(g, new Vector3(PosX - 1, PosY - 1, PosZ), (int)TextureOuputSize.X, (int)TextureOuputSize.Y, PlayerColor);
                    g.End();
                    g.Begin();
                }
                //Unit can't move, grayed.
                if (IsGreyed)
                {
                    g.End();
                    g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, Map.fxGrayscale);

                    ActiveUnit.Draw2DOnMap(g, new Vector3(PosX, PosY, PosZ), Color.White);

                    g.End();
                    g.Begin();

                    if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.Colored)
                        ActiveUnit.Draw2DOnMap(g, new Vector3(PosX, PosY, PosZ), Color.FromNonPremultiplied(UnitColor.R, UnitColor.G, UnitColor.B, 140));
                }
                else
                {
                    ActiveUnit.Draw2DOnMap(g, new Vector3(PosX, PosY, PosZ), UnitColor);
                }

                ActiveUnit.DrawExtraOnMap(g, new Vector3(PosX, PosY, PosZ), Color.White);
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

        public void Draw(CustomSpriteBatch g)
        {
            Map.MapEnvironment.Draw(g);

            g.End();

            DrawMap(g);

            g.Begin();

            Map.MapEnvironment.EndDraw(g);

            DrawItems(g);

            if (Map.ShowUnits)
            {
                DrawDrawablePoints(g);

                DrawCursor(g);
            }

            DrawDelayedAttacks(g);

            DrawPERAttacks(g);

            DrawPlayers(g);

            DrawDamageNumbers(g);
        }

        private void DrawMap(CustomSpriteBatch g)
        {
            if (Map.ShowLayerIndex == -1)
            {
                if (Map.IsEditor)
                {
                    for (int L = 0; L < Map.LayerManager.ListLayer.Count; L++)
                    {
                        foreach (KeyValuePair<int, Tile2DHolder> ActiveTileSet in DicTile2DByLayerByTileset[L])
                        {
                            ActiveTileSet.Value.Draw(g, Map, Map.LayerManager.ListLayer[L].Depth);
                        }
                    }
                }
                else
                {
                    int MaxLayerIndex = Map.LayerManager.ListLayer.Count;

                    for (int L = 0; L < MaxLayerIndex; L++)
                    {
                        foreach (KeyValuePair<int, Tile2DHolder> ActiveTileSet in DicTile2DByLayerByTileset[L])
                        {
                            ActiveTileSet.Value.Draw(g, Map, Map.LayerManager.ListLayer[L].Depth);
                        }
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<int, Tile2DHolder> ActiveTileSet in DicTile2DByLayerByTileset[Map.ShowLayerIndex])
                {
                    ActiveTileSet.Value.Draw(g, Map, 0);
                }
            }
        }

        private void DrawItems(CustomSpriteBatch g)
        {
            if (Map.ShowLayerIndex == -1)
            {
                if (Map.IsEditor)
                {
                    for (int L = 0; L < 1; L++)
                    {
                        DrawEditorOverlay(g, Map.LayerManager.ListLayer[L], L, false);
                    }

                    for (int L = 0; L < Map.LayerManager.ListLayer.Count; L++)
                    {
                        DrawItems(g, Map.LayerManager.ListLayer[L], false);
                    }
                }
                else
                {
                    int MaxLayerIndex = Map.LayerManager.ListLayer.Count;

                    for (int L = 0; L < MaxLayerIndex; L++)
                    {
                        DrawItems(g, Map.LayerManager.ListLayer[L], false);
                    }
                }
            }
            else
            {
                DrawEditorOverlay(g, Map.LayerManager.ListLayer[Map.ShowLayerIndex], Map.ShowLayerIndex, false);

                DrawItems(g, Map.LayerManager.ListLayer[Map.ShowLayerIndex], false);
            }
        }

        public void DrawItems(CustomSpriteBatch g, MapLayer Owner, bool IsSubLayer)
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
                Owner.ListProp[P].Draw(g);
            }

            for (int P = 0; P < Owner.ListAttackPickup.Count; ++P)
            {
                g.Draw(Owner.ListAttackPickup[P].sprWeapon, new Rectangle((int)(Owner.ListAttackPickup[P].Position.X), (int)(Owner.ListAttackPickup[P].Position.Y), Map.TileSize.X, Map.TileSize.Y), null, Color.White);
            }

            for (int I = 0; I < Owner.ListHoldableItem.Count; ++I)
            {
                g.Draw(Owner.ListHoldableItem[I].sprItem, new Rectangle((int)(Owner.ListHoldableItem[I].Position.X), (int)(Owner.ListHoldableItem[I].Position.Y), Map.TileSize.X, Map.TileSize.Y), null, Color.White);
            }
        }

        public void DrawPlayers(CustomSpriteBatch g)
        {
            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                for (int S = 0; S < Map.ListPlayer[P].ListUnit.Count; S++)
                {
                    DrawUnitMap(g, Map.ListPlayer[P].Color, Map.ListPlayer[P].ListUnit[S].Components, !Map.ListPlayer[P].ListUnit[S].CanMove && P == Map.ActivePlayerIndex);
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
                ActiveAttack.ActiveAttack.PERAttributes.ProjectileAnimation.Draw(g, new Vector2(ActiveAttack.Position.X * Map.TileSize.X, ActiveAttack.Position.Y * Map.TileSize.Y));
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
                                new Vector2((X - Map.Camera2DPosition.X) * Map.TileSize.X + XOffset,
                                (Y - Map.Camera2DPosition.Y) * Map.TileSize.Y + YOffset), TextColor);
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
                                new Vector2((X - Map.Camera2DPosition.X) * Map.TileSize.X + XOffset,
                                (Y - Map.Camera2DPosition.Y) * Map.TileSize.Y + YOffset), TextColor);
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

                    for (int i = 0; i < Owner.ListCampaignSpawns.Count; i++)
                    {
                        g.Draw(GameScreen.sprPixel, new Rectangle((int)(Owner.ListCampaignSpawns[i].Position.X - Map.Camera2DPosition.X) * Map.TileSize.X,
                                                      (int)(Owner.ListCampaignSpawns[i].Position.Y - Map.Camera2DPosition.Y) * Map.TileSize.Y,
                                                       Map.TileSize.X, Map.TileSize.Y),
                                                      null,
                                        BrushPlayer, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);

                        g.DrawString(Map.fntArial9, Owner.ListCampaignSpawns[i].Tag,
                            new Vector2((Owner.ListCampaignSpawns[i].Position.X - Map.Camera2DPosition.X) * Map.TileSize.X + 10,
                                        (Owner.ListCampaignSpawns[i].Position.Y - Map.Camera2DPosition.Y) * Map.TileSize.Y + 10),
                            Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.2f);
                    }

                    for (int i = 0; i < Owner.ListMultiplayerSpawns.Count; i++)
                    {
                        g.Draw(GameScreen.sprPixel, new Rectangle((int)(Owner.ListMultiplayerSpawns[i].Position.X - Map.Camera2DPosition.X) * Map.TileSize.X,
                                                      (int)(Owner.ListMultiplayerSpawns[i].Position.Y - Map.Camera2DPosition.Y) * Map.TileSize.Y,
                                                       Map.TileSize.X, Map.TileSize.Y), null,
                                        BrushPlayer, 0f, Vector2.Zero, SpriteEffects.None, 0.001f);

                        g.DrawString(Map.fntArial9, Owner.ListMultiplayerSpawns[i].Tag,
                            new Vector2((Owner.ListMultiplayerSpawns[i].Position.X - Map.Camera2DPosition.X) * Map.TileSize.X + 10,
                                        (Owner.ListMultiplayerSpawns[i].Position.Y - Map.Camera2DPosition.Y) * Map.TileSize.Y + 10),
                            Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }

                    for (int S = 0; S < Owner.ListMapSwitchPoint.Count; S++)
                    {
                        g.Draw(GameScreen.sprPixel, new Rectangle((int)(Owner.ListMapSwitchPoint[S].Position.X - Map.Camera2DPosition.X) * Map.TileSize.X,
                                                       (int)(Owner.ListMapSwitchPoint[S].Position.Y - Map.Camera2DPosition.Y) * Map.TileSize.Y,
                                                       Map.TileSize.X, Map.TileSize.Y), null,
                                        BrushMapSwitchEventPoint, 0f, Vector2.Zero, SpriteEffects.None, 0.001f);

                        g.DrawString(Map.fntArial9, Owner.ListMapSwitchPoint[S].Tag,
                            new Vector2((Owner.ListMapSwitchPoint[S].Position.X - Map.Camera2DPosition.X) * Map.TileSize.X + 10,
                                        (Owner.ListMapSwitchPoint[S].Position.Y - Map.Camera2DPosition.Y) * Map.TileSize.Y + 10),
                            Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }

                    for (int T = 0; T < Owner.ListTeleportPoint.Count; T++)
                    {
                        g.Draw(GameScreen.sprPixel, new Rectangle((int)(Owner.ListTeleportPoint[T].Position.X - Map.Camera2DPosition.X) * Map.TileSize.X,
                                                       (int)(Owner.ListTeleportPoint[T].Position.Y - Map.Camera2DPosition.Y) * Map.TileSize.Y,
                                                       Map.TileSize.X, Map.TileSize.Y), null,
                                        BrushTeleportPoint, 0f, Vector2.Zero, SpriteEffects.None, 0.001f);

                        g.DrawString(Map.fntArial9, Owner.ListTeleportPoint[T].Tag,
                            new Vector2((Owner.ListTeleportPoint[T].Position.X - Map.Camera2DPosition.X) * Map.TileSize.X + 10,
                                        (Owner.ListTeleportPoint[T].Position.Y - Map.Camera2DPosition.Y) * Map.TileSize.Y + 10),
                            Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                    for (int i = 0; i < Owner.ListUnitSpawn.Count; i++)
                    {
                        g.Draw(GameScreen.sprPixel, new Rectangle((int)(Owner.ListUnitSpawn[i].SpawnPositionX - Map.Camera2DPosition.X) * Map.TileSize.X,
                                                      (int)(Owner.ListUnitSpawn[i].SpawnPositionY - Map.Camera2DPosition.Y) * Map.TileSize.Y,
                                                       Map.TileSize.X, Map.TileSize.Y),
                                                      null,
                                        Color.FromNonPremultiplied(255, 255, 255, 180), 0f, Vector2.Zero, SpriteEffects.None, 0.1f);

                        g.Draw(Owner.ListUnitSpawn[i].UnitToSpawn.SpriteMap, new Rectangle((int)(Owner.ListUnitSpawn[i].SpawnPositionX - Map.Camera2DPosition.X) * Map.TileSize.X,
                                                      (int)(Owner.ListUnitSpawn[i].SpawnPositionY - Map.Camera2DPosition.Y) * Map.TileSize.Y,
                                                       Map.TileSize.X, Map.TileSize.Y),
                                                      null,
                                        Color.FromNonPremultiplied(255, 255, 255, 180), 0f, Vector2.Zero, SpriteEffects.None, 0.1f);

                        int OffsetX = 18;
                        int OffsetY = 6;
                        g.DrawString(Map.fntArial8, Owner.ListUnitSpawn[i].SpawnPlayer.ToString(),
                            new Vector2((Owner.ListUnitSpawn[i].SpawnPositionX - Map.Camera2DPosition.X) * Map.TileSize.X + OffsetX - 2,
                                        (Owner.ListUnitSpawn[i].SpawnPositionY - Map.Camera2DPosition.Y) * Map.TileSize.Y + OffsetY - 2),
                            Color.Black, 0f, Vector2.Zero, 1.8f, SpriteEffects.None, 0.22f);
                        g.DrawString(Map.fntArial8, Owner.ListUnitSpawn[i].SpawnPlayer.ToString(),
                            new Vector2((Owner.ListUnitSpawn[i].SpawnPositionX - Map.Camera2DPosition.X) * Map.TileSize.X + OffsetX + 2,
                                        (Owner.ListUnitSpawn[i].SpawnPositionY - Map.Camera2DPosition.Y) * Map.TileSize.Y + OffsetY - 2),
                            Color.Black, 0f, Vector2.Zero, 1.8f, SpriteEffects.None, 0.22f);

                        g.DrawString(Map.fntArial8, Owner.ListUnitSpawn[i].SpawnPlayer.ToString(),
                            new Vector2((Owner.ListUnitSpawn[i].SpawnPositionX - Map.Camera2DPosition.X) * Map.TileSize.X + OffsetX - 2,
                                        (Owner.ListUnitSpawn[i].SpawnPositionY - Map.Camera2DPosition.Y) * Map.TileSize.Y + OffsetY + 2),
                            Color.Black, 0f, Vector2.Zero, 1.8f, SpriteEffects.None, 0.22f);
                        g.DrawString(Map.fntArial8, Owner.ListUnitSpawn[i].SpawnPlayer.ToString(),
                            new Vector2((Owner.ListUnitSpawn[i].SpawnPositionX - Map.Camera2DPosition.X) * Map.TileSize.X + OffsetX + 2,
                                        (Owner.ListUnitSpawn[i].SpawnPositionY - Map.Camera2DPosition.Y) * Map.TileSize.Y + OffsetY + 2),
                            Color.Black, 0f, Vector2.Zero, 1.8f, SpriteEffects.None, 0.22f);

                        g.DrawString(Map.fntArial8, Owner.ListUnitSpawn[i].SpawnPlayer.ToString(),
                            new Vector2((Owner.ListUnitSpawn[i].SpawnPositionX - Map.Camera2DPosition.X) * Map.TileSize.X + OffsetX,
                                        (Owner.ListUnitSpawn[i].SpawnPositionY - Map.Camera2DPosition.Y) * Map.TileSize.Y + OffsetY - 2),
                            Color.Black, 0f, Vector2.Zero, 1.8f, SpriteEffects.None, 0.22f);
                        g.DrawString(Map.fntArial8, Owner.ListUnitSpawn[i].SpawnPlayer.ToString(),
                            new Vector2((Owner.ListUnitSpawn[i].SpawnPositionX - Map.Camera2DPosition.X) * Map.TileSize.X + OffsetX,
                                        (Owner.ListUnitSpawn[i].SpawnPositionY - Map.Camera2DPosition.Y) * Map.TileSize.Y + OffsetY + 2),
                            Color.Black, 0f, Vector2.Zero, 1.8f, SpriteEffects.None, 0.22f);

                        g.DrawString(Map.fntArial8, Owner.ListUnitSpawn[i].SpawnPlayer.ToString(),
                            new Vector2((Owner.ListUnitSpawn[i].SpawnPositionX - Map.Camera2DPosition.X) * Map.TileSize.X + OffsetX,
                                        (Owner.ListUnitSpawn[i].SpawnPositionY - Map.Camera2DPosition.Y) * Map.TileSize.Y + OffsetY),
                            Color.White, 0f, Vector2.Zero, 1.8f, SpriteEffects.None, 0.2f);
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
