using System;
using System.IO;
using System.Text;
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
            public readonly Point NewMapSize;
            public readonly byte PlayersMin;
            public readonly byte PlayersMax;
            public readonly string MapPlayers;
            public readonly string MapDescription;
            public readonly Texture2D MapImage;

            public MapInfo(string MapName, string MapType, string MapPath, Point NewMapSize, byte PlayersMin, byte PlayersMax, string MapDescription, Texture2D MapImage)
            {
                this.MapName = MapName;
                this.MapType = MapType;
                this.MapPath = MapPath;
                this.NewMapSize = NewMapSize;
                this.PlayersMin = PlayersMin;
                this.PlayersMax = PlayersMax;
                this.MapDescription = MapDescription;
                this.MapImage = MapImage;
                if (PlayersMin != PlayersMax)
                {
                    MapPlayers = PlayersMin + "-" + PlayersMax + " Players";
                }
                else
                {
                    MapPlayers = PlayersMin + " Players";
                }
            }
        }

        private SpriteFont fntText;
        private BoxScrollbar MapScrollbar;

        private readonly RoomInformations Room;
        private readonly GamePreparationScreen Owner;

        private Dictionary<string, MapInfo> DicMapInfoByPath;
        private MapInfo ActiveMapInfo;
        private int MapScrollbarValue;

        int PanelY = (int)(Constants.Height * 0.15);
        int PanelWidth = (int)(Constants.Width * 0.4);
        int PanelHeight = (int)(Constants.Height * 0.75);

        int LeftPanelX = (int)(Constants.Width * 0.03);

        public GameOptionsSelectMapScreen(RoomInformations Room, GamePreparationScreen Owner)
        {
            this.Room = Room;
            this.Owner = Owner;

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
            Owner.UpdateSelectedMap(ActiveMapInfo.MapName, ActiveMapInfo.MapType, ActiveMapInfo.MapPath, ActiveMapInfo.PlayersMin, ActiveMapInfo.PlayersMax);
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

                        FileStream FS = new FileStream(ActiveFile, FileMode.Open, FileAccess.Read);
                        BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
                        Point NewMapSize = Point.Zero;
                        BR.BaseStream.Seek(0, SeekOrigin.Begin);
                        NewMapSize.X = BR.ReadInt32();
                        NewMapSize.Y = BR.ReadInt32();
                        int TileSizeX = BR.ReadInt32();
                        int TileSizeY = BR.ReadInt32();

                        int CameraPositionX = BR.ReadInt32();
                        int CameraPositionY = BR.ReadInt32();

                        byte PlayersMin = BR.ReadByte();
                        byte PlayersMax = BR.ReadByte();

                        string Description = BR.ReadString();

                        BR.Close();
                        FS.Close();

                        DicMapInfoByPath.Add(FilePath, new MapInfo(FileName, MapType, FilePath, NewMapSize, PlayersMin, PlayersMax, Description, null));
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

            if (ActiveMapInfo.MapName != null)
            {
                g.DrawStringCentered(fntText, ActiveMapInfo.MapPlayers,
                    new Vector2(DescriptionBoxNameX + DescriptionBoxNameWidth / 2,
                        PreviewBoxY + PreviewBoxHeight + DescriptionBoxNameHeight / 2), Color.White);
                g.DrawStringCentered(fntText, "Size: " + ActiveMapInfo.NewMapSize.X + " x " + ActiveMapInfo.NewMapSize.Y,
                    new Vector2(DescriptionBoxNameX + DescriptionBoxNameWidth / 2,
                        PreviewBoxY + PreviewBoxHeight + 20 + DescriptionBoxNameHeight / 2), Color.White);
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
