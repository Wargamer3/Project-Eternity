using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class PolygonCutterTimeline : CoreTimeline
    {
        private const string TimelineType = "Polygon";

        public enum PolygonCutterTypes { Move, Crop }

        public enum PolygonCutterSpecialEffects { None, Colorise, Blur }

        public class PolygonCutterKeyFrame : VisibleAnimationObjectKeyFrame
        {
            public List<Polygon> ListPolygon;
            public readonly AnimationClass.AnimationLayer ActiveLayer;

            private PolygonCutterKeyFrame(AnimationClass.AnimationLayer ActiveLayer)
            {
                this.ActiveLayer = ActiveLayer;
                ListPolygon = new List<Polygon>();
            }

            public PolygonCutterKeyFrame(AnimationClass.AnimationLayer ActiveLayer, Vector2 NextPosition, bool IsProgressive, int NextKeyFrame)
                : base(NextPosition, IsProgressive, NextKeyFrame)
            {
                this.ActiveLayer = ActiveLayer;
                ListPolygon = new List<Polygon>();
            }

            public PolygonCutterKeyFrame(BinaryReader BR, AnimationClass.AnimationLayer ActiveLayer)
                : base(BR)
            {
                this.ActiveLayer = ActiveLayer;

                int ListPolygonCount = BR.ReadInt32();
                ListPolygon = new List<Polygon>();
                for (int P = 0; P < ListPolygonCount; P++)
                {
                    int ArrayKeyFrameVertexLength = BR.ReadInt32();
                    Polygon NewPolygon = new Polygon();
                    NewPolygon.ArrayVertex = new Vector2[ArrayKeyFrameVertexLength];
                    NewPolygon.ArrayColorPoints = new Color[ArrayKeyFrameVertexLength];
                    for (int V = 0; V < ArrayKeyFrameVertexLength; V++)
                    {
                        NewPolygon.ArrayVertex[V] = new Vector2(BR.ReadSingle(), BR.ReadSingle());
                    }
                    int ArrayIndexLength = BR.ReadInt32();
                    NewPolygon.ArrayIndex = new short[ArrayIndexLength];
                    for (int I = 0; I < ArrayIndexLength; I++)
                    {
                        NewPolygon.ArrayIndex[I] = BR.ReadInt16();
                    }
                    int ArrayUVCoordinatesLength = BR.ReadInt32();
                    NewPolygon.ArrayUVCoordinates = new Vector2[ArrayUVCoordinatesLength];
                    for (int UV = 0; UV < ArrayUVCoordinatesLength; UV++)
                    {
                        NewPolygon.ArrayUVCoordinates[UV] = new Vector2(BR.ReadSingle(), BR.ReadSingle());
                    }

                    NewPolygon.ComputePerpendicularAxis();
                    NewPolygon.UpdateWorldPosition(Vector2.Zero, 0);
                    NewPolygon.TriangleCount = NewPolygon.ArrayIndex.Length / 3;

                    ListPolygon.Add(NewPolygon);
                }
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(ListPolygon.Count);
                for (int P = 0; P < ListPolygon.Count; P++)
                {
                    BW.Write(ListPolygon[P].ArrayVertex.Length);
                    for (int V = 0; V < ListPolygon[P].ArrayVertex.Length; V++)
                    {
                        BW.Write(ListPolygon[P].ArrayVertex[V].X);
                        BW.Write(ListPolygon[P].ArrayVertex[V].Y);
                    }

                    BW.Write(ListPolygon[P].ArrayIndex.Length);
                    for (int I = 0; I < ListPolygon[P].ArrayIndex.Length; I++)
                    {
                        BW.Write(ListPolygon[P].ArrayIndex[I]);
                    }

                    BW.Write(ListPolygon[P].ArrayUVCoordinates.Length);
                    for (int UV = 0; UV < ListPolygon[P].ArrayUVCoordinates.Length; UV++)
                    {
                        BW.Write(ListPolygon[P].ArrayUVCoordinates[UV].X);
                        BW.Write(ListPolygon[P].ArrayUVCoordinates[UV].Y);
                    }
                }
            }

            protected override VisibleAnimationObjectKeyFrame CopyAsVisibleAnimationObjectKeyFrame(AnimationClass.AnimationLayer ActiveLayer)
            {
                PolygonCutterKeyFrame NewPolygonCutterKeyFrame = new PolygonCutterKeyFrame(ActiveLayer);

                NewPolygonCutterKeyFrame.UpdateFrom(this);

                NewPolygonCutterKeyFrame.ListPolygon = new List<Polygon>(ListPolygon.Count);
                foreach (Polygon ActivePolygon in ListPolygon)
                {
                    NewPolygonCutterKeyFrame.ListPolygon.Add(new Polygon(ActivePolygon));
                }

                return NewPolygonCutterKeyFrame;
            }

            [Editor(typeof(PolygonCutterSelector), typeof(UITypeEditor)),
            CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute(".")]
            public List<Polygon> Polygons
            {
                get
                {
                    return ListPolygon;
                }
                set
                {
                    ListPolygon = value;
                }
            }
        }

        public List<Polygon> ListPolygon;
        private PolygonCutterTypes _PolygonCutterType;
        private PolygonCutterSpecialEffects _PolygonCutterSpecialEffect;
        public Vector2[][] ArrayMoveValue;
        private PolygonCutterKeyFrame EventOld = null;
        public Rectangle SourceRectangle;
        private AnimationClass.AnimationLayer ActiveLayer;

        public PolygonCutterTimeline()
            : base(TimelineType, "New Polygon Cutter")
        {
        }

        public PolygonCutterTimeline(string Name, Vector2 Position, AnimationClass.AnimationLayer ActiveLayer)
            : this()
        {
            _PolygonCutterType = PolygonCutterTypes.Move;

            this.Name = Name;

            this.Position = Position;
            this.ActiveLayer = ActiveLayer;
            ListPolygon = new List<Polygon>();
        }

        public PolygonCutterTimeline(BinaryReader BR, AnimationClass.AnimationLayer ActiveLayer)
            : base(BR, TimelineType)
        {
            this.ActiveLayer = ActiveLayer;
            _PolygonCutterType = (PolygonCutterTypes)BR.ReadByte();
            _PolygonCutterSpecialEffect = (PolygonCutterSpecialEffects)BR.ReadByte();

            ListPolygon = new List<Polygon>();

            float MinX = float.MaxValue;
            float MaxX = float.MinValue;
            float MinY = float.MaxValue;
            float MaxY = float.MinValue;

            foreach (Polygon ActivePolygon in ListPolygon)
            {
                for (int V = 0; V < ActivePolygon.VertexCount; V++)
                {
                    int VertexX = (int)ActivePolygon.ArrayVertex[V].X;
                    int VertexY = (int)ActivePolygon.ArrayVertex[V].Y;

                    if (VertexX < MinX)
                        MinX = VertexX;
                    if (VertexX > MaxX)
                        MaxX = VertexX;

                    if (VertexY < MinY)
                        MinY = VertexY;
                    if (VertexY > MaxY)
                        MaxY = VertexY;
                }
            }

            SourceRectangle = new Rectangle((int)MinX, (int)MinY, (int)(MaxX - MinX), (int)(MaxY - MinY));
            Origin = new Point(Width / 2, Height / 2);

            int DicAnimationSpriteKeyFrameCount = BR.ReadInt32();
            for (int E = 0; E < DicAnimationSpriteKeyFrameCount; E++)
            {
                int Key = BR.ReadInt32();

                PolygonCutterKeyFrame NewPolygonCutterKeyFrame = new PolygonCutterKeyFrame(BR, ActiveLayer);

                DicAnimationKeyFrame.Add(Key, NewPolygonCutterKeyFrame);
            }

            int FirstKey = int.MaxValue;
            foreach (int ActiveKey in DicAnimationKeyFrame.Keys)
            {
                if (FirstKey > ActiveKey)
                    FirstKey = ActiveKey;
            }
            PolygonCutterKeyFrame FirstKeyFrame = (PolygonCutterKeyFrame)DicAnimationKeyFrame[FirstKey];
            for (int P = 0; P < FirstKeyFrame.ListPolygon.Count; P++)
            {
                Polygon ActivePolygon = new Polygon(FirstKeyFrame.ListPolygon[P]);
                ListPolygon.Add(ActivePolygon);
            }
        }

        protected override VisibleTimeline DoLoadCopy(BinaryReader BR, Microsoft.Xna.Framework.Content.ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            return new PolygonCutterTimeline(BR, ActiveLayer);
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_PolygonCutterType);
            BW.Write((byte)_PolygonCutterSpecialEffect);

            BW.Write(DicAnimationKeyFrame.Count);
            foreach (KeyValuePair<int, VisibleAnimationObjectKeyFrame> KeyFrame in DicAnimationKeyFrame)
            {
                BW.Write(KeyFrame.Key);
                KeyFrame.Value.Save(BW);
            }
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            PolygonCutterTimeline NewPolygonCutterTimeline = new PolygonCutterTimeline();

            NewPolygonCutterTimeline.SourceRectangle = SourceRectangle;

            NewPolygonCutterTimeline.ListPolygon = new List<Polygon>(ListPolygon.Count);
            foreach (Polygon ActivePolygon in ListPolygon)
            {
                NewPolygonCutterTimeline.ListPolygon.Add(new Polygon(ActivePolygon));
            }

            NewPolygonCutterTimeline._PolygonCutterType = _PolygonCutterType;
            NewPolygonCutterTimeline._PolygonCutterSpecialEffect = _PolygonCutterSpecialEffect;
            NewPolygonCutterTimeline.ArrayMoveValue = ArrayMoveValue;

            NewPolygonCutterTimeline.UpdateFrom(this, ActiveLayer);

            NewPolygonCutterTimeline.EventOld = (PolygonCutterKeyFrame)NewPolygonCutterTimeline.DicAnimationKeyFrame[0];

            return NewPolygonCutterTimeline;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            Vector2[] LocalPoints = new Vector2[4]
            {
                new Vector2(MousePosition.X - 40, MousePosition.Y - 40),
                new Vector2(MousePosition.X - 40, MousePosition.Y + 40),
                new Vector2(MousePosition.X + 40, MousePosition.Y + 40),
                new Vector2(MousePosition.X + 40, MousePosition.Y - 40),
            };

            PolygonCutterTimeline NewCreatePolygonCutterEvent = new PolygonCutterTimeline("New Polygon Cutter", Vector2.Zero, ActiveLayer);
            PolygonCutterHelper NewSpawner = new PolygonCutterHelper(ActiveLayer.renderTarget, NewCreatePolygonCutterEvent.ListPolygon, true);

            NewSpawner.PolygonCutterViewer.ListPolygon.Add(new Polygon(LocalPoints, GameScreen.GraphicsDevice.Viewport.Width, GameScreen.GraphicsDevice.Viewport.Height));

            if (NewSpawner.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                NewCreatePolygonCutterEvent.SpawnFrame = KeyFrame;
                NewCreatePolygonCutterEvent.DeathFrame = KeyFrame + 10;
                NewCreatePolygonCutterEvent.IsUsed = true;//Disable the spawner as we spawn the timeline manually.

                PolygonCutterKeyFrame NewPolygonCutterKeyFrame = new PolygonCutterKeyFrame(ActiveLayer, NewCreatePolygonCutterEvent.Position,
                                                                                true, -1);

                foreach (Polygon NewPolygon in NewSpawner.PolygonCutterViewer.ListPolygon)
                {
                    NewPolygonCutterKeyFrame.ListPolygon.Add(new Polygon(NewPolygon));
                }

                NewCreatePolygonCutterEvent.ListPolygon.AddRange(NewSpawner.PolygonCutterViewer.ListPolygon);

                NewCreatePolygonCutterEvent.Add(KeyFrame, NewPolygonCutterKeyFrame);

                ReturnValue.Add(NewCreatePolygonCutterEvent);
            }

            return ReturnValue;
        }

        public override void SpawnItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame)
        {
            ActiveAnimation.OnPolygonCutterTimelineSpawn(ActiveLayer, this);
        }

        public override void OnDeathFrame(AnimationClass ActiveAnimation)
        {
            ActiveAnimation.OnPolygonCutterTimelineDeath(this);
        }

        public override void UpdateAnimationObject(int KeyFrame)
        {
            //An Event is being executed.
            if (NextEvent != null && ArrayMoveValue != null)
            {
                UpdateAnimationSprite(KeyFrame);

                for (int P = 0; P < ListPolygon.Count; P++)
                {
                    for (int V = 0; V < ListPolygon[P].ArrayVertex.Length; V++)
                    {
                        Vector2 Result = ArrayMoveValue[P][V] * (KeyFrame - EventKeyFrameOld);
                        ListPolygon[P].ArrayVertex[V] = new Vector2(EventOld.ListPolygon[P].ArrayVertex[V].X + (int)Result.X, EventOld.ListPolygon[P].ArrayVertex[V].Y + (int)Result.Y);
                    }

                    if (PolygonCutterType == PolygonCutterTypes.Crop)
                    {
                        ListPolygon[P].ComputeColorAndUVCoordinates(ActiveLayer.Owner.ScreenWidth, ActiveLayer.Owner.ScreenHeight);
                    }
                    ListPolygon[P].ComputePerpendicularAxis();
                    ListPolygon[P].UpdateWorldPosition(Position, Angle);
                }
            }

            VisibleAnimationObjectKeyFrame ActiveKeyFrame;

            if (DicAnimationKeyFrame.TryGetValue(KeyFrame, out ActiveKeyFrame))
            {
                //If that animation has already been used, skip it.
                if (ActiveKeyFrame.IsUsed)
                    return;

                int NextKeyFrame = ActiveKeyFrame.NextKeyFrame;

                OnNewKeyFrameAnimationSprite(ActiveKeyFrame);

                PolygonCutterKeyFrame ActivePolygonCutterKeyFrame = (PolygonCutterKeyFrame)ActiveKeyFrame;
                EventOld = ActivePolygonCutterKeyFrame;

                if (DicAnimationKeyFrame.TryGetValue(NextKeyFrame, out ActiveKeyFrame))
                {
                    PolygonCutterKeyFrame NextActivePolygonCutterKeyFrame = (PolygonCutterKeyFrame)ActiveKeyFrame;

                    //Get the next polygons.
                    if (NextActivePolygonCutterKeyFrame.ListPolygon.Count != ListPolygon.Count)
                    {
                        ListPolygon.Clear();
                        for (int P = 0; P < NextActivePolygonCutterKeyFrame.ListPolygon.Count; P++)
                        {
                            Polygon ActivePolygon = new Polygon(NextActivePolygonCutterKeyFrame.ListPolygon[P]);
                            ListPolygon.Add(ActivePolygon);
                        }
                    }
                    for (int P = 0; P < NextActivePolygonCutterKeyFrame.ListPolygon.Count; P++)
                    {
                        Polygon ActivePolygon = NextActivePolygonCutterKeyFrame.ListPolygon[P];

                        if (PolygonCutterType == PolygonCutterTypes.Crop)
                        {
                            ActivePolygon.ComputeColorAndUVCoordinates(AnimationClass.GraphicsDevice.PresentationParameters.BackBufferWidth, AnimationClass.GraphicsDevice.PresentationParameters.BackBufferHeight);
                        }
                    }

                    if (ActiveKeyFrame.IsProgressive)
                    {
                        OnProgressiveNextKeyFrameAnimationSprite(ActiveKeyFrame, KeyFrame, NextKeyFrame);

                        float KeyFrameChange = KeyFrame - NextKeyFrame;
                        ArrayMoveValue = null;

                        if (ActivePolygonCutterKeyFrame.ListPolygon.Count == NextActivePolygonCutterKeyFrame.ListPolygon.Count)
                        {
                            ArrayMoveValue = new Vector2[NextActivePolygonCutterKeyFrame.ListPolygon.Count][];

                            for (int P = 0; P < NextActivePolygonCutterKeyFrame.ListPolygon.Count; P++)
                            {
                                if (ActivePolygonCutterKeyFrame.ListPolygon[P].ArrayVertex.Length != NextActivePolygonCutterKeyFrame.ListPolygon[P].ArrayVertex.Length)
                                {
                                    ArrayMoveValue = null;
                                    break;
                                }
                                else
                                {
                                    ArrayMoveValue[P] = new Vector2[NextActivePolygonCutterKeyFrame.ListPolygon[P].ArrayVertex.Length];
                                    for (int V = 0; V < NextActivePolygonCutterKeyFrame.ListPolygon[P].ArrayVertex.Length; V++)
                                    {
                                        ArrayMoveValue[P][V] = new Vector2(
                                                                (ActivePolygonCutterKeyFrame.ListPolygon[P].ArrayVertex[V].X - NextActivePolygonCutterKeyFrame.ListPolygon[P].ArrayVertex[V].X) / KeyFrameChange,
                                                                (ActivePolygonCutterKeyFrame.ListPolygon[P].ArrayVertex[V].Y - NextActivePolygonCutterKeyFrame.ListPolygon[P].ArrayVertex[V].Y) / KeyFrameChange);
                                    }
                                }
                            }
                        }
                    }
                    else
                        NextEvent = null;
                }
            }
        }

        public override void GetMinMax(out int MinX, out int MinY, out int MaxX, out int MaxY)
        {
            MinX = int.MaxValue;
            MinY = int.MaxValue;
            MaxX = int.MinValue;
            MaxY = int.MinValue;

            foreach (Polygon ActivePolygon in ListPolygon)
            {
                for (int V = 0; V < ActivePolygon.ArrayVertex.Length; V++)
                {
                    int PolygonX = (int)(Position.X + ActivePolygon.ArrayVertex[V].X);
                    int PolygonY = (int)(Position.Y + ActivePolygon.ArrayVertex[V].Y);

                    if (PolygonX < MinX)
                        MinX = PolygonX;
                    if (PolygonX > MaxX)
                        MaxX = PolygonX;

                    if (PolygonY < MinY)
                        MinY = PolygonY;
                    if (PolygonY > MaxY)
                        MaxY = PolygonY;
                }
            }
        }

        public override void OnUpdatePosition(Vector2 Translation)
        {
            foreach (Polygon ActivePolygon in ListPolygon)
            {
                ActivePolygon.UpdateWorldPosition(Translation, Angle);
            }

            float MinX = float.MaxValue;
            float MaxX = float.MinValue;
            float MinY = float.MaxValue;
            float MaxY = float.MinValue;

            foreach (Polygon ActivePolygon in ListPolygon)
            {
                for (int V = 0; V < ActivePolygon.VertexCount; V++)
                {
                    int VertexX = (int)ActivePolygon.ArrayVertex[V].X;
                    int VertexY = (int)ActivePolygon.ArrayVertex[V].Y;

                    if (VertexX < MinX)
                        MinX = VertexX;
                    if (VertexX > MaxX)
                        MaxX = VertexX;

                    if (VertexY < MinY)
                        MinY = VertexY;
                    if (VertexY > MaxY)
                        MaxY = VertexY;
                }
            }

            SourceRectangle = new Rectangle((int)MinX, (int)MinY, (int)(MaxX - MinX), (int)(MaxY - MinY));
        }

        public override bool MouseDownExtra(int RealX, int RealY)
        {
            foreach (Polygon ActivePolygon in ListPolygon)
            {
                for (int V = 0; V < ActivePolygon.ArrayVertex.Length; V++)
                { 
                    int KeyFrameX = (int)ActivePolygon.ArrayVertex[V].X - 2;
                    int KeyFrameY = (int)ActivePolygon.ArrayVertex[V].Y - 2;

                    if (RealX >= KeyFrameX && RealX < KeyFrameX + 5 &&
                        RealY >= KeyFrameY && RealY < KeyFrameY + 5)
                    {
                        return true;
                    }
                }
            }
            return base.MouseDownExtra(RealX, RealY);
        }

        public override void MouseMoveExtra(int KeyFrame, int RealX, int RealY, int MouseChangeX, int MouseChangeY)
        {
            PolygonCutterKeyFrame ActiveObjectKeyFrame = (PolygonCutterKeyFrame)CreateOrRetriveKeyFrame(ActiveLayer, KeyFrame);

            for (int P = 0; P < ActiveObjectKeyFrame.ListPolygon.Count; P++)
            {
                Polygon ActivePolygon = ListPolygon[P];
                Polygon ActiveKeyFramePolygon = ActiveObjectKeyFrame.ListPolygon[P];

                for (int V = 0; V < ActiveKeyFramePolygon.ArrayVertex.Length; V++)
                {
                    int KeyFrameX = (int)ActiveKeyFramePolygon.ArrayVertex[V].X - 2;
                    int KeyFrameY = (int)ActiveKeyFramePolygon.ArrayVertex[V].Y - 2;

                    if (RealX >= KeyFrameX && RealX < KeyFrameX + 5 &&
                        RealY >= KeyFrameY && RealY < KeyFrameY + 5)
                    {
                        ActiveKeyFramePolygon.ArrayVertex[V] =
                            new Vector2(ActiveKeyFramePolygon.ArrayVertex[V].X + MouseChangeX,
                                ActiveKeyFramePolygon.ArrayVertex[V].Y + MouseChangeY);

                        ActivePolygon.ArrayVertex[V] = ActiveKeyFramePolygon.ArrayVertex[V];
                        break;
                    }
                }
            }

            base.MouseMoveExtra(KeyFrame, RealX, RealY, MouseChangeX, MouseChangeY);
        }

        public override void MouseUpExtra()
        {
            base.MouseUpExtra();
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
        }

        public override void DrawExtra(CustomSpriteBatch g, Texture2D sprPixel)
        {
            base.DrawExtra(g, sprPixel);

            foreach (Polygon ActivePolygon in ListPolygon)
            {
                for (int V = 0; V < ActivePolygon.ArrayVertex.Length; V++)
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle((int)ActivePolygon.ArrayVertex[V].X - 2,
                                                    (int)ActivePolygon.ArrayVertex[V].Y - 2, 5, 5), Color.Black);
                }
            }
        }

        public override void Draw(CustomSpriteBatch g, bool IsInEditMode)
        {
            foreach (Polygon ActivePolygon in ListPolygon)
            {
                ActivePolygon.Draw(g.GraphicsDevice);
            }

            if (IsInEditMode)
            {
                foreach (Polygon ActivePolygon in ListPolygon)
                {
                    for (int I = 0; I < ActivePolygon.ArrayIndex.Length; I += 3)
                    {
                        Vector2 Vertex1 = ActivePolygon.ArrayVertex[ActivePolygon.ArrayIndex[I]];
                        Vector2 Vertex2 = ActivePolygon.ArrayVertex[ActivePolygon.ArrayIndex[I + 1]];
                        Vector2 Vertex3 = ActivePolygon.ArrayVertex[ActivePolygon.ArrayIndex[I + 2]];

                        g.DrawLine(GameScreen.sprPixel, Vertex1, Vertex2, Color.Black, 1);
                        g.DrawLine(GameScreen.sprPixel, Vertex2, Vertex3, Color.Black, 1);
                        g.DrawLine(GameScreen.sprPixel, Vertex3, Vertex1, Color.Black, 1);
                    }
                }
            }
        }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public override int Width { get { return SourceRectangle.Width; } }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public override int Height { get { return SourceRectangle.Height; } }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public PolygonCutterTypes PolygonCutterType
        {
            get
            {
                return _PolygonCutterType;
            }
        }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public PolygonCutterSpecialEffects PolygonCutterSpecialEffect
        {
            get
            {
                return _PolygonCutterSpecialEffect;
            }
        }
    }
}
