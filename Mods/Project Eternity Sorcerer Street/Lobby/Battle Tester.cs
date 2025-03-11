﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FMOD;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
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
            Nothing, DefenderCreature, DefenderItem, DefenderEnchant, DefenderPlayerEnchant, DefenderMapCreatures,
            InvaderCreature, InvaderItem, InvaderEnchant, InvaderPlayerEnchant, InvaderMapCreatures,
        }

        private enum PhasesChoices
        {
            Idle, IntroPhase, LandModifierPhase, CreatureModifierPhase, EnchantModifierPhase, ItemModifierPhase, BoostModifierPhase, BeforeBattle,
            BattleStartAnimation, PrepareAttackPhase, InvaderAttackBonusAnimation, InvaderAttackPhase1, InvaderAttackPhase2, CounterAttackBonusAnimation, CounterPhase1, CounterPhase2,
            InvaderBattleEnd, DefenderBattleEnd, UponVictory, UponDefeat, ResultPhase,
        }

        #region Ressources

        private CardSymbols Symbols;

        private SpriteFont fntMenuText;
        private Texture2D sprVS;

        private FMODSound sndButtonOver;
        protected FMODSound sndButtonClick;

        #region Setup UI

        private BoxButton DefenderCreatureButton;
        private BoxButton DefenderItemButton;
        private BoxButton DefenderEnchantButton;
        private BoxButton DefenderPlayerEnchantButton;
        private TextInput DefenderPlayerCardsInHandInput;
        private TextInput DefenderPlayerCardsInDeckInput;
        private TextInput DefenderHPInput;
        private TextInput DefenderMaxHPInput;
        private TextInput DefenderSTInput;
        private TextInput DefenderTerrainHPBonusInput;
        private TextInput DefenderSupportSTBonusInput;
        private TextInput DefenderAirLandsInput;
        private TextInput DefenderEarthLandsInput;
        private TextInput DefenderFireLandsInput;
        private TextInput DefenderWaterLandsInput;
        private TextInput DefenderRankInput;
        private TextInput DefenderGoldInput;
        private BoxButton DefenderMapCreaturesButton;
        private TextInput DefenderLapInput;
        private TextInput DefenderRoundInput;
        private DropDownButton DefenderTerrainType;

        private BoxButton InvaderCreatureButton;
        private BoxButton InvaderItemButton;
        private BoxButton InvaderEnchantButton;
        private BoxButton InvaderPlayerEnchantButton;
        private TextInput InvaderPlayerCardsInHandInput;
        private TextInput InvaderPlayerCardsInDeckInput;
        private TextInput InvaderHPInput;
        private TextInput InvaderMaxHPInput;
        private TextInput InvaderSTInput;
        private TextInput InvaderSupportSTBonusInput;
        private TextInput InvaderAirLandsInput;
        private TextInput InvaderEarthLandsInput;
        private TextInput InvaderFireLandsInput;
        private TextInput InvaderWaterLandsInput;
        private TextInput InvaderRankInput;
        private TextInput InvaderGoldInput;
        private TextInput InvaderLapInput;
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
        int SetupMenuItemsDefender = 21;
        int SetupMenuItemsInvader = 18;
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
            AllCardsBook = CardBook.LoadGlobalBook();

            ListAttackAnimation = new List<SimpleAnimation>();
            DicCreatureCountByElementType = new Dictionary<CreatureCard.ElementalAffinity, byte>();
            DicCreatureCountByElementType.Add(CreatureCard.ElementalAffinity.Air, 2);
            DicCreatureCountByElementType.Add(CreatureCard.ElementalAffinity.Earth, 2);
            DicCreatureCountByElementType.Add(CreatureCard.ElementalAffinity.Fire, 2);
            DicCreatureCountByElementType.Add(CreatureCard.ElementalAffinity.Water, 2);
            DicCreatureCountByElementType.Add(CreatureCard.ElementalAffinity.Neutral, 2);
        }

        public override void Load()
        {
            fntMenuText = Content.Load<SpriteFont>("Fonts/Arial12");
            sprVS = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/VS");
            Context = SorcererStreetBattleParams.DicParams[string.Empty].GlobalContext;
            Context.ActiveParser = SorcererStreetBattleParams.DicParams[string.Empty].ActiveParser = new SorcererStreetFormulaParser(SorcererStreetBattleParams.DicParams[string.Empty]);
            Context.ListSummonedCreature = new List<TerrainSorcererStreet>();
            Context.DicCreatureCountByElementType = DicCreatureCountByElementType;
            Context.DefenderTerrain = new TerrainSorcererStreet(0, 0, 0, 0, 0, 0, 0);
            Context.DefenderTerrain.LandLevel = 1;
            Context.DefenderTerrain.TerrainTypeIndex = 2;
            Context.EffectActivationPhase = SorcererStreetBattleContext.EffectActivationPhases.Battle;

            ButtonHeight = fntMenuText.LineSpacing + 2;

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            int MenuHeight = ButtonHeight * SetupMenuItemsInvader;

            int X = SetupMenuWidth;
            int Y = (Constants.Height - MenuHeight - HeaderHeight) / 2 + HeaderHeight;

            #region Setup UI

            InvaderCreatureButton = new BoxButton(new Rectangle(X, Y, ButtonsWidth, ButtonHeight), fntMenuText, "Select", OnButtonOver, InvaderCreatureSelection);
            InvaderItemButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntMenuText, "Select", OnButtonOver, InvaderItemSelection);
            InvaderEnchantButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntMenuText, "Select", OnButtonOver, InvaderEnchantSelection);
            InvaderPlayerEnchantButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntMenuText, "Select", OnButtonOver, InvaderPlayerEnchantSelection);

            InvaderPlayerCardsInHandInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderPlayerCardsInHand, true);
            InvaderPlayerCardsInHandInput.SetText("1");
            InvaderPlayerCardsInDeckInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderPlayerCardsInDeck, true);
            InvaderPlayerCardsInDeckInput.SetText("1");
            InvaderHPInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderHPInput, true);
            InvaderHPInput.SetText("30");
            InvaderMaxHPInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderMaxHPInput, true);
            InvaderMaxHPInput.SetText("30");
            InvaderSTInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderSTInput, true);
            InvaderSTInput.SetText("30");
            InvaderSupportSTBonusInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderSupportSTBonusInput, true);
            InvaderSupportSTBonusInput.SetText("1");
            InvaderAirLandsInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderAirLandsInput, true);
            InvaderAirLandsInput.SetText("1");
            InvaderEarthLandsInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderEarthLandsInput, true);
            InvaderEarthLandsInput.SetText("1");
            InvaderFireLandsInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderFireLandsInput, true);
            InvaderFireLandsInput.SetText("1");
            InvaderWaterLandsInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderWaterLandsInput, true);
            InvaderWaterLandsInput.SetText("1");
            InvaderRankInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderRankInput, true);
            InvaderRankInput.SetText("1");
            InvaderGoldInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderGoldInput, true);
            InvaderGoldInput.SetText("100");
            InvaderLapInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetInvaderLapInput, true);
            InvaderLapInput.SetText("0");

            InvaderMapCreaturesButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntMenuText, "Select", OnButtonOver, InvaderMapCreaturesSelection);

            MenuHeight = ButtonHeight * SetupMenuItemsDefender;
            X = Constants.Width - ButtonsWidth;
            Y = (Constants.Height - MenuHeight - HeaderHeight) / 2 + HeaderHeight;

            DefenderCreatureButton = new BoxButton(new Rectangle(X, Y, ButtonsWidth, fntMenuText.LineSpacing), fntMenuText, "Select", OnButtonOver, DefenderCreatureSelection);
            DefenderItemButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntMenuText, "Select", OnButtonOver, DefenderItemSelection);
            DefenderEnchantButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntMenuText, "Select", OnButtonOver, DefenderEnchantSelection);
            DefenderPlayerEnchantButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntMenuText, "Select", OnButtonOver, DefenderPlayerEnchantSelection);

            DefenderPlayerCardsInHandInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderPlayerCardsInHand, true);
            DefenderPlayerCardsInHandInput.SetText("1");
            DefenderPlayerCardsInDeckInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderPlayerCardsInDeck, true);
            DefenderPlayerCardsInDeckInput.SetText("1");
            DefenderHPInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderHPInput, true);
            DefenderHPInput.SetText("30");
            DefenderMaxHPInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderMaxHPInput, true);
            DefenderMaxHPInput.SetText("30");
            DefenderSTInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderSTInput, true);
            DefenderSTInput.SetText("30");
            DefenderTerrainHPBonusInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderTerrainHPBonusInput, true);
            DefenderTerrainHPBonusInput.SetText("1");
            DefenderSupportSTBonusInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderSupportSTBonusInput, true);
            DefenderSupportSTBonusInput.SetText("1");
            DefenderAirLandsInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderAirLandsInput, true);
            DefenderAirLandsInput.SetText("1");
            DefenderEarthLandsInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderEarthLandsInput, true);
            DefenderEarthLandsInput.SetText("1");
            DefenderFireLandsInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderFireLandsInput, true);
            DefenderFireLandsInput.SetText("1");
            DefenderWaterLandsInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderWaterLandsInput, true);
            DefenderWaterLandsInput.SetText("1");
            DefenderRankInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderRankInput, true);
            DefenderRankInput.SetText("2");
            DefenderGoldInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderGoldInput, true);
            DefenderGoldInput.SetText("100");

            DefenderMapCreaturesButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, ButtonsWidth, ButtonHeight), fntMenuText, "Select", OnButtonOver, DefenderMapCreaturesSelection);

            DefenderLapInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetDefenderLapInput, true);
            DefenderLapInput.SetText("0");
            DefenderRoundInput = new TextInput(fntMenuText, sprPixel, sprPixel, new Vector2(X + 10, Y += ButtonHeight), new Vector2(ButtonsWidth, ButtonHeight), SetRoundInput, true);
            DefenderRoundInput.SetText("1");

            DefenderTerrainType = new DropDownButton(new Rectangle(X, Y += ButtonHeight, 95, 30), fntMenuText, TerrainSorcererStreet.FireElement,
                new string[] { TerrainSorcererStreet.FireElement, TerrainSorcererStreet.WaterElement, TerrainSorcererStreet.EarthElement, TerrainSorcererStreet.AirElement, TerrainSorcererStreet.NeutralElement }, OnButtonOver, (SelectedItem) => { SetDefenderTerrainTerrainTypeIndex(SelectedItem); });

            #endregion

            #region Phases UI

            MenuHeight = fntMenuText.LineSpacing * PhaseMenuItems;
            X = (Constants.Width - PhasesMenuWidth) / 2;
            Y = (Constants.Height - MenuHeight - HeaderHeight) / 2 + HeaderHeight;

            IntroPhaseButton = new BoxButton(new Rectangle(X, Y, PhasesMenuWidth, fntMenuText.LineSpacing), fntMenuText, "Intro Modifier", OnButtonOver, IntroPhaseSelection);
            LandModifierPhaseButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, PhasesMenuWidth, fntMenuText.LineSpacing), fntMenuText, "Land Modifier", OnButtonOver, LandModifierPhaseSelection);
            CreatureModifierPhaseButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, PhasesMenuWidth, fntMenuText.LineSpacing), fntMenuText, "Creature Modifier", OnButtonOver, CreatureModifierPhaseSelection);
            EnchantModifierPhaseButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, PhasesMenuWidth, fntMenuText.LineSpacing), fntMenuText, "Enchant Modifier", OnButtonOver, EnchantModifierPhaseSelection);
            ItemModifierPhaseButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, PhasesMenuWidth, fntMenuText.LineSpacing), fntMenuText, "Item Modifier", OnButtonOver, ItemModifierPhaseSelection);
            BoostModifierPhaseButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, PhasesMenuWidth, fntMenuText.LineSpacing), fntMenuText, "Boost Modifier", OnButtonOver, BoostModifierPhaseSelection);
            AttackPhaseButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, PhasesMenuWidth, fntMenuText.LineSpacing), fntMenuText, "Attack", OnButtonOver, AttackPhaseSelection);
            CounterPhaseButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, PhasesMenuWidth, fntMenuText.LineSpacing), fntMenuText, "Counter", OnButtonOver, CounterPhaseSelection);
            ResultPhaseButton = new BoxButton(new Rectangle(X, Y += ButtonHeight, PhasesMenuWidth, fntMenuText.LineSpacing), fntMenuText, "Result", OnButtonOver, ResultPhaseSelection);

            #endregion

            ArrayMenuButton = new IUIElement[]
            {
                DefenderCreatureButton, DefenderItemButton, DefenderEnchantButton, DefenderPlayerEnchantButton,
                DefenderPlayerCardsInHandInput, DefenderPlayerCardsInDeckInput, DefenderHPInput, DefenderMaxHPInput, DefenderSTInput, DefenderTerrainHPBonusInput, DefenderSupportSTBonusInput,
                DefenderAirLandsInput, DefenderEarthLandsInput, DefenderFireLandsInput, DefenderWaterLandsInput, DefenderRankInput, DefenderGoldInput, DefenderMapCreaturesButton, DefenderLapInput, DefenderRoundInput, DefenderTerrainType,
                InvaderCreatureButton, InvaderItemButton, InvaderEnchantButton, InvaderPlayerEnchantButton,
                InvaderPlayerCardsInHandInput, InvaderPlayerCardsInDeckInput, InvaderHPInput, InvaderMaxHPInput, InvaderSTInput, InvaderSupportSTBonusInput,
                InvaderAirLandsInput, InvaderEarthLandsInput, InvaderFireLandsInput, InvaderWaterLandsInput, InvaderRankInput, InvaderGoldInput, InvaderLapInput, InvaderMapCreaturesButton,
                IntroPhaseButton, LandModifierPhaseButton, CreatureModifierPhaseButton, EnchantModifierPhaseButton, ItemModifierPhaseButton,
                BoostModifierPhaseButton, AttackPhaseButton, CounterPhaseButton, ResultPhaseButton,
            };

            AllCardsBook = CardBook.LoadGlobalBook();

            Card CopyCard = ActiveBook.DicCardsByType[CreatureCard.CreatureCardType].First().Value.Card;

            Context.Defender.Owner = new Player("Defender Player", "Defender Player", false);
            Context.Defender.OwnerTeam = new Team(2);
            Context.Defender.OwnerTeam.Rank = 2;
            Context.Defender.Owner.Gold = 100;
            Context.Defender.Owner.ListCardInHand.Add(new CreatureCard(""));
            Context.Defender.OwnerTeam.DicCreatureCountByElementType.Add((byte)CreatureCard.ElementalAffinity.Air, 1);
            Context.Defender.OwnerTeam.DicCreatureCountByElementType.Add((byte)CreatureCard.ElementalAffinity.Earth, 1);
            Context.Defender.OwnerTeam.DicCreatureCountByElementType.Add((byte)CreatureCard.ElementalAffinity.Fire, 1);
            Context.Defender.OwnerTeam.DicCreatureCountByElementType.Add((byte)CreatureCard.ElementalAffinity.Water, 1);
            Context.Defender.OwnerTeam.DicCreatureCountByElementType.Add((byte)CreatureCard.ElementalAffinity.Neutral, 1);
            Context.Defender.Creature = (CreatureCard)CopyCard.Copy(PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
            Context.Defender.Animation = new SimpleAnimation("Defender", "Defender", Context.Defender.Creature.sprCard);
            Context.Defender.Animation.Position = new Vector2(Constants.Width - Context.Defender.Creature.sprCard.Width - Constants.Width / 9, Constants.Height / 12);
            Context.Defender.Animation.Scale = new Vector2(1f);
            DefenderHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
            DefenderMaxHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
            DefenderSTInput.SetText(Context.Defender.Creature.OriginalST.ToString());

            Context.Invader.Owner = new Player("Invader Player", "Invader Player", false);
            Context.Invader.OwnerTeam = new Team(1);
            Context.Invader.OwnerTeam.Rank = 1;
            Context.Invader.Owner.Gold = 100;
            Context.Invader.Owner.ListCardInHand.Add(new CreatureCard(""));
            Context.Invader.OwnerTeam.DicCreatureCountByElementType.Add((byte)CreatureCard.ElementalAffinity.Air, 1);
            Context.Invader.OwnerTeam.DicCreatureCountByElementType.Add((byte)CreatureCard.ElementalAffinity.Earth, 1);
            Context.Invader.OwnerTeam.DicCreatureCountByElementType.Add((byte)CreatureCard.ElementalAffinity.Fire, 1);
            Context.Invader.OwnerTeam.DicCreatureCountByElementType.Add((byte)CreatureCard.ElementalAffinity.Water, 1);
            Context.Invader.OwnerTeam.DicCreatureCountByElementType.Add((byte)CreatureCard.ElementalAffinity.Neutral, 1);
            Context.Invader.Creature = (CreatureCard)CopyCard.Copy(PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
            Context.Invader.Animation = new SimpleAnimation("Invader", "Invader", Context.Invader.Creature.sprCard);
            Context.Invader.Animation.Position = new Vector2(Constants.Width / 9, Constants.Height / 12);
            Context.Invader.Animation.Scale = new Vector2(1f);
            InvaderHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
            InvaderMaxHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
            InvaderSTInput.SetText(Context.Defender.Creature.OriginalST.ToString());

            Context.TerrainRestrictions = new UnitAndTerrainValues();

            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType("Not Assigned"));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType(TerrainSorcererStreet.Castle));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType(TerrainSorcererStreet.FireElement));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType(TerrainSorcererStreet.WaterElement));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType(TerrainSorcererStreet.EarthElement));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType(TerrainSorcererStreet.AirElement));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType(TerrainSorcererStreet.NeutralElement));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType(TerrainSorcererStreet.MorphElement));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType(TerrainSorcererStreet.MultiElement));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType(TerrainSorcererStreet.EastTower));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType(TerrainSorcererStreet.WestTower));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType(TerrainSorcererStreet.SouthTower));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType(TerrainSorcererStreet.NorthTower));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType("Warp"));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType("Bridge"));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType("Fortune Teller"));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType("Spell Circle"));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType("Path Switch"));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType("Card Shop"));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType("Magic Trap"));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType("Siege Tower"));
            Context.TerrainRestrictions.ListTerrainType.Add(new TerrainType("Gem Store"));

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
                            ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, ActionPanelBattleAttackPhase.BeforeBattleStartRequirement);
                            PhasesChoice = PhasesChoices.BeforeBattle;
                        }
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

                        ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, ActionPanelBattleCreatureModifierPhase.RequirementName);
                        PhasesChoice = PhasesChoices.CreatureModifierPhase;
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
                            ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, ActionPanelBattleEnchantModifierPhase.RequirementName);
                            PhasesChoice = PhasesChoices.EnchantModifierPhase;
                        }
                    }
                    break;

                case PhasesChoices.EnchantModifierPhase:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        if (PhasesEnd == PhasesChoices.EnchantModifierPhase)
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
                            //TODO: Loop through boost creatures
                            PhasesChoice = PhasesChoices.BoostModifierPhase;
                        }
                    }
                    break;

                case PhasesChoices.BoostModifierPhase:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        Context.ListActivatedEffect.Clear();

                        if (!Context.Invader.Creature.UseCardAnimation)
                        {
                            ActionPanelBattleAttackAnimationPhase.InitAnimation(true, Context.Invader.Creature.IdleAnimationPath, Context.Invader, true);
                            Context.Invader.Animation.Position = new Vector2(1750, 190);
                        }

                        PhasesChoice = PhasesChoices.PrepareAttackPhase;
                    }
                    break;

                case PhasesChoices.PrepareAttackPhase:
                    if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Context))
                    {
                        if (Context.Invader.Creature.CurrentHP <= 0)
                        {
                            ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, false, ActionPanelBattleAttackPhase.UponVictoryRequirement);
                            PhasesChoice = PhasesChoices.UponVictory;
                            break;
                        }
                        else if (Context.Defender.Creature.CurrentHP <= 0)
                        {
                            ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, true, ActionPanelBattleAttackPhase.UponVictoryRequirement);
                            PhasesChoice = PhasesChoices.UponVictory;
                            break;
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

                    if (ListAttackAnimation[0].HasEnded)
                    {
                        AnimationScreen ActiveAnimation = (AnimationScreen)ListAttackAnimation[0].ActiveAnimation;
                        if (ActiveAnimation.Defender.FinalHP < ActiveAnimation.Defender.Creature.CurrentHP)
                        {
                            ActiveAnimation.Defender.Creature.CurrentHP = ActiveAnimation.Defender.FinalHP;
                        }

                        Context.Invader.Animation = new SimpleAnimation("Invader", "Invader", Context.Invader.Creature.sprCard);
                        Context.Invader.Animation.Position = new Vector2(Constants.Width / 9, Constants.Height / 12);
                        Context.Invader.Animation.Scale = new Vector2(1f);
                        Context.Defender.Animation = new SimpleAnimation("Defender", "Defender", Context.Defender.Creature.sprCard);
                        Context.Defender.Animation.Position = new Vector2(Constants.Width - Context.Defender.Creature.sprCard.Width - Constants.Width / 9, Constants.Height / 12);
                        Context.Defender.Animation.Scale = new Vector2(1f);

                        ListAttackAnimation.RemoveAt(0);

                        if (ListAttackAnimation.Count > 0)
                        {
                            ActiveAnimation.Defender.Animation = ListAttackAnimation[0];
                        }
                        else
                        {
                            SorcererStreetBattleContext.BattleCreatureInfo FirstAttacker;
                            SorcererStreetBattleContext.BattleCreatureInfo SecondAttacker;
                            ActionPanelBattleAttackPhase.DetermineAttackOrder(Context, out FirstAttacker, out SecondAttacker);

                            if (ActiveAnimation.Defender.FinalHP > 0)
                            {
                                if (PhasesChoice == PhasesChoices.InvaderAttackPhase1)
                                {
                                    if (FirstAttacker.Creature.GetCurrentAbilities(Context.EffectActivationPhase).AttackTwice)
                                    {
                                        StartAttackAnimation(FirstAttacker, SecondAttacker);
                                        PhasesChoice = PhasesChoices.InvaderAttackPhase2;
                                        break;
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
                                    if (FirstAttacker.DamageReceived > 0)
                                    {
                                        ActionPanelBattleItemModifierPhase.StartAnimationIfAvailable(Context, SecondAttacker == Context.Invader, ActionPanelBattleAttackPhase.AttackBonusRequirement);
                                    }
                                    PhasesChoice = PhasesChoices.CounterAttackBonusAnimation;
                                }
                                else if (PhasesChoice == PhasesChoices.CounterPhase1)
                                {
                                    if (SecondAttacker.Creature.GetCurrentAbilities(Context.EffectActivationPhase).AttackTwice)
                                    {
                                        StartAttackAnimation(SecondAttacker, FirstAttacker);
                                        PhasesChoice = PhasesChoices.CounterPhase2;
                                        break;
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
                        Context.TotalCreaturesDestroyed++;
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

                        DefenderSTInput.SetText(Context.Defender.Creature.CurrentST.ToString());
                        DefenderMaxHPInput.SetText(Context.Defender.Creature.MaxHP.ToString());
                        DefenderGoldInput.SetText(Context.Defender.Owner.Gold.ToString());

                        InvaderSTInput.SetText(Context.Invader.Creature.CurrentST.ToString());
                        InvaderMaxHPInput.SetText(Context.Invader.Creature.MaxHP.ToString());
                        InvaderGoldInput.SetText(Context.Invader.Owner.Gold.ToString());
                        DefenderTerrainHPBonusInput.SetText((Context.DefenderTerrain.LandLevel).ToString());
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
                    CursorIndex = ActivePlayer.Inventory.DicOwnedBook.Count + 1;
                }
            }
            else if (InputHelper.InputDownPressed())
            {
                if (++CursorIndex > ActivePlayer.Inventory.DicOwnedBook.Count + 1)
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

            SpellCard ActiveSpellCard;

            switch (SetupChoice)
            {
                case SetupChoices.DefenderCreature:
                    Context.Defender.Creature = (CreatureCard)CardSelectionScreen.ListSelectedCard[0];
                    Context.Defender.Animation = new SimpleAnimation("Defender", "Defender", Context.Defender.Creature.sprCard);
                    Context.Defender.Animation.Position = new Vector2(Constants.Width - Context.Defender.Creature.sprCard.Width - Constants.Width / 9, Constants.Height / 12);
                    Context.Defender.Animation.Scale = new Vector2(1f);
                    DefenderHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
                    DefenderMaxHPInput.SetText(Context.Defender.Creature.OriginalMaxHP.ToString());
                    DefenderSTInput.SetText(Context.Defender.Creature.OriginalST.ToString());
                    break;

                case SetupChoices.InvaderCreature:
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

                case SetupChoices.DefenderEnchant:
                    ActiveSpellCard = (SpellCard)CardSelectionScreen.ListSelectedCard[0];
                    if (ActiveSpellCard.Spell.Target.TargetType == ManualSkillActivationSorcererStreet.PlayerTargetType)
                    {
                        ActiveSpellCard.GetAvailableActivation(null);
                    }
                    break;

                case SetupChoices.InvaderPlayerEnchant:
                    ActiveSpellCard = (SpellCard)CardSelectionScreen.ListSelectedCard[0];
                    if (ActiveSpellCard.Spell.Target.TargetType == ManualSkillActivationSorcererStreet.PlayerTargetType)
                    {
                        PushScreen(new PlayerSelectionScreen(Context, Context.Invader, Context.Defender, ActiveSpellCard));
                    }
                    break;

                case SetupChoices.DefenderPlayerEnchant:
                    ActiveSpellCard = (SpellCard)CardSelectionScreen.ListSelectedCard[0];
                    if (ActiveSpellCard.Spell.Target.TargetType == ManualSkillActivationSorcererStreet.PlayerTargetType)
                    {
                        PushScreen(new PlayerSelectionScreen(Context, Context.Defender, Context.Invader, ActiveSpellCard));
                    }
                    break;
            }
            SetupChoice = SetupChoices.Nothing;
            CardSelectionScreen = null;
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        private void StartAttackAnimation(SorcererStreetBattleContext.BattleCreatureInfo FirstAttacker, SorcererStreetBattleContext.BattleCreatureInfo Defender)
        {
            int ReflectedDamage = Defender.DamageReflectedByOpponent;
            int Damage = Defender.DamageReceived;

            if (ReflectedDamage > 0)
            {
                ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(Defender != Context.Defender, "Sorcerer Street/Default", FirstAttacker, true));
            }

            if (Damage > 0)
            {
                if (FirstAttacker.Creature.UseCardAnimation)
                {
                    foreach (string ActiveAnimationPath in FirstAttacker.GetAttackAnimationPaths())
                    {
                        if (string.IsNullOrEmpty(ActiveAnimationPath))
                        {
                            ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(Defender == Context.Defender, "Sorcerer Street/Default", Defender, true));
                        }
                        else
                        {
                            ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(Defender == Context.Defender, ActiveAnimationPath, Defender, true));
                        }
                    }
                }
                else
                {
                    ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(Defender == Context.Defender, FirstAttacker.Creature.AttackStartAnimationPath, FirstAttacker, true));
                    FirstAttacker.Animation.Position = new Vector2(1750, 190);
                    ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(Defender == Context.Defender, FirstAttacker.Creature.AttackEndAnimationPath, Defender, true));
                    Defender.Animation.Position = new Vector2(573, 90);

                    FirstAttacker.Animation = null;
                    Defender.Animation = null;
                    Defender.Animation = new SimpleAnimation("Defender", "Defender", Context.Defender.Creature.sprCard);
                    Defender.Animation.Position = new Vector2(Constants.Width - Context.Defender.Creature.sprCard.Width - Constants.Width / 9, Constants.Height / 12);
                    Defender.Animation.Scale = new Vector2(1f);
                }
            }
            else
            {
                ListAttackAnimation.Add(ActionPanelBattleAttackAnimationPhase.InitAnimation(Defender == Context.Defender, "Sorcerer Street/Neutralize", Defender, true));
            }

            AnimationScreen ActiveAnimation = (AnimationScreen)ListAttackAnimation[0].ActiveAnimation;
            ActiveAnimation.Defender.Animation = ListAttackAnimation[0];
        }

        #region Setup UI

        private void DefenderCreatureSelection()
        {
            SetupChoice = SetupChoices.DefenderCreature;
            PhasesChoice = PhasesChoices.Idle;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Creatures, Context.Defender.Creature, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void DefenderItemSelection()
        {
            SetupChoice = SetupChoices.DefenderItem;
            PhasesChoice = PhasesChoices.Idle;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Item, Context.Defender.Item, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void DefenderEnchantSelection()
        {
            SetupChoice = SetupChoices.DefenderEnchant;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.EnchantCreature, null, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void DefenderPlayerEnchantSelection()
        {
            SetupChoice = SetupChoices.DefenderPlayerEnchant;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.EnchantPlayer, null, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void DefenderMapCreaturesSelection()
        {
            SetupChoice = SetupChoices.DefenderMapCreatures;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Creatures, null, true);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void SetDefenderPlayerCardsInHand(TextInput Sender, string InputValue)
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

        private void SetDefenderPlayerCardsInDeck(TextInput Sender, string InputValue)
        {
            int CardsInHand;
            int.TryParse(InputValue, out CardsInHand);
            while (Context.Defender.Owner.ListCardInDeck.Count < CardsInHand)
            {
                Context.Defender.Owner.ListCardInDeck.Add(new CreatureCard("Dummy"));
            }
            while (Context.Defender.Owner.ListCardInDeck.Count > CardsInHand)
            {
                Context.Defender.Owner.ListCardInDeck.RemoveAt(0);
            }
        }

        private void SetDefenderHPInput(TextInput Sender, string InputValue)
        {
            int.TryParse(InputValue, out Context.Defender.Creature.CurrentHP);
        }

        private void SetDefenderMaxHPInput(TextInput Sender, string InputValue)
        {
            int.TryParse(InputValue, out Context.Defender.Creature.MaxHP);
        }

        private void SetDefenderSTInput(TextInput Sender, string InputValue)
        {
            int.TryParse(InputValue, out Context.Defender.Creature.CurrentST);
        }

        private void SetDefenderTerrainHPBonusInput(TextInput Sender, string InputValue)
        {
            int.TryParse(DefenderTerrainHPBonusInput.Text, out Context.DefenderTerrain.LandLevel);
        }

        private void SetDefenderSupportSTBonusInput(TextInput Sender, string InputValue)
        {
        }

        private void SetDefenderAirLandsInput(TextInput Sender, string InputValue)
        {
            byte FinalValue;
            byte.TryParse(InputValue, out FinalValue);
            Context.Defender.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Air] = FinalValue;
            DicCreatureCountByElementType[CreatureCard.ElementalAffinity.Air] = (byte)(Context.Invader.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Air] + Context.Defender.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Air]);
        }

        private void SetDefenderEarthLandsInput(TextInput Sender, string InputValue)
        {
            byte FinalValue;
            byte.TryParse(InputValue, out FinalValue);
            Context.Defender.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Earth] = FinalValue;
            DicCreatureCountByElementType[CreatureCard.ElementalAffinity.Earth] = (byte)(Context.Invader.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Earth] + Context.Defender.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Earth]);
        }

        private void SetDefenderFireLandsInput(TextInput Sender, string InputValue)
        {
            byte FinalValue;
            byte.TryParse(InputValue, out FinalValue);
            Context.Defender.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Fire] = FinalValue;
            DicCreatureCountByElementType[CreatureCard.ElementalAffinity.Fire] = (byte)(Context.Invader.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Fire] + Context.Defender.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Fire]);
        }

        private void SetDefenderWaterLandsInput(TextInput Sender, string InputValue)
        {
            byte FinalValue;
            byte.TryParse(InputValue, out FinalValue);
            Context.Defender.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Water] = FinalValue;
            DicCreatureCountByElementType[CreatureCard.ElementalAffinity.Water] = (byte)(Context.Invader.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Water] + Context.Defender.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Water]);
        }

        private void SetDefenderRankInput(TextInput Sender, string InputValue)
        {
            int.TryParse(InputValue, out Context.Defender.OwnerTeam.Rank);
        }

        private void SetDefenderGoldInput(TextInput Sender, string InputValue)
        {
            int.TryParse(InputValue, out Context.Defender.Owner.Gold);
        }

        private void SetDefenderLapInput(TextInput Sender, string InputValue)
        {
            int.TryParse(InputValue, out Context.Defender.Owner.CompletedLaps);
        }
        
        private void SetRoundInput(TextInput Sender, string InputValue)
        {
            int.TryParse(InputValue, out Context.CurrentTurn);
        }

        private void SetDefenderTerrainTerrainTypeIndex(string SelectedItem)
        {
            switch (SelectedItem)
            {
                case TerrainSorcererStreet.FireElement:
                    Context.DefenderTerrain.TerrainTypeIndex = 2;
                    break;
                case TerrainSorcererStreet.WaterElement:
                    Context.DefenderTerrain.TerrainTypeIndex = 3;
                    break;
                case TerrainSorcererStreet.EarthElement:
                    Context.DefenderTerrain.TerrainTypeIndex = 4;
                    break;
                case TerrainSorcererStreet.AirElement:
                    Context.DefenderTerrain.TerrainTypeIndex = 5;
                    break;
                case TerrainSorcererStreet.NeutralElement:
                    Context.DefenderTerrain.TerrainTypeIndex = 6;
                    break;
            }
        }

        #region Invader

        private void InvaderCreatureSelection()
        {
            SetupChoice = SetupChoices.InvaderCreature;
            PhasesChoice = PhasesChoices.Idle;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Creatures, Context.Invader.Creature, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void InvaderItemSelection()
        {
            SetupChoice = SetupChoices.InvaderItem;
            PhasesChoice = PhasesChoices.Idle;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Item, Context.Invader.Item, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void InvaderEnchantSelection()
        {
            SetupChoice = SetupChoices.InvaderEnchant;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.EnchantCreature, null, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void InvaderPlayerEnchantSelection()
        {
            SetupChoice = SetupChoices.InvaderPlayerEnchant;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.EnchantPlayer, null, false);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void InvaderMapCreaturesSelection()
        {
            SetupChoice = SetupChoices.InvaderMapCreatures;
            CardSelectionScreen = new EditBookCardListFilterScreen(ActiveBook, EditBookCardListFilterScreen.Filters.Creatures, null, true);
            PushScreen(CardSelectionScreen);
            sndButtonClick.Play();
        }

        private void SetInvaderPlayerCardsInHand(TextInput Sender, string InputValue)
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

        private void SetInvaderPlayerCardsInDeck(TextInput Sender, string InputValue)
        {
            int CardsInHand;
            int.TryParse(InputValue, out CardsInHand);
            while (Context.Invader.Owner.ListCardInDeck.Count < CardsInHand)
            {
                Context.Invader.Owner.ListCardInDeck.Add(new CreatureCard("Dummy"));
            }
            while (Context.Invader.Owner.ListCardInDeck.Count > CardsInHand)
            {
                Context.Invader.Owner.ListCardInDeck.RemoveAt(0);
            }
        }

        private void SetInvaderHPInput(TextInput Sender, string InputValue)
        {
            int.TryParse(InputValue, out Context.Invader.Creature.CurrentHP);
        }

        private void SetInvaderMaxHPInput(TextInput Sender, string InputValue)
        {
            int.TryParse(InputValue, out Context.Invader.Creature.MaxHP);
        }

        private void SetInvaderSTInput(TextInput Sender, string InputValue)
        {
            int.TryParse(InputValue, out Context.Invader.Creature.CurrentST);
        }

        private void SetInvaderSupportSTBonusInput(TextInput Sender, string InputValue)
        {
        }

        private void SetInvaderAirLandsInput(TextInput Sender, string InputValue)
        {
            byte FinalValue;
            byte.TryParse(InputValue, out FinalValue);
            Context.Invader.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Air] = FinalValue;
            DicCreatureCountByElementType[CreatureCard.ElementalAffinity.Air] = (byte)(Context.Invader.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Air] + Context.Defender.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Air]);
        }

        private void SetInvaderEarthLandsInput(TextInput Sender, string InputValue)
        {
            byte FinalValue;
            byte.TryParse(InputValue, out FinalValue);
            Context.Invader.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Earth] = FinalValue;
            DicCreatureCountByElementType[CreatureCard.ElementalAffinity.Earth] = (byte)(Context.Invader.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Earth] + Context.Defender.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Earth]);
        }

        private void SetInvaderFireLandsInput(TextInput Sender, string InputValue)
        {
            byte FinalValue;
            byte.TryParse(InputValue, out FinalValue);
            Context.Invader.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Fire] = FinalValue;
            DicCreatureCountByElementType[CreatureCard.ElementalAffinity.Fire] = (byte)(Context.Invader.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Fire] + Context.Defender.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Fire]);
        }

        private void SetInvaderWaterLandsInput(TextInput Sender, string InputValue)
        {
            byte FinalValue;
            byte.TryParse(InputValue, out FinalValue);
            Context.Invader.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Water] = FinalValue;
            DicCreatureCountByElementType[CreatureCard.ElementalAffinity.Water] = (byte)(Context.Invader.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Water] + Context.Defender.OwnerTeam.DicCreatureCountByElementType[(byte)CreatureCard.ElementalAffinity.Water]);
        }

        private void SetInvaderRankInput(TextInput Sender, string InputValue)
        {
            int.TryParse(InputValue, out Context.Invader.OwnerTeam.Rank);
        }

        private void SetInvaderGoldInput(TextInput Sender, string InputValue)
        {
            int.TryParse(InputValue, out Context.Invader.Owner.Gold);
        }

        private void SetInvaderLapInput(TextInput Sender, string InputValue)
        {
            int.TryParse(InputValue, out Context.Invader.Owner.CompletedLaps);
        }

        #endregion

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
            Context.Defender.DamageNeutralizedByOpponent = 0;
            Context.Defender.DamageReceived = 0;
            Context.Defender.DamageReflectedByOpponent = 0;
            Context.Defender.DamageReceivedIgnoreLandBonus = false;

            Context.Invader.Creature.CurrentHP = int.Parse(InvaderHPInput.Text);
            Context.Invader.Creature.CurrentST = int.Parse(InvaderSTInput.Text);
            Context.Invader.BonusHP = 0;
            Context.Invader.BonusST = 0;
            Context.Invader.LandHP = 0;
            Context.Invader.DamageNeutralizedByOpponent = 0;
            Context.Invader.DamageReceived = 0;
            Context.Invader.DamageReflectedByOpponent = 0;
            Context.Invader.DamageReceivedIgnoreLandBonus = false;

            ActionPanelBattleStartPhase.InitIntroAnimation(Context);

            sndButtonClick.Play();
        }

        private void LandModifierPhaseSelection()
        {
            IntroPhaseSelection();

            PhasesChoice = PhasesChoices.LandModifierPhase;
            PhasesEnd = PhasesChoices.LandModifierPhase;

            Context.Defender.LandHP = int.Parse(DefenderTerrainHPBonusInput.Text) * 10;
            Context.Defender.BonusST = int.Parse(DefenderSupportSTBonusInput.Text) * 10;

            Context.Invader.BonusST = int.Parse(InvaderSupportSTBonusInput.Text) * 10;

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
                    ActionPanelBattle.DrawInvaderBattle(fntMenuText, Context, g);
                    ActionPanelBattle.DrawDefenderBattle(fntMenuText, Context, g);
                    break;

                case PhasesChoices.IntroPhase:
                    ActionPanelBattleStartPhase.DrawAnimation(g, Context, fntMenuText, sprVS);
                    break;

                case PhasesChoices.CreatureModifierPhase:
                case PhasesChoices.EnchantModifierPhase:
                case PhasesChoices.ItemModifierPhase:
                case PhasesChoices.BeforeBattle:
                case PhasesChoices.InvaderAttackBonusAnimation:
                case PhasesChoices.CounterAttackBonusAnimation:
                case PhasesChoices.BattleStartAnimation:
                case PhasesChoices.PrepareAttackPhase:
                case PhasesChoices.InvaderBattleEnd:
                case PhasesChoices.UponDefeat:
                case PhasesChoices.UponVictory:
                    ActionPanelBattle.DrawInvaderBattle(fntMenuText, Context, g);
                    ActionPanelBattle.DrawDefenderBattle(fntMenuText, Context, g);
                    ActionPanelBattleItemModifierPhase.DrawItemActivation(g, fntMenuText, Context);
                    break;

                case PhasesChoices.InvaderAttackPhase1:
                case PhasesChoices.InvaderAttackPhase2:
                case PhasesChoices.CounterPhase1:
                case PhasesChoices.CounterPhase2:
                    ActionPanelBattle.DrawInvaderBattle(fntMenuText, Context, g);
                    ActionPanelBattle.DrawDefenderBattle(fntMenuText, Context, g);
                    break;

                default:
                    ActionPanelBattle.DrawInvaderBattle(fntMenuText, Context, g);
                    ActionPanelBattle.DrawDefenderBattle(fntMenuText, Context, g);
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
            g.DrawStringCentered(fntMenuText, "Setup", new Vector2(X + SetupMenuWidth / 2, Y + HeaderHeight / 2), Color.White);

            Y += HeaderHeight;
            DrawBox(g, new Vector2(X, Y), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader Creature", new Vector2(X + 5, Y + 1), Color.White);

            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader Item", new Vector2(X + 5, Y + 1), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader Enchant", new Vector2(X + 5, Y + 1), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader Player Enchant", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader Player Cards in hand", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader Player Cards in deck", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader HP", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader Max HP", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader ST", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader Support ST Bonus", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader Air Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader Earth Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader Fire Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader Water Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader Rank", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader Gold", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader Lap", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Invader Map Creatures", new Vector2(X + 5, Y), Color.White);

            MenuHeight = ButtonHeight * SetupMenuItemsDefender;
            X = Constants.Width - SetupMenuWidth - ButtonsWidth;
            Y = (Constants.Height - MenuHeight - HeaderHeight) / 2;

            DrawBox(g, new Vector2(X, Y), SetupMenuWidth, HeaderHeight, Color.Black);
            g.DrawStringCentered(fntMenuText, "Setup", new Vector2(X + SetupMenuWidth / 2, Y + HeaderHeight / 2), Color.White);

            Y += HeaderHeight;
            DrawBox(g, new Vector2(X, Y), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Defender Creature", new Vector2(X + 5, Y + 1), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Defender Item", new Vector2(X + 5, Y + 1), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Defender Enchant", new Vector2(X + 5, Y + 1), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Defender Player Enchant", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Defender Player Cards in hand", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Defender Player Cards in deck", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Defender HP", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Defender Max HP", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Defender ST", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Terrain HP Bonus", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Defender Support ST Bonus", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Defender Air Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Defender Earth Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Defender Fire Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Defender Water Lands", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Defender Rank", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Defender Gold", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Defender Map Creatures", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Current Lap", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Current Round", new Vector2(X + 5, Y), Color.White);
            DrawBox(g, new Vector2(X + SetupMenuWidth, Y), ButtonsWidth, ButtonHeight, Color.White);
            DrawBox(g, new Vector2(X, Y += ButtonHeight), SetupMenuWidth, ButtonHeight, Color.White);
            g.DrawString(fntMenuText, "Terrain Type", new Vector2(X + 5, Y), Color.White);
        }

        public void DrawPhaseSelectionMenu(CustomSpriteBatch g)
        {
            int MenuHeight = fntMenuText.LineSpacing * PhaseMenuItems;

            int X = (Constants.Width - PhasesMenuWidth) / 2;
            int Y = (Constants.Height - MenuHeight - HeaderHeight) / 2;

            DrawBox(g, new Vector2(X, Y), PhasesMenuWidth, HeaderHeight, Color.Black);
            g.DrawStringCentered(fntMenuText, "Phases", new Vector2(X + PhasesMenuWidth / 2, Y + HeaderHeight / 2), Color.White);

            Y += HeaderHeight;
            DrawBox(g, new Vector2(X, Y), PhasesMenuWidth, MenuHeight, Color.White);
        }
    }
}
