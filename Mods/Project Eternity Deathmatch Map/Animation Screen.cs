using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.BattleMapScreen;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class AnimationScreen : AnimationClass
    {
        public enum BattleAnimationTypes { RightAttackLeft, RightConteredByLeft, LeftAttackRight, LeftConteredByRight }
        public class AnimationUnitStats
        {
            public int RightUnitHP;
            public int RightUnitHPMax;
            public int LeftUnitHP;
            public int LeftUnitHPMax;

            public int RightUnitEN;
            public int RightUnitENMax;
            public int LeftUnitEN;
            public int LeftUnitENMax;

            public AnimationUnitStats(Squad ActiveSquad, Squad EnemySquad, bool IsActiveSquadOnRight)
            {
                if (IsActiveSquadOnRight)
                {
                    RightUnitHP = ActiveSquad.CurrentLeader.HP;
                    RightUnitHPMax = ActiveSquad.CurrentLeader.MaxHP;
                    LeftUnitHP = EnemySquad.CurrentLeader.HP;
                    LeftUnitHPMax = EnemySquad.CurrentLeader.MaxHP;

                    RightUnitEN = ActiveSquad.CurrentLeader.EN;
                    RightUnitENMax = ActiveSquad.CurrentLeader.MaxEN;
                    LeftUnitEN = EnemySquad.CurrentLeader.EN;
                    LeftUnitENMax = ActiveSquad.CurrentLeader.MaxEN;
                }
                else
                {
                    RightUnitHP = EnemySquad.CurrentLeader.HP;
                    RightUnitHPMax = EnemySquad.CurrentLeader.MaxHP;
                    LeftUnitHP = ActiveSquad.CurrentLeader.HP;
                    LeftUnitHPMax = ActiveSquad.CurrentLeader.MaxHP;

                    RightUnitEN = EnemySquad.CurrentLeader.EN;
                    RightUnitENMax = EnemySquad.CurrentLeader.MaxEN;
                    LeftUnitEN = ActiveSquad.CurrentLeader.EN;
                    LeftUnitENMax = ActiveSquad.CurrentLeader.MaxEN;
                }
            }
        }

        public enum QuoteTypes { BattleStart, Dodge, Damaged, Destroyed, SupportAttack, SupportDefend };

        private DeathmatchMap Map;
        private AnimationUnitStats UnitStats;
        public Squad AttackingSquad;
        public Squad EnemySquad;
        private Squad RightSquad;
        private Squad LeftSquad;
        private Attack ActiveAttack;
        public BattleMap.SquadBattleResult BattleResult;
        private string ExtraText;
        public bool IsLeftAttacking;

        private Random Random;
        private SpriteFont fntFinlanderFont;
        private Texture2D sprBarExtraLargeBackground;
        private Texture2D sprBarExtraLargeEN;
        private Texture2D sprBarExtraLargeHP;
        private Texture2D sprInfinity;
        private bool IsLoaded;

        public AnimationScreen(string AnimationPath, DeathmatchMap Map, Squad AttackingSquad, Squad EnemySquad, Attack ActiveAttack,
            BattleMap.SquadBattleResult BattleResult, AnimationUnitStats UnitStats, AnimationBackground ActiveTerrain, string ExtraText, bool IsLeftAttacking)
            : base(AnimationPath)
        {
            RequireFocus = false;
            RequireDrawFocus = false;
            IsOnTop = false;
            Random = new Random();
            IsLoaded = false;

            this.Map = Map;
            this.AttackingSquad = AttackingSquad;
            this.EnemySquad = EnemySquad;
            this.ActiveAttack = ActiveAttack;
            this.BattleResult = BattleResult;
            this.UnitStats = UnitStats;
            ActiveAnimationBackground = ActiveTerrain;
            this.ExtraText = ExtraText;
            this.IsLeftAttacking = IsLeftAttacking;

            RightSquad = AttackingSquad;
            LeftSquad = EnemySquad;

            if (IsLeftAttacking)
            {
                RightSquad = EnemySquad;
                LeftSquad = AttackingSquad;
            }
        }

        public Tuple<string, string> GetQuote(QuoteTypes QuoteType, Character ActivePilot, Character EnemyPilot, bool UseRandomIndex, ref int QuoteIndex)
        {
            Character.QuoteSet Quote = null;
            switch (QuoteType)
            {
                case QuoteTypes.BattleStart:
                    Quote = ActivePilot.QuoteSetBattleStart;
                    break;

                case QuoteTypes.Dodge:
                    Quote = ActivePilot.QuoteSetDodge;
                    break;

                case QuoteTypes.Damaged:
                    Quote = ActivePilot.QuoteSetDamaged;
                    break;

                case QuoteTypes.Destroyed:
                    Quote = ActivePilot.QuoteSetDestroyed;
                    break;

                case QuoteTypes.SupportAttack:
                    Quote = ActivePilot.QuoteSetSupportAttack;
                    break;

                case QuoteTypes.SupportDefend:
                    Quote = ActivePilot.QuoteSetSupportDefend;
                    break;
            }

            List<string> ListQuote = null;
            bool IsVersus = false;

            if (ActivePilot.ListQuoteSetVersusName.Contains(EnemyPilot.Name))
                IsVersus = true;

            //Versus.
            if (IsVersus)
            {
                //Calculate if it should use a base quote or a Versus quote, favorising the versus ones.
                int RandomQuoteIndex = Random.Next(Quote.ListQuote.Count + (int)(Quote.ListQuoteVersus.Count * 2.5f));

                //Use one of the base quote.
                if (RandomQuoteIndex < Quote.ListQuote.Count)
                {
                    //Get a random index if needed.
                    if (UseRandomIndex)
                        QuoteIndex = Random.Next(ListQuote.Count);
                    else if (QuoteIndex >= ListQuote.Count)
                        return new Tuple<string, string>(Quote.PortraitPath, Quote.ListQuote[0]);

                    return new Tuple<string, string>(Quote.PortraitPath, Quote.ListQuote[QuoteIndex]);
                }
                //Use a versus quote.
                else if (Quote.ListQuoteVersus.Count > 0)
                {
                    //Get a random index if needed.
                    if (UseRandomIndex)
                        QuoteIndex = Random.Next(Quote.ListQuoteVersus.Count);
                    else if (QuoteIndex >= Quote.ListQuoteVersus.Count)
                        return new Tuple<string, string>(Quote.PortraitPath, Quote.ListQuoteVersus[0]);

                    return new Tuple<string, string>(Quote.PortraitPath, Quote.ListQuoteVersus[QuoteIndex]);
                }
            }
            else//Not versus.
            {
                if (IsVersus)
                    ListQuote = Quote.ListQuoteVersus;
                else
                    ListQuote = Quote.ListQuote;

                if (ListQuote.Count == 0)
                    return new Tuple<string, string>("", "");

                //Get a random index if needed.
                if (UseRandomIndex)
                    QuoteIndex = Random.Next(ListQuote.Count);
                else if (QuoteIndex >= ListQuote.Count)
                    return new Tuple<string, string>(Quote.PortraitPath, Quote.ListQuote[0]);

                return new Tuple<string, string>(Quote.PortraitPath, ListQuote[QuoteIndex]);
            }

            return new Tuple<string, string>("", "");
        }

        public Tuple<string, string> GetAttackQuote(string QuoteSet, Character ActivePilot, Character EnemyPilot, bool UseRandomIndex, ref int QuoteIndex)
        {
            Character.QuoteSet Quote = null;
            if (!ActivePilot.DicAttackQuoteSet.TryGetValue(QuoteSet, out Quote))
                return new Tuple<string, string>("", "");

            List<string> ListQuote = null;
            bool IsVersus = false;

            if (ActivePilot.ListQuoteSetVersusName.Contains(EnemyPilot.Name))
                IsVersus = true;

            //Versus.
            if (IsVersus)
                ListQuote = Quote.ListQuoteVersus;
            else//Not versus.
                ListQuote = Quote.ListQuote;

            if (ListQuote.Count == 0)
                return new Tuple<string, string>("", "");

            //Get a random index if needed.
            if (UseRandomIndex)
                QuoteIndex = Random.Next(ListQuote.Count);
            else if (QuoteIndex >= ListQuote.Count)
                return new Tuple<string, string>(Quote.PortraitPath, Quote.ListQuote[0]);

            return new Tuple<string, string>(Quote.PortraitPath, ListQuote[QuoteIndex]);
        }

        public override void Load()
        {
            if (IsLoaded)
                return;

            IsLoaded = true;

            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");
            sprBarExtraLargeBackground = Content.Load<Texture2D>("Battle/Bars/Extra Long Bar");
            sprBarExtraLargeEN = Content.Load<Texture2D>("Battle/Bars/Extra Long Energy");
            sprBarExtraLargeHP = Content.Load<Texture2D>("Battle/Bars/Extra Long Health");
            sprInfinity = Content.Load<Texture2D>("Battle/Infinity");

            foreach (KeyValuePair<string, Timeline> Timeline in LoadTimelines(typeof(CoreTimeline)))
            {
                if (Timeline.Value is AnimationOriginTimeline)
                    continue;

                DicTimeline.Add(Timeline.Key, Timeline.Value);
            }

            foreach (KeyValuePair<string, Timeline> Timeline in LoadTimelines(typeof(DeathmatchMapTimeline), this, Content))
            {
                DicTimeline.Add(Timeline.Key, Timeline.Value);
            }

            base.Load();

            InitRessources();

            Unit ActiveUnit = AttackingSquad.CurrentLeader;
            Unit EnemyUnit = EnemySquad.CurrentLeader;

            for (int L = ListAnimationLayer.Count - 1; L >= 0; --L)
            {
                InitMarkers(ListAnimationLayer[L], AttackingSquad, EnemySquad);

                #region Quotes

                foreach (QuoteSetTimeline.QuoteSetKeyFrame ActiveKeyFrame in ((QuoteSetTimeline)ListAnimationLayer.EngineLayer.DicTimelineEvent[0][1]).DicAnimationKeyFrame.Values)
                {
                    Quote ActiveQuote = ActiveKeyFrame.QuoteSet;
                    int QuoteSetIndex = 0;
                    int QuoteSetCount = 0;

                    //Once a Quote is selected, keep it, if random quotes are needed, use multiple QuoteSet.
                    //If the number of QuoteSet changed, assume the user did something wrong and get a new index.
                    if (ActiveQuote.ListQuoteSet.Count > 1 && QuoteSetCount != ActiveQuote.ListQuoteSet.Count)
                    {
                        QuoteSetCount = ActiveQuote.ListQuoteSet.Count;
                        QuoteSetIndex = Random.Next(ActiveQuote.ListQuoteSet.Count);
                        ActiveQuote.SelectedQuoteSet = QuoteSetIndex;
                    }

                    Quote.Targets ActiveTarget = ActiveQuote.Target;
                    QuoteSet ActiveQuoteSet = ActiveQuote.ActiveQuoteSet;

                    Character ActivePilot = ActiveUnit.Pilot;
                    Character EnemyPilot = EnemyUnit.Pilot;
                    if (ActiveTarget == Quote.Targets.Defender)
                    {
                        ActivePilot = EnemyUnit.Pilot;
                        EnemyPilot = ActiveUnit.Pilot;
                    }

                    bool UseRandomIndex = !ActiveQuoteSet.QuoteSetUseLast && ActiveQuoteSet.QuoteSetChoice == QuoteSet.QuoteSetChoices.Random;
                    QuoteSet.QuoteStyles QuoteStyle = ActiveQuoteSet.QuoteStyle;
                    int QuoteIndex = 0;
                    if (ActiveQuoteSet.QuoteSetChoice == QuoteSet.QuoteSetChoices.Fixed)
                        QuoteIndex = ActiveQuoteSet.QuoteSetChoiceValue;

                    Tuple<string, string> ActiveQuoteTuple;

                    switch (QuoteStyle)
                    {
                        case QuoteSet.QuoteStyles.Reaction:
                            QuoteTypes ActiveQuoteType = QuoteTypes.Damaged;
                            ActiveQuoteTuple = GetQuote(ActiveQuoteType, ActivePilot, EnemyPilot, UseRandomIndex, ref QuoteIndex);
                            ActiveQuote.ActiveText = TextHelper.FitToWidth(fntFinlanderFont, ActiveQuoteTuple.Item2, 500);
                            ActiveQuote.PortraitPath = ActiveQuoteTuple.Item1;
                            ActiveQuote.ActiveCharacterName = ActivePilot.Name;
                            break;

                        case QuoteSet.QuoteStyles.QuoteSet:
                            ActiveQuoteTuple = GetAttackQuote(ActiveQuoteSet.QuoteSetName, ActivePilot, EnemyPilot, UseRandomIndex, ref QuoteIndex);
                            ActiveQuote.ActiveText = TextHelper.FitToWidth(fntFinlanderFont, ActiveQuoteTuple.Item2, 500);
                            ActiveQuote.PortraitPath = ActiveQuoteTuple.Item1;
                            ActiveQuote.ActiveCharacterName = ActivePilot.Name;
                            break;

                        case QuoteSet.QuoteStyles.Custom:
                            ActiveQuote.ActiveText = TextHelper.FitToWidth(fntFinlanderFont, ActiveQuoteSet.CustomText, 500);
                            break;

                        case QuoteSet.QuoteStyles.MoveIn:
                            ActiveQuoteTuple = GetQuote(QuoteTypes.BattleStart, ActivePilot, EnemyPilot, UseRandomIndex, ref QuoteIndex);
                            ActiveQuote.ActiveText = TextHelper.FitToWidth(fntFinlanderFont, ActiveQuoteTuple.Item2, 500);
                            ActiveQuote.PortraitPath = ActiveQuoteTuple.Item1;
                            ActiveQuote.ActiveCharacterName = ActivePilot.Name;
                            break;
                    }

                    if (!string.IsNullOrEmpty(ActiveQuote.PortraitPath))
                    {
                        if (ActiveQuote.PortraitPath.StartsWith("Animations"))
                        {
                            ActiveQuote.ActiveCharacterSprite = new SimpleAnimation("", ActiveQuote.PortraitPath, new AnimationLooped(ActiveQuote.PortraitPath));
                            ActiveQuote.ActiveCharacterSprite.ActiveAnimation.Content = Content;
                            ActiveQuote.ActiveCharacterSprite.ActiveAnimation.Load();
                        }
                        else
                        {
                            ActiveQuote.ActiveCharacterSprite = new SimpleAnimation("", ActiveQuote.PortraitPath, Content.Load<Texture2D>(ActiveQuote.PortraitPath));
                        }
                    }
                }

                #endregion
                
                #region Init renderTarget

                ListAnimationLayer[L].renderTarget = new RenderTarget2D(
                    GraphicsDevice,
                    GraphicsDevice.PresentationParameters.BackBufferWidth,
                    GraphicsDevice.PresentationParameters.BackBufferHeight);

                #endregion
            }
        }

        private void InitMarkers(AnimationLayer ActiveLayer, Squad AttackingSquad, Squad EnemySquad)
        {
            Unit ActiveUnit = AttackingSquad.CurrentLeader;
            Unit EnemyUnit = EnemySquad.CurrentLeader;

            foreach (List<Timeline> ListActiveEvent in ActiveLayer.DicTimelineEvent.Values)
            {
                foreach (Timeline ActiveTimeline in ListActiveEvent)
                {
                    MarkerTimeline ActiveMarkerEvent = ActiveTimeline as MarkerTimeline;
                    if (ActiveMarkerEvent == null)
                        continue;

                    string AnimationPath;

                    switch (ActiveMarkerEvent.MarkerType)
                    {
                        case "Support Stand":
                        case "Support Standing":
                        case "Enemy Standing":
                        case "Enemy Stand":
                        case "Enemy Default":
                            ActiveMarkerEvent.AnimationMarker = new AnimationClass(EnemyUnit.Animations.Default.AnimationName);

                            ActiveMarkerEvent.AnimationMarker.DicTimeline = DicTimeline;
                            ActiveMarkerEvent.AnimationMarker.Load();

                            for (int L = ActiveMarkerEvent.AnimationMarker.ListAnimationLayer.Count - 1; L >= 0; --L)
                            {
                                InitMarkers(ActiveMarkerEvent.AnimationMarker.ListAnimationLayer[L], EnemySquad, AttackingSquad);
                            }
                            break;

                        case "Enemy Wingman A Standing":
                        case "Enemy Wingman 1 Standing":
                            if (EnemySquad.CurrentWingmanA != null)
                            {
                                ActiveMarkerEvent.AnimationMarker = new AnimationClass(EnemySquad.CurrentWingmanA.Animations.Default.AnimationName);

                                ActiveMarkerEvent.AnimationMarker.DicTimeline = DicTimeline;
                                ActiveMarkerEvent.AnimationMarker.Load();

                                for (int L = ActiveMarkerEvent.AnimationMarker.ListAnimationLayer.Count - 1; L >= 0; --L)
                                {
                                    InitMarkers(ActiveMarkerEvent.AnimationMarker.ListAnimationLayer[L], EnemySquad, AttackingSquad);
                                }
                            }
                            break;

                        case "Enemy Wingman B Standing":
                        case "Enemy Wingman 2 Standing":
                            if (EnemySquad.CurrentWingmanB != null)
                            {
                                ActiveMarkerEvent.AnimationMarker = new AnimationClass(EnemySquad.CurrentWingmanB.Animations.Default.AnimationName);

                                ActiveMarkerEvent.AnimationMarker.DicTimeline = DicTimeline;
                                ActiveMarkerEvent.AnimationMarker.Load();

                                for (int L = ActiveMarkerEvent.AnimationMarker.ListAnimationLayer.Count - 1; L >= 0; --L)
                                {
                                    InitMarkers(ActiveMarkerEvent.AnimationMarker.ListAnimationLayer[L], EnemySquad, AttackingSquad);
                                }
                            }
                            break;

                        case "Enemy Hit":
                            if (BattleResult.ArrayResult[0].AttackMissed)
                            {
                                AnimationPath = EnemyUnit.Animations.Default.AnimationName;
                            }
                            else
                            {
                                AnimationPath = EnemyUnit.Animations.Hit.AnimationName;
                            }
                            if (File.Exists("Content/Animations/" + AnimationPath + ".pea"))
                            {
                                ActiveMarkerEvent.AnimationMarker = new AnimationClass(AnimationPath);
                            }
                            else
                            {
                                ActiveMarkerEvent.AnimationMarker = new AnimationClass(EnemyUnit.Animations.Default.AnimationName);
                            }

                            ActiveMarkerEvent.AnimationMarker.DicTimeline = DicTimeline;
                            ActiveMarkerEvent.AnimationMarker.Load();

                            for (int L = ActiveMarkerEvent.AnimationMarker.ListAnimationLayer.Count - 1; L >= 0; --L)
                            {
                                InitMarkers(ActiveMarkerEvent.AnimationMarker.ListAnimationLayer[L], EnemySquad, AttackingSquad);
                            }
                            break;

                        case "Player Stand":
                        case "Player Standing":
                        case "Player Default":
                            ActiveMarkerEvent.AnimationMarker = new AnimationClass(ActiveUnit.Animations.Default.AnimationName);

                            ActiveMarkerEvent.AnimationMarker.DicTimeline = DicTimeline;
                            ActiveMarkerEvent.AnimationMarker.Load();

                            for (int L = ActiveMarkerEvent.AnimationMarker.ListAnimationLayer.Count - 1; L >= 0; --L)
                            {
                                InitMarkers(ActiveMarkerEvent.AnimationMarker.ListAnimationLayer[L], AttackingSquad, EnemySquad);
                            }
                            break;

                        default:
                            AnimationPath = EnemyUnit.Animations.Default.AnimationName;
                            if (ActiveMarkerEvent.MarkerType.StartsWith("Player "))
                            {
                                AnimationPath = ActiveMarkerEvent.MarkerType.Split(new string[] { "Player " }, StringSplitOptions.RemoveEmptyEntries)[0];
                            }
                            ActiveMarkerEvent.AnimationMarker = new AnimationClass(AnimationPath);

                            ActiveMarkerEvent.AnimationMarker.DicTimeline = DicTimeline;
                            ActiveMarkerEvent.AnimationMarker.Load();

                            for (int L = ActiveMarkerEvent.AnimationMarker.ListAnimationLayer.Count - 1; L >= 0; --L)
                            {
                                InitMarkers(ActiveMarkerEvent.AnimationMarker.ListAnimationLayer[L], AttackingSquad, EnemySquad);
                            }
                            break;
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ActiveKeyFrame >= LoopEnd)
            {
                RemoveScreen(this);
            }

            if (KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                RemoveScreen(this);
            }
        }

        public void DamageEnemyUnit(int Damage)
        {
            if (IsLeftAttacking)
            {
                UnitStats.RightUnitHP -= Damage;
            }
            else
            {
                UnitStats.LeftUnitHP -= Damage;
            }
        }

        public void ConsumeEN(int EN)
        {
            if (IsLeftAttacking)
            {
                UnitStats.RightUnitHP -= EN;
            }
            else
            {
                UnitStats.LeftUnitHP -= EN;
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            g.BeginUnscaled(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            GraphicsDevice.Clear(Color.Black);

            base.BeginDraw(g);

            g.End();

            if (ActiveCharacterSprite != null && ActiveCharacterSprite.IsAnimated)
            {
                ActiveCharacterSprite.ActiveAnimation.BeginDraw(g);
            }

            for (int L = 0; L < ListAnimationLayer.Count; L++)
            {
                if (ListAnimationLayer[L].renderTarget.Width != GraphicsDevice.PresentationParameters.BackBufferWidth ||
                    ListAnimationLayer[L].renderTarget.Height != GraphicsDevice.PresentationParameters.BackBufferHeight)
                {
                    ListAnimationLayer[L].renderTarget = new RenderTarget2D(
                        GraphicsDevice,
                        GraphicsDevice.PresentationParameters.BackBufferWidth,
                        GraphicsDevice.PresentationParameters.BackBufferHeight);
                }
                for (int M = 0; M < ListAnimationLayer[L].ListActiveMarker.Count; M++)
                {
                    AnimationClass ActiveMarkerAnimation = ListAnimationLayer[L].ListActiveMarker[M].AnimationMarker;

                    for (int i = 0; i < ActiveMarkerAnimation.ListAnimationLayer.Count; i++)
                    {
                        if (ActiveMarkerAnimation.ListAnimationLayer[i].renderTarget == null ||
                            ActiveMarkerAnimation.ListAnimationLayer[i].renderTarget.Width != GraphicsDevice.PresentationParameters.BackBufferWidth ||
                            ActiveMarkerAnimation.ListAnimationLayer[i].renderTarget.Height != GraphicsDevice.PresentationParameters.BackBufferHeight)
                        {
                            ActiveMarkerAnimation.ListAnimationLayer[i].renderTarget = new RenderTarget2D(
                                GraphicsDevice,
                                GraphicsDevice.PresentationParameters.BackBufferWidth,
                                GraphicsDevice.PresentationParameters.BackBufferHeight);
                        }
                    }
                }

                for (int M = 0; M < ListAnimationLayer[L].ListActiveMarker.Count; M++)
                {
                    AnimationClass ActiveMarkerAnimation = ListAnimationLayer[L].ListActiveMarker[M].AnimationMarker;

                    for (int i = 0; i < ActiveMarkerAnimation.ListAnimationLayer.BasicLayerCount; i++)
                    {
                        GraphicsDevice.SetRenderTarget(ActiveMarkerAnimation.ListAnimationLayer[i].renderTarget);
                        GraphicsDevice.Clear(Color.Transparent);
                        //Draw submarkers.
                        DrawLayer(g, ActiveMarkerAnimation.ListAnimationLayer[i], true, false, null, true);
                    }
                }

                GraphicsDevice.SetRenderTarget(ListAnimationLayer[L].renderTarget);
                GraphicsDevice.Clear(Color.Transparent);

                DrawLayer(g, ListAnimationLayer[L], false, true, null, true);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            if (ActiveAnimationBackground != null)
                ActiveAnimationBackground.Draw(g, Constants.Width, Constants.Height);
            g.Begin();

            for (int A = ListAnimationLayer.Count - 1; A >= 0; --A)
            {
                if (ListAnimationLayer[A].ListPolygonCutter.Count > 0)
                {
                    PolygonEffect.Texture = ListAnimationLayer[A].renderTarget;
                    PolygonEffect.CurrentTechnique.Passes[0].Apply();

                    GraphicsDevice.RasterizerState = RasterizerState.CullNone;

                    for (int P = 0; P < ListAnimationLayer[A].ListPolygonCutter.Count; P++)
                        ListAnimationLayer[A].ListPolygonCutter[P].Draw(g, false);
                }
                else
                {
                    if (IsLeftAttacking)
                    {
                        g.Draw(ListAnimationLayer[A].renderTarget, new Vector2(Constants.Width / 2, Constants.Height / 2), null, Color.White, 0, new Vector2(Constants.Width / 2, Constants.Height / 2), 1, SpriteEffects.FlipHorizontally, 0);
                    }
                    else
                    {
                        g.Draw(ListAnimationLayer[A].renderTarget, Vector2.Zero, Color.White);
                    }
                }
            }

            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            DrawBox(g, new Vector2(0, 0), Constants.Width / 2, 84, Color.Red);
            int PosX = 0;
            DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeHP, new Vector2(PosX + 75, 30), UnitStats.LeftUnitHP, UnitStats.LeftUnitHPMax);
            DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeEN, new Vector2(PosX + 75, 50), UnitStats.LeftUnitEN, UnitStats.LeftUnitENMax);
            g.DrawString(fntFinlanderFont, "HP", new Vector2(PosX + 40, 20), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, UnitStats.LeftUnitHP + "/" + UnitStats.LeftUnitHPMax, new Vector2(PosX + 242, 17), Color.White);
            g.DrawString(fntFinlanderFont, "EN", new Vector2(PosX + 40, 40), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, UnitStats.LeftUnitEN + "/" + UnitStats.LeftUnitENMax, new Vector2(PosX + 242, 37), Color.White);
            g.Draw(LeftSquad.CurrentLeader.SpriteMap, new Vector2(PosX + 7, 30), Color.White);

            PosX = Constants.Width / 2 + 68;
            DrawBox(g, new Vector2(Constants.Width / 2, 0), Constants.Width / 2, 84, Color.Blue);
            DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeHP, new Vector2(PosX + 75, 30), UnitStats.RightUnitHP, UnitStats.RightUnitHPMax);
            DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeEN, new Vector2(PosX + 75, 50), UnitStats.RightUnitEN, UnitStats.RightUnitENMax);
            g.DrawString(fntFinlanderFont, "HP", new Vector2(PosX + 40, 20), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, UnitStats.RightUnitHP + "/" + UnitStats.RightUnitHPMax, new Vector2(PosX + 242, 17), Color.White);
            g.DrawString(fntFinlanderFont, "EN", new Vector2(PosX + 40, 40), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, UnitStats.RightUnitEN + "/" + UnitStats.RightUnitENMax, new Vector2(PosX + 242, 37), Color.White);
            g.Draw(RightSquad.CurrentLeader.SpriteMap, new Vector2(PosX + 7, 30), Color.White);
            g.Draw(sprInfinity, new Vector2((Constants.Width - sprInfinity.Width) / 2, 15), Color.White);

            DrawBox(g, new Vector2(0, Constants.Height - VNBoxHeight), Constants.Width, VNBoxHeight, Color.White);

            if (ActiveCharacterSprite != null)
            {
                if (ActiveCharacterSprite.IsAnimated)
                {
                    for (int L = ActiveCharacterSprite.ActiveAnimation.ListAnimationLayer.Count - 1; L >= 0; --L)
                    {
                        int OriginX = (int)ActiveCharacterSprite.ActiveAnimation.AnimationOrigin.Position.X;

                        g.Draw(ActiveCharacterSprite.ActiveAnimation.ListAnimationLayer[L].renderTarget,
                            new Vector2(0, Constants.Height - VNBoxHeight), null, Color.White, 0,
                            new Vector2(OriginX, ActiveCharacterSprite.ActiveAnimation.AnimationOrigin.Position.Y),
                            new Vector2(1, 1), SpriteEffects.None, 0);
                    }
                }
                else
                {
                    g.Draw(ActiveCharacterSprite.StaticSprite,
                        new Rectangle(20, Constants.Height - VNBoxHeight + 15, ActiveCharacterSprite.StaticSprite.Width, ActiveCharacterSprite.StaticSprite.Height),
                        new Rectangle(0, 0, ActiveCharacterSprite.StaticSprite.Width, ActiveCharacterSprite.StaticSprite.Height), Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                }
            }
            if (ActiveCharacterName != null)
            {
                g.DrawString(fntFinlanderFont, ActiveCharacterName, new Vector2(105, Constants.Height - VNBoxHeight + 8), Color.White);

                if (ActiveQuoteSet != null)
                {
                    TextHelper.DrawTextMultiline(g, fntFinlanderFont, ActiveQuoteSet, TextHelper.TextAligns.Left, 360, Constants.Height - VNBoxHeight + 38, 500);
                }
            }
            else if (ActiveQuoteSet != null)
            {
                TextHelper.DrawTextMultiline(g, fntFinlanderFont, ActiveQuoteSet, TextHelper.TextAligns.Left, 360, Constants.Height - VNBoxHeight + 10, 500);
            }

            g.End();
            g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
        }
    }

    public class EndBattleAnimationScreen : GameScreen
    {
        private readonly DeathmatchMap Map;
        private readonly Squad Attacker;
        private readonly SupportSquadHolder ActiveSquadSupport;
        private readonly int AttackerPlayerIndex;
        private readonly Squad TargetSquad;
        private readonly SupportSquadHolder TargetSquadSupport;
        private readonly int DefenderPlayerIndex;
        private readonly SquadBattleResult ResultAttack;
        private readonly SquadBattleResult ResultDefend;

        public EndBattleAnimationScreen(DeathmatchMap Map, Squad Attacker, SupportSquadHolder ActiveSquadSupport, int AttackerPlayerIndex,
            Squad TargetSquad, SupportSquadHolder TargetSquadSupport, int DefenderPlayerIndex, SquadBattleResult ResultAttack, SquadBattleResult ResultDefend)
        {
            this.Map = Map;
            this.Attacker = Attacker;
            this.ActiveSquadSupport = ActiveSquadSupport;
            this.AttackerPlayerIndex = AttackerPlayerIndex;
            this.TargetSquad = TargetSquad;
            this.TargetSquadSupport = TargetSquadSupport;
            this.DefenderPlayerIndex = DefenderPlayerIndex;
            this.ResultAttack = ResultAttack;
            this.ResultDefend = ResultDefend;
        }

        public override void Load()
        {
        }

        public override void Update(GameTime gameTime)
        {
            Map.FinalizeBattle(Attacker, ActiveSquadSupport, AttackerPlayerIndex, TargetSquad, TargetSquadSupport, DefenderPlayerIndex, ResultAttack, ResultDefend);
            RemoveScreen(this);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
