using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class Player : OnlinePlayerBase
    {
        public override string SaveFileFolder => "Sorcerer Street/";

        public SorcererStreetInventory Inventory;

        public SorcererStreetUnit GamePiece;
        public int Rank;//Rank in the game between players
        public int Magic;
        public int TotalMagic;
        public int CompletedLaps;
        public List<SorcererStreetMap.Checkpoints> ListPassedCheckpoint;
        public Dictionary<byte, byte> DicChainLevelByTerrainTypeIndex;
        public readonly Card[] ArrayCardInDeck;
        public readonly List<Card> ListCardInHand;
        public readonly List<Card> ListRemainingCardInDeck;

        public Player()
        {
            Inventory = new SorcererStreetInventory();
            ListPassedCheckpoint = new List<SorcererStreetMap.Checkpoints>();
            DicChainLevelByTerrainTypeIndex = new Dictionary<byte, byte>();
            GamePiece = new SorcererStreetUnit();
            GamePiece.Direction = Core.Units.UnitMapComponent.Directions.None;
            ListRemainingCardInDeck = new List<Card>(ArrayCardInDeck);
            ListCardInHand = new List<Card>();
        }

        public Player(string ID, string Name, string OnlinePlayerType, bool IsOnline, int Team, bool IsPlayerControlled, Color Color, Card[] ArrayCardInDeck)
            : base(ID, Name, OnlinePlayerType, IsOnline, Team, IsPlayerControlled, Color)
        {
            this.ArrayCardInDeck = ArrayCardInDeck;

            Inventory = new SorcererStreetInventory();
            ListPassedCheckpoint = new List<SorcererStreetMap.Checkpoints>();
            DicChainLevelByTerrainTypeIndex = new Dictionary<byte, byte>();
            GamePiece = new SorcererStreetUnit();
            GamePiece.Direction = UnitMapComponent.Directions.None;
            ListRemainingCardInDeck = new List<Card>(ArrayCardInDeck);
            ListCardInHand = new List<Card>();
        }

        public Player(string ID, string Name, PlayerTypes OnlinePlayerType, bool IsOnline, int Team, bool IsPlayerControlled, Color Color)
            : base(ID, Name, OnlinePlayerType, IsOnline, Team, IsPlayerControlled, Color)
        {
            ArrayCardInDeck = new Card[0];
            Inventory = new SorcererStreetInventory();
            ListPassedCheckpoint = new List<SorcererStreetMap.Checkpoints>();
            DicChainLevelByTerrainTypeIndex = new Dictionary<byte, byte>();
            GamePiece = new SorcererStreetUnit();
            GamePiece.Direction = UnitMapComponent.Directions.None;
            ListRemainingCardInDeck = new List<Card>(ArrayCardInDeck);
            ListCardInHand = new List<Card>();
        }


        public Player(Player Clone)
            : base(Clone)
        {
            if (Clone == null)
            {
                return;
            }

            Inventory = Clone.Inventory;
            ArrayCardInDeck = new Card[Clone.Inventory.ActiveBook.ListCard.Count];
            for (int C = 0; C < Clone.Inventory.ActiveBook.ListCard.Count; C++)
            {
                ArrayCardInDeck[C] = Clone.Inventory.ActiveBook.ListCard[C].Copy(PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
            }

            ListPassedCheckpoint = new List<SorcererStreetMap.Checkpoints>();
            DicChainLevelByTerrainTypeIndex = new Dictionary<byte, byte>();
            GamePiece = Clone.GamePiece;
            GamePiece.Direction = UnitMapComponent.Directions.None;
            ListRemainingCardInDeck = new List<Card>(ArrayCardInDeck);
            ListCardInHand = new List<Card>();
        }

        protected override void DoLoadLocally(ContentManager Content, BinaryReader BR)
        {
            if (Content != null && GamePiece != null)
            {
                GamePiece.SpriteMap = Content.Load<Texture2D>("Units/Default");
                GamePiece.Unit3DSprite = new UnitMap3D(GameScreen.GraphicsDevice, Content.Load<Effect>("Shaders/Squad shader 3D"), GamePiece.SpriteMap, 1);
                GamePiece.Unit3DModel = new AnimatedModel("Units/Normal/Models/Bomberman/Default");
                GamePiece.Unit3DModel.LoadContent(Content);
                GamePiece.Unit3DModel.AddAnimation("Units/Normal/Models/Bomberman/Waving", "Idle", Content);
                GamePiece.Unit3DModel.AddAnimation("Units/Normal/Models/Bomberman/Walking", "Walking", Content);
                GamePiece.Unit3DModel.PlayAnimation("Walking");
            }

            Inventory.Load(BR, Content, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
        }

        public override void InitFirstTimeInventory()
        {
            IniFile IniDefaultCards = IniFile.ReadFromFile("Content/Sorcerer Street Lobby Default Cards.ini");

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

        public void IncreaseChainLevels(byte TerrainTypeIndex)
        {
            byte ChainValue;

            if (!DicChainLevelByTerrainTypeIndex.TryGetValue(TerrainTypeIndex, out ChainValue))
            {
                DicChainLevelByTerrainTypeIndex.Add(TerrainTypeIndex, 1);
            }
            else
            {
                DicChainLevelByTerrainTypeIndex[TerrainTypeIndex] = (byte)(ChainValue + 1);
            }
        }

        public void DecreaseChainLevels(byte TerrainTypeIndex)
        {
            byte ChainValue;

            if (!DicChainLevelByTerrainTypeIndex.TryGetValue(TerrainTypeIndex, out ChainValue))
            {
                DicChainLevelByTerrainTypeIndex.Add(TerrainTypeIndex, 0);
            }
            else
            {
                DicChainLevelByTerrainTypeIndex[TerrainTypeIndex] = (byte)(ChainValue - 1);
            }
        }
    }
}
