//Thanks to whoever made the original code(FastBitmap.cs or something)
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ProjectEternity.Editors.MapEditor
{
    public unsafe class UnsafeBitmap
    {
        private struct PixelData
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
        }
        public Bitmap Picture;
        BitmapData WorkingPicture;
        byte* StartByte;
        PixelData* Pixel = null;
        int PictureWidth;

        public UnsafeBitmap(Bitmap Picture)
        {
            this.Picture = Picture;
        }

        public void LockPicture()
        {
            Rectangle bounds = new Rectangle(Point.Empty, Picture.Size);

            PictureWidth = (int)(bounds.Width * sizeof(PixelData));
            //Make sure the bitmap is ARGB, else fix the width.
            if (PictureWidth % 4 != 0)
                PictureWidth = 4 * (PictureWidth / 4 + 1);

            //Lock the WorkingPicture so GDI+ won't slow it down.
            WorkingPicture = Picture.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            //Get the first pixel data.
            StartByte = (Byte*)WorkingPicture.Scan0.ToPointer();
        }

        /// <summary>
        /// Release the Picture so it can be used for something else.
        /// </summary>
        public void UnlockPicture()
        {
            Picture.UnlockBits(WorkingPicture);
            WorkingPicture = null;
            StartByte = null;
        }

        public Color GetPixel(int x, int y)
        {
            //Get the pixel(picture is a 1D array).
            Pixel = (PixelData*)(StartByte + y * PictureWidth + x * sizeof(PixelData));
            return Color.FromArgb(Pixel->Alpha, Pixel->Red, Pixel->Green, Pixel->Blue);
        }

        public void SetPixel(int x, int y, Color color)
        {
            PixelData* data = (PixelData*)(StartByte + y * PictureWidth + x * sizeof(PixelData));
            data->Alpha = color.A;
            data->Red = color.R;
            data->Green = color.G;
            data->Blue = color.B;
        }

        public void DrawRectangle(int X, int Y, int Width, int Height, Color color)
        {
            PixelData* data;
            for (int W = 0; W <= Width; W++)
            {
                if (X + W < 0 || X + W > PictureWidth)
                    continue;
                data = (PixelData*)(StartByte + Y * PictureWidth + (X + W) * sizeof(PixelData));
                data->Alpha = color.A;
                data->Red = color.R;
                data->Green = color.G;
                data->Blue = color.B;
                data = (PixelData*)(StartByte + (Y + Height) * PictureWidth + (X + W) * sizeof(PixelData));
                data->Alpha = color.A;
                data->Red = color.R;
                data->Green = color.G;
                data->Blue = color.B;
            }

            for (int H = 0; H < Height; H++)
            {
                if (Y + H < 0 || Y + H > WorkingPicture.Height || X < 0 || X + Width > PictureWidth)
                    continue;
                data = (PixelData*)(StartByte + (Y + H) * PictureWidth + X * sizeof(PixelData));
                data->Alpha = color.A;
                data->Red = color.R;
                data->Green = color.G;
                data->Blue = color.B;
                data = (PixelData*)(StartByte + (Y + H) * PictureWidth + (X + Width) * sizeof(PixelData));
                data->Alpha = color.A;
                data->Red = color.R;
                data->Green = color.G;
                data->Blue = color.B;
            }
        }

        public void FillRectangle(int X, int Y, int Width, int Height, Color color)
        {
            PixelData* data;
            for (int W = 0; W < Width; W++)
                for (int H = 0; H < Height; H++)
                {
                    if (X + W < 0 || X + W > PictureWidth || Y + H < 0 || Y + H > WorkingPicture.Height)
                        continue;
                    data = (PixelData*)(StartByte + (Y + H) * PictureWidth + (X + W) * sizeof(PixelData));
                    data->Alpha = color.A;
                    data->Red = color.R;
                    data->Green = color.G;
                    data->Blue = color.B;
                }
        }

        public void DrawImage(UnsafeBitmap picture, int X, int Y, int srcWidth, int srcHeight)
        {
            PixelData* data;
            Color color;
            for (int W = 0; W < srcWidth; W++)
                for (int H = 0; H < srcHeight; H++)
                {
                    if (X + W < 0 || X + W > picture.WorkingPicture.Width || Y + H < 0 || Y + H > picture.WorkingPicture.Height)
                        continue;
                    color = picture.GetPixel(X + W, Y + H);
                    data = (PixelData*)(StartByte + (H) * PictureWidth + (W) * sizeof(PixelData));
                    data->Alpha = color.A;
                    data->Red = color.R;
                    data->Green = color.G;
                    data->Blue = color.B;
                }
        }

        public void DrawImage(UnsafeBitmap picture, Point Target, Rectangle Source)
        {
            PixelData* data;
            Color color;
            for (int W = 0; W < Source.Width; W++)
                for (int H = 0; H < Source.Height; H++)
                {
                    if (Source.X + W < 0 || Source.X + W > picture.WorkingPicture.Width || Source.Y + H < 0 || Source.Y + H > picture.WorkingPicture.Height
                        || Target.X + W < 0 || Target.Y + H < 0)
                        continue;
                    color = picture.GetPixel(Source.X + W, Source.Y + H);
                    data = (PixelData*)(StartByte + (Target.Y + H) * PictureWidth + (Target.X + W) * sizeof(PixelData));
                    data->Alpha = color.A;
                    data->Red = color.R;
                    data->Green = color.G;
                    data->Blue = color.B;
                }
        }
    }
}
