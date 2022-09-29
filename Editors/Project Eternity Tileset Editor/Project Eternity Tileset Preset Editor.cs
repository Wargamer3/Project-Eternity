using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.ImageViewer;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.MapEditor
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

            cboTerrainBonusActivation.Items.Add("On every turns");
            cboTerrainBonusActivation.Items.Add("On this turn");
            cboTerrainBonusActivation.Items.Add("On next turn");
            cboTerrainBonusActivation.Items.Add("On enter");
            cboTerrainBonusActivation.Items.Add("On leaved");
            cboTerrainBonusActivation.Items.Add("On attack");
            cboTerrainBonusActivation.Items.Add("On hit");
            cboTerrainBonusActivation.Items.Add("On miss");
            cboTerrainBonusActivation.Items.Add("On defend");
            cboTerrainBonusActivation.Items.Add("On hited");
            cboTerrainBonusActivation.Items.Add("On missed");

            cboTerrainBonusType.Items.Add("HP regen");
            cboTerrainBonusType.Items.Add("EN regen");
            cboTerrainBonusType.Items.Add("HP regain");
            cboTerrainBonusType.Items.Add("EN regain");
            cboTerrainBonusType.Items.Add("Armor");
            cboTerrainBonusType.Items.Add("Accuracy");
            cboTerrainBonusType.Items.Add("Evasion");

            cboBattleAnimationBackground.Items.Add("None");
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

            ArrayTerrain = NewTilesetPreset.ArrayTerrain;

            foreach (string BattleBackgroundAnimationPath in NewTilesetPreset.ListBattleBackgroundAnimationPath)
            {
                if (!string.IsNullOrEmpty(BattleBackgroundAnimationPath) && !cboBattleAnimationBackground.Items.Contains(BattleBackgroundAnimationPath))
                {
                    cboBattleAnimationBackground.Items.Add(BattleBackgroundAnimationPath);
                }
            }

            if (!string.IsNullOrWhiteSpace(TilesetName))
                InitTileset(TilesetName);
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void cboTerrainType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListActiveTile.Count > 0)
            {
                Point ActiveTile = ListActiveTile[0];
                Terrain ActiveTerrain = ArrayTerrain[ActiveTile.X, ActiveTile.Y];
                ActiveTerrain.TerrainTypeIndex = (byte)cboTerrainType.SelectedIndex;

                UpdateAllTiles();
            }
        }

        private void btnAddNewBonus_Click(object sender, EventArgs e)
        {
            if (ListActiveTile.Count > 0)
            {
                Point ActiveTile = ListActiveTile[0];
                Terrain ActiveTerrain = ArrayTerrain[ActiveTile.X, ActiveTile.Y];
                lstTerrainBonus.Items.Add((lstTerrainBonus.Items.Count + 1) + ". HP regen (5 ) - On every turn");

                int LastBonusIndex = ActiveTerrain.ListActivation.Length;

                Array.Resize(ref ActiveTerrain.ListActivation, ActiveTerrain.ListActivation.Length + 1);
                Array.Resize(ref ActiveTerrain.ListBonus, ActiveTerrain.ListBonus.Length + 1);
                Array.Resize(ref ActiveTerrain.ListBonusValue, ActiveTerrain.ListBonusValue.Length + 1);

                ActiveTerrain.ListActivation[LastBonusIndex] = TerrainActivation.OnEveryTurns;
                ActiveTerrain.ListBonus[LastBonusIndex] = TerrainBonus.HPRegen;
                ActiveTerrain.ListBonusValue[LastBonusIndex] = 5;

                UpdateAllTiles();
            }
        }

        private void btnRemoveBonus_Click(object sender, EventArgs e)
        {
            if (ListActiveTile.Count > 0 && lstTerrainBonus.SelectedIndex >= 0)
            {
                Point ActiveTile = ListActiveTile[0];
                Terrain ActiveTerrain = ArrayTerrain[ActiveTile.X, ActiveTile.Y];
                int Index = lstTerrainBonus.SelectedIndex;
                Array.Resize(ref ActiveTerrain.ListActivation, ActiveTerrain.ListActivation.Length - 1);
                Array.Resize(ref ActiveTerrain.ListBonus, ActiveTerrain.ListBonus.Length - 1);
                Array.Resize(ref ActiveTerrain.ListBonusValue, ActiveTerrain.ListBonusValue.Length - 1);
                lstTerrainBonus.Items.RemoveAt(lstTerrainBonus.SelectedIndex);

                if (lstTerrainBonus.Items.Count > 0)
                {
                    if (Index >= lstTerrainBonus.Items.Count)
                        lstTerrainBonus.SelectedIndex = lstTerrainBonus.Items.Count - 1;
                    else
                        lstTerrainBonus.SelectedIndex = Index;
                }
                else
                {
                    cboTerrainBonusActivation.Text = "";
                    cboTerrainBonusType.Text = "";
                    txtBonusValue.Text = "";
                }

                UpdateAllTiles();
            }
        }

        private void btnClearBonuses_Click(object sender, EventArgs e)
        {
            if (lstTerrainBonus.Items.Count > 0)
            {
                lstTerrainBonus.SelectedIndex = 0;
                while (lstTerrainBonus.Items.Count > 0)
                    btnRemoveBonus_Click(sender, e);
            }
        }

        private void lstTerrainBonus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListActiveTile.Count > 0 && lstTerrainBonus.SelectedIndex != -1)
            {
                Point ActiveTile = ListActiveTile[0];
                Terrain ActiveTerrain = ArrayTerrain[ActiveTile.X, ActiveTile.Y];
                cboTerrainBonusActivation.SelectedIndex = (int)ActiveTerrain.ListActivation[lstTerrainBonus.SelectedIndex];
                cboTerrainBonusType.SelectedIndex = (int)ActiveTerrain.ListBonus[lstTerrainBonus.SelectedIndex];
                txtBonusValue.Text = ActiveTerrain.ListBonusValue[lstTerrainBonus.SelectedIndex].ToString();

                UpdateAllTiles();
            }
        }

        private void cboTerrainBonusType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListActiveTile.Count > 0 && lstTerrainBonus.SelectedIndex != -1)
            {
                Point ActiveTile = ListActiveTile[0];
                Terrain ActiveTerrain = ArrayTerrain[ActiveTile.X, ActiveTile.Y];
                ActiveTerrain.ListBonus[lstTerrainBonus.SelectedIndex] = (TerrainBonus)cboTerrainBonusType.SelectedIndex;
                lstTerrainBonus.Items[lstTerrainBonus.SelectedIndex] = (lstTerrainBonus.SelectedIndex + 1) + ". " + cboTerrainBonusType.Text + " (" + txtBonusValue.Text + " ) - " + cboTerrainBonusActivation.Text;

                UpdateAllTiles();
            }
        }

        private void cboTerrainBonusActivation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListActiveTile.Count > 0 && lstTerrainBonus.SelectedIndex != -1)
            {
                Point ActiveTile = ListActiveTile[0];
                Terrain ActiveTerrain = ArrayTerrain[ActiveTile.X, ActiveTile.Y];
                ActiveTerrain.ListActivation[lstTerrainBonus.SelectedIndex] = (TerrainActivation)cboTerrainBonusActivation.SelectedIndex;
                lstTerrainBonus.Items[lstTerrainBonus.SelectedIndex] = (lstTerrainBonus.SelectedIndex + 1) + ". " + cboTerrainBonusType.Text + " (" + txtBonusValue.Text + " ) - " + cboTerrainBonusActivation.Text;

                UpdateAllTiles();
            }
        }

        private void txtBonusValue_TextChanged(object sender, EventArgs e)
        {
            if (ListActiveTile.Count > 0 && lstTerrainBonus.SelectedIndex != -1 && txtBonusValue.Text != "")
            {
                Point ActiveTile = ListActiveTile[0];
                Terrain ActiveTerrain = ArrayTerrain[ActiveTile.X, ActiveTile.Y];
                ActiveTerrain.ListBonusValue[lstTerrainBonus.SelectedIndex] = (int)txtBonusValue.Value;
                lstTerrainBonus.Items[lstTerrainBonus.SelectedIndex] = (lstTerrainBonus.SelectedIndex + 1) + ". " + cboTerrainBonusType.Text + " (" + txtBonusValue.Text + " ) - " + cboTerrainBonusActivation.Text;

                UpdateAllTiles();
            }
        }

        private void cboBattleAnimationBackground_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListActiveTile.Count > 0)
            {
                Point ActiveTile = ListActiveTile[0];
                Terrain ActiveTerrain = ArrayTerrain[ActiveTile.X, ActiveTile.Y];
                ActiveTerrain.BattleBackgroundAnimationIndex = (byte)cboBattleAnimationBackground.SelectedIndex;

                UpdateAllTiles();
            }
        }

        private void btnNewBattleAnimationBackground_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.BattleBackgroundAnimation;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathAnimationsBackgroundsAll));
        }

        private void btnDeleteBattleAnimationBackground_Click(object sender, EventArgs e)
        {
            if (cboBattleAnimationBackground.SelectedIndex >= 0)
            {
                cboBattleAnimationBackground.Items.RemoveAt(cboBattleAnimationBackground.SelectedIndex);
            }
        }

        private void UpdateAllTiles()
        {
            if (ListActiveTile.Count > 1)
            {
                Point OriginalTile = ListActiveTile[0];
                Terrain OriginalTerrain = ArrayTerrain[OriginalTile.X, OriginalTile.Y];

                for (int T = 1; T < ListActiveTile.Count; ++T)
                {
                    ArrayTerrain[ListActiveTile[T].X, ListActiveTile[T].Y] = new Terrain(OriginalTerrain, new Microsoft.Xna.Framework.Point(ListActiveTile[T].X, ListActiveTile[T].Y), 0);
                }
            }
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
                cboTerrainType.SelectedIndex = ArrayTerrain[ActiveTile.X, ActiveTile.Y].TerrainTypeIndex;
                cboBattleAnimationBackground.SelectedIndex = ArrayTerrain[ActiveTile.X, ActiveTile.Y].BattleBackgroundAnimationIndex + 1;

                lstTerrainBonus.Items.Clear();

                //Load the lstTerrainBonus.
                for (int i = 0; i < ArrayTerrain[ActiveTile.X, ActiveTile.Y].ListActivation.Length; i++)
                {
                    string ActiveBonus = cboTerrainBonusType.Items[(int)ActiveTerrain.ListBonus[i]].ToString();
                    string ActiveBonusValue = ActiveTerrain.ListBonusValue[i].ToString();
                    string ActiveBonusActivation = cboTerrainBonusActivation.Items[(int)ActiveTerrain.ListActivation[i]].ToString();

                    lstTerrainBonus.Items.Add((i + 1) + ". " + ActiveBonus + " (" + ActiveBonusValue.ToString() + " ) - " + ActiveBonusActivation);
                }

                if (lstTerrainBonus.Items.Count > 0)
                {
                    lstTerrainBonus.SelectedIndex = 0;
                }
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
                                    ArrayTerrain[X, Y] = new Terrain(X, Y, 0, 0);
                                    ArrayTerrain[X, Y].TerrainTypeIndex = 1;
                                }
                            }
                            pbTilePreview_MouseClick(this, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                        }
                        break;

                    case ItemSelectionChoices.BattleBackgroundAnimation:
                        string BackgroundPath = Items[I];
                        if (BackgroundPath != null)
                        {
                            cboBattleAnimationBackground.Items.Add(BackgroundPath.Substring(0, BackgroundPath.Length - 5).Substring(19));
                        }
                        break;
                }
            }
        }
    }
}
