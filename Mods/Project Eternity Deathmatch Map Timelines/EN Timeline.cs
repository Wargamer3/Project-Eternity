using Microsoft.Xna.Framework;
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
    public class ENTimeline : DeathmatchMapTimeline
    {
        private const string TimelineType = "EN Cost";

        public class ENKeyFrame : VisibleAnimationObjectKeyFrame
        {
            public string _EN;

            private ENKeyFrame()
            {
            }

            public ENKeyFrame(Vector2 NextPosition, bool IsProgressive, int NextKeyFrame)
                : base(NextPosition, IsProgressive, NextKeyFrame)
            {
                _EN = "EN";
            }

            public ENKeyFrame(BinaryReader BR)
                : base(BR)
            {
                _EN = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(_EN);
            }

            protected override VisibleAnimationObjectKeyFrame CopyAsVisibleAnimationObjectKeyFrame(AnimationClass.AnimationLayer ActiveLayer)
            {
                ENKeyFrame NewAnimatedBitmapKeyFrame = new ENKeyFrame();

                NewAnimatedBitmapKeyFrame.UpdateFrom(this);
                NewAnimatedBitmapKeyFrame._EN = _EN;

                return NewAnimatedBitmapKeyFrame;
            }

            #region Properties

            [CategoryAttribute("Animated Bitmap Attributes"),
            DescriptionAttribute(".")]
            public string Damage { get { return _EN; } set { _EN = value; } }

            #endregion
        }

        private double Progress;
        private SpriteFont fntDamage;
        private RenderTarget2D renderTarget;//Buffer used to render the AnimationLayer.
        private int ENOld;
        private int ENNext;
        private int ENCurrent;
        private double ENChangeValue;
        private int[] RoundTrip;
        private Vector2 TextSize;
        private AnimationScreen Owner;

        public ENTimeline()
            : this(null, "New EN Cost", null)
        {
        }

        private ENTimeline(AnimationScreen Owner, string Name, SpriteFont fntDamage)
            : base(TimelineType, Name)
        {
            this.Owner = Owner;
            this.Name = Name;
            this.fntDamage = fntDamage;

            Origin = new Point(0, 0);
        }

        public ENTimeline(AnimationScreen Owner, ContentManager Content)
            : this(Owner, "New EN Cost", Content.Load<SpriteFont>("Fonts/Finlander Font"))
        {
        }

        private ENTimeline(BinaryReader BR, ContentManager Content)
            : base(BR, TimelineType)
        {
            _SpawnFrame = BR.ReadInt32();
            _DeathFrame = BR.ReadInt32();

            fntDamage = Content.Load<SpriteFont>("Fonts/Finlander Font");

            int DicAnimationSpriteKeyFrameCount = BR.ReadInt32();
            for (int E = 0; E < DicAnimationSpriteKeyFrameCount; E++)
            {
                int Key = BR.ReadInt32();

                ENKeyFrame NewAnimatedBitmapKeyFrame = new ENKeyFrame(BR);

                DicAnimationKeyFrame.Add(Key, NewAnimatedBitmapKeyFrame);
            }
        }

        protected override VisibleTimeline DoLoadCopy(BinaryReader BR, ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            ENTimeline Copy = new ENTimeline(BR, Content);
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
            ENTimeline NewENTimeline = new ENTimeline(Owner, Name, fntDamage);

            NewENTimeline.UpdateFrom(this, ActiveLayer);

            return NewENTimeline;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            ENTimeline NewENTimeline = new ENTimeline(Owner, ActiveAnimation.Content);
            NewENTimeline.Position = new Vector2(535, 170);
            NewENTimeline.SpawnFrame = KeyFrame;
            NewENTimeline.DeathFrame = KeyFrame + 10;
            NewENTimeline.IsUsed = true;//Disable the spawner as we spawn the Marker manually.
            NewENTimeline.Add(KeyFrame,
                                    new ENKeyFrame(new Vector2(NewENTimeline.Position.X, NewENTimeline.Position.Y),
                                                                        true, -1));

            ReturnValue.Add(NewENTimeline);

            return ReturnValue;
        }

        public override void UpdateAnimationObject(int KeyFrame)
        {
            Progress = 0;
            //An Event is being executed.
            if (NextEvent != null)
            {
                Progress = (KeyFrame - EventKeyFrameOld) / (float)(NextEventKeyFrame - EventKeyFrameOld);
                ENCurrent = ENOld + (int)(ENChangeValue * Progress);
                if (Owner != null)
                {
                    Owner.ConsumeEN((int)ENChangeValue);
                }

                UpdateAnimationSprite(KeyFrame);
            }

            ENKeyFrame ActiveKeyFrame;
            VisibleAnimationObjectKeyFrame ActiveAnimationSpriteKeyFrame;

            if (DicAnimationKeyFrame.TryGetValue(KeyFrame, out ActiveAnimationSpriteKeyFrame))
            {
                ActiveKeyFrame = (ENKeyFrame)ActiveAnimationSpriteKeyFrame;
                //If that animation has already been used, skip it.
                if (ActiveKeyFrame.IsUsed)
                    return;

                int NextKeyFrame = ActiveKeyFrame.NextKeyFrame;
                OnNewKeyFrameAnimationSprite(ActiveKeyFrame);

                ComputeRealDamage(ActiveKeyFrame);

                if (DicAnimationKeyFrame.TryGetValue(NextKeyFrame, out ActiveAnimationSpriteKeyFrame))
                {
                    ENKeyFrame NextActiveKeyFrame = (ENKeyFrame)ActiveAnimationSpriteKeyFrame;
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
                    Owner.ConsumeEN(ENCurrent);
                }
            }
        }

        private int ComputeDamage(ENKeyFrame KeyFrame)
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
                    case "en":
                        return Owner.AttackingSquad.CurrentLeader.EN - Owner.BattleResult.ArrayResult[0].AttackAttackerFinalEN;

                    case "wingman a":
                    case "wingman 1":
                        return Owner.AttackingSquad.CurrentWingmanA.EN - Owner.BattleResult.ArrayResult[1].AttackAttackerFinalEN;

                    case "wingman b":
                    case "wingman 2":
                        return Owner.AttackingSquad.CurrentWingmanB.EN - Owner.BattleResult.ArrayResult[2].AttackAttackerFinalEN;
                }
            }

            return 9999999;
        }

        public void ComputeRealDamage(ENKeyFrame KeyFrame)
        {
            ENOld = ENCurrent;

            ENCurrent = ComputeDamage(KeyFrame);

            ENNext = 0;
            TextSize = fntDamage.MeasureString("EN-" + ENCurrent.ToString());
        }

        public void ComputeNextDamage(ENKeyFrame KeyFrame)
        {
            ENNext = ComputeDamage(KeyFrame);
            TextSize = fntDamage.MeasureString("EN-" + ENNext.ToString());
            int DamageDifference = ENNext - ENCurrent;
            ENChangeValue = DamageDifference;
            string DamageString = ENNext.ToString();

            RoundTrip = new int[Math.Max(DamageString.Length, ENCurrent.ToString().Length)];

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
            if (ENNext == 0)
            {
                g.DrawString(fntDamage, "EN-" + ENCurrent.ToString(), new Vector2(0, 0), Color.White);
            }
            else
            {
                g.DrawString(fntDamage, "EN-", new Vector2(0, 0), Color.White);

                int DamageChangeSign = Math.Sign(ENCurrent - ENOld);
                string DamageString = ENCurrent.ToString();
                string DamageOldString = ENOld.ToString().PadLeft(DamageString.Length, '0');
                string DrawnString = "";
                char CurrentChar;

                for (int i = DamageString.Length - 1; i >= 0; --i)
                {
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
