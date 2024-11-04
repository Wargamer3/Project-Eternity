using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Editors.MapEditor
{
    public class PropTab : IMapEditorTab
    {
        private TabPage tabProps;
        private SplitContainer PropsContainer;
        private PropertyGrid pgPropProperties;
        private TabControl tabPropsChoices;
        private TabPage tabInteractiveProps;
        private ListBox lsInteractiveProps;
        private TabPage tabPhysicalProps;
        private TabPage tabVisualProps;
        private ListBox lsVisualProps;
        private ListBox lsPhysicalProps;

        public BattleMapViewerControl BattleMapViewer { get; set; }
        public IMapHelper Helper { get; set; }
        private BattleMap ActiveMap => BattleMapViewer.ActiveMap;

        public TabPage InitTab(MenuStrip mnuToolBar)
        {
            this.tabProps = new TabPage();
            this.PropsContainer = new SplitContainer();
            this.tabPropsChoices = new TabControl();
            this.tabInteractiveProps = new TabPage();
            this.lsInteractiveProps = new ListBox();
            this.tabPhysicalProps = new TabPage();
            this.lsPhysicalProps = new ListBox();
            this.tabVisualProps = new TabPage();
            this.lsVisualProps = new ListBox();
            this.pgPropProperties = new PropertyGrid();

            this.tabProps.SuspendLayout();
            // 
            // tabProps
            // 
            this.tabProps.Controls.Add(this.PropsContainer);
            this.tabProps.Location = new System.Drawing.Point(4, 22);
            this.tabProps.Name = "tabProps";
            this.tabProps.Padding = new System.Windows.Forms.Padding(3);
            this.tabProps.Size = new System.Drawing.Size(325, 497);
            this.tabProps.TabIndex = 5;
            this.tabProps.Text = "Props";
            this.tabProps.UseVisualStyleBackColor = true;
            // 
            // PropsContainer
            // 
            this.PropsContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PropsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PropsContainer.Location = new System.Drawing.Point(3, 3);
            this.PropsContainer.Name = "PropsContainer";
            this.PropsContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // PropsContainer.Panel1
            // 
            this.PropsContainer.Panel1.Controls.Add(this.tabPropsChoices);
            // 
            // PropsContainer.Panel2
            // 
            this.PropsContainer.Panel2.Controls.Add(this.pgPropProperties);
            this.PropsContainer.Size = new System.Drawing.Size(319, 491);
            this.PropsContainer.SplitterDistance = 241;
            this.PropsContainer.TabIndex = 8;
            // 
            // tabPropsChoices
            // 
            this.tabPropsChoices.Controls.Add(this.tabInteractiveProps);
            this.tabPropsChoices.Controls.Add(this.tabPhysicalProps);
            this.tabPropsChoices.Controls.Add(this.tabVisualProps);
            this.tabPropsChoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPropsChoices.Location = new System.Drawing.Point(0, 0);
            this.tabPropsChoices.Multiline = true;
            this.tabPropsChoices.Name = "tabPropsChoices";
            this.tabPropsChoices.SelectedIndex = 0;
            this.tabPropsChoices.Size = new System.Drawing.Size(315, 237);
            this.tabPropsChoices.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabPropsChoices.TabIndex = 0;
            // 
            // tabInteractiveProps
            // 
            this.tabInteractiveProps.Controls.Add(this.lsInteractiveProps);
            this.tabInteractiveProps.Location = new System.Drawing.Point(4, 22);
            this.tabInteractiveProps.Name = "tabInteractiveProps";
            this.tabInteractiveProps.Size = new System.Drawing.Size(307, 211);
            this.tabInteractiveProps.TabIndex = 2;
            this.tabInteractiveProps.Text = "Interactive";
            this.tabInteractiveProps.UseVisualStyleBackColor = true;
            // 
            // lsInteractiveProps
            // 
            this.lsInteractiveProps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsInteractiveProps.FormattingEnabled = true;
            this.lsInteractiveProps.Location = new System.Drawing.Point(0, 0);
            this.lsInteractiveProps.Name = "lsInteractiveProps";
            this.lsInteractiveProps.Size = new System.Drawing.Size(307, 211);
            this.lsInteractiveProps.TabIndex = 0;
            // 
            // tabPhysicalProps
            // 
            this.tabPhysicalProps.Controls.Add(this.lsPhysicalProps);
            this.tabPhysicalProps.Location = new System.Drawing.Point(4, 22);
            this.tabPhysicalProps.Name = "tabPhysicalProps";
            this.tabPhysicalProps.Size = new System.Drawing.Size(222, 211);
            this.tabPhysicalProps.TabIndex = 0;
            this.tabPhysicalProps.Text = "Physical";
            this.tabPhysicalProps.UseVisualStyleBackColor = true;
            // 
            // lsPhysicalProps
            // 
            this.lsPhysicalProps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsPhysicalProps.FormattingEnabled = true;
            this.lsPhysicalProps.Location = new System.Drawing.Point(0, 0);
            this.lsPhysicalProps.Name = "lsPhysicalProps";
            this.lsPhysicalProps.Size = new System.Drawing.Size(222, 211);
            this.lsPhysicalProps.TabIndex = 1;
            // 
            // tabVisualProps
            // 
            this.tabVisualProps.Controls.Add(this.lsVisualProps);
            this.tabVisualProps.Location = new System.Drawing.Point(4, 22);
            this.tabVisualProps.Name = "tabVisualProps";
            this.tabVisualProps.Size = new System.Drawing.Size(222, 211);
            this.tabVisualProps.TabIndex = 1;
            this.tabVisualProps.Text = "Visual";
            this.tabVisualProps.UseVisualStyleBackColor = true;
            // 
            // lsVisualProps
            // 
            this.lsVisualProps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsVisualProps.FormattingEnabled = true;
            this.lsVisualProps.Location = new System.Drawing.Point(0, 0);
            this.lsVisualProps.Name = "lsVisualProps";
            this.lsVisualProps.Size = new System.Drawing.Size(222, 211);
            this.lsVisualProps.TabIndex = 0;
            // 
            // pgPropProperties
            // 
            this.pgPropProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgPropProperties.Location = new System.Drawing.Point(0, 0);
            this.pgPropProperties.Name = "pgPropProperties";
            this.pgPropProperties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgPropProperties.Size = new System.Drawing.Size(315, 242);
            this.pgPropProperties.TabIndex = 0;
            this.pgPropProperties.ToolbarVisible = false;
            this.tabProps.ResumeLayout(false);
            return tabProps;
        }

        public void OnMapLoaded()
        {
            foreach (InteractiveProp Instance in ActiveMap.DicInteractiveProp.Values)
            {
                if (Instance.PropCategory == InteractiveProp.PropCategories.Interactive)
                {
                    lsInteractiveProps.Items.Add(Instance);
                }
                else if (Instance.PropCategory == InteractiveProp.PropCategories.Physical)
                {
                    lsPhysicalProps.Items.Add(Instance);
                }
                else if (Instance.PropCategory == InteractiveProp.PropCategories.Visual)
                {
                    lsVisualProps.Items.Add(Instance);
                }
            }
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
            int GridX = (int)(ActiveMap.CursorPosition.X) / ActiveMap.TileSize.X;
            int GridY = (int)(ActiveMap.CursorPosition.Y) / ActiveMap.TileSize.Y;

            if (e.Button == MouseButtons.Left)
            {
                HandleProps(GridX, GridY);
            }
            else if (e.Button == MouseButtons.Right)
            {
                RemoveProps(GridX, GridY);
            }
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            int GridX = (int)(ActiveMap.CursorPosition.X) / ActiveMap.TileSize.X;
            int GridY = (int)(ActiveMap.CursorPosition.Y) / ActiveMap.TileSize.Y;

            if (e.Button == MouseButtons.Left)
            {
                HandleProps(GridX, GridY);
            }
            else if (e.Button == MouseButtons.Right)
            {
                RemoveProps(GridX, GridY);
            }
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

        private void HandleProps(int X, int Y)
        {
            if (ActiveMap.TileSize.X != 0)
            {
                int TopLayerIndex = BattleMapViewer.GetRealTopLayerIndex(BattleMapViewer.SelectedListLayerIndex);
                BaseMapLayer TopLayer = Helper.GetLayersAndSubLayers()[BattleMapViewer.SelectedListLayerIndex];

                //Loop in the Prop list to find if a Prop already exist at the X, Y position.
                for (int P = 0; P < TopLayer.ListProp.Count; P++)
                {
                    if (TopLayer.ListProp[P].Position.X == X && TopLayer.ListProp[P].Position.Y == Y)
                    {
                        pgPropProperties.SelectedObject = TopLayer.ListProp[P];
                        return;
                    }
                }

                InteractiveProp ActiveProp = null;
                if (tabPropsChoices.SelectedIndex == 0 && lsInteractiveProps.SelectedItem != null)
                {
                    ActiveProp = (InteractiveProp)lsInteractiveProps.SelectedItem;
                }
                else if (tabPropsChoices.SelectedIndex == 1 && lsPhysicalProps.SelectedItem != null)
                {
                    ActiveProp = (InteractiveProp)lsPhysicalProps.SelectedItem;
                }
                else if (tabPropsChoices.SelectedIndex == 2 && lsVisualProps.SelectedItem != null)
                {
                    ActiveProp = (InteractiveProp)lsVisualProps.SelectedItem;
                }
                else
                {
                    return;
                }

                ActiveProp = ActiveProp.Copy(new Vector3(X * ActiveMap.TileSize.X + ActiveMap.TileSize.X / 2, Y * ActiveMap.TileSize.Y + ActiveMap.TileSize.Y / 2, 0), TopLayerIndex);
                pgPropProperties.SelectedObject = ActiveProp;

                TopLayer.ListProp.Add(ActiveProp);
            }
        }

        private void RemoveProps(int X, int Y)
        {
            if (ActiveMap.TileSize.X != 0)
            {
                int TopLayerIndex = BattleMapViewer.GetRealTopLayerIndex(BattleMapViewer.SelectedListLayerIndex);
                BaseMapLayer TopLayer = Helper.GetLayersAndSubLayers()[BattleMapViewer.SelectedListLayerIndex];

                //Loop in the Prop list to find if a Prop already exist at the X, Y position.
                for (int P = 0; P < TopLayer.ListProp.Count; P++)
                {
                    if (Math.Floor(TopLayer.ListProp[P].Position.X) == X && Math.Floor(TopLayer.ListProp[P].Position.Y) == Y)
                    {
                        TopLayer.ListProp.RemoveAt(P);
                        break;
                    }
                }
            }
        }
    }
}
