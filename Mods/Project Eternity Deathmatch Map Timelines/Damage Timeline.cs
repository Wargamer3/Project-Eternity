﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class DamageTimeline : DeathmatchMapTimeline
    {
        private const string TimelineType = "Damage";

        public class DamageKeyFrame : VisibleAnimationObjectKeyFrame
        {
            public string _Damage;

            private DamageKeyFrame()
            {
            }

            public DamageKeyFrame(Vector2 NextPosition, bool IsProgressive, int NextKeyFrame)
                : base(NextPosition, IsProgressive, NextKeyFrame)
            {
                _Damage = "Damage";
            }

            public DamageKeyFrame(BinaryReader BR)
                : base(BR)
            {
                _Damage = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(_Damage);
            }

            protected override VisibleAnimationObjectKeyFrame CopyAsVisibleAnimationObjectKeyFrame(AnimationClass.AnimationLayer ActiveLayer)
            {
                DamageKeyFrame NewAnimatedBitmapKeyFrame = new DamageKeyFrame();

                NewAnimatedBitmapKeyFrame.UpdateFrom(this);
                NewAnimatedBitmapKeyFrame._Damage = _Damage;

                return NewAnimatedBitmapKeyFrame;
            }

            #region Properties

            [CategoryAttribute("Animated Bitmap Attributes"),
            DescriptionAttribute(".")]
            public string Damage { get { return _Damage; } set { _Damage = value; } }

            #endregion
        }

        private double Progress;
        private SpriteFont fntDamage;
        private RenderTarget2D renderTarget;//Buffer used to render the AnimationLayer.
        private int DamageOld;
        private int DamageNext;
        private int DamageCurrent;
        private double DamageChangeValue;
        private int[] RoundTrip;
        private Vector2 TextSize;
        private AnimationScreen Owner;

        public DamageTimeline()
            : this(null, "New Damage", null)
        {
        }

        private DamageTimeline(AnimationScreen Owner, string Name, SpriteFont fntDamage)
            : base(TimelineType, Name)
        {
            this.Owner = Owner;
            this.Name = Name;
            this.fntDamage = fntDamage;

            Origin = new Point(0, 0);
        }

        public DamageTimeline(AnimationScreen Owner, ContentManager Content)
            : this(Owner, "New Damage", Content.Load<SpriteFont>("Fonts/Battle Damage"))
        {
        }

        private DamageTimeline(BinaryReader BR, ContentManager Content)
            : base(BR, TimelineType)
        {
            _SpawnFrame = BR.ReadInt32();
            _DeathFrame = BR.ReadInt32();

            fntDamage = Content.Load<SpriteFont>("Fonts/Battle Damage");
            fntDamage.Spacing = -5;

            int DicAnimationSpriteKeyFrameCount = BR.ReadInt32();
            for (int E = 0; E < DicAnimationSpriteKeyFrameCount; E++)
            {
                int Key = BR.ReadInt32();

                DamageKeyFrame NewAnimatedBitmapKeyFrame = new DamageKeyFrame(BR);

                DicAnimationKeyFrame.Add(Key, NewAnimatedBitmapKeyFrame);
            }
        }

        protected override VisibleTimeline DoLoadCopy(BinaryReader BR, ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            DamageTimeline Copy = new DamageTimeline(BR, Content);
            Copy.Owner = Owner;
            return Copy;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(SpawnFrame);
            BW.Write(DeathFrame);

            BW.Write(DicAnimationKeyFrame.Count);
            foreach (KeyValuePair<int, VisibleAnimationObjectKeyFrame> KeyFrame in DicAnimationKeyFrame)
            {
                BW.Write(KeyFrame.Key);
                KeyFrame.Value.Save(BW);
            }
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            DamageTimeline NewDamageTimeline = new DamageTimeline(Owner, Name, fntDamage);

            NewDamageTimeline.UpdateFrom(this, ActiveLayer);

            return NewDamageTimeline;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            DamageTimeline NewDamageTimeline = new DamageTimeline(Owner, ActiveAnimation.Content);
            NewDamageTimeline.Position = new Vector2(535, 170);
            NewDamageTimeline.SpawnFrame = KeyFrame;
            NewDamageTimeline.DeathFrame = KeyFrame + 10;
            NewDamageTimeline.IsUsed = true;//Disable the spawner as we spawn the Marker manually.
            NewDamageTimeline.Add(KeyFrame,
                                    new DamageKeyFrame(new Vector2(NewDamageTimeline.Position.X, NewDamageTimeline.Position.Y),
                                                                        true, -1));

            ReturnValue.Add(NewDamageTimeline);

            return ReturnValue;
        }

        public override void UpdateAnimationObject(int KeyFrame)
        {
            Progress = 0;
            //An Event is being executed.
            if (NextEvent != null)
            {
                Progress = (KeyFrame - EventKeyFrameOld) / (float)(NextEventKeyFrame - EventKeyFrameOld);
                DamageCurrent = DamageOld + (int)(DamageChangeValue * Progress);
                if (Owner != null)
                {
                    Owner.DamageEnemyUnit((int)DamageChangeValue);
                }

                UpdateAnimationSprite(KeyFrame);
            }

            DamageKeyFrame ActiveKeyFrame;
            VisibleAnimationObjectKeyFrame ActiveAnimationSpriteKeyFrame;

            if (DicAnimationKeyFrame.TryGetValue(KeyFrame, out ActiveAnimationSpriteKeyFrame))
            {
                ActiveKeyFrame = (DamageKeyFrame)ActiveAnimationSpriteKeyFrame;
                //If that animation has already been used, skip it.
                if (ActiveKeyFrame.IsUsed)
                    return;

                int NextKeyFrame = ActiveKeyFrame.NextKeyFrame;
                OnNewKeyFrameAnimationSprite(ActiveKeyFrame);

                ComputeRealDamage(ActiveKeyFrame);

                if (DicAnimationKeyFrame.TryGetValue(NextKeyFrame, out ActiveAnimationSpriteKeyFrame))
                {
                    DamageKeyFrame NextActiveKeyFrame = (DamageKeyFrame)ActiveAnimationSpriteKeyFrame;
                    if (NextActiveKeyFrame.IsProgressive)
                    {
                        OnProgressiveNextKeyFrameAnimationSprite(NextActiveKeyFrame, KeyFrame, NextKeyFrame);

                        ComputeNextDamage(NextActiveKeyFrame);
                    }
                    else
                        NextEvent = null;
                }
                else if (Owner != null && (!ActiveKeyFrame.IsProgressive || KeyFrame == SpawnFrame))
                {
                    Owner.DamageEnemyUnit(DamageCurrent);
                }
            }
        }

        private int ComputeDamage(DamageKeyFrame KeyFrame)
        {
            int DamageDealt = GetDamageFromText(KeyFrame.Damage);

            string[] DamageSplit = KeyFrame.Damage.Split('/');
            if (DamageSplit.Length == 1)
            {
                return DamageDealt;
            }
            else if (DamageSplit.Length == 2)
            {
                return (int)(DamageDealt / Convert.ToDouble(DamageSplit[1], System.Globalization.CultureInfo.InvariantCulture));
            }

            return 0;
        }

        private int GetDamageFromText(string Text)
        {
            if (char.IsDigit(Text[0]))
                return Convert.ToInt32(Text);

            if (Owner != null)
            {
                switch (Text.ToLower())
                {
                    case "leader":
                    case "damage":
                        return Owner.BattleResult.ArrayResult[0].AttackDamage;

                    case "wingman a":
                    case "wingman 1":
                        return Owner.BattleResult.ArrayResult[1].AttackDamage;

                    case "wingman b":
                    case "wingman 2":
                        return Owner.BattleResult.ArrayResult[2].AttackDamage;
                }
            }

            return 9999999;
        }

        public void ComputeRealDamage(DamageKeyFrame KeyFrame)
        {
            DamageOld = DamageCurrent = ComputeDamage(KeyFrame);

            DamageNext = 0;
            TextSize = fntDamage.MeasureString(DamageCurrent.ToString());
        }

        public void ComputeNextDamage(DamageKeyFrame KeyFrame)
        {
            DamageNext = ComputeDamage(KeyFrame);
            TextSize = fntDamage.MeasureString(DamageNext.ToString());
            int DamageDifference = DamageNext - DamageCurrent;
            DamageChangeValue = DamageDifference;
            string DamageString = DamageNext.ToString();

            RoundTrip = new int[Math.Max(DamageString.Length, DamageCurrent.ToString().Length)];

            for (int i = DamageString.Length - 1; i >= 0; --i)
            {
                if (DamageDifference > 0)
                {
                    RoundTrip[i] = DamageDifference;
                    DamageDifference /= 10;
                }
                else
                {
                    RoundTrip[i] = 0;
                }
            }
        }

        private void DrawDamage(CustomSpriteBatch g)
        {
            if (DamageNext == 0)
            {
                g.DrawString(fntDamage, DamageCurrent.ToString(), new Vector2(0, 0), Color.White);
            }
            else
            {
                int DamageChangeSign = Math.Sign(DamageCurrent - DamageOld);
                string DamageString = DamageCurrent.ToString();
                string DamageOldString = DamageOld.ToString().PadLeft(DamageString.Length, '0');
                string DrawnString = "";
                char CurrentChar;

                for (int i = DamageString.Length - 1; i >= 0; --i)
                {
                    if (i >= RoundTrip.Length)
                    {
                        continue;
                    }
                    int CurrentRoundTrip = RoundTrip[i];
                    if (CurrentRoundTrip == 0)
                    {
                        DrawnString += DamageString[i];

                        g.DrawString(fntDamage, DamageString[i].ToString(),
                            new Vector2(TextSize.X - fntDamage.MeasureString(DrawnString).X, 0),
                            Color.White);

                        continue;
                    }
                    CurrentChar = DamageOldString[i];
                    DrawnString += CurrentChar;

                    int CurrentNumber = CurrentChar - '0';
                    double FinalNumber = CurrentNumber + (CurrentRoundTrip * Progress);
                    double Decimal = FinalNumber - Math.Truncate(FinalNumber);
                    int DrawnNumber = (int)(Math.Truncate(FinalNumber)) % 10;
                    int DrawnNumberNext = DrawnNumber + DamageChangeSign;

                    if (DrawnNumberNext > 9)
                        DrawnNumberNext = 0;
                    else if (DrawnNumberNext < 0)
                        DrawnNumberNext = 9;

                    // First number is 0 changing to 1, don't draw it.
                    if (i == 0 && DrawnNumber == 0)
                    {
                        continue;
                    }

                    g.DrawString(fntDamage, DrawnNumber.ToString(),
                        new Vector2(TextSize.X - fntDamage.MeasureString(DrawnString).X, (float)(-Decimal * TextSize.Y)),
                        Color.White);

                    g.DrawString(fntDamage, DrawnNumberNext.ToString(),
                        new Vector2(TextSize.X - fntDamage.MeasureString(DrawnString).X, (float)(TextSize.Y - Decimal * TextSize.Y)),
                        Color.White);
                }
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            if (TextSize.X == 0 || TextSize.Y == 0)
                return;

            if (renderTarget == null ||
                renderTarget.Width != (int)TextSize.X ||
                renderTarget.Height != (int)TextSize.Y)
            {
                renderTarget = new RenderTarget2D(
                    GameScreen.GraphicsDevice,
                    (int)TextSize.X,
                    (int)TextSize.Y);
            }
            GameScreen.GraphicsDevice.SetRenderTarget(renderTarget);
            GameScreen.GraphicsDevice.Clear(Color.Transparent);
            DrawDamage(g);
        }

        public override void Draw(CustomSpriteBatch g, bool IsInEditMode)
        {
            if (TextSize.X == 0 || TextSize.Y == 0)
                return;

            SpriteEffects ActiveEffect = SpriteEffects.None;
            if (ScaleFactor.X < 0 || (Owner != null && Owner.IsLeftAttacking))
                ActiveEffect = SpriteEffects.FlipHorizontally;
            if (ScaleFactor.Y < 0)
                ActiveEffect |= SpriteEffects.FlipVertically;

            g.Draw(renderTarget,
                Position, null,
                Color.FromNonPremultiplied(255, 255, 255, Alpha),
                Angle, new Vector2(Origin.X, Origin.Y),
                new Vector2(Math.Abs(ScaleFactor.X), Math.Abs(ScaleFactor.Y)), ActiveEffect, DrawingDepth);
        }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public override int Width { get { return (int)TextSize.X; } }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public override int Height { get { return (int)TextSize.Y; } }
    }
}
