using System.Windows;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public partial class TilesetOriginEditor : Form
    {
        public TilesetOriginEditor()
        {
            InitializeComponent();
        }

        private void TilesetViewer_MouseClick(object sender, MouseEventArgs e)
        {
            Rectangle OldBrush = TilesetViewer.TileBrushSize;
            TilesetViewer.TileBrushSize = new Rectangle(e.X, e.Y, OldBrush.Width, OldBrush.Height);
        }
    }
}
