using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.MapEditor
{
    public class ZoneTab : IMapEditorTab
    {
        private TabPage tabZones;
        private Button btnAddZoneOval;
        private ListBox lsZones;
        private Button btnRemoveZone;
        private Button btnAddZoneRectangle;
        private Button btnEditZone;
        private PropertyGrid pgZoneProperties;
        private Button btnAddZoneFullMap;

        public BattleMapViewerControl BattleMapViewer { get; set; }
        public IMapHelper Helper { get; set; }
        private BattleMap ActiveMap => BattleMapViewer.ActiveMap;

        public TabPage InitTab(MenuStrip mnuToolBar)
        {
            this.tabZones = new TabPage();
            this.btnAddZoneFullMap = new Button();
            this.pgZoneProperties = new PropertyGrid();
            this.btnEditZone = new Button();
            this.btnAddZoneOval = new Button();
            this.lsZones = new ListBox();
            this.btnRemoveZone = new Button();
            this.btnAddZoneRectangle = new Button();
            this.tabZones.SuspendLayout();

            // 
            // tabZones
            // 
            this.tabZones.Controls.Add(this.btnAddZoneFullMap);
            this.tabZones.Controls.Add(this.pgZoneProperties);
            this.tabZones.Controls.Add(this.btnEditZone);
            this.tabZones.Controls.Add(this.btnAddZoneOval);
            this.tabZones.Controls.Add(this.lsZones);
            this.tabZones.Controls.Add(this.btnRemoveZone);
            this.tabZones.Controls.Add(this.btnAddZoneRectangle);
            this.tabZones.Location = new System.Drawing.Point(4, 22);
            this.tabZones.Name = "tabZones";
            this.tabZones.Padding = new Padding(3);
            this.tabZones.Size = new System.Drawing.Size(325, 497);
            this.tabZones.TabIndex = 6;
            this.tabZones.Text = "Zones";
            this.tabZones.UseVisualStyleBackColor = true;
            // 
            // btnAddZoneFullMap
            // 
            this.btnAddZoneFullMap.Location = new System.Drawing.Point(6, 204);
            this.btnAddZoneFullMap.Name = "btnAddZoneFullMap";
            this.btnAddZoneFullMap.Size = new System.Drawing.Size(109, 23);
            this.btnAddZoneFullMap.TabIndex = 33;
            this.btnAddZoneFullMap.Text = "Add Full Map";
            this.btnAddZoneFullMap.UseVisualStyleBackColor = true;
            this.btnAddZoneFullMap.Click += new System.EventHandler(this.btnAddZoneFullMap_Click);
            // 
            // pgZoneProperties
            // 
            this.pgZoneProperties.Location = new System.Drawing.Point(6, 249);
            this.pgZoneProperties.Name = "pgZoneProperties";
            this.pgZoneProperties.PropertySort = PropertySort.Categorized;
            this.pgZoneProperties.Size = new System.Drawing.Size(228, 242);
            this.pgZoneProperties.TabIndex = 32;
            this.pgZoneProperties.ToolbarVisible = false;
            // 
            // btnEditZone
            // 
            this.btnEditZone.Location = new System.Drawing.Point(121, 175);
            this.btnEditZone.Name = "btnEditZone";
            this.btnEditZone.Size = new System.Drawing.Size(109, 23);
            this.btnEditZone.TabIndex = 31;
            this.btnEditZone.Text = "Edit";
            this.btnEditZone.UseVisualStyleBackColor = true;
            this.btnEditZone.Click += new EventHandler(this.btnEditZone_Click);
            // 
            // btnAddZoneOval
            // 
            this.btnAddZoneOval.Location = new System.Drawing.Point(6, 175);
            this.btnAddZoneOval.Name = "btnAddZoneOval";
            this.btnAddZoneOval.Size = new System.Drawing.Size(109, 23);
            this.btnAddZoneOval.TabIndex = 30;
            this.btnAddZoneOval.Text = "Add Oval";
            this.btnAddZoneOval.UseVisualStyleBackColor = true;
            this.btnAddZoneOval.Click += new EventHandler(this.btnAddZoneOval_Click);
            // 
            // lsZones
            // 
            this.lsZones.FormattingEnabled = true;
            this.lsZones.Location = new System.Drawing.Point(6, 6);
            this.lsZones.Name = "lsZones";
            this.lsZones.Size = new System.Drawing.Size(228, 134);
            this.lsZones.TabIndex = 29;
            this.lsZones.SelectedIndexChanged += new EventHandler(this.lsZones_SelectedIndexChanged);
            // 
            // btnRemoveZone
            // 
            this.btnRemoveZone.Location = new System.Drawing.Point(121, 146);
            this.btnRemoveZone.Name = "btnRemoveZone";
            this.btnRemoveZone.Size = new System.Drawing.Size(109, 23);
            this.btnRemoveZone.TabIndex = 28;
            this.btnRemoveZone.Text = "Remove";
            this.btnRemoveZone.UseVisualStyleBackColor = true;
            this.btnRemoveZone.Click += new EventHandler(this.btnRemoveZone_Click);
            // 
            // btnAddZoneRectangle
            // 
            this.btnAddZoneRectangle.Location = new System.Drawing.Point(6, 146);
            this.btnAddZoneRectangle.Name = "btnAddZoneRectangle";
            this.btnAddZoneRectangle.Size = new System.Drawing.Size(109, 23);
            this.btnAddZoneRectangle.TabIndex = 27;
            this.btnAddZoneRectangle.Text = "Add Rectangle";
            this.btnAddZoneRectangle.UseVisualStyleBackColor = true;
            this.btnAddZoneRectangle.Click += new EventHandler(this.btnAddZoneRectangle_Click);
            this.tabZones.ResumeLayout(false);

            return tabZones;
        }

        public void OnMapLoaded()
        {
        }

        public bool TabProcessCmdKey(ref Message msg, Keys keyData)
        {
            return false;
        }

        public void TabOnMouseDown(MouseEventArgs e)
        {
        }

        public void TabOnMouseUp(MouseEventArgs e)
        {
        }

        public void OnMouseMove(MouseEventArgs e, int MouseX, int MouseY)
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

        private void btnAddZoneRectangle_Click(object sender, EventArgs e)
        {
            MapZone NewZone = Helper.CreateNewZone(ZoneShape.ZoneShapeTypes.Rectangle);
            ActiveMap.MapEnvironment.ListMapZone.Add(NewZone);
            lsZones.Items.Add("Zone");
            pgZoneProperties.SelectedObject = NewZone;
            lsZones.SelectedIndex = lsZones.Items.Count - 1;
        }

        private void btnAddZoneOval_Click(object sender, EventArgs e)
        {
            MapZone NewZone = Helper.CreateNewZone(ZoneShape.ZoneShapeTypes.Oval);
            ActiveMap.MapEnvironment.ListMapZone.Add(NewZone);
            lsZones.Items.Add("Zone");
            pgZoneProperties.SelectedObject = NewZone;
            lsZones.SelectedIndex = lsZones.Items.Count - 1;
        }

        private void btnAddZoneFullMap_Click(object sender, EventArgs e)
        {
            MapZone NewZone = Helper.CreateNewZone(ZoneShape.ZoneShapeTypes.Full);
            ActiveMap.MapEnvironment.ListMapZone.Add(NewZone);
            lsZones.Items.Add("Zone");
            pgZoneProperties.SelectedObject = NewZone;
            lsZones.SelectedIndex = lsZones.Items.Count - 1;
        }

        private void btnRemoveZone_Click(object sender, EventArgs e)
        {
            if (lsZones.SelectedIndex >= 0)
            {
                ActiveMap.MapEnvironment.ListMapZone.RemoveAt(lsZones.SelectedIndex);
                lsZones.Items.RemoveAt(lsZones.SelectedIndex);
            }
        }

        private void btnEditZone_Click(object sender, EventArgs e)
        {
            if (lsZones.SelectedIndex >= 0)
            {
                new ZoneEditor(ActiveMap.MapEnvironment.ListMapZone[lsZones.SelectedIndex]).ShowDialog();
            }
        }

        private void lsZones_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsZones.SelectedIndex >= 0)
            {
                pgZoneProperties.SelectedObject = ActiveMap.MapEnvironment.ListMapZone[lsZones.SelectedIndex];
            }
        }
    }
}
