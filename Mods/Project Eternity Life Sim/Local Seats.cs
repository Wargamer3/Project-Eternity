using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum LocalDevice
    {
        Keyboard,
        Pad1,
        Pad2,
        Pad3,
        Pad4,
    }

    public enum PlayerOneDevices
    {
        KeyboardAndPad1,
        Pad1,
        Keyboard,
    }

    public static class LocalSeats
    {
        public static List<List<LocalDevice>> Resolve(PlayerOneDevices PlayerOne, bool Pad1Connected, bool Pad2Connected, bool Pad3Connected, bool Pad4Connected)
        {
            List<List<LocalDevice>> Seats = new List<List<LocalDevice>>();
            for (int i = 0; i < 4; i++)
            {
                Seats.Add(new List<LocalDevice>());
            }

            bool KeyboardOnOne = PlayerOne != PlayerOneDevices.Pad1;
            bool Pad1OnOne = PlayerOne != PlayerOneDevices.Keyboard;
            if (KeyboardOnOne)
            {
                Seats[0].Add(LocalDevice.Keyboard);
            }

            if (Pad1OnOne)
            {
                Seats[0].Add(LocalDevice.Pad1);
            }

            if (!KeyboardOnOne)
            {
                Seats[1].Add(LocalDevice.Keyboard);
            }

            if (!Pad1OnOne && Pad1Connected)
            {
                Seats[1].Add(LocalDevice.Pad1);
            }

            if (Pad2Connected)
            {
                Seats[1].Add(LocalDevice.Pad2);
            }

            if (Pad3Connected)
            {
                Seats[2].Add(LocalDevice.Pad3);
            }

            if (Pad4Connected)
            {
                Seats[3].Add(LocalDevice.Pad4);
            }

            return Seats;
        }
    }

    public static class SplitLayout
    {
        public static Rectangle[] Panes(int Count, int Width, int Height)
        {
            return Panes(Count, Width, Height, false);
        }

        public static Rectangle[] Panes(int Count, int Width, int Height, bool SecondOnLeft)
        {
            if (Width < 1)
            {
                Width = 1;
            }

            if (Height < 1)
            {
                Height = 1;
            }

            if (Count <= 1)
            {
                return new Rectangle[] { new Rectangle(0, 0, Width, Height) };
            }

            int HalfW = Width / 2;
            int HalfH = Height / 2;
            int Right = Width - HalfW;
            int Bottom = Height - HalfH;
            if (Count == 2)
            {
                Rectangle Left = new Rectangle(0, 0, HalfW, Height);
                Rectangle RightPane = new Rectangle(HalfW, 0, Right, Height);
                if (SecondOnLeft)
                {
                    return new Rectangle[] { RightPane, Left };
                }

                return new Rectangle[] { Left, RightPane };
            }

            if (Count == 3)
            {
                return new Rectangle[]
                {
                    new Rectangle(0, 0, HalfW, HalfH),
                    new Rectangle(HalfW, 0, Right, HalfH),
                    new Rectangle(0, HalfH, Width, Bottom),
                };
            }

            return new Rectangle[]
            {
                new Rectangle(0, 0, HalfW, HalfH),
                new Rectangle(HalfW, 0, Right, HalfH),
                new Rectangle(0, HalfH, HalfW, Bottom),
                new Rectangle(HalfW, HalfH, Right, Bottom),
            };
        }
    }
}
