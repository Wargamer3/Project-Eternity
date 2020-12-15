using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Magic;
using ProjectEternity.GameScreens.AnimationScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using static ProjectEternity.Core.ProjectileParams;

namespace ProjectEternity.Units.Magic
{
    public class MagicTimeline : CoreTimeline, IProjectileSandbox
    {
        private const string TimelineType = "Magic";

        public override int Width => throw new NotImplementedException();

        public override int Height => throw new NotImplementedException();

        public List<Projectile> ListProjectile;
        private MagicSpell ActiveSpell;
        protected ProjectileContext GlobalContext;
        protected SharedProjectileParams SharedParams;

        public MagicTimeline()
            : base(TimelineType, "New Magic")
        {
            ListProjectile = new List<Projectile>();
        }

        public MagicTimeline(MagicSpell ActiveSpell, ProjectileContext GlobalContext, SharedProjectileParams SharedParams)
            : this()
        {
            this.ActiveSpell = ActiveSpell;
            this.GlobalContext = GlobalContext;
            this.SharedParams = SharedParams;

            GlobalContext.OwnerSandbox = this;
            this.ActiveSpell.ComputeSpell();
        }

        protected override VisibleTimeline DoLoadCopy(BinaryReader BR, ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            return new MagicTimeline();
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            MagicTimeline NewTimelineEvent = new MagicTimeline();

            NewTimelineEvent.UpdateFrom(this, ActiveLayer);

            return NewTimelineEvent;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            return ReturnValue;
        }

        public override void UpdateAnimationObject(int KeyFrame)
        {
            int Milliseconds = (int)(KeyFrame * (1000f / 60f));
            GameTime gameTime = new GameTime(new TimeSpan(0, 0, 0, 0, Milliseconds), new TimeSpan(0, 0, 0, 0, (int)(1000f / 60f)));
            for (int P = 0; P < ListProjectile.Count; P++)
            {
                ListProjectile[P].Update(gameTime);
            }
            
            ActiveSpell.ExecuteSpell();
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
        }

        public override void Draw(CustomSpriteBatch g, bool IsInEditMode)
        {
            foreach (Projectile ActiveProjectile in ListProjectile)
            {
                g.Draw(GameScreens.GameScreen.sprPixel, new Rectangle((int)ActiveProjectile.ListCollisionPolygon[0].ArrayVertex[0].X, (int)ActiveProjectile.ListCollisionPolygon[0].ArrayVertex[0].Y, 10, 10), Color.Red);
            }
        }

        public void AddProjectile(Projectile NewProjectile)
        {
            ListProjectile.Add(NewProjectile);
        }
    }
}
