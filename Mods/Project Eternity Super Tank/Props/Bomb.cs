using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    public abstract class EnemyAttack : AnimatedSprite
    {
        public enum AttackTypes { NormalBomb, LaserBomb, Laser }
        public static List<EnemyAttack> ListEnemyAttack = new List<EnemyAttack>();
        public static void AddEnemyAttack(EnemyAttack NewEnemyAttack)
        {
            for (int B = 0; B < EnemyAttack.ListEnemyAttack.Count; B++)
            {
                if (EnemyAttack.ListEnemyAttack[B].Resist <= 0)
                {
                    EnemyAttack.ListEnemyAttack[B] = NewEnemyAttack;
                    return;
                }
            }

            EnemyAttack.ListEnemyAttack.Add(NewEnemyAttack);
        }

        public float Damage;
        public float Resist;

        Vector2 PositionOld;
        public Vector2 Speed;
        public AttackTypes AttackType;

        protected EnemyAttack(Vector2 Position, Vector2 Speed, float Resist, float Damage, AnimatedSprite Clone, Color[] Mask)
            : base(Clone.ActiveSprite, Mask, Clone.AnimationFrameCount, Clone.Origin, Clone.AnimationSpeed)
        {
            this.Position = Position;
            this.Speed = Speed;
            this.Resist = Resist;
            this.Damage = Damage;
        }

        public virtual void Update(float TimeEllapsed)
        {
            Position += Speed * TimeEllapsed;
            Speed += SuperTank2.Gravity * TimeEllapsed;
            PositionOld = Position;

            Angle = (float)Math.Atan2(Speed.Y, Speed.X);

            UpdateTransformationMatrix();

            if (Position.Y > 740)
            {
                Resist = 0;
                GroundExplosion.AddGroundExplosion(new GroundExplosion(new Vector2(Position.X, 730)));
            }
            else if (Position.X < -10 || Position.Y < 0)
                Resist = 0;

            int ParticleCount = 10;

            for (int i = 0; i < ParticleCount; i++)
				new Propulsor(Position, Angle + (15 - (float)SuperTank2.Randomizer.NextDouble() * 30) * SuperTank2.DegToRad, -SuperTank2.Randomizer.NextDouble() * 5).AddParticule();
        }

		public abstract void Destroyed(Vehicule Destroyer);

        public virtual void Draw(CustomSpriteBatch g)
        {
			g.Draw(ActiveSprite, Position, SpriteSource, Color.White, Angle, Origin, Scale, SpriteEffects.None, 0);
        }
    }

    public class Bomb1 : EnemyAttack
    {
        public static AnimatedSprite BombSprite;
        public static Color[] BombMask;

        public Bomb1(Vector2 Position, Vector2 Speed, float Resist, float Damage)
            : base(Position, Speed, Resist, Damage, BombSprite, BombMask)
        {
            AttackType = AttackTypes.NormalBomb;
        }

        public override void Destroyed(Vehicule Destroyer)
        {
            for (int i = 0; i < 5; i++)
            {
                new FlameParticle(Position, SuperTank2.Randomizer.Next(360) * SuperTank2.DegToRad, SuperTank2.Randomizer.NextDouble() * 5).AddParticule();
                Smoke.AddSmoke(new PlaneSmoke(Position, (float)(Angle - 10 * SuperTank2.DegToRad + SuperTank2.Randomizer.Next(20) * SuperTank2.DegToRad), 1 + SuperTank2.Randomizer.Next(4)));
            }
            if (SuperTank2.Difficulty == DifficultyChoices.Normal)
            {
                Destroyer.Points += 6;
                Destroyer.Argent += 3;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
            {
                Destroyer.Points += 10;
                Destroyer.Argent += 5;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
            {
                Destroyer.Points += 16;
                Destroyer.Argent += 8;
            }
        }
    }

    public class Bomb2 : EnemyAttack
    {
        public static AnimatedSprite BombSprite;
        public static Color[] BombMask;

        public Bomb2(Vector2 Position, Vector2 Speed, float Resist, float Damage)
            : base(Position, Speed, Resist, Damage, BombSprite, BombMask)
        {
            AttackType = AttackTypes.NormalBomb;
        }

        public override void Destroyed(Vehicule Destroyer)
        {
            for (int i = 0; i < 5; i++)
            {
                new FlameParticle(Position, SuperTank2.Randomizer.Next(360) * SuperTank2.DegToRad, SuperTank2.Randomizer.NextDouble() * 5).AddParticule();
                Smoke.AddSmoke(new PlaneSmoke(Position, (float)(Angle - 10 * SuperTank2.DegToRad + SuperTank2.Randomizer.Next(20) * SuperTank2.DegToRad), 1 + SuperTank2.Randomizer.Next(4)));
            }
            if (SuperTank2.Difficulty == DifficultyChoices.Normal)
            {
                Destroyer.Points += 6;
                Destroyer.Argent += 3;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
            {
                Destroyer.Points += 10;
                Destroyer.Argent += 5;
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
            {
                Destroyer.Points += 16;
                Destroyer.Argent += 8;
            }
        }
    }
}