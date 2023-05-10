using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.UI;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class GameOptionsSelectMapScreen : GameScreen
    {
        private class MapCache
        {
            public readonly string GameMode;

            public Dictionary<string, MapInfo> DicMapInfoByPath;

            public MapCache(string GameMode)
            {
                this.GameMode = GameMode;

                DicMapInfoByPath = new Dictionary<string, MapInfo>();
            }

            public void PopulateMaps(string ContentRootDirectory, string ActiveModName)
            {
                DicMapInfoByPath.Clear();

                string RootDirectory = ContentRootDirectory + "/Maps/";

                IEnumerable<string> ListMapFolder;
                if (!string.IsNullOrEmpty(ActiveModName))
                {
                    ListMapFolder = Directory.EnumerateDirectories(ContentRootDirectory + "/Maps/" + ActiveModName + "/", "Multiplayer", SearchOption.AllDirectories);
                }
                else
                {
                    ListMapFolder = Directory.EnumerateDirectories(ContentRootDirectory + "/Maps/", "Multiplayer", SearchOption.AllDirectories);
                }

                foreach (string ActiveMultiplayerFolder in ListMapFolder)
                {
                    foreach (string ActiveCampaignFolder in Directory.EnumerateDirectories(ActiveMultiplayerFolder, GameMode, SearchOption.AllDirectories))
                    {
                        foreach (string ActiveFile in Directory.EnumerateFiles(ActiveCampaignFolder, "*.pem", SearchOption.AllDirectories))
                        {
                            string MapFolder = ActiveMultiplayerFolder.Substring(RootDirectory.Length);
                            MapFolder = MapFolder.Substring(0, MapFolder.Length - "Multiplayer/".Length);
                            string FilePath = ActiveFile.Substring(RootDirectory.Length + MapFolder.Length + 1).Replace('\\', '/');
                            FilePath = FilePath.Substring(0, FilePath.Length - 4);
                            string FileName = ActiveFile.Substring(ActiveCampaignFolder.Length + 1);
                            FileName = FileName.Substring(0, FileName.Length - 4);

                            DicMapInfoByPath.Add(FilePath, new MapInfo(FileName, MapFolder, FilePath));
                        }
                    }
                }
            }
        }

        private class MapFolderCache
        {
            private Dictionary<string, MapCache> DicMapCacheByFolder;

            public MapFolderCache()
            {
                DicMapCacheByFolder = new Dictionary<string, MapCache>();
            }

            public Dictionary<string, MapInfo> GetCampaignMaps(string ContentRootDirectory, string ActiveModName, string GameMode, List<MissionInfo> ListUnlockedMission)
            {
                MapCache FoundMapCache;

                if (!DicMapCacheByFolder.TryGetValue(GameMode, out FoundMapCache))
                {
                    FoundMapCache = new MapCache(GameMode);
                    FoundMapCache.PopulateMaps(ContentRootDirectory, ActiveModName);
                    DicMapCacheByFolder.Add(GameMode, FoundMapCache);

                    List<string> ListLockedMission = new List<string>();

                    foreach (string ActiveMission in FoundMapCache.DicMapInfoByPath.Keys)
                    {
                        bool MissionFound = false;
                        foreach (MissionInfo UnlockedMission in ListUnlockedMission)
                        {
                            if (ActiveMission == UnlockedMission.MapPath)
                            {
                                MissionFound = true;
                            }
                        }
                        if (!MissionFound)
                        {
                            ListLockedMission.Add(ActiveMission);
                        }
                    }

                    foreach (string LockedMission in ListLockedMission)
                    {
                        FoundMapCache.DicMapInfoByPath.Remove(LockedMission);
                    }
                }

                return FoundMapCache.DicMapInfoByPath;
            }

            public Dictionary<string, MapInfo> GetMaps(string ContentRootDirectory, string ActiveModName, string GameMode)
            {
                MapCache FoundMapCache;

                if (!DicMapCacheByFolder.TryGetValue(GameMode, out FoundMapCache))
                {
                    FoundMapCache = new MapCache(GameMode);
                    FoundMapCache.PopulateMaps(ContentRootDirectory, ActiveModName);
                    DicMapCacheByFolder.Add(GameMode, FoundMapCache);
                }

                return FoundMapCache.DicMapInfoByPath;
            }
        }

        private class MapInfo
        {
            public bool IsLoaded;
            public readonly string MapName;
            public readonly string MapModName;
            public readonly string MapPath;
            public Point MapSize;
            public List<Color> ListMapTeam;
            public byte MinNumberOfPlayer;
            public byte MaxNumberOfPlayer;
            public byte MaxSquadPerPlayer;
            public string MapPlayers;
            public string MapDescription;
            public Texture2D MapImage;
            public GameModeInfo GameInfo;
            public List<string> ListMandatoryMutator;

            public MapInfo(string MapName, string MapModName, string MapPath)
            {
                this.MapName = MapName;
                this.MapModName = MapModName;
                this.MapPath = MapPath;
                MapSize = Point.Zero;
                ListMapTeam = new List<Color>();
                MinNumberOfPlayer = 0;
                MaxNumberOfPlayer = 0;
                MaxSquadPerPlayer = 0;
                MapDescription = string.Empty;
                ListMandatoryMutator = new List<string>();
                GameInfo = null;
                MapImage = null;
                IsLoaded = false;

                if (MinNumberOfPlayer != MaxNumberOfPlayer)
                {
                    MapPlayers = MinNumberOfPlayer + "-" + MaxNumberOfPlayer + " Players";
                }
                else
                {
                    MapPlayers = MinNumberOfPlayer + " Players";
                }
            }
        }

        private SpriteFont fntText;
        private BoxScrollbar MapScrollbar;

        private readonly RoomInformations Room;
        private readonly GameOptionsScreen OptionsScreen;
        private readonly GamePreparationScreen Owner;

        private MapFolderCache DicCacheByFolderName;

        private Dictionary<string, MapInfo> DicMapInfoByPath;
        private MapInfo ActiveMapInfo;
        private int MapScrollbarValue;

        int PanelY = (int)(Constants.Height * 0.15);
        int PanelWidth = (int)(Constants.Width * 0.4);
        int PanelHeight = (int)(Constants.Height * 0.75);

        int LeftPanelX = (int)(Constants.Width * 0.03);

        public GameOptionsSelectMapScreen(RoomInformations Room, GameOptionsScreen OptionsScreen, GamePreparationScreen Owner)
        {
            this.Room = Room;
            this.OptionsScreen = OptionsScreen;
            this.Owner = Owner;

            DicCacheByFolderName = new MapFolderCache();

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

            float DrawY = PanelY + 5;
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
            LoadMapInfo(MapInfoToSelect, Room.GameMode);
            Owner.UpdateSelectedMap(ActiveMapInfo.MapName, ActiveMapInfo.MapModName, ActiveMapInfo.MapPath, Room.GameMode,
                ActiveMapInfo.MinNumberOfPlayer, ActiveMapInfo.MaxNumberOfPlayer, ActiveMapInfo.MaxSquadPerPlayer,
                ActiveMapInfo.GameInfo, ActiveMapInfo.ListMandatoryMutator, ActiveMapInfo.ListMapTeam);
            OptionsScreen.OnMapUpdate();
        }

        private static void LoadMapInfo(MapInfo MapInfoToSelect, string GameMode)
        {
            if (MapInfoToSelect.IsLoaded)
                return;

            FileStream FS = new FileStream("Content/Maps/" + MapInfoToSelect.MapModName + "/" + MapInfoToSelect.MapPath + ".pem", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            MapInfoToSelect.MapSize.X = BR.ReadInt32();
            MapInfoToSelect.MapSize.Y = BR.ReadInt32();

            MapInfoToSelect.MinNumberOfPlayer = BR.ReadByte();
            MapInfoToSelect.MaxNumberOfPlayer = BR.ReadByte();
            MapInfoToSelect.MaxSquadPerPlayer = BR.ReadByte();

            MapInfoToSelect.MapDescription = BR.ReadString();

            MapInfoToSelect.ListMandatoryMutator.Clear();
            int ListMandatoryMutatorCount = BR.ReadByte();
            for (int M = 0; M < ListMandatoryMutatorCount; M++)
            {
                MapInfoToSelect.ListMandatoryMutator.Add(BR.ReadString());
            }

            int NumberOfTeams = BR.ReadByte();
            MapInfoToSelect.ListMapTeam = new List<Color>(NumberOfTeams);
            //Deathmatch colors
            for (int D = 0; D < NumberOfTeams; D++)
            {
                MapInfoToSelect.ListMapTeam.Add(Color.FromNonPremultiplied(BR.ReadByte(), BR.ReadByte(), BR.ReadByte(), 255));
            }

            Dictionary<string, GameModeInfo> DicAvailableGameType = BattleMap.DicBattmeMapType[MapInfoToSelect.MapModName].GetAvailableGameModes();

            int ListGameTypeCount = BR.ReadByte();
            for (int G = 0; G < ListGameTypeCount; G++)
            {
                string GameTypeName = BR.ReadString();
                GameModeInfo LoadedGameInfo = DicAvailableGameType[GameTypeName].Copy();
                LoadedGameInfo.Load(BR);
                if (GameMode == GameTypeName)
                {
                    MapInfoToSelect.GameInfo = LoadedGameInfo;
                    break;
                }
            }

            if (MapInfoToSelect.GameInfo == null && DicAvailableGameType.ContainsKey(GameMode))
            {
                MapInfoToSelect.GameInfo = DicAvailableGameType[GameMode];
            }

            FS.Close();
            BR.Close();

            MapInfoToSelect.IsLoaded = true;
        }

        public void UpdateMaps()
        {
            if (Room.GameMode == "Campaign" || Room.GameMode == "Arcade")
            {
                DicMapInfoByPath = DicCacheByFolderName.GetCampaignMaps(Content.RootDirectory, Room.MapModName, Room.GameMode, PlayerManager.ListLocalPlayer[0].GetUnlockedMissions());
            }
            else
            {
                DicMapInfoByPath = DicCacheByFolderName.GetMaps(Content.RootDirectory, Room.MapModName, Room.GameMode);
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

            if (ActiveMapInfo != null && ActiveMapInfo.MapName != null)
            {
                g.DrawStringCentered(fntText, ActiveMapInfo.MapPlayers,
                    new Vector2(DescriptionBoxNameX + DescriptionBoxNameWidth / 2,
                        PreviewBoxY + PreviewBoxHeight + DescriptionBoxNameHeight / 2), Color.White);
                g.DrawStringCentered(fntText, "Size: " + ActiveMapInfo.MapSize.X + " x " + ActiveMapInfo.MapSize.Y,
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
