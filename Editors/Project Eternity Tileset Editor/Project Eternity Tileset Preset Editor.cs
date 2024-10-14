using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.ImageViewer;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.TilesetEditor
{
    public partial class ProjectEternityTilesetPresetEditor : BaseEditor
    {
        private string TilesetName;
        private Bitmap ActiveTileset;
        private List<Point> ListActiveTile;//X, Y position of the cursor in the TilePreview, used to select the origin for the next Tile.
        private Point TileSize;
        private Terrain[,] ArrayTerrain;
        private List<string> ListBattleBackgroundAnimationPath;

        //Buffer used to draw the Tile preview.
        private BufferedGraphicsContext pbTilePreviewContext;
        private BufferedGraphics pbTilePreviewGraphicDevice;
        private Point pbTilePreviewStartingPoint;//Point from which to start drawing the Tile preview.

        private TileEditor frmTileEditor;

        private enum ItemSelectionChoices { Tile, BattleBackgroundAnimation };

        private ItemSelectionChoices ItemSelectionChoice;

        public ProjectEternityTilesetPresetEditor()
        {
            InitializeComponent();

            TilesetName = "";
            ListActiveTile = new List<Point>();
            TileSize = new Point(32, 32);
            ArrayTerrain = new Terrain[0, 0];
            ListBattleBackgroundAnimationPath = new List<string>();
        }

        public ProjectEternityTilesetPresetEditor(string FilePath, object[] Params)
            :this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadTileset(FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathMapTilesetImages, GUIRootPathMapTilesets }, "Maps/Tilesets/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false),
                new EditorInfo(new string[] { GUIRootPathMapTilesetPresets, GUIRootPathMapTilesets }, "Maps/Tileset Presets/", new string[] { ".pet" }, typeof(ProjectEternityTilesetPresetEditor), true)
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            FileStream FS = new FileStream(ItemPath, FileMode.Create);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.Unicode);

            BW.Write(TileSize.X);
            BW.Write(TileSize.Y);

            BW.Write(TilesetName);
            BW.Write((byte)cbTilesetType.SelectedIndex);

            BW.Write(ArrayTerrain.GetLength(0));
            BW.Write(ArrayTerrain.GetLength(1));

            //Tiles
            for (int Y = 0; Y < ArrayTerrain.GetLength(1); Y++)
            {
                for (int X = 0; X < ArrayTerrain.GetLength(0); X++)
                {
                    ArrayTerrain[X, Y].Save(BW);
                }
            }

            BW.Write(ListBattleBackgroundAnimationPath.Count);
            foreach (string BattleBackgroundAnimationPath in ListBattleBackgroundAnimationPath)
            {
                BW.Write(BattleBackgroundAnimationPath);
            }

            BW.Flush();
            BW.Close();
            FS.Close();
        }

        private void LoadTileset(string Path)
        {
            string Name = Path.Substring(0, Path.Length - 4).Substring(29);

            this.Text = Name + " - Project Eternity Tileset Editor";

            FileStream FS = new FileStream("Content/Maps/Tileset presets/" + Name + ".pet", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            TileSize.X = BR.ReadInt32();
            TileSize.Y = BR.ReadInt32();

            Terrain.TilesetPreset NewTilesetPreset = new Terrain.TilesetPreset(BR, TileSize.X, TileSize.Y, 0);

            BR.Close();
            FS.Close();

            TilesetName = NewTilesetPreset.TilesetName;
            cbTilesetType.SelectedIndex = (int)NewTilesetPreset.TilesetType;

            ArrayTerrain = NewTilesetPreset.ArrayTerrain;

            frmTileEditor = new TileEditor();
            frmTileEditor.LoadTileset(NewTilesetPreset);

            if (!string.IsNullOrWhiteSpace(TilesetName))
                InitTileset(TilesetName);
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void DrawTilePreview()
        {
            if (ActiveTileset == null)
                return;

            //bmpTilePreview.LockPicture();
            pbTilePreviewGraphicDevice.Graphics.Clear(Color.Black);
            //Draw the selected tile set.
            pbTilePreviewGraphicDevice.Graphics.DrawImage(ActiveTileset,
                new Rectangle(0, 0, panTilesetPreview.Width, panTilesetPreview.Height),
                new Rectangle(pbTilePreviewStartingPoint.X, pbTilePreviewStartingPoint.Y, panTilesetPreview.Width, panTilesetPreview.Height), GraphicsUnit.Pixel);

            //Draw the vertical lines for the grid.
            for (int X = 0; X <= panTilesetPreview.Width / TileSize.X + 1; X++)
                pbTilePreviewGraphicDevice.Graphics.DrawLine(Pens.Black, X * TileSize.X - pbTilePreviewStartingPoint.X % TileSize.X, 0,
                                                        X * TileSize.X - pbTilePreviewStartingPoint.X % TileSize.X, panTilesetPreview.Height);
            //Draw the horizontal lines for the grid.
            for (int Y = 0; Y <= panTilesetPreview.Height / TileSize.Y + 1; Y++)
                pbTilePreviewGraphicDevice.Graphics.DrawLine(Pens.Black, 0, Y * TileSize.Y - pbTilePreviewStartingPoint.Y % TileSize.Y,
                       panTilesetPreview.Width, Y * TileSize.Y - pbTilePreviewStartingPoint.Y % TileSize.Y);

            foreach (Point ActiveTile in ListActiveTile)
            {
                //Draw a rectangle at the ActiveTile position.(Shows the selected origin)
                pbTilePreviewGraphicDevice.Graphics.DrawRectangle(Pens.Red,
                    ActiveTile.X * TileSize.X - pbTilePreviewStartingPoint.X,
                    ActiveTile.Y * TileSize.Y - pbTilePreviewStartingPoint.Y, TileSize.X, TileSize.Y);
            }

            pbTilePreviewGraphicDevice.Render();
        }

        private void btnAddTile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Tile;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathMapTilesetImages));
        }

        private void pbTilePreview_MouseClick(object sender, MouseEventArgs e)
        {
            Point ActiveTile = new Point((e.X + pbTilePreviewStartingPoint.X) / TileSize.X,
                                    (e.Y + pbTilePreviewStartingPoint.Y) / TileSize.Y);

            Terrain ActiveTerrain = ArrayTerrain[ActiveTile.X, ActiveTile.Y];

            if (Control.ModifierKeys != Keys.Shift)
                ListActiveTile.Clear();

            ListActiveTile.Add(ActiveTile);

            if (ListActiveTile.Count == 1)
            {
                frmTileEditor.SelectTerrain(ActiveTerrain);
            }

            panTilesetPreview.Refresh();
            DrawTilePreview();
        }

        private void InitTileset(string TilesetName)
        {
            ActiveTileset = new Bitmap("Content/Maps/Tilesets/" + TilesetName + ".png");
            
            //Add the file name to the tile combo box.
            txtTilesetName.Text = TilesetName;
            //Create a new buffer based on the picturebox.
            this.pbTilePreviewContext = BufferedGraphicsManager.Current;
            this.pbTilePreviewContext.MaximumBuffer = new Size(panTilesetPreview.Width, panTilesetPreview.Height);
            this.pbTilePreviewGraphicDevice = pbTilePreviewContext.Allocate(panTilesetPreview.CreateGraphics(), new Rectangle(0, 0, panTilesetPreview.Width, panTilesetPreview.Height));

            sclTileWidth.Maximum = ActiveTileset.Width - panTilesetPreview.Width - 7 + TileSize.X / 2;
            sclTileHeight.Maximum = ActiveTileset.Height - panTilesetPreview.Height - 7 + TileSize.X / 2;

            //Refresh the TilePreview.
            DrawTilePreview();
        }
        
        private void sclTileWidth_Scroll(object sender, ScrollEventArgs e)
        {
            pbTilePreviewStartingPoint.X = e.NewValue;
            DrawTilePreview();
        }

        private void sclTileHeight_Scroll(object sender, ScrollEventArgs e)
        {
            pbTilePreviewStartingPoint.Y = e.NewValue;
            DrawTilePreview();
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Tile:
                        string TilePath = Items[I];
                        if (TilePath != null)
                        {
                            TilesetName = Path.GetFileNameWithoutExtension(TilePath);
                            InitTileset(TilesetName);
                            int TilesetWidth = ActiveTileset.Width / TileSize.X;
                            int TilesetHeight = ActiveTileset.Height / TileSize.Y;
                            ArrayTerrain = new Terrain[TilesetWidth, TilesetHeight];
                            for (int X = TilesetWidth - 1; X >= 0; --X)
                            {
                                for (int Y = TilesetHeight - 1; Y >= 0; --Y)
                                {
                                    ArrayTerrain[X, Y] = new Terrain(X, Y, TileSize.X, TileSize.Y, 0, 0, 0);
                                    ArrayTerrain[X, Y].TerrainTypeIndex = 1;
                                }
                            }
                            pbTilePreview_MouseClick(this, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                        }
                        break;
                }
            }
        }

        private void tsmTileEditor_Click(object sender, EventArgs e)
        {
            frmTileEditor.Show();
        }

        private void ProjectEternityTilesetPresetEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmTileEditor.Close();
        }
    }
}
