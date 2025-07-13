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
        private enum ItemSelectionChoices { Tile, TileAsBackground, BGM, UnitPosition, Cutscene };

        private ItemSelectionChoices ItemSelectionChoice;

        private TabPage tabTiles;
        private Button btnTileAttributes;
        private Button btnRemoveTile;
        private Button btnAddTile;
        private Button btnAddNewTileSetAsBackground;
        private Label lblActiveTileSet;
        protected ComboBox cboTiles;
        private Button btn3DTileAttributes;
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
            this.tabTiles = new TabPage();
            this.btn3DTileAttributes = new Button();
            this.BattleMapViewer.TilesetViewer = new TilesetViewerControl();
            this.btnTileAttributes = new Button();
            this.sclTileHeight = new VScrollBar();
            this.sclTileWidth = new HScrollBar();
            this.btnRemoveTile = new Button();
            this.btnAddTile = new Button();
            this.btnAddNewTileSetAsBackground = new Button();
            this.lblActiveTileSet = new Label();
            this.cboTiles = new ComboBox();
            this.tabTiles.SuspendLayout();
            // 
            // tabTiles
            // 
            this.tabTiles.Controls.Add(this.btn3DTileAttributes);
            this.tabTiles.Controls.Add(this.BattleMapViewer.TilesetViewer);
            this.tabTiles.Controls.Add(this.btnTileAttributes);
            this.tabTiles.Controls.Add(this.sclTileHeight);
            this.tabTiles.Controls.Add(this.sclTileWidth);
            this.tabTiles.Controls.Add(this.btnRemoveTile);
            this.tabTiles.Controls.Add(this.btnAddTile);
            this.tabTiles.Controls.Add(this.btnAddNewTileSetAsBackground);
            this.tabTiles.Controls.Add(this.lblActiveTileSet);
            this.tabTiles.Controls.Add(this.cboTiles);
            this.tabTiles.Location = new System.Drawing.Point(4, 22);
            this.tabTiles.Name = "tabTiles";
            this.tabTiles.Padding = new Padding(3);
            this.tabTiles.Size = new System.Drawing.Size(325, 497);
            this.tabTiles.TabIndex = 2;
            this.tabTiles.Text = "Tiles";
            this.tabTiles.UseVisualStyleBackColor = true;
            // 
            // btn3DTileAttributes
            // 
            this.btn3DTileAttributes.Location = new System.Drawing.Point(126, 111);
            this.btn3DTileAttributes.Name = "btn3DTileAttributes";
            this.btn3DTileAttributes.Size = new System.Drawing.Size(105, 23);
            this.btn3DTileAttributes.TabIndex = 8;
            this.btn3DTileAttributes.Text = "3D Tile attributes";
            this.btn3DTileAttributes.UseVisualStyleBackColor = true;
            this.btn3DTileAttributes.Click += new System.EventHandler(this.btn3DTileAttributes_Click);
            // 
            // BattleMapViewer.TilesetViewer
            // 
            this.BattleMapViewer.TilesetViewer.Location = new System.Drawing.Point(3, 140);
            this.BattleMapViewer.TilesetViewer.Margin = new Padding(3, 3, 0, 0);
            this.BattleMapViewer.TilesetViewer.Name = "BattleMapViewer.TilesetViewer";
            this.BattleMapViewer.TilesetViewer.Size = new System.Drawing.Size(302, 340);
            this.BattleMapViewer.TilesetViewer.TabIndex = 7;
            this.BattleMapViewer.TilesetViewer.Click += new System.EventHandler(this.TileViewer_Click);
            this.BattleMapViewer.TilesetViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TileViewer_MouseDown);
            this.BattleMapViewer.TilesetViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TileViewer_MouseMove);
            // 
            // btnTileAttributes
            // 
            this.btnTileAttributes.Location = new System.Drawing.Point(10, 111);
            this.btnTileAttributes.Name = "btnTileAttributes";
            this.btnTileAttributes.Size = new System.Drawing.Size(105, 23);
            this.btnTileAttributes.TabIndex = 6;
            this.btnTileAttributes.Text = "Tile attributes";
            this.btnTileAttributes.UseVisualStyleBackColor = true;
            this.btnTileAttributes.Click += new System.EventHandler(this.btnTileAttributes_Click);
            // 
            // btnRemoveTile
            // 
            this.btnRemoveTile.Location = new System.Drawing.Point(148, 53);
            this.btnRemoveTile.Name = "btnRemoveTile";
            this.btnRemoveTile.Size = new System.Drawing.Size(125, 23);
            this.btnRemoveTile.TabIndex = 4;
            this.btnRemoveTile.Text = "Remove";
            this.btnRemoveTile.UseVisualStyleBackColor = true;
            this.btnRemoveTile.Click += new System.EventHandler(this.btnRemoveTile_Click);
            // 
            // btnAddTile
            // 
            this.btnAddTile.Location = new System.Drawing.Point(10, 53);
            this.btnAddTile.Name = "btnAddTile";
            this.btnAddTile.Size = new System.Drawing.Size(132, 23);
            this.btnAddTile.TabIndex = 3;
            this.btnAddTile.Text = "Add";
            this.btnAddTile.UseVisualStyleBackColor = true;
            this.btnAddTile.Click += new System.EventHandler(this.btnAddTile_Click);
            // 
            // btnAddNewTileSetAsBackground
            // 
            this.btnAddNewTileSetAsBackground.Location = new System.Drawing.Point(10, 82);
            this.btnAddNewTileSetAsBackground.Name = "btnAddNewTileSetAsBackground";
            this.btnAddNewTileSetAsBackground.Size = new System.Drawing.Size(263, 23);
            this.btnAddNewTileSetAsBackground.TabIndex = 5;
            this.btnAddNewTileSetAsBackground.Text = "Add new tile set as background";
            this.btnAddNewTileSetAsBackground.UseVisualStyleBackColor = true;
            this.btnAddNewTileSetAsBackground.Click += new System.EventHandler(this.btnAddNewTileSetAsBackground_Click);
            // 
            // label1
            // 
            this.lblActiveTileSet.AutoSize = true;
            this.lblActiveTileSet.Location = new System.Drawing.Point(7, 7);
            this.lblActiveTileSet.Name = "lblActiveTileSet";
            this.lblActiveTileSet.Size = new System.Drawing.Size(70, 13);
            this.lblActiveTileSet.TabIndex = 1;
            this.lblActiveTileSet.Text = "Active tile set";
            // 
            // cboTiles
            // 
            this.cboTiles.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboTiles.FormattingEnabled = true;
            this.cboTiles.Location = new System.Drawing.Point(6, 23);
            this.cboTiles.Name = "cboTiles";
            this.cboTiles.Size = new System.Drawing.Size(267, 21);
            this.cboTiles.TabIndex = 2;
            this.cboTiles.SelectedIndexChanged += new System.EventHandler(this.cboTiles_SelectedIndexChanged);
            // 
            // sclTileHeight
            // 
            this.sclTileHeight.Location = new System.Drawing.Point(305, 140);
            this.sclTileHeight.Name = "sclTileHeight";
            this.sclTileHeight.Size = new System.Drawing.Size(17, 340);
            this.sclTileHeight.TabIndex = 6;
            this.sclTileHeight.Scroll += new ScrollEventHandler(this.sclTileHeight_Scroll);
            // 
            // sclTileWidth
            // 
            this.sclTileWidth.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
            this.sclTileWidth.Location = new System.Drawing.Point(3, 480);
            this.sclTileWidth.Name = "sclTileWidth";
            this.sclTileWidth.Size = new System.Drawing.Size(302, 17);
            this.sclTileWidth.TabIndex = 6;
            this.sclTileWidth.Scroll += new ScrollEventHandler(this.sclTileWidth_Scroll);

            this.tabTiles.ResumeLayout(false);
            this.tabTiles.PerformLayout();

            return tabTiles;
        }

        public bool TabProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((GetAsyncKeyState(Keys.X) & 0x8000) > 0)
            {
                PlaceTile((int)ActiveMap.CursorPosition.X, (int)ActiveMap.CursorPosition.Y, (int)ActiveMap.CursorPosition.Z, false, 0);
                return true;
            }

            return false;
        }

        public void OnMapLoaded()
        {
            for (int T = 0; T < ActiveMap.ListTilesetPreset.Count; T++)
            {
                ItemInfo Item = BaseEditor.GetItemByKey(BaseEditor.GUIRootPathMapTilesetImages, ActiveMap.ListTilesetPreset[T].TilesetName);

                if (Item.Path != null)
                {
                    if (Item.Name.StartsWith("Tileset presets"))
                    {
                        cboTiles.Items.Add(Item.Name);
                    }
                    else
                    {
                        cboTiles.Items.Add(Item.Name);
                    }
                }
                else
                {
                    MessageBox.Show(ActiveMap.ListTilesetPreset[T].TilesetName + " not found, loading default tileset instead.");
                    cboTiles.Items.Add("Default");
                }
            }

            if (ActiveMap.ListTilesetPreset.Count > 0)
            {
                cboTiles.SelectedIndex = 0;
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
                            Helper.ReplaceTerrain(TilePos.X, TilePos.Y, TileAttributesEditor.ActiveTerrain, BattleMapViewer.SelectedListLayerIndex, true);
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
                Terrain PresetTerrain = ActiveMap.ListTilesetPreset[BattleMapViewer.SelectedTilesetIndex].ArrayTerrain[DefaultTile.X / ActiveMap.TileSize.X, DefaultTile.Y / ActiveMap.TileSize.Y];
                DrawableTile PresetTile = ActiveMap.ListTilesetPreset[BattleMapViewer.SelectedTilesetIndex].ArrayTiles[DefaultTile.X / ActiveMap.TileSize.X, DefaultTile.Y / ActiveMap.TileSize.Y];

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

        //Change the ActiveTile to the mouse position.
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

        private void btnAddTile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Tile;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathMapTilesets));
        }

        private void btnAddNewTileSetAsBackground_Click(object sender, EventArgs e)
        {//If there is a map loaded(and so ActiveMap.TileSize.X is not 0).
            if (ActiveMap.TileSize.X != 0)
            {
                ItemSelectionChoice = ItemSelectionChoices.TileAsBackground;
                ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathMapTilesetImages));
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
                Terrain SelectedTerrain = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];

                TileAttributesEditor.Init(SelectedTerrain, ActiveMap);

                if (TileAttributesEditor.ShowDialog() == DialogResult.OK)
                {
                    ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y] = TileAttributesEditor.ActiveTerrain;
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
                ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTiles[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y],
                ActiveMap);

            if (TileAttributesEditor.ShowDialog() == DialogResult.OK)
            {
            }
        }

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

        private void PlaceTile(int X, int Y, int LayerIndex, bool ConsiderSubLayers, int BrushIndex)
        {
            if (X < 0 || X >= ActiveMap.MapSize.X
                || Y < 0 || Y >= ActiveMap.MapSize.Y)
            {
                return;
            }

            Point TilePos = BattleMapViewer.TilesetViewer.GetTileFromBrush(new Point(X * ActiveMap.TileSize.X, Y * ActiveMap.TileSize.Y), BrushIndex);

            if (BattleMapViewer.TilesetViewer.ListSmartTilesetPresets.Count > 0)
            {
                if (TilePos.Y == 0)
                {
                    if (TilePos.X >= BattleMapViewer.TilesetViewer.ListSmartTilesetPresets.Count)
                    {
                        return;
                    }

                    Terrain SmartPresetTerrain = BattleMapViewer.TilesetViewer.ListSmartTilesetPresets[TilePos.X].ArrayTerrain[0, 0];
                    DrawableTile SmartPresetTile = BattleMapViewer.TilesetViewer.ListSmartTilesetPresets[TilePos.X].ArrayTiles[0, 0];

                    Helper.ReplaceTerrain(X, Y,
                        SmartPresetTerrain, LayerIndex, ConsiderSubLayers);

                    Helper.ReplaceTile(X, Y,
                        SmartPresetTile, LayerIndex, ConsiderSubLayers);

                    return;
                }
                else
                {
                    TilePos.Y -= 1;
                }
            }

            if (TilePos.X < 0 || TilePos.X >= ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain.GetLength(0) * ActiveMap.TileSize.X
                || TilePos.Y < 0 || TilePos.Y >= ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain.GetLength(1) * ActiveMap.TileSize.Y)
            {
                return;
            }

            Terrain PresetTerrain = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];
            DrawableTile PresetTile = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTiles[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];

            Helper.ReplaceTerrain(X, Y,
                PresetTerrain, LayerIndex, ConsiderSubLayers);

            Helper.ReplaceTile(X, Y,
                PresetTile, LayerIndex, ConsiderSubLayers);
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
                            if (TilePath.StartsWith("Content/Maps/Tileset Presets"))
                            {
                                string Name = TilePath.Substring(0, TilePath.Length - 4).Substring(29);
                                if (cboTiles.Items.Contains(Name))
                                {
                                    MessageBox.Show("This tile is already listed.\r\n" + Name);
                                    continue;
                                }

                                Terrain.TilesetPreset NewTileset = Terrain.TilesetPreset.FromFile(Name, ActiveMap.ListTilesetPreset.Count);
                                Microsoft.Xna.Framework.Graphics.Texture2D NewTilesetSprite = BattleMapViewer.TilesetViewer.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Maps/Tilesets/" + NewTileset.TilesetName);
                                for (int BackgroundIndex = 0; BackgroundIndex < NewTileset.ListBattleBackgroundAnimationPath.Count; BackgroundIndex++)
                                {
                                    string NewBattleBackgroundPath = NewTileset.ListBattleBackgroundAnimationPath[BackgroundIndex];

                                    if (ActiveMap.ListBattleBackgroundAnimationPath.Contains(NewBattleBackgroundPath))
                                    {
                                        byte MapBackgroundIndex = (byte)ActiveMap.ListBattleBackgroundAnimationPath.IndexOf(NewBattleBackgroundPath);

                                        for (int X = 0; X < NewTileset.ArrayTerrain.GetLength(0); ++X)
                                        {
                                            for (int Y = 0; Y < NewTileset.ArrayTerrain.GetLength(1); ++Y)
                                            {
                                                if (NewTileset.ArrayTerrain[X, Y].BonusInfo.BattleBackgroundAnimationIndex == BackgroundIndex)
                                                {
                                                    NewTileset.ArrayTerrain[X, Y].BonusInfo.BattleBackgroundAnimationIndex = MapBackgroundIndex;
                                                }
                                                if (NewTileset.ArrayTerrain[X, Y].BonusInfo.BattleForegroundAnimationIndex == BackgroundIndex)
                                                {
                                                    NewTileset.ArrayTerrain[X, Y].BonusInfo.BattleForegroundAnimationIndex = MapBackgroundIndex;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        byte NewBattleBackgroundIndex = (byte)ActiveMap.ListBattleBackgroundAnimationPath.Count;
                                        ActiveMap.ListBattleBackgroundAnimationPath.Add(NewBattleBackgroundPath);

                                        for (int X = 0; X < NewTileset.ArrayTerrain.GetLength(0); ++X)
                                        {
                                            for (int Y = 0; Y < NewTileset.ArrayTerrain.GetLength(1); ++Y)
                                            {
                                                if (NewTileset.ArrayTerrain[X, Y].BonusInfo.BattleBackgroundAnimationIndex == BackgroundIndex)
                                                {
                                                    NewTileset.ArrayTerrain[X, Y].BonusInfo.BattleBackgroundAnimationIndex = NewBattleBackgroundIndex;
                                                }
                                                if (NewTileset.ArrayTerrain[X, Y].BonusInfo.BattleForegroundAnimationIndex == BackgroundIndex)
                                                {
                                                    NewTileset.ArrayTerrain[X, Y].BonusInfo.BattleForegroundAnimationIndex = NewBattleBackgroundIndex;
                                                }
                                            }
                                        }
                                    }
                                }

                                ActiveMap.ListTilesetPreset.Add(NewTileset);
                                ActiveMap.ListTileSet.Add(BattleMapViewer.TilesetViewer.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Maps/Tilesets/" + NewTileset.TilesetName));

                                BattleMapViewer.TilesetViewer.ListSmartTilesetPresets.Add(NewTileset);
                                BattleMapViewer.TilesetViewer.ListTilesetPresetsSprite.Add(NewTilesetSprite);
                                cboTiles.Items.Add(Name);
                            }
                            else
                            {
                                string Name = TilePath.Substring(0, TilePath.Length - 4).Substring(22);
                                if (cboTiles.Items.Contains(Name))
                                {
                                    MessageBox.Show("This tile is already listed.\r\n" + Name);
                                    continue;
                                }
                                Microsoft.Xna.Framework.Graphics.Texture2D Tile = BattleMapViewer.TilesetViewer.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Maps/Tilesets/" + Name);

                                ActiveMap.ListTilesetPreset.Add(new Terrain.TilesetPreset(Name, Tile.Width, Tile.Height, ActiveMap.TileSize.X, ActiveMap.TileSize.Y, ActiveMap.ListTilesetPreset.Count));
                                ActiveMap.ListTileSet.Add(Tile);
                                //Add the file name to the tile combo box.
                                cboTiles.Items.Add(Name);
                            }

                            cboTiles.SelectedIndex = ActiveMap.ListTilesetPreset.Count - 1;

                            if (ActiveMap.ListTileSet.Count == 1)
                            {
                                Terrain PresetTerrain = ActiveMap.ListTilesetPreset[0].ArrayTerrain[0, 0];
                                DrawableTile PresetTile = ActiveMap.ListTilesetPreset[0].ArrayTiles[0, 0];

                                //Asign a new tile at the every position, based on its atribtues.
                                for (int X = ActiveMap.MapSize.X - 1; X >= 0; --X)
                                {
                                    for (int Y = ActiveMap.MapSize.Y - 1; Y >= 0; --Y)
                                    {
                                        Helper.ReplaceTerrain(X, Y, PresetTerrain, 0, true);
                                        Helper.ReplaceTile(X, Y, PresetTile, 0, true);
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
                                    Helper.ReplaceTerrain(X, Y, new Terrain(X, Y, ActiveMap.TileSize.X, ActiveMap.TileSize.Y, BattleMapViewer.SelectedListLayerIndex, 0, ActiveMap.LayerHeight,
                                       0, new TerrainActivation[0], new TerrainBonus[0], new int[0]),
                                       0, true);

                                    Helper.ReplaceTile(X, Y,
                                       new DrawableTile(
                                           new Rectangle((X % (ActiveMap.ListTilesetPreset.Last().ArrayTerrain.GetLength(0))) * ActiveMap.TileSize.X,
                                                        (Y % (ActiveMap.ListTilesetPreset.Last().ArrayTerrain.GetLength(1))) * ActiveMap.TileSize.Y,
                                                        ActiveMap.TileSize.X, ActiveMap.TileSize.Y),
                                           cboTiles.Items.Count - 1),
                                       0, true);
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}
