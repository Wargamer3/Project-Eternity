using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens;

namespace Project_Eternity
{
    public sealed class Shop : GameScreen
    {
        Texture2D sprRectangle;
        Texture2D sprBackground;
        SpriteFont fntArial8;
        SpriteFont fntArial10;

        int CursorAlpha;
        bool CursorAppearing;
        int CursorIndex;
        int CursorIndexMax;
        int CursorIndexStart;//Starting position of the cursor from where to start drawing.
        FilterItem CursorFilter;
        FilterItem MainFilter;

        int CheckOutAmount;

        public Shop()
            : base()
        {
            CursorIndex = 0;
            CursorIndexMax = 0;
            CursorIndexStart = 0;
            CheckOutAmount = 0;
        }

        public override void Load()
        {
            sprRectangle = Content.Load<Texture2D>("Rectangle");
            sprBackground = Content.Load<Texture2D>("Intermission Screens/Shop");
            fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
            fntArial10 = Content.Load<SpriteFont>("Fonts/Arial10");
            MainFilter = new FilterItem("", new List<ShopItem>(), new List<FilterItem>());
            List<ShopItem> ListItem = new List<ShopItem>();
            ListItem.Add((Unit)Unit.ListUnit.ElementAt(0).Value.LoadItem());
            MainFilter.ListItem.Add((Unit)Unit.ListUnit.ElementAt(0).Value.LoadItem());
            MainFilter.ListFilter.Add(new FilterItem("Mechs0", ListItem, new List<FilterItem>()));
            MainFilter.ListFilter.Add(new FilterItem("Mechs1", ListItem, new List<FilterItem>()));
            MainFilter.ListFilter.Add(new FilterItem("Mechs2", ListItem, new List<FilterItem>()));
            MainFilter.ListFilter.Add(new FilterItem("Mechs3", ListItem, new List<FilterItem>()));
            MainFilter.ListFilter.Add(new FilterItem("Mechs4", ListItem, new List<FilterItem>()));
            MainFilter.ListFilter[0].ListFilter.Add(new FilterItem("Mechs3", ListItem, new List<FilterItem>()));

            MainFilter.IsOpen = true;
            MainFilter.CursorIndex = 0;
            MainFilter.ListFilter[0].IsOpen = true;
            MainFilter.ListFilter[0].ListFilter[0].IsOpen = true;
            CursorFilter = MainFilter;
            SetCursorIndexMax(MainFilter);
        }
        public override void Update(GameTime gameTime)
        {
            //Change the alpha of the cursor.
            if (CursorAppearing)
            {
                if (++CursorAlpha >= 200)
                    CursorAppearing = false;
            }
            else
            {
                if (--CursorAlpha <= 70)
                    CursorAppearing = true;
            }
            if (KeyboardHelper.InputUpPressed())
            {
                CursorIndex--;
                if (CursorIndex < 0)
                {
                    CursorIndex = CursorIndexMax;
                    if (CursorIndex > 32)
                        CursorIndexStart = CursorIndexMax - 31;
                }
                if (CursorIndex < CursorIndexStart)
                    CursorIndexStart--;
                //Reset the CursorFilter.
                int CursorPos = CursorIndex;
                ResetCursorFilter(MainFilter);
                GetCursorFilter(MainFilter, ref CursorPos);
            }
            else if (KeyboardHelper.InputDownPressed())
            {
                CursorIndex++;
                if (CursorIndex > CursorIndexMax)
                {
                    CursorIndex = 0;
                    CursorIndexStart = 0;
                }
                if (CursorIndex >= 32 + CursorIndexStart)
                    CursorIndexStart++;
                //Reset the CursorFilter.
                int CursorPos = CursorIndex;
                ResetCursorFilter(MainFilter);
                GetCursorFilter(MainFilter, ref CursorPos);
            }
            if (KeyboardHelper.InputRightPressed())
            {
                //If the cursor points on an Item and the item can be bought and is not on Check Out.
                if (CursorFilter.CursorIndex < CursorFilter.ListItem.Count && CursorFilter.ListItem[CursorFilter.CursorIndex].QuantityToBuy + 1 <= CursorFilter.ListItem[CursorFilter.CursorIndex].Quantity && CursorIndex != CursorIndexMax)
                {
                    CursorFilter.ListItem[CursorFilter.CursorIndex].QuantityToBuy++;
                    CheckOutAmount += CursorFilter.ListItem[CursorFilter.CursorIndex].Price;
                }
            }
            else if (KeyboardHelper.InputLeftPressed())
            {
                //If the cursor points on an Item and the Item have at least 1 Quantity to buy and is not on Check Out.
                if (CursorFilter.CursorIndex < CursorFilter.ListItem.Count && CursorFilter.ListItem[CursorFilter.CursorIndex].QuantityToBuy - 1 >= 0 && CursorIndex != CursorIndexMax)
                {
                    CursorFilter.ListItem[CursorFilter.CursorIndex].QuantityToBuy--;
                    CheckOutAmount -= CursorFilter.ListItem[CursorFilter.CursorIndex].Price;
                }
            }
            else if (KeyboardHelper.InputConfirmPressed())
            {//If the cursor is not on Check Out
                if (CursorIndex != CursorIndexMax)
                {
                    //If the cursor points on a FilterItem.
                    if (CursorFilter.CursorIndex >= CursorFilter.ListItem.Count)
                    {//Open/Close the selected FilterItem.
                        CursorFilter.ListFilter[CursorFilter.CursorIndex - CursorFilter.ListItem.Count].IsOpen = !CursorFilter.ListFilter[CursorFilter.CursorIndex - CursorFilter.ListItem.Count].IsOpen;
                        //Reset the CursorIndexMax.
                        CursorIndexMax = 0;
                        SetCursorIndexMax(MainFilter);
                    }
                }
                else
                {
                    if (Game.Money - CheckOutAmount >= 0)
                    {
                        BuyItem(MainFilter);
                        Game.Money -= CheckOutAmount;
                        CheckOutAmount = 0;
                    }
                }
            }
            else if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
                GameScreen.RemoveScreen(this);
        }
        public override void Draw(SpriteBatch g)
        {
            g.Draw(sprBackground, new Vector2(0, 0), Color.White);
            //Draw the item list with its filters and get the lowest Y position.
            int Index = 0;
            int Y = DrawFilter(g, MainFilter, 305, 41, ref Index);
            if (Y < Game.Height - 60)
            {
                g.DrawString(fntArial10, "Check Out", new Vector2(325, Y + fntArial10.LineSpacing), Color.White);
                if (CursorIndex == CursorIndexMax)
                    g.Draw(sprRectangle, new Rectangle(325, Y + fntArial10.LineSpacing, 300, fntArial10.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
            }
            //Item description
            if (CursorFilter.CursorIndex >= 0 && CursorFilter.CursorIndex < CursorFilter.ListItem.Count)
            {
                g.DrawString(fntArial10, CursorFilter.ListItem[CursorFilter.CursorIndex].Name, new Vector2(10, 45), Color.White);
                g.DrawString(fntArial10, "Cost: " + CursorFilter.ListItem[CursorFilter.CursorIndex].Price, new Vector2(10, 45 + fntArial10.LineSpacing * 2), Color.White);
                g.DrawString(fntArial10, CursorFilter.ListItem[CursorFilter.CursorIndex].Quantity + " in stock, " + CursorFilter.ListItem[CursorFilter.CursorIndex].QuantityOwned + " Owned", new Vector2(10, 45 + fntArial10.LineSpacing * 3), Color.White);
                //Bottom
                //Item cost * number of item you buy.
                g.DrawString(fntArial10, (CursorFilter.ListItem[CursorFilter.CursorIndex].Price * CursorFilter.ListItem[CursorFilter.CursorIndex].QuantityToBuy).ToString(), new Vector2(170, Game.Height - 60), Color.White, 0, new Vector2(fntArial10.MeasureString((CursorFilter.ListItem[CursorFilter.CursorIndex].Price * CursorFilter.ListItem[CursorFilter.CursorIndex].QuantityToBuy).ToString()).X, 0), 1, SpriteEffects.None, 0);
            }
            else//Item cost (None selected).
                g.DrawString(fntArial10, "0", new Vector2(170, Game.Height - 60), Color.White, 0, new Vector2(fntArial10.MeasureString("0").X, 0), 1, SpriteEffects.None, 0);
            //Bottom.
            g.DrawString(fntArial10, Game.Money.ToString(), new Vector2(2, Game.Height - 60), Color.White);
            //Remaining funds
            if (Game.Money - CheckOutAmount >= 0)
                g.DrawString(fntArial10, (Game.Money - CheckOutAmount).ToString(), new Vector2(300, Game.Height - 60), Color.White, 0, new Vector2(fntArial10.MeasureString((Game.Money - CheckOutAmount).ToString()).X, 0), 1, SpriteEffects.None, 0);
            else
                g.DrawString(fntArial10, (Game.Money - CheckOutAmount).ToString(), new Vector2(300, Game.Height - 60), Color.Red, 0, new Vector2(fntArial10.MeasureString((Game.Money - CheckOutAmount).ToString()).X, 0), 1, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Set the CursorIndex of a FilterItem and its child to -1.
        /// </summary>
        /// <param name="Filter">FilterItem to reset.</param>
        private void ResetCursorFilter(FilterItem Filter)
        {
            Filter.CursorIndex = -1;
            //Reset its child.
            for (int i = 0; i < Filter.ListFilter.Count; i++)
                ResetCursorFilter(Filter.ListFilter[i]);
        }
        /// <summary>
        /// Give CursorFilter a value based on a CursorPos and a FilterItem.
        /// </summary>
        /// <param name="Filter">FilterItem to calculate from.</param>
        /// <param name="CursorPos">Index of the cursor to use.</param>
        /// <returns></returns>
        private bool GetCursorFilter(FilterItem Filter, ref int CursorPos)
        {
            if (Filter.IsOpen)
            {
                //If the CursorPos is lower then the count of ShopItem.
                if (CursorPos < Filter.ListItem.Count)
                {//Set the CursorFilter and its CursorIndex.
                    CursorFilter = Filter;
                    CursorFilter.CursorIndex = CursorPos;
                    return true;
                }
                //Decrement CursorPos of the number of ShopItem in the ListItem.
                CursorPos -= Filter.ListItem.Count;
                //Loop through the ListFilter to get any child FilterItem.
                for (int F = 0; F < Filter.ListFilter.Count; F++)
                {//If the CursorPos didn't reached 0.
                    if (CursorPos > 0)
                    {
                        CursorPos--;
                        //Move in the child FilterItem, if it suceed then the CursorFitler is set and there's no need to keep on.
                        if (GetCursorFilter(Filter.ListFilter[F], ref CursorPos))
                            return true;
                    }
                    //Else if the CursorPos reached 0
                    else if (CursorPos == 0)
                    {//Set the CursorFilter and its CursorIndex.
                        CursorFilter = Filter;
                        CursorFilter.CursorIndex = F + Filter.ListItem.Count;
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Set the CursorIndexMax by counting the content of a FilterItem.
        /// </summary>
        /// <param name="Filter">FilterItem to calculate.</param>
        private void SetCursorIndexMax(FilterItem Filter)
        {
            //Add the size of the ListItem and the ListFilter.
            CursorIndexMax += Filter.ListItem.Count + Filter.ListFilter.Count;
            //Loop through every FilterItem in the ListFilter to add them to the CursorIndexMax.
            for (int F = 0; F < Filter.ListFilter.Count; F++)
                if (Filter.ListFilter[F].IsOpen)
                    SetCursorIndexMax(Filter.ListFilter[F]);
        }
        /// <summary>
        /// Start drawing A FilterItem and its content at a given position.
        /// </summary>
        /// <param name="g">SpriteBatch to draw on.</param>
        /// <param name="Filter">Filter to draw.</param>
        /// <param name="X">X Position to start drawing.</param>
        /// <param name="Y">Y Position to start drawing.</param>
        /// <returns></returns>
        private int DrawFilter(SpriteBatch g, FilterItem Filter, int X, int Y, ref int Index)
        {
            if (Filter.IsOpen)
            {//Loop through every ShopItem.
                for (int i = 0; i < Filter.ListItem.Count; i++)
                {
                    if (Index >= CursorIndexStart && Index < 32 + CursorIndexStart)
                    {
                        //Draw the name,
                        g.DrawString(fntArial8, Filter.ListItem[i].Name, new Vector2(X + 20, Y), Color.White);
                        //Draw the quantity related informations.
                        g.DrawString(fntArial8, Filter.ListItem[i].QuantityToBuy + " / " + Filter.ListItem[i].Quantity + " (" + Filter.ListItem[i].QuantityOwned + ")", new Vector2(Game.Width - 60, Y), Color.Aqua);
                        //If the current ShopItem is selected, highlight it.
                        if (i == Filter.CursorIndex)
                            g.Draw(sprRectangle, new Rectangle(X + 16, Y, (int)fntArial8.MeasureString(Filter.ListItem[i].Name).X + 10, fntArial8.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                        Y += fntArial8.LineSpacing;
                    }
                    Index++;
                }
                //Loop through every FilterItem.
                for (int F = 0; F < Filter.ListFilter.Count; F++)
                {
                    if (Index >= CursorIndexStart && Index < 32 + CursorIndexStart)
                    {
                        //If it's open
                        if (Filter.ListFilter[F].IsOpen)
                            g.DrawString(fntArial8, "- " + Filter.ListFilter[F].Name, new Vector2(X + 20, Y), Color.White);
                        else
                            g.DrawString(fntArial8, "+ " + Filter.ListFilter[F].Name, new Vector2(X + 20, Y), Color.White);
                        //If the current FilterItem is selected, highlight it.
                        if (F == Filter.CursorIndex - Filter.ListItem.Count)
                            g.Draw(sprRectangle, new Rectangle(X + 16, Y, (int)fntArial8.MeasureString("+ " + Filter.ListFilter[F].Name).X + 10, fntArial8.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                        Y += fntArial8.LineSpacing;
                    }
                    Index++;
                        //Loop in the filter so it draws itself if open.
                        Y = DrawFilter(g, Filter.ListFilter[F], X + 20, Y, ref Index);
                    
                }
            }
            return Y;
        }

        private void BuyItem(FilterItem Filter)
        {
            for (int I = 0; I < Filter.ListItem.Count; I++)
            {
                Filter.ListItem[I].Quantity -= Filter.ListItem[I].QuantityToBuy;
                Filter.ListItem[I].QuantityOwned += Filter.ListItem[I].QuantityToBuy;
                Filter.ListItem[I].QuantityToBuy = 0;
            }
            //Reset its child.
            for (int i = 0; i < Filter.ListFilter.Count; i++)
                BuyItem(Filter.ListFilter[i]);
        }
    }
}
