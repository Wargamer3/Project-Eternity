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
        #region Ressources

        private SpriteFont fntArial12;

        public Texture2D sprMenuCursor;

        #endregion

        private BoxScrollbar MissionScrollbar;

        private const int CardSpacing = 7;
        private const int CardWidth = 85;
        private const int CardHeight = 110;
        private int HeaderHeight = Constants.Height / 16;

        private readonly Player ActivePlayer;
        private readonly CardBook ActiveBook;
        private readonly List<Card> ListFilteredCard;

        private int CursorIndex;
        private int ScrollbarIndex;

        public EditBookCardListFilterScreen(Player ActivePlayer, CardBook ActiveBook, string Filter)
        {
            this.ActivePlayer = ActivePlayer;
            this.ActiveBook = ActiveBook;
            ListFilteredCard = new List<Card>();
            FillCardList(Filter);
        }

        private void FillCardList(string Filter)
        {
            switch (Filter)
            {
                #region Creatures

                case "creatures":
                    foreach (Card ActiveCard in ActivePlayer.Inventory.GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            ListFilteredCard.Add(ActiveCreatureCard);
                        }
                    }
                    break;

                case "neutral":
                    foreach (Card ActiveCard in ActivePlayer.Inventory.GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.ArrayAffinity.Length == 1 && ActiveCreatureCard.ArrayAffinity[0] == CreatureCard.ElementalAffinity.Neutral)
                            {
                                ListFilteredCard.Add(ActiveCreatureCard);
                            }
                        }
                    }
                    break;

                case "fire":
                    foreach (Card ActiveCard in ActivePlayer.Inventory.GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.ArrayAffinity.Length == 1 && ActiveCreatureCard.ArrayAffinity[0] == CreatureCard.ElementalAffinity.Fire)
                            {
                                ListFilteredCard.Add(ActiveCreatureCard);
                            }
                        }
                    }
                    break;

                case "water":
                    foreach (Card ActiveCard in ActivePlayer.Inventory.GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.ArrayAffinity.Length == 1 && ActiveCreatureCard.ArrayAffinity[0] == CreatureCard.ElementalAffinity.Water)
                            {
                                ListFilteredCard.Add(ActiveCreatureCard);
                            }
                        }
                    }
                    break;

                case "earth":
                    foreach (Card ActiveCard in ActivePlayer.Inventory.GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.ArrayAffinity.Length == 1 && ActiveCreatureCard.ArrayAffinity[0] == CreatureCard.ElementalAffinity.Earth)
                            {
                                ListFilteredCard.Add(ActiveCreatureCard);
                            }
                        }
                    }
                    break;

                case "air":
                    foreach (Card ActiveCard in ActivePlayer.Inventory.GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.ArrayAffinity.Length == 1 && ActiveCreatureCard.ArrayAffinity[0] == CreatureCard.ElementalAffinity.Air)
                            {
                                ListFilteredCard.Add(ActiveCreatureCard);
                            }
                        }
                    }
                    break;

                case "multi":
                    foreach (Card ActiveCard in ActivePlayer.Inventory.GlobalBook.ListCard)
                    {
                        CreatureCard ActiveCreatureCard = ActiveCard as CreatureCard;
                        if (ActiveCreatureCard != null)
                        {
                            if (ActiveCreatureCard.ArrayAffinity.Length > 1)
                            {
                                ListFilteredCard.Add(ActiveCreatureCard);
                            }
                        }
                    }
                    break;

                    #endregion
            }
        }

        public override void Load()
        {
            float MaxY = Constants.Height / 6 + 130 * 2 + (CardHeight + 20) * ((ActiveBook.ListCard.Count - 14) / 7);
            MissionScrollbar = new BoxScrollbar(new Vector2(Constants.Width - 20, Constants.Height / 6), Constants.Height - Constants.Height / 3, MaxY, OnMissionScrollbarChange);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            sprMenuCursor = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/Cursor");
        }

        private void OnMissionScrollbarChange(float ScrollbarValue)
        {
            ScrollbarIndex = (int)ScrollbarValue;
        }

        public override void Update(GameTime gameTime)
        {
            MissionScrollbar.Update(gameTime);

            if (InputHelper.InputConfirmPressed())
            {
                Card SelectedCard = ListFilteredCard[CursorIndex];
                if (!ActiveBook.DicCardsByType.ContainsKey(SelectedCard.CardType) || !ActiveBook.DicCardsByType[SelectedCard.CardType].ContainsKey(SelectedCard.Path))
                {
                    Card CopyCard = ActivePlayer.Inventory.GlobalBook.DicCardsByType[SelectedCard.CardType][SelectedCard.Path].Copy(PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                    CopyCard.QuantityOwned = 0;
                    ActiveBook.AddCard(CopyCard);
                }

                PushScreen(new EditBookCardScreen(ActivePlayer, ActiveBook, ActiveBook.DicCardsByType[SelectedCard.CardType][SelectedCard.Path]));
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
                    CursorIndex -= CursorIndex % 7;
                }
            }
            else if (InputHelper.InputLeftPressed())
            {
                CursorIndex -= 1;
                if (CursorIndex < 0)
                {
                    CursorIndex = 6;
                }
                else if (CursorIndex % 7 == 6)
                {
                    if (CursorIndex + 7 >= ListFilteredCard.Count)
                    {
                        CursorIndex = ListFilteredCard.Count - 1;
                    }
                    else
                    {
                        CursorIndex += 7;
                    }
                }
            }
            else if (InputHelper.InputDownPressed())
            {
                if (CursorIndex + 7 >= ListFilteredCard.Count)
                {
                    CursorIndex = CursorIndex % 7;
                }
                else
                {
                    CursorIndex += 7;

                    if (CursorIndex > ListFilteredCard.Count)
                    {
                        CursorIndex -= 7;
                    }
                }

                int CursorY = (CardHeight + 20) * (CursorIndex / 7);

                if (CursorY < ScrollbarIndex)
                {
                    ScrollbarIndex = (CardHeight + 20) * (CursorIndex / 7);
                }
                else if (CursorY > ScrollbarIndex + (CardHeight + 20) * 2)
                {
                    ScrollbarIndex = (CardHeight + 20) * ((CursorIndex / 7) - 2);
                }
            }
            else if (InputHelper.InputUpPressed())
            {
                CursorIndex -= 7;
                if (CursorIndex < 0)
                {
                    if ((CursorIndex + 7) % 7 >= ListFilteredCard.Count % 7)
                    {
                        CursorIndex = (ListFilteredCard.Count / 7 - 1) * 7 + (CursorIndex + 7) % 7;
                    }
                    else
                    {
                        CursorIndex = (ListFilteredCard.Count / 7) * 7 + (CursorIndex + 7) % 7;
                    }
                }

                int CursorY = (CardHeight + 20) * (CursorIndex / 7);

                if (CursorY < ScrollbarIndex)
                {
                    ScrollbarIndex = (CardHeight + 20) * (CursorIndex / 7);
                }
                else if (CursorY > ScrollbarIndex + (CardHeight + 20) * 2)
                {
                    ScrollbarIndex = (CardHeight + 20) * ((CursorIndex / 7) - 2);
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
            g.DrawStringMiddleAligned(fntArial12, ActivePlayer.Name + "/" + ActiveBook.BookName, new Vector2(Constants.Width / 2, Y), Color.White);
            X = Constants.Width - Constants.Width / 8;
            g.DrawStringRightAligned(fntArial12, ActiveBook.TotalCards + " card(s)", new Vector2(X, Y), Color.White);
            g.DrawString(fntArial12, "OK", new Vector2(X + 20, Y), Color.White);

            int CursorX = Constants.Width / 2 - CardWidth / 2 - (CardWidth + CardSpacing) * 3 + (CardWidth + CardSpacing) * (CursorIndex % 7);
            int CursorY = Constants.Height / 6 + CardHeight/ 2 + (CardHeight + 20) * (CursorIndex / 7);
            g.Draw(sprMenuCursor, new Rectangle(CursorX, CursorY - ScrollbarIndex, 40, 40), Color.White);

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
                float X = Constants.Width / 2 - CardWidth / 2 - (CardWidth + CardSpacing) * 3 + (CardWidth + CardSpacing) * (C % 7);
                float Y = StartY + (CardHeight + 20) * (C / 7);

                g.Draw(ListFilteredCard[C].sprCard, new Rectangle((int)X, (int)Y, CardWidth, CardHeight), new Rectangle(0, 0, ListFilteredCard[C].sprCard.Width, ListFilteredCard[C].sprCard.Height), Color.White);
                DrawBox(g, new Vector2(X + CardWidth / 2 - CardNumberBoxWidth / 2, Y + CardHeight - CardNumberBoxWidth / 2), CardNumberBoxWidth, CardNumberBoxWidth, Color.White);
                TextHelper.DrawTextMiddleAligned(g, ListFilteredCard[C].QuantityOwned.ToString(), new Vector2(X + CardWidth / 2, Y + 3 + CardHeight - CardNumberBoxWidth / 2), Color.White);
            }
        }
    }
}
