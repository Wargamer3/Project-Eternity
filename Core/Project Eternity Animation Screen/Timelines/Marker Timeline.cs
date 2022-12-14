using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class MarkerTimeline : CoreTimeline
    {
        private const string TimelineType = "Marker";

        public Texture2D Sprite;
        public string BitmapName;
        private string _MarkerType;
        public AnimationClass AnimationMarker;//Draw the marker from an AnimationClass.

        public MarkerTimeline()
            : base(TimelineType, "New Marker")
        {
            BitmapName = string.Empty;
            _MarkerType = string.Empty;
        }

        public MarkerTimeline(string Name, string BitmapName, Texture2D Sprite)
            : this()
        {
            this.Name = Name;
            this.BitmapName = BitmapName;
            this.Sprite = Sprite;
            _MarkerType = "Enemy Stand";

            Origin = new Point(Width / 2, Height / 2);
        }

        public MarkerTimeline(BinaryReader BR, ContentManager Content)
            : base(BR, TimelineType)
        {
            BitmapName = BR.ReadString();
            _MarkerType = BR.ReadString();

            if (!string.IsNullOrEmpty(BitmapName) && Content != null)
            {
                Sprite = Content.Load<Texture2D>("Animations/Sprites/" + BitmapName);
            }

            Origin = new Point(Width / 2, Height / 2);

            int DicAnimationSpriteKeyFrameCount = BR.ReadInt32();
            for (int E = 0; E < DicAnimationSpriteKeyFrameCount; E++)
            {
                int Key = BR.ReadInt32();

                VisibleAnimationObjectKeyFrame NewAnimatedBitmapKeyFrame = new VisibleAnimationObjectKeyFrame(BR);

                DicAnimationKeyFrame.Add(Key, NewAnimatedBitmapKeyFrame);
            }
        }

        protected override VisibleTimeline DoLoadCopy(BinaryReader BR, ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            return new MarkerTimeline(BR, Content);
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(BitmapName);
            BW.Write(_MarkerType);

            BW.Write(DicAnimationKeyFrame.Count);
            foreach (KeyValuePair<int, VisibleAnimationObjectKeyFrame> KeyFrame in DicAnimationKeyFrame)
            {
                BW.Write(KeyFrame.Key);
                KeyFrame.Value.Save(BW);
            }
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            MarkerTimeline NewSetMarkerEvent = new MarkerTimeline();

            if (AnimationMarker != null)
                NewSetMarkerEvent.AnimationMarker = AnimationMarker.Copy();

            NewSetMarkerEvent.BitmapName = BitmapName;
            NewSetMarkerEvent._MarkerType = _MarkerType;
            NewSetMarkerEvent.Sprite = Sprite;

            NewSetMarkerEvent.UpdateFrom(this, ActiveLayer);

            return NewSetMarkerEvent;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            MarkerHelper NewSpawner = new MarkerHelper();
            if (NewSpawner.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MarkerTimeline NewSetMarkerEvent = NewSpawner.MarkerViewer.ActiveMarker;

                NewSetMarkerEvent.Position = new Vector2(535, 170);
                NewSetMarkerEvent.SpawnFrame = KeyFrame;
                NewSetMarkerEvent.DeathFrame = KeyFrame + 10;
                NewSetMarkerEvent.IsUsed = true;//Disable the spawner as we spawn the Marker manually.
                NewSetMarkerEvent.Add(KeyFrame,
                                            new VisibleAnimationObjectKeyFrame(new Vector2(NewSpawner.MarkerViewer.ActiveMarker.Position.X, NewSpawner.MarkerViewer.ActiveMarker.Position.Y),
                                                                                true, -1));

                ReturnValue.Add(NewSetMarkerEvent);
            }

            return ReturnValue;
        }

        public override void SpawnItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame)
        {
            ActiveAnimation.OnMarkerTimelineSpawn(ActiveLayer, this);
        }

        public override void OnDeathFrame(AnimationClass ActiveAnimation)
        {
            ActiveAnimation.OnMarkerTimelineDeath(this);
        }

        public override void UpdateAnimationObject(int KeyFrame)
        {
            //An Event is being executed.
            if (NextEvent != null)
            {
                UpdateAnimationSprite(KeyFrame);
            }

            VisibleAnimationObjectKeyFrame ActiveKeyFrame;

            if (DicAnimationKeyFrame.TryGetValue(KeyFrame, out ActiveKeyFrame))
            {
                //If that animation has already been used, skip it.
                if (ActiveKeyFrame.IsUsed)
                    return;

                int NextKeyFrame = ActiveKeyFrame.NextKeyFrame;

                OnNewKeyFrameAnimationSprite(ActiveKeyFrame);

                if (DicAnimationKeyFrame.TryGetValue(NextKeyFrame, out ActiveKeyFrame))
                {
                    if (ActiveKeyFrame.IsProgressive)
                    {
                        OnProgressiveNextKeyFrameAnimationSprite(ActiveKeyFrame, KeyFrame, NextKeyFrame);
                    }
                    else
                    {
                        NextEvent = null;
                    }
                }
            }

            if (AnimationMarker != null)
            {
                AnimationMarker.UpdateKeyFrame(AnimationMarker.ActiveKeyFrame);
                AnimationMarker.ActiveKeyFrame++;

                if (AnimationMarker.LoopEnd > 0 && AnimationMarker.ActiveKeyFrame >= AnimationMarker.LoopEnd)
                {
                    AnimationMarker.ActiveKeyFrame = AnimationMarker.LoopStart;
                    AnimationMarker.CurrentQuote = "";
                    AnimationMarker.ListActiveSFX.Clear();

                    for (int L = 0; L < AnimationMarker.ListAnimationLayer.Count; L++)
                        AnimationMarker.ListAnimationLayer[L].ResetAnimationLayer();

                    AnimationMarker.UpdateKeyFrame(0);
                    if (AnimationMarker.ActiveKeyFrame > 0)
                        AnimationMarker.UpdateKeyFrame(AnimationMarker.ActiveKeyFrame);
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

        [Editor(typeof(MarkerSelector), typeof(UITypeEditor)),
        CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute(".")]
        public MarkerTimeline PlaceHolder
        {
            get
            {
                return this;
            }
            set
            {
                Sprite = value.Sprite;
                BitmapName = value.BitmapName;
            }
        }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public string MarkerType { get { return _MarkerType; } set { _MarkerType = value; } }

        public override int Width
        {
            get
            {
                if (Sprite == null)
                    return 32;
                else
                    return Sprite.Width;
            }
        }

        public override int Height
        {
            get
            {
                if (Sprite == null)
                    return 32;
                else
                    return Sprite.Height;
            }
        }

        #endregion
    }
}
