using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.MapEditor
{
    public class TilesetTab : IMapEditorTab
    {
        protected enum ItemSelectionChoices { Tile, TileAsBackground, Autotile, BGM, UnitPosition, Cutscene };

        protected ItemSelectionChoices ItemSelectionChoice;

        protected ComboBox cboTiles;
        private Button btnAddTile;
        private Button btnRemoveTile;
        private Button btnAddNewTileSetAsBackground;
        private Button btnTileAttributes;
        private Button btn3DTileAttributes;

        protected ComboBox cboAutotiles;
        private Button btnAddAutotile;
        private Button btnRemoveAutotile;

        private VScrollBar sclTileHeight;
        private HScrollBar sclTileWidth;

        protected ITileAttributes TileAttributesEditor;

        private Point TileOriginPoint;

        public BattleMapViewerControl BattleMapViewer { get; set; }
        public IMapHelper Helper { get; set; }
        protected BattleMap ActiveMap => BattleMapViewer.ActiveMap;

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(Keys key);

        public TabPage InitTab(MenuStrip mnuToolBar)
        {
            TabsUserControl SpawnControl = new TabsUserControl();
            TabPage tabTiles = SpawnControl.tabControl1.TabPages[0];
            tabTiles.Resize += tabControl1_Resize;

            this.BattleMapViewer.TilesetViewer = SpawnControl.TilesetViewer;
            this.btnAddTile = SpawnControl.btnAddTileset;
            this.btnRemoveTile = SpawnControl.btnRemoveTileset;
            this.btnAddNewTileSetAsBackground = SpawnControl.btnAddTilesetAsBackground;
            this.btnTileAttributes = SpawnControl.btnTileAttributes;
            this.btn3DTileAttributes = SpawnControl.btn3DTileAttributes;
            this.cboTiles = SpawnControl.cboTilesets;

            this.cboAutotiles = SpawnControl.cboAutotile;
            this.btnAddAutotile = SpawnControl.btnAddAutotile;
            this.btnRemoveAutotile = SpawnControl.btnRemoveAutotile;

            this.sclTileHeight = SpawnControl.sclTileHeight;
            this.sclTileWidth = SpawnControl.sclTileWidth;

            this.BattleMapViewer.TilesetViewer.Click += new System.EventHandler(this.TileViewer_Click);
            this.BattleMapViewer.TilesetViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TileViewer_MouseDown);
            this.BattleMapViewer.TilesetViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TileViewer_MouseMove);
            this.btnAddTile.Click += new System.EventHandler(this.btnAddTile_Click);
            this.btnRemoveTile.Click += new System.EventHandler(this.btnRemoveTile_Click);
            this.btnAddNewTileSetAsBackground.Click += new System.EventHandler(this.btnAddNewTileSetAsBackground_Click);
            this.btnTileAttributes.Click += new System.EventHandler(this.btnTileAttributes_Click);
            this.btn3DTileAttributes.Click += new System.EventHandler(this.btn3DTileAttributes_Click);
            this.cboTiles.SelectedIndexChanged += new System.EventHandler(this.cboTiles_SelectedIndexChanged);

            this.btnAddAutotile.Click += new System.EventHandler(this.btnAddAutotile_Click);
            this.btnRemoveAutotile.Click += new System.EventHandler(this.btnRemoveAutotile_Click);

            this.sclTileHeight.Scroll += new ScrollEventHandler(this.sclTileHeight_Scroll);
            this.sclTileWidth.Scroll += new ScrollEventHandler(this.sclTileWidth_Scroll);

            return tabTiles;
        }

        public bool TabProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((GetAsyncKeyState(Keys.X) & 0x8000) > 0)
            {
                PlaceTile((int)ActiveMap.CursorPosition.X / ActiveMap.TileSize.X, (int)ActiveMap.CursorPosition.Y / ActiveMap.TileSize.Y, (int)ActiveMap.CursorPosition.Z / ActiveMap.LayerHeight, false, 0);
                return true;
            }

            return false;
        }

        public void OnMapLoaded()
        {
            for (int T = 0; T < ActiveMap.ListTilesetPreset.Count; T++)
            {
                if (ActiveMap.ListTilesetPreset[T].TilesetType == TilesetPreset.TilesetTypes.Regular)
                {
                    cboTiles.Items.Add(ActiveMap.ListTilesetPreset[T].ArrayTilesetInformation[0].TilesetName);
                }
                else
                {
                    if (string.IsNullOrEmpty(ActiveMap.ListTilesetPreset[T].ArrayTilesetInformation[0].TilesetName))
                    {
                        cboTiles.Items.Add("Autotile Not Finished");
                    }
                    else
                    {
                        cboTiles.Items.Add("Autotile " + ActiveMap.ListTilesetPreset[T].ArrayTilesetInformation[0].TilesetName);
                    }

                    BattleMapViewer.TilesetViewer.ListAutoTileSprite.Add(ActiveMap.ListTileSet[T]);
                    BattleMapViewer.TilesetViewer.ListAutoTileTilesetPresets.Add(ActiveMap.ListTilesetPreset[T]);
                    if (ActiveMap.ListTilesetPreset[T].TilesetType != TilesetPreset.TilesetTypes.Slave)
                    {
                        cboAutotiles.Items.Add(ActiveMap.ListTilesetPreset[T].ArrayTilesetInformation[0].TilesetName);
                    }
                }
            }

            if (cboTiles.Items.Count > 0)
            {
                cboTiles.SelectedIndex = 0;
            }

            if (cboAutotiles.Items.Count > 0)
            {
                cboAutotiles.SelectedIndex = 0;
            }

            if (cboTiles.SelectedIndex >= 0)
            {
                BattleMapViewer.TilesetViewer.InitTileset(ActiveMap.ListTileSet[cboTiles.SelectedIndex], ActiveMap.TileSize);
            }
            else
            {
                BattleMapViewer.TilesetViewer.InitTileset(string.Empty, ActiveMap.TileSize);
            }

            TileAttributesEditor = Helper.GetTileEditor();
        }

        private void tabControl1_Resize(object sender, EventArgs e)
        {
            sclTileHeight.Location = new System.Drawing.Point(BattleMapViewer.TilesetViewer.Right - sclTileHeight.Width, BattleMapViewer.TilesetViewer.Top);
            sclTileHeight.Height = BattleMapViewer.TilesetViewer.Height - sclTileWidth.Height;

            sclTileWidth.Location = new System.Drawing.Point(BattleMapViewer.TilesetViewer.Left, BattleMapViewer.TilesetViewer.Bottom - sclTileWidth.Height);
            sclTileWidth.Width = BattleMapViewer.TilesetViewer.Width - sclTileHeight.Width;
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            if (cboTiles.Items.Count == 0)
                return;

            int GridX = (int)(ActiveMap.CursorPosition.X) / ActiveMap.TileSize.X;
            int GridY = (int)(ActiveMap.CursorPosition.Y) / ActiveMap.TileSize.Y;

            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                int BrushIndex = 0;
                if (e.Button == MouseButtons.Right)
                {
                    BrushIndex = 1;
                }

                Rectangle TileReplacementZone = BattleMapViewer.TileReplacementZone;
                if (TileReplacementZone.Width > 0)
                {
                    if (GridX > TileReplacementZone.X)
                    {
                        TileReplacementZone.Width = GridX - TileReplacementZone.X + 1;
                    }
                    else if (GridX < TileReplacementZone.X)
                    {
                        int Right = TileReplacementZone.Right;
                        TileReplacementZone.X = GridX;
                        TileReplacementZone.Width = Right - GridX;
                    }
                    if (GridY > TileReplacementZone.Y)
                    {
                        TileReplacementZone.Height = GridY - TileReplacementZone.Y + 1;
                    }
                    else if (GridY < TileReplacementZone.Y)
                    {
                        int Bottom = TileReplacementZone.Bottom;
                        TileReplacementZone.Y = GridY;
                        TileReplacementZone.Height = Bottom - GridY;
                    }

                    BattleMapViewer.TileReplacementZone = TileReplacementZone;
                }
                else if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        Point TilePos = new Point(GridX, GridY);
                        Terrain SelectedTerrain = Helper.GetTerrain(TilePos.X, TilePos.Y, BattleMapViewer.SelectedListLayerIndex);

                        TileAttributesEditor.Init(SelectedTerrain, ActiveMap);

                        if (TileAttributesEditor.ShowDialog() == DialogResult.OK)
                        {
                            DrawableTile SelectedTile = Helper.GetTile(TilePos.X, TilePos.Y, BattleMapViewer.SelectedListLayerIndex);
                            Helper.ReplaceTile(TilePos.X, TilePos.Y, SelectedTile, TileAttributesEditor.ActiveTerrain, BattleMapViewer.SelectedListLayerIndex, true, false);
                        }
                    }
                    else if (e.Button == MouseButtons.Right)
                    {//Get the Tile under the mouse base on the map starting pos.
                        Point TilePos = new Point(GridX, GridY);
                        DrawableTile SelectedTerrain = Helper.GetTile(TilePos.X, TilePos.Y, BattleMapViewer.SelectedListLayerIndex);

                        TileAttributesEditor3D TileAttributesEditor = new TileAttributesEditor3D(
                            SelectedTerrain,
                            ActiveMap);

                        if (TileAttributesEditor.ShowDialog() == DialogResult.OK)
                        {
                        }
                    }
                }
                //Just create a new Tile.
                else if (ActiveMap.TileSize.X != 0)
                {
                    PlaceTile(GridX, GridY, BattleMapViewer.SelectedListLayerIndex, true, BrushIndex);
                }
            }
        }

        public void TabOnMouseDown(MouseEventArgs e)
        {
            if (cboTiles.Items.Count == 0)
                return;

            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                int GridX = (int)(ActiveMap.CursorPosition.X) / ActiveMap.TileSize.X;
                int GridY = (int)(ActiveMap.CursorPosition.Y) / ActiveMap.TileSize.Y;

                BattleMapViewer.TileReplacementZone = new Rectangle(GridX, GridY, 1, 1);
            }
            else
            {
                OnMouseMove(e);
            }
        }

        public void TabOnMouseUp(MouseEventArgs e)
        {
            if (cboTiles.Items.Count == 0)
                return;

            int GridX = (int)(ActiveMap.CursorPosition.X) / ActiveMap.TileSize.X;
            int GridY = (int)(ActiveMap.CursorPosition.Y) / ActiveMap.TileSize.Y;

            Rectangle TileReplacementZone = BattleMapViewer.TileReplacementZone;

            if (TileReplacementZone.Width > 0 && ActiveMap.TileSize.X != 0)
            {
                int BrushIndex = 0;
                if (e.Button == MouseButtons.Right)
                {
                    BrushIndex = 1;
                }

                for (int X = TileReplacementZone.X; X < TileReplacementZone.Right; ++X)
                {
                    for (int Y = TileReplacementZone.Y; Y < TileReplacementZone.Bottom; ++Y)
                    {
                        PlaceTile(X, Y, BattleMapViewer.SelectedListLayerIndex, true, BrushIndex);
                    }
                }
            }
            else
            {
                OnMouseMove(e);
            }

            BattleMapViewer.TileReplacementZone = new Rectangle();
        }

        public void OnMapResize(int NewMapSizeX, int NewMapSizeY)
        {
            if (cboTiles.SelectedIndex >= 0)
            {
                Rectangle DefaultTile = BattleMapViewer.TilesetViewer.ListTileBrush[0];
                Terrain PresetTerrain = ActiveMap.ListTilesetPreset[BattleMapViewer.SelectedTilesetIndex].ArrayTilesetInformation[0].ArrayTerrain[DefaultTile.X / ActiveMap.TileSize.X, DefaultTile.Y / ActiveMap.TileSize.Y];
                DrawableTile PresetTile = ActiveMap.ListTilesetPreset[BattleMapViewer.SelectedTilesetIndex].ArrayTilesetInformation[0].ArrayTiles[DefaultTile.X / ActiveMap.TileSize.X, DefaultTile.Y / ActiveMap.TileSize.Y];

                Helper.ResizeTerrain(NewMapSizeX, NewMapSizeY, PresetTerrain, PresetTile);
            }
        }

        public void DrawInfo(ToolStripStatusLabel tslInformation)
        {
            if (cboTiles.Items.Count == 0)
                return;

            Terrain SelectedTerrain = Helper.GetTerrain((int)ActiveMap.CursorPosition.X, (int)ActiveMap.CursorPosition.Y, (int)ActiveMap.CursorPosition.Z);

            tslInformation.Text += " " + TileAttributesEditor.GetTerrainName(SelectedTerrain.TerrainTypeIndex);

            //Add the selection informations.
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                tslInformation.Text += " Tile attribues";
            else
            {
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                    tslInformation.Text += " Rectangle";
                else
                    tslInformation.Text += " Hold shift to place tiles in a rectangle";
                tslInformation.Text += ", hold ctrl to change the selected tile attributes";
            }
        }

        public void DrawMap(CustomSpriteBatch g, GraphicsDevice GraphicsDevice)
        {
            BattleMapViewer.DrawMap();
        }

        private void TileViewer_Click(object sender, EventArgs e)
        {//If there is a map loaded(and so ActiveMap.TileSize.X is not 0).
            if (ActiveMap.TileSize.X != 0)
            {
                int BrushIndex = 0;
                if (((MouseEventArgs)e).Button == MouseButtons.Right)
                {
                    BrushIndex = 1;
                }

                Point DrawOffset = BattleMapViewer.TilesetViewer.DrawOffset;//Used to avoid warnings.
                //Set the ActiveTile to the mouse position.
                BattleMapViewer.TilesetViewer.SelectTile(TileOriginPoint, new Point(((((MouseEventArgs)e).X + DrawOffset.X) / ActiveMap.TileSize.X) * ActiveMap.TileSize.X,
                                                     ((((MouseEventArgs)e).Y + DrawOffset.Y) / ActiveMap.TileSize.Y) * ActiveMap.TileSize.Y),
                                                     Control.ModifierKeys == Keys.Shift, BrushIndex);
            }
        }

        private void TileViewer_MouseDown(object sender, MouseEventArgs e)
        {//If there is a map loaded(and so ActiveMap.TileSize.X is not 0).
            if (ActiveMap.TileSize.X != 0 && (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right))
            {
                Point DrawOffset = BattleMapViewer.TilesetViewer.DrawOffset;//Used to avoid warnings.
                TileOriginPoint = new Point(((e.X + DrawOffset.X) / ActiveMap.TileSize.X) * ActiveMap.TileSize.X,
                                                     (((e.Y + DrawOffset.X)) / ActiveMap.TileSize.Y) * ActiveMap.TileSize.Y);
            }
        }

        private void TileViewer_MouseMove(object sender, MouseEventArgs e)
        {//If there is a map loaded(and so ActiveMap.TileSize.X is not 0).
            if (ActiveMap.TileSize.X != 0 && (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right))
            {
                int BrushIndex = 0;
                if (e.Button == MouseButtons.Right)
                {
                    BrushIndex = 1;
                }

                Point DrawOffset = BattleMapViewer.TilesetViewer.DrawOffset;//Used to avoid warnings.
                //Set the ActiveTile to the mouse position.
                BattleMapViewer.TilesetViewer.SelectTile(TileOriginPoint, new Point(((e.X + DrawOffset.X) / ActiveMap.TileSize.X) * ActiveMap.TileSize.X,
                                                     (((e.Y + DrawOffset.X)) / ActiveMap.TileSize.Y) * ActiveMap.TileSize.Y),
                                                     Control.ModifierKeys == Keys.Shift, BrushIndex);
            }
        }

        #region Tileset

        private void cboTiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Initialise the scroll bars.
            if (ActiveMap.ListTileSet[cboTiles.SelectedIndex].Width >= BattleMapViewer.TilesetViewer.Width)
            {
                sclTileWidth.Maximum = ActiveMap.ListTileSet[cboTiles.SelectedIndex].Width - BattleMapViewer.TilesetViewer.Width;
                sclTileWidth.Visible = true;
            }
            else
                sclTileWidth.Visible = false;
            if (ActiveMap.ListTileSet[cboTiles.SelectedIndex].Height >= BattleMapViewer.TilesetViewer.Height)
            {
                sclTileHeight.Maximum = ActiveMap.ListTileSet[cboTiles.SelectedIndex].Height - BattleMapViewer.TilesetViewer.Height;
                sclTileHeight.Visible = true;
            }
            else
                sclTileHeight.Visible = false;

            BattleMapViewer.SelectedTilesetIndex = cboTiles.SelectedIndex;
            BattleMapViewer.TilesetViewer.InitTileset(ActiveMap.ListTileSet[cboTiles.SelectedIndex], ActiveMap.TileSize);
        }

        protected virtual void btnAddTile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Tile;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathMapTilesets));
        }

        private void btnAddNewTileSetAsBackground_Click(object sender, EventArgs e)
        {//If there is a map loaded(and so ActiveMap.TileSize.X is not 0).
            if (ActiveMap.TileSize.X != 0)
            {
                ItemSelectionChoice = ItemSelectionChoices.TileAsBackground;
                ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathMapTilesetImages));
            }
        }

        private void btnRemoveTile_Click(object sender, EventArgs e)
        {//If there's a tile set selected.
            if (cboTiles.SelectedIndex >= 0)
            {//Put the current index in a buffer.
                int Index = cboTiles.SelectedIndex;

                Helper.RemoveTileset(Index);
                //Move the current tile set.
                cboTiles.Items.RemoveAt(Index);

                //Replace the index with a new one.
                if (cboTiles.Items.Count > 0)
                {
                    if (Index >= cboTiles.Items.Count)
                        cboTiles.SelectedIndex = cboTiles.Items.Count - 1;
                    else
                        cboTiles.SelectedIndex = Index;
                }
                else
                    cboTiles.Text = "";
            }
        }

        //Open a tile attributes dialog.
        protected virtual void btnTileAttributes_Click(object sender, EventArgs e)
        {
            if (cboTiles.SelectedIndex >= 0)
            {
                Rectangle TilePos = BattleMapViewer.TilesetViewer.ListTileBrush[0];
                Terrain SelectedTerrain = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTilesetInformation[0].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];

                TileAttributesEditor.Init(SelectedTerrain, ActiveMap);

                if (TileAttributesEditor.ShowDialog() == DialogResult.OK)
                {
                    ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTilesetInformation[0].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y] = TileAttributesEditor.ActiveTerrain;
                }
            }
        }

        private void btn3DTileAttributes_Click(object sender, EventArgs e)
        {
            if (ActiveMap.ListTilesetPreset.Count <= 0)
            {
                return;
            }

            Rectangle TilePos = BattleMapViewer.TilesetViewer.ListTileBrush[0];
            TileAttributesEditor3D TileAttributesEditor = new TileAttributesEditor3D(
                ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTilesetInformation[0].ArrayTiles[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y],
                ActiveMap);

            if (TileAttributesEditor.ShowDialog() == DialogResult.OK)
            {
            }
        }

        #endregion

        #region Autotile

        protected virtual void btnAddAutotile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Autotile;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathMapAutotilesPresets));
        }

        private void btnRemoveAutotile_Click(object sender, EventArgs e)
        {//If there's a tile set selected.
            if (cboTiles.SelectedIndex >= 0)
            {//Put the current index in a buffer.
                int Index = cboTiles.SelectedIndex;

                Helper.RemoveTileset(Index);
                //Move the current tile set.
                cboTiles.Items.RemoveAt(Index);

                //Replace the index with a new one.
                if (cboTiles.Items.Count > 0)
                {
                    if (Index >= cboTiles.Items.Count)
                        cboTiles.SelectedIndex = cboTiles.Items.Count - 1;
                    else
                        cboTiles.SelectedIndex = Index;
                }
                else
                    cboTiles.Text = "";
            }
        }

        #endregion

        private void sclTileWidth_Scroll(object sender, ScrollEventArgs e)
        {
            Point DrawOffset = BattleMapViewer.TilesetViewer.DrawOffset;
            DrawOffset.X = e.NewValue;
            BattleMapViewer.TilesetViewer.DrawOffset = DrawOffset;
        }

        private void sclTileHeight_Scroll(object sender, ScrollEventArgs e)
        {
            Point DrawOffset = BattleMapViewer.TilesetViewer.DrawOffset;
            DrawOffset.Y = e.NewValue;
            BattleMapViewer.TilesetViewer.DrawOffset = DrawOffset;
        }

        private void PlaceTile(int GridX, int GridY, int LayerIndex, bool ConsiderSubLayers, int BrushIndex)
        {
            if (GridX < 0 || GridX >= ActiveMap.MapSize.X
                || GridY < 0 || GridY >= ActiveMap.MapSize.Y)
            {
                return;
            }

            Point TilePos = BattleMapViewer.TilesetViewer.GetTileFromBrush(new Point(GridX * ActiveMap.TileSize.X, GridY * ActiveMap.TileSize.Y), BrushIndex);

            if (BattleMapViewer.TilesetViewer.ListAutoTileTilesetPresets.Count > 0)
            {
                if (TilePos.Y == 0)
                {
                    int AutotileIndex = TilePos.X / ActiveMap.TileSize.X;
                    if (AutotileIndex >= BattleMapViewer.TilesetViewer.ListAutoTileTilesetPresets.Count)
                    {
                        return;
                    }

                    Terrain SmartPresetTerrain = BattleMapViewer.TilesetViewer.ListAutoTileTilesetPresets[AutotileIndex].ArrayTilesetInformation[0].ArrayTerrain[0, 0];
                    DrawableTile SmartPresetTile = BattleMapViewer.TilesetViewer.ListAutoTileTilesetPresets[AutotileIndex].ArrayTilesetInformation[0].ArrayTiles[0, 0];

                    //Replace tile before terrain so it can read the terrain before change.
                    Helper.ReplaceTile(GridX, GridY,
                        SmartPresetTile, SmartPresetTerrain, LayerIndex, ConsiderSubLayers, true);

                    return;
                }
                else
                {
                    TilePos.Y -= ActiveMap.TileSize.Y;
                }
            }

            if (TilePos.X < 0 || TilePos.X >= ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTilesetInformation[0].ArrayTerrain.GetLength(0) * ActiveMap.TileSize.X
                || TilePos.Y < 0 || TilePos.Y >= ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTilesetInformation[0].ArrayTerrain.GetLength(1) * ActiveMap.TileSize.Y)
            {
                return;
            }

            Terrain PresetTerrain = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTilesetInformation[0].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];
            DrawableTile PresetTile = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTilesetInformation[0].ArrayTiles[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];

            Helper.ReplaceTile(GridX, GridY,
                PresetTile, PresetTerrain, LayerIndex, ConsiderSubLayers, false);
        }

        private Texture2D AddTilesetPreset(TilesetPreset NewTileset, string AssetFolder)
        {
            for (int BackgroundIndex = 0; BackgroundIndex < NewTileset.ListBattleBackgroundAnimationPath.Count; BackgroundIndex++)
            {
                string NewBattleBackgroundPath = NewTileset.ListBattleBackgroundAnimationPath[BackgroundIndex];

                if (ActiveMap.ListBattleBackgroundAnimationPath.Contains(NewBattleBackgroundPath))
                {
                    byte MapBackgroundIndex = (byte)ActiveMap.ListBattleBackgroundAnimationPath.IndexOf(NewBattleBackgroundPath);

                    for (int X = 0; X < NewTileset.ArrayTilesetInformation[0].ArrayTerrain.GetLength(0); ++X)
                    {
                        for (int Y = 0; Y < NewTileset.ArrayTilesetInformation[0].ArrayTerrain.GetLength(1); ++Y)
                        {
                            if (NewTileset.ArrayTilesetInformation[0].ArrayTerrain[X, Y].BattleBackgroundAnimationIndex == BackgroundIndex)
                            {
                                NewTileset.ArrayTilesetInformation[0].ArrayTerrain[X, Y].BattleBackgroundAnimationIndex = MapBackgroundIndex;
                            }
                            if (NewTileset.ArrayTilesetInformation[0].ArrayTerrain[X, Y].BattleForegroundAnimationIndex == BackgroundIndex)
                            {
                                NewTileset.ArrayTilesetInformation[0].ArrayTerrain[X, Y].BattleForegroundAnimationIndex = MapBackgroundIndex;
                            }
                        }
                    }
                }
                else
                {
                    byte NewBattleBackgroundIndex = (byte)ActiveMap.ListBattleBackgroundAnimationPath.Count;
                    ActiveMap.ListBattleBackgroundAnimationPath.Add(NewBattleBackgroundPath);

                    for (int X = 0; X < NewTileset.ArrayTilesetInformation[0].ArrayTerrain.GetLength(0); ++X)
                    {
                        for (int Y = 0; Y < NewTileset.ArrayTilesetInformation[0].ArrayTerrain.GetLength(1); ++Y)
                        {
                            if (NewTileset.ArrayTilesetInformation[0].ArrayTerrain[X, Y].BattleBackgroundAnimationIndex == BackgroundIndex)
                            {
                                NewTileset.ArrayTilesetInformation[0].ArrayTerrain[X, Y].BattleBackgroundAnimationIndex = NewBattleBackgroundIndex;
                            }
                            if (NewTileset.ArrayTilesetInformation[0].ArrayTerrain[X, Y].BattleForegroundAnimationIndex == BackgroundIndex)
                            {
                                NewTileset.ArrayTilesetInformation[0].ArrayTerrain[X, Y].BattleForegroundAnimationIndex = NewBattleBackgroundIndex;
                            }
                        }
                    }
                }
            }

            Texture2D NewTilesetSprite = BattleMapViewer.TilesetViewer.content.Load<Texture2D>(AssetFolder + NewTileset.ArrayTilesetInformation[0].TilesetName);
            ActiveMap.ListTileSet.Add(NewTilesetSprite);

            return NewTilesetSprite;
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
                            string Name = TilePath.Substring(0, TilePath.Length - 4).Substring(34);
                            if (cboTiles.Items.Contains(Name))
                            {
                                MessageBox.Show("This tile is already listed.\r\n" + Name);
                                continue;
                            }

                            TilesetPreset NewTileset = Helper.LoadTilesetPreset("Tilesets Presets", TilePath.Substring(34), ActiveMap.ListTilesetPreset.Count);
                            NewTileset.RelativePath = Name;

                            AddTilesetPreset(NewTileset, "Assets/Tilesets/");

                            cboTiles.Items.Add(Name);

                            cboTiles.SelectedIndex = ActiveMap.ListTilesetPreset.Count - 1;

                            if (ActiveMap.ListTileSet.Count == 1)
                            {
                                Terrain PresetTerrain = ActiveMap.ListTilesetPreset[0].ArrayTilesetInformation[0].ArrayTerrain[0, 0];
                                DrawableTile PresetTile = ActiveMap.ListTilesetPreset[0].ArrayTilesetInformation[0].ArrayTiles[0, 0];

                                //Asign a new tile at the every position, based on its atribtues.
                                for (int X = ActiveMap.MapSize.X - 1; X >= 0; --X)
                                {
                                    for (int Y = ActiveMap.MapSize.Y - 1; Y >= 0; --Y)
                                    {
                                        Helper.ReplaceTile(X, Y, PresetTile, PresetTerrain, 0, true, false);
                                    }
                                }
                            }
                        }
                        break;

                    case ItemSelectionChoices.TileAsBackground:
                        string TileAsBackgroundPath = Items[I];
                        if (TileAsBackgroundPath != null)
                        {
                            string TileName = Path.GetFileNameWithoutExtension(TileAsBackgroundPath);
                            if (cboTiles.Items.Contains(TileName))
                            {
                                MessageBox.Show("This tile is already listed.\r\n" + TileName);
                                return;
                            }

                            ActiveMap.ListTileSet.Add(BattleMapViewer.TilesetViewer.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Maps/Tilesets/" + TileName));
                            //Add the file name to the tile combo box.
                            cboTiles.Items.Add(TileName);
                            cboTiles.SelectedIndex = ActiveMap.ListTileSet.Count - 1;

                            //Initialise the scroll bars.
                            if (ActiveMap.ListTileSet.Last().Width >= BattleMapViewer.TilesetViewer.Width)
                            {
                                sclTileWidth.Maximum = ActiveMap.ListTileSet.Last().Width - BattleMapViewer.TilesetViewer.Width - 1;
                                sclTileWidth.Visible = true;
                            }
                            else
                                sclTileWidth.Visible = false;
                            if (ActiveMap.ListTileSet.Last().Height >= BattleMapViewer.TilesetViewer.Height)
                            {
                                sclTileHeight.Maximum = ActiveMap.ListTileSet.Last().Height - BattleMapViewer.TilesetViewer.Height - 1;
                                sclTileHeight.Visible = true;
                            }
                            else
                                sclTileHeight.Visible = false;

                            //Asign a new tile at the every position, based on its atribtues.
                            for (int X = ActiveMap.MapSize.X - 1; X >= 0; --X)
                            {
                                for (int Y = ActiveMap.MapSize.Y - 1; Y >= 0; --Y)
                                {
                                    Helper.ReplaceTile(X, Y,
                                       new DrawableTile(
                                           new Rectangle((X % (ActiveMap.ListTilesetPreset.Last().ArrayTilesetInformation[0].ArrayTerrain.GetLength(0))) * ActiveMap.TileSize.X,
                                                        (Y % (ActiveMap.ListTilesetPreset.Last().ArrayTilesetInformation[0].ArrayTerrain.GetLength(1))) * ActiveMap.TileSize.Y,
                                                        ActiveMap.TileSize.X, ActiveMap.TileSize.Y),
                                           cboTiles.Items.Count - 1),
                                       new Terrain(X, Y, ActiveMap.TileSize.X, ActiveMap.TileSize.Y, BattleMapViewer.SelectedListLayerIndex, 0, ActiveMap.LayerHeight,
                                            0, new TerrainActivation[0], new TerrainBonus[0], new int[0]),
                                       0, true, false);
                                }
                            }
                        }
                        break;

                    case ItemSelectionChoices.Autotile:
                        string AutotilePath = Items[I];
                        if (AutotilePath != null)
                        {
                            string Name = AutotilePath.Substring(0, AutotilePath.Length - 5).Substring(35);
                            if (cboTiles.Items.Contains(Name))
                            {
                                MessageBox.Show("This autotile is already listed.\r\n" + Name);
                                continue;
                            }

                            TilesetPreset NewTileset = Helper.LoadTilesetPreset("Autotiles Presets", AutotilePath.Substring(35), ActiveMap.ListTilesetPreset.Count);

                            for (int i = 0; i < NewTileset.ArrayTilesetInformation.Length; i++)
                            {
                                TilesetPreset ExtraTileset = NewTileset;
                                if (i > 0)
                                {
                                    ExtraTileset = NewTileset.CreateSlave(i);
                                    ActiveMap.ListTilesetPreset.Add(ExtraTileset);
                                }
                                Texture2D NewTilesetSprite = AddTilesetPreset(ExtraTileset, "Assets/Autotiles/");
                                NewTileset.RelativePath = Name;

                                BattleMapViewer.TilesetViewer.ListAutoTileSprite.Add(NewTilesetSprite);
                                BattleMapViewer.TilesetViewer.ListAutoTileTilesetPresets.Add(ExtraTileset);
                                cboTiles.Items.Add(Name);
                                cboAutotiles.Items.Add(Name);
                            }
                        }
                        break;

                }
            }
        }
    }
}
