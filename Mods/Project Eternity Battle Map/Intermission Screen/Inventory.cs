using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class Inventory : GameScreen
    {
        private enum SortType { Type = 0, Alphabetical = 1 };
        SortType Sort;
        Texture2D sprRectangle;
        SpriteFont fntArial8;
        SpriteFont fntArial12;
        bool BlueprintIsOpen;
        bool ConsumablesIsOpen;
        bool UnitPartsIsOpen;
        bool UnitsIsOpen;
        bool ScrapPartsIsOpen;
        List<Blueprint> ListBlueprints;
        List<Consumables> ListConsumables;
        List<UnitParts> ListUnitParts;
        List<Units> ListUnits;
        List<ScrapParts> ListScrapParts;

        public static Dictionary<string, ShopItem> ListItems = new Dictionary<string, ShopItem>();//Name, number owned.

        int CursorAlpha;
        bool CursorAppearing;
        int CursorIndex;

        public Inventory()
            : base()
        {
            Sort = SortType.Type;
            BlueprintIsOpen = true;
            ConsumablesIsOpen = false;
            UnitPartsIsOpen = false;
            UnitsIsOpen = false;
            ScrapPartsIsOpen = false;

            CursorIndex = 0;
        }

        public override void Load()
        {
            sprRectangle = Content.Load<Texture2D>("Rectangle");
            fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            ListBlueprints = new List<Blueprint>();
            ListConsumables = new List<Consumables>();
            ListUnitParts = new List<UnitParts>();
            ListUnits = new List<Units>();
            ListScrapParts = new List<ScrapParts>();
            ListBlueprints.Add(new Blueprint("Blueprint 1", "Some blueprint", 100, new List<Blueprint.ItemRequirement>(), null));
            ListBlueprints.Add(new Blueprint("Blueprint 6", "Some blueprint", 100, new List<Blueprint.ItemRequirement>(), null));
            ListBlueprints.Add(new Blueprint("Blueprint 2", "Some blueprint", 100, new List<Blueprint.ItemRequirement>(), null));
            ListBlueprints.Add(new Blueprint("Blueprint 7", "Some blueprint", 100, new List<Blueprint.ItemRequirement>(), null));
            ListConsumables.Add(new Consumables("Consumable 1", "Some consumable", 100));
            ListConsumables.Add(new Consumables("Consumable 5", "Some consumable", 100));
            ListConsumables.Add(new Consumables("Consumable 3", "Some consumable", 100));
            ListConsumables.Add(new Consumables("Consumable 12", "Some consumable", 100));
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
                if (--CursorAlpha <= 20)
                    CursorAppearing = true;
            }
            if (InputHelper.InputUpPressed())
                CursorIndex -= (CursorIndex > 0) ? 1 : 0;
            else if (InputHelper.InputDownPressed())
            {
                if (Sort == SortType.Type)
                {
                    CursorIndex += (CursorIndex < 5) ? 1 : 0;
                }
                else
                    CursorIndex += (CursorIndex < ListItems.Count() - 1) ? 1 : 0;
            }
            else if (InputHelper.InputLeftPressed())
            {
                if (CursorIndex == 0)
                    Sort -= ((int)Sort > 0) ? 1 : 0;
            }
            else if (InputHelper.InputRightPressed())
            {
                if (CursorIndex == 0)
                    Sort += ((int)Sort < 1) ? 1 : 0;
            }
            if (InputHelper.InputCancelPressed())
            {
                RemoveScreen(this);
            }
            else if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
                RemoveScreen(this);
        }
        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprRectangle, new Rectangle(0, 0, Constants.Width - 238, Constants.Height), Color.White);
            g.Draw(sprRectangle, new Rectangle(0, Constants.Height - 100, Constants.Height - 100, 100), Color.Black);
            g.Draw(sprRectangle, new Rectangle(Constants.Height - 100, 0, 1, Constants.Height), Color.Black);
            g.Draw(sprRectangle, new Rectangle(Constants.Height - 100, 50, 238, 1), Color.Black);
            g.DrawString(fntArial12, "Shop", new Vector2(Constants.Height - 80, 15), Color.Black);
                int Y = 61;
                g.DrawString(fntArial12, "Sort type: " + Sort.ToString(), new Vector2(Constants.Height - 90, Y), Color.Black);
                g.Draw(sprRectangle, new Rectangle(Constants.Height - 100, 90, 238, 1), Color.Black);
                if (CursorIndex == 0)
                    g.Draw(sprRectangle, new Rectangle(Constants.Height - 100, Y, 238, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                Y += 30;
                if (Sort == SortType.Type)
                {
                    g.DrawString(fntArial12, "Blueprints (" + ListBlueprints.Count + " )", new Vector2(Constants.Height - 90, Y), Color.Black);
                    if (CursorIndex == 1)
                        g.Draw(sprRectangle, new Rectangle(Constants.Height - 100, Y, 238, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                    if (BlueprintIsOpen)
                    {
                        Y += 10;
                        for (int i = 0; i < ListBlueprints.Count; i++)
                            g.DrawString(fntArial8, ListBlueprints[i].RelativePath, new Vector2(Constants.Height - 80, Y += fntArial8.LineSpacing), Color.Black);
                    }
                    Y += 20;
                    g.DrawString(fntArial12, "Consumables (" + ListConsumables.Count + " )", new Vector2(Constants.Height - 90, Y), Color.Black);
                    if (CursorIndex == 2)
                        g.Draw(sprRectangle, new Rectangle(Constants.Height - 100, Y, 238, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                    if (ConsumablesIsOpen)
                    {
                        Y += 10;
                        for (int i = 0; i < ListConsumables.Count; i++)
                            g.DrawString(fntArial8, ListConsumables[i].RelativePath, new Vector2(Constants.Height - 80, Y += fntArial8.LineSpacing), Color.Black);
                    }
                    Y += 20;
                    g.DrawString(fntArial12, "Unit parts (" + ListUnitParts.Count + " )", new Vector2(Constants.Height - 90, Y), Color.Black);
                    if (CursorIndex == 3)
                        g.Draw(sprRectangle, new Rectangle(Constants.Height - 100, Y, 238, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                    if (UnitPartsIsOpen)
                    {
                        Y += 10;
                        for (int i = 0; i < ListUnitParts.Count; i++)
                            g.DrawString(fntArial8, ListUnitParts[i].RelativePath, new Vector2(Constants.Height - 80, Y += fntArial8.LineSpacing), Color.Black);
                    }
                    Y += 20;
                    g.DrawString(fntArial12, "Units (" + ListUnits.Count + " )", new Vector2(Constants.Height - 90, Y), Color.Black);
                    if (CursorIndex == 4)
                        g.Draw(sprRectangle, new Rectangle(Constants.Height - 100, Y, 238, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                    if (UnitsIsOpen)
                    {
                        Y += 10;
                        for (int i = 0; i < ListUnits.Count; i++)
                            g.DrawString(fntArial8, ListUnits[i].RelativePath, new Vector2(Constants.Height - 80, Y += fntArial8.LineSpacing), Color.Black);
                    }
                    Y += 20;
                    g.DrawString(fntArial12, "Scrap parts (" + ListScrapParts.Count + " )", new Vector2(Constants.Height - 90, Y), Color.Black);
                    if (CursorIndex == 5)
                        g.Draw(sprRectangle, new Rectangle(Constants.Height - 100, Y, 238, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                    if (ScrapPartsIsOpen)
                    {
                        Y += 10;
                        for (int i = 0; i < ListScrapParts.Count; i++)
                            g.DrawString(fntArial8, ListScrapParts[i].RelativePath, new Vector2(Constants.Height - 80, Y += fntArial8.LineSpacing), Color.Black);
                    }
                }
                else if (Sort == SortType.Alphabetical)
                {
                    int i = 0;
                    foreach (KeyValuePair<string, ShopItem> ActiveItem in ListItems)
                    {
                        g.DrawString(fntArial8, ActiveItem.Value.RelativePath, new Vector2(Constants.Height - 90, Y), Color.Black);
                        Y += fntArial8.LineSpacing;
                        i++;
                    }
                }
                g.DrawString(fntArial8, ListItems.First().Value.Description, new Vector2(10, Constants.Height - 97), Color.White);
        }
        private class Consumables : ShopItem
        {
            public Consumables(string Name, string Description, int Price)
                : base(Name, Description, Price)
            {

            }
        }
        private class UnitParts : ShopItem
        {
            public UnitParts(string Name, string Description, int Price)
                : base(Name, Description, Price)
            {

            }
        }
        private class Units : ShopItem
        {
            public Units(string Name, string Description, int Price)
                : base(Name, Description, Price)
            {

            }
        }

        private class ScrapParts : ShopItem
        {
            public ScrapParts(string Name, string Description, int Price)
                : base(Name, Description, Price)
            {

            }
        }
    }
}
