using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public abstract class FixedTimeline : Timeline
    {
        public Dictionary<int, AnimationObjectKeyFrame> DicAnimationKeyFrame;

        public FixedTimeline(string TimelineEventType, string Name)
            : base(TimelineEventType, Name)
        {
            DicAnimationKeyFrame = new Dictionary<int, AnimationObjectKeyFrame>();
            CanDelete = false;
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(_TimelineEventType);

            DoSave(BW);
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(DicAnimationKeyFrame.Count);
            foreach (KeyValuePair<int, AnimationObjectKeyFrame> ActiveKeyFrame in DicAnimationKeyFrame)
            {
                BW.Write(ActiveKeyFrame.Key);
                ActiveKeyFrame.Value.Save(BW);
            }
        }

        protected override Timeline LoadCopy(BinaryReader BR, Microsoft.Xna.Framework.Content.ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            FixedTimeline NewTimeline = DoLoadCopy(BR);

            return NewTimeline;
        }

        protected abstract FixedTimeline DoLoadCopy(BinaryReader BR);

        protected abstract AnimationObjectKeyFrame CreateFirstKeyFrame();

        public override void DrawKeyFrames(Graphics g, int TimelineStartX, int ScrollbarStartIndex, int VisibleIndex)
        {
            foreach (KeyValuePair<int, AnimationObjectKeyFrame> Move in DicAnimationKeyFrame)
            {
                int KeyFrameStartPos = TimelineStartX + 2 + Move.Key * 8 - ScrollbarStartIndex;
                int KeyFrameVisibleWidth = 4;
                KeyFrameVisibleWidth = Math.Min(4, KeyFrameStartPos + 2 - TimelineStartX);

                if (KeyFrameVisibleWidth > 0)
                    g.FillRectangle(Brushes.Black, new Rectangle(KeyFrameStartPos, 28 + VisibleIndex * 21, KeyFrameVisibleWidth, 4));
            }
        }

        public override void ResetAnimationLayer()
        {
            IsUsed = false;

            foreach (KeyValuePair<int, AnimationObjectKeyFrame> Move in DicAnimationKeyFrame)
                Move.Value.IsUsed = false;
        }

        public override AnimationObjectKeyFrame Get(int Key)
        {
            return DicAnimationKeyFrame[Key];
        }

        public override bool TryGetValue(int Key, out AnimationObjectKeyFrame Value)
        {
            AnimationObjectKeyFrame OutVisibleAnimationObjectKeyFrame = null;
            bool ReturnValue = DicAnimationKeyFrame.TryGetValue(Key, out OutVisibleAnimationObjectKeyFrame);
            Value = OutVisibleAnimationObjectKeyFrame;
            return ReturnValue;
        }

        public override void Add(int Index, AnimationObjectKeyFrame NewAnimationObjectKeyFrame)
        {
            if (NewAnimationObjectKeyFrame == null)
            {
                DicAnimationKeyFrame.Add(Index, CreateFirstKeyFrame());
            }
            else
            {
                DicAnimationKeyFrame.Add(Index, NewAnimationObjectKeyFrame);
            }
        }

        public override void Remove(int Key)
        {
            DicAnimationKeyFrame.Remove(Key);
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, Point MousePosition)
        {
            throw new NotImplementedException();
        }

        public override int[] Keys
        {
            get
            {
                return DicAnimationKeyFrame.Keys.ToArray();
            }
        }

        #region Properties

        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
            }
        }

        public override int SpawnFrame
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public override int DeathFrame
        {
            get
            {
                return 200000000;
            }
            set
            {
            }
        }

        #endregion
    }
}
