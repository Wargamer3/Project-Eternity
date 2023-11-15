using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.UI;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class EditBookCardListFilterScreen : GameScreen
    {
        public enum Filters { Creatures, Neutral, Fire, Water, Earth, Air, Dual, Item, EnchantPlayer, EnchantCreature, }

        #region Ressources

        private SpriteFont fntArial12;

        #endregion

        private BoxScrollbar MissionScrollbar;

        private const int CardSpacing = 7;
        private const int CardWidth = 85;
        private const int CardHeight = 110;
        private int HeaderHeight = Constants.Height / 16;

        private readonly Player ActivePlayer;
        private readonly CardBook ActiveBook;
        private readonly CardBook GlobalBook;
        private readonly List<Card> ListFilteredCard;

        private int CursorIndex;
        private int ScrollbarIndex;
        int CardsPerLine = 7;

        public List<Card> ListSelectedCard;

        public EditBookCardListFilterScreen(Player ActivePlayer, CardBook ActiveBook, Filters Filter)
        {
            this.ActivePlayer = ActivePlayer;
            this.ActiveBook = ActiveBook;
            GlobalBook = ActivePlayer.Inventory.GlobalBook;
            ListFilteredCard = new List<Card>();
            FillCardList(Filter, null);
        }

        public EditBookCardListFilterScreen(CardBook GlobalBook, Filters Filter, Card LastCard, bool MultipleSelection)
        {
            this.GlobalBook = ActiveBook = GlobalBook;
            ListFilteredCard = new List<Card>();
            ListSelectedCard = new List<Card>();
            FillCardList(Filter, LastCard);

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

        private void FillCardList(Filters Filter, Card LastCard)
        {
            switch (Filter)
            {
                #region Creatures

                case Filters.Creatures:
                    foreach (Card ActiveCard in GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (LastCard != null && ActiveCard.Name == LastCard.Name)
                            {
                                CursorIndex = ListFilteredCard.Count;
                            }

                            ListFilteredCard.Add(ActiveCreatureCard);
                        }
                    }
                    break;

                case Filters.Neutral:
                    foreach (Card ActiveCard in GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity.Length == 1 && ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity[0] == CreatureCard.ElementalAffinity.Neutral)
                            {
                                if (LastCard != null && ActiveCard.Name == LastCard.Name)
                                {
                                    CursorIndex = ListFilteredCard.Count;
                                }

                                ListFilteredCard.Add(ActiveCreatureCard);
                            }
                        }
                    }
                    break;

                case Filters.Fire:
                    foreach (Card ActiveCard in GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity.Length == 1 && ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity[0] == CreatureCard.ElementalAffinity.Fire)
                            {
                                if (LastCard != null && ActiveCard.Name == LastCard.Name)
                                {
                                    CursorIndex = ListFilteredCard.Count;
                                }

                                ListFilteredCard.Add(ActiveCreatureCard);
                            }
                        }
                    }
                    break;

                case Filters.Water:
                    foreach (Card ActiveCard in GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity.Length == 1 && ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity[0] == CreatureCard.ElementalAffinity.Water)
                            {
                                if (LastCard != null && ActiveCard.Name == LastCard.Name)
                                {
                                    CursorIndex = ListFilteredCard.Count;
                                }

                                ListFilteredCard.Add(ActiveCreatureCard);
                            }
                        }
                    }
                    break;

                case Filters.Earth:
                    foreach (Card ActiveCard in GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity.Length == 1 && ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity[0] == CreatureCard.ElementalAffinity.Earth)
                            {
                                if (LastCard != null && ActiveCard.Name == LastCard.Name)
                                {
                                    CursorIndex = ListFilteredCard.Count;
                                }

                                ListFilteredCard.Add(ActiveCreatureCard);
                            }
                        }
                    }
                    break;

                case Filters.Air:
                    foreach (Card ActiveCard in GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity.Length == 1 && ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity[0] == CreatureCard.ElementalAffinity.Air)
                            {
                                if (LastCard != null && ActiveCard.Name == LastCard.Name)
                                {
                                    CursorIndex = ListFilteredCard.Count;
                                }

                                ListFilteredCard.Add(ActiveCreatureCard);
                            }
                        }
                    }
                    break;

                case Filters.Dual:
                    foreach (Card ActiveCard in GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None).ArrayElementAffinity.Length > 1)
                            {
                                if (LastCard != null && ActiveCard.Name == LastCard.Name)
                                {
                                    CursorIndex = ListFilteredCard.Count;
                                }

                                ListFilteredCard.Add(ActiveCreatureCard);
                            }
                        }
                    }
                    break;

                case Filters.Item:
                    foreach (Card ActiveCard in GlobalBook.ListCard)
                    {
                        ItemCard ActiveItemCard = ActiveCard as ItemCard;
                        if (ActiveItemCard != null)
                        {
                            if (LastCard != null && ActiveCard.Name == LastCard.Name)
                            {
                                CursorIndex = ListFilteredCard.Count;
                            }

                            ListFilteredCard.Add(ActiveItemCard);
                        }
                    }
                    break;

                #endregion

                case Filters.EnchantPlayer:
                    foreach (Card ActiveCard in GlobalBook.ListCard)
                    {
                        SpellCard ActiveSpellCard = ActiveCard as SpellCard;
                        if (ActiveSpellCard != null)
                        {
                            if (ActiveSpellCard.Spell.Target.TargetType == ManualSkillActivationSorcererStreet.PlayerTargetType)
                            {
                                if (LastCard != null && ActiveCard.Name == LastCard.Name)
                                {
                                    CursorIndex = ListFilteredCard.Count;
                                }

                                ListFilteredCard.Add(ActiveSpellCard);
                            }
                        }
                    }
                    break;

                case Filters.EnchantCreature:
                    foreach (Card ActiveCard in GlobalBook.ListCard)
                    {
                        SpellCard ActiveSpellCard = ActiveCard as SpellCard;
                        if (ActiveSpellCard != null)
                        {
                            //if (ActiveSpellCard.SpellTarget == SpellCard.SpellTargets.NoTarget || ActiveSpellCard.SpellTarget == SpellCard.SpellTargets.Creature || ActiveSpellCard.SpellTarget == SpellCard.SpellTargets.Area)
                            {
                                if (LastCard != null && ActiveCard.Name == LastCard.Name)
                                {
                                    CursorIndex = ListFilteredCard.Count;
                                }

                                ListFilteredCard.Add(ActiveSpellCard);
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

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
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
                    Card SelectedCard = ListFilteredCard[CursorIndex];
                    Card CopyCard = ActiveBook.DicCardsByType[SelectedCard.CardType][SelectedCard.Path].Copy(PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                    ListSelectedCard.Add(CopyCard);

                    RemoveScreen(this);
                }
                else
                {
                    Card SelectedCard = ListFilteredCard[CursorIndex];
                    if (!ActiveBook.DicCardsByType.ContainsKey(SelectedCard.CardType) || !ActiveBook.DicCardsByType[SelectedCard.CardType].ContainsKey(SelectedCard.Path))
                    {
                        Card CopyCard = GlobalBook.DicCardsByType[SelectedCard.CardType][SelectedCard.Path].Copy(PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                        CopyCard.QuantityOwned = 0;
                        ActiveBook.AddCard(CopyCard);
                    }

                    PushScreen(new EditBookCardScreen(ActivePlayer, ActiveBook, ActiveBook.DicCardsByType[SelectedCard.CardType][SelectedCard.Path]));
                }
            }
            else if (InputHelper.InputCancelPressed())
            {
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
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawBox(g, new Vector2(-5, -5), Constants.Width + 10, Constants.Height + 10, Color.White);

            DrawBookCards(g, Constants.Height / 6 - ScrollbarIndex);

            float X = -10;
            float Y = Constants.Height / 20;
            DrawBox(g, new Vector2(X, Y), Constants.Width + 20, HeaderHeight, Color.White);

            X = Constants.Width / 20;
            Y += HeaderHeight / 2 - fntArial12.LineSpacing / 2;
            g.DrawString(fntArial12, "Book Edit", new Vector2(X, Y), Color.White);
            if (ActivePlayer != null)
            {
                g.DrawStringMiddleAligned(fntArial12, ActivePlayer.Name + "/" + ActiveBook.BookName, new Vector2(Constants.Width / 2, Y), Color.White);
            }
            X = Constants.Width - Constants.Width / 8;
            g.DrawStringRightAligned(fntArial12, ActiveBook.TotalCards + " card(s)", new Vector2(X, Y), Color.White);
            g.DrawString(fntArial12, "OK", new Vector2(X + 20, Y), Color.White);

            int CursorX = Constants.Width / 2 - CardWidth / 2 - (CardWidth + CardSpacing) * 3 + (CardWidth + CardSpacing) * (CursorIndex % 7) - 50;
            int CursorY = Constants.Height / 6 + CardHeight/ 2 + (CardHeight + 20) * (CursorIndex / 7);
            MenuHelper.DrawFingerIcon(g, new Vector2(CursorX, CursorY - ScrollbarIndex));

            X = -10;
            Y = Constants.Height - Constants.Height / 20 - HeaderHeight;
            DrawBox(g, new Vector2(X, Y), Constants.Width + 20, HeaderHeight, Color.White);
            X = Constants.Width / 18;
            Y += HeaderHeight / 2 - fntArial12.LineSpacing / 2;
            g.DrawString(fntArial12, "Edit this Book's contents", new Vector2(X, Y), Color.White);

            MissionScrollbar.Draw(g);
        }

        private void DrawBookCards(CustomSpriteBatch g, float StartY)
        {
            int CardNumberBoxWidth = (int)(CardWidth / 3.2f);

            for (int C = 0; C < ListFilteredCard.Count; ++C)
            {
                float X = Constants.Width / 2 - CardWidth / 2 - (CardWidth + CardSpacing) * 3 + (CardWidth + CardSpacing) * (C % CardsPerLine);
                float Y = StartY + (CardHeight + 20) * (C / CardsPerLine);

                g.Draw(ListFilteredCard[C].sprCard, new Rectangle((int)X, (int)Y, CardWidth, CardHeight), new Rectangle(0, 0, ListFilteredCard[C].sprCard.Width, ListFilteredCard[C].sprCard.Height), Color.White);
                DrawBox(g, new Vector2(X + CardWidth / 2 - CardNumberBoxWidth / 2, Y + CardHeight - CardNumberBoxWidth / 2), CardNumberBoxWidth, CardNumberBoxWidth, Color.White);
                TextHelper.DrawTextMiddleAligned(g, ListFilteredCard[C].QuantityOwned.ToString(), new Vector2(X + CardWidth / 2, Y + 3 + CardHeight - CardNumberBoxWidth / 2), Color.White);
            }
        }
    }
}
