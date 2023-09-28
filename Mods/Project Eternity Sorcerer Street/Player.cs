using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using static ProjectEternity.GameScreens.SorcererStreetScreen.CreatureCard;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class Player : OnlinePlayerBase
    {
        public override string SaveFileFolder => "Sorcerer Street/";

        public SorcererStreetInventory Inventory;

        public SorcererStreetUnit GamePiece;
        public int Rank;//Rank in the game between players
        public int Gold;
        public int TotalMagic;
        public int CompletedLaps;
        public List<SorcererStreetMap.Checkpoints> ListPassedCheckpoint;
        public Dictionary<byte, byte> DicCreatureCountByElementType;
        public Dictionary<ElementalAffinity, int> DicOwnedSymbols;
        public readonly List<Card> ListCardInDeck;
        public readonly List<Card> ListCardInHand;
        public readonly List<Card> ListRemainingCardInDeck;

        public Player()
        {
            Inventory = new SorcererStreetInventory();
            ListPassedCheckpoint = new List<SorcererStreetMap.Checkpoints>();
            DicCreatureCountByElementType = new Dictionary<byte, byte>();
            GamePiece = new SorcererStreetUnit();
            GamePiece.Direction = UnitMapComponent.DirectionNone;
            ListCardInDeck = new List<Card>();
            ListRemainingCardInDeck = new List<Card>(ListCardInDeck);
            ListCardInHand = new List<Card>();
            Init();
        }

        public Player(string ID, string Name, bool IsOnline)
            : base(ID, Name, IsOnline)
        {
            Inventory = new SorcererStreetInventory();
            ListPassedCheckpoint = new List<SorcererStreetMap.Checkpoints>();
            DicCreatureCountByElementType = new Dictionary<byte, byte>();
            GamePiece = new SorcererStreetUnit();
            GamePiece.Direction = UnitMapComponent.DirectionNone;
            ListCardInDeck = new List<Card>();
            ListRemainingCardInDeck = new List<Card>(ListCardInDeck);
            ListCardInHand = new List<Card>();
            Init();
        }

        public Player(string ID, string Name, string OnlinePlayerType, bool IsOnline, int Team, bool IsPlayerControlled, Color Color, List<Card> ListCardInDeck)
            : base(ID, Name, OnlinePlayerType, IsOnline, Team, IsPlayerControlled, Color)
        {
            this.ListCardInDeck = ListCardInDeck;

            Inventory = new SorcererStreetInventory();
            ListPassedCheckpoint = new List<SorcererStreetMap.Checkpoints>();
            DicCreatureCountByElementType = new Dictionary<byte, byte>();
            GamePiece = new SorcererStreetUnit();
            GamePiece.Direction = UnitMapComponent.DirectionNone;
            ListRemainingCardInDeck = new List<Card>();
            ListCardInHand = new List<Card>();

            CardBook NewCardBook = new CardBook();
            foreach (Card ActiveCard in ListCardInDeck)
            {
                NewCardBook.AddCard(ActiveCard);
            }

            Inventory.UseBook(NewCardBook);
            Init();
        }

        public Player(string ID, string Name, PlayerTypes OnlinePlayerType, bool IsOnline, int Team, bool IsPlayerControlled, Color Color)
            : base(ID, Name, OnlinePlayerType, IsOnline, Team, IsPlayerControlled, Color)
        {
            ListCardInDeck = new List<Card>();
            Inventory = new SorcererStreetInventory();
            ListPassedCheckpoint = new List<SorcererStreetMap.Checkpoints>();
            DicCreatureCountByElementType = new Dictionary<byte, byte>();
            GamePiece = new SorcererStreetUnit();
            GamePiece.Direction = UnitMapComponent.DirectionNone;
            ListRemainingCardInDeck = new List<Card>(ListCardInDeck);
            ListCardInHand = new List<Card>();
            Init();
        }

        public Player(Player Clone, SorcererStreetBattleParams Params)
            : base(Clone)
        {
            if (Clone == null)
            {
                return;
            }

            Inventory = Clone.Inventory;
            ListCardInDeck = new List<Card>(Clone.Inventory.ActiveBook.ListCard.Count);
            ListPassedCheckpoint = new List<SorcererStreetMap.Checkpoints>();
            DicCreatureCountByElementType = new Dictionary<byte, byte>();
            GamePiece = Clone.GamePiece;
            GamePiece.Direction = UnitMapComponent.DirectionNone;
            ListRemainingCardInDeck = new List<Card>();
            ListCardInHand = new List<Card>();
            Init();
            FillDeck(Params);
            Refill();
        }

        private void Init()
        {
            DicOwnedSymbols = new Dictionary<ElementalAffinity, int>();
            DicOwnedSymbols.Add(ElementalAffinity.Air, 0);
            DicOwnedSymbols.Add(ElementalAffinity.Earth, 0);
            DicOwnedSymbols.Add(ElementalAffinity.Fire, 0);
            DicOwnedSymbols.Add(ElementalAffinity.Water, 0);
            DicOwnedSymbols.Add(ElementalAffinity.Neutral, 0);
        }

        public void LoadGamePieceModel()
        {
            GamePiece.SpriteMap = GameScreen.ContentFallback.Load<Texture2D>("Units/Default");
            GamePiece.Unit3DSprite = new UnitMap3D(GameScreen.GraphicsDevice, GameScreen.ContentFallback.Load<Effect>("Shaders/Squad shader 3D"), GamePiece.SpriteMap, 1);
            GamePiece.Unit3DModel = new AnimatedModel("Units/Normal/Unit Models/Bomberman/Default");
            GamePiece.Unit3DModel.LoadContent(GameScreen.ContentFallback);
            GamePiece.Unit3DModel.AddAnimation("Units/Normal/Unit Models/Bomberman/Idle", "Idle", GameScreen.ContentFallback);
            GamePiece.Unit3DModel.AddAnimation("Units/Normal/Unit Models/Bomberman/Walking", "Walking", GameScreen.ContentFallback);
            GamePiece.Unit3DModel.PlayAnimation("Walking");
        }

        protected override void DoLoadLocally(ContentManager Content, BinaryReader BR)
        {
            if (Content != null && GamePiece != null)
            {
                LoadGamePieceModel();
            }

            Inventory.Load(BR, Content, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
        }

        public override void InitFirstTimeInventory()
        {
            IniFile IniDefaultCards = IniFile.ReadFromFile("Content/Sorcerer Street Lobby Default Cards.ini");

            /*foreach (string ActiveMultiplayerFolder in Directory.EnumerateDirectories(GameScreen.ContentFallback.RootDirectory + "/Sorcerer Street/", "* Cards"))
            {
                foreach (string ActiveCampaignFolder in Directory.EnumerateDirectories(ActiveMultiplayerFolder, "*", SearchOption.AllDirectories))
                {
                    foreach (string ActiveFile in Directory.EnumerateFiles(ActiveCampaignFolder, "*.pec", SearchOption.AllDirectories))
                    {
                        Card LoadedCard = Card.LoadCard(ActiveFile.Remove(ActiveFile.Length - 4, 4).Remove(0, 24), GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                        LoadedCard.QuantityOwned = 1;

                        Inventory.GlobalBook.AddCard(LoadedCard);
                    }
                }
            }*/

            foreach (string ActiveKey in IniDefaultCards.ReadAllKeys())
            {
                string CardPath = IniDefaultCards.ReadField(ActiveKey, "Path");
                string CardQuantity = IniDefaultCards.ReadField(ActiveKey, "Quantity");
                Card LoadedCard = Card.LoadCard(CardPath, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                LoadedCard.QuantityOwned = int.Parse(CardQuantity);

                Inventory.GlobalBook.AddCard(LoadedCard);
            }

            CardBook DefaultBook = new CardBook("Default");
            for (int C = 0; C < 50 && C < Inventory.GlobalBook.ListCard.Count; ++C)
            {
                DefaultBook.AddCard(Inventory.GlobalBook.ListCard[C]);
            }

            Inventory.ActiveBook = DefaultBook;
            Inventory.ListBook.Add(DefaultBook);
        }

        protected override void DoSaveLocally(BinaryWriter BW)
        {
            Inventory.Save(BW);
        }

        public void FillDeck(SorcererStreetBattleParams Params)
        {
            ListCardInDeck.Clear();
            for (int C = 0; C < Inventory.ActiveBook.ListCard.Count; C++)
            {
                for (int Q = 0; Q < Inventory.ActiveBook.ListCard[C].QuantityOwned; ++Q)
                {
                    ListCardInDeck.Add(Inventory.ActiveBook.ListCard[C].Copy(Params.DicRequirement, Params.DicEffect, Params.DicAutomaticSkillTarget));
                }
            }
        }

        public void Refill()
        {
            ListRemainingCardInDeck.Clear();
            ListRemainingCardInDeck.AddRange(ListCardInDeck);

            int RemainingCardToShuffleCount = ListRemainingCardInDeck.Count;

            while (RemainingCardToShuffleCount-- > 1)
            {
                int ShuffleIndex = RandomHelper.Next(RemainingCardToShuffleCount + 1);
                Card ActiveCard = ListRemainingCardInDeck[ShuffleIndex];

                ListRemainingCardInDeck[ShuffleIndex] = ListRemainingCardInDeck[RemainingCardToShuffleCount];
                ListRemainingCardInDeck[RemainingCardToShuffleCount] = ActiveCard;
            }
        }

        public void IncreaseChainLevels(byte TerrainTypeIndex)
        {
            byte ChainValue;

            if (!DicCreatureCountByElementType.TryGetValue(TerrainTypeIndex, out ChainValue))
            {
                DicCreatureCountByElementType.Add(TerrainTypeIndex, 1);
            }
            else
            {
                DicCreatureCountByElementType[TerrainTypeIndex] = (byte)(ChainValue + 1);
            }
        }

        public void DecreaseChainLevels(byte TerrainTypeIndex)
        {
            byte ChainValue;

            if (!DicCreatureCountByElementType.TryGetValue(TerrainTypeIndex, out ChainValue))
            {
                DicCreatureCountByElementType.Add(TerrainTypeIndex, 0);
            }
            else
            {
                DicCreatureCountByElementType[TerrainTypeIndex] = (byte)(ChainValue - 1);
            }
        }

        public override List<MissionInfo> GetUnlockedMissions()
        {
            throw new NotImplementedException();
        }
    }
}
