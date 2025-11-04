using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public partial class SorcererStreetMap : BattleMap, IProjectile3DSandbox
    {
        public static readonly string MapType = "Sorcerer Street";

        public enum Checkpoints { North, South, West, East }
        public override MovementAlgorithmTile CursorTerrain { get { return LayerManager.ListLayer[(int)CursorPosition.Z].ArrayTerrain[(int)CursorPosition.X / TileSize.X, (int)CursorPosition.Y / TileSize.Y]; } }

        #region Ressources

        public SpriteFont fntMenuText;
        public SpriteFont fntDefaultText;

        public Texture2D sprActiveCreatureCursor;
        public Texture2D sprPlayerBackground;
        public Texture2D sprPlayerBlue1;
        public Texture2D sprPlayerBlue2;
        public Texture2D sprPlayerRed1;
        public Texture2D sprPlayerRed2;

        public Texture2D sprPortraitStart;
        public Texture2D sprPortraitMiddle;
        public Texture2D sprPortraitEnd;

        public Texture2D sprTerritory;
        public Texture2D sprMap;
        public Texture2D sprInfo;
        public Texture2D sprOptions;
        public Texture2D sprHelp;
        public Texture2D sprEndTurn;
        public Texture2D sprReturn;
        public Texture2D sprSuspend;
        public Texture2D sprCheat;

        public Texture2D sprVS;

        public Texture2D sprDirectionNorth;
        public Texture2D sprDirectionEast;
        public Texture2D sprDirectionWest;
        public Texture2D sprDirectionSouth;
        public Texture2D sprDirectionNorthFilled;
        public Texture2D sprDirectionEastFilled;
        public Texture2D sprDirectionWestFilled;
        public Texture2D sprDirectionSouthFilled;

        public Texture2D sprTowerPopupBackground;
        public Texture2D sprTowerPopupRing;
        public Texture2D sprTowerPopupEast;
        public Texture2D sprTowerPopupWest;
        public Texture2D sprTowerPopupSouth;
        public Texture2D sprTowerPopupNorth;

        public Texture2D sprTileset;

        public CardSymbols Symbols;

        public Texture2D sprTileBorderEmpty;
        public Texture2D sprTileBorderRed;
        public Texture2D sprTileBorderBlue;

        #endregion

        public static Color TextColor = Color.FromNonPremultiplied(210, 210, 210, 255);

        public List<DelayedAttack> ListDelayedAttack;
        public List<PERAttack> ListPERAttack;

        public readonly Vector3 LastPosition;
        private List<Player> ListLocalPlayerInfo;
        public List<Player> ListPlayer;
        public Dictionary<int, Team> DicTeam;
        public List<Player> ListLocalPlayer { get { return ListLocalPlayerInfo; } }
        public List<Player> ListAllPlayer { get { return ListPlayer; } }
        private TextInput ChatInput;
        public int MagicAtStart;
        public int MagicGainPerLap;
        public int TowerMagicGain;
        public int MagicGoal;
        public int LowestDieRoll;
        public int HighestDieRoll;
        public List<Checkpoints> ListCheckpoint;
        public List<TerrainSorcererStreet> ListSummonedCreature;
        public Dictionary<CreatureCard.ElementalAffinity, byte> DicCreatureCountByElementType;
        public int TotalCreaturesDestroyed;
        public readonly MovementAlgorithm Pathfinder;
        public readonly SorcererStreetPlayerContext GlobalPlayerContext;
        public readonly SorcererStreetBattleContext GlobalSorcererStreetBattleContext;
        public readonly SorcererStreetBattleParams SorcererStreetParams;
        public LayerHolderSorcererStreet LayerManager;
        public List<Area> ListArea;
        public List<TerrainSorcererStreet> ListPassedTerrain;
        public Dictionary<Vector3, TerrainSorcererStreet> DicTemporaryTerrain;//Temporary obstacles
        public SorcererStreetTerrainHolder TerrainHolder;

        public SorcererStreetMap()
            : this(new SorcererStreetBattleParams(new SorcererStreetBattleContext()))
        {
            SorcererStreetBattleParams.DicParams.TryAdd(string.Empty, SorcererStreetParams);
        }

        public SorcererStreetMap(SorcererStreetBattleParams Params)
            : base()
        {
            this.Params = SorcererStreetParams = Params;

            GlobalSorcererStreetBattleContext = SorcererStreetParams.GlobalContext;
            GlobalPlayerContext = SorcererStreetParams.GlobalPlayerContext;

            LowestDieRoll = 1;
            HighestDieRoll = 6;

            DicTemporaryTerrain = new Dictionary<Vector3, TerrainSorcererStreet>();

            RequireDrawFocus = false;
            ListActionMenuChoice = new ActionPanelHolderSorcererStreet(this);
            Pathfinder = new MovementAlgorithmSorcererStreet(this);
            DicTeam = new Dictionary<int, Team>();
            ListPlayer = new List<Player>();
            ListLocalPlayerInfo = new List<Player>();
            ListDelayedAttack = new List<DelayedAttack>();
            ListPERAttack = new List<PERAttack>();
            ListArea = new List<Area>();
            ListPassedTerrain = new List<TerrainSorcererStreet>();

            ListTilesetPreset = new List<TilesetPreset>();
            LayerManager = new LayerHolderSorcererStreet(this);
            MapEnvironment = new EnvironmentManagerSorcererStreet(this);
            Params.ActiveParser = new SorcererStreetFormulaParser(Params);
            ListCheckpoint = new List<Checkpoints>();
            ListSummonedCreature = new List<TerrainSorcererStreet>();
            DicCreatureCountByElementType = new Dictionary<CreatureCard.ElementalAffinity, byte>();
            DicCreatureCountByElementType.Add(CreatureCard.ElementalAffinity.Air, 0);
            DicCreatureCountByElementType.Add(CreatureCard.ElementalAffinity.Earth, 0);
            DicCreatureCountByElementType.Add(CreatureCard.ElementalAffinity.Fire, 0);
            DicCreatureCountByElementType.Add(CreatureCard.ElementalAffinity.Water, 0);
            DicCreatureCountByElementType.Add(CreatureCard.ElementalAffinity.Neutral, 0);

            CursorPosition = new Vector3(0, 0, 0);
            CursorPositionVisible = CursorPosition;

            TerrainHolder = new SorcererStreetTerrainHolder();

            ListTileSet = new List<Texture2D>();
            this.Camera2DPosition = Vector3.Zero;
        }

        public SorcererStreetMap(GameModeInfo GameInfo, SorcererStreetBattleParams Params)
            : this(Params)
        {
            CursorPosition = new Vector3(9, 13, 0);
            CursorPositionVisible = CursorPosition;

            ListTileSet = new List<Texture2D>();
            ListTilesetPreset = new List<TilesetPreset>();
            Camera2DPosition = Vector3.Zero;

            if (GameInfo == null)
            {
                GameRule = new DeathmatchGameRule(this, new DeathmatchGameInfo(true, null));
            }
            else
            {
                GameRule = GameInfo.GetRule(this);
                if (GameRule == null)
                {
                    GameRule = new DeathmatchGameRule(this, new DeathmatchGameInfo(true, null));
                }
            }
        }

        public SorcererStreetMap(string BattleMapPath, GameModeInfo GameInfo, SorcererStreetBattleParams Params)
            : this(GameInfo, Params)
        {
            this.BattleMapPath = BattleMapPath;
        }
        
        public override void Save(string FilePath)
        {
            //Create the Part file.
            FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            SaveProperties(BW);

            BW.Write(MagicAtStart);
            BW.Write(MagicGainPerLap);
            BW.Write(TowerMagicGain);
            BW.Write(MagicGoal);
            BW.Write((byte)LowestDieRoll);
            BW.Write((byte)HighestDieRoll);

            MapScript.SaveMapScripts(BW, ListMapScript);

            SaveTilesets(BW);

            LayerManager.Save(BW);

            MapEnvironment.Save(BW);

            SaveAreas(BW);

            FS.Close();
            BW.Close();
        }

        private void SaveAreas(BinaryWriter BW)
        {
            BW.Write((byte)ListArea.Count);
            foreach (Area ActiveArea in ListArea)
            {
                BW.Write(ActiveArea.Name);
                BW.Write((byte)ActiveArea.ListTerrainInArea.Count);
                foreach (TerrainSorcererStreet ActiveTile in ActiveArea.ListTerrainInArea)
                {
                    BW.Write(ActiveTile.GridPosition.X);
                    BW.Write(ActiveTile.GridPosition.Y);
                    BW.Write(ActiveTile.LayerIndex);
                }
            }
        }

        public override void Load()
        {
            base.Load();

            if (!IsServer)
            {
                ChatInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(15, Constants.Height - 26), new Vector2(470, 20), SendMessage);

                fntMenuText = Content.Load<SpriteFont>("Fonts/Arial30");
                fntDefaultText = Content.Load<SpriteFont>("Fonts/Oxanium Bold Bigger");

                sprActiveCreatureCursor = Content.Load<Texture2D>("Sorcerer Street/Ressources/Active Creature Cursor");

                sprTerritory = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cards/Territory");
                sprMap = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cards/Map");
                sprInfo = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cards/Info");
                sprOptions = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cards/Options");
                sprHelp = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cards/Help");
                sprEndTurn = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cards/End");
                sprReturn = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cards/Return");
                sprSuspend = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cards/Suspend");
                sprCheat = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cards/Cheat");

                sprPortraitStart = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Portrait Start");
                sprPortraitMiddle = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Portrait Middle");
                sprPortraitEnd = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Portrait End");

                sprPlayerBackground = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Player Background");
                sprPlayerBlue1 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Player Blue 1");
                sprPlayerBlue2 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Player Blue 2");
                sprPlayerRed1 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Player Red 1");
                sprPlayerRed2 = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Player Red 2");

                sprVS = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/VS");

                sprDirectionNorth = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/South Black");
                sprDirectionWest = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/West Black");
                sprDirectionEast = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/East Black");
                sprDirectionSouth = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/South Black");
                sprDirectionNorthFilled = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/South White");
                sprDirectionWestFilled = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/West White");
                sprDirectionEastFilled = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/East White");
                sprDirectionSouthFilled = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/South White");

                sprTowerPopupBackground = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/Inner Ring");
                sprTowerPopupRing = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/Quarter Ring");
                sprTowerPopupEast = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/East Tower");
                sprTowerPopupWest = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/West Tower");
                sprTowerPopupSouth = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/South Tower");
                sprTowerPopupNorth = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Checkpoints/North Tower");

                sprTileset = Content.Load<Texture2D>("Sorcerer Street/Ressources/Land/Tiles");

                Symbols = CardSymbols.Symbols;

                sprTileBorderEmpty = Content.Load<Texture2D>("Sorcerer Street/Ressources/Tile Border Empty");
                sprTileBorderRed = Content.Load<Texture2D>("Sorcerer Street/Ressources/Tile Border Red Tile");
                sprTileBorderBlue = Content.Load<Texture2D>("Sorcerer Street/Ressources/Tile Border Blue Tile");

                if (Card.TextParser == null)
                {
                    Card.TextParser = new DynamicText();
                    Card.TextParser.TextMaxWidthInPixel = Card.BoxWidth - 100;
                    Card.TextParser.LineHeight = fntMenuText.LineSpacing;
                    Card.TextParser.TextColor = TextColor;
                    Card.TextParser.ListProcessor.Add(new RegularTextProcessor(Card.TextParser, fntMenuText));
                    IconProcessor IconParser = new IconProcessor(Card.TextParser);
                    Card.TextParser.ListProcessor.Add(IconParser);
                    Card.TextParser.ListProcessor.Add(new PlayerNameProcessor(Card.TextParser, fntMenuText, this));
                    Card.TextParser.ListProcessor.Add(new DefaultTextProcessor(Card.TextParser, fntMenuText));
                    Card.TextParser.SetDefaultProcessor(new DefaultTextProcessor(Card.TextParser, fntMenuText));
                    Card.TextParser.Load(Content);

                    IconParser.PreloadImage("rarityE", CardSymbols.Symbols.sprRarityE);
                    IconParser.PreloadImage("rarityN", CardSymbols.Symbols.sprRarityN);
                    IconParser.PreloadImage("rarityR", CardSymbols.Symbols.sprRarityR);
                    IconParser.PreloadImage("rarityS", CardSymbols.Symbols.sprRarityS);

                    IconParser.PreloadImage("MenuG", CardSymbols.Symbols.sprMenuG);
                    IconParser.PreloadImage("MenuTG", CardSymbols.Symbols.sprMenuTG);
                    IconParser.PreloadImage("MenuST", CardSymbols.Symbols.sprMenuST);
                    IconParser.PreloadImage("MenuHP", CardSymbols.Symbols.sprMenuHP);
                    IconParser.PreloadImage("MenuMHP", CardSymbols.Symbols.sprMenuMHP);

                    IconParser.PreloadImage("water", CardSymbols.Symbols.sprElementWater);
                    IconParser.PreloadImage("fire", CardSymbols.Symbols.sprElementFire);
                    IconParser.PreloadImage("earth", CardSymbols.Symbols.sprElementEarth);
                    IconParser.PreloadImage("air", CardSymbols.Symbols.sprElementAir);
                    IconParser.PreloadImage("neutral", CardSymbols.Symbols.sprElementNeutral);
                    IconParser.PreloadImage("multi", CardSymbols.Symbols.sprElementMulti);

                    IconParser.PreloadImage("weapon", CardSymbols.Symbols.sprItemsWeapon);
                    IconParser.PreloadImage("armor", CardSymbols.Symbols.sprItemsArmor);
                    IconParser.PreloadImage("tool", CardSymbols.Symbols.sprItemsTool);
                    IconParser.PreloadImage("scroll", CardSymbols.Symbols.sprItemsScroll);

                    IconParser.PreloadImage("spellsingle", CardSymbols.Symbols.sprSpellsSingle);
                    IconParser.PreloadImage("spellmultiple", CardSymbols.Symbols.sprSpellsMultiple);

                    IconParser.PreloadImage("enchantsingle", CardSymbols.Symbols.sprEnchantSingle);
                    IconParser.PreloadImage("enchantmultiple", CardSymbols.Symbols.sprEnchantMultiple);

                    IconParser.PreloadImage("creature", CardSymbols.Symbols.sprCreature);
                }
            }

            TerrainHolder.LoadData();

            LoadMap();
            LoadMapAssets();

            Dictionary<string, CutsceneScript> ConquestScripts = CutsceneScriptHolder.LoadAllScripts(typeof(SorcererStreetMapCutsceneScriptHolder), this);
            foreach (CutsceneScript ActiveListScript in ConquestScripts.Values)
            {
                DicCutsceneScript.Add(ActiveListScript.Name, ActiveListScript);
            }
        }

        public override void Load(byte[] ArrayGameData)
        {
            ByteReader BR = new ByteReader(ArrayGameData);

            int ListCharacterCount = BR.ReadByte();
            for (int P = 0; P < ListCharacterCount; ++P)
            {
                string PlayerID = BR.ReadString();
                string PlayerName = BR.ReadString();
                int PlayerTeam = BR.ReadInt32();
                bool IsPlayerControlled = BR.ReadBoolean();
                Color PlayerColor = Color.FromNonPremultiplied(BR.ReadByte(), BR.ReadByte(), BR.ReadByte(), 255);
                int PlayerMagic = BR.ReadInt32();

                Player NewPlayer = null;
                bool IsExistingPlayer = false;
                foreach (Player ActivePlayer in PlayerManager.ListLocalPlayer)
                {
                    if (PlayerManager.OnlinePlayerID == PlayerID)
                    {
                        NewPlayer = new Player(ActivePlayer, SorcererStreetParams);
                        NewPlayer.TeamIndex = PlayerTeam;
                        NewPlayer.Color = PlayerColor;
                        AddLocalCharacter(NewPlayer);
                        //NewPlayer.InputManagerHelper = new PlayerRobotInputManager();
                        //NewPlayer.UpdateControls(GameplayTypes.MouseAndKeyboard);
                        IsExistingPlayer = true;
                        break;
                    }
                }

                if (!IsExistingPlayer)
                {
                    foreach (Player ActivePlayer in Room.ListRoomPlayer)
                    {
                        if (ActivePlayer.ConnectionID == PlayerID)
                        {
                            NewPlayer = new Player(ActivePlayer, SorcererStreetParams);
                            ListAllPlayer.Add(NewPlayer);
                            break;
                        }
                    }
                }

                NewPlayer.TeamIndex = PlayerTeam;
                NewPlayer.Color = PlayerColor;
                NewPlayer.Gold = PlayerMagic;
                NewPlayer.ListRemainingCardInDeck.Clear();
                NewPlayer.ListCardInHand.Clear();

                if (!DicTeam.ContainsKey(PlayerTeam))
                {
                    DicTeam.Add(PlayerTeam, new Team(PlayerTeam));
                }

                DicTeam[PlayerTeam].ListPlayer.Add(NewPlayer);

                byte ListRemainingCardInDeckCount = BR.ReadByte();
                for (int C = 0; C < ListRemainingCardInDeckCount; ++C)
                {
                    string CardType = BR.ReadString();
                    string CardPath = BR.ReadString();

                    foreach (Card ActiveCard in NewPlayer.ListCardInDeck)
                    {
                        if (ActiveCard.CardType == CardType && ActiveCard.Path == CardPath)
                        {
                            NewPlayer.ListRemainingCardInDeck.Add(ActiveCard);
                            break;
                        }
                    }
                }

                byte CardInHand = BR.ReadByte();
                for (int C = 0; C < CardInHand; ++C)
                {
                    string CardType = BR.ReadString();
                    string CardPath = BR.ReadString();

                    foreach (Card ActiveCard in NewPlayer.ListCardInDeck)
                    {
                        if (ActiveCard.CardType == CardType && ActiveCard.Path == CardPath)
                        {
                            NewPlayer.ListCardInHand.Add(ActiveCard);
                            break;
                        }
                    }
                }

                NewPlayer.IsPlayerControlled = IsPlayerControlled;
            }

            UpdatePlayersRank();

            BattleMapPath = BR.ReadString();

            Load();

            OnlineClient.Host.Send(new ClientIsReadyScriptClient());

            BR.Clear();
        }

        public void LoadMap(bool BackgroundOnly = false)
        {
            //Clear everything.
            ListTileSet = new List<Texture2D>();
            FileStream FS = new FileStream("Content/Sorcerer Street/Maps/" + BattleMapPath + ".pem", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            //Map parameters.
            MapName = Path.GetFileNameWithoutExtension(BattleMapPath);

            LoadProperties(BR);

            MagicAtStart = BR.ReadInt32();
            MagicGainPerLap = BR.ReadInt32();
            TowerMagicGain = BR.ReadInt32();
            MagicGoal = BR.ReadInt32();
            LowestDieRoll = BR.ReadByte();
            HighestDieRoll = BR.ReadByte();

            ListMapScript = MapScript.LoadMapScripts(BR, DicMapEvent, DicMapCondition, DicMapTrigger, out ListMapEvent);

            LoadTilesets(BR);

            LayerManager = new LayerHolderSorcererStreet(this, BR);

            MapEnvironment = new EnvironmentManagerSorcererStreet(BR, this);

            LoadAreas(BR);

            BR.Close();
            FS.Close();

            TogglePreview(BackgroundOnly);
        }

        private void LoadAreas(BinaryReader BR)
        {
            byte ListAreaCount = BR.ReadByte();
            ListArea = new List<Area>(ListAreaCount);
            for (int A = 0; A < ListAreaCount; A++)
            {
                string ActiveAreaName = BR.ReadString();
                byte ListTerrainCount = BR.ReadByte();
                Area LoadedArea = new Area(ActiveAreaName);
                LoadedArea.ListTerrainInArea = new List<TerrainSorcererStreet>(ListTerrainCount);

                for (int T = 0; T < ListTerrainCount; T++)
                {
                    int GridX = BR.ReadInt32();
                    int GridY = BR.ReadInt32();
                    int LayerIndex = BR.ReadInt32();

                    TerrainSorcererStreet FoundTile = LayerManager.ListLayer[LayerIndex].ArrayTerrain[GridX, GridY];
                    LoadedArea.ListTerrainInArea.Add(FoundTile);
                }

                ListArea.Add(LoadedArea);
            }
        }

        protected override TilesetPreset ReadTileset(string TilesetPresetPath, bool IsAutotile, int Index)
        {
            return SorcererStreetTilesetPreset.FromFile(TilesetPresetPath, Index);
        }

        private void SendMessage(TextInput Sender, string InputMessage)
        {
            ChatInput.SetText(string.Empty);
            OnlineCommunicationClient.SendMessage(OnlineCommunicationClient.Chat.ActiveTabID, new ChatManager.ChatMessage(DateTime.UtcNow, InputMessage, ChatManager.MessageColors.White));
        }

        public void AddPlayer(Player NewPlayer)
        {
            ListPlayer.Add(NewPlayer);
            ListLocalPlayerInfo.Add(NewPlayer);
        }

        public void AddLocalCharacter(Player NewLocalCharacter)
        {
            ListPlayer.Add(NewLocalCharacter);
            ListLocalPlayerInfo.Add(NewLocalCharacter);
        }

        public void EndPlayerPhase()
        {
            ListActionMenuChoice.RemoveAllActionPanels();

            ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelPlayerDefault(this));
        }

        public void OnNewTurn()
        {
            ActivePlayerIndex = 0;
            GameTurn++;

            UpdateMapEvent(EventTypeTurn, 0);
        }

        public void UpdateTotalMagic(Player ActivePlayer)
        {
            foreach (Team ActiveTeam in DicTeam.Values)
            {
                ActiveTeam.TotalMagic = ActivePlayer.Gold;

                foreach (TerrainSorcererStreet ActiveTerrain in ListSummonedCreature)
                {
                    if (ActiveTerrain.PlayerOwner == ActivePlayer)
                    {
                        ActiveTerrain.UpdateValue(ActiveTeam.DicCreatureCountByElementType[ActiveTerrain.TerrainTypeIndex], ActiveTerrain.DefendingCreature);
                        ActiveTeam.TotalMagic += ActiveTerrain.CurrentValue;
                    }
                }

                UpdatePlayersRank();
            }
        }

        public void UpdatePlayersRank()
        {
            List<Team> SortedList = DicTeam.Values.OrderBy(P => P.TotalMagic).ThenBy(P => P.TeamIndex).ToList();

            for (int P = 0; P < SortedList.Count; ++P)
            {
                SortedList[P].Rank = P + 1;
            }
        }

        public void OnCreatureSummon(CreatureCard SummonedCreature, TerrainSorcererStreet ActiveTerrain)
        {
            SummonedCreature.GamePiece.SetPosition(ActiveTerrain.WorldPosition + new Vector3(TileSize.X / 2, TileSize.Y / 2, 0));

            foreach (CreatureCard.ElementalAffinity ActiveElement in SummonedCreature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity)
            {
                byte ChainValue;

                if (!DicCreatureCountByElementType.TryGetValue(ActiveElement, out ChainValue))
                {
                    DicCreatureCountByElementType.Add(ActiveElement, 1);
                }
                else
                {
                    DicCreatureCountByElementType[ActiveElement] = (byte)(ChainValue + 1);
                }
            }
        }

        public void OnCreatureDeath(CreatureCard KilledCreature)
        {
            foreach (CreatureCard.ElementalAffinity ActiveElement in KilledCreature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity)
            {
                byte ChainValue;

                if (!DicCreatureCountByElementType.TryGetValue(ActiveElement, out ChainValue))
                {
                    DicCreatureCountByElementType.Add(ActiveElement, 0);
                }
                else
                {
                    DicCreatureCountByElementType[ActiveElement] = (byte)(ChainValue - 1);
                }
            }
        }

        public override void TogglePreview(bool UsePreview)
        {
            ShowUnits = UsePreview;
            if (MenuHelper.sprArrowUp == null)
            {
                MenuHelper.Init(Content);
            }

            if (!UsePreview)
            {
                //Reset game
                if (IsInit)
                {
                    Init();
                }
            }

            if (!IsServer)
            {
                ListBackground.Clear();
                ListForeground.Clear();

                for (int B = 0; B < ListBackgroundsPath.Count; B++)
                {
                    ListBackground.Add(AnimationBackground.LoadAnimationBackground(ListBackgroundsPath[B], Content, GraphicsDevice));
                }

                for (int F = 0; F < ListForegroundsPath.Count; F++)
                {
                    ListForeground.Add(AnimationBackground.LoadAnimationBackground(ListForegroundsPath[F], Content, GraphicsDevice));
                }
            }

            LayerManager.TogglePreview(UsePreview);
            MapEnvironment.Reset();
        }

        public void Reset()
        {
            LayerManager.LayerHolderDrawable.Reset();
            MapEnvironment.Reset();
        }

        public override bool CheckForObstacleAtPosition(Vector3 Position, Vector3 Displacement)
        {
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            SorcererStreetParams.Map = this;

            if (OnlineCommunicationClient != null)
            {
                OnlineCommunicationClient.ExecuteDelayedScripts();

                if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
                {
                    IsChatOpen = !IsChatOpen;

                }
                if (IsChatOpen)
                {
                    ChatHelper.UpdateChat(gameTime, OnlineCommunicationClient.Chat, ChatInput);
                }
            }

            if (!IsFrozen)
            {
                MenuHelper.UpdateAnimationTimer(gameTime);

                if (ShowUnits)
                {
                    MapEnvironment.Update(gameTime);
                }

                if (Show3DObjects)
                {
                    for (int B = 0; B < ListBackground.Count; ++B)
                    {
                        ListBackground[B].Update(gameTime);
                    }

                    for (int F = 0; F < ListForeground.Count; ++F)
                    {
                        ListForeground[F].Update(gameTime);
                    }
                }

                LayerManager.Update(gameTime);

                UpdateCursorVisiblePosition(gameTime);

                if (!IsOnTop || IsAPlatform)//Everything should be handled by the main map.
                {
                    return;
                }

                if (!IsInit)
                {
                    Init();
                }
                if (MovementAnimation.Count > 0)
                {
                    MovementAnimation.MoveSquad(gameTime, this, 100);
                }
                if (!MovementAnimation.IsBlocking || MovementAnimation.Count == 0)
                {
                    GameRule.Update(gameTime);
                }

                foreach (BattleMapPlatform ActivePlatform in ListPlatform)
                {
                    ActivePlatform.Update(gameTime);
                }
            }
        }

        public override void Update(double ElapsedSeconds)
        {
            GameTime UpdateTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(ElapsedSeconds));

            if (!IsInit)
            {
                if (ListGameScreen.Count == 0)
                {
                    Load();
                    Init();
                    TogglePreview(true);

                    if (ListGameScreen.Count == 0)
                    {
                    }
                    else
                    {
                        IsInit = false;
                    }
                }
                else
                {
                    ListGameScreen[0].Update(UpdateTime);
                    if (!ListGameScreen[0].Alive)
                    {
                        ListGameScreen.RemoveAt(0);
                    }

                    if (ListGameScreen.Count == 0)
                    {
                        IsInit = true;
                    }
                }
            }

            if (!GameGroup.IsGameReady)
            {
                return;
            }

            if (!IsServer)
            {
                LayerManager.Update(UpdateTime);
            }

            if (!ListPlayer[ActivePlayerIndex].IsPlayerControlled && ListActionMenuChoice.HasMainPanel)
            {
                ListActionMenuChoice.Last().Update(UpdateTime);
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            g.GraphicsDevice.SetRenderTarget(null);
            g.BeginUnscaled(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            GraphicsDevice.Clear(Color.Black);

            LayerManager.BeginDraw(g);

            g.End();

            if (IsOnTop)
            {
                if (ListActionMenuChoice.HasMainPanel)
                {
                    ListActionMenuChoice.Last().BeginDraw(g);
                }
            }

            foreach (BattleMapPlatform ActivePlatform in ListPlatform)
            {
                ActivePlatform.BeginDraw(g);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (!IsInit)
            {
                return;
            }

            if (!IsAPlatform)
            {
                g.GraphicsDevice.Clear(Color.Black);
            }

            //Handle screen shaking.
            if (IsShaking)
            {
                g.End();

                //Run during initialization
                ShakingRenderTraget = new RenderTarget2D(GraphicsDevice, Constants.Width, Constants.Height, false,
                    GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

                GraphicsDevice.SetRenderTarget(ShakingRenderTraget);

                g.Begin();
            }

            if (ListBackground.Count > 0 && Show3DObjects)
            {
                g.End();
                g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                g.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                g.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
                g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                for (int B = 0; B < ListBackground.Count; B++)
                {
                    ListBackground[B].Draw(g, Constants.Width, Constants.Height);
                    ListBackground[B].Draw3D(Camera3D, Matrix.CreateTranslation(new Vector3(0, 0, 0)));
                }
                g.Begin();
            }

            LayerManager.Draw(g);

            if (ListForeground.Count > 0)
            {
                g.End();
                for (int F = 0; F < ListForeground.Count; F++)
                {
                    ListForeground[F].Draw(g, Constants.Width, Constants.Height);
                }
                g.Begin();
            }

            GameRule.Draw(g);

            if (IsOnTop)
            {
                if (ListActionMenuChoice.HasMainPanel)
                {
                    ListActionMenuChoice.Last().Draw(g);
                }
            }

            if (OnlineCommunicationClient != null && IsChatOpen)
            {
                ChatHelper.DrawChat(g, fntMenuText, OnlineCommunicationClient.Chat, ChatInput);
            }

            #region Handle screen shaking.

            if (IsShaking)
            {
                g.End();

                //Switches rendertarget back to backbuffer
                GraphicsDevice.SetRenderTarget(null);

                GraphicsDevice.Clear(Color.Black);

                g.Begin();

                // counter is a float initially set to zero
                ShakeCounter += 0.45f;
                Vector2 Translation = new Vector2(ShakeOffsetX + ShakeAngleVariation.X * (float)Math.Sin(ShakeCounter),
                                                                                                    ShakeOffsetY + ShakeAngleVariation.Y * (float)Math.Sin(ShakeCounter));
                //Reached the peak of the sin function.
                if (ShakeCounter >= MathHelper.PiOver2)
                {
                    //Remember where and how the shake ended.
                    ShakeOffsetX = ShakeOffsetX + ShakeAngleVariation.X;
                    ShakeOffsetY = ShakeOffsetY + ShakeAngleVariation.Y;

                    //Calculate new shake angle.
                    ShakeAngle = (ShakeAngle + 150 + RandomHelper.Next(60)) % 360;
                    float Angle = MathHelper.ToRadians(ShakeAngle);
                    ShakeAngleVariation.X = (float)Math.Cos(Angle);
                    ShakeAngleVariation.Y = (float)Math.Sin(Angle);

                    float DestinationX = ShakeRadiusMax * ShakeAngleVariation.X;
                    float DestinationY = ShakeRadiusMax * ShakeAngleVariation.Y;

                    ShakeAngleVariation.X = DestinationX - ShakeOffsetX;
                    ShakeAngleVariation.Y = DestinationY - ShakeOffsetY;
                    ShakeCounter = 0;

                    if (IsShakingEnded)
                        IsShaking = false;
                }

                g.Draw(ShakingRenderTraget, Translation, null, Color.White,
                    0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.9f);
            }

            #endregion

            #region Handle fade to black

            if (FadeIsActive)
            {
                g.Draw(sprPixel, new Rectangle(0, 0, Constants.Width, Constants.Height), Color.FromNonPremultiplied(0, 0, 0, (int)FadeAlpha));
            }

            #endregion
        }

        public TerrainSorcererStreet GetTerrain(Vector3 GridPosition)
        {
            GridPosition = ConvertToGridPosition(GridPosition);

            if (GridPosition.X < 0 || GridPosition.X >= MapSize.X || GridPosition.Y < 0 || GridPosition.Y >= MapSize.Y || GridPosition.Z < 0 || GridPosition.Z >= LayerManager.ListLayer.Count)
            {
                return null;
            }

            TerrainSorcererStreet TemporaryTerrain;
            if (DicTemporaryTerrain.TryGetValue(GridPosition, out TemporaryTerrain))
            {
                return TemporaryTerrain;
            }

            return LayerManager.ListLayer[(int)GridPosition.Z].ArrayTerrain[(int)GridPosition.X, (int)GridPosition.Y];
        }

        public Vector3 GetNextLayerTile(MovementAlgorithmTile StartingPosition, int GridOffsetX, int GridOffsetY, float MaxClearance, float ClimbValue, out List<MovementAlgorithmTile> ListLayerPossibility)
        {
            ListLayerPossibility = new List<MovementAlgorithmTile>();
            int NextX = StartingPosition.GridPosition.X + GridOffsetX;
            int NextY = StartingPosition.GridPosition.Y + GridOffsetY;

            if (NextX < 0 || NextX >= MapSize.X || NextY < 0 || NextY >= MapSize.Y)
            {
                return StartingPosition.WorldPosition;
            }

            byte CurrentTerrainIndex = StartingPosition.TerrainTypeIndex;

            float CurrentZ = StartingPosition.WorldPosition.Z;

            MovementAlgorithmTile ClosestLayerIndexDown = null;
            MovementAlgorithmTile ClosestLayerIndexUp = StartingPosition;
            float ClosestTerrainDistanceDown = float.MaxValue;
            float ClosestTerrainDistanceUp = float.MinValue;

            bool IsOnUsableTerrain = CurrentTerrainIndex > 0;

            for (int L = 0; L < LayerManager.ListLayer.Count; L++)
            {
                MovementAlgorithmTile NextTerrain = GetTerrainIncludingPlatforms(StartingPosition, GridOffsetX, GridOffsetY, L);
                byte NextTerrainIndex = NextTerrain.TerrainTypeIndex;
                bool IsNextTerrainnUsable = NextTerrainIndex > 0;

                Terrain PreviousTerrain = GetTerrain(new Vector3(StartingPosition.WorldPosition.X, StartingPosition.WorldPosition.Y, L));
                bool IsPreviousTerrainnUsable = PreviousTerrain.TerrainTypeIndex > 0;

                if (L > StartingPosition.LayerIndex && PreviousTerrain.TerrainTypeIndex == 0)//Void, abort
                {
                    break;
                }

                float NextTerrainZ = NextTerrain.WorldPosition.Z;

                //Check lower or higher neighbors if on solid ground
                if (IsOnUsableTerrain)
                {
                    if (IsNextTerrainnUsable)
                    {
                        //Prioritize going downward
                        if (NextTerrainZ <= CurrentZ)
                        {
                            float ZDiff = CurrentZ - NextTerrainZ;
                            if (ZDiff <= ClosestTerrainDistanceDown && HasEnoughClearance(NextTerrainZ, NextX, NextY, L, MaxClearance))
                            {
                                ClosestTerrainDistanceDown = ZDiff;
                                ClosestLayerIndexDown = NextTerrain;
                                ListLayerPossibility.Add(NextTerrain);
                            }
                        }
                        else
                        {
                            float ZDiff = NextTerrainZ - CurrentZ;
                            if (ZDiff >= ClosestTerrainDistanceUp && ZDiff <= ClimbValue)
                            {
                                if (IsPreviousTerrainnUsable)
                                {
                                    ClosestTerrainDistanceUp = ZDiff;
                                    ClosestLayerIndexUp = NextTerrain;
                                    ListLayerPossibility.Add(NextTerrain);
                                }
                            }
                        }
                    }
                }
                //Already in void, check for any neighbors
                else
                {
                    if (NextTerrainZ == StartingPosition.LayerIndex && NextTerrainIndex == CurrentTerrainIndex)
                    {
                        return NextTerrain.WorldPosition;
                    }
                    //Prioritize going upward
                    else if (NextTerrainZ > StartingPosition.LayerIndex)
                    {
                        float ZDiff = NextTerrainZ - CurrentZ;
                        if (ZDiff < ClosestTerrainDistanceUp && ZDiff <= ClimbValue)
                        {
                            ClosestTerrainDistanceUp = ZDiff;
                            ClosestLayerIndexUp = NextTerrain;
                            ListLayerPossibility.Add(NextTerrain);
                        }
                    }
                }
            }

            if (ClosestLayerIndexDown != null)
            {
                return ClosestLayerIndexDown.WorldPosition;
            }
            else
            {
                return ClosestLayerIndexUp.WorldPosition;
            }
        }

        public MovementAlgorithmTile GetTerrainIncludingPlatforms(MovementAlgorithmTile StartingPosition, int OffsetX, int OffsetY, int NextLayerIndex)
        {
            if (!IsAPlatform)
            {
                foreach (BattleMapPlatform ActivePlatform in ListPlatform)
                {
                    MovementAlgorithmTile FoundTile = ((SorcererStreetMap)ActivePlatform.Map).LayerManager.ListLayer[NextLayerIndex].ArrayTerrain[StartingPosition.GridPosition.X + OffsetX, StartingPosition.GridPosition.Y + OffsetY];

                    if (FoundTile != null)
                    {
                        return FoundTile;
                    }
                }
            }

            return GetTerrain(new Vector3(StartingPosition.WorldPosition.X + OffsetX, (int)StartingPosition.WorldPosition.Y + OffsetY, NextLayerIndex));
        }

        private bool HasEnoughClearance(float CurrentZ, int NextX, int NextY, int StartLayer, float MaxClearance)
        {
            for (int L = StartLayer + 1; L < LayerManager.ListLayer.Count; L++)
            {
                Terrain ActiveTerrain = GetTerrain(new Vector3(NextX, NextY, L));

                byte NextTerrainType = ActiveTerrain.TerrainTypeIndex;
                float NextTerrainZ = ActiveTerrain.WorldPosition.Z;

                float ZDiff = NextTerrainZ - CurrentZ;

                if (NextTerrainType > 0 && ZDiff < MaxClearance)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
