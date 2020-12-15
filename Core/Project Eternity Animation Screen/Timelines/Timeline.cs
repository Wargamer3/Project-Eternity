using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public abstract class Timeline
    {
        public const double FramesPerSeconds = 60d;
        protected readonly string _TimelineEventType;
        public bool IsUsed;
        public bool CanDelete;

        private string _Name;
        public int GroupIndex;

        public Timeline(string TimelineEventType, string Name)
        {
            this._TimelineEventType = TimelineEventType;
            _Name = Name;
            CanDelete = true;
            IsUsed = false;
            GroupIndex = -1;
        }

        public Timeline(BinaryReader BR, string TimelineEventType)
        {
            this._TimelineEventType = TimelineEventType;

            _Name = BR.ReadString();
            GroupIndex = BR.ReadInt32();
        }

        public abstract void DrawKeyFrames(Graphics g, int TimelineStartX, int ScrollbarStartIndex, int VisibleIndex);

        public abstract void ResetAnimationLayer();

        public virtual void Save(BinaryWriter BW)
        {
            BW.Write(_TimelineEventType);
            BW.Write(Name);
            BW.Write(GroupIndex);

            DoSave(BW);
        }

        protected abstract void DoSave(BinaryWriter BW);

        public static Timeline Load(BinaryReader BR, Microsoft.Xna.Framework.Content.ContentManager Content, AnimationClass.AnimationLayer ActiveLayer, Dictionary<string, Timeline> DicTimeline)
        {
            string TimelineEventType = BR.ReadString();

            Timeline NewTimeline = DicTimeline[TimelineEventType].LoadCopy(BR, Content, ActiveLayer);

            return NewTimeline;
        }

        protected abstract Timeline LoadCopy(BinaryReader BR, Microsoft.Xna.Framework.Content.ContentManager Content, AnimationClass.AnimationLayer ActiveLayer);

        public abstract Timeline Copy(AnimationClass.AnimationLayer ActiveLayer);

        public abstract void SpawnItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame);

        public abstract void Add(int Key, AnimationObjectKeyFrame NewAnimationObjectKeyFrame);

        public abstract bool TryGetValue(int Key, out AnimationObjectKeyFrame Value);

        public abstract AnimationObjectKeyFrame Get(int Key);

        public abstract void Remove(int Key);

        public virtual void OnAnimationEditorLoad(AnimationClass ActiveAnimation)
        {
        }

        public abstract List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, Point MousePosition);

        public abstract int[] Keys { get; }

        public bool ContainsKey(int Key)
        {
            return TryGetValue(Key, out _);
        }

        public int Count
        {
            get
            {
                return Keys.Length;
            }
        }

        public virtual void MoveTimeline(int Difference)
        {
        }

        public void UpdateFrom(Timeline Other, AnimationClass.AnimationLayer ActiveLayer)
        {
            Name = Other.Name;

            foreach (int Key in Other.Keys)
            {
                Add(Key, Other.Get(Key).Copy(ActiveLayer));
            }
        }

        public void GetSurroundingKeyFrames(int KeyFrame, out int HighestPreviousKeyFrame, out int NextKeyFrame)
        {
            NextKeyFrame = -1;
            HighestPreviousKeyFrame = -1;
            int LowestNextKeyFrame = int.MaxValue;
            bool LowerNextKeyFrameFound = false;

            foreach (int Key in Keys)
            {
                if (Key < KeyFrame && Key > HighestPreviousKeyFrame)
                    HighestPreviousKeyFrame = Key;

                if (Key > KeyFrame && Key < LowestNextKeyFrame)
                {
                    LowestNextKeyFrame = Key;
                    LowerNextKeyFrameFound = true;
                }
            }

            if (LowerNextKeyFrameFound)
                NextKeyFrame = LowestNextKeyFrame;
        }

        public AnimationObjectKeyFrame CreateOrRetriveKeyFrame(AnimationClass.AnimationLayer ActiveLayer, int KeyFrame)
        {
            //Add a new move Key Frame if needed.
            if (!ContainsKey(KeyFrame))
            {
                CreateKeyFrame(ActiveLayer, KeyFrame);
            }
            
            return Get(KeyFrame);
        }

        public virtual void CreateKeyFrame(AnimationClass.AnimationLayer ActiveLayer, int KeyFrame)
        {
            int HighestPreviousKeyFrame;
            GetSurroundingKeyFrames(KeyFrame, out HighestPreviousKeyFrame, out _);
            AnimationObjectKeyFrame HighestPreviousKeyFrameObject;
            TryGetValue(HighestPreviousKeyFrame, out HighestPreviousKeyFrameObject);

            if (HighestPreviousKeyFrameObject != null)
            {
                AnimationObjectKeyFrame ActiveAnimationSprite = HighestPreviousKeyFrameObject.Copy(ActiveLayer);

                Add(KeyFrame, ActiveAnimationSprite);
            }
            else
            {
                Add(KeyFrame, null);
            }
        }

        public virtual void DeleteKeyFrame(int KeyFrame)
        {
            Remove(KeyFrame);
        }

        public virtual void MoveKeyFrame(int OriginalKeyFrame, int NewKeyFrame)
        {
            AnimationObjectKeyFrame KeyFrameToMove = Get(OriginalKeyFrame);
            DeleteKeyFrame(OriginalKeyFrame);
            Add(NewKeyFrame, KeyFrameToMove);
        }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public virtual string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        public abstract int SpawnFrame { get; set; }
        public abstract int DeathFrame { get; set; }
        public string TimelineEventType { get { return _TimelineEventType; } }
    }
}
