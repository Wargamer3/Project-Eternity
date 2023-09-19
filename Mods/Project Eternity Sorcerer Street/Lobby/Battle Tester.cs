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
            Idle, IntroPhase, LandModifierPhase, CreatureModifierPhase, EnchantModifierPhase, ItemModifierPhase, BoostModifierPhase, BeforeBattle,
            BattleStartAnimation, PrepareAttackPhase, InvaderAttackBonusAnimation, InvaderAttackPhase1, InvaderAttackPhase2, CounterAttackBonusAnimation, CounterPhase1, CounterPhase2,
            InvaderBattleEnd, DefenderBattleEnd, UponVictory, UponDefeat, ResultPhase,
        }

        #region Ressources

        private CardSymbols Symbols;

        private SpriteFont fntArial12;
        private Texture2D sprVS;

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

        private BoxButton IntroPhaseButton;
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
        int PhaseMenuItems = 10;
        int PhasesMenuWidth = 150;
        int ButtonHeight;

        private readonly Player ActivePlayer;
        private CardBook ActiveBook;
        private static CardBook AllCardsBook;

        private int CursorIndex;
        private SetupChoices SetupChoice;
        private PhasesChoices PhasesChoice;
        private PhasesChoices PhasesEnd;
        private EditBookCardListFilterScreen CardSelectionScreen;
        private SorcererStreetBattleContext Context;

        private List<SimpleAnimation> ListAttackAnimation;

        private Dictionary<CreatureCard.ElementalAffinity, byte> DicCreatureCountByElementType;

        public BattleTesterScreen(CardSymbols Symbols, Player ActivePlayer)
        {
            this.ActivePlayer = ActivePlayer;
            this.Symbols = Symbols;

            ActiveBook = ActivePlayer.Inventory.ActiveBook;
            ActiveBook = AllCardsBook;

            ListAttackAnimation = new List<SimpleAnimation>();
            DicCreatureCountByElementType = new Dictionary<CreatureCard.ElementalAffinity, byte>();
            DicCreatureCountByElementType.Add(CreatureCard.ElementalAffinity.Air, 1);
            DicCreatureCountByElementType.Add(CreatureCard.ElementalAffinity.Earth, 1);
            DicCreatureCountByElementType.Add(CreatureCard.ElementalAffinity.Fire, 1);
            DicCreatureCountByElementType.Add(CreatureCard.ElementalAffinity.Water, 1);
            DicCreatureCountByElementType.Add(CreatureCard.ElementalAffinity.Neutral, 1);
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            sprVS = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/VS");
            Context = SorcererStreetBattleParams.DicParams[string.Empty].GlobalContext;
            Context.ActiveParser = SorcererStreetBattleParams.DicParams[string.Empty].ActiveParser = new SorcererStreetFormulaParser(SorcererStreetBattleParams.DicParams[string.Empty]);
            Context.DicCreatureCountByElementType = DicCreatureCountByElementType;
            Context.DefenderTerrain = new TerrainSorcererStreet(0, 0, 0, 0);
            Context.DefenderTerrain.LandLevel = 1;

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
            InvaderCepterCardsInHandInput.SetText("1");
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
            DefenderCepterCardsInHandInput.SetText("1");
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

            MenuHeight = fntArial12.LineSpacing * PhaseMenuItems;
            X = (Constants.Width - PhasesMenuWidth) / 2;
            Y = (Constants.Height - MenuHeight - HeaderHeight) / 2 + HeaderHeight;

            IntroPhaseButton = new BoxButton(new Rectangle(X, Y, PhasesMenuWidth, fntArial12.LineSpacing), fntArial12, "Intro Modifier", OnButtonOver, IntroPhaseSelection);
            LandModifierPhaseButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, PhasesMenuWidth, fntArial12.LineSpacing), fntArial12, "Land Modifier", OnButtonOver, LandModifierPhaseSelection);
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
                IntroPhaseButton, LandModifierPhaseButton, CreatureModifierPhaseButton, EnchantModifierPhaseButton, ItemModifierPhaseButton,
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
            Context.Defender.Owner.ListCardInHand.Add(new CreatureCard(""));
            Context.Defender.Creature = (CreatureCard)CopyCard.Copy(PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
            Context.Defender.Animation = new SimpleAnimation("Defender", "Defender", Context.Defender.Creature.sprCard);
            Context.Defender.Animation.Position = new Vector2(Constants.Width - Context.Defender.Creature.sprCard.Width - Constants.Width / 9, Constants.Height / 12);
            Context.Defender.Animation.Scale = new Vector2(1f);
            DefenderHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
            DefenderMaxHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
            DefenderSTInput.SetText(Context.Defender.Creature.OriginalST.ToString());

            Context.Invader.Owner = new Player("Defender Player", "Defender Player", false);
            Context.Invader.Owner.ListCardInHand.Add(new CreatureCard(""));
            Context.Invader.Creature = (CreatureCard)CopyCard.Copy(PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
            Context.Invader.Animation = new SimpleAnimation("Invader", "Invader", Context.Invader.Creature.sprCard);
            Context.Invader.Animation.Position = new Vector2(Constants.Width / 9, Constants.Height / 12);
            Context.Invader.Animation.Scale = new Vector2(1f);
            InvaderHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
            InvaderMaxHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
            InvaderSTInput.SetText(Context.Defender.Creature.OriginalST.ToString());


            if (Context.ListActivatedEffect.Count > 0)
            {
                Context.ListActivatedEffect.Clear();
            }
        }

        public override void Update(GameTime gameTime)
        {
            ActionPanelBattle.HasFinishedUpdatingBars(gameTime, Context);

            switch (PhasesChoice)
            {
                case PhasesChoices.Idle:
                    ActionPanelBattle.UpdateCards(gameTime, Context);
                    break;

                case PhasesChoices.IntroPhase:
                    if (!ActionPanelBattleStartPhase.UpdateAnimation(gameTime, Context))
                    {
                        PhasesChoice = PhasesChoices.Idle;
                    }
                    if (!Context.Invader.Creature.UseCardAnimation && Context.Invader.Animation.HasEnded)
                    {
                        ActionPanelBattleAttackAnimationPhase.InitAnimation(true, Context.Invader.Creature.IdleAnimationPath, Context.Invader, true);
                        Context.Invader.Animation.Position = new Vector2(1750, 190);
                    }
                    break;

                case PhasesChoices.LandModifierPhase:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        if (PhasesEnd == PhasesChoices.LandModifierPhase)
                        {
                            PhasesChoice = PhasesChoices.Idle;
                        }
                        else
                        {
                            ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, ActionPanelBattleCreatureModifierPhase.RequirementName);
                            PhasesChoice = PhasesChoices.CreatureModifierPhase;
                        }
                    }
                    break;

                case PhasesChoices.CreatureModifierPhase:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        if (PhasesEnd == PhasesChoices.CreatureModifierPhase)
                        {
                            PhasesChoice = PhasesChoices.Idle;
                        }
                        else
                        {
                            ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, ActionPanelBattleItemModifierPhase.RequirementName);
                            PhasesChoice = PhasesChoices.ItemModifierPhase;
                        }
                    }
                    break;

                case PhasesChoices.ItemModifierPhase:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        if (PhasesEnd == PhasesChoices.ItemModifierPhase)
                        {
                            PhasesChoice = PhasesChoices.Idle;
                        }
                        else
                        {
                            ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, ActionPanelBattleBoostsModifierPhase.RequirementName);
                            PhasesChoice = PhasesChoices.BoostModifierPhase;
                        }
                    }
                    break;

                case PhasesChoices.BoostModifierPhase:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        Context.ListActivatedEffect.Clear();

                        ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context,ActionPanelBattleAttackPhase.BeforeBattleStartRequirement);

                        if (!Context.Invader.Creature.UseCardAnimation)
                        {
                            ActionPanelBattleAttackAnimationPhase.InitAnimation(true, Context.Invader.Creature.IdleAnimationPath, Context.Invader, true);
                            Context.Invader.Animation.Position = new Vector2(1750, 190);
                        }

                        PhasesChoice = PhasesChoices.BeforeBattle;
                    }
                    break;

                case PhasesChoices.BeforeBattle:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        Context.ListActivatedEffect.Clear();

                        ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, ActionPanelBattleAttackPhase.BattleStartRequirement);

                        PhasesChoice = PhasesChoices.BattleStartAnimation;
                    }
                    break;

                case PhasesChoices.BattleStartAnimation:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        Context.ListActivatedEffect.Clear();

                        PhasesChoice = PhasesChoices.PrepareAttackPhase;
                    }
                    break;

                case PhasesChoices.PrepareAttackPhase:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        if (Context.Invader.Creature.CurrentHP <= 0)
                        {
                            PhasesChoice = PhasesChoices.Idle;
                            return;
                        }

                        PhasesChoice = PhasesChoices.InvaderAttackBonusAnimation;
                        SorcererStreetBattleContext.BattleCreatureInfo FirstAttacker;
                        SorcererStreetBattleContext.BattleCreatureInfo SecondAttacker;
                        ActionPanelBattleAttackPhase.DetermineAttackOrder(Context, out FirstAttacker, out SecondAttacker);

                        ActionPanelBattleAttackPhase.ProcessAttack(FirstAttacker, SecondAttacker);
                        if (SecondAttacker.DamageReceived > 0)
                        {
                            ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, FirstAttacker == Context.Invader, ActionPanelBattleAttackPhase.AttackBonusRequirement);
                        }
                    }
                    break;

                case PhasesChoices.InvaderAttackBonusAnimation:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        SorcererStreetBattleContext.BattleCreatureInfo FirstAttacker;
                        SorcererStreetBattleContext.BattleCreatureInfo SecondAttacker;
                        ActionPanelBattleAttackPhase.DetermineAttackOrder(Context, out FirstAttacker, out SecondAttacker);
                        StartAttackAnimation(FirstAttacker, SecondAttacker);
                        PhasesChoice = PhasesChoices.InvaderAttackPhase1;
                    }
                    break;

                case PhasesChoices.CounterAttackBonusAnimation:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        SorcererStreetBattleContext.BattleCreatureInfo FirstAttacker;
                        SorcererStreetBattleContext.BattleCreatureInfo SecondAttacker;
                        ActionPanelBattleAttackPhase.DetermineAttackOrder(Context, out FirstAttacker, out SecondAttacker);
                        StartAttackAnimation(SecondAttacker, FirstAttacker);
                        PhasesChoice = PhasesChoices.CounterPhase1;
                    }
                    break;

                case PhasesChoices.InvaderAttackPhase1:
                case PhasesChoices.InvaderAttackPhase2://Attack Twice
                case PhasesChoices.CounterPhase1:
                case PhasesChoices.CounterPhase2://Attack Twice
                    ActionPanelBattle.UpdateCards(gameTime, Context);

                    ListAttackAnimation[0].Update(gameTime);
                    if (ListAttackAnimation[0].HasEnded)
                    {
                        AnimationScreen ActiveAnimation = (AnimationScreen)ListAttackAnimation[0].ActiveAnimation;
                        if (ActiveAnimation.Defender.FinalHP < ActiveAnimation.Defender.Creature.CurrentHP)
                        {
                            ActiveAnimation.Defender.Creature.CurrentHP = ActiveAnimation.Defender.FinalHP;
                        }

                        ListAttackAnimation.RemoveAt(0);

                        if (ListAttackAnimation.Count == 0)
                        {
                            SorcererStreetBattleContext.BattleCreatureInfo FirstAttacker;
                            SorcererStreetBattleContext.BattleCreatureInfo SecondAttacker;
                            ActionPanelBattleAttackPhase.DetermineAttackOrder(Context, out FirstAttacker, out SecondAttacker);

                            if (ActiveAnimation.Defender.FinalHP > 0)
                            {
                                if (PhasesChoice == PhasesChoices.InvaderAttackPhase1)
                                {
                                    if (FirstAttacker.Creature.BattleAbilities.AttackTwice)
                                    {
                                        StartAttackAnimation(FirstAttacker, SecondAttacker);
                                        PhasesChoice = PhasesChoices.InvaderAttackPhase2;
                                        return;
                                    }
                                    else
                                    {
                                        ActionPanelBattleAttackPhase.ProcessAttack(SecondAttacker, FirstAttacker);
                                        if (FirstAttacker.DamageReceived > 0)
                                        {
                                            ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, SecondAttacker == Context.Invader, ActionPanelBattleAttackPhase.AttackBonusRequirement);
                                        }
                                        PhasesChoice = PhasesChoices.CounterAttackBonusAnimation;
                                    }
                                }
                                else if (PhasesChoice == PhasesChoices.InvaderAttackPhase2)
                                {
                                    ActionPanelBattleAttackPhase.ProcessAttack(SecondAttacker, FirstAttacker);
                                    PhasesChoice = PhasesChoices.CounterPhase1;
                                }
                                else if (PhasesChoice == PhasesChoices.CounterPhase1)
                                {
                                    if (SecondAttacker.Creature.BattleAbilities.AttackTwice)
                                    {
                                        StartAttackAnimation(SecondAttacker, FirstAttacker);
                                        PhasesChoice = PhasesChoices.InvaderAttackPhase2;
                                        return;
                                    }
                                    else
                                    {
                                        ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, ActionPanelBattleAttackPhase.BattleEndRequirement);
                                        PhasesChoice = PhasesChoices.InvaderBattleEnd;
                                    }
                                }
                                else if (PhasesChoice == PhasesChoices.CounterPhase2)
                                {
                                    ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, ActionPanelBattleAttackPhase.BattleEndRequirement);
                                    PhasesChoice = PhasesChoices.InvaderBattleEnd;
                                }
                            }
                            else
                            {
                                if (Context.Invader.Creature.CurrentHP <= 0)
                                {
                                    ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, false, ActionPanelBattleAttackPhase.UponVictoryRequirement);
                                }
                                else
                                {
                                    ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, true, ActionPanelBattleAttackPhase.UponVictoryRequirement);
                                }

                                PhasesChoice = PhasesChoices.UponVictory;
                            }
                        }
                    }
                    break;

                case PhasesChoices.InvaderBattleEnd:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, false, ActionPanelBattleAttackPhase.BattleEndRequirement);

                        PhasesChoice = PhasesChoices.DefenderBattleEnd;
                    }
                    break;

                case PhasesChoices.UponVictory:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        if (Context.Invader.Creature.CurrentHP <= 0)
                        {
                            ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, true, ActionPanelBattleAttackPhase.UponDefeatRequirement);
                        }
                        else
                        {
                            ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, false, ActionPanelBattleAttackPhase.UponDefeatRequirement);
                        }

                        PhasesChoice = PhasesChoices.UponDefeat;
                    }
                    break;

                case PhasesChoices.UponDefeat:
                case PhasesChoices.DefenderBattleEnd:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        PhasesChoice = PhasesChoices.Idle;

                        Context.Invader.Animation = new SimpleAnimation("Invader", "Invader", Context.Invader.Creature.sprCard);
                        Context.Invader.Animation.Position = new Vector2(Constants.Width / 9, Constants.Height / 12);
                        Context.Invader.Animation.Scale = new Vector2(1f);
                        Context.Defender.Animation = new SimpleAnimation("Defender", "Defender", Context.Defender.Creature.sprCard);
                        Context.Defender.Animation.Position = new Vector2(Constants.Width - Context.Defender.Creature.sprCard.Width - Constants.Width / 9, Constants.Height / 12);
                        Context.Defender.Animation.Scale = new Vector2(1f);
                        DefenderTerrainHPBonusInput.SetText((Context.DefenderTerrain.LandLevel * 10).ToString());
                    }
                    break;

                default:
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
                    InvaderHPInput.SetText(Context.Invader.Creature.OriginalMaxHP.ToString());
                    InvaderMaxHPInput.SetText(Context.Invader.Creature.OriginalMaxHP.ToString());
                    InvaderSTInput.SetText(Context.Invader.Creature.OriginalST.ToString());
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

        private void StartAttackAnimation(SorcererStreetBattleContext.BattleCreatureInfo FirstAttacker, SorcererStreetBattleContext.BattleCreatureInfo SecondAttacker)
        {
            int ReflectedDamage = FirstAttacker.DamageReceived;
            int Damage = SecondAttacker.DamageReceived;

            if (ReflectedDamage > 0)
            {
                ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(SecondAttacker != Context.Defender, "Sorcerer Street/Default", FirstAttacker, true));
            }

            if (Damage > 0)
            {
                if (FirstAttacker.Creature.UseCardAnimation)
                {
                    foreach (string ActiveAnimationPath in FirstAttacker.GetAttackAnimationPaths())
                    {
                        if (string.IsNullOrEmpty(ActiveAnimationPath))
                        {
                            ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(SecondAttacker == Context.Defender, "Sorcerer Street/Default", SecondAttacker, true));
                        }
                        else
                        {
                            ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(SecondAttacker == Context.Defender, ActiveAnimationPath, SecondAttacker, true));
                        }
                    }
                }
                else
                {
                    ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(SecondAttacker == Context.Defender, FirstAttacker.Creature.AttackStartAnimationPath, FirstAttacker, true));
                    FirstAttacker.Animation.Position = new Vector2(1750, 190);
                    ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(SecondAttacker == Context.Defender, FirstAttacker.Creature.AttackEndAnimationPath, SecondAttacker, true));
                    SecondAttacker.Animation.Position = new Vector2(573, 90);

                    FirstAttacker.Animation = null;
                    SecondAttacker.Animation = null;
                    SecondAttacker.Animation = new SimpleAnimation("Defender", "Defender", Context.Defender.Creature.sprCard);
                    SecondAttacker.Animation.Position = new Vector2(Constants.Width - Context.Defender.Creature.sprCard.Width - Constants.Width / 9, Constants.Height / 12);
                    SecondAttacker.Animation.Scale = new Vector2(1f);
                }
            }
            else
            {
                ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(SecondAttacker == Context.Defender, "Sorcerer Street/Neutralize", SecondAttacker, true));
            }
        }

        #region Setup UI

        private void DefenderCreatureSelection()
        {
            SetupChoice = SetupChoices.DefenderCreature;
            PhasesChoice = PhasesChoices.Idle;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Creatures, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void DefenderItemSelection()
        {
            SetupChoice = SetupChoices.DefenderItem;
            PhasesChoice = PhasesChoices.Idle;
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
            int CardsInHand;
            int.TryParse(InputValue, out CardsInHand);
            while (Context.Defender.Owner.ListCardInHand.Count < CardsInHand)
            {
                Context.Defender.Owner.ListCardInHand.Add(new CreatureCard("Dummy"));
            }
            while (Context.Defender.Owner.ListCardInHand.Count > CardsInHand)
            {
                Context.Defender.Owner.ListCardInHand.RemoveAt(0);
            }
        }

        private void SetDefenderHPInput(string InputValue)
        {
            int.TryParse(InputValue, out Context.Defender.Creature.CurrentHP);
        }

        private void SetDefenderMaxHPInput(string InputValue)
        {
            int.TryParse(InputValue, out Context.Defender.Creature.MaxHP);
        }

        private void SetDefenderSTInput(string InputValue)
        {
            int.TryParse(InputValue, out Context.Defender.Creature.CurrentST);
        }

        private void SetDefenderTerrainHPBonusInput(string InputValue)
        {
        }

        private void SetDefenderSupportSTBonusInput(string InputValue)
        {
        }

        private void SetDefenderAirLandsInput(string InputValue)
        {
            byte FinalValue;
            byte.TryParse(InputValue, out FinalValue);
            DicCreatureCountByElementType[CreatureCard.ElementalAffinity.Air] = FinalValue;
        }

        private void SetDefenderEarthLandsInput(string InputValue)
        {
            byte FinalValue;
            byte.TryParse(InputValue, out FinalValue);
            DicCreatureCountByElementType[CreatureCard.ElementalAffinity.Earth] = FinalValue;
        }

        private void SetDefenderFireLandsInput(string InputValue)
        {
            byte FinalValue;
            byte.TryParse(InputValue, out FinalValue);
            DicCreatureCountByElementType[CreatureCard.ElementalAffinity.Fire] = FinalValue;
        }

        private void SetDefenderWaterLandsInput(string InputValue)
        {
            byte FinalValue;
            byte.TryParse(InputValue, out FinalValue);
            DicCreatureCountByElementType[CreatureCard.ElementalAffinity.Water] = FinalValue;
        }

        private void InvaderCreatureSelection()
        {
            SetupChoice = SetupChoices.InvaderCreature;
            PhasesChoice = PhasesChoices.Idle;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Creatures, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void InvaderItemSelection()
        {
            SetupChoice = SetupChoices.InvaderItem;
            PhasesChoice = PhasesChoices.Idle;
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
            int CardsInHand;
            int.TryParse(InputValue, out CardsInHand);
            while (Context.Invader.Owner.ListCardInHand.Count < CardsInHand)
            {
                Context.Invader.Owner.ListCardInHand.Add(new CreatureCard("Dummy"));
            }
            while (Context.Invader.Owner.ListCardInHand.Count > CardsInHand)
            {
                Context.Invader.Owner.ListCardInHand.RemoveAt(0);
            }
        }

        private void SetInvaderHPInput(string InputValue)
        {
            int.TryParse(InputValue, out Context.Invader.Creature.CurrentHP);
        }

        private void SetInvaderMaxHPInput(string InputValue)
        {
            int.TryParse(InputValue, out Context.Invader.Creature.MaxHP);
        }

        private void SetInvaderSTInput(string InputValue)
        {
            int.TryParse(InputValue, out Context.Invader.Creature.CurrentST);
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
            PhasesChoice = PhasesChoices.IntroPhase;

            Context.Invader.Creature.InitBattleBonuses();
            Context.Defender.Creature.InitBattleBonuses();

            Context.Invader.Animation = new SimpleAnimation("Invader", "Invader", Context.Invader.Creature.sprCard);
            Context.Invader.Animation.Position = new Vector2(Constants.Width / 9, Constants.Height / 12);
            Context.Invader.Animation.Scale = new Vector2(1f);
            Context.Defender.Animation = new SimpleAnimation("Defender", "Defender", Context.Defender.Creature.sprCard);
            Context.Defender.Animation.Position = new Vector2(Constants.Width - Context.Defender.Creature.sprCard.Width - Constants.Width / 9, Constants.Height / 12);
            Context.Defender.Animation.Scale = new Vector2(1f);

            Context.Defender.Creature.CurrentHP = int.Parse(DefenderHPInput.Text);
            Context.Defender.Creature.CurrentST = int.Parse(DefenderSTInput.Text);
            Context.Defender.BonusHP = 0;
            Context.Defender.BonusST = 0;
            Context.Defender.LandHP = 0;

            Context.Invader.Creature.CurrentHP = int.Parse(InvaderHPInput.Text);
            Context.Invader.Creature.CurrentST = int.Parse(InvaderSTInput.Text);
            Context.Invader.BonusHP = 0;
            Context.Invader.BonusST = 0;
            Context.Invader.LandHP = 0;

            ActionPanelBattleStartPhase.InitIntroAnimation(Context);

            sndButtonClick.Play();
        }

        private void LandModifierPhaseSelection()
        {
            IntroPhaseSelection();

            PhasesChoice = PhasesChoices.LandModifierPhase;
            PhasesEnd = PhasesChoices.LandModifierPhase;

            Context.Defender.LandHP = int.Parse(DefenderTerrainHPBonusInput.Text);
            Context.Defender.BonusST = int.Parse(DefenderSupportSTBonusInput.Text);

            Context.Invader.BonusST = int.Parse(InvaderSupportSTBonusInput.Text);

            sndButtonClick.Play();
        }

        private void CreatureModifierPhaseSelection()
        {
            LandModifierPhaseSelection();

            PhasesEnd = PhasesChoices.CreatureModifierPhase;

            sndButtonClick.Play();
        }

        private void EnchantModifierPhaseSelection()
        {
            CreatureModifierPhaseSelection();

            PhasesEnd = PhasesChoices.EnchantModifierPhase;

            sndButtonClick.Play();
        }

        private void ItemModifierPhaseSelection()
        {
            EnchantModifierPhaseSelection();

            PhasesEnd = PhasesChoices.ItemModifierPhase;
            sndButtonClick.Play();
        }

        private void BoostModifierPhaseSelection()
        {
            ItemModifierPhaseSelection();

            PhasesEnd = PhasesChoices.BoostModifierPhase;

            sndButtonClick.Play();
        }

        private void AttackPhaseSelection()
        {
            BoostModifierPhaseSelection();
            ListAttackAnimation.Clear();
            Context.ListActivatedEffect.Clear();

            PhasesEnd = PhasesChoices.InvaderAttackPhase2;

            sndButtonClick.Play();
        }

        private void CounterPhaseSelection()
        {
            BoostModifierPhaseSelection();

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

            switch (PhasesChoice)
            {
                case PhasesChoices.InvaderAttackPhase1:
                    ListAttackAnimation[0].BeginDraw(g);
                    break;
            }

            g.End();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1, 0);

            switch (PhasesChoice)
            {
                case PhasesChoices.Idle:
                    ActionPanelBattle.DrawInvaderBattle(fntArial12, Context, g);
                    ActionPanelBattle.DrawDefenderBattle(fntArial12, Context, g);
                    break;

                case PhasesChoices.IntroPhase:
                    ActionPanelBattleStartPhase.DrawAnimation(g, Context, fntArial12, sprVS);
                    break;

                case PhasesChoices.CreatureModifierPhase:
                case PhasesChoices.ItemModifierPhase:
                case PhasesChoices.BeforeBattle:
                case PhasesChoices.InvaderAttackBonusAnimation:
                case PhasesChoices.BattleStartAnimation:
                case PhasesChoices.PrepareAttackPhase:
                    ActionPanelBattle.DrawInvaderBattle(fntArial12, Context, g);
                    ActionPanelBattle.DrawDefenderBattle(fntArial12, Context, g);
                    ActionPanelBattleItemModifierPhase.DrawItemActivation(g, fntArial12, Context);
                    break;

                case PhasesChoices.InvaderAttackPhase1:
                case PhasesChoices.InvaderAttackPhase2:
                case PhasesChoices.CounterPhase1:
                case PhasesChoices.CounterPhase2:
                    ActionPanelBattle.DrawInvaderBattle(fntArial12, Context, g);
                    ActionPanelBattle.DrawDefenderBattle(fntArial12, Context, g);
                    break;

                default:
                    ActionPanelBattle.DrawInvaderBattle(fntArial12, Context, g);
                    ActionPanelBattle.DrawDefenderBattle(fntArial12, Context, g);
                    break;
            }

            DrawSetupMenu(g);
            DrawPhaseSelectionMenu(g);

            foreach (IUIElement ActiveElement in ArrayMenuButton)
            {
                ActiveElement.Draw(g);
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
            int MenuHeight = fntArial12.LineSpacing * PhaseMenuItems;

            int X = (Constants.Width - PhasesMenuWidth) / 2;
            int Y = (Constants.Height - MenuHeight - HeaderHeight) / 2;

            DrawBox(g, new Vector2(X, Y), PhasesMenuWidth, HeaderHeight, Color.Black);
            g.DrawStringCentered(fntArial12, "Phases", new Vector2(X + PhasesMenuWidth / 2, Y + HeaderHeight / 2), Color.White);

            Y += HeaderHeight;
            DrawBox(g, new Vector2(X, Y), PhasesMenuWidth, MenuHeight, Color.White);
        }
    }
}
