using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class GroupTimeline : Timeline
    {
        private const string TimelineType = "Group";

        public List<Timeline> ListEvent;
        public bool IsOpen;
        public uint KeyValue;

        public GroupTimeline(string Name = "New Group")
            : this(TimelineType, Name)
        {
            ListEvent = new List<Timeline>();
            IsOpen = true;
            this.Name = Name;
        }

        protected GroupTimeline(string TimelineEventType, string Name)
            : base(TimelineEventType, Name)
        {
            ListEvent = new List<Timeline>();
            IsOpen = true;
            this.Name = Name;
        }

        public GroupTimeline(BinaryReader BR)
            : base(BR, TimelineType)
        {
        }

        protected override Timeline LoadCopy(BinaryReader BR, ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            throw new NotImplementedException();
        }

        protected override void DoSave(BinaryWriter BW)
        {
            throw new NotImplementedException();
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            GroupTimeline NewGroupEvent = new GroupTimeline(Name);

            NewGroupEvent.UpdateFrom(this, ActiveLayer);

            NewGroupEvent.ListEvent = ListEvent;

            NewGroupEvent.KeyValue = KeyValue;
            NewGroupEvent.IsOpen = IsOpen;

            return NewGroupEvent;
        }

        public override void DrawKeyFrames(Graphics g, int TimelineStartX, int ScrollbarStartIndex, int VisibleIndex)
        {
        }

        public override void ResetAnimationLayer()
        {
        }

        public override void SpawnItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame)
        {
            throw new NotImplementedException();
        }

        public override AnimationObjectKeyFrame Get(int Key)
        {
            return null;
        }

        public override bool TryGetValue(int Key, out AnimationObjectKeyFrame Value)
        {
            Value = null;
            return false;
        }

        public override void Add(int Index, AnimationObjectKeyFrame NewAnimationObjectKeyFrame)
        {
        }

        public override void Remove(int Key)
        {
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, Point MousePosition)
        {
            throw new NotImplementedException();
        }

        public override int[] Keys
        {
            get
            {
                return new int[0];
            }
        }

        #region Properties

        [CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute(".")]
        public override int SpawnFrame
        {
            get
            {
                return 0;
            }
            set { }
        }

        [CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute(".")]
        public override int DeathFrame
        {
            get
            {
                return 0;
            }
            set { }
        }

        #endregion
    }
}
