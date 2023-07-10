using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class DeathmatchDamageTimeline : DamageTimeline
    {
        protected override bool IsFlipped { get { return Owner != null && Owner.IsLeftAttacking; } }

        private AnimationScreen Owner;

        public DeathmatchDamageTimeline()
            : this(null, "New Damage", null)
        {
        }

        private DeathmatchDamageTimeline(AnimationScreen Owner, string Name, SpriteFont fntDamage)
            : base(Name, fntDamage)
        {
            this.Owner = Owner;
        }

        public DeathmatchDamageTimeline(AnimationScreen Owner, ContentManager Content)
            : this(Owner, "New Damage", Content.Load<SpriteFont>("Fonts/Battle Damage"))
        {
        }

        private DeathmatchDamageTimeline(BinaryReader BR, ContentManager Content)
            : base(BR, Content)
        {
        }

        protected override VisibleTimeline DoLoadCopy(BinaryReader BR, ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            DeathmatchDamageTimeline Copy = new DeathmatchDamageTimeline(BR, Content);
            Copy.Owner = Owner;
            return Copy;
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            DeathmatchDamageTimeline NewDamageTimeline = new DeathmatchDamageTimeline(Owner, Name, fntDamage);

            NewDamageTimeline.UpdateFrom(this, ActiveLayer);

            return NewDamageTimeline;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            DeathmatchDamageTimeline NewDamageTimeline = new DeathmatchDamageTimeline(Owner, ActiveAnimation.Content);
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
                Owner.DamageEnemyUnit(Damage);
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
    }
}
