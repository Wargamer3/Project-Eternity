using System.IO;
using ProjectEternity.Core;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class ChargeAttackTimeline : TripleThunderTimeline
    {
        private const string TimelineType = "Charge Attack";

        private RobotAnimation Owner;
        private bool _ShootSecondaryWeapon;
        private int _MaxCharge;
        private int _ChargeAmountPerFrame;

        public ChargeAttackTimeline()
            : base(TimelineType, "Charge timeline")
        {
            Origin = new Point(Width / 2, Height / 2);
            _MaxCharge = 60;
            _ChargeAmountPerFrame = 1;
        }

        /// <summary>
        /// Called by reflection
        /// </summary>
        /// <param name="Owner"></param>
        public ChargeAttackTimeline(RobotAnimation Owner)
            : this()
        {
            this.Owner = Owner;
        }

        public ChargeAttackTimeline(BinaryReader BR, RobotAnimation Owner)
            : base(BR, TimelineType)
        {
            Origin = new Point(Width / 2, Height / 2);
            this.Owner = Owner;

            _SpawnFrame = BR.ReadInt32();
            _DeathFrame = BR.ReadInt32();
            _ShootSecondaryWeapon = BR.ReadBoolean();
            _MaxCharge = BR.ReadInt32();
            _ChargeAmountPerFrame = BR.ReadInt32();

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
            return new ChargeAttackTimeline(BR, Owner);
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(SpawnFrame);
            BW.Write(DeathFrame);
            BW.Write(_ShootSecondaryWeapon);
            BW.Write(_MaxCharge);
            BW.Write(_ChargeAmountPerFrame);

            BW.Write(DicAnimationKeyFrame.Count);
            foreach (KeyValuePair<int, VisibleAnimationObjectKeyFrame> KeyFrame in DicAnimationKeyFrame)
            {
                BW.Write(KeyFrame.Key);
                KeyFrame.Value.Save(BW);
            }
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            ChargeAttackTimeline NewTimeline = new ChargeAttackTimeline(Owner);

            NewTimeline._ShootSecondaryWeapon = _ShootSecondaryWeapon;
            NewTimeline.MaxCharge = MaxCharge;

            NewTimeline.UpdateFrom(this, ActiveLayer);

            return NewTimeline;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            ChargeAttackTimeline NewDamageTimeline = new ChargeAttackTimeline(Owner);
            NewDamageTimeline.Position = new Vector2(535, 170);
            NewDamageTimeline.SpawnFrame = KeyFrame;
            NewDamageTimeline.DeathFrame = KeyFrame + 10;
            NewDamageTimeline.IsUsed = true;//Disable the spawner as we spawn the Timeline manually.
            NewDamageTimeline.Add(KeyFrame, new VisibleAnimationObjectKeyFrame(new Vector2(NewDamageTimeline.Position.X, NewDamageTimeline.Position.Y),
                                                                        true, -1));

            ReturnValue.Add(NewDamageTimeline);

            return ReturnValue;
        }

        public override void UpdateAnimationObject(int KeyFrame)
        {
            if (Owner != null)
            {
                Owner.Charge(_ShootSecondaryWeapon, _MaxCharge, _ChargeAmountPerFrame);
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
        public int MaxCharge
        {
            get
            {
                return _MaxCharge;
            }
            set
            {
                _MaxCharge = value;
            }
        }

        [CategoryAttribute("Ranged Attack Attributes"),
        DescriptionAttribute(".")]
        public int ChargeAmountPerFrame
        {
            get
            {
                return _ChargeAmountPerFrame;
            }
            set
            {
                _ChargeAmountPerFrame = value;
            }
        }

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
