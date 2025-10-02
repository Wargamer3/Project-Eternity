using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class Forge : GameScreen
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

        int Stage;

        public Forge()
            : base()
        {
            CursorIndex = 0;
            CursorIndexMax = 0;
            CursorIndexStart = 0;
        }

        public override void Load()
        {
            sprRectangle = Content.Load<Texture2D>("Pixel");
            sprBackground = Content.Load<Texture2D>("Deathmatch/Intermission Screens/Forge");
            fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
            fntArial10 = Content.Load<SpriteFont>("Fonts/Arial10");
            MainFilter = new FilterItem("", new List<ShopItem>(), new List<FilterItem>());
            MainFilter.ListFilter.Add(new FilterItem("Common parts", new List<ShopItem>(), new List<FilterItem>()));
            MainFilter.ListFilter.Add(new FilterItem("Custom parts", new List<ShopItem>(), new List<FilterItem>()));
            MainFilter.ListFilter.Add(new FilterItem("New Mechs", new List<ShopItem>(), new List<FilterItem>()));
            MainFilter.ListFilter.Add(new FilterItem("Mech Upgrades", new List<ShopItem>(), new List<FilterItem>()));
            List<ShopItem> ListItem = new List<ShopItem>();
            MainFilter.ListFilter[0].ListItem.Add(new Blueprint("Some Blueprint", "Create something", 1000, new List<Blueprint.ItemRequirement>(), null));

            MainFilter.IsOpen = true;
            MainFilter.CursorIndex = 0;

            CursorFilter = MainFilter;
            SetCursorIndexMax(MainFilter);

            Stage = 0;
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
                if (Stage == 0)
                {
                    CursorIndex--;
                    if (CursorIndex < 0)
                    {
                        CursorIndex = CursorIndexMax - 1;
                        if (CursorIndex >= 32)
                            CursorIndexStart = CursorIndexMax - 31;
                    }
                    if (CursorIndex < CursorIndexStart)
                        CursorIndexStart--;
                    //Reset the CursorFilter.
                    int CursorPos = CursorIndex;
                    ResetCursorFilter(MainFilter);
                    GetCursorFilter(MainFilter, ref CursorPos);
                }
            }
            else if (InputHelper.InputDownPressed())
            {
                if (Stage == 0)
                {
                    CursorIndex++;
                    if (CursorIndex > CursorIndexMax - 1)
                    {
                        CursorIndex = 0;
                        CursorIndexStart = 0;
                    }
                    if (CursorIndex > 32 + CursorIndexStart)
                        CursorIndexStart++;
                    //Reset the CursorFilter.
                    int CursorPos = CursorIndex;
                    ResetCursorFilter(MainFilter);
                    GetCursorFilter(MainFilter, ref CursorPos);
                }
            }
            else if (InputHelper.InputConfirmPressed())
            {
                if (Stage == 0)
                {
                    //If the cursor points on a FilterItem.
                    if (CursorFilter.CursorIndex >= CursorFilter.ListItem.Count)
                    {//Open/Close the selected FilterItem.
                        CursorFilter.ListFilter[CursorFilter.CursorIndex - CursorFilter.ListItem.Count].IsOpen = !CursorFilter.ListFilter[CursorFilter.CursorIndex - CursorFilter.ListItem.Count].IsOpen;
                        //Reset the CursorIndexMax.
                        CursorIndexMax = 0;
                        SetCursorIndexMax(MainFilter);
                    }
                    else
                    {
                        Stage++;
                    }
                }
                else
                {

                }
            }
            else if (InputHelper.InputCancelPressed())
            {
                if (Stage == 0)
                {
                    RemoveScreen(this);
                }
                else
                {
                    Stage--;
                }
            }
            else if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
                RemoveScreen(this);
        }
        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprBackground, new Vector2(0, 0), Color.White);
            if (Stage == 0)
            {
                g.Draw(sprRectangle, new Rectangle(320, 40, 320, 440), Color.FromNonPremultiplied(255, 255, 255, 100));
            }
            else if (Stage == 1)
            {
                g.Draw(sprRectangle, new Rectangle(0, 297, 320, 183), Color.FromNonPremultiplied(255, 255, 255, 200));
            }
            //Draw the item list with its filters and get the lowest Y position.
            int Index = 0;
            int Y = DrawFilter(g, MainFilter, 305, 41, ref Index);
            g.DrawString(fntArial10, "Requirements:", new Vector2(10, 315), Color.White);
            //Item description
            if (CursorFilter.CursorIndex >= 0 && CursorFilter.CursorIndex < CursorFilter.ListItem.Count)
            {
                g.DrawString(fntArial10, CursorFilter.ListItem[CursorFilter.CursorIndex].RelativePath, new Vector2(10, 45), Color.White);
                g.DrawString(fntArial10, '"' + CursorFilter.ListItem[CursorFilter.CursorIndex].RelativePath + '"' + " Blueprint", new Vector2(50, 330), Color.White);
                Blueprint Item = (Blueprint)CursorFilter.ListItem[CursorFilter.CursorIndex];
                for (int i = 0; i < Item.ListRequirement.Count; i++)
                {

                }
            }
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
    }
}
