using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.Editors.ConquestMapEditor;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.ConquestMapScreen;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.MapEditor
{
    public class InfrastructureTab : IMapEditorTab
    {
        private TabPage tabInfrastructure;
        private ListView lvInfrastructure;

        private ImageList imageList;

        private List<UnitConquest> ListFactionUnit;

        public BattleMapViewerControl BattleMapViewer { get; set; }
        public IMapHelper Helper { get; set; }
        private ConquestMap ActiveMap;

        public TabPage InitTab(MenuStrip mnuToolBar)
        {
            ListFactionUnit = new List<UnitConquest>();

            ConquestSpawnUserControl SpawnControl = new ConquestSpawnUserControl();
            tabInfrastructure = SpawnControl.tabControl1.TabPages[2];

            this.lvInfrastructure = SpawnControl.lvInfrastructure;

            imageList = new ImageList();
            imageList.ImageSize = new Size(32, 32);
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            lvInfrastructure.LargeImageList = imageList;

            lvInfrastructure.SelectedIndexChanged += lvUnits_SelectedIndexChanged;

            return tabInfrastructure;
        }

        public void OnMapLoaded()
        {
            ActiveMap = (ConquestMap)BattleMapViewer.ActiveMap;
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
            }
            else if (e.Button == MouseButtons.Right)
            {
            }
        }

        public void OnMapResize(int NewMapSizeX, int NewMapSizeY)
        {
        }

        public void DrawInfo(ToolStripStatusLabel tslInformation)
        {
            tslInformation.Text += " Left click to place a new spawn point";
            tslInformation.Text += " Right click to remove a spawn point";
        }

        public void DrawMap(CustomSpriteBatch g, GraphicsDevice GraphicsDevice)
        {
            BattleMapViewer.DrawMap();
        }

        private void lvUnits_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public static Image Texture2Image(Texture2D sprTexture2D)
        {
            Image ReturnImage;
            using (MemoryStream MS = new MemoryStream())
            {
                sprTexture2D.SaveAsPng(MS, sprTexture2D.Width, sprTexture2D.Height);
                MS.Seek(0, SeekOrigin.Begin);
                ReturnImage = Image.FromStream(MS);
            }
            return ReturnImage;
        }
    }
}
