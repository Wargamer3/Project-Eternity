using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.AdventureScreen;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.Editors.BitmapAnimationEditor;

namespace ProjectEternity.Editors.AdventureEditor
{
    public partial class AdventureMapEditor : BaseEditor
    {
        private enum SelectionChoices { None, Camera, Move }

        private enum ItemSelectionChoices { Images };

        private ItemSelectionChoices ItemSelectionChoice;

        private SelectionChoices SelectionChoice;

        private MouseEventArgs MouseEventOld;

        public AdventureMapEditor()
        {
            InitializeComponent();
        }

        public AdventureMapEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { GUIRootPathAdventureMaps }, "Maps/Adventure/", new string[] {".am" }, typeof(AdventureMapEditor)),
                new EditorInfo(new string[] { GUIRootPathAdventureRessources }, "Adventure/Ressources/",new string[] { ".xnb" }, typeof(ProjectEternityBitmapAnimationEditor))
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
                BW.Write(0); // Left camera bound
                BW.Write(0); // Up camera bound
                BW.Write(1000); // Camera width
                BW.Write(700); // Camera height
                BW.Write(0); // Layer count
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
            this.Text = Name + " - Project Eternity Adventure Map Editor";
            LayerViewer.Preload();
            AdventureMap LoadedFightingZone = new AdventureMap(Name);
            LayerViewer.ActiveFightingZone = LoadedFightingZone;
            LayerViewer.ActiveFightingZone.Content = LayerViewer.content;
            LayerViewer.ActiveFightingZone.Load();
        }

        #region Viewer

        private void LayerViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Alt)
            {
                SelectionChoice = SelectionChoices.Camera;
            }

            MouseEventOld = e;
        }

        private void LayerViewer_MouseMove(object sender, MouseEventArgs e)
        {
            Point Offset = LayerViewer.ActiveFightingZone.Camera.Location;

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
                        MouseMoveCollisions(e, RealX, RealY);
                        break;

                    case 1://Images
                        MouseMoveImage(e, RealX, RealY);
                        break;

                    case 2://Props
                        MouseMoveProp(e, RealX, RealY);
                        break;
                }
            }

            MouseEventOld = e;
        }

        private void LayerViewer_MouseUp(object sender, MouseEventArgs e)
        {
            Point Offset = LayerViewer.ActiveFightingZone.Camera.Location;

            int RealX = e.X + Offset.X;
            int RealY = e.Y + Offset.Y;

            SelectionChoice = SelectionChoices.None;
            switch (tabControl1.SelectedIndex)
            {
                case 0://Collisions
                    if (e.Button == MouseButtons.Right && LayerViewer.SelectedPolygonTriangle != null)
                    {
                        LayerViewer.ActiveFightingZone.ListWorldCollisionPolygon.Remove(LayerViewer.SelectedPolygonTriangle.ActivePolygon);
                        lstCollisionBox.Items.Clear();

                        for (int C = 0; C < LayerViewer.ActiveFightingZone.ListWorldCollisionPolygon.Count; C++)
                            lstCollisionBox.Items.Add("Collision box " + (lstCollisionBox.Items.Count + 1));
                    }
                    break;

                case 1://Images
                    if (LayerViewer.SelectedAnimation != null)
                    {
                        if (e.Button == MouseButtons.Right)
                        {
                            LayerViewer.ActiveFightingZone.ListImages.Remove(LayerViewer.SelectedAnimation);
                        }
                        pgImage.SelectedObject = LayerViewer.SelectedAnimation;
                        LayerViewer.SelectedAnimation = null;
                    }
                    break;

                case 2://Props
                    if (e.Button == MouseButtons.Left && LayerViewer.SelectedProp != null)
                    {
                        pgProps.SelectedObject = LayerViewer.SelectedProp;
                    }
                    else if (e.Button == MouseButtons.Right && LayerViewer.SelectedProp != null)
                    {
                        LayerViewer.ActiveFightingZone.ListProp.Remove(LayerViewer.SelectedProp);
                    }
                    LayerViewer.SelectedProp = null;
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
            Point Offset = LayerViewer.ActiveFightingZone.Camera.Location;

            System.Drawing.Point MousePos = PointToClient(new System.Drawing.Point(e.X, e.Y));

            int RealX = MousePos.X + Offset.X;
            int RealY = MousePos.Y + Offset.Y;

            switch (tabControl1.SelectedIndex)
            {
                case 1:
                    SimpleAnimation NewAnimation = CreateSimpleAnimation(
                        "Adventure/Ressources/" + lstImages.SelectedItem,
                        new System.Drawing.Point(RealX, RealY));

                    LayerViewer.ActiveFightingZone.ListImages.Add(NewAnimation);
                    break;

                case 2:
                    Prop NewProp = ((Prop)lstProps.SelectedItem).Copy();
                    NewProp.Position = new Vector2(RealX, RealY);
                    LayerViewer.ActiveFightingZone.ListProp.Add(NewProp);
                    break;
            }
        }

        #endregion
        
        #region Collision Boxes

        private void btnAddCollisionBox_Click(object sender, EventArgs e)
        {
            Vector2[] ArrayVertex = new Vector2[4];
            ArrayVertex[0] = new Vector2(LayerViewer.ActiveFightingZone.Camera.X, LayerViewer.ActiveFightingZone.Camera.Y);
            ArrayVertex[1] = new Vector2(LayerViewer.ActiveFightingZone.Camera.X, LayerViewer.ActiveFightingZone.Camera.Y + 50);
            ArrayVertex[2] = new Vector2(LayerViewer.ActiveFightingZone.Camera.X + 50, LayerViewer.ActiveFightingZone.Camera.Y + 50);
            ArrayVertex[3] = new Vector2(LayerViewer.ActiveFightingZone.Camera.X + 50, LayerViewer.ActiveFightingZone.Camera.Y);

            Polygon NewPolygon = new Polygon(ArrayVertex, LayerViewer.Width, LayerViewer.Height);

            LayerViewer.ActiveFightingZone.ListWorldCollisionPolygon.Add(NewPolygon);
            lstCollisionBox.Items.Add("Collision box " + (lstCollisionBox.Items.Count + 1));
        }

        private void btnRemoveCollisionBox_Click(object sender, EventArgs e)
        {
            if (lstCollisionBox.SelectedIndex >= 0)
            {
                LayerViewer.ActiveFightingZone.ListWorldCollisionPolygon.RemoveAt(lstCollisionBox.SelectedIndex);
            }

            lstCollisionBox.Items.Clear();
            for (int C = 0; C < LayerViewer.ActiveFightingZone.ListWorldCollisionPolygon.Count; C++)
                lstCollisionBox.Items.Add("Collision box " + (lstCollisionBox.Items.Count + 1));
        }

        private void lstCollisionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstCollisionBox.SelectedIndex >= 0)
            {
                LayerViewer.SelectedPolygonTriangle = new PolygonTriangle(PolygonTriangle.SelectionTypes.Polygon, LayerViewer.ActiveFightingZone.ListWorldCollisionPolygon[lstCollisionBox.SelectedIndex], 0, 0);
            }
        }

        #endregion

        #region Images

        private void lstImages_MouseDown(object sender, MouseEventArgs e)
        {
            if (lstImages.SelectedItem != null)
            {
                ImageViewer.ChangeTexture("Adventure/Ressources/" + lstImages.SelectedItem.ToString());
                DoDragDrop("Adventure/Ressources/" + lstImages.SelectedItem, DragDropEffects.Move);
            }
        }

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Images;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathAdventureRessources));
        }

        private void btnRemoveImage_Click(object sender, EventArgs e)
        {
            if (lstImages.SelectedIndex >= 0)
            {
                lstImages.Items.RemoveAt(lstImages.SelectedIndex);
            }
        }

        private void lstImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstImages.SelectedItem != null)
            {
                ImageViewer.ChangeTexture("Adventure/Ressources/" + lstImages.SelectedItem.ToString());
            }
        }

        #endregion

        private void lstProps_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(lstProps.SelectedItem, DragDropEffects.Move);
        }

        private void MouseMoveCamera(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                LayerViewer.ActiveFightingZone.Camera.X -= e.X - MouseEventOld.X;
                LayerViewer.ActiveFightingZone.Camera.Y -= e.Y - MouseEventOld.Y;
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

                foreach (Polygon ActivePolygon in LayerViewer.ActiveFightingZone.ListWorldCollisionPolygon)
                {
                    PolygonTriangle Result = ActivePolygon.PolygonCollisionWithMouse(RealX, RealY);

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

                foreach (SimpleAnimation ActiveImage in LayerViewer.ActiveFightingZone.ListImages)
                {
                    bool CollideWithMouse = ActiveImage.PositionRectangle.Intersects(new Rectangle(RealX, RealY, 1, 1));

                    if (CollideWithMouse)
                    {
                        LayerViewer.SelectedAnimation = ActiveImage;
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
                    LayerViewer.SelectedProp.Position.X += e.X - MouseEventOld.X;
                    LayerViewer.SelectedProp.Position.Y += e.Y - MouseEventOld.Y;
                }
            }
            else
            {
                LayerViewer.SelectedProp = null;
                SelectionChoice = SelectionChoices.None;

                foreach (Prop ActiveProp in LayerViewer.ActiveFightingZone.ListProp)
                {
                    bool CollideWithMouse = new Rectangle((int)ActiveProp.Position.X, (int)ActiveProp.Position.Y, 32, 32).Intersects(new Rectangle(RealX, RealY, 1, 1));

                    if (CollideWithMouse)
                    {
                        LayerViewer.SelectedProp = ActiveProp;
                        SelectionChoice = SelectionChoices.Move;
                        break;
                    }
                }
            }
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

        private void tsmProperties_Click(object sender, EventArgs e)
        {
            MapProperties frmMapProperties = new MapProperties();
            frmMapProperties.txtLeft.Value = LayerViewer.ActiveFightingZone.CameraBounds.X;
            frmMapProperties.txtTop.Value = LayerViewer.ActiveFightingZone.CameraBounds.Y;
            frmMapProperties.txtRight.Value = LayerViewer.ActiveFightingZone.CameraBounds.Right;
            frmMapProperties.txtBottom.Value = LayerViewer.ActiveFightingZone.CameraBounds.Bottom;

            if (frmMapProperties.ShowDialog() == DialogResult.OK)
            {
                LayerViewer.ActiveFightingZone.CameraBounds.X = (int)frmMapProperties.txtLeft.Value;
                LayerViewer.ActiveFightingZone.CameraBounds.Y = (int)frmMapProperties.txtTop.Value;
                LayerViewer.ActiveFightingZone.CameraBounds.Width = (int)frmMapProperties.txtRight.Value - LayerViewer.ActiveFightingZone.CameraBounds.X;
                LayerViewer.ActiveFightingZone.CameraBounds.Height = (int)frmMapProperties.txtBottom.Value - LayerViewer.ActiveFightingZone.CameraBounds.Y;
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
                    case ItemSelectionChoices.Images:
                        string ImageName = Items[I].Substring(0, Items[I].Length - 4).Substring(34);
                        lstImages.Items.Add(ImageName);
                        break;
                }
            }
        }
    }
}
