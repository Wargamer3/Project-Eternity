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
            public int DiceValue;
            public int DiceValueMin;
            public int DiceValueMax;

            public float ItemCostMultiplier;
            public float SpellCostMultiplier;
            public float CreatureCostMultiplier;

            public bool ItemLimit;//Cannont use Items
            public bool SpellLimit;//Cannont use Spells
            public bool CreatureLimit;//Cannont use Creatures
            public bool InvasionLimit;//Cannot invade.
            public bool CreatureEnchantProtection;//Target Player's territories with Enchantments cannot be targeted by Enchantment spells or Enchantment territory abilities for 8 rounds.

            public float TollGainShareMultiplier;//User gains X% of tolls collected by other Players until user reaches the castle.
            public bool TollProtection;//Target Player is exempt from tolls.
            public bool TollLimit;//Target enemy Player cannot claim tolls.

            public float CastleValueMultiplier;//% gold gained by castles
            public bool AllowTerrainCommands;//Can use Territory commands on any of his/her creatures. This is the same effect as landing on a fort or castle.

            public PlayerAbilities()
            {
                Backward = false;
                DiceValue = -1;
                DiceValueMin = -1;
                DiceValueMax = -1;

                ItemCostMultiplier = 1;
                SpellCostMultiplier = 1;
                CreatureCostMultiplier = 1;

                CreatureLimit = false;
                InvasionLimit = false;
                CreatureEnchantProtection = false;

                TollGainShareMultiplier = 0;
                TollProtection = false;
                TollLimit = false;

                CastleValueMultiplier = 1;
                AllowTerrainCommands = false;
            }

            public PlayerAbilities(PlayerAbilities Other)
            {
                Backward = Other.Backward;
                DiceValue = Other.DiceValue;
                DiceValueMin = Other.DiceValueMin;
                DiceValueMax = Other.DiceValueMax;

                ItemCostMultiplier = Other.ItemCostMultiplier;
                SpellCostMultiplier = Other.SpellCostMultiplier;
                CreatureCostMultiplier = Other.CreatureCostMultiplier;

                CreatureLimit = Other.CreatureLimit;
                InvasionLimit = Other.InvasionLimit;
                CreatureEnchantProtection = Other.CreatureEnchantProtection;

                TollLimit = Other.TollLimit;
                TollProtection = Other.TollProtection;
                TollGainShareMultiplier = Other.TollGainShareMultiplier;

                CastleValueMultiplier = Other.CastleValueMultiplier;
                AllowTerrainCommands = Other.AllowTerrainCommands;
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
                NewCardBook.AddCard(new CardInfo(ActiveCard, 1));
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

            foreach (string ActiveKey in IniDefaultCards.ReadAllKeys())
            {
                string CardPath = IniDefaultCards.ReadField(ActiveKey, "Path");
                string CardQuantity = IniDefaultCards.ReadField(ActiveKey, "Quantity");
                Card LoadedCard = Card.LoadCard(CardPath, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);

                Inventory.GlobalBook.AddCard(new CardInfo(LoadedCard, byte.Parse(CardQuantity)));
            }

            CardBook DefaultBook = new CardBook("Default");
            for (int C = 0; C < 50 && C < Inventory.GlobalBook.ListCard.Count; ++C)
            {
                DefaultBook.AddCard(Inventory.GlobalBook.ListCard[C]);
            }

            Inventory.ActiveBook = DefaultBook;
            Inventory.AddBook(DefaultBook);

            IniFile IniDefaultCharacters = IniFile.ReadFromFile("Content/Sorcerer Street Lobby Default Characters.ini");

            PlayerCharacterInfo FirstCharacter = null;
            foreach (string ActiveKey in IniDefaultCharacters.ReadAllKeys())
            {
                string CharacterPath = IniDefaultCharacters.ReadField(ActiveKey, "Path");
                PlayerCharacterInfo LoadedCharacter = new PlayerCharacterInfo(new PlayerCharacter(CharacterPath, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget));
                Inventory.DicOwnedCharacter.Add(LoadedCharacter.Character.CharacterPath, LoadedCharacter);
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
                CardInfo ActiveCard = null;
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
                        ActiveCard = new CardInfo(Card.LoadCard(ActiveHeaderValues.Item2, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget), 0);
                        ActiveBook.AddCard(ActiveCard);
                    }
                    else if (ActiveHeaderValues.Item1 == "Quantity")
                    {
                        ActiveCard.QuantityOwned = byte.Parse(ActiveHeaderValues.Item2);
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
                    ListCardInDeck.Add(Inventory.ActiveBook.ListCard[C].Card.Copy(Params.DicRequirement, Params.DicEffect, Params.DicAutomaticSkillTarget, Params.DicManualSkillTarget));
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

        public int GetFinalCardCost(Card SelectedCard)
        {
            int TotalCost = SelectedCard.MagicCost;

            switch (SelectedCard.CardType)
            {
                case CreatureCardType:
                    TotalCost = (int)(TotalCost * GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).CreatureCostMultiplier);
                    break;
                case SpellCard.SpellCardType:
                    TotalCost = (int)(TotalCost * GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).SpellCostMultiplier);
                    break;
                case ItemCard.ItemCardType:
                    TotalCost = (int)(TotalCost * GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).ItemCostMultiplier);
                    break;
            }

            return TotalCost;
        }

        public bool CanUseCard(Card SelectedCard)
        {
            switch (SelectedCard.CardType)
            {
                case CreatureCardType:
                    if (GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).CreatureLimit)
                    {
                        return false;
                    }
                    break;

                case SpellCard.SpellCardType:
                    if (GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).SpellLimit)
                    {
                        return false;
                    }
                    break;

                case ItemCard.ItemCardType:
                    if (GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).ItemLimit)
                    {
                        return false;
                    }
                    break;
            }

            return Gold >= GetFinalCardCost(SelectedCard);
        }

        public override List<MissionInfo> GetUnlockedMissions()
        {
            return new List<MissionInfo>();
        }
    }
}
