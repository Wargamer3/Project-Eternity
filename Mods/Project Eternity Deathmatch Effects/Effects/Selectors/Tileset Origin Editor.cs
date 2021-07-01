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
            TilesetViewer.ActiveTile = new Microsoft.Xna.Framework.Point(e.X, e.Y);
        }
    }
}
