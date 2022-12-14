using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
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
            if (ActiveInputManager.InputConfirmPressed())
            {
                CancelPanel();
            }
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
            Terrain ActiveTerrain = Map.GetTerrain(Map.CursorPosition);
            DrawableTile ActiveTile = ActiveTerrain.DrawableTile;
            g.Draw(Map.sprCursorTerrainSelection, new Vector2(FinalMenuX, FinalMenuY), Color.White);
            g.Draw(Map.ListTileSet[ActiveTile.TilesetIndex], new Vector2(FinalMenuX + 6, FinalMenuY + 22), ActiveTile.Origin, Color.White);
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
                        g.DrawString(Map.fntArial9, BonusValue, new Vector2(FinalMenuX + 108, FinalMenuY + 36), Color.White);
                        break;

                    case TerrainBonus.Armor:
                        g.DrawStringRightAligned(Map.fntArial9, BonusValue, new Vector2(FinalMenuX + 85, FinalMenuY + 36), Color.White);
                        break;

                    case TerrainBonus.HPRegen:
                        g.DrawStringRightAligned(Map.fntArial9, BonusValue + "%", new Vector2(FinalMenuX + 90, FinalMenuY + 17), Color.White);
                        break;

                    case TerrainBonus.HPRestore:
                        g.DrawStringRightAligned(Map.fntArial9, BonusValue, new Vector2(FinalMenuX + 85, FinalMenuY + 17), Color.White);
                        break;

                    case TerrainBonus.ENRegen:
                        g.DrawString(Map.fntArial9, BonusValue + "%", new Vector2(FinalMenuX + 103, FinalMenuY + 17), Color.White);
                        break;

                    case TerrainBonus.ENRestore:
                        g.DrawString(Map.fntArial9, BonusValue, new Vector2(FinalMenuX + 105, FinalMenuY + 17), Color.White);
                        break;
                }
            }
        }
    }
}
