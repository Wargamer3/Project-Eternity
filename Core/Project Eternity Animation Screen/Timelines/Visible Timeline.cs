using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProjectEternity.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using System.IO;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public abstract class VisibleTimeline : Timeline
    {
        public Dictionary<int, VisibleAnimationObjectKeyFrame> DicAnimationKeyFrame;

        public int _SpawnFrame;
        public int _DeathFrame;
        public Vector2 _Position;
        public Vector2 PositionOld;
        public int EventKeyFrameOld;
        public Point Origin;
        public float DrawingDepth;
        public float DrawingDepthOld;

        public Vector2 ScaleFactor;
        public Vector2 ScaleFactorOld;
        public float Angle;
        public float AngleOld;
        public int Alpha;
        public int AlphaOld;

        public int NextEventKeyFrame;
        protected VisibleAnimationObjectKeyFrame NextEvent;

        public VisibleTimeline(string TimelineEventType, string Name)
            : base(TimelineEventType, Name)
        {
            DicAnimationKeyFrame = new Dictionary<int, VisibleAnimationObjectKeyFrame>();
            CanDelete = true;
            NextEvent = null;
            NextEventKeyFrame = -1;
            GroupIndex = -1;
        }

        public VisibleTimeline(BinaryReader BR, string TimelineEventType)
            : base(BR, TimelineEventType)
        {
            _SpawnFrame = BR.ReadInt32();
            _DeathFrame = BR.ReadInt32();

            DicAnimationKeyFrame = new Dictionary<int, VisibleAnimationObjectKeyFrame>();
            CanDelete = true;
            NextEvent = null;
            NextEventKeyFrame = -1;
            GroupIndex = -1;
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(_TimelineEventType);
            BW.Write(Name);
            BW.Write(GroupIndex);
            BW.Write(_SpawnFrame);
            BW.Write(_DeathFrame);

            DoSave(BW);
        }

        protected override Timeline LoadCopy(BinaryReader BR, Microsoft.Xna.Framework.Content.ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            return DoLoadCopy(BR, Content, ActiveLayer);
        }

        protected abstract VisibleTimeline DoLoadCopy(BinaryReader BR, Microsoft.Xna.Framework.Content.ContentManager Content, AnimationClass.AnimationLayer ActiveLayer);

        public override void ResetAnimationLayer()
        {
            IsUsed = false;

            foreach (KeyValuePair<int, VisibleAnimationObjectKeyFrame> Move in DicAnimationKeyFrame)
            {
                Move.Value.IsUsed = false;
            }
        }

        public override void SpawnItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame)
        {
            EventKeyFrameOld = KeyFrame;
            ActiveAnimation.OnVisibleTimelineSpawn(ActiveLayer, this);
        }

        public virtual void OnDeathFrame(AnimationClass ActiveAnimation)
        {
            ActiveAnimation.OnVisibleTimelineDeath(this);
        }

        public abstract void BeginDraw(CustomSpriteBatch g);

        public abstract void Draw(CustomSpriteBatch g, bool IsInEditMode);

        public abstract void UpdateAnimationObject(int KeyFrame);

        public override AnimationObjectKeyFrame Get(int Key)
        {
            return DicAnimationKeyFrame[Key];
        }

        public override bool TryGetValue(int Key, out AnimationObjectKeyFrame Value)
        {
            VisibleAnimationObjectKeyFrame OutVisibleAnimationObjectKeyFrame = null;
            bool ReturnValue = DicAnimationKeyFrame.TryGetValue(Key, out OutVisibleAnimationObjectKeyFrame);
            Value = OutVisibleAnimationObjectKeyFrame;
            return ReturnValue;
        }

        public override void Add(int Index, AnimationObjectKeyFrame NewAnimationObjectKeyFrame)
        {
            DicAnimationKeyFrame.Add(Index, (VisibleAnimationObjectKeyFrame)NewAnimationObjectKeyFrame);
        }

        public override void Remove(int Key)
        {
            DicAnimationKeyFrame.Remove(Key);
        }

        public override int[] Keys
        {
            get
            {
                return DicAnimationKeyFrame.Keys.ToArray();
            }
        }

        public override void MoveTimeline(int Difference)
        {
            if (NextEventKeyFrame >= 0)//If there is a next key frame, update it.
                NextEventKeyFrame += Difference;

            //Remove first to avoid duplicate error
            Dictionary<int, VisibleAnimationObjectKeyFrame> ListRemovedKeyFrame = new Dictionary<int, VisibleAnimationObjectKeyFrame>();
            foreach (int KeyFrame in Keys)
            {
                VisibleAnimationObjectKeyFrame ActiveKeyFrame = DicAnimationKeyFrame[KeyFrame];
                ListRemovedKeyFrame.Add(KeyFrame, ActiveKeyFrame);

                Remove(KeyFrame);
            }

            foreach (KeyValuePair<int, VisibleAnimationObjectKeyFrame> ActiveKeyFrame in ListRemovedKeyFrame)
            {
                if (ActiveKeyFrame.Value.NextKeyFrame >= 0)//If there is a next key frame, update it.
                    ActiveKeyFrame.Value.NextKeyFrame += Difference;

                Add(ActiveKeyFrame.Key + Difference, ActiveKeyFrame.Value);
            }

            //Replace the old Tag.
            SpawnFrame += Difference;
            DeathFrame += Difference;//Changing the deathframe will automatically destroy keyframes so only change it after the movement.
        }

        public override void CreateKeyFrame(AnimationClass.AnimationLayer ActiveLayer, int KeyFrame)
        {
            int HighestPreviousKeyFrame;
            int NextKeyFrame;
            GetSurroundingKeyFrames(KeyFrame, out HighestPreviousKeyFrame, out NextKeyFrame);
            VisibleAnimationObjectKeyFrame HighestPreviousKeyFrameObject;
            DicAnimationKeyFrame.TryGetValue(HighestPreviousKeyFrame, out HighestPreviousKeyFrameObject);

            if (HighestPreviousKeyFrame != -1)
            {
                HighestPreviousKeyFrameObject.NextKeyFrame = KeyFrame;
            }

            if (HighestPreviousKeyFrameObject != null)
            {
                AnimationObjectKeyFrame ActiveAnimationSprite = HighestPreviousKeyFrameObject.Copy(ActiveLayer, NextKeyFrame);

                Add(KeyFrame, ActiveAnimationSprite);
            }
            else
            {
                Add(KeyFrame, null);
            }

            NextEventKeyFrame = KeyFrame;
        }

        public override void DeleteKeyFrame(int KeyFrame)
        {
            base.DeleteKeyFrame(KeyFrame);

            int HighestPreviousKeyFrame;
            int NextKeyFrame;

            GetSurroundingKeyFrames(KeyFrame, out HighestPreviousKeyFrame, out NextKeyFrame);

            if (HighestPreviousKeyFrame >= 0)
            {
                DicAnimationKeyFrame[HighestPreviousKeyFrame].NextKeyFrame = NextKeyFrame;
            }
            NextEventKeyFrame = NextKeyFrame;
        }

        public override void MoveKeyFrame(int OriginalKeyFrame, int NewKeyFrame)
        {
            int HighestPreviousKeyFrame;
            int NextKeyFrame;

            GetSurroundingKeyFrames(OriginalKeyFrame, out HighestPreviousKeyFrame, out NextKeyFrame);

            AnimationObjectKeyFrame KeyFrameToMove = Get(OriginalKeyFrame);

            if (HighestPreviousKeyFrame >= 0)
            {
                if (HighestPreviousKeyFrame == NewKeyFrame || NextKeyFrame == NewKeyFrame)
                {
                    AnimationObjectKeyFrame KeyFrameToSwapWith = Get(NewKeyFrame);

                    if (NewKeyFrame > OriginalKeyFrame)
                    {
                        DicAnimationKeyFrame[OriginalKeyFrame].NextKeyFrame = NextKeyFrame;
                        DicAnimationKeyFrame[NewKeyFrame].NextKeyFrame = NewKeyFrame;
                    }
                    else
                    {
                        DicAnimationKeyFrame[OriginalKeyFrame].NextKeyFrame = OriginalKeyFrame;
                        DicAnimationKeyFrame[NewKeyFrame].NextKeyFrame = NextKeyFrame;
                    }
                    Remove(NewKeyFrame);
                    Remove(OriginalKeyFrame);

                    Add(NewKeyFrame, KeyFrameToMove);
                    Add(OriginalKeyFrame, KeyFrameToSwapWith);
                }
                else
                {
                    DicAnimationKeyFrame[HighestPreviousKeyFrame].NextKeyFrame = NewKeyFrame;
                    Remove(OriginalKeyFrame);
                    Add(NewKeyFrame, KeyFrameToMove);
                }
            }

            NextEventKeyFrame = NewKeyFrame;
        }

        protected void UpdateFrom(VisibleTimeline Other, AnimationClass.AnimationLayer ActiveLayer)
        {
            base.UpdateFrom(Other, ActiveLayer);

            NextEventKeyFrame = Other.NextEventKeyFrame;
            if (NextEventKeyFrame >= 0 && Other.Count > 0)
                NextEvent = DicAnimationKeyFrame[NextEventKeyFrame];
            else
                NextEvent = null;

            _Position = Other._Position;
            PositionOld = Other.PositionOld;
            EventKeyFrameOld = Other.EventKeyFrameOld;
            Origin = Other.Origin;
            SpawnFrame = Other.SpawnFrame;
            DeathFrame = Other.DeathFrame;
            DrawingDepth = Other.DrawingDepth;
            DrawingDepthOld = Other.DrawingDepthOld;

            ScaleFactor = Other.ScaleFactor;
            ScaleFactorOld = Other.ScaleFactorOld;
            Angle = Other.Angle;
            AngleOld = Other.AngleOld;
            Alpha = Other.Alpha;
            AlphaOld = Other.AlphaOld;
        }

        public virtual void GetMinMax(out int MinX, out int MinY, out int MaxX, out int MaxY)
        {
            MinX = (int)(Position.X - Origin.X * Math.Abs(ScaleFactor.X));
            MinY = (int)(Position.Y - Origin.Y * Math.Abs(ScaleFactor.Y));
            MaxX = (int)(MinX + Width * Math.Abs(ScaleFactor.X));
            MaxY = (int)(MinY + Height * Math.Abs(ScaleFactor.Y));
        }

        public virtual bool CanSelect(int PosX, int PosY)
        {
            int MinX, MinY, MaxX, MaxY;
            GetMinMax(out MinX, out MinY, out MaxX, out MaxY);

            //Select Item.
            return PosX >= MinX && PosX < MaxX && PosY >= MinY && PosY < MaxY;
        }

        //Generic Update methods.
        protected void UpdateAnimationSprite(int KeyFrame)
        {
            float ProgressionValue = (KeyFrame - EventKeyFrameOld) / (float)(NextEventKeyFrame - EventKeyFrameOld);

            if (DicAnimationKeyFrame.ContainsKey(EventKeyFrameOld) && DicAnimationKeyFrame[EventKeyFrameOld].ListSpecialEffectNode.Count > 0)
            {
                #region Bezier Curve

                float t = ProgressionValue;

                float x0 = PositionOld.X;
                float x2 = DicAnimationKeyFrame[NextEventKeyFrame].Position.X;
                float y0 = PositionOld.Y;
                float y2 = DicAnimationKeyFrame[NextEventKeyFrame].Position.Y;

                int n = DicAnimationKeyFrame[EventKeyFrameOld].ListSpecialEffectNode.Count + 1;
                double ResultX = Math.Pow(1 - t, n) * x0;
                double ResultY = Math.Pow(1 - t, n) * y0;

                int nFac = 1;
                for (int k = 1; k <= n; k++)
                    nFac *= k;

                for (int N = 0; N < DicAnimationKeyFrame[EventKeyFrameOld].ListSpecialEffectNode.Count; N++)
                {
                    int i = N + 1;

                    int niFac = 1;
                    int iFac = 1;

                    for (int k = 1; k <= n - i; k++)
                        niFac *= k;

                    for (int k = 1; k <= i; k++)
                        iFac *= k;

                    int nChoosek = nFac / (niFac * iFac);

                    //n! / ((n-i)!*i!) * ((1 - t)^(n-1)) * (t ^ i) * Point[i]
                    ResultX += nChoosek * Math.Pow(1 - t, n - i) * Math.Pow(t, i) * DicAnimationKeyFrame[EventKeyFrameOld].ListSpecialEffectNode[N].X;
                    ResultY += nChoosek * Math.Pow(1 - t, n - i) * Math.Pow(t, i) * DicAnimationKeyFrame[EventKeyFrameOld].ListSpecialEffectNode[N].Y;
                }
                ResultX += Math.Pow(t, n) * x2;
                ResultY += Math.Pow(t, n) * y2;

                Position = new Vector2((float)ResultX, (float)ResultY);

                #endregion
            }
            else
            {
                Vector2 Result = NextEvent.MoveValue * (KeyFrame - EventKeyFrameOld);
                Position = new Vector2(PositionOld.X + (int)Result.X, PositionOld.Y + (int)Result.Y);
            }

            ScaleFactor = ScaleFactorOld + NextEvent.ScaleValue * (KeyFrame - EventKeyFrameOld);
            Angle = AngleOld + NextEvent.AngleValue * (KeyFrame - EventKeyFrameOld);
            Alpha = AlphaOld + (int)(NextEvent.AlphaValue * (KeyFrame - EventKeyFrameOld));

            if (KeyFrame >= NextEventKeyFrame)
                NextEvent = null;
        }

        public virtual void OnUpdatePosition(Vector2 Translation)
        {
        }

        protected void OnNewKeyFrameAnimationSprite(VisibleAnimationObjectKeyFrame ActiveKeyFrame)
        {
            Position = ActiveKeyFrame.Position;
            ScaleFactor = ActiveKeyFrame.ScaleFactor;
            Angle = ActiveKeyFrame.AngleInRad;
            Alpha = ActiveKeyFrame.Alpha;
            DrawingDepth = ActiveKeyFrame.DrawingDepth;

            PositionOld = ActiveKeyFrame.Position;
            ScaleFactorOld = ActiveKeyFrame.ScaleFactor;
            AngleOld = ActiveKeyFrame.AngleInRad;
            AlphaOld = ActiveKeyFrame.Alpha;
            DrawingDepthOld = ActiveKeyFrame.DrawingDepth;

            ActiveKeyFrame.IsUsed = true;
        }

        protected void OnProgressiveNextKeyFrameAnimationSprite(VisibleAnimationObjectKeyFrame ActiveKeyFrame, int KeyFrame, int NextKeyFrame)
        {
            NextEvent = ActiveKeyFrame;
            PositionOld = Position;
            ScaleFactorOld = ScaleFactor;
            AngleOld = Angle;
            AlphaOld = Alpha;
            DrawingDepthOld = DrawingDepth;

            EventKeyFrameOld = KeyFrame;
            NextEventKeyFrame = NextKeyFrame;

            float KeyFrameChange = KeyFrame - NextKeyFrame;
            //Calculate of how many pixel the AnimatedBitmap will move per step.
            ActiveKeyFrame.MoveValue = new Vector2((Position.X - ActiveKeyFrame.Position.X) / KeyFrameChange,
                                                   (Position.Y - ActiveKeyFrame.Position.Y) / KeyFrameChange);
            ActiveKeyFrame.ScaleValue = (ScaleFactor - ActiveKeyFrame.ScaleFactor) / KeyFrameChange;
            ActiveKeyFrame.AngleValue = (Angle - ActiveKeyFrame.AngleInRad) / KeyFrameChange;
            ActiveKeyFrame.AlphaValue = (Alpha - ActiveKeyFrame.Alpha) / KeyFrameChange;
            ActiveKeyFrame.DrawingDepthValue = (DrawingDepth - ActiveKeyFrame.DrawingDepth) / KeyFrameChange;
        }

        public override void DrawKeyFrames(System.Drawing.Graphics g, int TimelineStartX, int ScrollbarStartIndex, int VisibleIndex)
        {
            foreach (KeyValuePair<int, VisibleAnimationObjectKeyFrame> Move in DicAnimationKeyFrame)
            {
                int KeyFrameStartPos = TimelineStartX + 2 + Move.Key * 8 - ScrollbarStartIndex;
                int KeyFrameVisibleWidth = 4;
                KeyFrameVisibleWidth = Math.Min(4, KeyFrameStartPos + 2 - TimelineStartX);

                if (KeyFrameVisibleWidth > 0)
                    g.FillRectangle(System.Drawing.Brushes.Black, new System.Drawing.Rectangle(KeyFrameStartPos, 28 + VisibleIndex * 21, KeyFrameVisibleWidth, 4));

                if (Move.Value.NextKeyFrame != -1)
                {//Draw an arrow linking the 2 Key Frames.
                    int ArrowLength = 3;
                    int ArrowStartPos = TimelineStartX + Move.Value.NextKeyFrame * 8 - ArrowLength - ScrollbarStartIndex;
                    ArrowLength = Math.Min(3, ArrowStartPos - TimelineStartX);
                    KeyFrameStartPos = Math.Max(TimelineStartX, KeyFrameStartPos);

                    if (ArrowLength > 0)
                    {
                        g.DrawLine(System.Drawing.Pens.Black, KeyFrameStartPos, 30 + VisibleIndex * 21, TimelineStartX + Move.Value.NextKeyFrame * 8 - 3 - ScrollbarStartIndex, 30 + VisibleIndex * 21);
                        g.DrawLine(System.Drawing.Pens.Black, TimelineStartX + Move.Value.NextKeyFrame * 8 - ArrowLength - ScrollbarStartIndex, 30 + VisibleIndex * 21 - ArrowLength, TimelineStartX + Move.Value.NextKeyFrame * 8 - ScrollbarStartIndex, 30 + VisibleIndex * 21);
                        g.DrawLine(System.Drawing.Pens.Black, TimelineStartX + Move.Value.NextKeyFrame * 8 - ArrowLength - ScrollbarStartIndex, 30 + VisibleIndex * 21 + ArrowLength, TimelineStartX + Move.Value.NextKeyFrame * 8 - ScrollbarStartIndex, 30 + VisibleIndex * 21);
                    }
                }
            }
        }

        public virtual bool MouseDownExtra(int RealX, int RealY)
        {
            if (Count > 1)
            {
                int Index = SpawnFrame;

                while (Index >= 0)
                {
                    if (DicAnimationKeyFrame[Index].NextKeyFrame >= 0)
                    {
                        VisibleAnimationObjectKeyFrame ActiveKeyFrame = (VisibleAnimationObjectKeyFrame)Get(Index);

                        for (int N = 0; N < ActiveKeyFrame.ListSpecialEffectNode.Count; N++)
                        {
                            int KeyFrameX = (int)ActiveKeyFrame.ListSpecialEffectNode[N].X - 2;
                            int KeyFrameY = (int)ActiveKeyFrame.ListSpecialEffectNode[N].Y - 2;

                            if (RealX >= KeyFrameX && RealX < KeyFrameX + 5 &&
                                RealY >= KeyFrameY && RealY < KeyFrameY + 5)
                            {
                                return true;
                            }
                        }
                    }

                    Index = DicAnimationKeyFrame[Index].NextKeyFrame;
                }
            }

            return false;
        }

        public virtual void MouseMoveExtra(int KeyFrame, int RealX, int RealY, int MouseChangeX, int MouseChangeY)
        {
            if (Count > 1)
            {
                int Index = SpawnFrame;

                while (Index >= 0)
                {
                    VisibleAnimationObjectKeyFrame ActiveKeyFrame = DicAnimationKeyFrame[Index];
                    if (ActiveKeyFrame.NextKeyFrame >= 0)
                    {
                        for (int N = 0; N < ActiveKeyFrame.ListSpecialEffectNode.Count; N++)
                        {
                            int KeyFrameX = (int)ActiveKeyFrame.ListSpecialEffectNode[N].X - 2;
                            int KeyFrameY = (int)ActiveKeyFrame.ListSpecialEffectNode[N].Y - 2;

                            if (RealX >= KeyFrameX && RealX < KeyFrameX + 5 &&
                                RealY >= KeyFrameY && RealY < KeyFrameY + 5)
                            {
                                ActiveKeyFrame.ListSpecialEffectNode[N] =
                                    new Vector2(ActiveKeyFrame.ListSpecialEffectNode[N].X + MouseChangeX,
                                        ActiveKeyFrame.ListSpecialEffectNode[N].Y + MouseChangeY);
                                break;
                            }
                        }
                    }

                    Index = DicAnimationKeyFrame[Index].NextKeyFrame;
                }
            }
        }

        public virtual void MouseUpExtra()
        {

        }

        public void DrawNextPositionForMakers(CustomSpriteBatch g, Texture2D sprPixel)
        {
            int Index = SpawnFrame;

            while (Index >= 0)
            {
                g.Draw(sprPixel, new Rectangle((int)DicAnimationKeyFrame[Index].Position.X - 2, (int)DicAnimationKeyFrame[Index].Position.Y - 2, 5, 5), Color.Black);
                Vector2 ActivePosition = DicAnimationKeyFrame[Index].Position;

                if (DicAnimationKeyFrame[Index].NextKeyFrame >= 0)
                {
                    g.DrawLine(sprPixel, new Vector2(ActivePosition.X, ActivePosition.Y),
                        new Vector2(DicAnimationKeyFrame[DicAnimationKeyFrame[Index].NextKeyFrame].Position.X,
                                    DicAnimationKeyFrame[DicAnimationKeyFrame[Index].NextKeyFrame].Position.Y), Color.Black);
                }

                Index = DicAnimationKeyFrame[Index].NextKeyFrame;
            }
        }

        public void DrawNextPositions(CustomSpriteBatch g, Texture2D sprPixel, Color DrawColor)
        {
            int Index = SpawnFrame;

            while (Index >= 0)
            {
                VisibleAnimationObjectKeyFrame ActiveKeyFrame = DicAnimationKeyFrame[Index];

                g.Draw(sprPixel, new Rectangle((int)ActiveKeyFrame.Position.X - 2,
                                               (int)ActiveKeyFrame.Position.Y - 2, 5, 5), null, DrawColor, 0, Vector2.Zero, SpriteEffects.None, 0.1f);

                Vector2 ActivePosition = ActiveKeyFrame.Position;

                if (ActiveKeyFrame.NextKeyFrame >= 0 && DicAnimationKeyFrame.ContainsKey(ActiveKeyFrame.NextKeyFrame))
                {
                    VisibleAnimationObjectKeyFrame NextKeyFrame = DicAnimationKeyFrame[ActiveKeyFrame.NextKeyFrame];
                    g.DrawLine(sprPixel, new Vector2(ActivePosition.X, ActivePosition.Y),
                        new Vector2(NextKeyFrame.Position.X,
                                    NextKeyFrame.Position.Y), Color.Black);
                }

                Index = DicAnimationKeyFrame[Index].NextKeyFrame;
            }
        }

        public virtual void DrawExtra(CustomSpriteBatch g, Texture2D sprPixel)
        {
            int Index = SpawnFrame;

            while (Index >= 0)
            {
                VisibleAnimationObjectKeyFrame ActiveKeyFrame = DicAnimationKeyFrame[Index];

                Vector2 ActivePosition = ActiveKeyFrame.Position;

                if (ActiveKeyFrame.NextKeyFrame >= 0 && DicAnimationKeyFrame.ContainsKey(ActiveKeyFrame.NextKeyFrame))
                {
                    VisibleAnimationObjectKeyFrame NextKeyFrame = DicAnimationKeyFrame[ActiveKeyFrame.NextKeyFrame];

                    if (ActiveKeyFrame.ListSpecialEffectNode.Count > 0)
                    {
                        Vector2 NextPos = Vector2.Zero;
                        for (int N = 0; N < ActiveKeyFrame.ListSpecialEffectNode.Count; N++)
                        {
                            NextPos = ActiveKeyFrame.ListSpecialEffectNode[N];

                            g.Draw(sprPixel, new Rectangle((int)ActiveKeyFrame.ListSpecialEffectNode[N].X,
                                                           (int)ActiveKeyFrame.ListSpecialEffectNode[N].Y, 1, 1), Color.White);
                            g.Draw(sprPixel, new Rectangle((int)ActiveKeyFrame.ListSpecialEffectNode[N].X - 2,
                                                           (int)ActiveKeyFrame.ListSpecialEffectNode[N].Y - 2, 5, 5), Color.LightGreen);

                            g.DrawLine(sprPixel, new Vector2(ActivePosition.X,
                                                    ActivePosition.Y),
                                        new Vector2(NextPos.X,
                                                    NextPos.Y), Color.LightGreen);

                            ActivePosition = NextPos;
                        }

                        g.DrawLine(sprPixel, new Vector2(NextPos.X,
                                                NextPos.Y),
                                    new Vector2(NextKeyFrame.Position.X,
                                                NextKeyFrame.Position.Y), Color.LightGreen);
                    }
                }

                Index = DicAnimationKeyFrame[Index].NextKeyFrame;
            }
        }

        #region Properties

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public Vector2 Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
            }
        }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public abstract int Width { get; }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public abstract int Height { get; }

        [CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute(".")]
        public override int SpawnFrame
        {
            get
            {
                return _SpawnFrame;
            }
            set
            {
                _SpawnFrame = value;
            }
        }

        [CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute(".")]
        public override int DeathFrame
        {
            get
            {
                return _DeathFrame;
            }
            set
            {
                _DeathFrame = value;

                //If there is a next key frame, update it.
                if (NextEventKeyFrame >= value)
                {
                    NextEventKeyFrame = -1;
                    NextEvent = null;
                }

                foreach (int KeyFrame in DicAnimationKeyFrame.Keys.ToList())
                {
                    if (KeyFrame >= value)//If the keyframe is out of bound, remove it.
                        DicAnimationKeyFrame.Remove(KeyFrame);
                }
            }
        }

        #endregion
    }
}
