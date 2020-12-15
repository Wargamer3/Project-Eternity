using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ProjectEternity.Editors.AttackEditor
{
    public partial class MAPAttackEditor : Form
    {
        //Buffer used to draw in the panDrawingSurface.
        BufferedGraphicsContext pbMapPreviewContext;
        BufferedGraphics pbMapPreviewGraphicDevice;
        Graphics pbDrawingSurfaceGraphics;
        public List<List<bool>> ListAttackchoice;
        public int AttackWidth = 1;
        public int AttackHeight = 1;

        public MAPAttackEditor()
        {
            InitializeComponent();

            ListAttackchoice = new List<List<bool>>(3);
            ListAttackchoice.Add(new List<bool>(3));
            ListAttackchoice.Add(new List<bool>(3));
            ListAttackchoice.Add(new List<bool>(3));

            ListAttackchoice[0].Add(false);
            ListAttackchoice[0].Add(false);
            ListAttackchoice[0].Add(false);
            ListAttackchoice[1].Add(false);
            ListAttackchoice[1].Add(false);
            ListAttackchoice[1].Add(false);
            ListAttackchoice[2].Add(false);
            ListAttackchoice[2].Add(false);
            ListAttackchoice[2].Add(false);

            pbMAPArea.Size = new Size(3 * 32, 3 * 32);

            //Create a new buffer based on the picturebox.
            pbDrawingSurfaceGraphics = pbMAPArea.CreateGraphics();
            this.pbMapPreviewContext = BufferedGraphicsManager.Current;
            this.pbMapPreviewContext.MaximumBuffer = new Size(pbMAPArea.Width, pbMAPArea.Height);
            this.pbMapPreviewGraphicDevice = pbMapPreviewContext.Allocate(pbDrawingSurfaceGraphics, new Rectangle(0, 0, pbMAPArea.Width, pbMAPArea.Height));
        }

        private void DrawMapArea()
        {
            pbMapPreviewGraphicDevice.Graphics.Clear(Color.White);

            for (int X = 0; X < AttackWidth * 2 + 1; ++X)
                for (int Y = 0; Y < AttackHeight * 2 + 1; ++Y)
                    if (ListAttackchoice[X][Y])
                        pbMapPreviewGraphicDevice.Graphics.FillRectangle(Brushes.Red, X * 32, Y * 32, 32, 32);


            for (int X = 32; X < pbMAPArea.Width; X += 32)
            {
                pbMapPreviewGraphicDevice.Graphics.DrawLine(Pens.Black, X, 0, X, pbMAPArea.Height);
            }

            for (int Y = 32; Y < pbMAPArea.Height; Y += 32)
            {
                pbMapPreviewGraphicDevice.Graphics.DrawLine(Pens.Black, 0, Y, pbMAPArea.Width, Y);
            }

            pbMapPreviewGraphicDevice.Render();
        }

        private void pbMAPArea_Paint(object sender, PaintEventArgs e)
        {
            DrawMapArea();
        }

        private void pbMAPArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X < 0 || e.Y < 0 ||
                e.X >= pbMAPArea.Size.Width || e.Y >= pbMAPArea.Height)
                return;

            if (e.Button == MouseButtons.Left)
            {
                int X = e.X / 32;
                int Y = e.Y / 32;
                ListAttackchoice[X][Y] = true;
                DrawMapArea();
            }
            else if (e.Button == MouseButtons.Right)
            {
                int X = e.X / 32;
                int Y = e.Y / 32;
                ListAttackchoice[X][Y] = false;
                DrawMapArea();
            }
        }

        private void txtAttackWidth_ValueChanged(object sender, EventArgs e)
        {
            int NewValue = (int)txtAttackWidth.Value;

            //List is getting bigger.
            if (AttackWidth < NewValue)
            {
                for (; AttackWidth < NewValue; ++AttackWidth)
                {
                    List<bool> NewListbool = new List<bool>(AttackHeight);
                    for (int Y = 0; Y < AttackHeight * 2 + 1; ++Y)
                        NewListbool.Add(false);
                    ListAttackchoice.Insert(0, NewListbool);
                    
                    NewListbool = new List<bool>(AttackHeight);
                    for (int Y = 0; Y < AttackHeight * 2 + 1; ++Y)
                        NewListbool.Add(false);
                    ListAttackchoice.Add(NewListbool);
                }
            }
            //List is getting smaller.
            else if (AttackWidth > NewValue)
            {
                ListAttackchoice.RemoveRange(ListAttackchoice.Count - (AttackWidth - NewValue), AttackWidth - NewValue);
                ListAttackchoice.RemoveRange(0, AttackWidth - NewValue);
                AttackWidth = NewValue;
            }

            pbMAPArea.Width = (AttackWidth * 2 + 1) * 32;
            this.pbMapPreviewContext.MaximumBuffer = new Size(pbMAPArea.Width, pbMAPArea.Height);
            this.pbMapPreviewGraphicDevice = pbMapPreviewContext.Allocate(pbDrawingSurfaceGraphics, new Rectangle(0, 0, pbMAPArea.Width, pbMAPArea.Height));
            panel1.Refresh();
            DrawMapArea();
        }

        private void txtAttackHeight_ValueChanged(object sender, EventArgs e)
        {
            int NewValue = (int)txtAttackHeight.Value;

            //List is getting bigger.
            if (AttackHeight < NewValue)
            {
                for (; AttackHeight < NewValue; ++AttackHeight)
                {
                    for (int Y = 0; Y < ListAttackchoice.Count; ++Y)
                    {
                        ListAttackchoice[Y].Insert(0, false);
                        ListAttackchoice[Y].Add(false);
                    }
                }
            }
            //List is getting smaller.
            else if (AttackHeight > NewValue)
            {
                for (int Y = 0; Y < ListAttackchoice.Count; ++Y)
                {
                    ListAttackchoice[Y].RemoveRange(ListAttackchoice[Y].Count - (AttackHeight - NewValue), AttackHeight - NewValue);
                    ListAttackchoice[Y].RemoveRange(0, AttackHeight - NewValue);
                }
                AttackHeight = NewValue;
            }

            pbMAPArea.Height = (AttackHeight * 2 + 1) * 32;
            this.pbMapPreviewContext.MaximumBuffer = new Size(pbMAPArea.Width, pbMAPArea.Height);
            this.pbMapPreviewGraphicDevice = pbMapPreviewContext.Allocate(pbDrawingSurfaceGraphics, new Rectangle(0, 0, pbMAPArea.Width, pbMAPArea.Height));
            panel1.Refresh();
            DrawMapArea();
        }

        private void MAPAttackEditor_Shown(object sender, EventArgs e)
        {
            panel1.Refresh();
            DrawMapArea();
        }
    }
}
