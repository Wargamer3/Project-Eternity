using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.RacingScreen;

namespace ProjectEternity.Editors.RacingMapEditor
{
    public partial class ProjectEternityRacingMapEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Tile, TileAsBackground, BGM, UnitPosition, Cutscene };

        private ItemSelectionChoices ItemSelectionChoice;
        
        private RadioButton cbMoveObject;
        private RadioButton cbRotateObject;

        public ProjectEternityRacingMapEditor()
        {
            InitializeComponent();

            #region cbMoveObject

            cbMoveObject = new RadioButton();
            cbMoveObject.Text = "Move";
            cbMoveObject.AutoSize = false;
            //Make it 10 pixel after the last mnuToolBar item.
            cbMoveObject.Padding = new Padding(10, 0, 0, 0);
            ToolStripControlHost s = new ToolStripControlHost(cbMoveObject);
            s.AutoSize = false;
            s.Width = 150;
            mnuToolBar.Items.Add(s);

            #endregion

            #region cbRotateObject

            cbRotateObject = new RadioButton();
            cbRotateObject.Text = "Rotate";
            cbRotateObject.AutoSize = false;
            //Make it 10 pixel after the last mnuToolBar item.
            cbRotateObject.Padding = new Padding(10, 0, 0, 0);
            s = new ToolStripControlHost(cbRotateObject);
            s.AutoSize = false;
            s.Width = 150;
            mnuToolBar.Items.Add(s);

            #endregion

            cbMoveObject.Checked = true;
            cbMoveObject.CheckedChanged += new EventHandler(ObjectSelection_CheckedChanged);
            cbRotateObject.CheckedChanged += new EventHandler(ObjectSelection_CheckedChanged);
        }

        private void ObjectSelection_CheckedChanged(object sender, EventArgs e)
        {
            if (cbMoveObject.Checked)
            {
                RacingMapViewer.SelectionMode = Scene3DViewerControl.SelectionModes.Move;
            }
            else if (cbRotateObject.Checked)
            {
                RacingMapViewer.SelectionMode = Scene3DViewerControl.SelectionModes.Rotate;
            }
        }

        public ProjectEternityRacingMapEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                string MapLogicName = FilePath.Substring(0, FilePath.Length - 4).Substring(20);
                RacingMapViewer.ActiveMap = new RacingMap(MapLogicName);

                SaveItem(FilePath, FilePath);
            }

            LoadMap(FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathRacingMaps }, "Maps/Racing/", new string[] { ".pem" }, typeof(ProjectEternityRacingMapEditor))
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            RacingMapViewer.ActiveMap.Save();
        }

        private void LoadMap(string Path)
        {
            string MapLogicName = FilePath.Substring(0, FilePath.Length - 4).Substring(20);

            RacingMapViewer.Preload();
            RacingMapViewer.ActiveMap = new RacingMap(MapLogicName);
            RacingMapViewer.ActiveMap.Content = RacingMapViewer.content;
            RacingMapViewer.ActiveMap.Load();
            RacingMapViewer.ActiveCamera = RacingMapViewer.ActiveMap.Camera = new FreeCamera(RacingMapViewer.GraphicsDevice);
            RacingMapViewer.ActiveMap.Camera.MoveCamera(250, Vector3.Forward);
            RacingMapViewer.BackgroundGrid = new Lines3D(RacingMapViewer.GraphicsDevice, RacingMapViewer.ActiveMap.Camera.Projection);
            RacingMapViewer.MoveHelper = new CrossArrow3D(RacingMapViewer.GraphicsDevice, RacingMapViewer.ActiveMap.Camera.Projection);
            RacingMapViewer.RotationHelper = new CrossRing3D(RacingMapViewer.GraphicsDevice, RacingMapViewer.ActiveMap.Camera.Projection);

            for (int T = 0; T < RacingMapViewer.ActiveMap.GetAITunnelCount(); ++T)
            {
                ListViewItem NewListViewItem = new ListViewItem("AI Tunnel " + (lvAItunnels.Items.Count + 1));
                lvAItunnels.Items.Add(NewListViewItem);
                NewListViewItem.Tag = RacingMapViewer.ActiveMap.GetAITunnel(T);
            }

            for (int C = 0; C < RacingMapViewer.ActiveMap.GetCollisionBoxCount(); ++C)
            {
                ListViewItem NewListViewItem = new ListViewItem("Collision Box " + (lvCollisionsBoxes.Items.Count + 1));
                lvCollisionsBoxes.Items.Add(NewListViewItem);
                NewListViewItem.Tag = RacingMapViewer.ActiveMap.GetCollisionBox(C);
            }
        }
        
        private Object3D GetObjectUnderMouse(MouseEventArgs e, int TabIndex)
        {
            switch (TabIndex)
            {
                case 0:
                    return RacingMapViewer.ActiveMap.GetAITunnelUnderMouse(e.X, e.Y, RacingMapViewer.GraphicsDevice.Viewport,
                                                                                                RacingMapViewer.ActiveMap.Camera.View,
                                                                                                RacingMapViewer.ActiveMap.Camera.Projection);
                case 1:
                    return RacingMapViewer.ActiveMap.GetCollisionBoxUnderMouse(e.X, e.Y, RacingMapViewer.GraphicsDevice.Viewport,
                                                                                                RacingMapViewer.ActiveMap.Camera.View,
                                                                                                RacingMapViewer.ActiveMap.Camera.Projection);
            }

            return null;
        }

        private void OnObjectSelected(Object3D SelectedObject)
        {
            switch (tabToolBox.SelectedIndex)
            {
                case 0:
                    pgAITunnel.SelectedObject = SelectedObject;
                    break;
                case 1:
                    pgCollisionBox.SelectedObject = SelectedObject;
                    break;
            }
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
                        break;

                    case ItemSelectionChoices.TileAsBackground:
                        break;
                }
            }
        }
        
        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void ProjectEternityRacingEditor_Shown(object sender, EventArgs e)
        {
            RacingMapViewer.GetObjectUnderMouse = delegate (MouseEventArgs E) { return GetObjectUnderMouse(E, tabToolBox.SelectedIndex); };
            RacingMapViewer.OnObjectSelected = OnObjectSelected;
        }

        #region AI Tunnels

        private void btnAddAITunnel_Click(object sender, EventArgs e)
        {
            ListViewItem NewListViewItem = new ListViewItem("AI Tunnel " + (lvAItunnels.Items.Count + 1));
            lvAItunnels.Items.Add(NewListViewItem);
            NewListViewItem.Tag = RacingMapViewer.ActiveMap.CreateAITunnel();
        }

        private void btnRemoveAITunnel_Click(object sender, EventArgs e)
        {
            if (lvAItunnels.SelectedIndices.Count > 0)
            {
                RacingMapViewer.ActiveMap.RemoveAITunnel(lvAItunnels.SelectedIndices[0]);
                lvAItunnels.Items.RemoveAt(lvAItunnels.SelectedIndices[0]);
            }
        }

        private void lvAItunnels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvAItunnels.SelectedIndices.Count > 0)
            {
                pgAITunnel.SelectedObject = lvAItunnels.SelectedItems[0].Tag;
                pgCollisionBox.SelectedObject = lvAItunnels.SelectedItems[0].Tag;
                Object3D SelectedCollisionBox = (Object3D)lvCollisionsBoxes.SelectedItems[0].Tag;
                RacingMapViewer.SelectObject(SelectedCollisionBox);
            }
        }

        #endregion

        #region Collision Boxes

        private void btnAddCollisionBox_Click(object sender, EventArgs e)
        {
            ListViewItem NewListViewItem = new ListViewItem("Collision Box " + (lvCollisionsBoxes.Items.Count + 1));
            lvCollisionsBoxes.Items.Add(NewListViewItem);
            NewListViewItem.Tag = RacingMapViewer.ActiveMap.CreateCollisionBox();
        }

        private void btnRemoveCollisionBox_Click(object sender, EventArgs e)
        {
            if (lvCollisionsBoxes.SelectedIndices.Count > 0)
            {
                RacingMapViewer.ActiveMap.RemoveAITunnel(lvCollisionsBoxes.SelectedIndices[0]);
                lvCollisionsBoxes.Items.RemoveAt(lvCollisionsBoxes.SelectedIndices[0]);
            }
        }

        private void lvCollisionsBoxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvCollisionsBoxes.SelectedIndices.Count > 0)
            {
                pgCollisionBox.SelectedObject = lvCollisionsBoxes.SelectedItems[0].Tag;
                Object3D SelectedCollisionBox = (Object3D)lvCollisionsBoxes.SelectedItems[0].Tag;
                RacingMapViewer.SelectObject(SelectedCollisionBox);
            }
        }

        #endregion
    }
}
