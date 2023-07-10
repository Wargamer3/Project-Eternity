using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FMOD;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;
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
            LandModifierPhase, CreatureModifierPhase, EnchantModifierPhase, ItemModifierPhase, BoostModifierPhase, PrepareAttackPhase, AttackPhase, CounterPhase, ResultPhase,
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

        int ButtonsWidth = 100;
        int HeaderHeight = 30;
        int SetupMenuWidth = 240;
        int SetupMenuItemsDefender = 15;
        int SetupMenuItemsInvader = 14;
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

        private List<AnimationScreen> ListAttackAnimation;

        public BattleTesterScreen(CardSymbols Symbols, Player ActivePlayer)
        {
            this.ActivePlayer = ActivePlayer;
            this.Symbols = Symbols;

            ActiveBook = ActivePlayer.Inventory.ActiveBook;
            ActiveBook = AllCardsBook;

            ListAttackAnimation = new List<AnimationScreen>();
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            Context = SorcererStreetBattleParams.DicParams[string.Empty].GlobalContext;
            Context.ActiveParser = SorcererStreetBattleParams.DicParams[string.Empty].ActiveParser = new SorcererStreetFormulaParser(SorcererStreetBattleParams.DicParams[string.Empty]);

            ButtonHeight = fntArial12.LineSpacing + 2;

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            int MenuHeight = ButtonHeight * SetupMenuItemsInvader;

            int X = SetupMenuWidth;
            int Y = (Constants.Height - MenuHeight - HeaderHeight) / 2 + HeaderHeight;

            #region Setup UI

            InvaderCreatureButton = new BoxButton(new Rectangle(X, Y, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, InvaderCreatureSelection);
            InvaderItemButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, InvaderItemSelection);
            InvaderEnchantButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, InvaderEnchantSelection);
            InvaderCepterEnchantButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, InvaderCepterEnchantSelection);

            InvaderCepterCardsInHandInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderCepterCardsInHand, true);
            InvaderCepterCardsInHandInput.SetText("6");
            InvaderHPInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderHPInput, true);
            InvaderHPInput.SetText("30");
            InvaderMaxHPInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderMaxHPInput, true);
            InvaderMaxHPInput.SetText("30");
            InvaderSTInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderSTInput, true);
            InvaderSTInput.SetText("30");
            InvaderSupportSTBonusInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderSupportSTBonusInput, true);
            InvaderSupportSTBonusInput.SetText("10");
            InvaderAirLandsInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderAirLandsInput, true);
            InvaderAirLandsInput.SetText("1");
            InvaderEarthLandsInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderEarthLandsInput, true);
            InvaderEarthLandsInput.SetText("1");
            InvaderFireLandsInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderFireLandsInput, true);
            InvaderFireLandsInput.SetText("1");
            InvaderWaterLandsInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderWaterLandsInput, true);
            InvaderWaterLandsInput.SetText("1");

            InvaderMapCreaturesButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, InvaderMapCreaturesSelection);

            MenuHeight = ButtonHeight * SetupMenuItemsDefender;
            X = Constants.Width - ButtonsWidth;
            Y = (Constants.Height - MenuHeight - HeaderHeight) / 2 + HeaderHeight;

            DefenderCreatureButton = new BoxButton(new Rectangle(X, Y, ButtonsWidth, fntArial12.LineSpacing), fntArial12, "Select", OnButtonOver, DefenderCreatureSelection);
            DefenderItemButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, DefenderItemSelection);
            DefenderEnchantButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, DefenderEnchantSelection);
            DefenderCepterEnchantButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, DefenderCepterEnchantSelection);

            DefenderCepterCardsInHandInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderCepterCardsInHand, true);
            DefenderCepterCardsInHandInput.SetText("6");
            DefenderHPInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderHPInput, true);
            DefenderHPInput.SetText("30");
            DefenderMaxHPInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderMaxHPInput, true);
            DefenderMaxHPInput.SetText("30");
            DefenderSTInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderSTInput, true);
            DefenderSTInput.SetText("30");
            DefenderTerrainHPBonusInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderTerrainHPBonusInput, true);
            DefenderTerrainHPBonusInput.SetText("10");
            DefenderSupportSTBonusInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderSupportSTBonusInput, true);
            DefenderSupportSTBonusInput.SetText("10");
            DefenderAirLandsInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderAirLandsInput, true);
            DefenderAirLandsInput.SetText("1");
            DefenderEarthLandsInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderEarthLandsInput, true);
            DefenderEarthLandsInput.SetText("1");
            DefenderFireLandsInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderFireLandsInput, true);
            DefenderFireLandsInput.SetText("1");
            DefenderWaterLandsInput = new TextInput(fntArial12, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderWaterLandsInput, true);
            DefenderWaterLandsInput.SetText("1");

            DefenderMapCreaturesButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntArial12, "Select", OnButtonOver, DefenderMapCreaturesSelection);

            #endregion

            #region Phases UI

            MenuHeight = fntArial12.LineSpacing * 9;
            X = (Constants.Width - PhasesMenuWidth) / 2;
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
                foreach (string ActiveCardsFolder in Directory.EnumerateDirectories(GameScreen.ContentFallback.RootDirectory + "/Sorcerer Street/", "* Cards"))
                {
                    foreach (string ActiveRootFolder in Directory.EnumerateDirectories(ActiveCardsFolder, "*", SearchOption.AllDirectories))
                    {
                        foreach (string ActiveGameFolder in Directory.EnumerateDirectories(ActiveRootFolder, "*", SearchOption.AllDirectories))
                        {
                            foreach (string ActiveFile in Directory.EnumerateFiles(ActiveGameFolder, "*.pec", SearchOption.AllDirectories))
                            {
                                Card LoadedCard = Card.LoadCard(ActiveFile.Remove(ActiveFile.Length - 4, 4).Remove(0, 24), GameScreen.ContentFallback,
                                    SorcererStreetBattleParams.DicParams[string.Empty].DicRequirement, SorcererStreetBattleParams.DicParams[string.Empty].DicEffect, SorcererStreetBattleParams.DicParams[string.Empty].DicAutomaticSkillTarget);
                                LoadedCard.QuantityOwned = 1;

                                ActiveBook.AddCard(LoadedCard);
                            }
                        }
                    }
                }
            }

            Card CopyCard = ActiveBook.DicCardsByType[CreatureCard.CreatureCardType].First().Value;

            Context.Defender.Owner = new Player("Defender Player", "Defender Player", false);
            Context.Defender.Creature = (CreatureCard)CopyCard.Copy(PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
            Context.Defender.Animation = new SimpleAnimation("Defender", "Defender", Context.Defender.Creature.sprCard);
            Context.Defender.Animation.Position = new Vector2(Constants.Width - Context.Defender.Creature.sprCard.Width - Constants.Width / 9, Constants.Height / 12);
            Context.Defender.Animation.Scale = new Vector2(1f);
            Context.Defender.FinalHP = Context.Defender.Creature.OriginalMaxHP;
            Context.Defender.FinalST = Context.Defender.Creature.OriginalST;
            DefenderHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
            DefenderMaxHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
            DefenderSTInput.SetText(Context.Defender.Creature.OriginalST.ToString());

            Context.Invader.Owner = new Player("Defender Player", "Defender Player", false);
            Context.Invader.Creature = (CreatureCard)CopyCard.Copy(PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
            Context.Invader.Animation = new SimpleAnimation("Invader", "Invader", Context.Invader.Creature.sprCard);
            Context.Invader.Animation.Position = new Vector2(Constants.Width / 9, Constants.Height / 12);
            Context.Invader.Animation.Scale = new Vector2(1f);
            Context.Invader.FinalHP = Context.Defender.Creature.OriginalMaxHP;
            Context.Invader.FinalST = Context.Defender.Creature.OriginalST;
            InvaderHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
            InvaderMaxHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
            InvaderSTInput.SetText(Context.Defender.Creature.OriginalST.ToString());
        }

        public override void Update(GameTime gameTime)
        {
            ActionPanelBattle.HasFinishedUpdatingBars(gameTime, Context);

            switch (PhasesChoice)
            {
                case PhasesChoices.ItemModifierPhase:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        PhasesChoice = PhasesChoices.LandModifierPhase;
                    }
                    break;

                case PhasesChoices.PrepareAttackPhase:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        PhasesChoice = PhasesChoices.AttackPhase;
                        SorcererStreetBattleContext.BattleCreatureInfo FirstAttacker;
                        SorcererStreetBattleContext.BattleCreatureInfo SecondAttacker;
                        ActionPanelBattleAttackPhase.DetermineAttackOrder(Context, out FirstAttacker, out SecondAttacker);
                        int ReflectedDamage;
                        int Damage = ActionPanelBattleAttackPhase.ProcessAttack(Context, FirstAttacker, SecondAttacker, out ReflectedDamage);

                        if (ReflectedDamage > 0)
                        {
                            ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(SecondAttacker == Context.Defender, "Sorcerer Street/Default", FirstAttacker, Content));
                        }

                        if (Damage > 0)
                        {
                            foreach (string ActiveAnimationPath in FirstAttacker.GetAttackAnimationPaths())
                            {
                                if (string.IsNullOrEmpty(ActiveAnimationPath))
                                {
                                    ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(SecondAttacker == Context.Defender, "Sorcerer Street/Default", SecondAttacker, Content));
                                }
                                else
                                {
                                    ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(SecondAttacker == Context.Defender, ActiveAnimationPath, SecondAttacker, Content));
                                }
                            }
                        }
                        else
                        {
                            ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(SecondAttacker == Context.Defender, "Sorcerer Street/Neutralize", SecondAttacker, Content));
                        }
                    }
                    break;

                case PhasesChoices.AttackPhase:

                    ActionPanelBattle.UpdateCards(gameTime, Context);

                    if (ListAttackAnimation[0].HasLooped)
                    {
                        if (ListAttackAnimation[0].Defender.FinalHP < ListAttackAnimation[0].Defender.Creature.CurrentHP)
                        {
                            ListAttackAnimation[0].Defender.Creature.CurrentHP = ListAttackAnimation[0].Defender.FinalHP;
                        }

                        ListAttackAnimation.RemoveAt(0);

                        if (ListAttackAnimation.Count == 0)
                        {
                            PhasesChoice = PhasesChoices.LandModifierPhase;

                            Context.Invader.Animation = new SimpleAnimation("Invader", "Invader", Context.Invader.Creature.sprCard);
                            Context.Invader.Animation.Position = new Vector2(Constants.Width / 9, Constants.Height / 12);
                            Context.Invader.Animation.Scale = new Vector2(1f);
                            Context.Defender.Animation = new SimpleAnimation("Defender", "Defender", Context.Defender.Creature.sprCard);
                            Context.Defender.Animation.Position = new Vector2(Constants.Width - Context.Defender.Creature.sprCard.Width - Constants.Width / 9, Constants.Height / 12);
                            Context.Defender.Animation.Scale = new Vector2(1f);
                        }
                    }
                    break;

                default:
                    ActionPanelBattle.HasFinishedUpdatingBars(gameTime, Context);
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
                    Context.Defender.Owner = new Player("Defender Player", "Defender Player", false);
                    Context.Defender.Creature = (CreatureCard)CardSelectionScreen.ListSelectedCard[0];
                    Context.Defender.Animation = new SimpleAnimation("Defender", "Defender", Context.Defender.Creature.sprCard);
                    Context.Defender.Animation.Position = new Vector2(Constants.Width - Context.Defender.Creature.sprCard.Width - Constants.Width / 9, Constants.Height / 12);
                    Context.Defender.Animation.Scale = new Vector2(1f);
                    Context.Defender.FinalHP = Context.Defender.Creature.OriginalMaxHP;
                    Context.Defender.FinalST = Context.Defender.Creature.OriginalST;
                    DefenderHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
                    DefenderMaxHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
                    DefenderSTInput.SetText(Context.Defender.Creature.OriginalST.ToString());
                    break;

                case SetupChoices.InvaderCreature:
                    Context.Invader.Owner = new Player("Defender Player", "Defender Player", false);
                    Context.Invader.Creature = (CreatureCard)CardSelectionScreen.ListSelectedCard[0];
                    Context.Invader.Animation = new SimpleAnimation("Invader", "Invader", Context.Invader.Creature.sprCard);
                    Context.Invader.Animation.Position = new Vector2(Constants.Width / 9, Constants.Height / 12);
                    Context.Invader.Animation.Scale = new Vector2(1f);
                    Context.Invader.FinalHP = Context.Defender.Creature.OriginalMaxHP;
                    Context.Invader.FinalST = Context.Defender.Creature.OriginalST;
                    InvaderHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
                    InvaderMaxHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
                    InvaderSTInput.SetText(Context.Defender.Creature.OriginalST.ToString());
                    break;

                case SetupChoices.DefenderItem:
                    Context.Defender.Item = CardSelectionScreen.ListSelectedCard[0];
                    break;

                case SetupChoices.InvaderItem:
                    Context.Invader.Item = CardSelectionScreen.ListSelectedCard[0];
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

        private void SetDefenderCepterCardsInHand(string InputValue)
        {
        }

        private void SetDefenderHPInput(string InputValue)
        {
            int.TryParse(InputValue, out Context.Defender.FinalHP);
            Context.Defender.Creature.CurrentHP = Context.Defender.FinalHP;
        }

        private void SetDefenderMaxHPInput(string InputValue)
        {
            int.TryParse(InputValue, out Context.Defender.Creature.MaxHP);
        }

        private void SetDefenderSTInput(string InputValue)
        {
            int.TryParse(InputValue, out Context.Defender.Creature.CurrentST);
            Context.Defender.FinalST = Context.Defender.Creature.CurrentST;
        }

        private void SetDefenderTerrainHPBonusInput(string InputValue)
        {
        }

        private void SetDefenderSupportSTBonusInput(string InputValue)
        {
        }

        private void SetDefenderAirLandsInput(string InputValue)
        {
        }

        private void SetDefenderEarthLandsInput(string InputValue)
        {
        }

        private void SetDefenderFireLandsInput(string InputValue)
        {
        }

        private void SetDefenderWaterLandsInput(string InputValue)
        {
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

        private void SetInvaderCepterCardsInHand(string InputValue)
        {
        }

        private void SetInvaderHPInput(string InputValue)
        {
            int.TryParse(InputValue, out Context.Invader.FinalHP);
            Context.Invader.Creature.CurrentHP = Context.Invader.FinalHP;
        }

        private void SetInvaderMaxHPInput(string InputValue)
        {
            int.TryParse(InputValue, out Context.Invader.Creature.MaxHP);
        }

        private void SetInvaderSTInput(string InputValue)
        {
            int.TryParse(InputValue, out Context.Invader.Creature.CurrentST);
            Context.Invader.FinalST = Context.Invader.Creature.CurrentST;
        }

        private void SetInvaderSupportSTBonusInput(string InputValue)
        {
        }

        private void SetInvaderAirLandsInput(string InputValue)
        {
        }

        private void SetInvaderEarthLandsInput(string InputValue)
        {
        }

        private void SetInvaderFireLandsInput(string InputValue)
        {
        }

        private void SetInvaderWaterLandsInput(string InputValue)
        {
        }

        #endregion

        #region Phases UI

        private void IntroPhaseSelection()
        {
            Context.Invader.Creature.InitBattleBonuses();
            Context.Defender.Creature.InitBattleBonuses();

            Context.Invader.Animation = new SimpleAnimation("Invader", "Invader", Context.Invader.Creature.sprCard);
            Context.Invader.Animation.Position = new Vector2(Constants.Width / 9, Constants.Height / 12);
            Context.Invader.Animation.Scale = new Vector2(1f);
            Context.Defender.Animation = new SimpleAnimation("Defender", "Defender", Context.Defender.Creature.sprCard);
            Context.Defender.Animation.Position = new Vector2(Constants.Width - Context.Defender.Creature.sprCard.Width - Constants.Width / 9, Constants.Height / 12);
            Context.Defender.Animation.Scale = new Vector2(1f);

            Context.Defender.FinalHP = int.Parse(DefenderHPInput.Text);
            Context.Defender.Creature.CurrentHP = Context.Defender.FinalHP;
            Context.Defender.FinalST = int.Parse(DefenderSTInput.Text);

            Context.Invader.FinalHP = int.Parse(InvaderMaxHPInput.Text);
            Context.Invader.Creature.CurrentHP = Context.Invader.FinalHP;
            Context.Invader.FinalST = int.Parse(InvaderSTInput.Text);

            sndButtonClick.Play();
        }

        private void LandModifierPhaseSelection()
        {
            IntroPhaseSelection();

            Context.Defender.FinalHP += int.Parse(DefenderTerrainHPBonusInput.Text);

            Context.Invader.FinalHP += int.Parse(DefenderTerrainHPBonusInput.Text);

            sndButtonClick.Play();
        }

        private void CreatureModifierPhaseSelection()
        {
            LandModifierPhaseSelection();

            Context.ActivateSkill(Context.Invader, Context.Defender, ActionPanelBattleCreatureModifierPhase.RequirementName);
            Context.ActivateSkill(Context.Defender, Context.Invader, ActionPanelBattleCreatureModifierPhase.RequirementName);

            sndButtonClick.Play();
        }

        private void EnchantModifierPhaseSelection()
        {
            CreatureModifierPhaseSelection();
            sndButtonClick.Play();
        }

        private void ItemModifierPhaseSelection()
        {
            EnchantModifierPhaseSelection();
            if (ActionPanelBattleItemModifierPhase.InitAnimations(Context))
            {
                PhasesChoice = PhasesChoices.ItemModifierPhase;
                sndButtonClick.Play();
            }
        }

        private void BoostModifierPhaseSelection()
        {
            ItemModifierPhaseSelection();
            ActionPanelBattleBoostsModifierPhase.Init(Context);
            sndButtonClick.Play();
        }

        private void AttackPhaseSelection()
        {
            BoostModifierPhaseSelection();
            PhasesChoice = PhasesChoices.PrepareAttackPhase;
            sndButtonClick.Play();
        }

        private void CounterPhaseSelection()
        {
            BoostModifierPhaseSelection();

            SorcererStreetBattleContext.BattleCreatureInfo FirstAttacker;
            SorcererStreetBattleContext.BattleCreatureInfo SecondAttacker;
            ActionPanelBattleAttackPhase.DetermineAttackOrder(Context, out FirstAttacker, out SecondAttacker);
            int ReflectedDamage;
            int Damage = ActionPanelBattleAttackPhase.ProcessAttack(Context, FirstAttacker, SecondAttacker, out ReflectedDamage);
            SecondAttacker.FinalHP = SecondAttacker.Creature.CurrentHP = Math.Max(0, SecondAttacker.FinalHP - Damage);
            if (SecondAttacker.Creature.CurrentHP > 0)
            {
                Damage = ActionPanelBattleAttackPhase.ProcessAttack(Context, SecondAttacker, FirstAttacker, out ReflectedDamage);
                foreach (string ActiveAnimationPath in FirstAttacker.GetAttackAnimationPaths())
                {
                    if (string.IsNullOrEmpty(ActiveAnimationPath))
                    {
                        ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(SecondAttacker == Context.Defender, "Sorcerer Street/Default", FirstAttacker, Content));
                    }
                    else
                    {
                        ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(SecondAttacker == Context.Defender, ActiveAnimationPath, FirstAttacker, Content));
                    }
                }
            }
            sndButtonClick.Play();
        }

        private void ResultPhaseSelection()
        {
            sndButtonClick.Play();
        }

        #endregion

        public override void BeginDraw(CustomSpriteBatch g)
        {
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            g.GraphicsDevice.Clear(Color.Black);

            if (Context.Invader.Animation != null)
            {
                Context.Invader.Animation.BeginDraw(g);
            }
            if (Context.Defender.Animation != null)
            {
                Context.Defender.Animation.BeginDraw(g);
            }

            g.End();
        }

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

            switch (PhasesChoice)
            {
                case PhasesChoices.ItemModifierPhase:
                    ActionPanelBattleItemModifierPhase.DrawItemActivation(g, fntArial12, Context);
                    break;

                case PhasesChoices.PrepareAttackPhase:
                    ActionPanelBattleItemModifierPhase.DrawItemActivation(g, fntArial12, Context);
                    break;

                default:
                    break;
            }
        }

        public void DrawSetupMenu(CustomSpriteBatch g)
        {
            int MenuHeight = ButtonHeight * SetupMenuItemsInvader;

            int X = 0;
            int Y = (Constants.Height - MenuHeight - HeaderHeight) / 2;
            DrawBox(g, new Vector2(X, Y), SetupMenuWidth, HeaderHeight, Color.Black);
            g.DrawStringCentered(fntArial12, "Setup", new Vector2(X + SetupMenuWidth / 2, Y + HeaderHeight / 2), Color.White);

            Y += HeaderHeight;
            DrawBox(g, new Vector2(X, Y), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Creature", new Vector2(X + 5, Y + 1), Color.White);

            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Item", new Vector2(X + 5, Y + 1), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Enchant", new Vector2(X + 5, Y + 1), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Cepter Enchant", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Cepter Cards in hand", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader HP", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Max HP", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader ST", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Support ST Bonus", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Air Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Earth Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Fire Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Water Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Invader Map Creatures", new Vector2(X + 5, Y), Color.White);

            MenuHeight = ButtonHeight * SetupMenuItemsDefender;
            X = Constants.Width - SetupMenuWidth - ButtonsWidth;
            Y = (Constants.Height - MenuHeight - HeaderHeight) / 2;

            DrawBox(g, new Vector2(X, Y), SetupMenuWidth, HeaderHeight, Color.Black);
            g.DrawStringCentered(fntArial12, "Setup", new Vector2(X + SetupMenuWidth / 2, Y + HeaderHeight / 2), Color.White);

            Y += HeaderHeight;
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
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender HP", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Max HP", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender ST", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Terrain HP Bonus", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Support ST Bonus", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Air Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Earth Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Fire Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Water Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntArial12, "Defender Map Creatures", new Vector2(X + 5, Y), Color.White);
        }

        public void DrawPhaseSelectionMenu(CustomSpriteBatch g)
        {
            int MenuHeight = fntArial12.LineSpacing * 9;

            int X = (Constants.Width - PhasesMenuWidth) / 2;
            int Y = (Constants.Height - MenuHeight - HeaderHeight) / 2;

            DrawBox(g, new Vector2(X, Y), PhasesMenuWidth, HeaderHeight, Color.Black);
            g.DrawStringCentered(fntArial12, "Phases", new Vector2(X + PhasesMenuWidth / 2, Y + HeaderHeight / 2), Color.White);

            Y += HeaderHeight;
            DrawBox(g, new Vector2(X, Y), PhasesMenuWidth, MenuHeight, Color.White);
        }
    }
}
