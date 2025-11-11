using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.UI;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class EditBookCardListFilterScreen : GameScreen
    {
        public enum Filters { All, Creatures, Neutral, Fire, Water, Earth, Air, Dual, Item, Spell, EnchantPlayer, EnchantCreature, }

        #region Ressources

        private SpriteFont fntMenuText;

        private Texture2D sprInfoBand;
        private Texture2D sprExtraFrame;

        #endregion

        private BoxScrollbar MissionScrollbar;

        private const int CardSpacing = 7;
        private const int CardWidth = 85;
        private const int CardHeight = 110;
        private int HeaderHeight = Constants.Height / 16;

        private readonly Player ActivePlayer;
        private readonly CardBook ActiveBook;
        private readonly CardBook GlobalBook;
        private readonly List<CardInfo> ListFilteredCard;

        private int CursorIndex;
        private int ScrollbarIndex;
        private bool DrawBackground;
        int CardsPerLine = 7;

        public List<Card> ListSelectedCard;
        private CardSymbols Symbols;
        private IconHolder Icons;

        public Dictionary<string, Unit> DicUnitType;
        public Dictionary<string, BaseSkillRequirement> DicRequirement;
        public Dictionary<string, BaseEffect> DicEffect;
        public Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget;
        public Dictionary<string, ManualSkillTarget> DicManualSkillTarget;

        public EditBookCardListFilterScreen(Player ActivePlayer, CardBook ActiveBook, Filters Filter)
        {
            RequireFocus = true;
            RequireDrawFocus = true;
            this.ActivePlayer = ActivePlayer;
            this.ActiveBook = ActiveBook;
            GlobalBook = ActivePlayer.Inventory.GlobalBook;
            ListFilteredCard = new List<CardInfo>();
            FillCardList(Filter, null);

            Symbols = CardSymbols.Symbols;
            Icons = IconHolder.Icons;

            DicUnitType = PlayerManager.DicUnitType;
            DicRequirement = PlayerManager.DicRequirement;
            DicEffect = PlayerManager.DicEffect;
            DicAutomaticSkillTarget = PlayerManager.DicAutomaticSkillTarget;
            DicManualSkillTarget = PlayerManager.DicManualSkillTarget;
        }

        public EditBookCardListFilterScreen(CardBook GlobalBook, Filters Filter, Card LastCard, bool MultipleSelection, bool DrawBackground = true)
        {
            RequireFocus = true;
            RequireDrawFocus = true;
            this.GlobalBook = ActiveBook = GlobalBook;
            this.DrawBackground = DrawBackground;
            ListFilteredCard = new List<CardInfo>();
            ListSelectedCard = new List<Card>();
            FillCardList(Filter, LastCard);

            Symbols = CardSymbols.Symbols;
            Icons = IconHolder.Icons;

            int CursorY = (CardHeight + 20) * (CursorIndex / CardsPerLine);

            if (CursorY < ScrollbarIndex)
            {
                ScrollbarIndex = (CardHeight + 20) * (CursorIndex / CardsPerLine);
            }
            else if (CursorY > ScrollbarIndex + (CardHeight + 20) * 2)
            {
                ScrollbarIndex = (CardHeight + 20) * ((CursorIndex / CardsPerLine) - 2);
            }

            DicUnitType = PlayerManager.DicUnitType;
            DicRequirement = PlayerManager.DicRequirement;
            DicEffect = PlayerManager.DicEffect;
            DicAutomaticSkillTarget = PlayerManager.DicAutomaticSkillTarget;
            DicManualSkillTarget = PlayerManager.DicManualSkillTarget;
        }

        private void FillCardList(Filters Filter, Card LastCard)
        {
            switch (Filter)
            {
                case Filters.All:
                    foreach (CardInfo ActiveCard in GlobalBook.ListCard)
                    {
                        if (LastCard != null && ActiveCard.Card.Name == LastCard.Name)
                        {
                            CursorIndex = ListFilteredCard.Count;
                        }

                        ListFilteredCard.Add(ActiveCard);
                    }
                    break;

                #region Creatures

                case Filters.Creatures:
                    foreach (CardInfo ActiveCard in GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard.Card as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (LastCard != null && ActiveCard.Card.Name == LastCard.Name)
                            {
                                CursorIndex = ListFilteredCard.Count;
                            }

                            ListFilteredCard.Add(ActiveCard);
                        }
                    }
                    break;

                case Filters.Neutral:
                    foreach (CardInfo ActiveCard in GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard.Card as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity.Length == 1 && ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity[0] == CreatureCard.ElementalAffinity.Neutral)
                            {
                                if (LastCard != null && ActiveCard.Card.Name == LastCard.Name)
                                {
                                    CursorIndex = ListFilteredCard.Count;
                                }

                                ListFilteredCard.Add(ActiveCard);
                            }
                        }
                    }
                    break;

                case Filters.Fire:
                    foreach (CardInfo ActiveCard in GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard.Card as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity.Length == 1 && ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity[0] == CreatureCard.ElementalAffinity.Fire)
                            {
                                if (LastCard != null && ActiveCard.Card.Name == LastCard.Name)
                                {
                                    CursorIndex = ListFilteredCard.Count;
                                }

                                ListFilteredCard.Add(ActiveCard);
                            }
                        }
                    }
                    break;

                case Filters.Water:
                    foreach (CardInfo ActiveCard in GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard.Card as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity.Length == 1 && ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity[0] == CreatureCard.ElementalAffinity.Water)
                            {
                                if (LastCard != null && ActiveCard.Card.Name == LastCard.Name)
                                {
                                    CursorIndex = ListFilteredCard.Count;
                                }

                                ListFilteredCard.Add(ActiveCard);
                            }
                        }
                    }
                    break;

                case Filters.Earth:
                    foreach (CardInfo ActiveCard in GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard.Card as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity.Length == 1 && ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity[0] == CreatureCard.ElementalAffinity.Earth)
                            {
                                if (LastCard != null && ActiveCard.Card.Name == LastCard.Name)
                                {
                                    CursorIndex = ListFilteredCard.Count;
                                }

                                ListFilteredCard.Add(ActiveCard);
                            }
                        }
                    }
                    break;

                case Filters.Air:
                    foreach (CardInfo ActiveCard in GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard.Card as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity.Length == 1 && ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity[0] == CreatureCard.ElementalAffinity.Air)
                            {
                                if (LastCard != null && ActiveCard.Card.Name == LastCard.Name)
                                {
                                    CursorIndex = ListFilteredCard.Count;
                                }

                                ListFilteredCard.Add(ActiveCard);
                            }
                        }
                    }
                    break;

                case Filters.Dual:
                    foreach (CardInfo ActiveCard in GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard.Card as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity.Length > 1)
                            {
                                if (LastCard != null && ActiveCard.Card.Name == LastCard.Name)
                                {
                                    CursorIndex = ListFilteredCard.Count;
                                }

                                ListFilteredCard.Add(ActiveCard);
                            }
                        }
                    }
                    break;

                case Filters.Item:
                    foreach (CardInfo ActiveCard in GlobalBook.ListCard)
                    {
                        ItemCard ActiveItemCard = ActiveCard.Card as ItemCard;
                        if (ActiveItemCard != null)
                        {
                            if (LastCard != null && ActiveCard.Card.Name == LastCard.Name)
                            {
                                CursorIndex = ListFilteredCard.Count;
                            }

                            ListFilteredCard.Add(ActiveCard);
                        }
                    }
                    break;

                #endregion

                case Filters.Spell:
                    foreach (CardInfo ActiveCard in GlobalBook.ListCard)
                    {
                        SpellCard ActiveSpellCard = ActiveCard.Card as SpellCard;
                        if (ActiveSpellCard != null)
                        {
                            if (LastCard != null && ActiveCard.Card.Name == LastCard.Name)
                            {
                                CursorIndex = ListFilteredCard.Count;
                            }

                            ListFilteredCard.Add(ActiveCard);
                        }
                    }
                    break;

                case Filters.EnchantPlayer:
                    foreach (CardInfo ActiveCard in GlobalBook.ListCard)
                    {
                        SpellCard ActiveSpellCard = ActiveCard.Card as SpellCard;
                        if (ActiveSpellCard != null)
                        {
                            if (ActiveSpellCard.ListSpell[0].Target.TargetType == ManualSkillActivationSorcererStreet.AllPlayerTargetType)
                            {
                                if (LastCard != null && ActiveCard.Card.Name == LastCard.Name)
                                {
                                    CursorIndex = ListFilteredCard.Count;
                                }

                                ListFilteredCard.Add(ActiveCard);
                            }
                        }
                    }
                    break;

                case Filters.EnchantCreature:
                    foreach (CardInfo ActiveCard in GlobalBook.ListCard)
                    {
                        SpellCard ActiveSpellCard = ActiveCard.Card as SpellCard;
                        if (ActiveSpellCard != null)
                        {
                            //if (ActiveSpellCard.SpellTarget == SpellCard.SpellTargets.NoTarget || ActiveSpellCard.SpellTarget == SpellCard.SpellTargets.Creature || ActiveSpellCard.SpellTarget == SpellCard.SpellTargets.Area)
                            {
                                if (LastCard != null && ActiveCard.Card.Name == LastCard.Name)
                                {
                                    CursorIndex = ListFilteredCard.Count;
                                }

                                ListFilteredCard.Add(ActiveCard);
                            }
                        }
                    }
                    break;
            }
        }

        public override void Load()
        {
            float MaxY = Constants.Height / 6 + 130 * 2 + (CardHeight + 20) * ((ActiveBook.ListCard.Count - 14) / CardsPerLine);
            MissionScrollbar = new BoxScrollbar(new Vector2(Constants.Width - 20, Constants.Height / 6), Constants.Height - Constants.Height / 3, MaxY, OnMissionScrollbarChange);

            fntMenuText = Content.Load<SpriteFont>("Fonts/Arial30");

            sprInfoBand = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Info/Info Band");
            sprExtraFrame = Content.Load<Texture2D>("Deathmatch/Lobby Menu/Extra Frame 2");
        }

        private void OnMissionScrollbarChange(float ScrollbarValue)
        {
            ScrollbarIndex = (int)ScrollbarValue;
        }

        public override void Update(GameTime gameTime)
        {
            MissionScrollbar.Update(gameTime);

            if (InputHelper.InputConfirmPressed() && ListFilteredCard.Count > 0)
            {
                if (ActivePlayer == null)
                {
                    Card SelectedCard = ListFilteredCard[CursorIndex].Card;
                    Card CopyCard = ActiveBook.DicCardsByType[SelectedCard.CardType][SelectedCard.Path].Card.Copy(DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                    ListSelectedCard.Add(CopyCard);

                    RemoveScreen(this);
                }
                else
                {
                    Card SelectedCard = ListFilteredCard[CursorIndex].Card;
                    if (!ActiveBook.DicCardsByType.ContainsKey(SelectedCard.CardType) || !ActiveBook.DicCardsByType[SelectedCard.CardType].ContainsKey(SelectedCard.Path))
                    {
                        Card CopyCard = GlobalBook.DicCardsByType[SelectedCard.CardType][SelectedCard.Path].Card.Copy(DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                        ActiveBook.AddCard(new CardInfo(CopyCard, 0));
                    }

                    PushScreen(new EditBookCardScreen(ActivePlayer, ActiveBook, ActiveBook.DicCardsByType[SelectedCard.CardType][SelectedCard.Path]));
                }
            }
            else if (InputHelper.InputCancelPressed())
            {
                if (ListSelectedCard != null)
                {
                    ListSelectedCard.Clear();
                }

                ListSelectedCard = null;
                RemoveScreen(this);
            }
            else if (InputHelper.InputRightPressed())
            {
                CursorIndex += 1;
                if (CursorIndex >= ListFilteredCard.Count)
                {
                    CursorIndex -= CursorIndex % CardsPerLine;
                }
            }
            else if (InputHelper.InputLeftPressed())
            {
                CursorIndex -= 1;
                if (CursorIndex < 0)
                {
                    CursorIndex = 6;
                }
                else if (CursorIndex % CardsPerLine == 6)
                {
                    if (CursorIndex + CardsPerLine >= ListFilteredCard.Count)
                    {
                        CursorIndex = ListFilteredCard.Count - 1;
                    }
                    else
                    {
                        CursorIndex += CardsPerLine;
                    }
                }
            }
            else if (InputHelper.InputDownPressed())
            {
                if (CursorIndex + CardsPerLine >= ListFilteredCard.Count)
                {
                    CursorIndex = CursorIndex % CardsPerLine;
                }
                else
                {
                    CursorIndex += CardsPerLine;

                    if (CursorIndex > ListFilteredCard.Count)
                    {
                        CursorIndex -= CardsPerLine;
                    }
                }

                int CursorY = (CardHeight + 20) * (CursorIndex / CardsPerLine);

                if (CursorY < ScrollbarIndex)
                {
                    ScrollbarIndex = (CardHeight + 20) * (CursorIndex / CardsPerLine);
                }
                else if (CursorY > ScrollbarIndex + (CardHeight + 20) * 2)
                {
                    ScrollbarIndex = (CardHeight + 20) * ((CursorIndex / CardsPerLine) - 2);
                }
            }
            else if (InputHelper.InputUpPressed())
            {
                CursorIndex -= CardsPerLine;
                if (CursorIndex < 0)
                {
                    if ((CursorIndex + CardsPerLine) % CardsPerLine >= ListFilteredCard.Count % CardsPerLine)
                    {
                        CursorIndex = (ListFilteredCard.Count / CardsPerLine - 1) * CardsPerLine + (CursorIndex + CardsPerLine) % CardsPerLine;
                    }
                    else
                    {
                        CursorIndex = (ListFilteredCard.Count / CardsPerLine) * CardsPerLine + (CursorIndex + CardsPerLine) % CardsPerLine;
                    }
                }

                int CursorY = (CardHeight + 20) * (CursorIndex / CardsPerLine);

                if (CursorY < ScrollbarIndex)
                {
                    ScrollbarIndex = (CardHeight + 20) * (CursorIndex / CardsPerLine);
                }
                else if (CursorY > ScrollbarIndex + (CardHeight + 20) * 2)
                {
                    ScrollbarIndex = (CardHeight + 20) * ((CursorIndex / CardsPerLine) - 2);
                }
            }
            else if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.E))
            {
                if (CursorIndex + CardsPerLine >= ListFilteredCard.Count)
                {
                    CursorIndex = CursorIndex % CardsPerLine;
                }
                else
                {
                    CursorIndex += CardsPerLine * 7;

                    if (CursorIndex > ListFilteredCard.Count)
                    {
                        CursorIndex = ListFilteredCard.Count - 1;
                    }
                }

                int CursorY = (CardHeight + 20) * (CursorIndex / CardsPerLine);

                if (CursorY < ScrollbarIndex)
                {
                    ScrollbarIndex = (CardHeight + 20) * (CursorIndex / CardsPerLine);
                }
                else if (CursorY > ScrollbarIndex + (CardHeight + 20) * 2)
                {
                    ScrollbarIndex = (CardHeight + 20) * ((CursorIndex / CardsPerLine) - 2);
                }
            }
            else if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Q))
            {
                CursorIndex -= CardsPerLine * 7;
                if (CursorIndex < 0)
                {
                    if ((CursorIndex + CardsPerLine) % CardsPerLine >= ListFilteredCard.Count % CardsPerLine)
                    {
                        CursorIndex = (ListFilteredCard.Count / CardsPerLine - 1) * CardsPerLine + (CursorIndex + CardsPerLine) % CardsPerLine;
                    }
                    else
                    {
                        CursorIndex = (ListFilteredCard.Count / CardsPerLine) * CardsPerLine + (CursorIndex + CardsPerLine) % CardsPerLine;
                    }
                }

                int CursorY = (CardHeight + 20) * (CursorIndex / CardsPerLine);

                if (CursorY < ScrollbarIndex)
                {
                    ScrollbarIndex = (CardHeight + 20) * (CursorIndex / CardsPerLine);
                }
                else if (CursorY > ScrollbarIndex + (CardHeight + 20) * 2)
                {
                    ScrollbarIndex = (CardHeight + 20) * ((CursorIndex / CardsPerLine) - 2);
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (DrawBackground)
            {
                DrawBox(g, new Vector2(-5, -5), Constants.Width + 10, Constants.Height + 10, Color.White);
            }

            DrawBookCards(g, Constants.Height / 6 - ScrollbarIndex);

            int Y = 56;
            g.Draw(sprInfoBand, new Rectangle(0, Y, Constants.Width, (int)(sprInfoBand.Height * 1.5)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            int X = 180;
            Y = 80;
            g.DrawString(fntMenuText, "Book Edit", new Vector2(X, Y), Color.White);
            if (ActivePlayer != null)
            {
                SorcererStreetInventoryScreen.DrawBookIsReady(g, fntMenuText, ActivePlayer.Name, ActiveBook);
            }

            int CursorX = Constants.Width / 2 - CardWidth / 2 - (CardWidth + CardSpacing) * 3 + (CardWidth + CardSpacing) * (CursorIndex % 7) - 50;
            int CursorY = Constants.Height / 6 + CardHeight/ 2 + (CardHeight + 20) * (CursorIndex / 7);
            MenuHelper.DrawFingerIcon(g, new Vector2(CursorX, CursorY - ScrollbarIndex));


            Y = 934;
            g.Draw(sprInfoBand, new Rectangle(0, Y, Constants.Width, (int)(sprInfoBand.Height * 1.5)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            X = 180;
            Y = 958;
            g.DrawString(fntMenuText, ListFilteredCard[CursorIndex].Card.Name, new Vector2(X, Y), Color.White);

            if (ActivePlayer != null)
            {
                SorcererStreetInventoryScreen.DrawBookInformationSmall(g, sprExtraFrame, fntMenuText, "Book Information", Symbols, Icons, ActivePlayer.Inventory.GlobalBook);
            }

            MissionScrollbar.Draw(g);
        }

        private void DrawBookCards(CustomSpriteBatch g, float StartY)
        {
            int CardNumberBoxWidth = (int)(CardWidth / 3.2f);

            for (int C = 0; C < ListFilteredCard.Count; ++C)
            {
                float X = Constants.Width / 2 - CardWidth / 2 - (CardWidth + CardSpacing) * 3 + (CardWidth + CardSpacing) * (C % CardsPerLine);
                float Y = StartY + (CardHeight + 20) * (C / CardsPerLine);

                g.Draw(ListFilteredCard[C].Card.sprCard, new Rectangle((int)X, (int)Y, CardWidth, CardHeight), new Rectangle(0, 0, ListFilteredCard[C].Card.sprCard.Width, ListFilteredCard[C].Card.sprCard.Height), Color.White);
                DrawBox(g, new Vector2(X + CardWidth / 2 - CardNumberBoxWidth / 2, Y + CardHeight - CardNumberBoxWidth / 2), CardNumberBoxWidth, CardNumberBoxWidth, Color.White);
                TextHelper.DrawTextMiddleAligned(g, ListFilteredCard[C].QuantityOwned.ToString(), new Vector2(X + CardWidth / 2, Y + 3 + CardHeight - CardNumberBoxWidth / 2), Color.White);
            }
        }
    }
}
