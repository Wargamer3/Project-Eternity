using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using ProjectEternity.Core.Editor;

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
                Image LoadedImage = Image.FromFile(FilePath);
                pbImageViewer.BackgroundImage = new Bitmap(LoadedImage);
                LoadedImage.Dispose();
            }
        }

        public override EditorInfo[] LoadEditors()
        {
            return null;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            FileStream imageStream = new FileStream(FilePath, FileMode.Create);
            pbImageViewer.BackgroundImage.Save(imageStream, ImageFormat.Png);
            imageStream.Close();
        }
    }
}
