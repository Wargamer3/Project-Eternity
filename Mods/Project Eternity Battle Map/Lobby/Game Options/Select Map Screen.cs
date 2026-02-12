using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class GameOptionsSelectMapScreen : GameScreen
    {
        private class MapCache
        {
            public readonly string GameMode;
            public static ContentManager Content;

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
                    RootDirectory = ContentRootDirectory + "/" + ActiveModName + "/Maps/";
                }

                ListMapFolder = Directory.EnumerateDirectories(RootDirectory, "Multiplayer", SearchOption.AllDirectories);

                foreach (string ActiveMultiplayerFolder in ListMapFolder)
                {
                    foreach (string ActiveCampaignFolder in Directory.EnumerateDirectories(ActiveMultiplayerFolder, GameMode, SearchOption.AllDirectories))
                    {
                        foreach (string ActiveFile in Directory.EnumerateFiles(ActiveCampaignFolder, "*.pem", SearchOption.AllDirectories))
                        {
                            string MapFolder = ActiveMultiplayerFolder.Substring(ContentRootDirectory.Length + 1);
                            MapFolder = MapFolder.Substring(0, MapFolder.Length - "Maps/Multiplayer/".Length);
                            string FilePath = ActiveFile.Substring(RootDirectory.Length + "Multiplayer/".Length).Replace('\\', '/');
                            FilePath = FilePath.Substring(0, FilePath.Length - 4);
                            string FileName = ActiveFile.Substring(ActiveCampaignFolder.Length + 1);
                            FileName = FileName.Substring(0, FileName.Length - 4);

                            MapInfo NewMapInfo = MapInfo.MapInfoTemplate.Clone(FileName, MapFolder, FilePath);

                            DicMapInfoByPath.Add(FilePath, NewMapInfo);
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
                }

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

        public class MapInfo
        {
            public static MapInfo MapInfoTemplate;

            public bool IsLoaded;
            public readonly string MapName;
            public readonly string MapModName;
            public readonly string MapPath;
            public uint OrderNumber;
            public Point MapSize;
            public List<Color> ListMapTeam;
            public byte MinNumberOfPlayer;
            public byte MaxNumberOfPlayer;
            public byte MaxSquadPerPlayer;
            public string MapDescription;
            public Texture2D MapImage;
            public GameModeInfo GameInfo;
            public List<string> ListMandatoryMutator;

            public MapInfo()
            {
                MapName = string.Empty;
                MapModName = string.Empty;
                MapPath = string.Empty;
                MapImage = null;
                MapSize = Point.Zero;
                ListMapTeam = new List<Color>();
                MinNumberOfPlayer = 0;
                MaxNumberOfPlayer = 0;
                MaxSquadPerPlayer = 0;
                MapDescription = string.Empty;
                ListMandatoryMutator = new List<string>();
                GameInfo = null;
                IsLoaded = false;
            }

            protected MapInfo(string MapName, string MapModName, string MapPath)
            {
                this.MapName = MapName;
                this.MapModName = MapModName;
                this.MapPath = MapPath;
                this.MapImage = null;
                MapSize = Point.Zero;
                ListMapTeam = new List<Color>();
                MinNumberOfPlayer = 0;
                MaxNumberOfPlayer = 0;
                MaxSquadPerPlayer = 0;
                MapDescription = string.Empty;
                ListMandatoryMutator = new List<string>();
                GameInfo = null;
                IsLoaded = false;
            }

            public virtual void ReadInfo(BinaryReader BR, string GameMode)
            {

                OrderNumber = BR.ReadUInt32();

                MapSize.X = BR.ReadInt32();
                MapSize.Y = BR.ReadInt32();

                MinNumberOfPlayer = BR.ReadByte();
                MaxNumberOfPlayer = BR.ReadByte();
                MaxSquadPerPlayer = BR.ReadByte();

                MapDescription = BR.ReadString();
                string sndBattleThemePath = BR.ReadString();

                ListMandatoryMutator.Clear();
                int ListMandatoryMutatorCount = BR.ReadByte();
                for (int M = 0; M < ListMandatoryMutatorCount; M++)
                {
                    ListMandatoryMutator.Add(BR.ReadString());
                }

                int NumberOfTeams = BR.ReadByte();
                ListMapTeam = new List<Color>(NumberOfTeams);
                //Deathmatch colors
                for (int D = 0; D < NumberOfTeams; D++)
                {
                    ListMapTeam.Add(Color.FromNonPremultiplied(BR.ReadByte(), BR.ReadByte(), BR.ReadByte(), 255));
                }

                Dictionary<string, GameModeInfo> DicAvailableGameType = BattleMap.DicBattmeMapType[MapModName].GetAvailableGameModes();

                int ListGameTypeCount = BR.ReadByte();
                for (int G = 0; G < ListGameTypeCount; G++)
                {
                    string GameTypeCategoryName = BR.ReadString();
                    string GameTypeName = BR.ReadString();
                    GameModeInfo LoadedGameInfo = DicAvailableGameType[GameTypeName].Copy();
                    LoadedGameInfo.Load(BR);
                    if (GameMode == GameTypeCategoryName)
                    {
                        GameInfo = LoadedGameInfo;
                        break;
                    }
                }

                if (GameInfo == null && DicAvailableGameType.ContainsKey(GameMode))
                {
                    GameInfo = DicAvailableGameType[GameMode];
                }
            }

            public virtual MapInfo Clone(string MapName, string MapModName, string MapPath)
            {
                return new MapInfo(MapName, MapModName, MapPath);
            }
        }

        private SpriteFont fntText;
        private SpriteFont fntOxanimumRegular;
        private SpriteFont fntOxanimumBold;

        private Texture2D sprHighlight;

        private Texture2D sprFrameTop;
        private Texture2D sprFrameDescription;
        private Texture2D sprScrollbarBackground;
        private Texture2D sprScrollbar;

        private Scrollbar MapScrollbar;

        private readonly RoomInformations Room;
        private readonly GameOptionsScreen OptionsScreen;
        private readonly GamePreparationScreen Owner;

        private MapFolderCache DicCacheByFolderName;

        private Dictionary<string, MapInfo> DicMapInfoByPath;
        private MapInfo ActiveMapInfo;
        private int MapScrollbarValue;

        int PanelY;
        int PanelWidth;

        int LeftPanelX;
        int CategoryOffsetY;
        int HeaderOffsetY;
        int FirstLineOffsetY;
        int LineOffsetY;

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
            fntOxanimumRegular = Content.Load<SpriteFont>("Fonts/Oxanium Regular");
            fntOxanimumBold = Content.Load<SpriteFont>("Fonts/Oxanium Bold");

            sprHighlight = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Room/Select Highlight");

            sprFrameTop = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Room/Frame Top Large");
            sprFrameDescription = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Extra Frame");
            sprScrollbarBackground = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Room/Scrollbar Background");
            sprScrollbar = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Room/Scrollbar Bar");

            float Ratio = Constants.Height / 2160f;
            PanelY = (int)(510 * Ratio);
            PanelWidth = (int)(sprFrameTop.Width * Ratio);

            LeftPanelX = (int)(390 * Ratio);

            CategoryOffsetY = (int)(200 * Ratio);
            HeaderOffsetY = (int)(78 * Ratio);
            FirstLineOffsetY = (int)(30 * Ratio);
            LineOffsetY = (int)(76 * Ratio);

            if (MapCache.Content == null)
            {
                MapCache.Content = new ContentManager(serviceProvider);
                MapCache.Content.RootDirectory = "Content";
            }

            MapScrollbar = new Scrollbar(sprScrollbar, new Vector2(LeftPanelX + PanelWidth - 20, PanelY), Ratio, (int)(sprScrollbarBackground.Height * Ratio), 10, OnGametypeScrollbarChange);
        }

        private void OnGametypeScrollbarChange(float ScrollbarValue)
        {
            MapScrollbarValue = (int)ScrollbarValue;
        }

        public override void Update(GameTime gameTime)
        {
            MapScrollbar.Update(gameTime);

            float DrawY = PanelY + FirstLineOffsetY;
            int CurrentIndex = 0;
            foreach (MapInfo ActiveMap in DicMapInfoByPath.Values)
            {
                if (CurrentIndex >= MapScrollbarValue)
                {
                    if (MouseHelper.MouseStateCurrent.X >= LeftPanelX && MouseHelper.MouseStateCurrent.X < LeftPanelX + PanelWidth
                        && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + LineOffsetY
                        && InputHelper.InputConfirmPressed())
                    {
                        SelectMap(ActiveMap);
                    }

                    DrawY += LineOffsetY;
                }

                ++CurrentIndex;
            }
        }

        private void SelectMap(MapInfo MapInfoToSelect)
        {
            ActiveMapInfo = MapInfoToSelect;
            LoadMapInfo(MapInfoToSelect, Room.GameMode);
            Owner.UpdateSelectedMap(ActiveMapInfo);
            OptionsScreen.OnMapUpdate();
        }

        public void SelectMap(string MapInfoToSelect)
        {
            ActiveMapInfo = DicMapInfoByPath[MapInfoToSelect];
            SelectMap(ActiveMapInfo);
        }

        private static void LoadMapInfo(MapInfo MapInfoToSelect, string GameMode)
        {
            if (MapInfoToSelect.IsLoaded)
                return;

            string MapPath = MapInfoToSelect.MapModName + "/Maps/Multiplayer/" + MapInfoToSelect.MapPath;
            Texture2D MapImage = null;

            if (File.Exists("Content/" + MapPath + ".xnb"))
            {
                MapImage = MapCache.Content.Load<Texture2D>(MapPath);
            }

            MapInfoToSelect.MapImage = MapImage;

            FileStream FS = new FileStream("Content/" + MapPath + ".pem", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            MapInfoToSelect.ReadInfo(BR, GameMode);

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
            float Ratio = Constants.Height / 2160f;
            Color ColorBox = Color.FromNonPremultiplied(204, 204, 204, 255);
            Color ColorText = Color.FromNonPremultiplied(65, 70, 65, 255);

            int RightPanelContentWidth = (int)(sprFrameDescription.Width * Ratio);
            int RightPanelX = (int)(2280 * Ratio);
            int RightPanelContentX = RightPanelX + RightPanelContentWidth / 2;
            float DrawY = PanelY;
            //MapScrollbar.Draw(g);

            g.Draw(sprFrameDescription, new Vector2(RightPanelX, DrawY + 78 * Ratio), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);

            int BoxHeight = (DicMapInfoByPath.Values.Count * LineOffsetY + FirstLineOffsetY);

            g.Draw(sprFrameTop, new Vector2(364 * Ratio, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);
            g.Draw(sprPixel, new Rectangle((int)(364 * Ratio), (int)(DrawY + sprFrameTop.Height * Ratio), (int)(sprFrameTop.Width * Ratio), BoxHeight), null, ColorBox, 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
            g.Draw(sprFrameTop, new Vector2(364 * Ratio, DrawY + sprFrameTop.Height * Ratio + BoxHeight), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.FlipVertically, 0.9f);

            DrawY += FirstLineOffsetY;

            int CurrentIndex = 0;
            foreach (MapInfo ActiveMap in DicMapInfoByPath.Values)
            {
                if (CurrentIndex >= MapScrollbarValue)
                {
                    if (ActiveMapInfo == ActiveMap ||
                        (MouseHelper.MouseStateCurrent.X >= LeftPanelX && MouseHelper.MouseStateCurrent.X < LeftPanelX + PanelWidth
                            && MouseHelper.MouseStateCurrent.Y >= DrawY && MouseHelper.MouseStateCurrent.Y < DrawY + 20
                            && OptionsScreen.IsOnTop))
                    {
                        g.Draw(sprHighlight, new Vector2(400 * Ratio, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.8f);
                    }

                    g.DrawString(fntOxanimumRegular, ActiveMap.MapName, new Vector2(LeftPanelX + 5, DrawY), ColorText);

                    DrawY += LineOffsetY;
                }

                ++CurrentIndex;
            }
            
            if (ActiveMapInfo != null && ActiveMapInfo.MapName != null)
            {
                string MapPlayers;
                if (ActiveMapInfo.MinNumberOfPlayer != ActiveMapInfo.MaxNumberOfPlayer)
                {
                    MapPlayers = ActiveMapInfo.MinNumberOfPlayer + "-" + ActiveMapInfo.MaxNumberOfPlayer + " Players";
                }
                else
                {
                    MapPlayers = ActiveMapInfo.MinNumberOfPlayer + " Players";
                }

                if (ActiveMapInfo.MapImage != null)
                {
                    g.DrawUnstretched(ActiveMapInfo.MapImage, RightPanelContentX, (int)(700 * Ratio), (int)(600 * Ratio), (int)(340 * Ratio), new Vector2(ActiveMapInfo.MapImage.Width / 2, 0));
                }

                DrawY = 560;
                g.DrawStringCentered(fntOxanimumRegular, MapPlayers,
                    new Vector2(RightPanelContentX,
                        DrawY), ColorText);

                DrawY += (int)(70 * Ratio);
                g.DrawStringCentered(fntOxanimumRegular, "Size: " + ActiveMapInfo.MapSize.X + " x " + ActiveMapInfo.MapSize.Y,
                    new Vector2(RightPanelContentX,
                        DrawY ), ColorText);

                DrawY += (int)(70 * Ratio);
                g.DrawStringCentered(fntOxanimumRegular, ActiveMapInfo.MapName, new Vector2(RightPanelContentX,
                    DrawY), ColorText);

                int DescriptionBoxNameWidth = (int)(RightPanelContentWidth * 0.9f);
                DrawY += (int)(70 * Ratio);
                foreach (string ActiveLine in TextHelper.FitToWidth(fntOxanimumRegular, ActiveMapInfo.MapDescription, DescriptionBoxNameWidth))
                {
                    g.DrawStringCentered(fntOxanimumRegular, ActiveLine, new Vector2(RightPanelContentX, DrawY), ColorText);
                    DrawY += (int)(70 * Ratio);
                }
            }
        }

        public override string ToString()
        {
            return "Select Map";
        }
    }
}
