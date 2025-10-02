using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.Editors.CutsceneEditor
{
    public partial class CutsceneViewer : UserControl
    {
        private enum ScriptLinkTypes { None, Trigger, Event };

        private enum ActiveScriptTypes { None, ActionScript, DataContainer };

        public delegate void SelectScriptDelegate(CutsceneScript SelectedScript);

        private Font fntScriptName;
        private Brush brScriptBrush;
        private Brush brScriptLink;

        #region Script Images

        private UnsafeScriptImage imgScriptTopLeft;
        private UnsafeScriptImage imgScriptTopMiddle;
        private UnsafeScriptImage imgScriptTopRight;
        private UnsafeScriptImage imgScriptMiddleLeft;
        private UnsafeScriptImage imgScriptMiddleMiddle;
        private UnsafeScriptImage imgScriptMiddleRight;
        private UnsafeScriptImage imgScriptBottomLeft;
        private UnsafeScriptImage imgScriptBottomMiddle;
        private UnsafeScriptImage imgScriptBottomRight;

        #endregion

        //Buffer used to draw in the panDrawingSurface.
        private BufferedGraphicsContext pbMapPreviewContext;

        private BufferedGraphics pbMapPreviewGraphicDevice;
        private Graphics pbDrawingSurfaceGraphics;
        private Point ScriptStartingPos;//Point from which to start drawing the scripts

        private Point MousePosOld;
        public Cutscene ActiveCutscene;

        private int ActiveScriptIndex;
        private ActiveScriptTypes ActiveScriptType;

        private CutsceneActionScript ScriptLink;
        private int ScriptLinkIndex;
        private ScriptLinkTypes ScriptLinkType;
        private int ScriptLinkEventIndex;
        private Point ScriptLinkStartPos;
        private Point ScriptLinkEndPos;

        public SelectScriptDelegate SelectScript;

        public CutsceneViewer()
        {
            InitializeComponent();
        }

        public void Init(SelectScriptDelegate SelectScript)
        {
            this.SelectScript = SelectScript;
            ActiveScriptIndex = -1;

            fntScriptName = new Font("Calibri", 8);
            brScriptBrush = Brushes.White;
            brScriptLink = Brushes.Black;

            imgScriptTopLeft = new UnsafeScriptImage("Content/Editors/Scripts/ScriptTopLeft2.png");
            imgScriptTopMiddle = new UnsafeScriptImage("Content/Editors/Scripts/ScriptTopMiddle2.png");
            imgScriptTopRight = new UnsafeScriptImage("Content/Editors/Scripts/ScriptTopRight2.png");
            imgScriptMiddleLeft = new UnsafeScriptImage("Content/Editors/Scripts/ScriptMiddleLeft2.png");
            imgScriptMiddleMiddle = new UnsafeScriptImage("Content/Editors/Scripts/ScriptMiddleMiddle2.png");
            imgScriptMiddleRight = new UnsafeScriptImage("Content/Editors/Scripts/ScriptMiddleRight2.png");
            imgScriptBottomLeft = new UnsafeScriptImage("Content/Editors/Scripts/ScriptBottomLeft2.png");
            imgScriptBottomMiddle = new UnsafeScriptImage("Content/Editors/Scripts/ScriptBottomMiddle2.png");
            imgScriptBottomRight = new UnsafeScriptImage("Content/Editors/Scripts/ScriptBottomRight2.png");

            //Create a new buffer based on the picturebox.
            pbDrawingSurfaceGraphics = panDrawingSurface.CreateGraphics();
            this.pbMapPreviewContext = BufferedGraphicsManager.Current;
            this.pbMapPreviewContext.MaximumBuffer = new Size(panDrawingSurface.Width, panDrawingSurface.Height);
            this.pbMapPreviewGraphicDevice = pbMapPreviewContext.Allocate(pbDrawingSurfaceGraphics, new Rectangle(0, 0, panDrawingSurface.Width, panDrawingSurface.Height));

            if (ActiveCutscene != null)
            {
                panDrawingSurface_Resize(this, null);

                InitCutscene(ActiveCutscene);
            }
        }

        public void InitCutscene(Cutscene ActiveCutscene)
        {
            for (int S = 0; S < ActiveCutscene.DicActionScript.Count; S++)
                InitScript(ActiveCutscene.DicActionScript[S]);

            for (int D = 0; D < ActiveCutscene.ListDataContainer.Count; D++)
                InitScript(ActiveCutscene.ListDataContainer[D]);
        }
        
        public void InitScript(CutsceneScript NewScript)
        {
            NewScript.Lock();
            NewScript.DrawImage(imgScriptTopLeft, 0, 0);
            NewScript.DrawImage(imgScriptMiddleLeft, 0, imgScriptTopLeft.ImageBase.Height, 0, NewScript.ScriptSize.Height - imgScriptTopMiddle.ImageBase.Height - imgScriptBottomMiddle.ImageBase.Height - 1);
            NewScript.DrawImage(imgScriptBottomLeft, 0, NewScript.ScriptSize.Height - imgScriptBottomLeft.ImageBase.Height);

            NewScript.DrawImage(imgScriptTopMiddle, imgScriptTopLeft.ImageBase.Width, 0, NewScript.ScriptSize.Width - imgScriptTopRight.ImageBase.Width - 1);

            NewScript.DrawImage(imgScriptMiddleMiddle, imgScriptMiddleLeft.ImageBase.Width,
                                                        imgScriptTopLeft.ImageBase.Height,
                                                        NewScript.ScriptSize.Width - imgScriptMiddleLeft.ImageBase.Width - imgScriptMiddleRight.ImageBase.Width - 1,
                                                        NewScript.ScriptSize.Height - imgScriptTopMiddle.ImageBase.Height - imgScriptBottomMiddle.ImageBase.Height - 1);

            NewScript.DrawImage(imgScriptBottomMiddle, imgScriptBottomLeft.ImageBase.Width,
                                                        NewScript.ScriptSize.Height - imgScriptBottomMiddle.ImageBase.Height,
                                                        NewScript.ScriptSize.Width - imgScriptBottomLeft.ImageBase.Width - imgScriptBottomRight.ImageBase.Width - 1);

            NewScript.DrawImage(imgScriptTopRight, NewScript.ScriptSize.Width - imgScriptTopRight.ImageBase.Width, 0);
            NewScript.DrawImage(imgScriptMiddleRight, NewScript.ScriptSize.Width - imgScriptMiddleRight.ImageBase.Width, imgScriptTopRight.ImageBase.Height, 0, NewScript.ScriptSize.Height - imgScriptTopMiddle.ImageBase.Height - imgScriptBottomMiddle.ImageBase.Height - 1);
            NewScript.DrawImage(imgScriptBottomRight, NewScript.ScriptSize.Width - imgScriptBottomRight.ImageBase.Width, NewScript.ScriptSize.Height - imgScriptBottomRight.ImageBase.Height);

            NewScript.Unlock();
        }
        
        public void DrawScripts()
        {
            if (pbMapPreviewGraphicDevice == null)
                return;

            pbMapPreviewGraphicDevice.Graphics.Clear(Color.White);

            for (int S = ActiveCutscene.DicActionScript.Count - 1; S >= 0; --S)
            {
                int X = ActiveCutscene.DicActionScript[S].ScriptSize.X - ScriptStartingPos.X;
                int Y = ActiveCutscene.DicActionScript[S].ScriptSize.Y - ScriptStartingPos.Y;

                pbMapPreviewGraphicDevice.Graphics.DrawImage(ActiveCutscene.DicActionScript[S].ScriptImage, X, Y);
                pbMapPreviewGraphicDevice.Graphics.DrawString(ActiveCutscene.DicActionScript[S].ToString(), fntScriptName, brScriptBrush, X, Y);

                if (ActiveScriptType == ActiveScriptTypes.ActionScript && S == ActiveScriptIndex)
                    pbMapPreviewGraphicDevice.Graphics.DrawRectangle(Pens.Black, X, Y, ActiveCutscene.DicActionScript[S].ScriptSize.Width, ActiveCutscene.DicActionScript[S].ScriptSize.Height);

                for (int T = ActiveCutscene.DicActionScript[S].NameTriggers.Length - 1; T >= 0; --T)
                {
                    pbMapPreviewGraphicDevice.Graphics.DrawString(ActiveCutscene.DicActionScript[S].NameTriggers[T], fntScriptName, brScriptBrush, X, Y + 15 + T * 12);
                    pbMapPreviewGraphicDevice.Graphics.FillRectangle(brScriptLink, X - 10, Y + 19 + T * 12, 5, 5);
                }
                for (int E = ActiveCutscene.DicActionScript[S].NameEvents.Length - 1; E >= 0; --E)
                {
                    pbMapPreviewGraphicDevice.Graphics.DrawString(ActiveCutscene.DicActionScript[S].NameEvents[E], fntScriptName, brScriptBrush,
                                                                                                            X + ActiveCutscene.DicActionScript[S].ScriptSize.Width - 5 - pbMapPreviewGraphicDevice.Graphics.MeasureString(ActiveCutscene.DicActionScript[S].NameEvents[E], fntScriptName).Width,
                                                                                                            Y + ActiveCutscene.DicActionScript[S].ScriptSize.Height - ActiveCutscene.DicActionScript[S].NameEvents.Length * 12 - 4 + E * 12);
                    pbMapPreviewGraphicDevice.Graphics.FillRectangle(brScriptLink, X + ActiveCutscene.DicActionScript[S].ScriptSize.Width + 5, Y + ActiveCutscene.DicActionScript[S].ScriptSize.Height - ActiveCutscene.DicActionScript[S].NameEvents.Length * 12 + 1 + E * 12, 5, 5);

                    for (int i = 0; i < ActiveCutscene.DicActionScript[S].ArrayEvents[E].Count; i++)
                    {
                        pbMapPreviewGraphicDevice.Graphics.DrawLine(Pens.Black,
                                                                    X + ActiveCutscene.DicActionScript[S].ScriptSize.Width + 7,
                                                                    Y + ActiveCutscene.DicActionScript[S].ScriptSize.Height - ActiveCutscene.DicActionScript[S].NameEvents.Length * 12 + 3 + E * 12,
                                                                    ActiveCutscene.DicActionScript[ActiveCutscene.DicActionScript[S].ArrayEvents[E][i].LinkedScriptIndex].ScriptSize.X - ScriptStartingPos.X - 8,
                                                                    ActiveCutscene.DicActionScript[ActiveCutscene.DicActionScript[S].ArrayEvents[E][i].LinkedScriptIndex].ScriptSize.Y - ScriptStartingPos.Y + 21 + ActiveCutscene.DicActionScript[S].ArrayEvents[E][i].LinkedScriptTriggerIndex * 12);
                    }
                }
            }

            for (int D = ActiveCutscene.ListDataContainer.Count - 1; D >= 0; --D)
            {
                if (ActiveScriptType == ActiveScriptTypes.DataContainer && D == ActiveScriptIndex)
                    pbMapPreviewGraphicDevice.Graphics.DrawRectangle(Pens.Black, ActiveCutscene.ListDataContainer[D].ScriptSize);

                pbMapPreviewGraphicDevice.Graphics.DrawImage(ActiveCutscene.ListDataContainer[D].ScriptImage, ActiveCutscene.ListDataContainer[D].ScriptSize.X, ActiveCutscene.ListDataContainer[D].ScriptSize.Y);
                pbMapPreviewGraphicDevice.Graphics.DrawString(ActiveCutscene.ListDataContainer[D].ToString(), fntScriptName, brScriptBrush, ActiveCutscene.ListDataContainer[D].ScriptSize.X, ActiveCutscene.ListDataContainer[D].ScriptSize.Y);
            }

            if (ScriptLink != null)
            {
                pbMapPreviewGraphicDevice.Graphics.DrawLine(Pens.Black, ScriptLinkStartPos.X - ScriptStartingPos.X, ScriptLinkStartPos.Y - ScriptStartingPos.Y, ScriptLinkEndPos.X, ScriptLinkEndPos.Y);
            }

            pbMapPreviewGraphicDevice.Render();
        }

        private void tsmCopyScriptAsText_Click(object sender, EventArgs e)
        {
            CutsceneScript ActiveScript = null;
            if (ActiveScriptType == ActiveScriptTypes.ActionScript)
            {
                ActiveScript = ActiveCutscene.DicActionScript[ActiveScriptIndex];
            }
            else if (ActiveScriptType == ActiveScriptTypes.DataContainer)
            {
                ActiveScript = ActiveCutscene.ListDataContainer[ActiveScriptIndex];
            }

            List<string> Output = new List<string>();
            Output.Add(ActiveScript.Name);

            System.Reflection.PropertyInfo[] ScriptProperties = ActiveScript.GetType().GetProperties();
            for (int i = 0; i < ScriptProperties.Length; ++i)
            {
                if (ScriptProperties[i].CanWrite)
                {
                    if (ScriptProperties[i].PropertyType.IsArray)
                    {
                        object[] ArrayValue = (object[])ScriptProperties[i].GetValue(ActiveScript);
                        Output.Add(string.Join(",", ArrayValue));
                    }
                    else
                    {
                        Output.Add(ScriptProperties[i].GetValue(ActiveScript).ToString());
                    }
                }
            }
            Clipboard.SetText(string.Join(", ", Output));
            ActiveScriptIndex = -1;
        }

        private void tsmDeleteScript_Click(object sender, EventArgs e)
        {
            if (ActiveScriptType == ActiveScriptTypes.ActionScript)
            {
                for (int S = 0; S < ActiveCutscene.DicActionScript.Count; S++)
                {
                    for (int E = 0; E < ActiveCutscene.DicActionScript[S].ArrayEvents.Length; E++)
                    {
                        for (int i = 0; i < ActiveCutscene.DicActionScript[S].ArrayEvents[E].Count; i++)
                        {
                            if (ActiveCutscene.DicActionScript[S].ArrayEvents[E][i].LinkedScriptIndex == ActiveScriptIndex)
                                ActiveCutscene.DicActionScript[S].ArrayEvents[E].RemoveAt(i--);
                            else if (ActiveCutscene.DicActionScript[S].ArrayEvents[E][i].LinkedScriptIndex > ActiveScriptIndex)
                            {
                                ActiveCutscene.DicActionScript[S].ArrayEvents[E][i] = new EventInfo(ActiveCutscene.DicActionScript[S].ArrayEvents[E][i].LinkedScriptIndex - 1, ActiveCutscene.DicActionScript[S].ArrayEvents[E][i].LinkedScriptTriggerIndex);
                            }
                        }
                    }
                }
                ActiveCutscene.RemoveActionScript(ActiveScriptIndex);
            }
            else if (ActiveScriptType == ActiveScriptTypes.DataContainer)
            {
                ActiveCutscene.ListDataContainer.RemoveAt(ActiveScriptIndex);
            }

            ActiveScriptIndex = -1;
            DrawScripts();
        }

        private void tsmCopy_Click(object sender, EventArgs e)
        {
            CutsceneScript NewScript = null;
            if (ActiveScriptType == ActiveScriptTypes.ActionScript)
            {
                NewScript = ActiveCutscene.DicActionScript[ActiveScriptIndex].CopyScript(ActiveCutscene);
            }
            else if (ActiveScriptType == ActiveScriptTypes.DataContainer)
            {
                NewScript = ActiveCutscene.ListDataContainer[ActiveScriptIndex].CopyScript(ActiveCutscene);
            }

            NewScript.ScriptSize.X = 0;
            NewScript.ScriptSize.Y = 0;

            InitScript(NewScript);
            if (NewScript is CutsceneDataContainer)
            {
                UInt32 NextID = 1;
                for (int S = ActiveCutscene.ListDataContainer.Count - 1; S >= 0; --S)
                    if (ActiveCutscene.ListDataContainer[S].Name == NewScript.Name)
                        NextID++;
                ((CutsceneDataContainer)NewScript).ID = NextID;
                ActiveCutscene.ListDataContainer.Add((CutsceneDataContainer)NewScript);
            }
            else
            {
                ActiveCutscene.AddActionScript((CutsceneActionScript)NewScript);
            }
            DrawScripts();
        }

        private void panDrawingSurface_Paint(object sender, PaintEventArgs e)
        {
            DrawScripts();
        }

        private void panDrawingSurface_MouseDown(object sender, MouseEventArgs e)
        {
            Rectangle MouseRec = new Rectangle(e.X + ScriptStartingPos.X, e.Y + ScriptStartingPos.Y, 1, 1);
            MousePosOld = e.Location;

            for (int S = ActiveCutscene.DicActionScript.Count - 1; S >= 0; --S)
            {
                if (MouseRec.IntersectsWith(ActiveCutscene.DicActionScript[S].ScriptSize))
                {
                    SelectScript(ActiveCutscene.DicActionScript[S]);
                    ActiveScriptIndex = S;
                    ActiveScriptType = ActiveScriptTypes.ActionScript;
                    return;
                }
                else if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                {
                    //Triggers.
                    for (int T = ActiveCutscene.DicActionScript[S].NameTriggers.Length - 1; T >= 0; --T)
                    {
                        if (MouseRec.X >= ActiveCutscene.DicActionScript[S].ScriptSize.X - 10 && MouseRec.X <= ActiveCutscene.DicActionScript[S].ScriptSize.X - 5
                             && MouseRec.Y >= ActiveCutscene.DicActionScript[S].ScriptSize.Y + 19 + T * 12 && MouseRec.Y <= ActiveCutscene.DicActionScript[S].ScriptSize.Y + 24 + T * 12)
                        {
                            ScriptLink = ActiveCutscene.DicActionScript[S];
                            ScriptLinkIndex = S;
                            ScriptLinkType = ScriptLinkTypes.Trigger;
                            ScriptLinkEventIndex = T;
                            ScriptLinkStartPos = new Point(ActiveCutscene.DicActionScript[S].ScriptSize.X - 7, ActiveCutscene.DicActionScript[S].ScriptSize.Y + 21 + T * 12);
                            return;
                        }
                    }
                    //Events.
                    for (int E = ActiveCutscene.DicActionScript[S].NameEvents.Length - 1; E >= 0; --E)
                    {
                        if (MouseRec.X >= ActiveCutscene.DicActionScript[S].ScriptSize.X + ActiveCutscene.DicActionScript[S].ScriptSize.Width + 5 && MouseRec.X <= ActiveCutscene.DicActionScript[S].ScriptSize.X + ActiveCutscene.DicActionScript[S].ScriptSize.Width + 10
                            && MouseRec.Y >= ActiveCutscene.DicActionScript[S].ScriptSize.Y + ActiveCutscene.DicActionScript[S].ScriptSize.Height - ActiveCutscene.DicActionScript[S].NameEvents.Length * 12 + 1 + E * 12 && MouseRec.Y <= ActiveCutscene.DicActionScript[S].ScriptSize.Y + ActiveCutscene.DicActionScript[S].ScriptSize.Height - ActiveCutscene.DicActionScript[S].NameEvents.Length * 12 + 6 + E * 12)
                        {
                            ScriptLink = ActiveCutscene.DicActionScript[S];
                            ScriptLinkIndex = S;
                            ScriptLinkType = ScriptLinkTypes.Event;
                            ScriptLinkEventIndex = E;
                            ScriptLinkStartPos = new Point(ActiveCutscene.DicActionScript[S].ScriptSize.X + ActiveCutscene.DicActionScript[S].ScriptSize.Width + 7, ActiveCutscene.DicActionScript[S].ScriptSize.Y + ActiveCutscene.DicActionScript[S].ScriptSize.Height - ActiveCutscene.DicActionScript[S].NameEvents.Length * 12 + 3 + E * 12);
                            return;
                        }
                    }
                }
            }
            for (int D = ActiveCutscene.ListDataContainer.Count - 1; D >= 0; --D)
            {
                if (MouseRec.IntersectsWith(ActiveCutscene.ListDataContainer[D].ScriptSize))
                {
                    SelectScript(ActiveCutscene.ListDataContainer[D]);
                    ActiveScriptIndex = D;
                    ActiveScriptType = ActiveScriptTypes.DataContainer;
                    return;
                }
            }
        }

        private void panDrawingSurface_MouseUp(object sender, MouseEventArgs e)
        {
            Rectangle MouseRec = new Rectangle(e.X + ScriptStartingPos.X, e.Y + ScriptStartingPos.Y, 1, 1);

            #region Action Scripts

            for (int S = ActiveCutscene.DicActionScript.Count - 1; S >= 0; --S)
            {
                if (MouseRec.IntersectsWith(ActiveCutscene.DicActionScript[S].ScriptSize))
                {
                    if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                    {
                        ActiveScriptIndex = -1;
                        return;
                    }
                    else if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
                    {
                        cmsScriptSelectedMenu.Show(this, PointToClient(Cursor.Position));
                        return;
                    }
                }

                #region Linking

                else
                {
                    #region Triggers

                    for (int T = ActiveCutscene.DicActionScript[S].NameTriggers.Length - 1; T >= 0; --T)
                    {
                        if (MouseRec.X >= ActiveCutscene.DicActionScript[S].ScriptSize.X - 10 && MouseRec.X <= ActiveCutscene.DicActionScript[S].ScriptSize.X - 5
                             && MouseRec.Y >= ActiveCutscene.DicActionScript[S].ScriptSize.Y + 19 + T * 12 && MouseRec.Y <= ActiveCutscene.DicActionScript[S].ScriptSize.Y + 24 + T * 12)
                        {
                            EventInfo NewEvent = new EventInfo(S, T);
                            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                            {
                                if (ScriptLink != null && ScriptLink != ActiveCutscene.DicActionScript[S])
                                {//Event to Trigger.
                                    if (ScriptLinkType == ScriptLinkTypes.Event && !ScriptLink.ArrayEvents[ScriptLinkEventIndex].Contains(NewEvent))
                                    {
                                        ScriptLink.ArrayEvents[ScriptLinkEventIndex].Add(NewEvent);
                                    }
                                    ScriptLink = null;
                                    ScriptLinkIndex = -1;
                                    ScriptLinkEventIndex = -1;
                                    ScriptLinkType = ScriptLinkTypes.None;
                                    DrawScripts();
                                    return;
                                }
                            }
                            else if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
                            {
                                for (int i = ActiveCutscene.DicActionScript.Count - 1; i >= 0; --i)
                                    for (int E = ActiveCutscene.DicActionScript[i].ArrayEvents.Length - 1; E >= 0; --E)
                                        ActiveCutscene.DicActionScript[i].ArrayEvents[E].Remove(NewEvent);
                                DrawScripts();
                            }
                        }
                    }

                    #endregion

                    #region Events

                    for (int E = ActiveCutscene.DicActionScript[S].NameEvents.Length - 1; E >= 0; --E)
                    {
                        if (MouseRec.X >= ActiveCutscene.DicActionScript[S].ScriptSize.X + ActiveCutscene.DicActionScript[S].ScriptSize.Width + 5 && MouseRec.X <= ActiveCutscene.DicActionScript[S].ScriptSize.X + ActiveCutscene.DicActionScript[S].ScriptSize.Width + 10
                            && MouseRec.Y >= ActiveCutscene.DicActionScript[S].ScriptSize.Y + ActiveCutscene.DicActionScript[S].ScriptSize.Height - ActiveCutscene.DicActionScript[S].NameEvents.Length * 12 + 1 + E * 12 && MouseRec.Y <= ActiveCutscene.DicActionScript[S].ScriptSize.Y + ActiveCutscene.DicActionScript[S].ScriptSize.Height - ActiveCutscene.DicActionScript[S].NameEvents.Length * 12 + 6 + E * 12)
                        {
                            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                            {
                                if (ScriptLink != null && ScriptLink != ActiveCutscene.DicActionScript[S])
                                {//Trigger to Event.
                                    if (ScriptLinkType == ScriptLinkTypes.Trigger)
                                    {
                                        ActiveCutscene.DicActionScript[S].ArrayEvents[E].Add(new EventInfo(ScriptLinkIndex, ScriptLinkEventIndex));
                                    }
                                    ScriptLink = null;
                                    ScriptLinkIndex = -1;
                                    ScriptLinkEventIndex = -1;
                                    ScriptLinkType = ScriptLinkTypes.None;
                                    DrawScripts();
                                    return;
                                }
                            }
                            else if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
                            {
                                ActiveCutscene.DicActionScript[S].ArrayEvents[E].Clear();
                                DrawScripts();
                            }
                            return;
                        }
                    }

                    #endregion
                }

                #endregion
            }

            #endregion

            //Data containers
            for (int D = ActiveCutscene.ListDataContainer.Count - 1; D >= 0; --D)
            {
                if (MouseRec.IntersectsWith(ActiveCutscene.ListDataContainer[D].ScriptSize))
                {
                    if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                    {
                        ActiveScriptIndex = -1;
                        return;
                    }
                    else if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
                    {
                        cmsScriptSelectedMenu.Show(this, PointToClient(Cursor.Position));
                        return;
                    }
                }
            }

            ScriptLink = null;
            ScriptLinkIndex = -1;
            ScriptLinkEventIndex = -1;
            ScriptLinkType = ScriptLinkTypes.None;
            ActiveScriptIndex = -1;
            DrawScripts();
        }

        private void panDrawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle MouseRec = new Rectangle(e.X, e.Y, 1, 1);
            if (ActiveScriptIndex != -1)
            {
                if (ActiveScriptType == ActiveScriptTypes.ActionScript)
                {
                    ActiveCutscene.DicActionScript[ActiveScriptIndex].ScriptSize.X += MouseRec.X - MousePosOld.X;
                    ActiveCutscene.DicActionScript[ActiveScriptIndex].ScriptSize.Y += MouseRec.Y - MousePosOld.Y;

                    int MaxX = 0, MaxY = 0;
                    for (int S = ActiveCutscene.DicActionScript.Count - 1; S >= 0; --S)
                    {
                        if (ActiveCutscene.DicActionScript[S].ScriptSize.Right > MaxX)
                            MaxX = ActiveCutscene.DicActionScript[S].ScriptSize.Right + 30;

                        if (ActiveCutscene.DicActionScript[S].ScriptSize.Bottom > MaxY)
                            MaxY = ActiveCutscene.DicActionScript[S].ScriptSize.Bottom + 30;
                    }

                    if (MaxX >= panDrawingSurface.Width)
                    {
                        sclScriptWidth.Maximum = MaxX - panDrawingSurface.Size.Width;
                        sclScriptWidth.Visible = true;
                    }
                    else
                        sclScriptWidth.Visible = false;

                    if (MaxY >= panDrawingSurface.Height)
                    {
                        sclScriptHeight.Maximum = MaxY - panDrawingSurface.Size.Height;
                        sclScriptHeight.Visible = true;
                    }
                    else
                        sclScriptHeight.Visible = false;
                }
                else if (ActiveScriptType == ActiveScriptTypes.DataContainer)
                {
                    ActiveCutscene.ListDataContainer[ActiveScriptIndex].ScriptSize.X += MouseRec.X - MousePosOld.X;
                    ActiveCutscene.ListDataContainer[ActiveScriptIndex].ScriptSize.Y += MouseRec.Y - MousePosOld.Y;

                    int MaxX = 0, MaxY = 0;
                    for (int S = ActiveCutscene.ListDataContainer.Count - 1; S >= 0; --S)
                    {
                        if (ActiveCutscene.ListDataContainer[S].ScriptSize.Right > MaxX)
                            MaxX = ActiveCutscene.ListDataContainer[S].ScriptSize.Right;

                        if (ActiveCutscene.ListDataContainer[S].ScriptSize.Bottom > MaxY)
                            MaxY = ActiveCutscene.ListDataContainer[S].ScriptSize.Bottom;
                    }

                    MaxX += 80;
                    MaxY += 30;

                    if (MaxX >= panDrawingSurface.Width)
                    {
                        sclScriptWidth.Maximum = MaxX - panDrawingSurface.Size.Width;
                        sclScriptWidth.Visible = true;
                    }
                    else
                        sclScriptWidth.Visible = false;

                    if (MaxY >= panDrawingSurface.Height)
                    {
                        sclScriptHeight.Maximum = MaxY - panDrawingSurface.Size.Height;
                        sclScriptHeight.Visible = true;
                    }
                    else
                        sclScriptHeight.Visible = false;
                }
                DrawScripts();
            }
            else if (ScriptLink != null)
            {
                ScriptLinkEndPos = e.Location;
                DrawScripts();
            }
            MousePosOld.X = MouseRec.X;
            MousePosOld.Y = MouseRec.Y;
        }

        private void panDrawingSurface_Resize(object sender, EventArgs e)
        {
            sclScriptHeight.Height = panDrawingSurface.Height - sclScriptWidth.Height;
            sclScriptWidth.Width = panDrawingSurface.Width - sclScriptHeight.Width;

            if (pbMapPreviewContext == null)
                return;

            this.pbMapPreviewContext.MaximumBuffer = new Size(panDrawingSurface.Width, panDrawingSurface.Height);
            this.pbMapPreviewGraphicDevice = pbMapPreviewContext.Allocate(pbDrawingSurfaceGraphics, new Rectangle(0, 0, panDrawingSurface.Width, panDrawingSurface.Height));

            int MaxX = 0, MaxY = 0;
            for (int S = ActiveCutscene.DicActionScript.Count - 1; S >= 0; --S)
            {
                if (ActiveCutscene.DicActionScript[S].ScriptSize.Right > MaxX)
                    MaxX = ActiveCutscene.DicActionScript[S].ScriptSize.Right;

                if (ActiveCutscene.DicActionScript[S].ScriptSize.Bottom > MaxY)
                    MaxY = ActiveCutscene.DicActionScript[S].ScriptSize.Bottom;
            }

            MaxX += 80;
            MaxY += 30;

            if (MaxX >= panDrawingSurface.Width)
            {
                sclScriptWidth.Maximum = MaxX - panDrawingSurface.Size.Width;
                sclScriptWidth.Visible = true;
            }
            else
                sclScriptWidth.Visible = false;

            if (MaxY >= panDrawingSurface.Height)
            {
                sclScriptHeight.Maximum = MaxY - panDrawingSurface.Size.Height;
                sclScriptHeight.Visible = true;
            }
            else
                sclScriptHeight.Visible = false;

            DrawScripts();
        }

        private void CutsceneEditor_Shown(object sender, EventArgs e)
        {
            if (ActiveCutscene.DicCutsceneScript.Count == 0)
            {
                ActiveCutscene.DicCutsceneScript = CutsceneScriptHolder.LoadAllScripts();
            }

            ActiveCutscene.Load();
            panDrawingSurface_Resize(sender, e);
            for (int S = 0; S < ActiveCutscene.DicActionScript.Count; S++)
                InitScript(ActiveCutscene.DicActionScript[S]);

            for (int D = 0; D < ActiveCutscene.ListDataContainer.Count; D++)
                InitScript(ActiveCutscene.ListDataContainer[D]);
        }

        private void sclScriptWidth_Scroll(object sender, ScrollEventArgs e)
        {
            ScriptStartingPos.X = e.NewValue;
            DrawScripts();
        }

        private void sclScriptHeight_Scroll(object sender, ScrollEventArgs e)
        {
            ScriptStartingPos.Y = e.NewValue;
            DrawScripts();
        }
    }
}
