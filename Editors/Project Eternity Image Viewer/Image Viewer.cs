using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Editors.ImageViewer
{
    public partial class ProjectEternityImageViewer : BaseEditor
    {
        public ProjectEternityImageViewer()
        {
            InitializeComponent();
        }

        public ProjectEternityImageViewer(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream imageStream = new FileStream(FilePath, FileMode.Create);
                Bitmap myBitmap = new Bitmap(32, 32, PixelFormat.Format32bppArgb);
                myBitmap.Save(imageStream, ImageFormat.Png);
                imageStream.Close();
                pbImageViewer.BackgroundImage = myBitmap;
            }
            else
            {
                string ImagePath = FilePath.Substring(0, FilePath.Length - 4).Substring(8);
                this.Text = ImagePath;
                pbImageViewer.BackgroundImage = new Bitmap(Texture2Image(GameScreen.ContentFallback.Load<Texture2D>(ImagePath)));
            }
        }

        public static Image Texture2Image(Texture2D sprTexture2D)
        {
            Image ReturnImage;
            using (MemoryStream MS = new MemoryStream())
            {
                sprTexture2D.SaveAsPng(MS, sprTexture2D.Width, sprTexture2D.Height);
                MS.Seek(0, SeekOrigin.Begin);
                ReturnImage = Image.FromStream(MS);
            }
            return ReturnImage;
        }

        public override EditorInfo[] LoadEditors()
        {
            return null;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
        }
    }
}
