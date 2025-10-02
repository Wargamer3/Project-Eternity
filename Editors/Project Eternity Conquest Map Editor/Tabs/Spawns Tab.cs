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

namespace ProjectEternity.Editors.MapEditor
{
    public class SpawnsTab : IMapEditorTab
    {
        private TabPage tabSpawns;
        private ComboBox cbFactions;
        private ComboBox cbMoveType;
        private ListView lvUnits;
        private PropertyGrid pgUnit;

        private ImageList imageList;

        private List<UnitConquest> ListFactionUnit;

        public BattleMapViewerControl BattleMapViewer { get; set; }
        public IMapHelper Helper { get; set; }
        private ConquestMap ActiveMap;

        public TabPage InitTab(MenuStrip mnuToolBar)
        {
            ListFactionUnit = new List<UnitConquest>();

            ConquestSpawnUserControl SpawnControl = new ConquestSpawnUserControl();
            tabSpawns = SpawnControl.tabControl1.TabPages[0];

            cbFactions = SpawnControl.cbFactions;
            cbMoveType = SpawnControl.cbMoveType;
            lvUnits = SpawnControl.lvUnits;
            pgUnit = SpawnControl.pgUnit;
            imageList = new ImageList();
            imageList.ImageSize = new Size(32, 32);
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            lvUnits.LargeImageList = imageList;

            cbFactions.SelectedIndexChanged += cbFactions_SelectedIndexChanged;
            cbMoveType.SelectedIndexChanged += cbMoveType_SelectedIndexChanged;
            lvUnits.SelectedIndexChanged += lvUnits_SelectedIndexChanged;

            return tabSpawns;
        }

        public void OnMapLoaded()
        {
            ActiveMap = (ConquestMap)BattleMapViewer.ActiveMap;

            string[] ArrayFactions = Directory.GetDirectories("Content/Conquest/Units", "*.*", SearchOption.TopDirectoryOnly);
            foreach (var ActiveFaction in ArrayFactions)
            {
                string FactionName = ActiveFaction.Substring(23);
                if (FactionName == "Map Sprite" || FactionName == "Unit Sprite")
                {
                    continue;
                }

                cbFactions.Items.Add(FactionName);
            }

            if (cbFactions.Items.Count > 0)
            {
                cbFactions.SelectedIndex = 0;
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

                if (lvUnits.SelectedIndices.Count > 0)
                {
                    int UnitIndex = lvUnits.SelectedIndices[0];

                    int TopLayerIndex = BattleMapViewer.GetRealTopLayerIndex(BattleMapViewer.SelectedListLayerIndex);
                    MapLayer TopLayer = (MapLayer)Helper.GetLayersAndSubLayers()[BattleMapViewer.SelectedListLayerIndex];
                    UnitSpawn NewUnit = new UnitSpawn(new UnitConquest(ListFactionUnit[UnitIndex].RelativePath, GameScreens.GameScreen.ContentFallback, null, null), new Microsoft.Xna.Framework.Point(GridX, GridY), (byte)TopLayerIndex);
                    pgUnit.SelectedObject = NewUnit;

                    NewSpawn(GridX, GridY, TopLayer, NewUnit);
                }
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

        private void cbFactions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFactions.SelectedIndex < 0)
            {
                return;
            }

            ListFactionUnit.Clear();
            lvUnits.Items.Clear();
            cbMoveType.Items.Clear();
            cbMoveType.Items.Add("All");
            imageList.Images.Clear();

            string[] ArrayUnitTypes = Directory.GetFiles("Content/Conquest/Units/" + cbFactions.Text, "*.peu*", SearchOption.TopDirectoryOnly);

            foreach (var ActiveUnit in ArrayUnitTypes)
            {
                ListFactionUnit.Add(new UnitConquest(ActiveUnit.Substring(0, ActiveUnit.Length - 4).Substring(23), GameScreens.GameScreen.ContentFallback, null, null));
            }

            foreach (UnitConquest ActiveUnit in ListFactionUnit)
            {
                cbMoveType.Items.Add(ActiveUnit.MovementType);
            }

            cbMoveType.SelectedIndex = 0;
        }

        private void cbMoveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMoveType.SelectedIndex < 0)
            {
                return;
            }

            lvUnits.Items.Clear();

            int ImageIndex = 0;
            foreach (UnitConquest ActiveUnit in ListFactionUnit)
            {
                if (cbMoveType.Text == "All" || ActiveUnit.MovementType == cbMoveType.Text)
                {
                    lvUnits.Items.Add(ActiveUnit.ItemName, ImageIndex);
                    lvUnits.Items[ImageIndex++].Text = ActiveUnit.ItemName;
                    imageList.Images.Add("itemImageKey", Texture2Image(ActiveUnit.SpriteMap));
                }
            }
        }

        private void lvUnits_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void NewSpawn(int X, int Y, MapLayer TopLayer, UnitSpawn Spawn)
        {
            //Loop in the SpawnPoint list to find if a SpawnPoint already exist at the X, Y position.
            for (int S = 0; S < TopLayer.ListUnitSpawn.Count; S++)
            {//If it exist.
                if (TopLayer.ListUnitSpawn[S].SpawnPositionX == X && TopLayer.ListUnitSpawn[S].SpawnPositionY == Y)
                {
                    //Delete it.
                    TopLayer.ListUnitSpawn.RemoveAt(S);
                }
            }
            if (Spawn != null)
            {
                //Add the new SpawnPoint.
                TopLayer.ListUnitSpawn.Add(Spawn);
            }
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
