using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class Shop : GameScreen
    {
        private Texture2D sprRectangle;
        private Texture2D sprBackground;
        private SpriteFont fntArial8;
        private SpriteFont fntArial10;

        private readonly BattleMapPlayer Player;

        private int CursorAlpha;
        private bool CursorAppearing;
        private int CursorIndex;
        private int CursorIndexMax;
        private int CursorIndexStart;//Starting position of the cursor from where to start drawing.
        private FilterItem CursorFilter;
        private FilterItem MainFilter;

        private int CheckOutAmount;

        public Shop(BattleMapPlayer Player)
            : base()
        {
            this.Player = Player;

            CursorIndex = 0;
            CursorIndexMax = 0;
            CursorIndexStart = 0;
            CheckOutAmount = 0;
        }

        public override void Load()
        {
            sprRectangle = Content.Load<Texture2D>("Pixel");
            sprBackground = Content.Load<Texture2D>("Menus/Intermission Screens/Shop");
            fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
            fntArial10 = Content.Load<SpriteFont>("Fonts/Arial10");
            MainFilter = new FilterItem("", new List<ShopItem>(), new List<FilterItem>());
            List<ShopItem> ListItem = new List<ShopItem>();
            //ListItem.Add((Unit)Unit.ListUnit.ElementAt(0).Value.LoadItem());
            //MainFilter.ListItem.Add((Unit)Unit.ListUnit.ElementAt(0).Value.LoadItem());
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
            if (InputHelper.InputUpPressed())
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
            else if (InputHelper.InputDownPressed())
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
            if (InputHelper.InputRightPressed())
            {
                //If the cursor points on an Item and the item can be bought and is not on Check Out.
                if (CursorFilter.CursorIndex < CursorFilter.ListItem.Count && CursorFilter.ListItem[CursorFilter.CursorIndex].QuantityToBuy + 1 <= CursorFilter.ListItem[CursorFilter.CursorIndex].Quantity && CursorIndex != CursorIndexMax)
                {
                    CursorFilter.ListItem[CursorFilter.CursorIndex].QuantityToBuy++;
                    CheckOutAmount += CursorFilter.ListItem[CursorFilter.CursorIndex].Price;
                }
            }
            else if (InputHelper.InputLeftPressed())
            {
                //If the cursor points on an Item and the Item have at least 1 Quantity to buy and is not on Check Out.
                if (CursorFilter.CursorIndex < CursorFilter.ListItem.Count && CursorFilter.ListItem[CursorFilter.CursorIndex].QuantityToBuy - 1 >= 0 && CursorIndex != CursorIndexMax)
                {
                    CursorFilter.ListItem[CursorFilter.CursorIndex].QuantityToBuy--;
                    CheckOutAmount -= CursorFilter.ListItem[CursorFilter.CursorIndex].Price;
                }
            }
            else if (InputHelper.InputConfirmPressed())
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
                    if (Player.Records.CurrentMoney - CheckOutAmount >= 0)
                    {
                        BuyItem(MainFilter);
                        Player.Records.CurrentMoney -= (uint)CheckOutAmount;
                        CheckOutAmount = 0;
                    }
                }
            }
            else if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
                RemoveScreen(this);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprBackground, new Vector2(0, 0), Color.White);
            //Draw the item list with its filters and get the lowest Y position.
            int Index = 0;
            int Y = DrawFilter(g, MainFilter, 305, 41, ref Index);
            if (Y < Constants.Height - 60)
            {
                g.DrawString(fntArial10, "Check Out", new Vector2(325, Y + fntArial10.LineSpacing), Color.White);
                if (CursorIndex == CursorIndexMax)
                    g.Draw(sprRectangle, new Rectangle(325, Y + fntArial10.LineSpacing, 300, fntArial10.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
            }
            //Item description
            if (CursorFilter.CursorIndex >= 0 && CursorFilter.CursorIndex < CursorFilter.ListItem.Count)
            {
                g.DrawString(fntArial10, CursorFilter.ListItem[CursorFilter.CursorIndex].RelativePath, new Vector2(10, 45), Color.White);
                g.DrawString(fntArial10, "Cost: " + CursorFilter.ListItem[CursorFilter.CursorIndex].Price, new Vector2(10, 45 + fntArial10.LineSpacing * 2), Color.White);
                int QuantityOwned = 0;
                if (Inventory.ListItems.ContainsKey(CursorFilter.ListItem[CursorFilter.CursorIndex].RelativePath))
                {
                    QuantityOwned = Inventory.ListItems[CursorFilter.ListItem[CursorFilter.CursorIndex].RelativePath].Quantity;
                }
                g.DrawString(fntArial10, CursorFilter.ListItem[CursorFilter.CursorIndex].Quantity + " in stock, " + QuantityOwned + " Owned", new Vector2(10, 45 + fntArial10.LineSpacing * 3), Color.White);
                //Bottom
                //Item cost * number of item you buy.
                g.DrawStringRightAligned(fntArial10, (CursorFilter.ListItem[CursorFilter.CursorIndex].Price * CursorFilter.ListItem[CursorFilter.CursorIndex].QuantityToBuy).ToString(), new Vector2(170, Constants.Height - 60), Color.White);
            }
            else//Item cost (None selected).
                g.DrawStringRightAligned(fntArial10, "0", new Vector2(170, Constants.Height - 60), Color.White);
            //Bottom.
            g.DrawString(fntArial10, Player.Records.CurrentMoney.ToString(), new Vector2(2, Constants.Height - 60), Color.White);
            //Remaining funds
            if (Player.Records.CurrentMoney - CheckOutAmount >= 0)
                g.DrawStringRightAligned(fntArial10, (Player.Records.CurrentMoney - CheckOutAmount).ToString(), new Vector2(300, Constants.Height - 60), Color.White);
            else
                g.DrawStringRightAligned(fntArial10, (Player.Records.CurrentMoney - CheckOutAmount).ToString(), new Vector2(300, Constants.Height - 60), Color.Red);
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
        private int DrawFilter(CustomSpriteBatch g, FilterItem Filter, int X, int Y, ref int Index)
        {
            if (Filter.IsOpen)
            {//Loop through every ShopItem.
                for (int i = 0; i < Filter.ListItem.Count; i++)
                {
                    if (Index >= CursorIndexStart && Index < 32 + CursorIndexStart)
                    {
                        //Draw the name,
                        g.DrawString(fntArial8, Filter.ListItem[i].RelativePath, new Vector2(X + 20, Y), Color.White);
                        //Draw the quantity related informations.
                        int QuantityOwned = 0;
                        if (Inventory.ListItems.ContainsKey(Filter.ListItem[i].RelativePath))
                        {
                            QuantityOwned = Inventory.ListItems[Filter.ListItem[i].RelativePath].Quantity;
                        }
                        g.DrawString(fntArial8, Filter.ListItem[i].QuantityToBuy + " / " + Filter.ListItem[i].Quantity + " (" + QuantityOwned + ")", new Vector2(Constants.Width - 60, Y), Color.Aqua);
                        //If the current ShopItem is selected, highlight it.
                        if (i == Filter.CursorIndex)
                            g.Draw(sprRectangle, new Rectangle(X + 16, Y, (int)fntArial8.MeasureString(Filter.ListItem[i].RelativePath).X + 10, fntArial8.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
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
                Inventory.ListItems[Filter.ListItem[I].RelativePath].Quantity += Filter.ListItem[I].QuantityToBuy;
                Filter.ListItem[I].QuantityToBuy = 0;
            }
            //Reset its child.
            for (int i = 0; i < Filter.ListFilter.Count; i++)
                BuyItem(Filter.ListFilter[i]);
        }
    }
}
