using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;
using FMOD;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class BattleTesterScreen : GameScreen
    {
        private enum SetupChoices
        {
            Nothing, DefenderCreature, DefenderItem, DefenderEnchant, DefenderCepterEnchant, DefenderMapCreatures,
            InvaderCreature, InvaderItem, InvaderEnchant, InvaderCepterEnchant, InvaderMapCreatures,
        }

        private enum PhasesChoices
        {
            LandModifierPhase, CreatureModifierPhase, EnchantModifierPhase, ItemModifierPhase, BoostModifierPhase, AttackPhase, CounterPhase, ResultPhase,
        }

        #region Ressources

        private CardSymbols Symbols;

        private SpriteFont fntArial12;

        private FMODSound sndButtonOver;
        protected FMODSound sndButtonClick;

        #region Setup UI

        private BoxButton DefenderCreatureButton;
        private BoxButton DefenderItemButton;
        private BoxButton DefenderEnchantButton;
        private BoxButton DefenderCepterEnchantButton;
        private TextInput DefenderCepterCardsInHandInput;
        private TextInput DefenderHPInput;
        private TextInput DefenderMaxHPInput;
        private TextInput DefenderSTInput;
        private TextInput DefenderTerrainHPBonusInput;
        private TextInput DefenderSupportSTBonusInput;
        private TextInput DefenderAirLandsInput;
        private TextInput DefenderEarthLandsInput;
        private TextInput DefenderFireLandsInput;
        private TextInput DefenderWaterLandsInput;
        private BoxButton DefenderMapCreaturesButton;

        private BoxButton InvaderCreatureButton;
        private BoxButton InvaderItemButton;
        private BoxButton InvaderEnchantButton;
        private BoxButton InvaderCepterEnchantButton;
        private TextInput InvaderCepterCardsInHandInput;
        private TextInput InvaderHPInput;
        private TextInput InvaderMaxHPInput;
        private TextInput InvaderSTInput;
        private TextInput InvaderSupportSTBonusInput;
        private TextInput InvaderAirLandsInput;
        private TextInput InvaderEarthLandsInput;
        private TextInput InvaderFireLandsInput;
        private TextInput InvaderWaterLandsInput;
        private BoxButton InvaderMapCreaturesButton;

        #endregion

        #region Phases UI

        private BoxButton LandModifierPhaseButton;
        private BoxButton CreatureModifierPhaseButton;
        private BoxButton EnchantModifierPhaseButton;
        private BoxButton ItemModifierPhaseButton;
        private BoxButton BoostModifierPhaseButton;
        private BoxButton AttackPhaseButton;
        private BoxButton CounterPhaseButton;
        private BoxButton ResultPhaseButton;

        #endregion

        private IUIElement[] ArrayMenuButton;

        #endregion

        int HeaderHeight = 30;
        int SetupMenuWidth = 240;
        int SetupMenuItems = 29;
        int PhasesMenuWidth = 150;
        int ButtonHeight;

        private readonly Player ActivePlayer;
        private CardBook ActiveBook;
        private static CardBook AllCardsBook;

        private int CursorIndex;
        private SetupChoices SetupChoice;
        private PhasesChoices PhasesChoice;
        private EditBookCardListFilterScreen CardSelectionScreen;
        private SorcererStreetBattleContext Context;

        public BattleTesterScreen(CardSymbols Symbols, Player ActivePlayer)
        {
            this.ActivePlayer = ActivePlayer;
            this.Symbols = Symbols;

            ActiveBook = ActivePlayer.Inventory.ActiveBook;
            ActiveBook = AllCardsBook;
            Context = new SorcererStreetBattleContext();
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            ButtonHeight = fntArial12.LineSpacing + 2;

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            int MenuHeight = ButtonHeight * SetupMenuItems;
            int ButtonsWidth = 100;

            int X = SetupMenuWidth;
            int Y = (Constants.Height - MenuHeight - HeaderHeight) / 2 + HeaderHeight;

            #region Setup UI

            DefenderCreatureButton = new BoxButton(new Rectangle(X, Y, ButtonsWidth, fntArial12.LineSpacing), fntArial12, "Select", OnButtonOver, DefenderCreatureSelection);
            DefenderItemButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, DefenderItemSelection);
            DefenderEnchantButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, DefenderEnchantSelection);
            DefenderCepterEnchantButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, DefenderCepterEnchantSelection);

            DefenderCepterCardsInHandInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            DefenderCepterCardsInHandInput.SetText("6");
            DefenderHPInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            DefenderHPInput.SetText("30");
            DefenderMaxHPInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            DefenderMaxHPInput.SetText("30");
            DefenderSTInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            DefenderSTInput.SetText("30");
            DefenderTerrainHPBonusInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            DefenderTerrainHPBonusInput.SetText("10");
            DefenderSupportSTBonusInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            DefenderSupportSTBonusInput.SetText("10");
            DefenderAirLandsInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            DefenderAirLandsInput.SetText("1");
            DefenderEarthLandsInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            DefenderEarthLandsInput.SetText("1");
            DefenderFireLandsInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            DefenderFireLandsInput.SetText("1");
            DefenderWaterLandsInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            DefenderWaterLandsInput.SetText("1");

            DefenderMapCreaturesButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, DefenderMapCreaturesSelection);

            InvaderCreatureButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, InvaderCreatureSelection);
            InvaderItemButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, InvaderItemSelection);
            InvaderEnchantButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, InvaderEnchantSelection);
            InvaderCepterEnchantButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, InvaderCepterEnchantSelection);

            InvaderCepterCardsInHandInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            InvaderCepterCardsInHandInput.SetText("6");
            InvaderHPInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            InvaderHPInput.SetText("30");
            InvaderMaxHPInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            InvaderMaxHPInput.SetText("30");
            InvaderSTInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            InvaderSTInput.SetText("30");
            InvaderSupportSTBonusInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            InvaderSupportSTBonusInput.SetText("10");
            InvaderAirLandsInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            InvaderAirLandsInput.SetText("1");
            InvaderEarthLandsInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            InvaderEarthLandsInput.SetText("1");
            InvaderFireLandsInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            InvaderFireLandsInput.SetText("1");
            InvaderWaterLandsInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            InvaderWaterLandsInput.SetText("1");

            InvaderMapCreaturesButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, InvaderMapCreaturesSelection);

            #endregion

            #region Phases UI

            MenuHeight = fntArial12.LineSpacing * 9;
            X = Constants.Width - PhasesMenuWidth;
            Y = (Constants.Height - MenuHeight - HeaderHeight) / 2 + HeaderHeight;

            LandModifierPhaseButton = new BoxButton(new Rectangle(X, Y, PhasesMenuWidth, fntArial12.LineSpacing), fntArial12, "Land Modifier", OnButtonOver, LandModifierPhaseSelection);
            CreatureModifierPhaseButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, PhasesMenuWidth, fntArial12.LineSpacing), fntArial12, "Creature Modifier", OnButtonOver, CreatureModifierPhaseSelection);
            EnchantModifierPhaseButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, PhasesMenuWidth, fntArial12.LineSpacing), fntArial12, "Enchant Modifier", OnButtonOver, EnchantModifierPhaseSelection);
            ItemModifierPhaseButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, PhasesMenuWidth, fntArial12.LineSpacing), fntArial12, "Item Modifier", OnButtonOver, ItemModifierPhaseSelection);
            BoostModifierPhaseButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, PhasesMenuWidth, fntArial12.LineSpacing), fntArial12, "Boost Modifier", OnButtonOver, BoostModifierPhaseSelection);
            AttackPhaseButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, PhasesMenuWidth, fntArial12.LineSpacing), fntArial12, "Attack", OnButtonOver, AttackPhaseSelection);
            CounterPhaseButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, PhasesMenuWidth, fntArial12.LineSpacing), fntArial12, "Counter", OnButtonOver, CounterPhaseSelection);
            ResultPhaseButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, PhasesMenuWidth, fntArial12.LineSpacing), fntArial12, "Result", OnButtonOver, ResultPhaseSelection);

            #endregion

            ArrayMenuButton = new IUIElement[]
            {
                DefenderCreatureButton, DefenderItemButton, DefenderEnchantButton, DefenderCepterEnchantButton,
                DefenderCepterCardsInHandInput, DefenderHPInput, DefenderMaxHPInput, DefenderSTInput, DefenderTerrainHPBonusInput, DefenderSupportSTBonusInput,
                DefenderAirLandsInput, DefenderEarthLandsInput, DefenderFireLandsInput, DefenderWaterLandsInput, DefenderMapCreaturesButton,
                InvaderCreatureButton, InvaderItemButton, InvaderEnchantButton, InvaderCepterEnchantButton,
                InvaderCepterCardsInHandInput, InvaderHPInput, InvaderMaxHPInput, InvaderSTInput, InvaderSupportSTBonusInput,
                InvaderAirLandsInput, InvaderEarthLandsInput, InvaderFireLandsInput, InvaderWaterLandsInput, InvaderMapCreaturesButton,
                LandModifierPhaseButton, CreatureModifierPhaseButton, EnchantModifierPhaseButton, ItemModifierPhaseButton,
                BoostModifierPhaseButton, AttackPhaseButton, CounterPhaseButton, ResultPhaseButton,
            };

            if (AllCardsBook == null)
            {
                ActiveBook = AllCardsBook = new CardBook();
                foreach (string ActiveMultiplayerFolder in Directory.EnumerateDirectories(GameScreen.ContentFallback.RootDirectory + "/Sorcerer Street/", "* Cards"))
                {
                    foreach (string ActiveCampaignFolder in Directory.EnumerateDirectories(ActiveMultiplayerFolder, "*", SearchOption.AllDirectories))
                    {
                        foreach (string ActiveFile in Directory.EnumerateFiles(ActiveCampaignFolder, "*.pec", SearchOption.AllDirectories))
                        {
                            Card LoadedCard = Card.LoadCard(ActiveFile.Remove(ActiveFile.Length - 4, 4).Remove(0, 24), GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                            LoadedCard.QuantityOwned = 1;

                            ActiveBook.AddCard(LoadedCard);
                        }
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            ActionPanelBattle.CanUpdate(gameTime, Context);
            switch (PhasesChoice)
            {
                case PhasesChoices.ItemModifierPhase:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        PhasesChoice = PhasesChoices.AttackPhase;
                    }
                    break;

                default:
                    ActionPanelBattle.CanUpdate(gameTime, Context);
                    break;
            }

            GetMenuResponse();

            if (InputHelper.InputConfirmPressed())
            {
            }
            else if (InputHelper.InputCancelPressed())
            {
            }
            else if (InputHelper.InputUpPressed())
            {
                if (--CursorIndex < 0)
                {
                    CursorIndex = ActivePlayer.Inventory.ListBook.Count + 1;
                }
            }
            else if (InputHelper.InputDownPressed())
            {
                if (++CursorIndex > ActivePlayer.Inventory.ListBook.Count + 1)
                {
                    CursorIndex = 0;
                }
            }

            foreach (IUIElement ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Update(gameTime);
            }
        }

        private void GetMenuResponse()
        {
            if (CardSelectionScreen == null || CardSelectionScreen.ListSelectedCard.Count == 0)
            {
                return;
            }

            switch (SetupChoice)
            {
                case SetupChoices.DefenderCreature:
                    Context.Defender = (CreatureCard)CardSelectionScreen.ListSelectedCard[0];
                    Context.DefenderCard = new SimpleAnimation("Defender", "Defender", Context.Defender.sprCard);
                    Context.DefenderCard.Position = new Vector2(Constants.Width - Context.Defender.sprCard.Width - Constants.Width / 9, Constants.Height / 12);
                    Context.DefenderCard.Scale = new Vector2(1f);
                    Context.DefenderFinalHP = Context.Defender.MaxHP;
                    Context.DefenderFinalST = Context.Defender.MaxST;
                    DefenderHPInput.SetText(Context.Defender.MaxHP.ToString());
                    DefenderMaxHPInput.SetText(Context.Defender.MaxHP.ToString());
                    DefenderSTInput.SetText(Context.Defender.MaxST.ToString());
                    Context.DefenderPlayer = new Player("Defender Player", "Defender Player", false);
                    break;

                case SetupChoices.InvaderCreature:
                    Context.Invader = (CreatureCard)CardSelectionScreen.ListSelectedCard[0];
                    Context.InvaderCard = new SimpleAnimation("Invader", "Invader", Context.Invader.sprCard);
                    Context.InvaderCard.Position = new Vector2(Constants.Width / 9, Constants.Height / 12);
                    Context.InvaderCard.Scale = new Vector2(1f);
                    Context.InvaderFinalHP = Context.Defender.MaxHP;
                    Context.InvaderFinalST = Context.Defender.MaxST;
                    InvaderHPInput.SetText(Context.Defender.MaxHP.ToString());
                    InvaderMaxHPInput.SetText(Context.Defender.MaxHP.ToString());
                    InvaderSTInput.SetText(Context.Defender.MaxST.ToString());
                    Context.InvaderPlayer = new Player("Defender Player", "Defender Player", false);
                    break;

                case SetupChoices.DefenderItem:
                    Context.DefenderItem = CardSelectionScreen.ListSelectedCard[0];
                    break;

                case SetupChoices.InvaderItem:
                    Context.InvaderItem = CardSelectionScreen.ListSelectedCard[0];
                    break;
            }
            SetupChoice = SetupChoices.Nothing;
            CardSelectionScreen = null;
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        #region Setup UI

        private void DefenderCreatureSelection()
        {
            SetupChoice = SetupChoices.DefenderCreature;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Creatures, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void DefenderItemSelection()
        {
            SetupChoice = SetupChoices.DefenderItem;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Item, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void DefenderEnchantSelection()
        {
            SetupChoice = SetupChoices.DefenderEnchant;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Spells, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void DefenderCepterEnchantSelection()
        {
            SetupChoice = SetupChoices.DefenderCepterEnchant;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Spells, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void DefenderMapCreaturesSelection()
        {
            SetupChoice = SetupChoices.DefenderMapCreatures;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Creatures, true);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void InvaderCreatureSelection()
        {
            SetupChoice = SetupChoices.InvaderCreature;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Creatures, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void InvaderItemSelection()
        {
            SetupChoice = SetupChoices.InvaderItem;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Item, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void InvaderEnchantSelection()
        {
            SetupChoice = SetupChoices.InvaderEnchant;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Spells, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void InvaderCepterEnchantSelection()
        {
            SetupChoice = SetupChoices.InvaderCepterEnchant;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Spells, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void InvaderMapCreaturesSelection()
        {
            SetupChoice = SetupChoices.InvaderMapCreatures;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Creatures, true);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void SetDefenderCepterCardsInHand(string InputValue)
        {
        }

        #endregion

        #region Phases UI

        private void LandModifierPhaseSelection()
        {
            Context.DefenderFinalHP = int.Parse(InvaderMaxHPInput.Text);
            Context.DefenderFinalST = int.Parse(InvaderSTInput.Text);
            Context.DefenderFinalHP += 10 * int.Parse(DefenderTerrainHPBonusInput.Text);
            sndButtonClick.Play();
        }

        private void CreatureModifierPhaseSelection()
        {
            LandModifierPhaseSelection();

            Context.ActiveSkill(Context.Invader, Context.Defender,Context.InvaderPlayer, Context.DefenderPlayer, ActionPanelBattleCreatureModifierPhase.RequirementName);
            Context.ActiveSkill(Context.Defender, Context.Invader, Context.DefenderPlayer, Context.InvaderPlayer, ActionPanelBattleCreatureModifierPhase.RequirementName);

            sndButtonClick.Play();
        }

        private void EnchantModifierPhaseSelection()
        {
            sndButtonClick.Play();
        }

        private void ItemModifierPhaseSelection()
        {
            if (ActionPanelBattleItemModifierPhase.InitAnimations(Context))
            {
                PhasesChoice = PhasesChoices.ItemModifierPhase;
                sndButtonClick.Play();
            }
        }

        private void BoostModifierPhaseSelection()
        {
            sndButtonClick.Play();
        }

        private void AttackPhaseSelection()
        {
            sndButtonClick.Play();
        }

        private void CounterPhaseSelection()
        {
            sndButtonClick.Play();
        }

        private void ResultPhaseSelection()
        {
            sndButtonClick.Play();
        }

        #endregion

        public override void Draw(CustomSpriteBatch g)
        {
            g.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1, 0);
            ActionPanelBattle.DrawInvaderBattle(fntArial12, Context, g);
            ActionPanelBattle.DrawDefenderBattle(fntArial12, Context, g);

            DrawSetupMenu(g);
            DrawPhaseSelectionMenu(g);

            foreach (IUIElement ActiveElement in ArrayMenuButton)
            {
                ActiveElement.Draw(g);
            }
        }

        public void DrawSetupMenu(CustomSpriteBatch g)
        {
            int MenuHeight = ButtonHeight * SetupMenuItems;

            int X = 0;
            int Y = (Constants.Height - MenuHeight - HeaderHeight) / 2;
            DrawBox(g, new Vector2(X, Y), SetupMenuWidth, HeaderHeight, Color.Black);
            g.DrawStringCentered(fntArial12, "Setup", new Vector2(X + SetupMenuWidth / 2, Y + HeaderHeight / 2), Color.White);

            Y += HeaderHeight;
            DrawBox(g, new Vector2(X, Y), SetupMenuWidth, MenuHeight, Color.White);
            DrawBox(g, new Vector2(X, Y), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Creature", new Vector2(X + 5, Y + 1), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Item", new Vector2(X + 5, Y + 1), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Enchant", new Vector2(X + 5, Y + 1), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Cepter Enchant", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Cepter Cards in hand", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender HP", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Max HP", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender ST", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Terrain HP Bonus", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Support ST Bonus", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Air Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Earth Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Fire Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Water Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Map Creatures", new Vector2(X + 5, Y), Color.White);

            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Creature", new Vector2(X + 5, Y + 1), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Item", new Vector2(X + 5, Y + 1), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Enchant", new Vector2(X + 5, Y + 1), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Cepter Enchant", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Cepter Cards in hand", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader HP", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Max HP", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader ST", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Support ST Bonus", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Air Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Earth Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Fire Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Water Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), 100, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Map Creatures", new Vector2(X + 5, Y), Color.White);
        }

        public void DrawPhaseSelectionMenu(CustomSpriteBatch g)
        {
            int MenuHeight = fntArial12.LineSpacing * 9;

            int X = Constants.Width - PhasesMenuWidth;
            int Y = (Constants.Height - MenuHeight - HeaderHeight) / 2;

            DrawBox(g, new Vector2(X, Y), PhasesMenuWidth, HeaderHeight, Color.Black);
            g.DrawStringCentered(fntArial12, "Phases", new Vector2(X + PhasesMenuWidth / 2, Y + HeaderHeight / 2), Color.White);

            Y += HeaderHeight;
            DrawBox(g, new Vector2(X, Y), PhasesMenuWidth, MenuHeight, Color.White);
        }
    }
}
