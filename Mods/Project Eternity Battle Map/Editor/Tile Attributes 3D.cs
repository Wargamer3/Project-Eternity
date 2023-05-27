using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public partial class TileAttributesEditor3D : Form
    {
        private readonly DrawableTile TileInfo;
        private readonly BattleMap Owner;

        public TileAttributesEditor3D()
        {
            InitializeComponent();
        }

        public TileAttributesEditor3D(DrawableTile TileInfo, BattleMap Owner)
        {
            InitializeComponent();

            this.TileInfo = TileInfo;
            this.Owner = Owner;

            cbo3DStyle.SelectedIndex = (int)TileInfo.Terrain3DInfo.TerrainStyle;

            for (int T = 0; T < Owner.ListTilesetPreset.Count; T++)
            {
                cboTilesFront.Items.Add(Owner.ListTilesetPreset[T].TilesetName);
                cboTilesBack.Items.Add(Owner.ListTilesetPreset[T].TilesetName);
                cboTilesLeft.Items.Add(Owner.ListTilesetPreset[T].TilesetName);
                cboTilesRight.Items.Add(Owner.ListTilesetPreset[T].TilesetName);
            }

            if (Owner.ListTilesetPreset.Count > 0)
            {
                cboTilesFront.SelectedIndex = TileInfo.Terrain3DInfo.FrontFace.TilesetIndex;
                cboTilesBack.SelectedIndex = TileInfo.Terrain3DInfo.BackFace.TilesetIndex;
                cboTilesLeft.SelectedIndex = TileInfo.Terrain3DInfo.LeftFace.TilesetIndex;
                cboTilesRight.SelectedIndex = TileInfo.Terrain3DInfo.RightFace.TilesetIndex;
                TilesetViewerFront.InitTileset(Owner.ListTileSet[cboTilesFront.SelectedIndex], Owner.TileSize);
                TilesetViewerBack.InitTileset(Owner.ListTileSet[cboTilesBack.SelectedIndex], Owner.TileSize);
                TilesetViewerLeft.InitTileset(Owner.ListTileSet[cboTilesLeft.SelectedIndex], Owner.TileSize);
                TilesetViewerRight.InitTileset(Owner.ListTileSet[cboTilesRight.SelectedIndex], Owner.TileSize);

                TilesetViewerBack.ListTileBrush[0] = TileInfo.Terrain3DInfo.FrontFace.Origin;
                TilesetViewerFront.ListTileBrush[0] = TileInfo.Terrain3DInfo.BackFace.Origin;
                TilesetViewerLeft.ListTileBrush[0] = TileInfo.Terrain3DInfo.LeftFace.Origin;
                TilesetViewerRight.ListTileBrush[0] = TileInfo.Terrain3DInfo.RightFace.Origin;
            }
            else
            {
                TilesetViewerFront.InitTileset(string.Empty, Owner.TileSize);
                TilesetViewerBack.InitTileset(string.Empty, Owner.TileSize);
                TilesetViewerLeft.InitTileset(string.Empty, Owner.TileSize);
                TilesetViewerRight.InitTileset(string.Empty, Owner.TileSize);
            }

            TileViewer3D.ListTileSet = Owner.ListTileSet;
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            TileViewer3D.ListTile3D = TileInfo.Terrain3DInfo.CreateTile3D(TileInfo.TilesetIndex, TileInfo.Origin.Location, 0, 0, 16, 0, Owner.TileSize, Owner.TileSize, Owner.ListTileSet, 0, 0, 0, 0, 0);
        }

        private void cbo3DStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            TileInfo.Terrain3DInfo.TerrainStyle = (Terrain3D.TerrainStyles)cbo3DStyle.SelectedIndex;
            UpdatePreview();
        }

        #region Tilesets

        private void cboTilesFront_SelectedIndexChanged(object sender, EventArgs e)
        {
            int SelectedIndex = cboTilesFront.SelectedIndex;

            if (SelectedIndex < 0)
                return;

            TilesetViewerFront.InitTileset(Owner.ListTileSet[SelectedIndex], Owner.TileSize);
            TilesetViewerFront.Size = new System.Drawing.Size(Owner.ListTileSet[SelectedIndex].Width, Owner.ListTileSet[SelectedIndex].Height);
            TileInfo.Terrain3DInfo.FrontFace.TilesetIndex = SelectedIndex;
            UpdatePreview();
        }

        private void TileViewerFront_Click(object sender, EventArgs e)
        {
            System.Drawing.Point MousePos = new System.Drawing.Point(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y);
            Point NewOrigin = new Point(MousePos.X / Owner.TileSize.X * Owner.TileSize.X,
                                                 MousePos.Y / Owner.TileSize.Y * Owner.TileSize.Y);
            TilesetViewerFront.ListTileBrush[0] = new Rectangle(NewOrigin.X, NewOrigin.Y, Owner.TileSize.X, Owner.TileSize.Y);

            TileInfo.Terrain3DInfo.FrontFace.Origin = new Rectangle(NewOrigin.X, NewOrigin.Y, Owner.TileSize.X, Owner.TileSize.Y);
            UpdatePreview();
        }

        private void cboTilesBack_SelectedIndexChanged(object sender, EventArgs e)
        {
            int SelectedIndex = cboTilesBack.SelectedIndex;

            if (SelectedIndex < 0)
                return;

            TilesetViewerBack.InitTileset(Owner.ListTileSet[SelectedIndex], Owner.TileSize);
            TilesetViewerBack.Size = new System.Drawing.Size(Owner.ListTileSet[SelectedIndex].Width, Owner.ListTileSet[SelectedIndex].Height);

            TileInfo.Terrain3DInfo.BackFace.TilesetIndex = SelectedIndex;
            UpdatePreview();
        }

        private void TilesetViewerBack_Click(object sender, EventArgs e)
        {
            System.Drawing.Point MousePos = new System.Drawing.Point(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y);
            Point NewOrigin = new Point(MousePos.X / Owner.TileSize.X * Owner.TileSize.X,
                                                 MousePos.Y / Owner.TileSize.Y * Owner.TileSize.Y);
            TilesetViewerBack.ListTileBrush[0] = new Rectangle(NewOrigin.X, NewOrigin.Y, Owner.TileSize.X, Owner.TileSize.Y);

            TileInfo.Terrain3DInfo.BackFace.Origin = new Rectangle(NewOrigin.X, NewOrigin.Y, Owner.TileSize.X, Owner.TileSize.Y);
            UpdatePreview();
        }

        private void cboTilesLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            int SelectedIndex = cboTilesLeft.SelectedIndex;

            if (SelectedIndex < 0)
                return;

            TilesetViewerLeft.InitTileset(Owner.ListTileSet[SelectedIndex], Owner.TileSize);
            TilesetViewerLeft.Size = new System.Drawing.Size(Owner.ListTileSet[SelectedIndex].Width, Owner.ListTileSet[SelectedIndex].Height);

            TileInfo.Terrain3DInfo.LeftFace.TilesetIndex = SelectedIndex;
            UpdatePreview();
        }

        private void TilesetViewerLeft_Click(object sender, EventArgs e)
        {
            System.Drawing.Point MousePos = new System.Drawing.Point(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y);
            Point NewOrigin = new Point(MousePos.X / Owner.TileSize.X * Owner.TileSize.X,
                                                 MousePos.Y / Owner.TileSize.Y * Owner.TileSize.Y);
            TilesetViewerLeft.ListTileBrush[0] = new Rectangle(NewOrigin.X, NewOrigin.Y, Owner.TileSize.X, Owner.TileSize.Y);

            TileInfo.Terrain3DInfo.LeftFace.Origin = new Rectangle(NewOrigin.X, NewOrigin.Y, Owner.TileSize.X, Owner.TileSize.Y);
            UpdatePreview();
        }

        private void cboTilesRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            int SelectedIndex = cboTilesRight.SelectedIndex;

            if (SelectedIndex < 0)
                return;

            TilesetViewerRight.InitTileset(Owner.ListTileSet[SelectedIndex], Owner.TileSize);
            TilesetViewerRight.Size = new System.Drawing.Size(Owner.ListTileSet[SelectedIndex].Width, Owner.ListTileSet[SelectedIndex].Height);

            TileInfo.Terrain3DInfo.RightFace.TilesetIndex = SelectedIndex;
            UpdatePreview();
        }

        private void TilesetViewerRight_Click(object sender, EventArgs e)
        {
            System.Drawing.Point MousePos = new System.Drawing.Point(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y);
            Point NewOrigin = new Point(MousePos.X / Owner.TileSize.X * Owner.TileSize.X,
                                                 MousePos.Y / Owner.TileSize.Y * Owner.TileSize.Y);
            TilesetViewerRight.ListTileBrush[0] = new Rectangle(NewOrigin.X, NewOrigin.Y, Owner.TileSize.X, Owner.TileSize.Y);

            TileInfo.Terrain3DInfo.RightFace.Origin = new Rectangle(NewOrigin.X, NewOrigin.Y, Owner.TileSize.X, Owner.TileSize.Y);
            UpdatePreview();
        }

        #endregion
    }
}
