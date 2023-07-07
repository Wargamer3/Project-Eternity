using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetDamageTimeline : DamageTimeline
    {
        private const string TimelineType = "Sorcerer Street Damage";

        protected override bool IsFlipped { get { return Owner != null && Owner.HorizontalMirror; } }

        private AnimationScreen Owner;

        public SorcererStreetDamageTimeline()
            : this(null, "New Damage", null)
        {
        }

        private SorcererStreetDamageTimeline(AnimationScreen Owner, string Name, SpriteFont fntDamage)
            : base(TimelineType, Name, fntDamage)
        {
            this.Owner = Owner;
        }

        public SorcererStreetDamageTimeline(AnimationScreen Owner, ContentManager Content)
            : this(Owner, "New Damage", Content.Load<SpriteFont>("Fonts/Battle Damage"))
        {
        }

        private SorcererStreetDamageTimeline(BinaryReader BR, ContentManager Content)
            : base(TimelineType, BR, Content)
        {
        }

        protected override VisibleTimeline DoLoadCopy(BinaryReader BR, ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            SorcererStreetDamageTimeline Copy = new SorcererStreetDamageTimeline(BR, Content);
            Copy.Owner = Owner;
            return Copy;
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            SorcererStreetDamageTimeline NewDamageTimeline = new SorcererStreetDamageTimeline(Owner, Name, fntDamage);

            NewDamageTimeline.UpdateFrom(this, ActiveLayer);

            return NewDamageTimeline;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            SorcererStreetDamageTimeline NewDamageTimeline = new SorcererStreetDamageTimeline(Owner, ActiveAnimation.Content);
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

        protected override void DoDamage(int Damage)
        {
            if (Owner != null)
            {
                Owner.DamageEnemyCreature(Damage);
            }
        }

        protected override int GetDamageFromText(string Text)
        {
            if (char.IsDigit(Text[0]))
                return Convert.ToInt32(Text);

            if (Owner != null)
            {
                switch (Text.ToLower())
                {
                    case "damage":
                        return 222;
                }
            }

            return 9999999;
        }
    }
}
