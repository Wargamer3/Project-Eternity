using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.ControlHelper;
using static ProjectEternity.Core.Projectile2DParams;
using ProjectEternity.Core.Online;

namespace ProjectEternity.Core.Magic
{
    class MagicPreviewerPanel : ActionPanel, IProjectile2DSandbox
    {
        private readonly Texture2D sprPixel;

        private readonly Polygon SandboxCollisionBox;
        private readonly Polygon EnemyCollisionBox;
        private readonly List<Projectile2D> ListProjectile;
        private readonly List<BaseAutomaticSkill> ListMagicSpell;
        private int RestartButtonX = 5;
        private int RestartButtonY = 50;
        private int CloseButtonX = Constants.Width - 100;
        private int CloseButtonY = 50;

        public MagicPreviewerPanel(ContentManager Content, MagicSpell ActiveMagicSpell, ActionPanelHolder ListActionMenuChoice, Projectile2DContext GlobalProjectileContext, SharedProjectileParams SharedParams)
            : this(Content, ActiveMagicSpell, ListActionMenuChoice, GlobalProjectileContext, SharedParams,
                  new Vector2(Constants.Width - 50, Constants.Height / 2),
                  new Rectangle(50, Constants.Height / 2, 100, 100))
        {
        }

        public MagicPreviewerPanel(ContentManager Content, MagicSpell ActiveMagicSpell, ActionPanelHolder ListActionMenuChoice, Projectile2DContext GlobalProjectileContext, SharedProjectileParams SharedParams, Vector2 UserPosition, Rectangle EnemyBounds)
            : base("Previewer", ListActionMenuChoice, true)
        {
            sprPixel = Content.Load<Texture2D>("pixel");

            ListProjectile = new List<Projectile2D>();

            Vector2[] SandboxPoints = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(0, Constants.Height),
                new Vector2(Constants.Width, Constants.Height),
                new Vector2(Constants.Width, 0),
            };
            SandboxCollisionBox = new Polygon(SandboxPoints, Constants.Width, Constants.Height);

            Vector2[] EnemyPoints = new Vector2[4]
            {
                new Vector2(EnemyBounds.X, EnemyBounds.Y),
                new Vector2(EnemyBounds.X, EnemyBounds.Bottom),
                new Vector2(EnemyBounds.Right, EnemyBounds.Bottom),
                new Vector2(EnemyBounds.Right, EnemyBounds.Y),
            };
            EnemyCollisionBox = new Polygon(EnemyPoints, Constants.Width, Constants.Height);

            MagicUser ActiveUser = new MagicUser();
            ActiveUser.ManaReserves = 1000;
            ActiveUser.CurrentMana = 100;

            SharedParams.OwnerPosition = UserPosition;
            GlobalProjectileContext.OwnerSandbox = this;

            MagicSpell NewSpell = new MagicSpell(ActiveMagicSpell, ActiveUser);
            ListMagicSpell = NewSpell.ComputeSpell();
        }

        public void AddProjectile(Projectile2D NewProjectile)
        {
            ListProjectile.Add(NewProjectile);
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            for (int S = 0; S < ListMagicSpell.Count; ++S)
            {
                ListMagicSpell[S].AddSkillEffectsToTarget(string.Empty);
            }

            for (int P = 0; P < ListProjectile.Count; P++)
            {
                Projectile2D ActiveProjectile = ListProjectile[P];

                if (ActiveProjectile.IsAlive)
                {
                    ActiveProjectile.Update(gameTime);
                    foreach (Polygon ProjectileCollision in ActiveProjectile.Collision.ListCollisionPolygon)
                    {
                        //Out of bound
                        if (Polygon.PolygonCollisionSAT(SandboxCollisionBox, ProjectileCollision, Vector2.Zero).Distance < 0
                            || Polygon.PolygonCollisionSAT(EnemyCollisionBox, ProjectileCollision, Vector2.Zero).Distance >= 0)
                        {
                            ActiveProjectile.IsAlive = false;
                        }
                    }
                }
            }

            if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                ListActionMenuChoice.Remove(this);
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            throw new NotImplementedException();
        }

        public override void DoWrite(ByteWriter BW)
        {
            throw new NotImplementedException();
        }

        protected override ActionPanel Copy()
        {
            throw new NotImplementedException();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            for (int P = 0; P < ListProjectile.Count; ++P)
            {
                ListProjectile[P].Draw(g);
            }

            g.DrawLine(sprPixel, SandboxCollisionBox.ArrayVertex[0], SandboxCollisionBox.ArrayVertex[1], Color.Black);
            g.DrawLine(sprPixel, SandboxCollisionBox.ArrayVertex[1], SandboxCollisionBox.ArrayVertex[2], Color.Black);
            g.DrawLine(sprPixel, SandboxCollisionBox.ArrayVertex[2], SandboxCollisionBox.ArrayVertex[3], Color.Black);
            g.DrawLine(sprPixel, SandboxCollisionBox.ArrayVertex[3], SandboxCollisionBox.ArrayVertex[0], Color.Black);

            g.DrawLine(sprPixel, EnemyCollisionBox.ArrayVertex[0], SandboxCollisionBox.ArrayVertex[1], Color.Red);
            g.DrawLine(sprPixel, EnemyCollisionBox.ArrayVertex[1], SandboxCollisionBox.ArrayVertex[2], Color.Red);
            g.DrawLine(sprPixel, EnemyCollisionBox.ArrayVertex[2], SandboxCollisionBox.ArrayVertex[3], Color.Red);
            g.DrawLine(sprPixel, EnemyCollisionBox.ArrayVertex[3], SandboxCollisionBox.ArrayVertex[0], Color.Red);

            g.Draw(sprPixel, new Rectangle(RestartButtonX, RestartButtonY, 30, 30), Color.Green);

            g.Draw(sprPixel, new Rectangle(CloseButtonX, CloseButtonY, 30, 30), Color.Red);
        }
    }
}
