using Microsoft.Xna.Framework;
using System.Windows.Forms;

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
            Rectangle OldBrush = TilesetViewer.ListTileBrush[0];
            TilesetViewer.ListTileBrush[0] = new Rectangle(e.X, e.Y, OldBrush.Width, OldBrush.Height);
        }
    }
}
