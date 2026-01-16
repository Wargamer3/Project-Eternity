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
    public class UnitsTab : IMapEditorTab
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

            ExtraTabsUserControl SpawnControl = new ExtraTabsUserControl();
            tabSpawns = SpawnControl.tabControl1.TabPages[0];

            cbFactions = SpawnControl.cbUnitsFactions;
            cbMoveType = SpawnControl.cbUnitsMoveType;
            lvUnits = SpawnControl.lvUnitsUnits;
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

            string[] ArrayFactions = Directory.GetFiles("Content/Conquest/Factions", "*.pef*", SearchOption.TopDirectoryOnly);
            foreach (var ActiveFaction in ArrayFactions)
            {
                string FactionName = ActiveFaction.Substring(0, ActiveFaction.Length - 4).Substring(26);

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

                    MapLayer TopLayer = (MapLayer)Helper.GetLayersAndSubLayers()[BattleMapViewer.SelectedListLayerIndex];

                    UnitSpawn NewUnit = null;

                    //Loop in the SpawnPoint list to find if a SpawnPoint already exist at the X, Y position.
                    for (int S = 0; S < TopLayer.ListUnitSpawn.Count; S++)
                    {
                        if (TopLayer.ListUnitSpawn[S].SpawnPositionX == GridX && TopLayer.ListUnitSpawn[S].SpawnPositionY == GridY)
                        {
                            NewUnit = TopLayer.ListUnitSpawn[S];
                        }
                    }

                    if (NewUnit == null)
                    {
                        NewUnit = new UnitSpawn(new UnitConquest(ListFactionUnit[UnitIndex].RelativePath, GameScreens.GameScreen.ContentFallback, null, null), new Microsoft.Xna.Framework.Point(GridX, GridY), (byte)BattleMapViewer.SelectedListLayerIndex);
                        TopLayer.ListUnitSpawn.Add(NewUnit);
                    }

                    pgUnit.SelectedObject = NewUnit;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                int GridX = (int)(ActiveMap.CursorPosition.X) / ActiveMap.TileSize.X;
                int GridY = (int)(ActiveMap.CursorPosition.Y) / ActiveMap.TileSize.Y;
                MapLayer TopLayer = (MapLayer)Helper.GetLayersAndSubLayers()[BattleMapViewer.SelectedListLayerIndex];

                for (int S = 0; S < TopLayer.ListUnitSpawn.Count; S++)
                {
                    if (TopLayer.ListUnitSpawn[S].SpawnPositionX == GridX && TopLayer.ListUnitSpawn[S].SpawnPositionY == GridY)
                    {
                        TopLayer.ListUnitSpawn.RemoveAt(S);
                    }
                }
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

            if (Directory.Exists("Content/Conquest/Units/" + cbFactions.Text))
            {
                string[] ArrayUnitTypes = Directory.GetFiles("Content/Conquest/Units/" + cbFactions.Text, "*.peu*", SearchOption.TopDirectoryOnly);

                foreach (var ActiveUnit in ArrayUnitTypes)
                {
                    ListFactionUnit.Add(new UnitConquest(ActiveUnit.Substring(0, ActiveUnit.Length - 4).Substring(23), GameScreens.GameScreen.ContentFallback, null, null));
                }
            }

            foreach (UnitConquest ActiveUnit in ListFactionUnit)
            {
                cbMoveType.Items.Add(ActiveUnit.MovementTypeIndex);
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
                if (cbMoveType.Text == "All" || ActiveUnit.MovementTypeIndex == cbMoveType.SelectedIndex)
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
