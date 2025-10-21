using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Editors.ConquestMapEditor;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.SorcererStreetScreen;

namespace ProjectEternity.Editors.MapEditor
{
    public class SorcererStreetAreasTab : IMapEditorTab
    {
        private TabPage tabSpawns;
        private ImageList imageList;

        private ComboBox cbAreas;
        private Button btnAddArea;
        private Button btnRemoveArea;
        private TextBox txtAreaName;
        private ListView lvAreasTiles;

        private Area ActiveArea;

        private bool AllowEvents;

        public BattleMapViewerControl BattleMapViewer { get; set; }
        public IMapHelper Helper { get; set; }
        private SorcererStreetMap ActiveMap;

        public TabPage InitTab(MenuStrip mnuToolBar)
        {
            AllowEvents = true;

            ExtraTabsUserControl ExtraTabs = new ExtraTabsUserControl();
            tabSpawns = ExtraTabs.tabControl1.TabPages[0];

            cbAreas = ExtraTabs.cbAreas;
            btnAddArea = ExtraTabs.btnAddArea;
            btnRemoveArea = ExtraTabs.btnRemoveArea;
            txtAreaName = ExtraTabs.txtAreaName;
            lvAreasTiles = ExtraTabs.lvAreasTiles;

            imageList = new ImageList();
            imageList.ImageSize = new Size(32, 32);
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            lvAreasTiles.LargeImageList = imageList;

            this.cbAreas.SelectedIndexChanged += cbAreas_SelectedIndexChanged;
            this.txtAreaName.TextChanged += txtAreaName_TextChanged;
            this.btnAddArea.Click += new System.EventHandler(this.btnAddArea_Click);
            this.btnRemoveArea.Click += new System.EventHandler(this.btnRemoveArea_Click);

            return tabSpawns;
        }

        public void OnMapLoaded()
        {
            ActiveMap = (SorcererStreetMap)BattleMapViewer.ActiveMap;

            foreach (Area ActiveArea in ActiveMap.ListArea)
            {
                cbAreas.Items.Add(ActiveArea.Name);
            }

            if (cbAreas.Items.Count > 0)
            {
                cbAreas.SelectedIndex = 0;
            }
        }

        public bool TabProcessCmdKey(ref Message msg, Keys keyData)
        {
            return false;
        }

        public void TabOnMouseDown(MouseEventArgs e)
        {
            OnMouseMove(e);
        }

        public void TabOnMouseUp(MouseEventArgs e)
        {
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int GridX = (int)(ActiveMap.CursorPosition.X) / ActiveMap.TileSize.X;
                int GridY = (int)(ActiveMap.CursorPosition.Y) / ActiveMap.TileSize.Y;

                if (cbAreas.SelectedIndex >= 0)
                {
                    CreateTile(GridX, GridY);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                int GridX = (int)(ActiveMap.CursorPosition.X) / ActiveMap.TileSize.X;
                int GridY = (int)(ActiveMap.CursorPosition.Y) / ActiveMap.TileSize.Y;

                if (cbAreas.SelectedIndex >= 0)
                {
                    DeleteTile(GridX, GridY);
                }
            }
        }

        public void OnMapResize(int NewMapSizeX, int NewMapSizeY)
        {
        }

        public void DrawInfo(ToolStripStatusLabel tslInformation)
        {
            tslInformation.Text += " Left click to add a tile to an area";
            tslInformation.Text += " Right click to remove the tile";
        }

        public void DrawMap(CustomSpriteBatch g, GraphicsDevice GraphicsDevice)
        {
            BattleMapViewer.DrawMap();

            if (!ActiveMap.ShowUnits)
            {
                g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

                foreach (Area ActiveArea in ActiveMap.ListArea)
                {
                    foreach (TerrainSorcererStreet ActiveTerrain in ActiveArea.ListTerrainInArea)
                    {
                        int PosX = (int)(ActiveTerrain.GridPosition.X * ActiveMap.TileSize.X - ActiveMap.Camera2DPosition.X);
                        int PosY = (int)(ActiveTerrain.GridPosition.Y * ActiveMap.TileSize.Y - ActiveMap.Camera2DPosition.Y);
                        g.Draw(GameScreens.GameScreen.sprPixel, new Microsoft.Xna.Framework.Rectangle(PosX, PosY, ActiveMap.TileSize.X, ActiveMap.TileSize.Y), Microsoft.Xna.Framework.Color.FromNonPremultiplied(255, 255, 255, 120));
                    }
                }

                g.End();
            }
        }

        private void cbAreas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAreas.SelectedIndex < 0)
            {
                return;
            }

            AllowEvents = false;

            lvAreasTiles.Items.Clear();
            imageList.Images.Clear();

            ActiveArea = ActiveMap.ListArea[cbAreas.SelectedIndex];
            txtAreaName.Text = ActiveArea.Name;

            int ImageIndex = 0;
            foreach (TerrainSorcererStreet ActiveTerrain in ActiveArea.ListTerrainInArea)
            {
                MapLayer TopLayer = (MapLayer)Helper.GetLayersAndSubLayers()[BattleMapViewer.SelectedListLayerIndex];

                DrawableTile ActiveTile = TopLayer.ArrayTile[ActiveTerrain.GridPosition.X, ActiveTerrain.GridPosition.Y];
                lvAreasTiles.Items.Add(ActiveTerrain.ToString(), ImageIndex);
                lvAreasTiles.Items[ImageIndex++].Text = ActiveTerrain.ToString();
                imageList.Images.Add("itemImageKey", Texture2Image(ActiveTile));
            }

            AllowEvents = true;
        }

        protected void btnAddArea_Click(object sender, EventArgs e)
        {
            cbAreas.Items.Add("New Area");
            ActiveMap.ListArea.Add(new Area("New Area"));
            cbAreas.SelectedIndex = cbAreas.Items.Count - 1;
        }

        private void btnRemoveArea_Click(object sender, EventArgs e)
        {
            if (cbAreas.SelectedIndex < 0)
            {
                return;
            }

            int OldIndex = cbAreas.SelectedIndex;
            ActiveMap.ListArea.RemoveAt(OldIndex);
            cbAreas.Items.RemoveAt(OldIndex);

            if (cbAreas.Items.Count > OldIndex)
            {
                cbAreas.SelectedIndex = OldIndex;
            }
            else if (cbAreas.Items.Count > 0)
            {
                cbAreas.SelectedIndex = cbAreas.Items.Count - 1;
            }
        }

        private void txtAreaName_TextChanged(object sender, EventArgs e)
        {
            if (!AllowEvents)
            {
                return;
            }

            ActiveArea.Name = txtAreaName.Text;

            cbAreas.Items[cbAreas.SelectedIndex] = txtAreaName.Text;
        }

        private void CreateTile(int GridX, int GridY)
        {
            MapLayer TopLayer = (MapLayer)Helper.GetLayersAndSubLayers()[BattleMapViewer.SelectedListLayerIndex];
            TerrainSorcererStreet ExistingTerrain = ActiveArea.FindExistingTerrain(GridX, GridY, BattleMapViewer.SelectedListLayerIndex);

            if (ExistingTerrain != null)
            {
                return;
            }
            else
            {
                var ActiveTerrain = TopLayer.ArrayTerrain[GridX, GridY];

                ActiveArea.ListTerrainInArea.Add(ActiveTerrain);
                DrawableTile ActiveTile = TopLayer.ArrayTile[ActiveTerrain.GridPosition.X, ActiveTerrain.GridPosition.Y];
                int ImageIndex = lvAreasTiles.Items.Count;
                lvAreasTiles.Items.Add(ActiveTerrain.ToString(), ImageIndex);
                lvAreasTiles.Items[ImageIndex++].Text = ActiveTerrain.ToString();
                imageList.Images.Add("itemImageKey", Texture2Image(ActiveTile));
            }
        }

        private void DeleteTile(int GridX, int GridY)
        {
            TerrainSorcererStreet ExistingTerrain = ActiveArea.FindExistingTerrain(GridX, GridY, BattleMapViewer.SelectedListLayerIndex);

            if (ExistingTerrain != null)
            {
                int ExistingTerrainIndex = ActiveArea.ListTerrainInArea.IndexOf(ExistingTerrain);

                ActiveArea.ListTerrainInArea.RemoveAt(ExistingTerrainIndex);
                lvAreasTiles.Items.RemoveAt(ExistingTerrainIndex);
                imageList.Images.RemoveAt(ExistingTerrainIndex);

                int ImageIndex = 0;
                foreach (ListViewItem ActiveItem in lvAreasTiles.Items)
                {
                    ActiveItem.ImageIndex = ImageIndex++;
                }
            }
        }

        public Image Texture2Image(DrawableTile ActiveTile)
        {
            Texture2D sprTexture2D = ActiveMap.ListTileSet[ActiveTile.TilesetIndex];

            Image ReturnImage;
            using (MemoryStream MS = new MemoryStream())
            {
                sprTexture2D.SaveAsPng(MS, sprTexture2D.Width, sprTexture2D.Height);
                MS.Seek(0, SeekOrigin.Begin);
                ReturnImage = Image.FromStream(MS);
            }

            if (ReturnImage.Width >= ActiveMap.TileSize.X && ReturnImage.Height >= ActiveMap.TileSize.Y)
            {
                Bitmap bmpImage = new Bitmap(ReturnImage);
                ReturnImage = bmpImage.Clone(new Rectangle(ActiveTile.Origin.X, ActiveTile.Origin.Y, ActiveMap.TileSize.X, ActiveMap.TileSize.Y), bmpImage.PixelFormat);
            }
            return ReturnImage;
        }
    }
}
