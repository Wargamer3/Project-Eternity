using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelTileStatus : ActionPanelDeathmatch
    {
        public ActionPanelTileStatus(DeathmatchMap Map)
            : base("Tile Status", Map)
        {
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void DoRead(ByteReader BR)
        {
            Map.CursorPosition.X = BR.ReadFloat();
            Map.CursorPosition.Y = BR.ReadFloat();
            Map.CameraPosition.X = BR.ReadFloat();
            Map.CameraPosition.Y = BR.ReadFloat();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendFloat(Map.CursorPosition.X);
            BW.AppendFloat(Map.CursorPosition.Y);
            BW.AppendFloat(Map.CameraPosition.X);
            BW.AppendFloat(Map.CameraPosition.Y);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelTileStatus(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float TileX = Map.CursorPosition.X;
            float TileY = Map.CursorPosition.Y;

            float DrawX = (TileX - Map.CameraPosition.X + 1) * Map.TileSize.X;
            float DrawY = (TileY - Map.CameraPosition.Y) * Map.TileSize.Y;

            if (DrawX + Map.sprCursorTerrainSelection.Width >= Constants.Width)
                DrawX = Constants.Width - Map.sprCursorTerrainSelection.Width;

            if (DrawY + Map.sprCursorTerrainSelection.Height >= Constants.Height)
                DrawY = Constants.Height - Map.sprCursorTerrainSelection.Height;

            Terrain ActiveTerrain = Map.GetTerrain(TileX, TileY, Map.ActiveLayerIndex);
            DrawableTile ActiveTile = Map.GetTile(TileX, TileY, Map.ActiveLayerIndex);
            g.Draw(Map.sprCursorTerrainSelection, new Vector2(DrawX, DrawY), Color.White);
            g.Draw(Map.ListTileSet[ActiveTile.Tileset], new Vector2(DrawX + 6, DrawY + 22), ActiveTile.Origin, Color.White);
            string BonusValue;

            //Draw the bonuses.
            for (int i = 0; i < ActiveTerrain.ListBonus.Length; i++)
            {
                if (ActiveTerrain.ListBonusValue[i] > 0)
                    BonusValue = "+" + ActiveTerrain.ListBonusValue[i];
                else
                    BonusValue = "-" + ActiveTerrain.ListBonusValue[i];
                switch (ActiveTerrain.ListBonus[i])
                {
                    case TerrainBonus.Accuracy://not used
                                               //g.DrawString(fntArial10, ArrayGround[TileX, TileY].ListBonusValue[i] + "%", new Vector2(DrawX + 63, DrawY - 2), Color.White);
                        break;

                    case TerrainBonus.Evasion:
                        g.DrawString(Map.fntArial9, BonusValue, new Vector2(DrawX + 108, DrawY + 36), Color.White);
                        break;

                    case TerrainBonus.Armor:
                        g.DrawStringRightAligned(Map.fntArial9, BonusValue, new Vector2(DrawX + 85, DrawY + 36), Color.White);
                        break;

                    case TerrainBonus.HPRegen:
                        g.DrawStringRightAligned(Map.fntArial9, BonusValue + "%", new Vector2(DrawX + 90, DrawY + 17), Color.White);
                        break;

                    case TerrainBonus.HPRestore:
                        g.DrawStringRightAligned(Map.fntArial9, BonusValue, new Vector2(DrawX + 85, DrawY + 17), Color.White);
                        break;

                    case TerrainBonus.ENRegen:
                        g.DrawString(Map.fntArial9, BonusValue + "%", new Vector2(DrawX + 103, DrawY + 17), Color.White);
                        break;

                    case TerrainBonus.ENRestore:
                        g.DrawString(Map.fntArial9, BonusValue, new Vector2(DrawX + 105, DrawY + 17), Color.White);
                        break;
                }
            }
        }
    }
}
