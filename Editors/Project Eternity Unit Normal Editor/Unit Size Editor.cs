using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ProjectEternity.Editors.UnitNormalEditor
{
    public partial class UnitSizeEditor : Form
    {
        //Buffer used to draw in the panDrawingSurface.
        BufferedGraphicsContext pbSizePreviewContext;
        BufferedGraphics pbSizePreviewGraphicDevice;
        Graphics pbDrawingSurfaceGraphics;
        public List<List<bool>> ListUnitSize;
        public int OldWidth;
        public int OldHeight;

        public UnitSizeEditor()
        {
            InitializeComponent();

            ListUnitSize = new List<List<bool>>(3);
            ListUnitSize.Add(new List<bool>());
            ListUnitSize[0].Add(true);
            OldWidth = 0;
            OldHeight = 0;
        }

        private void DrawUnitSize()
        {
            if (pbSizePreviewContext == null)
                return;

            pbSizePreviewGraphicDevice.Graphics.Clear(Color.White);

            if (rbCustomSizeBox.Checked)
            {
                for (int X = 0; X < txtWidth.Value; ++X)
                    for (int Y = 0; Y < txtLength.Value; ++Y)
                        if (ListUnitSize[X][Y])
                            pbSizePreviewGraphicDevice.Graphics.FillRectangle(Brushes.Red, X * 32, Y * 32, 32, 32);


                for (int X = 32; X < pbUnitSize.Width; X += 32)
                {
                    pbSizePreviewGraphicDevice.Graphics.DrawLine(Pens.Black, X, 0, X, pbUnitSize.Height);
                }

                for (int Y = 32; Y < pbUnitSize.Height; Y += 32)
                {
                    pbSizePreviewGraphicDevice.Graphics.DrawLine(Pens.Black, 0, Y, pbUnitSize.Width, Y);
                }
            }

            pbSizePreviewGraphicDevice.Render();
        }

        private void pbUnitSize_Paint(object sender, PaintEventArgs e)
        {
            DrawUnitSize();
        }

        private void pbUnitSize_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X < 0 || e.Y < 0 ||
                e.X / 32 >= ListUnitSize.Count || e.Y / 32 >= ListUnitSize[0].Count)
                return;

            if (e.Button == MouseButtons.Left)
            {
                int X = e.X / 32;
                int Y = e.Y / 32;
                ListUnitSize[X][Y] = true;
                DrawUnitSize();
            }
            else if (e.Button == MouseButtons.Right)
            {
                int X = e.X / 32;
                int Y = e.Y / 32;
                ListUnitSize[X][Y] = false;
                DrawUnitSize();
            }
        }

        private void txtWidth_ValueChanged(object sender, EventArgs e)
        {
            int NewValue = (int)txtWidth.Value;
            int CurrentHeight = (int)txtLength.Value;

            //List is getting bigger.
            if (OldWidth < NewValue)
            {
                for (; OldWidth < NewValue; ++OldWidth)
                {
                    List<bool> NewListbool = new List<bool>(CurrentHeight);
                    for (int Y = 0; Y < CurrentHeight; ++Y)
                        NewListbool.Add(false);
                    ListUnitSize.Insert(0, NewListbool);
                    
                    NewListbool = new List<bool>(CurrentHeight);
                    for (int Y = 0; Y < CurrentHeight; ++Y)
                        NewListbool.Add(false);
                    ListUnitSize.Add(NewListbool);
                }
            }
            //List is getting smaller.
            else if (OldWidth > NewValue)
            {
                ListUnitSize.RemoveRange(ListUnitSize.Count - (OldWidth - NewValue), OldWidth - NewValue);
                ListUnitSize.RemoveRange(0, OldWidth - NewValue);
                OldWidth = NewValue;
            }
            pbUnitSize.Width = OldWidth * 32;

            if (pbSizePreviewContext == null)
                return;

            this.pbSizePreviewContext.MaximumBuffer = new Size(pbUnitSize.Width, pbUnitSize.Height);
            this.pbSizePreviewGraphicDevice = pbSizePreviewContext.Allocate(pbDrawingSurfaceGraphics, new Rectangle(0, 0, pbUnitSize.Width, pbUnitSize.Height));
            panel1.Refresh();
            DrawUnitSize();
        }

        private void txtLength_ValueChanged(object sender, EventArgs e)
        {
            int NewValue = (int)txtLength.Value;

            //List is getting bigger.
            if (OldHeight < NewValue)
            {
                for (; OldHeight < NewValue; ++OldHeight)
                {
                    for (int Y = 0; Y < ListUnitSize.Count; ++Y)
                    {
                        ListUnitSize[Y].Insert(0, false);
                        ListUnitSize[Y].Add(false);
                    }
                }
            }
            //List is getting smaller.
            else if (OldHeight > NewValue)
            {
                for (int Y = 0; Y < ListUnitSize.Count; ++Y)
                {
                    ListUnitSize[Y].RemoveRange(ListUnitSize[Y].Count - (OldHeight - NewValue), OldHeight - NewValue);
                    ListUnitSize[Y].RemoveRange(0, OldHeight - NewValue);
                }
                OldHeight = NewValue;
            }

            pbUnitSize.Height = NewValue * 32;

            if (pbSizePreviewContext == null)
                return;

            this.pbSizePreviewContext.MaximumBuffer = new Size(pbUnitSize.Width, pbUnitSize.Height);
            this.pbSizePreviewGraphicDevice = pbSizePreviewContext.Allocate(pbDrawingSurfaceGraphics, new Rectangle(0, 0, pbUnitSize.Width, pbUnitSize.Height));
            panel1.Refresh();
            DrawUnitSize();
        }

        private void UnitSizeEditor_Shown(object sender, EventArgs e)
        {
            //Create a new buffer based on the picturebox.
            pbDrawingSurfaceGraphics = pbUnitSize.CreateGraphics();
            this.pbSizePreviewContext = BufferedGraphicsManager.Current;
            this.pbSizePreviewContext.MaximumBuffer = new Size(pbUnitSize.Width, pbUnitSize.Height);
            this.pbSizePreviewGraphicDevice = pbSizePreviewContext.Allocate(pbDrawingSurfaceGraphics, new Rectangle(0, 0, pbUnitSize.Width, pbUnitSize.Height));
            panel1.Refresh();
            DrawUnitSize();
        }

        private void rbNone_CheckedChanged(object sender, EventArgs e)
        {
            txtWidth.Enabled = false;
            txtLength.Enabled = false;
            DrawUnitSize();
        }

        private void rbSizeOnly_CheckedChanged(object sender, EventArgs e)
        {
            txtWidth.Enabled = true;
            txtLength.Enabled = true;
            DrawUnitSize();
        }

        private void rbCustomSizeBox_CheckedChanged(object sender, EventArgs e)
        {
            txtWidth.Enabled = true;
            txtLength.Enabled = true;
            DrawUnitSize();
        }
    }
}
