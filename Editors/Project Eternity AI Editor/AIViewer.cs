using System;
using System.Drawing;
using System.Windows.Forms;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.Editors.AIEditor
{
    public partial class AIViewer : UserControl
    {
        private enum ScriptLinkTypes { None, FromFollowingScriptToEvaluator, FromEvaluatorToFollowingScript, FromReferenceToGetContent, FromGetContentToReference };
        private enum ScriptBoxTypes { Evaluator, FollowingScript, GetContent, Reference };

        public delegate void SelectScriptDelegate(AIScript SelectedScript);

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
        public AIContainer AI;
        public string AIPath;
        public bool ShowExecutionOrder;

        private BufferedGraphics pbMapPreviewGraphicDevice;
        private Graphics pbDrawingSurfaceGraphics;
        private Point ScriptStartingPos;//Point from which to start drawing the scripts

        private Point MousePosOld;

        private int ActiveScriptIndex;

        private AIScript ScriptLink;
        private int ScriptLinkIndex;
        private ScriptLinkTypes ScriptLinkType;
        private int ScriptLinkEventIndex;
        private Point ScriptLinkStartPos;
        private Point ScriptLinkEndPos;

        public SelectScriptDelegate SelectScript;
        private StringFormat StringFormatCenter = new StringFormat();
        private StringFormat StringFormatRight = new StringFormat();

        public AIViewer()
        {
            InitializeComponent();
        }

        public void Init(SelectScriptDelegate SelectScript)
        {
            this.SelectScript = SelectScript;
            ActiveScriptIndex = -1;
            ShowExecutionOrder = true;

            StringFormatCenter.Alignment = StringAlignment.Center;
            StringFormatCenter.LineAlignment = StringAlignment.Center;

            StringFormatRight.Alignment = StringAlignment.Far;
            StringFormatRight.LineAlignment = StringAlignment.Center;

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

            AI.Load(AIPath);
            panDrawingSurface_Resize(this, null);
            for (int S = 0; S < AI.ListScript.Count; S++)
                InitScript(AI.ListScript[S]);
        }

        #region Scripting

        public void InitScript(BasicScript NewScript)
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

        public void DeleteScript()
        {
            if (ActiveScriptIndex < 0)
                return;

            AIScript ActiveScript = AI.ListScript[ActiveScriptIndex];

            //Following Scripts.
            for (int F = ActiveScript.ArrayFollowingScript.Length - 1; F >= 0; --F)
            {
                ActiveScript.ArrayFollowingScript[F].ListScript.Clear();
            }

            //References.
            for (int R = ActiveScript.ArrayReferences.Length - 1; R >= 0; --R)
            {
                ActiveScript.ArrayReferences[R].ReferencedScript = null;
            }

            //Evaluator.
            foreach (AIScript OtherScript in AI.ListScript)
            {
                if (OtherScript == ActiveScript)
                    continue;

                //Following Scripts.
                for (int F = OtherScript.ArrayFollowingScript.Length - 1; F >= 0; --F)
                {
                    for (int S = OtherScript.ArrayFollowingScript[F].ListScript.Count - 1; S >= 0; --S)
                    {
                        if (OtherScript.ArrayFollowingScript[F].ListScript[S] == ActiveScript)
                        {
                            OtherScript.ArrayFollowingScript[F].ListScript.RemoveAt(S);
                        }
                    }
                }
            }

            //GetContent.
            foreach (AIScript OtherScript in AI.ListScript)
            {
                if (OtherScript == ActiveScript)
                    continue;

                //Following Scripts.
                for (int R = OtherScript.ArrayReferences.Length - 1; R >= 0; --R)
                {
                    if (OtherScript.ArrayReferences[R].ReferencedScript == ActiveScript)
                    {
                        OtherScript.ArrayReferences[R].ReferencedScript = null;
                    }
                }
            }

            AI.ListScript.RemoveAt(ActiveScriptIndex);
            ActiveScriptIndex = -1;
            DrawScripts();
        }

        private void StartLinkingScript(int ActiveScriptIndex, Rectangle MouseRec)
        {
            AIScript ActiveScript = AI.ListScript[ActiveScriptIndex];
            
            //Following Scripts.
            for (int T = ActiveScript.ArrayFollowingScript.Length - 1; T >= 0; --T)
            {
                Rectangle LinkBoxPosition = GetLinkBoxPosition(ScriptBoxTypes.FollowingScript, ActiveScript, T);

                if (MouseRec.IntersectsWith(LinkBoxPosition))
                {
                    ScriptLink = ActiveScript;
                    ScriptLinkIndex = ActiveScriptIndex;
                    ScriptLinkType = ScriptLinkTypes.FromFollowingScriptToEvaluator;
                    ScriptLinkEventIndex = T;
                    ScriptLinkStartPos = new Point(LinkBoxPosition.X + 2, LinkBoxPosition.Y + 2);
                    return;
                }
            }

            //References.
            for (int T = ActiveScript.ArrayReferences.Length - 1; T >= 0; --T)
            {
                Rectangle LinkBoxPosition = GetLinkBoxPosition(ScriptBoxTypes.Reference, ActiveScript, T);

                if (MouseRec.IntersectsWith(LinkBoxPosition))
                {
                    ScriptLink = ActiveScript;
                    ScriptLinkIndex = ActiveScriptIndex;
                    ScriptLinkType = ScriptLinkTypes.FromReferenceToGetContent;
                    ScriptLinkEventIndex = T;
                    ScriptLinkStartPos = new Point(LinkBoxPosition.X + 2, LinkBoxPosition.Y + 2);
                    return;
                }
            }

            //Evaluator.
            Rectangle EvaluatorLinkBoxPosition = GetLinkBoxPosition(ScriptBoxTypes.Evaluator, ActiveScript, 0);

            if (MouseRec.IntersectsWith(EvaluatorLinkBoxPosition)
                 && ActiveScript is ScriptEvaluator)
            {
                ScriptLink = ActiveScript;
                ScriptLinkIndex = ActiveScriptIndex;
                ScriptLinkType = ScriptLinkTypes.FromEvaluatorToFollowingScript;
                ScriptLinkEventIndex = -1;
                ScriptLinkStartPos = new Point(EvaluatorLinkBoxPosition.X + 2, EvaluatorLinkBoxPosition.Y + 2);
                return;
            }

            //GetContent.
            Rectangle GetContentLinkBoxPosition = GetLinkBoxPosition(ScriptBoxTypes.GetContent, ActiveScript, 0);

            if (MouseRec.IntersectsWith(GetContentLinkBoxPosition)
                 && ActiveScript is ScriptReference)
            {
                ScriptLink = ActiveScript;
                ScriptLinkIndex = ActiveScriptIndex;
                ScriptLinkType = ScriptLinkTypes.FromGetContentToReference;
                ScriptLinkEventIndex = -1;
                ScriptLinkStartPos = new Point(GetContentLinkBoxPosition.X + 2, GetContentLinkBoxPosition.Y + 2);
                return;
            }
        }

        private void FinishLinkingScript(int ActiveScriptIndex, Rectangle MouseRec)
        {
            AIScript ActiveScript = AI.ListScript[ActiveScriptIndex];
            int ScriptX = ActiveScript.ScriptSize.X - ScriptStartingPos.X;
            int ScriptY = ActiveScript.ScriptSize.Y - ScriptStartingPos.Y;

            switch (ScriptLinkType)
            {
                #region FromEvaluatorToFollowingScript

                case ScriptLinkTypes.FromEvaluatorToFollowingScript:
                    var ScriptLinkEvaluator = ScriptLink as ScriptEvaluator;
                    if (ScriptLinkEvaluator == null)
                        return;

                    for (int T = ActiveScript.ArrayFollowingScript.Length - 1; T >= 0; --T)
                    {
                        Rectangle FollowingScriptPosition = GetLinkBoxPosition(ScriptBoxTypes.FollowingScript, ActiveScript, T);

                        if (MouseRec.IntersectsWith(FollowingScriptPosition))
                        {
                            ActiveScript.ArrayFollowingScript[T].ListScript.Add(ScriptLinkEvaluator);

                            ScriptLink = null;
                            ScriptLinkIndex = -1;
                            ScriptLinkEventIndex = -1;
                            ScriptLinkType = ScriptLinkTypes.None;
                            DrawScripts();
                            return;
                        }
                    }
                    break;

                #endregion

                #region FromFollowingScriptToEvaluator

                case ScriptLinkTypes.FromFollowingScriptToEvaluator:
                    var ActiveScriptEvaluator = ActiveScript as ScriptEvaluator;
                    if (ActiveScriptEvaluator == null)
                        return;

                    Rectangle EvaluatorPosition = GetLinkBoxPosition(ScriptBoxTypes.Evaluator, ActiveScript, 0);

                    if (MouseRec.IntersectsWith(EvaluatorPosition)
                        && !AI.ListScript[ScriptLinkIndex].ArrayFollowingScript[ScriptLinkEventIndex].ListScript.Contains(ActiveScriptEvaluator))
                    {
                        AI.ListScript[ScriptLinkIndex].ArrayFollowingScript[ScriptLinkEventIndex].ListScript.Add(ActiveScriptEvaluator);

                        ScriptLink = null;
                        ScriptLinkIndex = -1;
                        ScriptLinkEventIndex = -1;
                        ScriptLinkType = ScriptLinkTypes.None;
                        DrawScripts();
                        return;
                    }
                    break;

                #endregion

                #region FromReferenceToGetContent

                case ScriptLinkTypes.FromReferenceToGetContent:
                    var ActiveScriptReference = ActiveScript as ScriptReference;
                    if (ActiveScriptReference == null)
                        return;

                    Rectangle ReferencePosition = GetLinkBoxPosition(ScriptBoxTypes.GetContent, ActiveScript, 0);

                    if (MouseRec.IntersectsWith(ReferencePosition)
                        && AI.ListScript[ScriptLinkIndex].ArrayReferences[ScriptLinkEventIndex].ReferencedScript != ActiveScriptReference)
                    {
                        AI.ListScript[ScriptLinkIndex].ArrayReferences[ScriptLinkEventIndex].ReferencedScript = ActiveScriptReference;

                        ScriptLink = null;
                        ScriptLinkIndex = -1;
                        ScriptLinkEventIndex = -1;
                        ScriptLinkType = ScriptLinkTypes.None;
                        DrawScripts();
                        return;
                    }
                    break;

                #endregion

                #region FromGetContentToReference

                case ScriptLinkTypes.FromGetContentToReference:
                    var ScriptLinkGetContent = ScriptLink as ScriptReference;
                    if (ScriptLinkGetContent == null)
                        return;
                    
                    for (int T = ActiveScript.ArrayReferences.Length - 1; T >= 0; --T)
                    {
                        Rectangle FollowingScriptPosition = GetLinkBoxPosition(ScriptBoxTypes.Reference, ActiveScript, T);

                        if (MouseRec.IntersectsWith(FollowingScriptPosition))
                        {
                            ActiveScript.ArrayReferences[T].ReferencedScriptIndex = ScriptLinkIndex;

                            ScriptLink = null;
                            ScriptLinkIndex = -1;
                            ScriptLinkEventIndex = -1;
                            ScriptLinkType = ScriptLinkTypes.None;
                            DrawScripts();
                            return;
                        }
                    }
                    break;

                    #endregion
            }
        }

        private void UnlinkScript(int ActiveScriptIndex, Rectangle MouseRec)
        {
            AIScript ActiveScript = AI.ListScript[ActiveScriptIndex];
            int ScriptX = ActiveScript.ScriptSize.X - ScriptStartingPos.X;
            int ScriptY = ActiveScript.ScriptSize.Y - ScriptStartingPos.Y;

            //Following Scripts.
            for (int F = ActiveScript.ArrayFollowingScript.Length - 1; F >= 0; --F)
            {
                Rectangle LinkBoxPosition = GetLinkBoxPosition(ScriptBoxTypes.FollowingScript, ActiveScript, F);

                if (MouseRec.IntersectsWith(LinkBoxPosition))
                {
                    ActiveScript.ArrayFollowingScript[F].ListScript.Clear();
                    ActiveScript.ArrayFollowingScript[F].ListScriptIndex.Clear();
                }
            }

            //References.
            for (int R = ActiveScript.ArrayReferences.Length - 1; R >= 0; --R)
            {
                Rectangle LinkBoxPosition = GetLinkBoxPosition(ScriptBoxTypes.Reference, ActiveScript, R);

                if (MouseRec.IntersectsWith(LinkBoxPosition))
                {
                    ActiveScript.ArrayReferences[R].ReferencedScript = null;
                }
            }

            //Evaluator.
            Rectangle EvaluatorLinkBoxPosition = GetLinkBoxPosition(ScriptBoxTypes.Evaluator, ActiveScript, 0);

            if (MouseRec.IntersectsWith(EvaluatorLinkBoxPosition)
                 && ActiveScript is ScriptEvaluator)
            {
                foreach (AIScript OtherScript in AI.ListScript)
                {
                    if (OtherScript == ActiveScript)
                        continue;

                    //Following Scripts.
                    for (int F = OtherScript.ArrayFollowingScript.Length - 1; F >= 0; --F)
                    {
                        for (int S = OtherScript.ArrayFollowingScript[F].ListScript.Count - 1; S >= 0; --S)
                        {
                            if (OtherScript.ArrayFollowingScript[F].ListScript[S] == ActiveScript)
                            {
                                OtherScript.ArrayFollowingScript[F].ListScript.RemoveAt(S);
                            }
                        }
                    }
                }
            }

            //GetContent.
            Rectangle GetContentLinkBoxPosition = GetLinkBoxPosition(ScriptBoxTypes.GetContent, ActiveScript, 0);

            if (MouseRec.IntersectsWith(GetContentLinkBoxPosition)
                 && ActiveScript is ScriptReference)
            {
                foreach (AIScript OtherScript in AI.ListScript)
                {
                    if (OtherScript == ActiveScript)
                        continue;

                    //Following Scripts.
                    for (int R = OtherScript.ArrayReferences.Length - 1; R >= 0; --R)
                    {
                        if (OtherScript.ArrayReferences[R].ReferencedScript == ActiveScript)
                        {
                            OtherScript.ArrayReferences[R].ReferencedScript = null;
                        }
                    }
                }
            }
        }

        private Rectangle GetLinkBoxPosition(ScriptBoxTypes ScriptBoxType, AIScript ActiveScript, int Index)
        {
            int ScriptX = ActiveScript.ScriptSize.X;
            int ScriptY = ActiveScript.ScriptSize.Y;

            switch (ScriptBoxType)
            {
                case ScriptBoxTypes.FollowingScript:
                    return new Rectangle(
                        ScriptX + ActiveScript.ScriptSize.Width + 4,
                        ScriptY + ActiveScript.ScriptSize.Height - 11 - Index * 12,
                        6,
                        6);

                case ScriptBoxTypes.Reference:
                    int SpacesBetweenReferences = ActiveScript.ScriptSize.Width / (ActiveScript.ArrayReferences.Length + 1);

                    int DistanceFromCenter = SpacesBetweenReferences * (Index + 1) - ActiveScript.ScriptSize.Width / 2;

                    return new Rectangle(
                        ScriptX - 1 + SpacesBetweenReferences * (Index + 1) + DistanceFromCenter / 5,
                        ScriptY - 11,
                        6,
                        6);

                case ScriptBoxTypes.Evaluator:
                    return new Rectangle(ScriptX - 11, ScriptY + 14, 6, 6);

                default:
                case ScriptBoxTypes.GetContent:
                    return new Rectangle(ScriptX - 1 + ActiveScript.ScriptSize.Width / 2, ScriptY + ActiveScript.ScriptSize.Height + 4, 6, 6);
            }
        }

        public void DrawScripts()
        {
            if (pbMapPreviewGraphicDevice == null)
                return;

            pbMapPreviewGraphicDevice.Graphics.Clear(Color.White);

            for (int S = AI.ListScript.Count - 1; S >= 0; --S)
            {
                AIScript ActiveScript = AI.ListScript[S];
                int X = ActiveScript.ScriptSize.X - ScriptStartingPos.X;
                int Y = ActiveScript.ScriptSize.Y - ScriptStartingPos.Y;

                pbMapPreviewGraphicDevice.Graphics.DrawImage(ActiveScript.ScriptImage, X, Y);
                pbMapPreviewGraphicDevice.Graphics.DrawString(ActiveScript.ToString(), fntScriptName, brScriptBrush, X + 2, Y + 12);

                pbMapPreviewGraphicDevice.Graphics.DrawString(
                    ActiveScript.Comment,
                    fntScriptName,
                    brScriptBrush,
                    X + ActiveScript.ScriptSize.Width - 5,
                    Y + ActiveScript.ScriptSize.Height - 7,
                    StringFormatRight);

                if (S == ActiveScriptIndex)
                    pbMapPreviewGraphicDevice.Graphics.DrawRectangle(Pens.Black, X, Y, ActiveScript.ScriptSize.Width, ActiveScript.ScriptSize.Height);

                for (int T = ActiveScript.ArrayFollowingScript.Length - 1; T >= 0; --T)
                {
                    pbMapPreviewGraphicDevice.Graphics.DrawString(
                        ActiveScript.ArrayFollowingScript[T].ReferenceName,
                        fntScriptName,
                        brScriptBrush,
                        X + ActiveScript.ScriptSize.Width - 5,
                        Y + ActiveScript.ScriptSize.Height - 7 - T * 12,
                        StringFormatRight);


                    pbMapPreviewGraphicDevice.Graphics.FillRectangle(
                        brScriptLink,
                        X + ActiveScript.ScriptSize.Width + 5,
                        Y + ActiveScript.ScriptSize.Height - 10 - T * 12,
                        5,
                        5);
                                        
                    //Display links
                    for (int i = 0; i < ActiveScript.ArrayFollowingScript[T].ListScript.Count; i++)
                    {
                        var OtherScript = ActiveScript.ArrayFollowingScript[T].ListScript[i] as AIScript;

                        int StartX = X + ActiveScript.ScriptSize.Width + 7;
                        int StartY = Y + ActiveScript.ScriptSize.Height - 8 - T * 12;
                        int OtherX = OtherScript.ScriptSize.X - ScriptStartingPos.X - 8;
                        int OtherY = OtherScript.ScriptSize.Y - ScriptStartingPos.Y + 17;

                        pbMapPreviewGraphicDevice.Graphics.DrawLine(Pens.Black,
                                        StartX,
                                        StartY,
                                        OtherX,
                                        OtherY);

                        if (ShowExecutionOrder && ActiveScript.ArrayFollowingScript[T].ListScript.Count > 1)
                        {
                            Point Center = new Point((StartX + OtherX) / 2, (StartY + OtherY) / 2);
                            Point Angle = new Point(OtherX - StartX, OtherY - StartY);

                            float distance = (float)Math.Sqrt(Angle.X * Angle.X + Angle.Y * Angle.Y);
                            PointF PerpendicularAxis = new PointF(-(Angle.Y / distance), Angle.X / distance);

                            pbMapPreviewGraphicDevice.Graphics.DrawString(
                                (i + 1).ToString(),
                                fntScriptName,
                                brScriptLink,
                                Center.X - PerpendicularAxis.X * 10,
                                Center.Y - PerpendicularAxis.Y * 10,
                                StringFormatCenter);

                        }
                    }
                }

                int SpacesBetweenReferences = ActiveScript.ScriptSize.Width / (ActiveScript.ArrayReferences.Length + 1);

                for (int T = ActiveScript.ArrayReferences.Length - 1; T >= 0; --T)
                {
                    int DistanceFromCenter = SpacesBetweenReferences * (T + 1) - ActiveScript.ScriptSize.Width / 2;

                    pbMapPreviewGraphicDevice.Graphics.DrawString(
                        ActiveScript.ArrayReferences[T].ReferenceName,
                        fntScriptName,
                        brScriptBrush,
                        X + SpacesBetweenReferences * (T + 1) + DistanceFromCenter / 5,
                        Y + 8,
                        StringFormatCenter);

                    pbMapPreviewGraphicDevice.Graphics.FillRectangle(
                        brScriptLink,
                        X + SpacesBetweenReferences * (T + 1) + DistanceFromCenter / 5,
                        Y - 10,
                        5,
                        5);

                    if (ActiveScript.ArrayReferences[T].ReferencedScript != null)
                    {
                        var OtherScript = ActiveScript.ArrayReferences[T].ReferencedScript as AIScript;
                        int OtherX = OtherScript.ScriptSize.X - ScriptStartingPos.X;
                        int OtherY = OtherScript.ScriptSize.Y - ScriptStartingPos.Y;

                        pbMapPreviewGraphicDevice.Graphics.DrawLine(Pens.Black,
                            X + SpacesBetweenReferences * (T + 1) + DistanceFromCenter / 5 + 2,
                            Y - 8,
                            OtherX + OtherScript.ScriptSize.Width / 2 + 2,
                            OtherY + OtherScript.ScriptSize.Height + 7);
                    }
                }

                if (AI.ListScript[S] is ScriptEvaluator)
                {
                    pbMapPreviewGraphicDevice.Graphics.FillRectangle(brScriptLink, X - 10, Y + 15, 5, 5);
                }

                if (AI.ListScript[S] is ScriptReference)
                {
                    pbMapPreviewGraphicDevice.Graphics.FillRectangle(brScriptLink, X + ActiveScript.ScriptSize.Width / 2, Y + ActiveScript.ScriptSize.Height + 5, 5, 5);
                }
            }

            if (ScriptLink != null)
            {
                pbMapPreviewGraphicDevice.Graphics.DrawLine(Pens.Black, ScriptLinkStartPos.X - ScriptStartingPos.X, ScriptLinkStartPos.Y - ScriptStartingPos.Y, ScriptLinkEndPos.X, ScriptLinkEndPos.Y);
            }

            pbMapPreviewGraphicDevice.Render();
        }
        
        #endregion

        private void panDrawingSurface_Paint(object sender, PaintEventArgs e)
        {
            DrawScripts();
        }

        private void panDrawingSurface_MouseDown(object sender, MouseEventArgs e)
        {
            Rectangle MouseRec = new Rectangle(e.X + ScriptStartingPos.X, e.Y + ScriptStartingPos.Y, 1, 1);
            MousePosOld.X = MouseRec.X;
            MousePosOld.Y = MouseRec.Y;

            for (int S = AI.ListScript.Count - 1; S >= 0; --S)
            {
                if (MouseRec.IntersectsWith(AI.ListScript[S].ScriptSize))
                {
                    SelectScript(AI.ListScript[S]);
                    ActiveScriptIndex = S;
                    return;
                }
                else if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                {
                    StartLinkingScript(S, MouseRec);
                }
            }
        }

        private void panDrawingSurface_MouseUp(object sender, MouseEventArgs e)
        {
            Rectangle MouseRec = new Rectangle(e.X + ScriptStartingPos.X, e.Y + ScriptStartingPos.Y, 1, 1);

            #region Action Scripts

            for (int S = AI.ListScript.Count - 1; S >= 0; --S)
            {
                if (ScriptLink == AI.ListScript[S])
                {
                    continue;
                }

                AIScript ActiveScript = AI.ListScript[S];
                int X = ActiveScript.ScriptSize.X - ScriptStartingPos.X;
                int Y = ActiveScript.ScriptSize.Y - ScriptStartingPos.Y;

                if (MouseRec.IntersectsWith(AI.ListScript[S].ScriptSize))
                {
                    if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                    {
                        ActiveScriptIndex = -1;
                        return;
                    }
                    else if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
                    {
                        cmsScriptMenu.Show(this, PointToClient(Cursor.Position));
                        return;
                    }
                }
                else if ((e.Button & MouseButtons.Left) == MouseButtons.Left && ScriptLink != null)
                {
                    FinishLinkingScript(S, MouseRec);
                }
                else if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
                {
                    UnlinkScript(S, MouseRec);
                }
            }

            #endregion

            ScriptLink = null;
            ScriptLinkIndex = -1;
            ScriptLinkEventIndex = -1;
            ScriptLinkType = ScriptLinkTypes.None;
            ActiveScriptIndex = -1;
            DrawScripts();
        }

        private void panDrawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle MouseRec = new Rectangle(e.X + ScriptStartingPos.X, e.Y + ScriptStartingPos.Y, 1, 1);
            if (ActiveScriptIndex != -1)
            {
                AI.ListScript[ActiveScriptIndex].ScriptSize.X += MouseRec.X - MousePosOld.X;
                AI.ListScript[ActiveScriptIndex].ScriptSize.Y += MouseRec.Y - MousePosOld.Y;

                int MaxX = 0, MaxY = 0;
                for (int S = AI.ListScript.Count - 1; S >= 0; --S)
                {
                    if (AI.ListScript[S].ScriptSize.Right > MaxX)
                        MaxX = AI.ListScript[S].ScriptSize.Right + 30;

                    if (AI.ListScript[S].ScriptSize.Bottom > MaxY)
                        MaxY = AI.ListScript[S].ScriptSize.Bottom + 30;
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
            for (int S = AI.ListScript.Count - 1; S >= 0; --S)
            {
                if (AI.ListScript[S].ScriptSize.Right > MaxX)
                    MaxX = AI.ListScript[S].ScriptSize.Right;

                if (AI.ListScript[S].ScriptSize.Bottom > MaxY)
                    MaxY = AI.ListScript[S].ScriptSize.Bottom;
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
