using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelBuildMenu : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "BuildMenu";

        int Tab;
        int Cursor;
        int Category;
        int Focus;
        int Assign;
        readonly string[] Tabs = new string[] { "Pause", "Buy", "Build", "Character" };
        readonly string[] PausePage = new string[] { "Resume", "Player assignation", "Split side" };
        readonly string[] AssignLabels = new string[] { "Keyboard", "Gamepad 1", "Keyboard and Gamepad 1" };
        readonly string[] SideLabels = new string[] { "Second player on the right", "Second player on the left" };
        int Side;
        readonly PlayerOneDevices[] AssignChoices = new PlayerOneDevices[] { PlayerOneDevices.Keyboard, PlayerOneDevices.Pad1, PlayerOneDevices.KeyboardAndPad1 };

        public ActionPanelBuildMenu(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, false)
        {
        }

        public override void OnSelect()
        {
            Owner.Session.BuildOpen = true;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (Owner.BuildPressed())
            {
                Owner.Session.CloseBuild();
                RemoveFromPanelList(this);
                Owner.EndKeys();
                return;
            }

            if (ActiveInputManager.InputCancelPressed() && (Tab == 1 || Tab == 2) && Focus != 0)
            {
                Focus = 0;
                Owner.EndKeys();
                return;
            }

            if (ActiveInputManager.InputCancelPressed() && Focus == 0)
            {
                Owner.Session.CloseBuild();
                RemoveFromPanelList(this);
                Owner.EndKeys();
                return;
            }

            if (Owner.KeyPressed(Microsoft.Xna.Framework.Input.Keys.F1))
            {
                Owner.Session.ApplyKey(Microsoft.Xna.Framework.Input.Keys.F1);
                if (Owner.Session.AdminOpen)
                {
                    AddToPanelListAndSelect(new ActionPanelAdminMenu(Owner, MapManager, ListActionMenuChoice));
                }

                Owner.EndKeys();
                return;
            }

            if (Tab == 1 || Tab == 2)
            {
                if (BrowseCatalog())
                {
                    Owner.EndKeys();
                    return;
                }
            }
            else if (Tab == 0)
            {
                BrowsePause();
                Owner.EndKeys();
                return;
            }

            if (ActiveInputManager.InputLeftPressed())
            {
                Tab--;
                if (Tab < 0)
                {
                    Tab = Tabs.Length - 1;
                }

                Cursor = 0;
                Category = 0;
                Focus = 0;
            }
            else if (ActiveInputManager.InputRightPressed())
            {
                Tab = (Tab + 1) % Tabs.Length;
                Cursor = 0;
                Category = 0;
                Focus = 0;
            }
            else if (ActiveInputManager.InputUpPressed())
            {
                if (Cursor > 0)
                {
                    Cursor--;
                }
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                string[] Page = PageOf(Tab);
                int Last = Page == null ? 0 : Page.Length - 1;
                if (Cursor < Last)
                {
                    Cursor++;
                }
            }
            else if (Chose())
            {
                Confirm();
            }

            Owner.EndKeys();
        }

        void BrowsePause()
        {
            if (Focus == 3)
            {
                if (ActiveInputManager.InputCancelPressed())
                {
                    Focus = 1;
                    Cursor = 2;
                    return;
                }

                if (ActiveInputManager.InputUpPressed() && Side > 0)
                {
                    Side--;
                }
                else if (ActiveInputManager.InputDownPressed() && Side < SideLabels.Length - 1)
                {
                    Side++;
                }
                else if (Chose())
                {
                    MapManager.SecondPlayerOnLeft = Side == 1;
                    Focus = 1;
                    Cursor = 2;
                }

                return;
            }

            if (Focus == 2)
            {
                if (ActiveInputManager.InputCancelPressed())
                {
                    Focus = 1;
                    Cursor = 1;
                    return;
                }

                if (ActiveInputManager.InputUpPressed() && Assign > 0)
                {
                    Assign--;
                }
                else if (ActiveInputManager.InputDownPressed() && Assign < AssignChoices.Length - 1)
                {
                    Assign++;
                }
                else if (Chose())
                {
                    MapManager.SetPlayerOne(AssignChoices[Assign]);
                    Focus = 1;
                    Cursor = 1;
                }

                return;
            }

            if (Focus == 0)
            {
                if (ActiveInputManager.InputLeftPressed())
                {
                    Tab--;
                    if (Tab < 0)
                    {
                        Tab = Tabs.Length - 1;
                    }

                    Cursor = 0;
                    Focus = 0;
                }
                else if (ActiveInputManager.InputRightPressed())
                {
                    Tab = (Tab + 1) % Tabs.Length;
                    Cursor = 0;
                    Focus = 0;
                }
                else if (ActiveInputManager.InputDownPressed() || Chose())
                {
                    Focus = 1;
                    Cursor = 0;
                }

                return;
            }

            if (ActiveInputManager.InputCancelPressed())
            {
                Focus = 0;
                return;
            }

            if (ActiveInputManager.InputUpPressed())
            {
                if (Cursor > 0)
                {
                    Cursor--;
                }
                else
                {
                    Focus = 0;
                }

                return;
            }

            if (ActiveInputManager.InputDownPressed())
            {
                if (Cursor < PausePage.Length - 1)
                {
                    Cursor++;
                }

                return;
            }

            if (Chose())
            {
                if (Cursor <= 0)
                {
                    Owner.Session.CloseBuild();
                    RemoveFromPanelList(this);
                    return;
                }

                if (Cursor == 2)
                {
                    Focus = 3;
                    Side = MapManager.SecondPlayerOnLeft ? 1 : 0;
                    return;
                }

                Focus = 2;
                Assign = 0;
                for (int i = 0; i < AssignChoices.Length; i++)
                {
                    if (AssignChoices[i] == MapManager.PlayerOneChoice)
                    {
                        Assign = i;
                    }
                }
            }
        }

        bool BrowseCatalog()
        {
            bool Surfaces = Tab == 2;
            int PageCount = Surfaces ? WallCatalog.Pages.Length : BuildCatalog.Pages.Length;
            if (Category < 0 || Category >= PageCount)
            {
                Category = 0;
            }

            int Last = Surfaces ? WallCatalog.In(WallCatalog.Pages[Category]).Count - 1 : BuildCatalog.In(BuildCatalog.Pages[Category], MapManager.Home).Count - 1;
            if (Cursor > Last)
            {
                Cursor = Last < 0 ? 0 : Last;
            }

            if (ActiveInputManager.InputLeftPressed())
            {
                if (Focus == 0)
                {
                    return false;
                }

                Category--;
                if (Category < 0)
                {
                    Category = PageCount - 1;
                }

                Cursor = 0;
                Focus = 1;
                return true;
            }

            if (ActiveInputManager.InputRightPressed())
            {
                if (Focus == 0)
                {
                    return false;
                }

                Category = (Category + 1) % PageCount;
                Cursor = 0;
                Focus = 1;
                return true;
            }

            if (ActiveInputManager.InputUpPressed())
            {
                if (Focus == 3)
                {
                    Focus = 2;
                }
                else if (Focus == 2 && Cursor > 0)
                {
                    Cursor--;
                }
                else
                {
                    Focus = 0;
                    return true;
                }

                return true;
            }

            if (ActiveInputManager.InputDownPressed())
            {
                if (Focus == 0)
                {
                    Focus = 1;
                }
                else if (Focus == 1)
                {
                    Focus = 2;
                }
                else if (Focus == 2 && Cursor < Last)
                {
                    Cursor++;
                }
                else
                {
                    Focus = 3;
                }

                return true;
            }

            if (Chose())
            {
                if (Focus == 3)
                {
                    OpenCursor(null, !Surfaces, Surfaces);
                }
                else if (Focus >= 1 && Last >= 0)
                {
                    OpenCursor(Chosen(Surfaces), false, false);
                }
                else
                {
                    Focus = 1;
                }

                return true;
            }

            if (Focus != 0 && Owner.KeyPressed(Microsoft.Xna.Framework.Input.Keys.G))
            {
                OpenCursor(null, !Surfaces, Surfaces);
                return true;
            }

            return Focus != 0;
        }

        bool Chose()
        {
            return ActiveInputManager.InputConfirmPressed() || Owner.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter);
        }

        void Confirm()
        {
            if (Tab == 0)
            {
                Owner.Session.CloseBuild();
                RemoveFromPanelList(this);
                return;
            }

            if (Tab == 1 || Tab == 2)
            {
                Focus = 1;
                return;
            }

            string[] Page = PageOf(Tab);
            if (Page == null || Page.Length == 0)
            {
                return;
            }

            if (Cursor < 0)
            {
                Cursor = 0;
            }
            else if (Cursor >= Page.Length)
            {
                Cursor = Page.Length - 1;
            }

            AddToPanelListAndSelect(new ActionPanelBuildCursor(Owner, MapManager, ListActionMenuChoice, Page[Cursor]));
        }

        string Chosen(bool Surfaces)
        {
            if (Surfaces)
            {
                List<SurfaceItem> Stock = WallCatalog.In(WallCatalog.Pages[Category]);
                if (Cursor < 0 || Cursor >= Stock.Count)
                {
                    return null;
                }

                return Stock[Cursor].Name;
            }

            List<CatalogItem> Cards = BuildCatalog.In(BuildCatalog.Pages[Category], MapManager.Home);
            if (Cursor < 0 || Cursor >= Cards.Count)
            {
                return null;
            }

            return Cards[Cursor].Name;
        }

        void OpenCursor(string Item, bool Grabbing, bool Deleting)
        {
            AddToPanelListAndSelect(new ActionPanelBuildCursor(Owner, MapManager, ListActionMenuChoice, Item, Grabbing, Deleting));
        }

        string[] PageOf(int Index)
        {
            return null;
        }

        protected override void OnCancelPanel()
        {
            Owner.Session.CloseBuild();
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBuildMenu(Owner, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int X = 40;
            for (int i = 0; i < Tabs.Length; i++)
            {
                Color Ink = i == Tab ? Color.Yellow : Color.White;
                TextHelper.DrawText(g, Tabs[i], new Vector2(X, 20), Ink);
                X += 140;
            }

            string[] Page = PageOf(Tab);
            int Funds = Owner.Session == null ? 0 : Owner.Session.Funds;
            if (Tab == 1 && Focus != 0)
            {
                BuildCatalog.Draw(g, BuildCatalog.Pages[Category], Cursor, Focus == 3, Funds, false);
                TextHelper.DrawText(g, "Left Right categories    Up Down items    Enter buy    G grab    Cancel back", new Vector2(80, 40), Color.White);
                return;
            }

            if (Tab == 2 && Focus != 0)
            {
                WallCatalog.Draw(g, WallCatalog.Pages[Category], Cursor, Focus == 3, Funds);
                TextHelper.DrawText(g, "Left Right categories    Up Down items    Enter place    G delete    Cancel back", new Vector2(80, 40), Color.White);
                return;
            }

            if (Tab == 1 || Tab == 2)
            {
                TextHelper.DrawText(g, Tab == 1 ? "Buy" : "Build", new Vector2(40, 70), Color.White);
                TextHelper.DrawText(g, "Enter opens the catalog", new Vector2(40, 98), Color.LightGray);
            }
            else             if (Tab == 0)
            {
                if (Focus == 3)
                {
                    int Y = 70;
                    for (int i = 0; i < SideLabels.Length; i++)
                    {
                        string Line = i == Side ? "> " + SideLabels[i] : SideLabels[i];
                        TextHelper.DrawText(g, Line, new Vector2(40, Y), Color.White);
                        Y += 28;
                    }

                    TextHelper.DrawText(g, "Enter applies it", new Vector2(40, Y + 12), Color.LightGray);
                }
                else if (Focus == 2)
                {
                    int Y = 70;
                    for (int i = 0; i < AssignLabels.Length; i++)
                    {
                        string Line = i == Assign ? "> " + AssignLabels[i] : AssignLabels[i];
                        TextHelper.DrawText(g, Line, new Vector2(40, Y), Color.White);
                        Y += 28;
                    }

                    TextHelper.DrawText(g, "Enter applies it", new Vector2(40, Y + 12), Color.LightGray);
                }
                else
                {
                    int Y = 70;
                    for (int i = 0; i < PausePage.Length; i++)
                    {
                        string Line = Focus != 0 && i == Cursor ? "> " + PausePage[i] : PausePage[i];
                        TextHelper.DrawText(g, Line, new Vector2(40, Y), Color.White);
                        Y += 28;
                    }
                }
            }
            else if (Page != null)
            {
                int Y = 70;
                for (int i = 0; i < Page.Length; i++)
                {
                    string Line = i == Cursor ? "> " + Page[i] : Page[i];
                    TextHelper.DrawText(g, Line, new Vector2(40, Y), Color.White);
                    Y += 28;
                }

                TextHelper.DrawText(g, "Enter aims it from above", new Vector2(40, Y + 12), Color.LightGray);
            }
            else
            {
                List<string> Lines = CharacterPage.Lines(Owner.Session.Controlled);
                int Y = 70;
                for (int i = 0; i < Lines.Count; i++)
                {
                    TextHelper.DrawText(g, Lines[i], new Vector2(40, Y), Color.White);
                    Y += 24;
                }
            }
        }
    }
}
