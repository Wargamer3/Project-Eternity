using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public partial class PolygonCutterHelper : Form
    {
        private enum SelectionChoices { None, Move, SplitPolygon, SplitTriangle, SelectPolygon }

        private MouseEventArgs MouseEventOld;
        private SelectionChoices SelectionChoice;
        private int ReplaceVertexIndex;

        public PolygonCutterHelper()
        {
            InitializeComponent();
            PolygonCutterViewer.sprSource = null;
        }

        public PolygonCutterHelper(Texture2D sprSource, List<Polygon> ListPolygon, bool EditOrigin)
        {
            InitializeComponent();
            PolygonCutterViewer.sprSource = sprSource;
            PolygonCutterViewer.ListPolygon = new List<Polygon>(ListPolygon);
            PolygonCutterViewer.EditOrigin = EditOrigin;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void PolygonCutterViewer_MouseDown(object sender, MouseEventArgs e)
        {
            MouseEventOld = e;
            int RealX = MouseEventOld.X;
            int RealY = MouseEventOld.Y;
            PolygonTriangle SelectedPolygonTriangle = PolygonCutterViewer.SelectedPolygonTriangle;

            switch (SelectionChoice)
            {
                #region None

                case SelectionChoices.None:
                    if (PolygonCutterViewer.SelectedPolygonTriangle.SelectionType != PolygonTriangle.SelectionTypes.None)
                    {
                        Polygon ActivePolygon = PolygonCutterViewer.SelectedPolygonTriangle.ActivePolygon;

                        if (e.Button == MouseButtons.Left)
                        {
                            SelectionChoice = SelectionChoices.Move;

                            if (SelectedPolygonTriangle.SelectionType == PolygonTriangle.SelectionTypes.Edge)
                            {
                                if (Control.ModifierKeys == Keys.Shift)
                                {
                                    SelectedPolygonTriangle.VertexIndex = (short)ActivePolygon.ArrayVertex.Length;

                                    ActivePolygon.AddVertexOnEdge(new Vector2(RealX, RealY), SelectedPolygonTriangle.EdgeIndex1, SelectedPolygonTriangle.EdgeIndex2);
                                    ActivePolygon.UpdateWorldPosition(Vector2.Zero, 0f);
                                }
                                else if (Control.ModifierKeys == Keys.Alt)
                                {
                                    SelectedPolygonTriangle.VertexIndex = (short)ActivePolygon.ArrayVertex.Length;

                                    ActivePolygon.SplitEdge(new Vector2(RealX, RealY), SelectedPolygonTriangle.TriangleIndex, SelectedPolygonTriangle.EdgeIndex1, SelectedPolygonTriangle.EdgeIndex2);
                                    ActivePolygon.UpdateWorldPosition(Vector2.Zero, 0f);
                                }
                            }
                            else if (SelectedPolygonTriangle.SelectionType == PolygonTriangle.SelectionTypes.Triangle)
                            {
                                if (Control.ModifierKeys == Keys.Shift)
                                {
                                    ActivePolygon.SplitTriangle(new Vector2(e.X, e.Y), SelectedPolygonTriangle.TriangleIndex);
                                    ActivePolygon.UpdateWorldPosition(Vector2.Zero, 0f);
                                }
                            }
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            if (SelectedPolygonTriangle.SelectionType == PolygonTriangle.SelectionTypes.Vertex && SelectedPolygonTriangle.VertexIndex >= 0)//Delete Vertex.
                            {
                                SelectedPolygonTriangle.ActivePolygon.RemoveVertex(SelectedPolygonTriangle.VertexIndex);
                            }
                            else if (SelectedPolygonTriangle.SelectionType == PolygonTriangle.SelectionTypes.Triangle && SelectedPolygonTriangle.TriangleIndex >= 0)//Delete Triangle.
                            {
                            }
                        }
                    }
                    break;

                #endregion

                case SelectionChoices.SplitPolygon:
                    PolygonCutterViewer.SplittingPoint1 = new Vector2(e.X, e.Y);
                    break;
            }
        }

        private void PolygonCutterViewer_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left && PolygonCutterViewer.SelectedPolygonTriangle.SelectionType != PolygonTriangle.SelectionTypes.None
                && SelectionChoice == SelectionChoices.Move)
            {
                PolygonTriangle SelectedPolygonTriangle = PolygonCutterViewer.SelectedPolygonTriangle;

                #region Move

                Polygon ActivePolygon = SelectedPolygonTriangle.ActivePolygon;

                if (SelectedPolygonTriangle.SelectionType == PolygonTriangle.SelectionTypes.Vertex)// Move Vertex.
                {
                    ReplaceVertexIndex = -1;

                    if (Control.ModifierKeys == Keys.Alt)//Snap to closest vertex.
                    {
                        double MinDistanceToVertex = 5;
                        for (int V = 0; V < ActivePolygon.ArrayVertex.Length; V++)
                        {
                            if (V == SelectedPolygonTriangle.VertexIndex)
                                continue;

                            double DistanceToVertexX = e.X - ActivePolygon.ArrayVertex[V].X;
                            double DistanceToVertexY = e.Y - ActivePolygon.ArrayVertex[V].Y;
                            double DistanceToVertex = Math.Sqrt(Math.Pow(DistanceToVertexX, 2) + Math.Pow(DistanceToVertexY, 2));

                            if (DistanceToVertex < MinDistanceToVertex)
                            {
                                MinDistanceToVertex = DistanceToVertex;
                                ReplaceVertexIndex = V;
                            }
                        }
                    }
                    if (ReplaceVertexIndex >= 0)
                    {
                        ActivePolygon.ArrayVertex[SelectedPolygonTriangle.VertexIndex].X = ActivePolygon.ArrayVertex[ReplaceVertexIndex].X;
                        ActivePolygon.ArrayVertex[SelectedPolygonTriangle.VertexIndex].Y = ActivePolygon.ArrayVertex[ReplaceVertexIndex].Y;
                        ActivePolygon.UpdateWorldPosition(Vector2.Zero, 0f);
                    }
                    else
                    {
                        ActivePolygon.ArrayVertex[SelectedPolygonTriangle.VertexIndex].X = e.X;
                        ActivePolygon.ArrayVertex[SelectedPolygonTriangle.VertexIndex].Y = e.Y;
                        ActivePolygon.UpdateWorldPosition(Vector2.Zero, 0f);
                    }
                }
                else//Move Polygon
                {
                    SelectedPolygonTriangle.Move(e.X - MouseEventOld.X, e.Y - MouseEventOld.Y);
                }

                #endregion
            }
            else
            {
                PolygonCutterViewer.SelectedPolygonTriangle.SelectionType = PolygonTriangle.SelectionTypes.None;

                for (int P = PolygonCutterViewer.ListPolygon.Count - 1; P >= 0; --P)
                {
                    Polygon ActivePolygon = PolygonCutterViewer.ListPolygon[P];
                    Vector2 MousePosition = new Vector2(e.X, e.Y);

                    PolygonTriangle PolygonResult = ActivePolygon.PolygonCollisionWithMouse(e.X, e.Y);

                    if (PolygonResult.SelectionType != PolygonTriangle.SelectionTypes.None)
                    {
                        PolygonCutterViewer.SelectedPolygonTriangle = PolygonResult;
                    }
                }

                switch (SelectionChoice)
                {
                    case SelectionChoices.SplitPolygon:
                        PolygonCutterViewer.SplittingPoint2 = new Vector2(e.X, e.Y);
                        break;
                }
            }

            if (Control.ModifierKeys == Keys.Control && PolygonCutterViewer.SelectedPolygonTriangle != null)
            {
                PolygonCutterViewer.SelectedPolygonTriangle.SelectionType = PolygonTriangle.SelectionTypes.Polygon;
            }
            MouseEventOld = e;
        }

        private void PolygonCutterViewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Polygon ActivePolygon = null;

                PolygonTriangle SelectedPolygonTriangle = PolygonCutterViewer.SelectedPolygonTriangle;

                if (SelectedPolygonTriangle.SelectionType != PolygonTriangle.SelectionTypes.None)
                    ActivePolygon = SelectedPolygonTriangle.ActivePolygon;

                switch (SelectionChoice)
                {
                    case SelectionChoices.Move:
                        if (Control.ModifierKeys == Keys.Alt && SelectedPolygonTriangle.VertexIndex >= 0 && ReplaceVertexIndex >= 0
                            && SelectedPolygonTriangle.SelectionType == PolygonTriangle.SelectionTypes.Vertex)
                        {
                            ActivePolygon.ReplaceVertex(SelectedPolygonTriangle.VertexIndex, (short)ReplaceVertexIndex);
                            ActivePolygon.UpdateWorldPosition(Vector2.Zero, 0f);
                        }

                        SelectionChoice = SelectionChoices.None;
                        break;

                    case SelectionChoices.SplitPolygon:
                        List<Polygon> ListNewPolygons = new List<Polygon>();
                        for (int P = PolygonCutterViewer.ListPolygon.Count - 1; P >= 0; --P)
                        {
                            Polygon NewPolygon = PolygonCutterViewer.ListPolygon[P].SplitPolygon(PolygonCutterViewer.SplittingPoint1, PolygonCutterViewer.SplittingPoint2);
                            if (NewPolygon != null)
                                ListNewPolygons.Add(NewPolygon);
                        }

                        PolygonCutterViewer.ListPolygon.AddRange(ListNewPolygons);
                        for (int P = PolygonCutterViewer.ListPolygon.Count - 1; P >= 0; --P)
                        {
                            PolygonCutterViewer.ListPolygon[P].FuseOverlappingVertex();
                            PolygonCutterViewer.ListPolygon[P].UpdateWorldPosition(Vector2.Zero, 0f);
                        }
                        PolygonCutterViewer.SplittingPoint2 = PolygonCutterViewer.SplittingPoint1 = Vector2.Zero;
                        break;

                    case SelectionChoices.SplitTriangle:
                        ActivePolygon.SplitTriangle(new Vector2(e.X, e.Y), SelectedPolygonTriangle.TriangleIndex);
                        break;

                    case SelectionChoices.SelectPolygon:
                        if (Control.ModifierKeys != Keys.Shift)
                        {
                            PolygonCutterViewer.ListSelectedPolygon.Clear();
                            SelectionChoice = SelectionChoices.None;
                        }
                        SelectedPolygonTriangle.SelectionType = PolygonTriangle.SelectionTypes.Polygon;
                        PolygonCutterViewer.ListSelectedPolygon.Add(ActivePolygon);
                        break;
                }
            }

            if (PolygonCutterViewer.EditOrigin)
            {
                for (int P = PolygonCutterViewer.ListPolygon.Count - 1; P >= 0; --P)
                {
                    PolygonCutterViewer.ListPolygon[P].ComputeColorAndUVCoordinates(PolygonCutterViewer.sprSource.Width, PolygonCutterViewer.sprSource.Height);
                }
            }
        }

        private void btnMultiTool_CheckedChanged(object sender, EventArgs e)
        {
            if (btnMultiTool.Checked)
            {
                SelectionChoice = SelectionChoices.None;
            }
        }

        private void btnSplitPolygonTool_CheckedChanged(object sender, EventArgs e)
        {
            if (btnSplitPolygonTool.Checked)
            {
                SelectionChoice = SelectionChoices.SplitPolygon;
                PolygonCutterViewer.SplittingPoint2 = PolygonCutterViewer.SplittingPoint1 = Vector2.Zero;
            }
        }

        private void btnSplitTriangle_CheckedChanged(object sender, EventArgs e)
        {
            if (btnSplitTriangle.Checked)
            {
                SelectionChoice = SelectionChoices.SplitTriangle;
            }
        }

        private void btnSelectPolygon_CheckedChanged(object sender, EventArgs e)
        {
            if (btnSelectPolygon.Checked)
            {
                SelectionChoice = SelectionChoices.SelectPolygon;
            }
        }
    }
}
