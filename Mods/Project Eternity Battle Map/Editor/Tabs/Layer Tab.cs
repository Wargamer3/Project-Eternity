using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.MapEditor
{
    public class LayerTab : IMapEditorTab
    {
        private TabPage tabLayers;
        private Button btnRemoveExtraLayer;
        private Button btnAddExtraLayer;
        private Button btnLayerAttributes;
        private Button btnLayerMoveDown;
        private Button btnLayerMoveUp;
        private ListBox lsLayers;
        private Button btnRemoveSublayer;
        private Button btnAddSublayer;

        private CheckBox cbShowAllLayers;

        private bool AllowEvents = true;

        public BattleMapViewerControl BattleMapViewer { get; set; }
        public IMapHelper Helper { get; set; }
        private BattleMap ActiveMap => BattleMapViewer.ActiveMap;

        public TabPage InitTab(MenuStrip mnuToolBar)
        {
            this.tabLayers = new TabPage();
            this.btnRemoveSublayer = new Button();
            this.btnAddSublayer = new Button();
            this.btnLayerMoveDown = new Button();
            this.btnLayerMoveUp = new Button();
            this.lsLayers = new ListBox();
            this.btnLayerAttributes = new Button();
            this.btnRemoveExtraLayer = new Button();
            this.btnAddExtraLayer = new Button();

            // 
            // tabLayers
            // 
            this.tabLayers.Controls.Add(this.btnRemoveSublayer);
            this.tabLayers.Controls.Add(this.btnAddSublayer);
            this.tabLayers.Controls.Add(this.btnLayerMoveDown);
            this.tabLayers.Controls.Add(this.btnLayerMoveUp);
            this.tabLayers.Controls.Add(this.lsLayers);
            this.tabLayers.Controls.Add(this.btnLayerAttributes);
            this.tabLayers.Controls.Add(this.btnRemoveExtraLayer);
            this.tabLayers.Controls.Add(this.btnAddExtraLayer);
            this.tabLayers.Location = new System.Drawing.Point(4, 22);
            this.tabLayers.Name = "tabLayers";
            this.tabLayers.Padding = new System.Windows.Forms.Padding(3);
            this.tabLayers.Size = new System.Drawing.Size(325, 497);
            this.tabLayers.TabIndex = 4;
            this.tabLayers.Text = "Layers";
            this.tabLayers.UseVisualStyleBackColor = true;
            // 
            // btnRemoveSublayer
            // 
            this.btnRemoveSublayer.Location = new System.Drawing.Point(121, 175);
            this.btnRemoveSublayer.Name = "btnRemoveSublayer";
            this.btnRemoveSublayer.Size = new System.Drawing.Size(109, 23);
            this.btnRemoveSublayer.TabIndex = 30;
            this.btnRemoveSublayer.Text = "Remove Sublayer";
            this.btnRemoveSublayer.UseVisualStyleBackColor = true;
            this.btnRemoveSublayer.Click += new System.EventHandler(this.btnRemoveSublayer_Click);
            // 
            // btnAddSublayer
            // 
            this.btnAddSublayer.Location = new System.Drawing.Point(6, 175);
            this.btnAddSublayer.Name = "btnAddSublayer";
            this.btnAddSublayer.Size = new System.Drawing.Size(109, 23);
            this.btnAddSublayer.TabIndex = 29;
            this.btnAddSublayer.Text = "Add Sublayer";
            this.btnAddSublayer.UseVisualStyleBackColor = true;
            this.btnAddSublayer.Click += new System.EventHandler(this.btnAddSublayer_Click);
            // 
            // btnLayerMoveDown
            // 
            this.btnLayerMoveDown.Location = new System.Drawing.Point(121, 204);
            this.btnLayerMoveDown.Name = "btnLayerMoveDown";
            this.btnLayerMoveDown.Size = new System.Drawing.Size(109, 23);
            this.btnLayerMoveDown.TabIndex = 28;
            this.btnLayerMoveDown.Text = "Move Down";
            this.btnLayerMoveDown.UseVisualStyleBackColor = true;
            // 
            // btnLayerMoveUp
            // 
            this.btnLayerMoveUp.Location = new System.Drawing.Point(6, 204);
            this.btnLayerMoveUp.Name = "btnLayerMoveUp";
            this.btnLayerMoveUp.Size = new System.Drawing.Size(109, 23);
            this.btnLayerMoveUp.TabIndex = 27;
            this.btnLayerMoveUp.Text = "Move Up";
            this.btnLayerMoveUp.UseVisualStyleBackColor = true;
            // 
            // lsLayers
            // 
            this.lsLayers.FormattingEnabled = true;
            this.lsLayers.Location = new System.Drawing.Point(6, 6);
            this.lsLayers.Name = "lsLayers";
            this.lsLayers.Size = new System.Drawing.Size(228, 134);
            this.lsLayers.TabIndex = 26;
            this.lsLayers.SelectedIndexChanged += new System.EventHandler(this.lsLayers_SelectedIndexChanged);
            // 
            // btnLayerAttributes
            // 
            this.btnLayerAttributes.Location = new System.Drawing.Point(6, 233);
            this.btnLayerAttributes.Name = "btnLayerAttributes";
            this.btnLayerAttributes.Size = new System.Drawing.Size(224, 23);
            this.btnLayerAttributes.TabIndex = 25;
            this.btnLayerAttributes.Text = "Layer Attributes";
            this.btnLayerAttributes.UseVisualStyleBackColor = true;
            this.btnLayerAttributes.Click += new System.EventHandler(this.btnLayerAttributes_Click);
            // 
            // btnRemoveExtraLayer
            // 
            this.btnRemoveExtraLayer.Location = new System.Drawing.Point(121, 146);
            this.btnRemoveExtraLayer.Name = "btnRemoveExtraLayer";
            this.btnRemoveExtraLayer.Size = new System.Drawing.Size(109, 23);
            this.btnRemoveExtraLayer.TabIndex = 22;
            this.btnRemoveExtraLayer.Text = "Remove";
            this.btnRemoveExtraLayer.UseVisualStyleBackColor = true;
            this.btnRemoveExtraLayer.Click += new System.EventHandler(this.btnRemoveExtraLayer_Click);
            // 
            // btnAddExtraLayer
            // 
            this.btnAddExtraLayer.Location = new System.Drawing.Point(6, 146);
            this.btnAddExtraLayer.Name = "btnAddExtraLayer";
            this.btnAddExtraLayer.Size = new System.Drawing.Size(109, 23);
            this.btnAddExtraLayer.TabIndex = 21;
            this.btnAddExtraLayer.Text = "Add";
            this.btnAddExtraLayer.UseVisualStyleBackColor = true;
            this.btnAddExtraLayer.Click += new System.EventHandler(this.btnAddExtraLayer_Click);

            cbShowAllLayers = new CheckBox
            {
                Text = "Show All Layers"
            };
            //Link a CheckedChanged event to a method.
            cbShowAllLayers.CheckedChanged += new EventHandler(cbShowAllLayers_CheckedChanged);
            cbShowAllLayers.Checked = true;
            //Make it 10 pixel after the last mnuToolBar item.
            cbShowAllLayers.Padding = new Padding(10, 0, 0, 0);
            mnuToolBar.Items.Add(new ToolStripControlHost(cbShowAllLayers));

            return tabLayers;
        }

        public void OnMapLoaded()
        {
            foreach (object ActiveLayer in Helper.GetLayersAndSubLayers())
            {
                lsLayers.Items.Add(ActiveLayer);
            }

            if (lsLayers.Items.Count > 0)
            {
                BattleMapViewer.SelectedListLayerIndex = 0;
            }
        }

        public bool TabProcessCmdKey(ref Message msg, Keys keyData)
        {

            if (keyData == Keys.Q)
            {
                float NextZ = ActiveMap.CursorPosition.Z + 1;

                if (NextZ >= lsLayers.Items.Count)
                {
                    NextZ = lsLayers.Items.Count - 1;
                }

                ActiveMap.CursorPosition.Z = NextZ;
                SetLayerIndex();

                return true;
            }
            else if (keyData == Keys.E)
            {
                float NextZ = ActiveMap.CursorPosition.Z - 1;

                if (NextZ < 0)
                {
                    NextZ = 0;
                }

                ActiveMap.CursorPosition.Z = NextZ;
                SetLayerIndex();

                return true;
            }

            return false;
        }

        public void TabOnMouseDown(MouseEventArgs e)
        {
        }

        public void TabOnMouseUp(MouseEventArgs e)
        {
        }

        public void OnMouseMove(MouseEventArgs e)
        {
        }

        public void OnMapResize(int NewMapSizeX, int NewMapSizeY)
        {
        }

        public void DrawInfo(ToolStripStatusLabel tslInformation)
        {
        }

        public void DrawMap(CustomSpriteBatch g, GraphicsDevice GraphicsDevice)
        {
            BattleMapViewer.DrawMap();
        }

        private void SetLayerIndex()
        {
            AllowEvents = false;
            int LastTopLayerIndex = -1;

            foreach (object ActiveLayer in Helper.GetLayersAndSubLayers())
            {
                if (LastTopLayerIndex == (int)ActiveMap.CursorPosition.Z)
                {
                    lsLayers.SelectedIndex = LastTopLayerIndex;
                    if (!cbShowAllLayers.Checked)
                    {
                        ActiveMap.ShowLayerIndex = LastTopLayerIndex;
                    }
                    return;
                }

                if (!(ActiveLayer is ISubMapLayer))
                {
                    ++LastTopLayerIndex;
                }
            }

            lsLayers.SelectedIndex = LastTopLayerIndex;
            if (!cbShowAllLayers.Checked)
            {
                ActiveMap.ShowLayerIndex = LastTopLayerIndex;
            }
            AllowEvents = true;
        }

        private void cbShowAllLayers_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveMap != null)
            {
                if (cbShowAllLayers.Checked)
                {
                    ActiveMap.ShowLayerIndex = -1;
                }
                else
                {
                    ActiveMap.ShowLayerIndex = BattleMapViewer.GetRealTopLayerIndex(lsLayers.SelectedIndex);
                }
            }
        }

        private void btnAddExtraLayer_Click(object sender, EventArgs e)
        {
            Rectangle DefaultTile = BattleMapViewer.TilesetViewer.ListTileBrush[0];
            Terrain PresetTerrain = ActiveMap.ListTilesetPreset[BattleMapViewer.SelectedTilesetIndex].ArrayTerrain[DefaultTile.X / ActiveMap.TileSize.X, DefaultTile.Y / ActiveMap.TileSize.Y];
            DrawableTile PresetTile = ActiveMap.ListTilesetPreset[BattleMapViewer.SelectedTilesetIndex].ArrayTiles[DefaultTile.X / ActiveMap.TileSize.X, DefaultTile.Y / ActiveMap.TileSize.Y];

            lsLayers.Items.Add(Helper.CreateNewLayer(PresetTerrain, PresetTile));
        }

        private void btnRemoveExtraLayer_Click(object sender, EventArgs e)
        {
            if (lsLayers.SelectedIndex >= 0)
            {
                int Index = lsLayers.SelectedIndex;
                Helper.RemoveLayer(Index);
                lsLayers.Items.RemoveAt(Index);
                lsLayers.SelectedIndex = lsLayers.Items.Count - 1;
            }
        }

        private void btnAddSublayer_Click(object sender, EventArgs e)
        {
            if (lsLayers.SelectedIndex >= 0)
            {
                for (int L = lsLayers.SelectedIndex; L >= 0; --L)
                {
                    if (lsLayers.Items[L] is BaseMapLayer)
                    {
                        lsLayers.Items.Insert(L + 1, Helper.CreateNewSubLayer((BaseMapLayer)lsLayers.Items[L]));
                        break;
                    }
                }
            }
        }

        private void btnRemoveSublayer_Click(object sender, EventArgs e)
        {
            if (lsLayers.SelectedIndex >= 0)
            {
                for (int L = lsLayers.SelectedIndex; L >= 0; --L)
                {
                    if (lsLayers.Items[L] is BaseMapLayer)
                    {
                        Helper.RemoveSubLayer((BaseMapLayer)lsLayers.Items[L], (ISubMapLayer)lsLayers.Items[lsLayers.SelectedIndex]);
                        lsLayers.Items.RemoveAt(lsLayers.SelectedIndex);
                    }
                }
            }
        }

        private void btnLayerAttributes_Click(object sender, EventArgs e)
        {
            if (lsLayers.SelectedIndex >= 0)
            {
                Helper.EditLayer(lsLayers.SelectedIndex);
            }
        }

        private void lsLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            BattleMapViewer.SelectedListLayerIndex = lsLayers.SelectedIndex;

            if (!AllowEvents)
            {
                return;
            }

            if (lsLayers.SelectedIndex >= 0)
            {
                if (cbShowAllLayers.Checked)
                {
                    ActiveMap.ShowLayerIndex = -1;
                }
                else
                {
                    ActiveMap.ShowLayerIndex = BattleMapViewer.GetRealTopLayerIndex(lsLayers.SelectedIndex);
                    ActiveMap.CursorPosition.Z = ActiveMap.ShowLayerIndex;
                }
            }
        }
    }
}
