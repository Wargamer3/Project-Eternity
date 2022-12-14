using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core;
using ProjectEternity.Core.Magic;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.AnimationScreen;
using static ProjectEternity.Core.Projectile2DParams;

namespace ProjectEternity.Units.Magic
{
    public class MagicTimeline : CoreTimeline, IProjectile2DSandbox
    {
        private const string TimelineType = "Magic";

        public override int Width => throw new NotImplementedException();

        public override int Height => throw new NotImplementedException();

        public List<Projectile2D> ListProjectile;
        private MagicSpell ActiveSpell;
        protected Projectile2DContext GlobalContext;
        protected SharedProjectileParams SharedParams;

        public MagicTimeline()
            : base(TimelineType, "New Magic")
        {
            ListProjectile = new List<Projectile2D>();
        }

        public MagicTimeline(MagicSpell ActiveSpell, Projectile2DContext GlobalContext, SharedProjectileParams SharedParams)
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
            foreach (Projectile2D ActiveProjectile in ListProjectile)
            {
                g.Draw(GameScreens.GameScreen.sprPixel, new Rectangle((int)ActiveProjectile.Collision.ListCollisionPolygon[0].ArrayVertex[0].X, (int)ActiveProjectile.Collision.ListCollisionPolygon[0].ArrayVertex[0].Y, 10, 10), Color.Red);
            }
        }

        public void AddProjectile(Projectile2D NewProjectile)
        {
            ListProjectile.Add(NewProjectile);
        }
    }
}
