using System.IO;
using ProjectEternity.Core;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class GunNozzleTimeline : TripleThunderTimeline
    {
        private const string TimelineType = "Ranged Attack";

        private readonly RobotAnimation Owner;
        private bool _ShootSecondaryWeapon;
        private ComboWeapon OwnerWeapon;

        public GunNozzleTimeline()
            : base(TimelineType, "New Gun Nozzle")
        {
            Origin = new Point(Width / 2, Height / 2);
        }

        /// <summary>
        /// Called by reflection
        /// </summary>
        /// <param name="Owner"></param>
        public GunNozzleTimeline(RobotAnimation Owner)
            : this()
        {
            this.Owner = Owner;
            if (Owner != null)
            {
                this.OwnerWeapon = Owner.ActiveAddedWeapon;
            }
        }

        public GunNozzleTimeline(BinaryReader BR, RobotAnimation Owner)
            : base(BR, TimelineType)
        {
            Origin = new Point(Width / 2, Height / 2);
            this.Owner = Owner;
            if (Owner != null)
            {
                this.OwnerWeapon = Owner.ActiveAddedWeapon;
            }

            _SpawnFrame = BR.ReadInt32();
            _DeathFrame = BR.ReadInt32();
            _ShootSecondaryWeapon = BR.ReadBoolean();

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
            return new GunNozzleTimeline(BR, Owner);
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(SpawnFrame);
            BW.Write(DeathFrame);
            BW.Write(_ShootSecondaryWeapon);

            BW.Write(DicAnimationKeyFrame.Count);
            foreach (KeyValuePair<int, VisibleAnimationObjectKeyFrame> KeyFrame in DicAnimationKeyFrame)
            {
                BW.Write(KeyFrame.Key);
                KeyFrame.Value.Save(BW);
            }
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            GunNozzleTimeline NewSetMarkerEvent = new GunNozzleTimeline(Owner);

            NewSetMarkerEvent.UpdateFrom(this, ActiveLayer);

            return NewSetMarkerEvent;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            GunNozzleTimeline NewDamageTimeline = new GunNozzleTimeline(Owner);
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

                if (Owner != null)
                {
                    if (_ShootSecondaryWeapon)
                    {
                        Owner.Shoot(Position, _ShootSecondaryWeapon);
                    }
                    else if (OwnerWeapon != null)
                    {
                        int i = Owner.PrimaryWeapons.ActiveWeapons.IndexOf(OwnerWeapon);
                        Owner.Shoot(Position, OwnerWeapon, i);
                    }
                }

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

        [CategoryAttribute("Ranged Attack Attributes"),
        DescriptionAttribute(".")]
        public bool ShootSecondaryWeapon
        {
            get
            {
                return _ShootSecondaryWeapon;
            }
            set
            {
                _ShootSecondaryWeapon = value;
            }
        }

        #endregion
    }
}
