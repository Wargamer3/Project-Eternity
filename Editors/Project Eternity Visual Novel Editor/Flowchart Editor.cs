using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.GameScreens.VisualNovelScreen;

namespace ProjectEternity.Editors.VisualNovelEditor
{
    public partial class FlowchartEditor : Form
    {
        public delegate void DialogSelect(Dialog DialogSelected);

        public DialogSelect OnDialogSelect;

        public FlowchartEditor()
        {
            InitializeComponent();
            VisualNovelViewer.DrawScripts = true;
        }

        public void Init(VisualNovel ActiveVisualNovel)
        {
            VisualNovelViewer.ActiveVisualNovel = ActiveVisualNovel;
            VisualNovelViewer.Preload();
            VisualNovelViewer.Services.AddService<GraphicsDevice>(VisualNovelViewer.GraphicsDevice);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true; // this cancels the close event.
        }

        //End of a click, mostly will execute an action.
        private void pbVisualNovelPreview_MouseClick(object sender, MouseEventArgs e)
        {
            VisualNovelViewer.MouseX = e.X;
            VisualNovelViewer.MouseY = e.Y;

            Microsoft.Xna.Framework.Point ScriptEditorOrigin = VisualNovelViewer.ScriptEditorOrigin;

            #region Left click

            if (e.Button == MouseButtons.Left)
            {
                Point MousePos = new Point(e.X + ScriptEditorOrigin.X, e.Y + ScriptEditorOrigin.Y);
                //Clicked on an object.
                if (VisualNovelViewer.MovingScripts.Count > 0)
                {
                    VisualNovelViewer.MovingScripts.Clear();//No need to move scripts at this point, make sure it's cleared.
                }
                else
                {
                    //Lool for a script under the mouse to find if it's a script that you were moving.
                    for (int S = 0; S < VisualNovelViewer.ActiveVisualNovel.ListDialog.Count; S++)
                    {//If linking scripts together and it's not linking itself.
                        #region Linking events

                        if (VisualNovelViewer.ScriptLinkTypeChoice != ScriptLinkType.None)
                        {//Make sure it's not pointing on itself.
                            if (VisualNovelViewer.ScriptLink != S)
                            {
                                switch (VisualNovelViewer.ScriptLinkTypeChoice)
                                {
                                    case ScriptLinkType.FromDialog:
                                        //If linking to an other other dialog and the Script is a dialog.
                                        //See if the mouse is over the left linking box of a script.
                                        if (MousePos.X >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 10 && MousePos.X < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 3
                                            && MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 2 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 9)
                                        {//If it's not already linked.
                                            if (!(VisualNovelViewer.ActiveVisualNovel.ListDialog[VisualNovelViewer.ScriptLink]).ListNextDialog.Contains(S))
                                            {//Add the link.
                                                (VisualNovelViewer.ActiveVisualNovel.ListDialog[VisualNovelViewer.ScriptLink]).ListNextDialog.Add(S);
                                                //If not holding shift, reset the starting link.
                                                if (Control.ModifierKeys != Keys.Shift)
                                                {
                                                    VisualNovelViewer.ScriptLinkStartingPoint = Microsoft.Xna.Framework.Point.Zero;
                                                    VisualNovelViewer.ScriptLink = -1;
                                                    VisualNovelViewer.ScriptLinkTypeChoice = ScriptLinkType.None;
                                                }
                                                return;
                                            }
                                        }
                                        break;

                                    //A dialog linked to an other dialog.
                                    case ScriptLinkType.ToDialog:
                                        if (MousePos.X >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + 83 && MousePos.X < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + 90
                                            && MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 28 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 35)
                                        {
                                            //If it's not already linked.
                                            if (!(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]).ListNextDialog.Contains(VisualNovelViewer.ScriptLink))
                                            {//Add the link.
                                                (VisualNovelViewer.ActiveVisualNovel.ListDialog[S]).ListNextDialog.Add(VisualNovelViewer.ScriptLink);
                                                //If not holding shift, reset the starting link.
                                                if (Control.ModifierKeys != Keys.Shift)
                                                {
                                                    VisualNovelViewer.ScriptLinkStartingPoint = Microsoft.Xna.Framework.Point.Zero;
                                                    VisualNovelViewer.ScriptLink = -1;
                                                    VisualNovelViewer.ScriptLinkTypeChoice = ScriptLinkType.None;
                                                }
                                                return;
                                            }
                                        }
                                        break;
                                }
                            }
                            //If it's pointing at itself.
                            else if (VisualNovelViewer.ScriptLink == S)
                            {
                                //See if the mouse is over a linking box.
                                if ((MousePos.X >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 10 && MousePos.X < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 3
                                        && MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 2 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 9)
                                    || ((MousePos.X >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + 83 && MousePos.X < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + 90)
                                        && ((MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 28 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 35)
                                        || MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 40 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 47)))
                                {//Don't reset the ScriptLinking.(So you won't clear the ScriptLinking if you click on the linking box instead of draging it)
                                    return;
                                }
                            }
                        }

                        #endregion

                        //Found one
                        if (CheckScriptCollisionWithMouse(MousePos.X, MousePos.Y, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y) && VisualNovelViewer.ListDialogSelected.Contains(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]))
                        {
                            return;
                        }
                    }
                    //If not holding shift, reset the starting link.
                    if (Control.ModifierKeys != Keys.Shift)
                    {
                        VisualNovelViewer.ScriptLinkStartingPoint = Microsoft.Xna.Framework.Point.Zero;
                        VisualNovelViewer.ScriptLink = -1;
                        VisualNovelViewer.ScriptLinkTypeChoice = ScriptLinkType.None;
                        VisualNovelViewer.ListDialogSelected.Clear();//If you reached this place, you weren't moving something so unselect everything.
                    }
                }
            }

            #endregion

            #region Right click

            //Right click and not moving scripts and not linking scripts.
            else if (e.Button == MouseButtons.Right && VisualNovelViewer.MovingScripts.Count == 0 && VisualNovelViewer.ScriptLinkTypeChoice == ScriptLinkType.None)
            {
                //Lool for a script under the mouse to find an object that you were moving.
                for (int S = 0; S < VisualNovelViewer.ActiveVisualNovel.ListDialog.Count; S++)
                {
                    Point MousePos = new Point(e.X + ScriptEditorOrigin.X, e.Y + ScriptEditorOrigin.Y);
                    //Script detected, open the context menu.
                    if (CheckScriptCollisionWithMouse(MousePos.X, MousePos.Y, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y))
                    {//Nothing selected, show New, Edit and delete.
                        if (VisualNovelViewer.ListDialogSelected.Count == 0)
                        {
                            VisualNovelViewer.ListDialogSelected.Add(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]);
                            tsmEdit.Visible = true;
                            tsmDelete.Visible = true;
                        }//One thing selected, Either it's the one we want or not, it's the new selected object, show New, Edit and delete.
                        else if (VisualNovelViewer.ListDialogSelected.Count == 1)
                        {
                            VisualNovelViewer.ListDialogSelected.Clear();
                            VisualNovelViewer.ListDialogSelected.Add(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]);
                            tsmEdit.Visible = true;
                            tsmDelete.Visible = true;
                        }
                        else//Multiple Dialogs selected.
                        {//If it's already selected, just show New and Delete. (Can't edit multiple Dialogs)
                            if (VisualNovelViewer.ListDialogSelected.Contains(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]))
                            {
                                tsmEdit.Visible = false;
                                tsmDelete.Visible = true;
                            }
                            else
                            {//It's the new selected object, show New, Edit and delete.
                                VisualNovelViewer.ListDialogSelected.Clear();
                                VisualNovelViewer.ListDialogSelected.Add(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]);
                                tsmEdit.Visible = true;
                                tsmDelete.Visible = true;
                            }
                        }
                        //Open the context menu.
                        cmsScriptEditor.Show(VisualNovelViewer, e.Location);
                        return;
                    }
                    else
                    {
                        #region Linking boxes

                        //Right side linking box.
                        if (MousePos.X >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + VisualNovelViewer.BoxWidth + 7 && MousePos.X < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + VisualNovelViewer.BoxWidth + 14)
                        {//Script box link.
                            if (MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 40 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 47)
                            {
                                VisualNovelViewer.ActiveVisualNovel.ListDialog[S].ListNextDialog.Clear();
                                return;
                            }
                            else
                            {
                                //Dialog box link.
                                if (MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 28 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 35)
                                {
                                    VisualNovelViewer.ActiveVisualNovel.ListDialog[S].ListNextDialog.Clear();
                                    return;
                                }
                            }
                        }
                        //Left side linking box.
                        else if (MousePos.X >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 10 && MousePos.X < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 3
                            && MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 2 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 9)
                        {
                            //Loop in the ListDialog to find a Dialog linked to it.
                            for (int D = 0; D < VisualNovelViewer.ActiveVisualNovel.ListDialog.Count; D++)
                            {
                                //If the Dialog is linked to the selected Dialog.
                                if ((VisualNovelViewer.ActiveVisualNovel.ListDialog[D]).ListNextDialog.Contains(S))
                                {//Remove the link.
                                    (VisualNovelViewer.ActiveVisualNovel.ListDialog[D]).ListNextDialog.Remove(S);
                                }
                            }
                            return;
                        }

                        #endregion
                    }
                }
                //No linking box detected.
                tsmEdit.Visible = false;
                tsmDelete.Visible = false;
                cmsScriptEditor.Show(VisualNovelViewer, e.Location);
            }

            #endregion
        }

        private void pbVisualNovelPreview_MouseMove(object sender, MouseEventArgs e)
        {
            int MouseOldX = VisualNovelViewer.MouseX;
            int MouseOldY = VisualNovelViewer.MouseY;
            VisualNovelViewer.MouseX = e.X;
            VisualNovelViewer.MouseY = e.Y;

            Microsoft.Xna.Framework.Point ScriptEditorOrigin = VisualNovelViewer.ScriptEditorOrigin;

            if (e.Button == MouseButtons.Left)
            {//Not currently moving something
                if (VisualNovelViewer.MovingScripts.Count == 0)
                {//If there is a line between a script and the mouse, draw with the mouse position so it draw at the right place.
                    if (VisualNovelViewer.ScriptLinkTypeChoice != ScriptLinkType.None)
                    {
                    }

                    #region Select Scripts

                    else if (Control.ModifierKeys == Keys.Shift)
                    {
                        Point MousePos = new Point(e.X + ScriptEditorOrigin.X, e.Y + ScriptEditorOrigin.Y);
                        //Lool for a script under the mouse.
                        for (int S = 0; S < VisualNovelViewer.ActiveVisualNovel.ListDialog.Count; S++)
                        {//Found one
                            if (CheckScriptCollisionWithMouse(MousePos.X, MousePos.Y, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y))
                            {//If it's not already in the list of selected scripts.
                                if (!VisualNovelViewer.ListDialogSelected.Contains(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]))
                                {
                                    VisualNovelViewer.ListDialogSelected.Add(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]);
                                }
                                break;
                            }
                        }
                    }
                    else if (Control.ModifierKeys == Keys.Alt)
                    {
                        ScriptEditorOrigin.X = Math.Max(0, ScriptEditorOrigin.X - e.X - MouseOldX);
                        ScriptEditorOrigin.Y -= e.Y - MouseOldY;
                        VisualNovelViewer.ScriptEditorOrigin = ScriptEditorOrigin;
                    }

                    #endregion
                }
                else
                {
                    #region Move Scripts

                    //Move the selected scripts.
                    for (int S = 0; S < VisualNovelViewer.ListDialogSelected.Count; S++)
                    {
                        #region X Movement

                        //If it's close enough to be inside the Timeline.
                        if (e.X + ScriptEditorOrigin.X - VisualNovelViewer.MovingScripts[S].X < VisualNovelViewer.BoxWidth / 2)
                        {
                            VisualNovelViewer.ListDialogSelected[S].Position.X = 0;
                            //If it's not already in the Timeline, add it.
                            if (!VisualNovelViewer.ActiveVisualNovel.Timeline.Contains(VisualNovelViewer.ListDialogSelected[S]))
                                VisualNovelViewer.ActiveVisualNovel.Timeline.Add((Dialog)VisualNovelViewer.ListDialogSelected[S]);
                        }
                        else
                        {//If it's near the border of the Timeline but not close enough to be inside.
                            if (e.X + ScriptEditorOrigin.X - VisualNovelViewer.MovingScripts[S].X < VisualNovelViewer.BoxWidth)
                                VisualNovelViewer.ListDialogSelected[S].Position.X = VisualNovelViewer.BoxWidth;//Put it next to the timeline.
                            else
                            {//Move it normally.
                                VisualNovelViewer.ListDialogSelected[S].Position.X = e.X + ScriptEditorOrigin.X - VisualNovelViewer.MovingScripts[S].X;
                            }
                            if (VisualNovelViewer.ActiveVisualNovel.Timeline.Contains(VisualNovelViewer.ListDialogSelected[S]))
                                VisualNovelViewer.ActiveVisualNovel.Timeline.Remove((Dialog)VisualNovelViewer.ListDialogSelected[S]);
                        }

                        #endregion

                        #region Y Movement

                        //If in the Timeline.
                        if (VisualNovelViewer.ListDialogSelected[S].Position.X == 0)
                        {//Snap it to a grid of the size of the imgScript.
                            VisualNovelViewer.ListDialogSelected[S].Position.Y = ((e.Y + ScriptEditorOrigin.Y - VisualNovelViewer.MovingScripts[S].Y) / VisualNovelViewer.BoxHeight) * VisualNovelViewer.BoxHeight;
                        }//If it's under 0, snap it to 0.
                        else if (e.Y + ScriptEditorOrigin.Y - VisualNovelViewer.MovingScripts[S].Y < 0)
                            VisualNovelViewer.ListDialogSelected[S].Position.Y = 0;
                        else
                        {//Move it normally.
                            VisualNovelViewer.ListDialogSelected[S].Position.Y = e.Y + ScriptEditorOrigin.Y - VisualNovelViewer.MovingScripts[S].Y;
                        }

                        #endregion
                    }

                    #endregion
                }
            }
            else if (e.Button == MouseButtons.Right)
            { }
        }

        //Called when you first hold a mouse bouton down.
        private void pbVisualNovelPreview_MouseDown(object sender, MouseEventArgs e)
        {
            VisualNovelViewer.MouseX = e.X;
            VisualNovelViewer.MouseY = e.Y;

            Microsoft.Xna.Framework.Point ScriptEditorOrigin = VisualNovelViewer.ScriptEditorOrigin;

            if (e.Button == MouseButtons.Left)
            {
                Point MousePos = new Point(e.X + ScriptEditorOrigin.X, e.Y + ScriptEditorOrigin.Y);
                //Lool for a script under the mouse.
                for (int S = 0; S < VisualNovelViewer.ActiveVisualNovel.ListDialog.Count; S++)
                {//Found one
                    if (CheckScriptCollisionWithMouse(MousePos.X, MousePos.Y, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y))
                    {//If it's not already in the list of selected scripts.
                        if (!VisualNovelViewer.ListDialogSelected.Contains(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]))
                        {
                            if (Control.ModifierKeys != Keys.Shift)
                                VisualNovelViewer.ListDialogSelected.Clear();

                            VisualNovelViewer.ListDialogSelected.Add(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]);
                            OnDialogSelect(VisualNovelViewer.ActiveVisualNovel.ListDialog[S]);
                        }
                        if (Control.ModifierKeys != Keys.Shift)
                        {
                            for (int M = 0; M < VisualNovelViewer.ListDialogSelected.Count; M++)
                            {
                                VisualNovelViewer.MovingScripts.Add(new Microsoft.Xna.Framework.Point(MousePos.X - VisualNovelViewer.ListDialogSelected[M].Position.X, MousePos.Y - VisualNovelViewer.ListDialogSelected[M].Position.Y));
                            }
                        }
                        return;//Stop here so it won't execute the rest.
                    }
                    else
                    {
                        if (VisualNovelViewer.ScriptLinkTypeChoice == ScriptLinkType.None)
                        {//Right side linking box.
                            if (MousePos.X >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + VisualNovelViewer.BoxWidth + 7 && MousePos.X < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + VisualNovelViewer.BoxWidth + 14)
                            {
                                //Dialog box.
                                if (MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 28 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 35)
                                {
                                    VisualNovelViewer.ScriptLinkStartingPoint = new Microsoft.Xna.Framework.Point(VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X + VisualNovelViewer.BoxWidth + 10, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 31);
                                    VisualNovelViewer.ScriptLink = S;
                                    VisualNovelViewer.ScriptLinkTypeChoice = ScriptLinkType.FromDialog;
                                }
                            }
                            //Left side linking box.
                            else if (MousePos.X >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 10 && MousePos.X < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 3
                                && MousePos.Y >= VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 2 && MousePos.Y < VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 9)
                            {
                                VisualNovelViewer.ScriptLinkStartingPoint = new Microsoft.Xna.Framework.Point(VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.X - 7, VisualNovelViewer.ActiveVisualNovel.ListDialog[S].Position.Y + 5);
                                VisualNovelViewer.ScriptLink = S;
                                VisualNovelViewer.ScriptLinkTypeChoice = ScriptLinkType.ToDialog;
                            }
                        }
                    }
                }
            }
        }

        private bool CheckScriptCollisionWithMouse(int MouseX, int MouseY, int ScriptX, int ScriptY)
        {
            if (MouseX >= ScriptX && MouseX <= ScriptX + VisualNovelViewer.BoxWidth && MouseY >= ScriptY && MouseY <= ScriptY + VisualNovelViewer.BoxHeight)
                return true;
            return false;
        }

        #region Tool Strip Menu

        private void tmsNewDialog_Click(object sender, EventArgs e)
        {
            //btnAddFrame_Click(sender, e);
        }

        private void tsmEdit_Click(object sender, EventArgs e)
        {
            /*if (VisualNovelViewer.ListDialogSelected.Count == 1)
            {
                lstDialogs.SelectedIndex = VisualNovelViewer.ActiveVisualNovel.ListDialog.IndexOf(VisualNovelViewer.ListDialogSelected[0]);
            }*/
        }

        private void tsmDelete_Click(object sender, EventArgs e)
        {
            /*for (int S = 0; S < VisualNovelViewer.ListDialogSelected.Count; S++)
            {//Select the dialog.
                lstDialogs.SelectedIndex = VisualNovelViewer.ActiveVisualNovel.ListDialog.IndexOf(VisualNovelViewer.ListDialogSelected[S]);
                btnDeleteFrame_Click(sender, e);
            }*/
        }

        #endregion

    }
}
