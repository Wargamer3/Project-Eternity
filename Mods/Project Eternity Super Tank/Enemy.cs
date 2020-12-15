using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    public abstract class Enemy : AnimatedSprite
    {
        public static List<Enemy> ListEnemy = new List<Enemy>();
        public static void AddEnemy(Enemy NewEnemy)
        {
            for (int P = 0; P < Enemy.ListEnemy.Count; P++)
            {
                if (!Enemy.ListEnemy[P].IsAlive)
                {
                    Enemy.ListEnemy[P] = NewEnemy;
                    return;
                }
            }

            Enemy.ListEnemy.Add(NewEnemy);
        }

        public Vector2 Speed;

        public float Resist;
        public bool IsAlive;

        protected Enemy()
        {
        }

		protected Enemy(int Resist, AnimatedSprite Clone, Color[] Mask)
            : base(Clone.ActiveSprite, Mask, Clone.AnimationFrameCount, Clone.Origin, Clone.AnimationSpeed)
        {
            this.Resist = Resist;
            IsAlive = true;
        }

        public void Direction(float SpeedX)
        {
            if (SuperTank2.Randomizer.Next(2) == 1)
            {
                Scale.X = 1f;
                Position.X = Constants.Width + SpriteWidth;
                Position.Y = 50 + SuperTank2.Randomizer.Next(45) * 10;
                Speed = new Vector2(-SpeedX, 0);
            }
            else
            {
                Scale.X = -1f;
                Position.X = -SpriteWidth;
                Position.Y = 50 + SuperTank2.Randomizer.Next(45) * 10;
                Speed = new Vector2(SpeedX, 0);

				Origin.X = ActiveSprite.Width - Origin.X;
            }
        }

        public virtual void Update(float TimeEllapsed)
        {
            Position += Speed * TimeEllapsed;

            UpdateTransformationMatrix();

            if ((Speed.X < 0 && Position.X < -SpriteWidth) || (Speed.X > 0 && Position.X > Constants.Width + SpriteWidth))
            {
                Resist = 0;
                IsAlive = false;
                Destroyed(null);
            }
        }

        public abstract void Destroyed(Vehicule Destroyed);

        public virtual void Draw(CustomSpriteBatch g)
        {
            if (Scale.X < 0)
				g.Draw(ActiveSprite, Position, SpriteSource, Color.White, 0, Origin, 1, SpriteEffects.FlipHorizontally, 0);
            else
                g.Draw(ActiveSprite, Position, SpriteSource, Color.White, 0, Origin, 1, SpriteEffects.None, 0);
        }
    }

    public class Plane1 : Enemy
    {
        public static AnimatedSprite PlaneSprite;
        public static Color[] PlaneMask;
        public float BombTime;
        public float BombTimeMax;

        public Plane1(int Resist, int BombTimeMax)
            : base(Resist, PlaneSprite, PlaneMask)
        {
            Direction(8);
            this.BombTimeMax = BombTimeMax;
            BombTime = BombTimeMax;
        }

        public override void Update(float TimeEllapsed)
        {
            --BombTime;

            if (BombTime <= 0)
            {
                SpawnBomb();
                BombTime = BombTimeMax;
            }

            base.Update(TimeEllapsed);

            new Propulsor(Position, new Vector2(Math.Sign(Scale.X) * -SuperTank2.Randomizer.Next(2), 0)).AddParticule();
            new Propulsor(Position, new Vector2(Math.Sign(Scale.X) * -SuperTank2.Randomizer.Next(2), 0)).AddParticule();
        }

        public override void Destroyed(Vehicule Destroyer)
        {
            IsAlive = false;

            if (Destroyer == null)
                return;

            for (int i = 0; i < 50; i++)
            {
                new FlameParticle(Position, SuperTank2.Randomizer.Next(360) * SuperTank2.DegToRad, SuperTank2.Randomizer.NextDouble() * 5).AddParticule();
            }

            if (SuperTank2.Randomizer.Next(10 - ((int)SuperTank2.Difficulty * 4)) == 1)
                PowerUp.AddPowerUp(new PowerUp(new Vector2(Position.X, Position.Y), PowerUp.ArrayPowerUpChoice[SuperTank2.Randomizer.Next(18)]));

            Explosion.AddExplosion(new Explosion1(Position, 0, 0));

            for (int i = 0; i < 4; i++)
                Smoke.AddSmoke(new PlaneSmoke(Position, (float)(Math.Sign(Speed.X) * Math.PI - 10 * SuperTank2.DegToRad + SuperTank2.Randomizer.Next(20) * SuperTank2.DegToRad), 1 + SuperTank2.Randomizer.Next(4)));

            if (SuperTank2.Difficulty == DifficultyChoices.Normal)
            {
                Destroyer.Points += 20;
                Destroyer.Argent += 10;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
            {
                Destroyer.Points += 30;
                Destroyer.Argent += 15;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
            {
                Destroyer.Points += 40;
                Destroyer.Argent += 20;
            }
        }

        public void SpawnBomb()
        {
            Vector2 BombSpeed = new Vector2(Math.Sign(Speed.X) * 2, 0);

            int BombResist = 0;
            int BombDamage = 0;
            if (SuperTank2.Difficulty == DifficultyChoices.Normal)
            {
                BombResist = 1;
                BombDamage = 1;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
            {
                BombResist = 2;
                BombDamage = 2;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
            {
                BombResist = 3;
                BombDamage = 3;
            }

            EnemyAttack.AddEnemyAttack(new Bomb1(Position, BombSpeed, BombResist, BombDamage));
        }
    }

    public class Plane2 : Enemy
    {
        public static AnimatedSprite PlaneSprite;
        public static Color[] PlaneMask;
        public float BombTime;
        public float BombTimeMax;

        public Plane2(int Resist, int BombTimeMax)
            : base(Resist, PlaneSprite, PlaneMask)
        {
            Direction(3);
            this.BombTimeMax = BombTimeMax;
            BombTime = BombTimeMax;
        }

        public override void Update(float TimeEllapsed)
        {
            --BombTime;

            if (BombTime <= 0)
            {
                SpawnBomb();
                BombTime = BombTimeMax;
            }

            base.Update(TimeEllapsed);

            new Propulsor(Position, new Vector2(Math.Sign(Scale.X) * -SuperTank2.Randomizer.Next(2), 0)).AddParticule();
            new Propulsor(Position, new Vector2(Math.Sign(Scale.X) * -SuperTank2.Randomizer.Next(2), 0)).AddParticule();
        }

        public override void Destroyed(Vehicule Destroyer)
        {
            IsAlive = false;

            if (Destroyer == null)
                return;

            for (int i = 0; i < 10; i++)
            {
                new FlameParticle(Position, SuperTank2.Randomizer.Next(360) * SuperTank2.DegToRad, SuperTank2.Randomizer.NextDouble() * 5).AddParticule();
            }

            if (SuperTank2.Randomizer.Next(10 - ((int)SuperTank2.Difficulty * 4)) == 1)
                PowerUp.AddPowerUp(new PowerUp(new Vector2(Position.X, Position.Y), PowerUp.ArrayPowerUpChoice[SuperTank2.Randomizer.Next(18)]));

            Explosion.AddExplosion(new Explosion1(Position, 0, 0));

            for (int i = 0; i < 4; i++)
                Smoke.AddSmoke(new PlaneSmoke(Position, (float)(Math.Sign(Speed.X) * Math.PI - 10 * SuperTank2.DegToRad + SuperTank2.Randomizer.Next(20) * SuperTank2.DegToRad), 1 + SuperTank2.Randomizer.Next(4)));

            if (SuperTank2.Difficulty == DifficultyChoices.Normal)
            {
                Destroyer.Points += 20;
                Destroyer.Argent += 10;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
            {
                Destroyer.Points += 30;
                Destroyer.Argent += 15;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
            {
                Destroyer.Points += 40;
                Destroyer.Argent += 20;
            }
        }

        public void SpawnBomb()
        {
            Vector2 BombSpeed = new Vector2(Math.Sign(Speed.X) * 2, 0);

            int BombResist = 0;
            int BombDamage = 0;
            if (SuperTank2.Difficulty == DifficultyChoices.Normal)
            {
                BombResist = 1;
                BombDamage = 1;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
            {
                BombResist = 2;
                BombDamage = 2;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
            {
                BombResist = 3;
                BombDamage = 3;
            }

            EnemyAttack.AddEnemyAttack(new Bomb1(Position, BombSpeed, BombResist, BombDamage));
        }
    }

    public class Plane3 : Enemy
    {
        public static AnimatedSprite PlaneSprite;
        public static Color[] PlaneMask;
        public float BombTime;
        public float BombTimeMax;

        public Plane3(int Resist, int BombTimeMax)
            : base(Resist, PlaneSprite, PlaneMask)
        {
            Direction(8);
            this.BombTimeMax = BombTimeMax;
            BombTime = BombTimeMax;
        }

        public override void Update(float TimeEllapsed)
        {
            --BombTime;

            if (BombTime <= 0)
            {
                SpawnBomb();
                BombTime = BombTimeMax;
            }

            base.Update(TimeEllapsed);

            new Propulsor(Position, new Vector2(Math.Sign(Scale.X) * -SuperTank2.Randomizer.Next(2), 0)).AddParticule();
            new Propulsor(Position, new Vector2(Math.Sign(Scale.X) * -SuperTank2.Randomizer.Next(2), 0)).AddParticule();
        }

        public override void Destroyed(Vehicule Destroyer)
        {
            IsAlive = false;

            if (Destroyer == null)
                return;

            for (int i = 0; i < 10; i++)
            {
                new FlameParticle(Position, SuperTank2.Randomizer.Next(360) * SuperTank2.DegToRad, SuperTank2.Randomizer.NextDouble() * 5).AddParticule();
            }

            if (SuperTank2.Randomizer.Next(10 - ((int)SuperTank2.Difficulty * 4)) == 1)
                PowerUp.AddPowerUp(new PowerUp(new Vector2(Position.X, Position.Y), PowerUp.ArrayPowerUpChoice[SuperTank2.Randomizer.Next(18)]));

            Explosion.AddExplosion(new Explosion1(Position, 0, 0));

            for (int i = 0; i < 4; i++)
                Smoke.AddSmoke(new PlaneSmoke(Position, (float)(Math.Sign(Speed.X) * Math.PI - 10 * SuperTank2.DegToRad + SuperTank2.Randomizer.Next(20) * SuperTank2.DegToRad), 1 + SuperTank2.Randomizer.Next(4)));

            if (SuperTank2.Difficulty == DifficultyChoices.Normal)
            {
                Destroyer.Points += 20;
                Destroyer.Argent += 10;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
            {
                Destroyer.Points += 30;
                Destroyer.Argent += 15;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
            {
                Destroyer.Points += 40;
                Destroyer.Argent += 20;
            }
        }

        public void SpawnBomb()
        {
            Vector2 BombSpeed = new Vector2(Math.Sign(Speed.X) * 2, 0);

            int BombResist = 0;
            int BombDamage = 0;
            if (SuperTank2.Difficulty == DifficultyChoices.Normal)
            {
                BombResist = 3;
                BombDamage = 3;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
            {
                BombResist = 6;
                BombDamage = 6;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
            {
                BombResist = 8;
                BombDamage = 8;
            }

            EnemyAttack.AddEnemyAttack(new Bomb2(Position, BombSpeed, BombResist, BombDamage));
        }
    }

    public class Plane4 : Enemy
    {
        public class LaserWeapon : EnemyAttack
        {
            float AnimationPosition;

            public LaserWeapon(Vector2 Position)
                : base(Position, Vector2.Zero, 1, 1, Plane4.sprLaser, Plane4.MaskLaser)
            {
                AttackType = AttackTypes.Laser;
                Scale.Y = 720 - Position.Y + 40;
                Angle = 0;
                AnimationPosition = 0;

                if (SuperTank2.Difficulty == DifficultyChoices.Normal)
                {
                    Damage = 0.2f;
                }
                else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
                {
                    Damage = 0.3f;
                }
                else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
                {
                    Damage = 0.4f;
                }
            }

            public override void Update(float TimeEllapsed)
            {
                AnimationPosition += 0.5f;
                if (AnimationPosition >= sprLaserBottom.AnimationFrameCount)
                    AnimationPosition -= sprLaserBottom.AnimationFrameCount;

                UpdateTransformationMatrix(Scale);
            }

            public override void Draw(CustomSpriteBatch g)
            {
                base.Draw(g);
                sprLaserBottom.Draw(g, (int)Math.Floor(AnimationPosition), new Vector2(Position.X, Position.Y + Scale.Y), 0, Color.White);
            }

            public override void Destroyed(Vehicule Destroyer)
            {
                throw new NotImplementedException();
            }
        }

        public static AnimatedSprite PlaneSprite;
        public static Color[] MaskPlane;
        public static AnimatedSprite sprLaser;
        public static Color[] MaskLaser;
        public static AnimatedSprite sprLaserBottom;
        private LaserWeapon ActiveLaser;

        public Plane4(int Resist, int BombTimeMax)
            : base(Resist, PlaneSprite, MaskPlane)
        {
            Direction(3);
            ActiveLaser = new LaserWeapon(Position);
            EnemyAttack.AddEnemyAttack(ActiveLaser);
        }

        public override void Update(float TimeEllapsed)
        {
            base.Update(TimeEllapsed);
            ActiveLaser.Position.X = Position.X;
        }

        public override void Destroyed(Vehicule Destroyer)
        {
            ActiveLaser.Resist = 0;
            IsAlive = false;

            if (Destroyer == null)
                return;

            for (int i = 0; i < 10; i++)
            {
                new FlameParticle(Position, SuperTank2.Randomizer.Next(360) * SuperTank2.DegToRad, SuperTank2.Randomizer.NextDouble() * 5).AddParticule();
            }

            if (SuperTank2.Randomizer.Next(10 - ((int)SuperTank2.Difficulty * 4)) == 1)
                PowerUp.AddPowerUp(new PowerUp(new Vector2(Position.X, Position.Y), PowerUp.ArrayPowerUpChoice[SuperTank2.Randomizer.Next(18)]));

            Explosion.AddExplosion(new Explosion1(Position, 0, 0));

            for (int i = 0; i < 4; i++)
                Smoke.AddSmoke(new PlaneSmoke(Position, (float)(Math.Sign(Speed.X) * Math.PI - 10 * SuperTank2.DegToRad + SuperTank2.Randomizer.Next(20) * SuperTank2.DegToRad), 1 + SuperTank2.Randomizer.Next(4)));

            if (SuperTank2.Difficulty == DifficultyChoices.Normal)
            {
                Destroyer.Points += 20;
                Destroyer.Argent += 10;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
            {
                Destroyer.Points += 30;
                Destroyer.Argent += 15;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
            {
                Destroyer.Points += 40;
                Destroyer.Argent += 20;
            }
        }
    }
}