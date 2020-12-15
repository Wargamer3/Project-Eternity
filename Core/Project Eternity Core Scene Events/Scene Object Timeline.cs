using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.Core.Scene
{
    public class SceneObjectTimeline : VisibleTimeline
    {
        private const string TimelineType = "Scene Object";

        public SceneObject SceneObject;

        public SceneObjectTimeline()
            : base(TimelineType, "New Gun Nozzle")
        {
        }

        public SceneObjectTimeline(SceneObject SceneObject)
            : this()
        {
            this.SceneObject = SceneObject;
            Origin = new Point(Width / 2, Height / 2);
        }

        public SceneObjectTimeline(BinaryReader BR, SceneObject SceneObject)
            : base(BR, TimelineType)
        {
            this.SceneObject = SceneObject;

            int DicAnimationSpriteKeyFrameCount = BR.ReadInt32();
            for (int E = 0; E < DicAnimationSpriteKeyFrameCount; E++)
            {
                int Key = BR.ReadInt32();

                VisibleAnimationObjectKeyFrame NewAnimatedBitmapKeyFrame = new VisibleAnimationObjectKeyFrame(BR);

                DicAnimationKeyFrame.Add(Key, NewAnimatedBitmapKeyFrame);
            }
        }

        protected override VisibleTimeline DoLoadCopy(BinaryReader BR, Microsoft.Xna.Framework.Content.ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            return new SceneObjectTimeline(BR, SceneObject);
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(DicAnimationKeyFrame.Count);
            foreach (KeyValuePair<int, VisibleAnimationObjectKeyFrame> KeyFrame in DicAnimationKeyFrame)
            {
                BW.Write(KeyFrame.Key);
                KeyFrame.Value.Save(BW);
            }
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            SceneObjectTimeline NewSetMarkerEvent = new SceneObjectTimeline(SceneObject);

            NewSetMarkerEvent.UpdateFrom(this, ActiveLayer);

            return NewSetMarkerEvent;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            SceneObjectTimeline NewDamageTimeline = new SceneObjectTimeline(SceneObject);
            NewDamageTimeline.Position = new Vector2(535, 170);
            NewDamageTimeline.SpawnFrame = KeyFrame;
            NewDamageTimeline.DeathFrame = KeyFrame + 10;
            NewDamageTimeline.IsUsed = true;//Disable the spawner as we spawn the Timeline manually.
            NewDamageTimeline.Add(KeyFrame,
                                    new VisibleAnimationObjectKeyFrame(new Vector2(NewDamageTimeline.Position.X, NewDamageTimeline.Position.Y),
                                                                        true, -1));

            ReturnValue.Add(NewDamageTimeline);

            return ReturnValue;
        }

        public override void UpdateAnimationObject(int KeyFrame)
        {
            //An Event is being executed.
            if (NextEvent != null)
            {
                UpdateAnimationSprite(KeyFrame);
            }

            VisibleAnimationObjectKeyFrame ActiveKeyFrame;
            VisibleAnimationObjectKeyFrame ActiveAnimationSpriteKeyFrame;

            if (DicAnimationKeyFrame.TryGetValue(KeyFrame, out ActiveAnimationSpriteKeyFrame))
            {
                ActiveKeyFrame = ActiveAnimationSpriteKeyFrame;
                //If that animation has already been used, skip it.
                if (ActiveKeyFrame.IsUsed)
                    return;

                int NextKeyFrame = ActiveKeyFrame.NextKeyFrame;
                OnNewKeyFrameAnimationSprite(ActiveKeyFrame);

                if (DicAnimationKeyFrame.TryGetValue(NextKeyFrame, out ActiveAnimationSpriteKeyFrame))
                {
                    ActiveKeyFrame = ActiveAnimationSpriteKeyFrame;
                    if (ActiveKeyFrame.IsProgressive)
                    {
                        OnProgressiveNextKeyFrameAnimationSprite(ActiveKeyFrame, KeyFrame, NextKeyFrame);
                    }
                    else
                        NextEvent = null;
                }
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
        }

        public override void Draw(CustomSpriteBatch g, bool IsInEditMode)
        {
        }

        #region Properties

        [CategoryAttribute("Ranged Attack Attributes"),
        DescriptionAttribute(".")]
        public override int Width { get { return 32; } }

        [CategoryAttribute("Ranged Attack Attributes"),
        DescriptionAttribute(".")]
        public override int Height { get { return 32; } }

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
                return int.MaxValue / 8;
            }

            set
            {
            }
        }

        #endregion
    }
}
