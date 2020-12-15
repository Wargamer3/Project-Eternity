using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class UnsafeLabel
    {
        public UnsafeLabel(int LabelName)
        {
            Index = LabelName;
            Parents = new HashSet<UnsafeLabel>();
        }

        public int UpdateParent(int NewIndex)
        {
            Index = NewIndex;
            foreach (UnsafeLabel Lave in Parents)
            {
                if (Lave.Index != Index)
                Lave.UpdateParent(NewIndex);
            }
            return Index;
        }

        public int Index;
        public HashSet<UnsafeLabel> Parents;
    }

    public struct Shape
    {
        public int X1;
        public int Y1;
        public int X2;
        public int Y2;

        public Shape(Point NewPixel)
        {
            X1 = -1;
            Y1 = -1;
            X2 = -1;
            Y2 = -1;

            Add(NewPixel);
        }

        public Shape(Microsoft.Xna.Framework.Vector2 NewPixel)
        {
            X1 = -1;
            Y1 = -1;
            X2 = -1;
            Y2 = -1;

            Add(NewPixel);
        }

        public void Add(Point NewPixel)
        {
            if (X1 == -1)
            {
                X1 = NewPixel.X;
                Y1 = NewPixel.Y;
                X2 = NewPixel.X;
                Y2 = NewPixel.Y;
            }
            else
            {
                if (NewPixel.X < X1)
                    X1 = NewPixel.X;
                else if (NewPixel.X > X2)
                    X2 = NewPixel.X;

                if (NewPixel.Y < Y1)
                    Y1 = NewPixel.Y;
                else if (NewPixel.Y > Y2)
                    Y2 = NewPixel.Y;
            }
        }

        public void Add(Microsoft.Xna.Framework.Vector2 NewPixel)
        {
            if (X1 == -1)
            {
                X1 = (int)NewPixel.X;
                Y1 = (int)NewPixel.Y;
                X2 = (int)NewPixel.X;
                Y2 = (int)NewPixel.Y;
            }
            else
            {
                if (NewPixel.X < X1)
                    X1 = (int)NewPixel.X;
                else if (NewPixel.X > X2)
                    X2 = (int)NewPixel.X;

                if (NewPixel.Y < Y1)
                    Y1 = (int)NewPixel.Y;
                else if (NewPixel.Y > Y2)
                    Y2 = (int)NewPixel.Y;
            }
        }

        public int Width { get { return X2 - X1; } }
        public int Height { get { return Y2 - Y1; } }
    }

    public struct Colorstruct
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;
    }
}