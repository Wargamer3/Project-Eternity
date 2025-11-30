using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Editors.ConquestMapEditor;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.ConquestMapScreen;

namespace ProjectEternity.Editors.MapEditor
{
    public class BuildingsTab : IMapEditorTab
    {
        private TabPage tabSpawns;
        private ComboBox cbFactions;
        private ListView lvBuildings;
        private PropertyGrid pgBuilding;

        private ImageList imageList;

        private List<BuildingConquest> ListFactionBuilding;

        public BattleMapViewerControl BattleMapViewer { get; set; }
        public IMapHelper Helper { get; set; }
        private ConquestMap ActiveMap;

        public TabPage InitTab(MenuStrip mnuToolBar)
        {
            ListFactionBuilding = new List<BuildingConquest>();

            ExtraTabsUserControl SpawnControl = new ExtraTabsUserControl();
            tabSpawns = SpawnControl.tabControl1.TabPages[1];

            cbFactions = SpawnControl.cbBuildingFaction;
            lvBuildings = SpawnControl.lvBuildings;
            pgBuilding = SpawnControl.pgBuilding;
            imageList = new ImageList();
            imageList.ImageSize = new Size(32, 32);
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            lvBuildings.LargeImageList = imageList;

            cbFactions.SelectedIndexChanged += cbFactions_SelectedIndexChanged;
            lvBuildings.SelectedIndexChanged += lvBuildings_SelectedIndexChanged;

            return tabSpawns;
        }

        public void OnMapLoaded()
        {
            ActiveMap = (ConquestMap)BattleMapViewer.ActiveMap;

            string[] ArrayFactions = Directory.GetDirectories("Content/Conquest/Buildings/", "*.*", SearchOption.TopDirectoryOnly);
            foreach (var ActiveFaction in ArrayFactions)
            {
                string FactionName = ActiveFaction.Substring(27);
                if (FactionName == "Map Sprites" || FactionName == "Menu Sprites")
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

                if (lvBuildings.SelectedIndices.Count > 0)
                {
                    int BuildingIndex = lvBuildings.SelectedIndices[0];

                    MapLayer TopLayer = (MapLayer)Helper.GetLayersAndSubLayers()[BattleMapViewer.SelectedListLayerIndex];
                    BuildingSpawn NewBuilding = new BuildingSpawn(new BuildingConquest(ListFactionBuilding[BuildingIndex].RelativePath, GameScreens.GameScreen.ContentFallback, null, null, null), new Microsoft.Xna.Framework.Point(GridX, GridY), (byte)BattleMapViewer.SelectedListLayerIndex);
                    NewBuilding.BuildingToSpawn.SpriteMap.Origin = new Microsoft.Xna.Framework.Vector2(NewBuilding.BuildingToSpawn.SpriteMap.SpriteWidth / 2, NewBuilding.BuildingToSpawn.SpriteMap.SpriteHeight - ActiveMap.TileSize.Y / 2);

                    pgBuilding.SelectedObject = NewBuilding;

                    NewSpawn(GridX, GridY, TopLayer, NewBuilding);
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

            ListFactionBuilding.Clear();
            lvBuildings.Items.Clear();
            imageList.Images.Clear();

            string[] ArrayBuildingTypes = Directory.GetFiles("Content/Conquest/Buildings/" + cbFactions.Text, "*.peb*", SearchOption.TopDirectoryOnly);

            foreach (var ActiveBuilding in ArrayBuildingTypes)
            {
                ListFactionBuilding.Add(new BuildingConquest(ActiveBuilding.Substring(0, ActiveBuilding.Length - 4).Substring(27), GameScreens.GameScreen.ContentFallback, null, null, null));
            }

            lvBuildings.Items.Clear();

            int ImageIndex = 0;
            foreach (BuildingConquest ActiveUnit in ListFactionBuilding)
            {
                lvBuildings.Items.Add(ActiveUnit.Name, ImageIndex);
                lvBuildings.Items[ImageIndex++].Text = ActiveUnit.Name;
                imageList.Images.Add("itemImageKey", Texture2Image(ActiveUnit.SpriteMap.ActiveSprite));
            }
        }

        private void lvBuildings_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void NewSpawn(int X, int Y, MapLayer TopLayer, BuildingSpawn Spawn)
        {
            //Loop in the SpawnPoint list to find if a SpawnPoint already exist at the X, Y position.
            for (int S = 0; S < TopLayer.ListBuildingSpawn.Count; S++)
            {//If it exist.
                if (TopLayer.ListBuildingSpawn[S].SpawnPositionX == X && TopLayer.ListBuildingSpawn[S].SpawnPositionY == Y)
                {
                    //Delete it.
                    TopLayer.ListBuildingSpawn.RemoveAt(S);
                }
            }
            if (Spawn != null)
            {
                //Add the new SpawnPoint.
                TopLayer.ListBuildingSpawn.Add(Spawn);
            }
        }

        public Image Texture2Image(Texture2D sprTexture2D)
        {
            Image ReturnImage;
            using (MemoryStream MS = new MemoryStream())
            {
                sprTexture2D.SaveAsPng(MS, sprTexture2D.Width, sprTexture2D.Height);
                MS.Seek(0, SeekOrigin.Begin);
                ReturnImage = Image.FromStream(MS);
            }

            if (ReturnImage.Width > ActiveMap.TileSize.X)
            {
                Bitmap bmpImage = new Bitmap(ReturnImage);
                ReturnImage = bmpImage.Clone(new Rectangle(0, 0, ActiveMap.TileSize.X, ReturnImage.Height), bmpImage.PixelFormat);
            }
            return ReturnImage;
        }
    }
}
