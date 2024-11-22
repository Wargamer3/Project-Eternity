using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using static ProjectEternity.GameScreens.SorcererStreetScreen.CreatureCard;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class Player : OnlinePlayerBase
    {
        public class PlayerAbilities
        {
            public bool Backward;
            public bool Blackout;
            public int DiceValue;
            public PlayerAbilities()
            {
                Backward = false;
                Blackout = false;
                DiceValue = -1;
            }

            public PlayerAbilities(PlayerAbilities Other)
            {
                Backward = Other.Backward;
                Blackout = Other.Blackout;
                DiceValue = Other.DiceValue;
            }
        }

        public override string SaveFileFolder => "Sorcerer Street/";

        public SorcererStreetInventory Inventory;
        public SorcererStreetPlayerUnlockInventory UnlockInventory;

        public SorcererStreetUnit GamePiece;
        public int Gold;
        public int CompletedLaps;

        private PlayerAbilities Abilities;//Base abilities that cannot be modified.
        private PlayerAbilities EnchantAbilities;//Based on Abilities

        private PlayerAbilities BattleAbilities;//Based on EnchantAbilities

        public Enchant Enchant;

        public List<SorcererStreetMap.Checkpoints> ListPassedCheckpoint;
        public Dictionary<ElementalAffinity, int> DicOwnedSymbols;

        public readonly List<Card> ListCardInDeck;
        public readonly List<Card> ListCardInHand;
        public readonly List<Card> ListRemainingCardInDeck;

        public readonly List<SorcererStreetUnit> ListCreatureOnBoard;

        public Player()
        {
            Inventory = new SorcererStreetInventory();
            UnlockInventory = new SorcererStreetPlayerUnlockInventory();
            UnlocksEvaluator = new SorcererStreetUnlockConditionsEvaluator(this);

            ListPassedCheckpoint = new List<SorcererStreetMap.Checkpoints>();
            GamePiece = new SorcererStreetUnit();
            GamePiece.Direction = UnitMapComponent.DirectionNone;
            ListCardInDeck = new List<Card>();
            ListRemainingCardInDeck = new List<Card>(ListCardInDeck);
            ListCardInHand = new List<Card>();
            ListCreatureOnBoard = new List<SorcererStreetUnit>();

            Init();
        }

        public Player(string ID, string Name, bool IsOnline)
            : base(ID, Name, IsOnline)
        {
            Inventory = new SorcererStreetInventory();
            UnlockInventory = new SorcererStreetPlayerUnlockInventory();
            UnlocksEvaluator = new SorcererStreetUnlockConditionsEvaluator(this);

            ListPassedCheckpoint = new List<SorcererStreetMap.Checkpoints>();
            GamePiece = new SorcererStreetUnit();
            GamePiece.Direction = UnitMapComponent.DirectionNone;
            ListCardInDeck = new List<Card>();
            ListRemainingCardInDeck = new List<Card>(ListCardInDeck);
            ListCardInHand = new List<Card>();
            ListCreatureOnBoard = new List<SorcererStreetUnit>();
            Init();
        }

        public Player(string ID, string Name, string OnlinePlayerType, bool IsOnline, int Team, bool IsPlayerControlled, Color Color, List<Card> ListCardInDeck)
            : base(ID, Name, OnlinePlayerType, IsOnline, Team, IsPlayerControlled, Color)
        {
            this.ListCardInDeck = ListCardInDeck;

            Inventory = new SorcererStreetInventory();
            UnlockInventory = new SorcererStreetPlayerUnlockInventory();
            UnlocksEvaluator = new SorcererStreetUnlockConditionsEvaluator(this);

            ListPassedCheckpoint = new List<SorcererStreetMap.Checkpoints>();
            GamePiece = new SorcererStreetUnit();
            GamePiece.Direction = UnitMapComponent.DirectionNone;
            ListRemainingCardInDeck = new List<Card>();
            ListCardInHand = new List<Card>();
            ListCreatureOnBoard = new List<SorcererStreetUnit>();

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
            UnlockInventory = new SorcererStreetPlayerUnlockInventory();
            UnlocksEvaluator = new SorcererStreetUnlockConditionsEvaluator(this);

            ListPassedCheckpoint = new List<SorcererStreetMap.Checkpoints>();
            GamePiece = new SorcererStreetUnit();
            GamePiece.Direction = UnitMapComponent.DirectionNone;
            ListRemainingCardInDeck = new List<Card>(ListCardInDeck);
            ListCardInHand = new List<Card>();
            ListCreatureOnBoard = new List<SorcererStreetUnit>();

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
            UnlockInventory = Clone.UnlockInventory;
            UnlocksEvaluator = Clone.UnlocksEvaluator;

            ListCardInDeck = new List<Card>(Clone.Inventory.ActiveBook.ListCard.Count);
            ListPassedCheckpoint = new List<SorcererStreetMap.Checkpoints>();
            GamePiece = Clone.GamePiece;
            GamePiece.Direction = UnitMapComponent.DirectionNone;
            ListRemainingCardInDeck = new List<Card>();
            ListCardInHand = new List<Card>();
            ListCreatureOnBoard = new List<SorcererStreetUnit>();

            Init();
            FillDeck(Params);
            Refill();
        }

        private void Init()
        {
            Abilities = new PlayerAbilities();
            EnchantAbilities = new PlayerAbilities();
            BattleAbilities = new PlayerAbilities();

            DicOwnedSymbols = new Dictionary<ElementalAffinity, int>();
            DicOwnedSymbols.Add(ElementalAffinity.Air, 0);
            DicOwnedSymbols.Add(ElementalAffinity.Earth, 0);
            DicOwnedSymbols.Add(ElementalAffinity.Fire, 0);
            DicOwnedSymbols.Add(ElementalAffinity.Water, 0);
            DicOwnedSymbols.Add(ElementalAffinity.Neutral, 0);
        }

        protected override void DoLoadLocally(ContentManager Content, BinaryReader BR)
        {
            Inventory.Load(BR, Content, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
            UnlockInventory.LoadPlayerUnlocks(Name);
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
                Card LoadedCard = Card.LoadCard(CardPath, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                LoadedCard.QuantityOwned = int.Parse(CardQuantity);

                Inventory.GlobalBook.AddCard(LoadedCard);
            }

            CardBook DefaultBook = new CardBook("Default");
            for (int C = 0; C < 50 && C < Inventory.GlobalBook.ListCard.Count; ++C)
            {
                DefaultBook.AddCard(Inventory.GlobalBook.ListCard[C]);
            }

            Inventory.ActiveBook = DefaultBook;
            Inventory.DicOwnedBook.Add(DefaultBook.BookName, DefaultBook);

            IniFile IniDefaultCharacters = IniFile.ReadFromFile("Content/Sorcerer Street Lobby Default Characters.ini");

            PlayerCharacter FirstCharacter = null;
            foreach (string ActiveKey in IniDefaultCharacters.ReadAllKeys())
            {
                string CharacterPath = IniDefaultCharacters.ReadField(ActiveKey, "Path");
                PlayerCharacter LoadedCharacter = new PlayerCharacter(CharacterPath, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                Inventory.DicOwnedCharacter.Add(LoadedCharacter.CharacterPath, LoadedCharacter);
                Inventory.AddCharacter(LoadedCharacter);

                if (FirstCharacter == null)
                {
                    FirstCharacter = LoadedCharacter;
                }
            }

            Inventory.Character = FirstCharacter;

            IniFile IniDefaultMissions = IniFile.ReadFromFile("Content/Sorcerer Street Lobby Default Missions.ini");

            foreach (string ActiveKey in IniDefaultMissions.ReadAllKeys())
            {
                string MissionPath = IniDefaultMissions.ReadField(ActiveKey, "Path");
                Inventory.DicOwnedMission.Add(MissionPath, new MissionInfo(MissionPath, 0));
            }

            InitFirstTimeBot();

            UnlockInventory.LoadPlayerUnlocks(Name);
        }

        private void InitFirstTimeBot()
        {
            IniFileReader GlobalUnlockIniAsync = new IniFileReader("Content/Sorcerer Street Lobby Default Bots.ini");

            Dictionary<string, List<Tuple<string, string>>> DicContentByHeader = GlobalUnlockIniAsync.ReadAllContent();

            foreach (string BotName in DicContentByHeader.Keys)
            {
                int BookNumber = 1;
                string CharacterName = null;
                CardBook ActiveBook = null;
                Card ActiveCard = null;
                Dictionary<string, CardBook> DicOwnedBook = new Dictionary<string, CardBook>();

                foreach (Tuple<string, string> ActiveHeaderValues in DicContentByHeader[BotName])
                {
                    if (ActiveHeaderValues.Item1 == "Character")
                    {
                        CharacterName = ActiveHeaderValues.Item2;
                    }
                    else
                    if (ActiveHeaderValues.Item1 == "Book" + BookNumber)
                    {
                        ActiveBook = new CardBook(ActiveHeaderValues.Item2);
                        BookNumber++;
                        DicOwnedBook.Add(ActiveHeaderValues.Item2, ActiveBook);
                    }
                    else if (ActiveHeaderValues.Item1 == "Card")
                    {
                        ActiveCard = Card.LoadCard(ActiveHeaderValues.Item2, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                        ActiveBook.AddCard(ActiveCard);
                    }
                    else if (ActiveHeaderValues.Item1 == "Quantity")
                    {
                        ActiveCard.QuantityOwned = int.Parse(ActiveHeaderValues.Item2);
                    }
                }

                Inventory.DicOwnedBot.Add(BotName, new Bot(new PlayerCharacter(CharacterName, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget), DicOwnedBook));
            }
        }

        protected override void DoSaveLocally(BinaryWriter BW)
        {
            Inventory.Save(BW);
            UnlockInventory.LoadPlayerUnlocks(Name);
        }

        public void FillDeck(SorcererStreetBattleParams Params)
        {
            ListCardInDeck.Clear();
            for (int C = 0; C < Inventory.ActiveBook.ListCard.Count; C++)
            {
                for (int Q = 0; Q < Inventory.ActiveBook.ListCard[C].QuantityOwned; ++Q)
                {
                    ListCardInDeck.Add(Inventory.ActiveBook.ListCard[C].Copy(Params.DicRequirement, Params.DicEffect, Params.DicAutomaticSkillTarget, Params.DicManualSkillTarget));
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

        public void ClearAbilities()
        {
            EnchantAbilities = null;
            BattleAbilities = null;
        }

        public PlayerAbilities GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases EffectActivationPhase)
        {
            if (EffectActivationPhase == SorcererStreetBattleContext.EffectActivationPhases.Enchant)
            {
                if (EnchantAbilities == null)
                {
                    EnchantAbilities = new PlayerAbilities(Abilities);
                }
                return EnchantAbilities;
            }
            else if (EffectActivationPhase == SorcererStreetBattleContext.EffectActivationPhases.Battle)
            {
                if (BattleAbilities == null)
                {
                    BattleAbilities = new PlayerAbilities(Abilities);
                }
                return BattleAbilities;
            }

            return Abilities;
        }

        public override List<MissionInfo> GetUnlockedMissions()
        {
            throw new NotImplementedException();
        }
    }
}
