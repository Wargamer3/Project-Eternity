using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum SurfacePage
    {
        Walls,
        Floors,
        Doors,
        Windows,
        Stairs,
    }

    public enum SurfaceUse
    {
        Wall,
        Floor,
        Door,
        Window,
        Stairs,
    }

    public class SurfaceItem
    {
        public string Name;
        public SurfacePage Page;
        public SurfaceUse Use;
        public string Description;
        public int Price;
        public Vector3 Swatch;
        public float Footstep;
        public string Model;
        public float Span;
        public float Rise;
        public Vector3 ModelShift;
    }

    public static class WallCatalog
    {
        public const float KitRise = 4.198f;
        const string BuildingModels = "Content/Life Sim/Assets/Models/Building Mode/";
        public static readonly SurfacePage[] Pages = new SurfacePage[]
        {
            SurfacePage.Walls,
            SurfacePage.Floors,
            SurfacePage.Doors,
            SurfacePage.Windows,
            SurfacePage.Stairs,
        };

        static readonly SurfaceItem[] Items = new SurfaceItem[]
        {
            Item(SurfacePage.Walls, SurfaceUse.Wall, "Plaster", 15, "A flat coat. The default wall.", new Vector3(0.78f, 0.76f, 0.7f), 1f),
            Item(SurfacePage.Walls, SurfaceUse.Wall, "Brick", 25, "A red brick face.", new Vector3(0.55f, 0.22f, 0.16f), 1f),
            Item(SurfacePage.Walls, SurfaceUse.Wall, "Wood Panel", 40, "Vertical boards.", new Vector3(0.45f, 0.3f, 0.16f), 1f),
            Item(SurfacePage.Walls, SurfaceUse.Wall, "Stone", 30, "Rough grey blocks.", new Vector3(0.5f, 0.5f, 0.48f), 1f),
            Item(SurfacePage.Walls, SurfaceUse.Wall, "Wallpaper", 20, "A printed paper cover.", new Vector3(0.35f, 0.45f, 0.62f), 1f),
            Item(SurfacePage.Doors, SurfaceUse.Door, "Door", 50, "Cuts a doorway into a wall.", new Vector3(0.55f, 0.36f, 0.18f), 1f),
            Opening(SurfaceUse.Door, "Door 1", 50, "A single door. It cuts into a wall.", new Vector3(0.55f, 0.36f, 0.18f), "Door_1.fbx", 1.736f, 4.188f, new Vector3(0.868f, 0f, 0f)),
            Opening(SurfaceUse.Door, "Door 2", 50, "A single door. It cuts into a wall.", new Vector3(0.55f, 0.36f, 0.18f), "Door_2.fbx", 1.740f, 4.198f, new Vector3(0.870f, 0f, 0f)),
            Opening(SurfaceUse.Door, "Door 3", 50, "A single door. It cuts into a wall.", new Vector3(0.55f, 0.36f, 0.18f), "Door_3.fbx", 1.736f, 4.188f, new Vector3(0.868f, 0f, 0f)),
            Opening(SurfaceUse.Door, "Door 4", 50, "A single door. It cuts into a wall.", new Vector3(0.55f, 0.36f, 0.18f), "Door_4.fbx", 1.740f, 4.198f, new Vector3(0.870f, 0f, 0f)),
            Opening(SurfaceUse.Door, "Door 5", 50, "A single door. It cuts into a wall.", new Vector3(0.55f, 0.36f, 0.18f), "Door_5.fbx", 1.740f, 4.198f, new Vector3(0.870f, 0f, 0f)),
            Opening(SurfaceUse.Door, "Door 6", 50, "A single door. It cuts into a wall.", new Vector3(0.55f, 0.36f, 0.18f), "Door_6.fbx", 1.740f, 4.198f, new Vector3(0.870f, 0f, 0f)),
            Opening(SurfaceUse.Door, "Door 7", 50, "A single door. It cuts into a wall.", new Vector3(0.55f, 0.36f, 0.18f), "Door_7.fbx", 1.740f, 4.198f, new Vector3(0.870f, 0f, 0f)),
            Opening(SurfaceUse.Door, "Door 8", 50, "A single door. It cuts into a wall.", new Vector3(0.55f, 0.36f, 0.18f), "Door_8.fbx", 1.740f, 4.198f, new Vector3(0.870f, 0f, 0f)),
            Opening(SurfaceUse.Door, "Door 9", 50, "A single door. It cuts into a wall.", new Vector3(0.55f, 0.36f, 0.18f), "Door_9.fbx", 1.740f, 4.198f, new Vector3(0.870f, 0f, 0f)),
            Opening(SurfaceUse.Door, "Double Door", 90, "A double door. It needs a longer wall.", new Vector3(0.55f, 0.36f, 0.18f), "Door_Double.fbx", 3.472f, 4.188f, Vector3.Zero),
            Item(SurfacePage.Windows, SurfaceUse.Window, "Window", 40, "Cuts a window into a wall.", new Vector3(0.45f, 0.75f, 0.9f), 1f),
            Opening(SurfaceUse.Window, "Large Window 1", 40, "A wide window. It cuts into a wall.", new Vector3(0.45f, 0.75f, 0.9f), "Window_Large1.fbx", 1.832f, 1.698f, Vector3.Zero),
            Opening(SurfaceUse.Window, "Large Window 2", 40, "A wide window. It cuts into a wall.", new Vector3(0.45f, 0.75f, 0.9f), "Window_Large2.fbx", 1.832f, 1.698f, Vector3.Zero),
            Opening(SurfaceUse.Window, "Round Window 1", 40, "A round window. It cuts into a wall.", new Vector3(0.45f, 0.75f, 0.9f), "Window_Round1.fbx", 0.906f, 1.550f, Vector3.Zero),
            Opening(SurfaceUse.Window, "Round Window 2", 40, "A round window. It cuts into a wall.", new Vector3(0.45f, 0.75f, 0.9f), "Window_Round2.fbx", 0.906f, 1.550f, Vector3.Zero),
            Opening(SurfaceUse.Window, "Round Window 3", 40, "A round window. It cuts into a wall.", new Vector3(0.45f, 0.75f, 0.9f), "Window_Round3.fbx", 0.906f, 1.550f, Vector3.Zero),
            Opening(SurfaceUse.Window, "Small Window 1", 40, "A small window. It cuts into a wall.", new Vector3(0.45f, 0.75f, 0.9f), "Window_Small1.fbx", 0.928f, 1.238f, Vector3.Zero),
            Opening(SurfaceUse.Window, "Small Window 2", 40, "A small window. It cuts into a wall.", new Vector3(0.45f, 0.75f, 0.9f), "Window_Small2.fbx", 0.928f, 1.238f, Vector3.Zero),
            Opening(SurfaceUse.Window, "Small Window 3", 40, "A small window. It cuts into a wall.", new Vector3(0.45f, 0.75f, 0.9f), "Window_Small3.fbx", 0.928f, 1.238f, Vector3.Zero),
            Item(SurfacePage.Floors, SurfaceUse.Floor, "Hardwood", 20, "A wood plank floor.", new Vector3(0.55f, 0.38f, 0.2f), 1f),
            Item(SurfacePage.Floors, SurfaceUse.Floor, "Tile", 25, "Square ceramic tiles.", new Vector3(0.75f, 0.78f, 0.8f), 1.25f),
            Item(SurfacePage.Floors, SurfaceUse.Floor, "Carpet", 30, "A soft covering.", new Vector3(0.4f, 0.22f, 0.28f), 0.5f),
            Item(SurfacePage.Floors, SurfaceUse.Floor, "Concrete", 10, "A bare poured floor.", new Vector3(0.55f, 0.55f, 0.52f), 1.05f),
            Item(SurfacePage.Floors, SurfaceUse.Floor, "Dark Gravel", 25, "A coarse dark surface.", new Vector3(0.22f, 0.22f, 0.24f), 1.35f),
            Item(SurfacePage.Floors, SurfaceUse.Floor, "Metal", 35, "A ringing plate.", new Vector3(0.72f, 0.74f, 0.78f), 1.75f),
            Item(SurfacePage.Floors, SurfaceUse.Floor, "Snow", 15, "A packed white layer.", new Vector3(0.92f, 0.94f, 0.96f), 0.35f),
            Item(SurfacePage.Floors, SurfaceUse.Floor, "Broken Glass", 20, "Shards underfoot.", new Vector3(0.55f, 0.82f, 0.72f), 2.2f),
            Item(SurfacePage.Stairs, SurfaceUse.Stairs, "Stairs", 80, "A flight up one story.", new Vector3(0.62f, 0.62f, 0.66f), 1f),
        };

        public static string Title(SurfacePage Page)
        {
            if (Page == SurfacePage.Walls)
            {
                return "Walls";
            }

            if (Page == SurfacePage.Doors)
            {
                return "Doors";
            }

            if (Page == SurfacePage.Windows)
            {
                return "Windows";
            }

            if (Page == SurfacePage.Stairs)
            {
                return "Stairs";
            }

            return "Floors";
        }

        public static List<SurfaceItem> In(SurfacePage Page)
        {
            List<SurfaceItem> Found = new List<SurfaceItem>();
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i].Page == Page)
                {
                    Found.Add(Items[i]);
                }
            }

            return Found;
        }

        public static SurfaceItem Find(string Name)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return null;
            }

            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i].Name == Name)
                {
                    return Items[i];
                }
            }

            return null;
        }

        public static void Draw(CustomSpriteBatch g, SurfacePage Page, int Index, bool OnDelete, int Funds)
        {
            Texture2D Pixel = GameScreen.sprPixel;
            if (Pixel == null)
            {
                return;
            }

            int X = 70;
            int Y = 64;
            int W = Constants.Width - 70 * 2;
            int H = 430;
            g.Draw(Pixel, new Rectangle(X, Y, W, H), new Color(48, 78, 130, 230));
            g.Draw(Pixel, new Rectangle(X, Y, W, 36), new Color(30, 52, 96, 255));

            int TabX = X + 12;
            for (int i = 0; i < Pages.Length; i++)
            {
                bool Here = Pages[i] == Page;
                g.Draw(Pixel, new Rectangle(TabX, Y + 6, 88, 24), Here ? new Color(230, 190, 70, 255) : new Color(20, 36, 70, 255));
                TextHelper.DrawText(g, Title(Pages[i]), new Vector2(TabX + 6, Y + 8), Here ? Color.Gray : Color.White);
                TabX += 92;
            }

            TextHelper.DrawText(g, Title(Page), new Vector2(X + 16, Y + 48), Color.White);
            g.Draw(Pixel, new Rectangle(X + W - 170, Y + 44, 150, 26), OnDelete ? new Color(230, 190, 70, 255) : new Color(120, 40, 40, 255));
            TextHelper.DrawTextRightAligned(g, "[G] Delete Mode", new Vector2(X + W - 28, Y + 48), OnDelete ? Color.Gray : Color.White);

            List<SurfaceItem> Stock = In(Page);
            int RowH = 32;
            int Visible = System.Math.Max(1, (H - 124) / RowH);
            int First = 0;
            if (Index >= Visible)
            {
                First = Index - Visible + 1;
            }

            if (First > Stock.Count - Visible)
            {
                First = System.Math.Max(0, Stock.Count - Visible);
            }

            int RowY = Y + 84;
            int Last = System.Math.Min(Stock.Count, First + Visible);
            for (int i = First; i < Last; i++)
            {
                bool Selected = i == Index && !OnDelete;
                g.Draw(Pixel, new Rectangle(X + 12, RowY, 180, 28), Selected ? new Color(220, 230, 245, 255) : new Color(20, 40, 80, 180));
                TextHelper.DrawText(g, Stock[i].Name, new Vector2(X + 20, RowY + 4), Selected ? Color.Gray : Color.White);
                RowY += RowH;
            }

            if (Stock.Count == 0 || Index < 0 || Index >= Stock.Count)
            {
                DrawFunds(g, Pixel, X, Y, W, H, Funds);
                return;
            }

            SurfaceItem Card = Stock[Index];
            int CardX = X + 214;
            TextHelper.DrawText(g, Card.Name, new Vector2(CardX, Y + 84), Color.White);
            DrawPreview(g, Card, new Rectangle(CardX, Y + 112, 200, 150));
            TextHelper.DrawText(g, "$ " + Card.Price, new Vector2(CardX + 220, Y + 120), Color.Gold);
            Wrap(g, Card.Description, CardX, Y + 276);
            DrawFunds(g, Pixel, X, Y, W, H, Funds);
        }

        static void DrawFunds(CustomSpriteBatch g, Texture2D Pixel, int X, int Y, int W, int H, int Funds)
        {
            g.Draw(Pixel, new Rectangle(X, Y + H - 36, W, 36), new Color(20, 40, 70, 255));
            TextHelper.DrawText(g, "Funds", new Vector2(X + 16, Y + H - 28), Color.White);
            TextHelper.DrawText(g, "$ " + Funds, new Vector2(X + W - 140, Y + H - 28), Color.Gold);
        }

        static void Wrap(CustomSpriteBatch g, string Text, int X, int Y)
        {
            if (string.IsNullOrEmpty(Text))
            {
                return;
            }

            string[] Words = Text.Split(' ');
            string Line = "";
            int Row = 0;
            for (int i = 0; i < Words.Length; i++)
            {
                string Next = Line.Length == 0 ? Words[i] : Line + " " + Words[i];
                if (Next.Length > 58 && Line.Length > 0)
                {
                    TextHelper.DrawText(g, Line, new Vector2(X, Y + Row * 20), Color.LightGray);
                    Row++;
                    Line = Words[i];
                }
                else
                {
                    Line = Next;
                }
            }

            if (Line.Length > 0)
            {
                TextHelper.DrawText(g, Line, new Vector2(X, Y + Row * 20), Color.LightGray);
            }
        }

        static void DrawPreview(CustomSpriteBatch g, SurfaceItem Card, Rectangle Frame)
        {
            g.End();
            GraphicsDevice Device = g.GraphicsDevice;
            Viewport Previous = Device.Viewport;
            Device.Viewport = new Viewport(Frame.X, Frame.Y, Frame.Width, Frame.Height);
            Device.Clear(ClearOptions.DepthBuffer, Color.Transparent, 1f, 0);
            BuildingItem Box = new BuildingItem();
            if (Card.Use == SurfaceUse.Floor)
            {
                Box.Kind = BuildingKind.Floor;
                Box.Width = 32f;
                Box.Height = 4f;
                Box.Length = 32f;
            }
            else if (Card.Use == SurfaceUse.Wall)
            {
                Box.Kind = BuildingKind.Wall;
                Box.Width = 36f;
                Box.Height = 28f;
                Box.Length = 4f;
            }
            else if (Card.Use == SurfaceUse.Window)
            {
                Box.Kind = BuildingKind.Window;
                Box.Height = Card.Rise > 0f ? 18f : 28f;
                Box.Width = Card.Span > 0f ? Card.Span / Card.Rise * Box.Height : 16f;
                Box.Length = 4f;
            }
            else
            {
                Box.Kind = BuildingKind.Door;
                Box.Height = 28f;
                Box.Width = Card.Span > 0f ? Card.Span / Card.Rise * Box.Height : 16f;
                Box.Length = 4f;
            }

            Box.Model = Card.Model;
            Box.ModelMeters = Card.Rise;
            Box.ModelShift = Card.ModelShift;

            Box.Swatch = Card.Swatch;
            Vector3 Target = new Vector3(0f, Box.Height * 0.5f, 0f);
            Vector3 Eye = Target + new Vector3(40f, 24f, 36f);
            Matrix View = Matrix.CreateLookAt(Eye, Target, Vector3.Up);
            float Aspect = Frame.Width / (float)Frame.Height;
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Aspect, 1f, 200f);
            PlacedBoxDraw.Show(Device, Box, View, Projection);
            Device.Viewport = Previous;
            g.Begin();
        }

        static SurfaceItem Item(SurfacePage Page, SurfaceUse Use, string Name, int Price, string Description, Vector3 Swatch, float Footstep)
        {
            SurfaceItem Card = new SurfaceItem();
            Card.Page = Page;
            Card.Use = Use;
            Card.Name = Name;
            Card.Price = Price;
            Card.Description = Description;
            Card.Swatch = Swatch;
            Card.Footstep = Footstep;
            return Card;
        }

        static SurfaceItem Opening(SurfaceUse Use, string Name, int Price, string Description, Vector3 Swatch, string File, float Span, float Rise, Vector3 Shift)
        {
            SurfacePage Page = Use == SurfaceUse.Window ? SurfacePage.Windows : SurfacePage.Doors;
            SurfaceItem Card = Item(Page, Use, Name, Price, Description, Swatch, 1f);
            Card.Model = BuildingModels + File;
            Card.Span = Span;
            Card.Rise = Rise;
            Card.ModelShift = Shift;
            return Card;
        }
    }
}
