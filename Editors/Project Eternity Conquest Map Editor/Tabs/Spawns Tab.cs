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
using Microsoft.Xna.Framework;

namespace ProjectEternity.Editors.MapEditor
{
    public class SpawnsTab : IMapEditorTab
    {
        private TabPage tabSpawns;
        private NumericUpDown txtSpawnsPlayer;
        private ListView lvUnits;
        private ListView lvBuildings;

        private ImageList UnitImageList;
        private ImageList BuildingImageList;

        private List<UnitConquest> ListFactionUnit;
        private List<BuildingConquest> ListFactionBuilding;

        public BattleMapViewerControl BattleMapViewer { get; set; }
        public IMapHelper Helper { get; set; }
        private ConquestMap ActiveMap;

        public TabPage InitTab(MenuStrip mnuToolBar)
        {
            ListFactionUnit = new List<UnitConquest>();
            ListFactionBuilding = new List<BuildingConquest>();

            ExtraTabsUserControl SpawnControl = new ExtraTabsUserControl();
            tabSpawns = SpawnControl.tabControl1.TabPages[3];

            txtSpawnsPlayer = SpawnControl.txtSpawnsPlayer;

            lvUnits = SpawnControl.lvSpawnsUnits;
            UnitImageList = new ImageList();
            UnitImageList.ImageSize = new Size(32, 32);
            UnitImageList.ColorDepth = ColorDepth.Depth32Bit;
            lvUnits.LargeImageList = UnitImageList;

            lvBuildings = SpawnControl.lvSpawnsBuildings;
            BuildingImageList = new ImageList();
            BuildingImageList.ImageSize = new Size(32, 32);
            BuildingImageList.ColorDepth = ColorDepth.Depth32Bit;
            lvBuildings.LargeImageList = BuildingImageList;

            lvUnits.SelectedIndexChanged += lvUnits_SelectedIndexChanged;

            return tabSpawns;
        }

        public void OnMapLoaded()
        {
            ActiveMap = (ConquestMap)BattleMapViewer.ActiveMap;

            string[] ArrayUnitTypes = Directory.GetFiles("Content/Conquest/Units/Neutral", "*.peu*", SearchOption.TopDirectoryOnly);

            foreach (var ActiveUnit in ArrayUnitTypes)
            {
                ListFactionUnit.Add(new UnitConquest(ActiveUnit.Substring(0, ActiveUnit.Length - 4).Substring(23), GameScreens.GameScreen.ContentFallback, null, null));
            }

            int ImageIndex = 0;
            foreach (UnitConquest ActiveUnit in ListFactionUnit)
            {
                lvUnits.Items.Add(ActiveUnit.ItemName, ImageIndex);
                lvUnits.Items[ImageIndex++].Text = ActiveUnit.ItemName;
                UnitImageList.Images.Add("itemImageKey", Texture2Image(ActiveUnit.SpriteMap));
            }

            string[] ArrayBuildingTypes = Directory.GetFiles("Content/Conquest/Buildings/Neutral", "*.peb*", SearchOption.TopDirectoryOnly);

            foreach (var ActiveBuilding in ArrayBuildingTypes)
            {
                ListFactionBuilding.Add(new BuildingConquest(ActiveBuilding.Substring(0, ActiveBuilding.Length - 4).Substring(27), GameScreens.GameScreen.ContentFallback, null, null, null));
            }

            lvBuildings.Items.Clear();

            ImageIndex = 0;
            foreach (BuildingConquest ActiveUnit in ListFactionBuilding)
            {
                lvBuildings.Items.Add(ActiveUnit.Name, ImageIndex);
                lvBuildings.Items[ImageIndex++].Text = ActiveUnit.Name;
                BuildingImageList.Images.Add("itemImageKey", Texture2Image(ActiveUnit.SpriteMap.ActiveSprite));
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

                    EventPoint NewUnit = null;

                    //Loop in the SpawnPoint list to find if a SpawnPoint already exist at the X, Y position.
                    for (int S = 0; S < TopLayer.ListMultiplayerSpawns.Count; S++)
                    {
                        if (TopLayer.ListMultiplayerSpawns[S].Position.X == GridX && TopLayer.ListMultiplayerSpawns[S].Position.Y == GridY)
                        {
                            NewUnit = TopLayer.ListMultiplayerSpawns[S];
                        }
                    }

                    if (NewUnit == null)
                    {
                        TopLayer.ListMultiplayerSpawns.Add(new ConquestEventPoint(new Vector3(GridX, GridY, BattleMapViewer.SelectedListLayerIndex), txtSpawnsPlayer.Text, 0, 0, 0, "Unit", ListFactionUnit[UnitIndex].RelativePath));
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                int GridX = (int)(ActiveMap.CursorPosition.X) / ActiveMap.TileSize.X;
                int GridY = (int)(ActiveMap.CursorPosition.Y) / ActiveMap.TileSize.Y;
                MapLayer TopLayer = (MapLayer)Helper.GetLayersAndSubLayers()[BattleMapViewer.SelectedListLayerIndex];

                for (int S = 0; S < TopLayer.ListMultiplayerSpawns.Count; S++)
                {
                    if (TopLayer.ListMultiplayerSpawns[S].Position.X == GridX && TopLayer.ListMultiplayerSpawns[S].Position.Y == GridY)
                    {
                        TopLayer.ListMultiplayerSpawns.RemoveAt(S);
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
