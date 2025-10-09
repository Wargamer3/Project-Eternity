using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.Editors.BitmapAnimationEditor;
using ProjectEternity.GameScreens.TripleThunderScreen;

namespace ProjectEternity.Editors.TripleThunderEditor
{
    public partial class TripleThunderMapEditor : BaseEditor
    {
        private enum SelectionChoices { None, Camera, Move }

        private enum ItemSelectionChoices { Background, Images };

        private ItemSelectionChoices ItemSelectionChoice;

        private SelectionChoices SelectionChoice;

        private Layer ActiveLayer { get { return LayerViewer.SelectedLayer; } set { LayerViewer.SelectedLayer = value; } }

        private MouseEventArgs MouseEventOld;
        private CheckBox cbDisplayUnselectedLayers;

        public TripleThunderMapEditor()
        {
            InitializeComponent();

            #region cbDrawScripts

            //Init the DisplayUnselectedLayers button (as it can't be done with the tool box)
            cbDisplayUnselectedLayers = new CheckBox();
            cbDisplayUnselectedLayers.Text = "Display Unselected Layers";
            cbDisplayUnselectedLayers.AutoSize = false;
            //Link a CheckedChanged event to a method.
            cbDisplayUnselectedLayers.CheckedChanged += new EventHandler(tsmDisplayUnselectedLayers_Click);
            cbDisplayUnselectedLayers.Checked = true;
            //Make it 10 pixel after the last mnuToolBar item.
            cbDisplayUnselectedLayers.Padding = new Padding(10, 0, 0, 0);
            ToolStripControlHost s = new ToolStripControlHost(cbDisplayUnselectedLayers);
            s.AutoSize = false;
            s.Width = 180;
            mnuToolBar.Items.Add(s);

            #endregion
        }

        public TripleThunderMapEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                SaveItem(FilePath, "New Item");
            }

            LoadMap(this.FilePath);

            foreach(Prop ActiveProp in Prop.GetAllProps())
            {
                lstProps.Items.Add(ActiveProp);
            }
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathTripleThunderMaps }, "Triple Thunder/Maps/", new string[] {".ttm" }, typeof(TripleThunderMapEditor)),
                new EditorInfo(new string[] { GUIRootPathTripleThunderRessources }, "Triple Thunder/Ressources/",new string[] { ".xnb" }, typeof(ProjectEternityBitmapAnimationEditor))
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            if (LayerViewer.ActiveFightingZone == null)
            {
                BW.Write(string.Empty); // Background Name
                BW.Write(0); // Left camera bound
                BW.Write(0); // Up camera bound
                BW.Write(1000); // Camera width
                BW.Write(700); // Camera height
                BW.Write(""); // BGM
                BW.Write(""); // Description
                BW.Write(0); // Layer count
                BW.Write(0); // Script count
            }
            else
            {
                LayerViewer.ActiveFightingZone.Save(BW);
            }

            FS.Close();
            BW.Close();
        }

        private void LoadMap(string MapPath)
        {
            string Name = MapPath.Substring(0, MapPath.Length - 4).Substring(28);
            this.Text = Name + " - Project Eternity Triple Thunder Map Editor";
            LayerViewer.Preload();

            FightingZone LoadedFightingZone = new FightingZone(Name, false);
            LoadedFightingZone.UsePreview = true;
            LayerViewer.ActiveFightingZone = LoadedFightingZone;
            LayerViewer.ActiveFightingZone.Content = LayerViewer.content;
            LayerViewer.ActiveFightingZone.Load();


            LayerViewer.SetListMapScript(LoadedFightingZone.ListMapScript);
            LayerViewer.Helper.OnSelect = (SelectedObject, RightClick) => {
                if (RightClick && SelectedObject != null)
                {
                    LayerViewer.cmsScriptMenu.Show(LayerViewer, PointToClient(Cursor.Position));
                }
                else
                {
                    pgScriptProperties.SelectedObject = SelectedObject;
                }
            };


            for (int S = LoadedFightingZone.ListMapScript.Count - 1; S >= 0; --S)
            {
                LayerViewer.Helper.InitScript(LoadedFightingZone.ListMapScript[S]);
            }

            for (int L = 0; L < LayerViewer.ActiveFightingZone.ListLayer.Count; L++)
            {
                lstLayers.Items.Add("Layer " + lstLayers.Items.Count);
            }

            if (LayerViewer.ActiveFightingZone.ListLayer.Count > 0)
            {
                lstLayers.SelectedIndex = 0;
            }
        }

        #region Viewer

        private void LayerViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (ActiveLayer == null)
                return;

            if (tabControl1.SelectedIndex == 5)//Scripts
            {
                LayerViewer.Helper.Select(e.Location);
            }
            else
            {
                if (Control.ModifierKeys == Keys.Alt)
                {
                    SelectionChoice = SelectionChoices.Camera;
                }

                if (tabControl1.SelectedIndex == 1 && e.Button == MouseButtons.Left && LayerViewer.SelectedPolygonTriangle != null)
                {
                    pgCollisionBox.SelectedObject = LayerViewer.SelectedPolygonTriangle.ActivePolygon;
                }

                MouseEventOld = e;
            }
        }

        private void LayerViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (ActiveLayer == null)
                return;

            Point Offset = LayerViewer.Camera.Location;

            int RealX = e.X + Offset.X;
            int RealY = e.Y + Offset.Y;

            if (SelectionChoice == SelectionChoices.Camera)
            {
                MouseMoveCamera(e);
            }
            else
            {
                switch (tabControl1.SelectedIndex)
                {
                    case 0://Layers
                        MouseGroundLevel(e, RealX, RealY);
                        break;

                    case 1://Collisions
                        MouseMoveCollisions(e, RealX, RealY);
                        break;

                    case 2://Backgrounds
                        MouseMoveImage(e, RealX, RealY);
                        break;

                    case 3://Props
                        MouseMoveProp(e, RealX, RealY);
                        break;

                    case 4://Spawns
                        MouseMoveSpawn(e, RealX, RealY);
                        break;

                    case 5://Scripts
                        MouseMoveScript(e);
                        break;
                }
            }

            MouseEventOld = e;
        }

        private void LayerViewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (ActiveLayer == null)
                return;

            SelectionChoice = SelectionChoices.None;
            switch (tabControl1.SelectedIndex)
            {
                case 0://Layers
                    break;
                case 1://Collisions
                    if (e.Button == MouseButtons.Right && LayerViewer.SelectedPolygonTriangle != null)
                    {
                        for (int P = ActiveLayer.ListWorldCollisionPolygon.Count - 1; P >= 0; --P)
                        {
                            if (ActiveLayer.ListWorldCollisionPolygon[P].Collision.ListCollisionPolygon[0] == LayerViewer.SelectedPolygonTriangle.ActivePolygon)
                            ActiveLayer.ListWorldCollisionPolygon.RemoveAt(P);
                        }
                        lstCollisionBox.Items.Clear();

                        for (int C = 0; C < ActiveLayer.ListWorldCollisionPolygon.Count; C++)
                        {
                            lstCollisionBox.Items.Add("Collision box " + (lstCollisionBox.Items.Count + 1));
                        }
                    }
                    break;

                case 2://Backgrounds
                    if (LayerViewer.SelectedAnimation != null)
                    {
                        if (e.Button == MouseButtons.Right)
                        {
                            ActiveLayer.ListImages.Remove(LayerViewer.SelectedAnimation);
                        }
                        pgBackground.SelectedObject = LayerViewer.SelectedAnimation;
                        LayerViewer.SelectedAnimation = null;
                    }
                    break;

                case 3://Props
                    if (e.Button == MouseButtons.Left && LayerViewer.SelectedProp != null)
                    {
                        pgProps.SelectedObject = LayerViewer.SelectedProp;
                    }
                    else if (e.Button == MouseButtons.Right && LayerViewer.SelectedProp != null)
                    {
                        ActiveLayer.ListProp.Remove(LayerViewer.SelectedProp);
                    }
                    LayerViewer.SelectedProp = null;
                    break;

                case 4://Spawn
                    if (e.Button == MouseButtons.Left && LayerViewer.SelectedSpawn != null)
                    {
                        pgSpawn.SelectedObject = LayerViewer.SelectedSpawn;
                    }
                    else if (e.Button == MouseButtons.Right && LayerViewer.SelectedSpawn != null)
                    {
                        ActiveLayer.ListSpawnPointTeam.Remove(LayerViewer.SelectedSpawn);
                    }
                    LayerViewer.SelectedSpawn = null;
                    break;

                case 5://Scripts
                    LayerViewer.Helper.Scripting_MouseUp(e.Location, (e.Button & MouseButtons.Left) == MouseButtons.Left, (e.Button & MouseButtons.Right) == MouseButtons.Right);
                    break;
            }

            MouseEventOld = e;
        }

        private void LayerViewer_DragEnter(object sender, DragEventArgs e)
        {
            //The data from the drag source is moved to the target.
            e.Effect = DragDropEffects.Move;
        }

        private void LayerViewer_DragDrop(object sender, DragEventArgs e)
        {
            Point Offset = LayerViewer.Camera.Location;

            System.Drawing.Point MousePos = PointToClient(new System.Drawing.Point(e.X, e.Y));

            int RealX = MousePos.X + Offset.X;
            int RealY = MousePos.Y + Offset.Y;

            switch (tabControl1.SelectedIndex)
            {
                case 2:
                    SimpleAnimation NewAnimation = CreateSimpleAnimation(
                        "Triple Thunder/Ressources/" + lstBackgrounds.SelectedItem,
                        new System.Drawing.Point(RealX, RealY));

                    ActiveLayer.ListImages.Add(NewAnimation);
                    break;

                case 3:
                    Prop NewProp = ((Prop)lstProps.SelectedItem).Copy();
                    NewProp._Position = new Vector2(RealX, RealY);
                    ActiveLayer.ListProp.Add(NewProp);
                    break;
            }
        }

        #endregion

        #region Layers

        private void btnAddLayer_Click(object sender, EventArgs e)
        {
            Layer NewLayer = new Layer(null);
            Vector2[] ArrayVertex = new Vector2[2];
            ArrayVertex[0] = new Vector2(10, 100);
            ArrayVertex[1] = new Vector2(50, 100);
            NewLayer.GroundLevelCollision = new Polygon(ArrayVertex, LayerViewer.Width, LayerViewer.Height);
            LayerViewer.ActiveFightingZone.ListLayer.Add(NewLayer);

            lstLayers.Items.Add("Layer" + lstLayers.Items.Count);
        }

        private void btnRemoveLayer_Click(object sender, EventArgs e)
        {
            int Index = lstLayers.SelectedIndex;
            lstLayers.Items.RemoveAt(Index);
            LayerViewer.ActiveFightingZone.ListLayer.RemoveAt(Index);
        }

        private void lstLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstLayers.SelectedIndex < 0)
            {
                return;
            }

            int Index = lstLayers.SelectedIndex;
            ActiveLayer = LayerViewer.ActiveFightingZone.ListLayer[Index];
            LayerViewer.SelectedPolygonTriangle = null;

            lstCollisionBox.Items.Clear();
            for (int C = 0; C < ActiveLayer.ListWorldCollisionPolygon.Count; C++)
            {
                lstCollisionBox.Items.Add("Collision box " + (lstCollisionBox.Items.Count + 1));
            }

            lstSpawns.Items.Clear();
            for (int C = 0; C < ActiveLayer.ListSpawnPointTeam.Count; C++)
            {
                lstSpawns.Items.Add("Spawn " + (lstSpawns.Items.Count + 1));
            }
        }

        private void tsmDisplayUnselectedLayers_Click(object sender, EventArgs e)
        {
            LayerViewer.DisplayOtherLayers = cbDisplayUnselectedLayers.Checked;
        }

        private void btnMoveUpLayer_Click(object sender, EventArgs e)
        {
            int Index = lstLayers.SelectedIndex;
            if (Index > 0)
            {
                Layer SelectedLayer = LayerViewer.ActiveFightingZone.ListLayer[Index];

                LayerViewer.ActiveFightingZone.ListLayer[Index] = LayerViewer.ActiveFightingZone.ListLayer[Index - 1];
                LayerViewer.ActiveFightingZone.ListLayer[Index - 1] = SelectedLayer;
            }

            --lstLayers.SelectedIndex;
        }

        private void btnMoveDownLayer_Click(object sender, EventArgs e)
        {
            int Index = lstLayers.SelectedIndex;
            if (Index + 1 < lstLayers.Items.Count)
            {
                Layer SelectedLayer = LayerViewer.ActiveFightingZone.ListLayer[Index];

                LayerViewer.ActiveFightingZone.ListLayer[Index] = LayerViewer.ActiveFightingZone.ListLayer[Index + 1];
                LayerViewer.ActiveFightingZone.ListLayer[Index + 1] = SelectedLayer;
            }

            ++lstLayers.SelectedIndex;
        }

        private void txtGroundLevelPoints_ValueChanged(object sender, EventArgs e)
        {
            if (ActiveLayer == null)
                return;

            if (txtGroundLevelPoints.Value > ActiveLayer.GroundLevelCollision.ArrayVertex.Length)
            {
                Vector2[] NewArray = new Vector2[(int)txtGroundLevelPoints.Value];
                Array.Copy(ActiveLayer.GroundLevelCollision.ArrayVertex, NewArray, ActiveLayer.GroundLevelCollision.ArrayVertex.Length);
                for (int V = ActiveLayer.GroundLevelCollision.ArrayVertex.Length; V < NewArray.Length; V++)
                {
                    NewArray[V] = new Vector2(NewArray[V - 1].X + 10, NewArray[V - 1].Y);
                }
                ActiveLayer.GroundLevelCollision.ArrayVertex = NewArray;
            }
            else if (txtGroundLevelPoints.Value < ActiveLayer.GroundLevelCollision.ArrayVertex.Length)
            {
                Vector2[] NewArray = new Vector2[(int)txtGroundLevelPoints.Value];
                Array.Copy(ActiveLayer.GroundLevelCollision.ArrayVertex, NewArray, NewArray.Length);
                ActiveLayer.GroundLevelCollision.ArrayVertex = NewArray;
            }
        }

        #endregion

        #region Collision Boxes

        private void btnAddCollisionBox_Click(object sender, EventArgs e)
        {
            Vector2[] ArrayVertex = new Vector2[4];
            ArrayVertex[0] = new Vector2(LayerViewer.Camera.X, LayerViewer.Camera.Y);
            ArrayVertex[1] = new Vector2(LayerViewer.Camera.X, LayerViewer.Camera.Y + 50);
            ArrayVertex[2] = new Vector2(LayerViewer.Camera.X + 50, LayerViewer.Camera.Y + 50);
            ArrayVertex[3] = new Vector2(LayerViewer.Camera.X + 50, LayerViewer.Camera.Y);

            WorldPolygon NewPolygon = new WorldPolygon(ArrayVertex, LayerViewer.Width, LayerViewer.Height);

            ActiveLayer.ListWorldCollisionPolygon.Add(NewPolygon);
            lstCollisionBox.Items.Add("Collision box " + (lstCollisionBox.Items.Count + 1));
        }

        private void btnRemoveCollisionBox_Click(object sender, EventArgs e)
        {
            if (lstCollisionBox.SelectedIndex >= 0)
            {
                ActiveLayer.ListWorldCollisionPolygon.RemoveAt(lstCollisionBox.SelectedIndex);
            }

            lstCollisionBox.Items.Clear();
            for (int C = 0; C < ActiveLayer.ListWorldCollisionPolygon.Count; C++)
            {
                lstCollisionBox.Items.Add("Collision box " + (lstCollisionBox.Items.Count + 1));
            }
        }

        private void lstCollisionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstCollisionBox.SelectedIndex >= 0)
            {
                LayerViewer.SelectedPolygonTriangle = new PolygonTriangle(PolygonTriangle.SelectionTypes.Polygon,
                    ActiveLayer.ListWorldCollisionPolygon[lstCollisionBox.SelectedIndex].Collision.ListCollisionPolygon[0],
                    0, 0);
            }
        }

        #endregion

        #region Images

        private void lstImages_MouseDown(object sender, MouseEventArgs e)
        {
            if (lstBackgrounds.SelectedItem != null)
            {
                BackgroundViewer.ChangeTexture("Triple Thunder/Ressources/" + lstBackgrounds.SelectedItem.ToString());
                DoDragDrop("Triple Thunder/Ressources/" + lstBackgrounds.SelectedItem, DragDropEffects.Move);
            }
        }

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Images;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathTripleThunderRessources));
        }

        private void btnRemoveImage_Click(object sender, EventArgs e)
        {
            if (lstBackgrounds.SelectedIndex >= 0)
            {
                lstBackgrounds.Items.RemoveAt(lstBackgrounds.SelectedIndex);
            }
        }

        private void lstBackgrounds_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstBackgrounds.SelectedIndex >= 0)
            {
                BackgroundViewer.ChangeTexture("Triple Thunder/Ressources/" + lstBackgrounds.SelectedItem.ToString());
            }
        }

        #endregion

        #region Spawns

        private void btnAddTeamSpawn_Click(object sender, EventArgs e)
        {
            ActiveLayer.ListSpawnPointTeam.Add(new PlayerSpawnPoint(LayerViewer.Camera.X, LayerViewer.Camera.Y, 0));
            lstSpawns.Items.Add("Spawn " + (lstSpawns.Items.Count + 1));
        }

        private void btnAddNoTeamSpawn_Click(object sender, EventArgs e)
        {
            ActiveLayer.ListSpawnPointNoTeam.Add(new PlayerSpawnPoint(LayerViewer.Camera.X, LayerViewer.Camera.Y, 0));
            lstSpawns.Items.Add("Spawn " + (lstSpawns.Items.Count + 1));
        }

        private void btnAddVehicleSpawn_Click(object sender, EventArgs e)
        {
            ActiveLayer.ListSpawnPointTeam.Add(new VehicleSpawnPoint(LayerViewer.Camera.X, LayerViewer.Camera.Y, 0));
            lstSpawns.Items.Add("Spawn " + (lstSpawns.Items.Count + 1));
        }

        private void btnRemoveSpawn_Click(object sender, EventArgs e)
        {
            if (lstSpawns.SelectedIndex >= 0)
            {
                ActiveLayer.ListSpawnPointTeam.RemoveAt(lstSpawns.SelectedIndex);

                lstSpawns.Items.Clear();
                for (int C = 0; C < ActiveLayer.ListSpawnPointTeam.Count; C++)
                {
                    lstSpawns.Items.Add("Spawn " + (lstSpawns.Items.Count + 1));
                }
            }
        }

        private void lstSpawns_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSpawns.SelectedIndex >= 0)
            {
                pgSpawn.SelectedObject = ActiveLayer.ListSpawnPointTeam[lstSpawns.SelectedIndex];
            }
        }

        #endregion

        #region Scripts

        private void lstScriptChoices_DoubleClick(object sender, EventArgs e)
        {
            if (!(sender is ListBox))
                return;
            if (((ListBox)sender).SelectedIndex == -1)
                return;

            LayerViewer.CreateScript((MapScript)((ListBox)sender).SelectedItem);
        }

        #endregion

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LayerViewer.ShowScripts = tabControl1.SelectedIndex == 5;
        }

        private void lstProps_MouseDown(object sender, MouseEventArgs e)
        {
            if (lstProps.SelectedItem != null)
            {
                DoDragDrop(lstProps.SelectedItem, DragDropEffects.Move);
            }
        }

        private void MouseMoveCamera(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                LayerViewer.Camera.X -= e.X - MouseEventOld.X;
                LayerViewer.Camera.Y -= e.Y - MouseEventOld.Y;
            }
        }

        private void MouseGroundLevel(MouseEventArgs e, int RealX, int RealY)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (SelectionChoice == SelectionChoices.Move)
                {
                    LayerViewer.SelectedPolygonTriangle.Move(e.X - MouseEventOld.X, e.Y - MouseEventOld.Y);
                }
            }
            else
            {
                LayerViewer.SelectedPolygonTriangle = null;
                SelectionChoice = SelectionChoices.None;

                if (LayerViewer.SelectedPolygonTriangle == null)
                {
                    for (int V = 0; V < ActiveLayer.GroundLevelCollision.ArrayVertex.Length; V++)
                    {
                        if (RealX >= ActiveLayer.GroundLevelCollision.ArrayVertex[V].X - 2 && RealX <= ActiveLayer.GroundLevelCollision.ArrayVertex[V].X + 2 &&
                            RealY >= ActiveLayer.GroundLevelCollision.ArrayVertex[V].Y - 2 && RealY <= ActiveLayer.GroundLevelCollision.ArrayVertex[V].Y + 2)
                        {
                            LayerViewer.SelectedPolygonTriangle = PolygonTriangle.VertexSelection(ActiveLayer.GroundLevelCollision, V);
                            SelectionChoice = SelectionChoices.Move;
                            break;
                        }
                    }
                }
                if (Control.ModifierKeys == Keys.Control && LayerViewer.SelectedPolygonTriangle != null)
                {
                    LayerViewer.SelectedPolygonTriangle.SelectionType = PolygonTriangle.SelectionTypes.Polygon;
                }
            }
        }

        private void MouseMoveCollisions(MouseEventArgs e, int RealX, int RealY)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (SelectionChoice == SelectionChoices.Move)
                {
                    LayerViewer.SelectedPolygonTriangle.Move(e.X - MouseEventOld.X, e.Y - MouseEventOld.Y);
                }
            }
            else
            {
                LayerViewer.SelectedPolygonTriangle = null;
                SelectionChoice = SelectionChoices.None;

                foreach (WorldPolygon ActivePolygon in ActiveLayer.ListWorldCollisionPolygon)
                {
                    PolygonTriangle Result = ActivePolygon.Collision.ListCollisionPolygon[0].PolygonCollisionWithMouse(RealX, RealY);

                    if (Result.ActivePolygon != null)
                    {
                        LayerViewer.SelectedPolygonTriangle = Result;
                        SelectionChoice = SelectionChoices.Move;
                        break;
                    }
                }

                if (Control.ModifierKeys == Keys.Control && LayerViewer.SelectedPolygonTriangle != null)
                {
                    LayerViewer.SelectedPolygonTriangle.SelectionType = PolygonTriangle.SelectionTypes.Polygon;
                }
            }
        }

        private void MouseMoveImage(MouseEventArgs e, int RealX, int RealY)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (SelectionChoice == SelectionChoices.Move)
                {
                    LayerViewer.SelectedAnimation.Position.X += e.X - MouseEventOld.X;
                    LayerViewer.SelectedAnimation.Position.Y += e.Y - MouseEventOld.Y;
                }
            }
            else
            {
                LayerViewer.SelectedAnimation = null;
                SelectionChoice = SelectionChoices.None;

                foreach (SimpleAnimation ActiveBackground in ActiveLayer.ListImages)
                {
                    bool CollideWithMouse = ActiveBackground.PositionRectangle.Intersects(new Rectangle(RealX, RealY, 1, 1));

                    if (CollideWithMouse)
                    {
                        LayerViewer.SelectedAnimation = ActiveBackground;
                        SelectionChoice = SelectionChoices.Move;
                        break;
                    }
                }
            }
        }

        private void MouseMoveProp(MouseEventArgs e, int RealX, int RealY)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (SelectionChoice == SelectionChoices.Move)
                {
                    LayerViewer.SelectedProp._Position.X += e.X - MouseEventOld.X;
                    LayerViewer.SelectedProp._Position.Y += e.Y - MouseEventOld.Y;
                }
            }
            else
            {
                LayerViewer.SelectedProp = null;
                SelectionChoice = SelectionChoices.None;

                foreach (Prop ActiveProp in ActiveLayer.ListProp)
                {
                    bool CollideWithMouse = new Rectangle((int)ActiveProp._Position.X, (int)ActiveProp._Position.Y, 32, 32).Intersects(new Rectangle(RealX, RealY, 1, 1));

                    if (CollideWithMouse)
                    {
                        LayerViewer.SelectedProp = ActiveProp;
                        SelectionChoice = SelectionChoices.Move;
                        break;
                    }
                }
            }
        }

        private void MouseMoveSpawn(MouseEventArgs e, int RealX, int RealY)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (SelectionChoice == SelectionChoices.Move)
                {
                    LayerViewer.SelectedSpawn.SpawnLocation += new Vector2(e.X - MouseEventOld.X, e.Y - MouseEventOld.Y);
                }
            }
            else
            {
                LayerViewer.SelectedSpawn = null;
                SelectionChoice = SelectionChoices.None;

                foreach (SpawnPoint ActiveSpawn in ActiveLayer.ListSpawnPointTeam)
                {
                    bool CollideWithMouse = new Rectangle((int)ActiveSpawn.SpawnLocation.X, (int)ActiveSpawn.SpawnLocation.Y, 32, 32).Intersects(new Rectangle(RealX, RealY, 1, 1));

                    if (CollideWithMouse)
                    {
                        LayerViewer.SelectedSpawn = ActiveSpawn;
                        SelectionChoice = SelectionChoices.Move;
                        break;
                    }
                }
            }
        }

        public void MouseMoveScript(MouseEventArgs e)
        {
            int MaxX, MaxY;
            LayerViewer.Helper.MoveScript(e.Location, out MaxX, out MaxY);
        }

        private SimpleAnimation CreateSimpleAnimation(string Path, System.Drawing.Point MousePos)
        {
            SimpleAnimation NewAnimation = new SimpleAnimation(Path, LayerViewer.content);
            NewAnimation.Position = new Vector2(MousePos.X, MousePos.Y);

            return NewAnimation;
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void tsmBackground_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Background;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathAnimationsBackgroundsAll));
        }

        private void tsmProperties_Click(object sender, EventArgs e)
        {
            MapProperties frmMapProperties = new MapProperties();
            frmMapProperties.txtLeft.Value = LayerViewer.ActiveFightingZone.CameraBounds.X;
            frmMapProperties.txtTop.Value = LayerViewer.ActiveFightingZone.CameraBounds.Y;
            frmMapProperties.txtRight.Value = LayerViewer.ActiveFightingZone.CameraBounds.Right;
            frmMapProperties.txtBottom.Value = LayerViewer.ActiveFightingZone.CameraBounds.Bottom;
            frmMapProperties.txtMusic.Text = LayerViewer.ActiveFightingZone.BGMPath;
            frmMapProperties.txtDescription.Text = LayerViewer.ActiveFightingZone.Description;

            if (frmMapProperties.ShowDialog() == DialogResult.OK)
            {
                LayerViewer.ActiveFightingZone.CameraBounds.X = (int)frmMapProperties.txtLeft.Value;
                LayerViewer.ActiveFightingZone.CameraBounds.Y = (int)frmMapProperties.txtTop.Value;
                LayerViewer.ActiveFightingZone.CameraBounds.Width = (int)frmMapProperties.txtRight.Value - LayerViewer.ActiveFightingZone.CameraBounds.X;
                LayerViewer.ActiveFightingZone.CameraBounds.Height = (int)frmMapProperties.txtBottom.Value - LayerViewer.ActiveFightingZone.CameraBounds.Y;
                LayerViewer.ActiveFightingZone.BGMPath = frmMapProperties.txtMusic.Text;
                LayerViewer.ActiveFightingZone.Description = frmMapProperties.txtDescription.Text;
            }
        }

        private void TripleThunderMapEditor_Shown(object sender, EventArgs e)
        {
            #region Scripting

            lstEvents.Items.Add(new FightingZoneEvent(FightingZoneEvent.EventTypeGame, new string[] { "Game Start" }));
            lstEvents.Items.Add(new FightingZoneEvent(FightingZoneEvent.EventTypeAllEnemiesDefeated, new string[] { FightingZoneEvent.EventTypeAllEnemiesDefeated }));
            lstEvents.Items.Add(new FightingZoneEvent(FightingZoneEvent.EventOnStep, new string[] { FightingZoneEvent.EventOnStep }));

            string[] Files = Directory.GetFiles("Scripts", "*.dll");
            for (int F = 0; F < Files.Length; F++)
            {
                System.Reflection.Assembly ActiveAssembly = System.Reflection.Assembly.LoadFile(Path.GetFullPath(Files[F]));
                List<MapCondition> ListMapCondition = ReflectionHelper.GetObjectsFromBaseTypes<MapCondition>(typeof(FightingZoneCondition), ActiveAssembly.GetTypes());

                foreach (MapCondition Instance in ListMapCondition)
                {
                    lstConditions.Items.Add(Instance);
                }
            }

            Files = Directory.GetFiles("Scripts", "*.dll");
            for (int F = 0; F < Files.Length; F++)
            {
                System.Reflection.Assembly ActiveAssembly = System.Reflection.Assembly.LoadFile(Path.GetFullPath(Files[F]));
                List<MapTrigger> ListMapTrigger = ReflectionHelper.GetObjectsFromBaseTypes<MapTrigger>(typeof(FightingZoneTrigger), ActiveAssembly.GetTypes());

                foreach (MapTrigger Instance in ListMapTrigger)
                {
                    lstTriggers.Items.Add(Instance);
                }
            }

            #endregion
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Background:
                        string BackgroundName = Items[I].Substring(0, Items[0].Length - 5).Substring(19);
                        LayerViewer.ActiveFightingZone.LoadBackground(BackgroundName);
                        break;

                    case ItemSelectionChoices.Images:
                        string ImageName = Items[I].Substring(0, Items[I].Length - 4).Substring(34);
                        lstBackgrounds.Items.Add(ImageName);
                        break;
                }
            }
        }
    }
}
