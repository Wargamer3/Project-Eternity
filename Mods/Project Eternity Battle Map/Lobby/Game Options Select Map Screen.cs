using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class GameOptionsSelectMapScreen : GameScreen
    {
        private struct MapInfo
        {
            public readonly string MapName;
            public readonly string MapType;
            public readonly string MapPath;
            public readonly string MapDescription;
            public readonly Texture2D MapImage;

            public MapInfo(string MapName, string MapType, string MapPath, string MapDescription, Texture2D MapImage)
            {
                this.MapName = MapName;
                this.MapType = MapType;
                this.MapPath = MapPath;
                this.MapDescription = MapDescription;
                this.MapImage = MapImage;
            }
        }

        private SpriteFont fntText;
        private BoxScrollbar MapScrollbar;

        private readonly RoomInformations Room;

        private Dictionary<string, MapInfo> DicMapInfoByPath;
        private MapInfo ActiveMapInfo;
        private int MapScrollbarValue;

        int PanelY = (int)(Constants.Height * 0.15);
        int PanelWidth = (int)(Constants.Width * 0.4);
        int PanelHeight = (int)(Constants.Height * 0.75);

        int LeftPanelX = (int)(Constants.Width * 0.03);

        public GameOptionsSelectMapScreen(RoomInformations Room)
        {
            this.Room = Room;

            DicMapInfoByPath = new Dictionary<string, MapInfo>();
        }

        public override void Load()
        {
            fntText = Content.Load<SpriteFont>("Fonts/Arial10");

            MapScrollbar = new BoxScrollbar(new Vector2(LeftPanelX + PanelWidth - 20, PanelY), PanelHeight, 10, OnGametypeScrollbarChange);
        }

        private void OnGametypeScrollbarChange(float ScrollbarValue)
        {
            MapScrollbarValue = (int)ScrollbarValue;
        }

        public override void Update(GameTime gameTime)
        {
            MapScrollbar.Update(gameTime);

            float DrawY = PanelY;
            int CurrentIndex = 0;
            foreach (MapInfo ActiveMap in DicMapInfoByPath.Values)
            {
                if (CurrentIndex >= MapScrollbarValue)
                {
                    if (MouseHelper.MouseStateCurrent.X >= LeftPanelX && MouseHelper.MouseStateCurrent.X < LeftPanelX + PanelWidth
                        && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + 20
                        && InputHelper.InputConfirmPressed())
                    {
                        SelectMap(ActiveMap);
                    }

                    DrawY += 20;
                }

                ++CurrentIndex;
            }
        }

        private void SelectMap(MapInfo MapInfoToSelect)
        {
            ActiveMapInfo = MapInfoToSelect;
            Room.MapName = ActiveMapInfo.MapName;
            Room.MapType = ActiveMapInfo.MapType;
            Room.MapPath = ActiveMapInfo.MapPath;
        }

        public void UpdateMaps()
        {
            ActiveMapInfo = new MapInfo();
            Room.MapType = null;
            Room.MapPath = null;

            DicMapInfoByPath.Clear();

            string RootDirectory = Content.RootDirectory + "/Maps/";

            foreach (string ActiveMultiplayerFolder in Directory.EnumerateDirectories(Content.RootDirectory + "/Maps/", "Multiplayer", SearchOption.AllDirectories))
            {
                foreach (string ActiveCampaignFolder in Directory.EnumerateDirectories(ActiveMultiplayerFolder, Room.RoomType, SearchOption.AllDirectories))
                {
                    foreach (string ActiveFile in Directory.EnumerateFiles(ActiveCampaignFolder, "*.pem", SearchOption.AllDirectories))
                    {
                        string MapType = ActiveMultiplayerFolder.Substring(RootDirectory.Length);
                        MapType = MapType.Substring(0, MapType.Length - "Multiplayer/".Length);
                        string FilePath = ActiveFile.Substring(RootDirectory.Length + MapType.Length + 1);
                        FilePath = FilePath.Substring(0, FilePath.Length - 4);
                        string FileName = ActiveFile.Substring(ActiveCampaignFolder.Length + 1);
                        FileName = FileName.Substring(0, FileName.Length - 4);
                        DicMapInfoByPath.Add(FilePath, new MapInfo(FileName, MapType, FilePath, "", null));
                    }
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawBox(g, new Vector2(LeftPanelX, PanelY), PanelWidth, PanelHeight, Color.White);
            MapScrollbar.Draw(g);

            float DrawY = PanelY + 5;
            int CurrentIndex = 0;
            foreach (MapInfo ActiveMap in DicMapInfoByPath.Values)
            {
                if (CurrentIndex >= MapScrollbarValue)
                {
                    g.DrawString(fntText, ActiveMap.MapName, new Vector2(LeftPanelX + 5, DrawY), Color.White);
                    if (MouseHelper.MouseStateCurrent.X >= LeftPanelX && MouseHelper.MouseStateCurrent.X < LeftPanelX + PanelWidth
                        && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + 20)
                    {
                        g.Draw(sprPixel, new Rectangle(LeftPanelX, (int)DrawY, PanelWidth, 20), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }

                    DrawY += 20;
                }

                ++CurrentIndex;
            }

            int RightPanelX = Constants.Width - LeftPanelX - PanelWidth;
            int RightPanelContentOffset = (int)(PanelWidth * 0.05);
            int RightPanelContentX = RightPanelX + RightPanelContentOffset;
            int RightPanelContentWidth = PanelWidth - RightPanelContentOffset - RightPanelContentOffset;

            int PreviewBoxY = PanelY + 10;
            int PreviewBoxHeight = (int)(PanelHeight * 0.4);

            int DescriptionBoxY = PreviewBoxY + PreviewBoxHeight + 50;
            int DescriptionHeight = PanelHeight - (DescriptionBoxY - PanelY) - 10;

            int DescriptionBoxNameOffset = (int)(RightPanelContentWidth * 0.25);
            int DescriptionBoxNameX = RightPanelContentX + DescriptionBoxNameOffset;
            int DescriptionBoxNameWidth = RightPanelContentWidth - DescriptionBoxNameOffset - DescriptionBoxNameOffset;
            int DescriptionBoxNameHeight = 30;

            DrawBox(g, new Vector2(RightPanelX, PanelY), PanelWidth, PanelHeight, Color.White);

            DrawBox(g, new Vector2(RightPanelContentX, PreviewBoxY), RightPanelContentWidth, PreviewBoxHeight, Color.White);
            DrawBox(g, new Vector2(RightPanelContentX, DescriptionBoxY), RightPanelContentWidth, DescriptionHeight, Color.White);
            DrawBox(g, new Vector2(DescriptionBoxNameX, DescriptionBoxY), DescriptionBoxNameWidth, 30, Color.White);

            g.DrawStringCentered(fntText, "2-8 Players", new Vector2(DescriptionBoxNameX + DescriptionBoxNameWidth / 2, PreviewBoxY + PreviewBoxHeight + 10 + DescriptionBoxNameHeight / 2), Color.White);
            if (ActiveMapInfo.MapName != null)
            {
                g.DrawStringCentered(fntText, ActiveMapInfo.MapName, new Vector2(DescriptionBoxNameX + DescriptionBoxNameWidth / 2, DescriptionBoxY + DescriptionBoxNameHeight / 2), Color.White);

                float DescriptionY = DescriptionBoxY + DescriptionBoxNameHeight;
                foreach (string ActiveLine in TextHelper.FitToWidth(fntText, ActiveMapInfo.MapDescription, RightPanelContentWidth - 5))
                {
                    g.DrawString(fntText, ActiveLine, new Vector2(RightPanelContentX + 5, DescriptionY), Color.White);
                    DescriptionY += 20;

                }
            }
        }

        public override string ToString()
        {
            return "Select Map";
        }
    }
}
