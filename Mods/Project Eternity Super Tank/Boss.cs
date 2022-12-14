using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    public class Boss1 : Enemy
    {
        public class LaserExplosion : GroundExplosion
        {
            public static AnimatedSprite ExplosionSprite;

            public LaserExplosion(Vector2 Position)
                : base(Position, ExplosionSprite)
            {
            }
        }

        public class BombFlame : Particule
        {
            public static Particle3DSample.ParticleSystem ParticleSystem;
            public static Particle3DSample.ParticleSettings ParticleSettings;

            public BombFlame(AnimatedSprite Clone, Vector2 Position)
                : base(Clone, Position, Vector2.Zero)
            {
            }

            public override void AddParticule()
            {
                ParticleSystem.AddParticle(Position, Speed);
            }
        }

        public class BossBomb : EnemyAttack
        {
            public static AnimatedSprite BombSprite;
            public static Color[] BombMask;

            public BossBomb(Vector2 Position, Vector2 Speed, float Resist, float Damage)
                : base(Position, Speed, Resist, Damage, BombSprite, BombMask)
            {
                AttackType = AttackTypes.LaserBomb;
            }

            public override void Update(float TimeEllapsed)
            {
                Speed.Y += 0.1f;
                Position += Speed * TimeEllapsed;
                new BombFlame(BombSprite, Position).AddParticule();

                UpdateTransformationMatrix();

                if (Position.Y > 740)
                {
                    Resist = 0;
                    GroundExplosion.AddGroundExplosion(new LaserExplosion(new Vector2(Position.X, 730)));
                }
            }

            public override void Destroyed(Vehicule Destroyed)
            {
                for (int i = 0; i < 5; i++)
                {
                    new FlameParticle(Position, SuperTank2.Randomizer.Next(360) * SuperTank2.DegToRad, SuperTank2.Randomizer.NextDouble() * 5).AddParticule();
                }
                if (SuperTank2.Difficulty == DifficultyChoices.Normal)
                {
                    Destroyed.Points += 20;
                    Destroyed.Argent += 20;
                }
                else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
                {
                    Destroyed.Points += 30;
                    Destroyed.Argent += 30;
                }
                else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
                {
                    Destroyed.Points += 40;
                    Destroyed.Argent += 40;
                }
            }
        }

        public class BossExplosion : Explosion
        {
            public BossExplosion(Vector2 Position, AnimatedSprite Clone, double Angle, double NewSpeed)
                : base(Position, Clone, Angle, NewSpeed)
            {
            }
        }

        public float MaxResist;
        float TimerAttack;
        public int Destroy = 90;
        AnimatedSprite sprActiveSprite;
        AnimatedSprite sprBossExplosion;
        Vector2[] ArrayBossPath;
        Vector2[] ArrayBossPath2;
        bool ComingDown;
        int CurrentPathIndex;
        float BossSpeed;

        public Boss1()
        {
            this.Position = new Vector2(1095, 75);
            BossSpeed = 1.5f;
            TimerAttack = 30;
            ComingDown = true;
            IsAlive = false;

            if (SuperTank2.Difficulty == DifficultyChoices.Normal)
                MaxResist = 200;
            else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
                MaxResist = 400;
            else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
                MaxResist = 600;

            CurrentPathIndex = 0;
            ArrayBossPath = new Vector2[14];
            ArrayBossPath[0] = new Vector2(1095, 75);
            ArrayBossPath[1] = new Vector2(95, 75);
            ArrayBossPath[2] = new Vector2(95, 150);
            ArrayBossPath[3] = new Vector2(935, 150);
            ArrayBossPath[4] = new Vector2(935, 225);
            ArrayBossPath[5] = new Vector2(95, 225);
            ArrayBossPath[6] = new Vector2(95, 300);
            ArrayBossPath[7] = new Vector2(935, 300);
            ArrayBossPath[8] = new Vector2(935, 375);
            ArrayBossPath[9] = new Vector2(95, 375);
            ArrayBossPath[10] = new Vector2(95, 450);
            ArrayBossPath[11] = new Vector2(935, 450);
            ArrayBossPath[12] = new Vector2(935, 525);
            ArrayBossPath[13] = new Vector2(95, 525);

            ArrayBossPath2 = new Vector2[3];
            ArrayBossPath2[0] = ArrayBossPath[12];
            ArrayBossPath2[1] = ArrayBossPath[13];
            ArrayBossPath2[2] = ArrayBossPath[12];
        }

        public void Load(ContentManager Content)
        {
            sprActiveSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Boss/Boss 1"), 1, new Vector2(74, 55));
            sprBossExplosion = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Boss/Boss 1 Explosion"), 11, new Vector2(0, 0));
            LaserExplosion.ExplosionSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Misc/Laser explosion 1"), 1, new Vector2(16, 0));
            BossBomb.BombSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Bombs/Boss bomb"), 1, new Vector2(8, 8));
            BossBomb.BombMask = new Color[BossBomb.BombSprite.SpriteWidth * BossBomb.BombSprite.SpriteHeight];
            BossBomb.BombSprite.ActiveSprite.GetData(BossBomb.BombMask);
            Mask = new Color[sprActiveSprite.SpriteWidth * sprActiveSprite.SpriteHeight];
            sprActiveSprite.ActiveSprite.GetData(Mask);

            SpriteWidth = sprActiveSprite.SpriteWidth;
            SpriteHeight = sprActiveSprite.SpriteHeight;
            Origin = sprActiveSprite.Origin;
        }

        public void FollowPath(Vector2 NextPosition, float TimeEllapsed)
        {
            if (Position.X < NextPosition.X)
            {
                Position.X += BossSpeed * TimeEllapsed;
                if (Position.X > NextPosition.X)
                    Position.X = NextPosition.X;
            }
            else if (Position.X > NextPosition.X)
            {
                Position.X -= BossSpeed * TimeEllapsed;
                if (Position.X < NextPosition.X)
                    Position.X = NextPosition.X;
            }

            if (Position.Y < NextPosition.Y)
            {
                Position.Y += BossSpeed * TimeEllapsed;
                if (Position.Y > NextPosition.Y)
                    Position.Y = NextPosition.Y;
            }
            else if (Position.Y > NextPosition.Y)
            {
                Position.Y -= BossSpeed * TimeEllapsed;
                if (Position.Y < NextPosition.Y)
                    Position.Y = NextPosition.Y;
            }
        }

        public override void Update(float TimeEllapsed)
        {
            TimerAttack-=TimeEllapsed;
            if (TimerAttack <= 0)
            {
                int BombHP = 0;
                int BombDamage = 0;
                if (SuperTank2.Difficulty == DifficultyChoices.Normal)
                {
                    BombHP = 10;
                    BombDamage = 8;
                }
                else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
                {
                    BombHP = 20;
                    BombDamage = 16;
                }
                else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
                {
                    BombHP = 30;
                    BombHP = 24;
                }
                TimerAttack = 30;

                EnemyAttack.AddEnemyAttack(new BossBomb(new Vector2(Position.X, Position.Y + 35), new Vector2(0, 6), BombHP, BombDamage));
            }

            if (ComingDown)
            {
                Vector2 NextPosition = ArrayBossPath[CurrentPathIndex + 1];

                FollowPath(NextPosition, TimeEllapsed);

                if (Position == NextPosition)
                {
                    ++CurrentPathIndex;
                    if (CurrentPathIndex + 1 == ArrayBossPath.Length)
                    {
                        ComingDown = false;
                        CurrentPathIndex = 0;
                    }
                }
            }
            else
            {
                Vector2 NextPosition = ArrayBossPath2[CurrentPathIndex + 1];

                FollowPath(NextPosition, TimeEllapsed);

                if (Position == NextPosition)
                {
                    ++CurrentPathIndex;
                    if (CurrentPathIndex + 1 == ArrayBossPath2.Length)
                    {
                        CurrentPathIndex = 0;
                    }
                }
            }

            UpdateTransformationMatrix();

            int ParticleCount = 50;

            for (int i = 0; i < ParticleCount; i++)
			{
                new Propulsor(new Vector2(Position.X - 65, Position.Y + 33), (80 + SuperTank2.Randomizer.Next(20)) * SuperTank2.DegToRad, 5 + SuperTank2.Randomizer.NextDouble() * 4).AddParticule();
                new Propulsor(new Vector2(Position.X + 65, Position.Y + 33), (80 + SuperTank2.Randomizer.Next(20)) * SuperTank2.DegToRad, 5 + SuperTank2.Randomizer.NextDouble() * 4).AddParticule();
			}
            if (Resist <= 0)
            {
                if (Destroy > 0)
                {
                    Destroy--;
                    Explosion.AddExplosion(new BossExplosion(new Vector2(Position.X + SuperTank2.Randomizer.Next(300) - 150, Position.Y + SuperTank2.Randomizer.Next(100) - 50), sprBossExplosion, SuperTank2.Randomizer.Next(360) * SuperTank2.DegToRad, 5 + SuperTank2.Randomizer.NextDouble() * 4));
                }
                else
                    IsAlive = false;
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            sprActiveSprite.Draw(g, 0, Position, 0, Color.White);
            if (Resist > 0)
            {
                g.Draw(sprPixel, new Rectangle((int)Position.X - 50, (int)Position.Y - 50, 100, 6), Color.FromNonPremultiplied(0, 0, 0, 200));
                g.Draw(sprPixel, new Rectangle((int)Position.X - 49, (int)Position.Y - 49, (int)(Resist * (98 / (float)MaxResist)), 4), Color.FromNonPremultiplied(0, 255, 0, 200));
            }
        }

        public override void Destroyed(Vehicule Destroyed)
        {
        }
    }

    public class Boss2 : Enemy
    {
        public class Missile : EnemyAttack
        {
            Vehicule Near;
            float TurnSpeed = 3 * SuperTank2.DegToRad;
            float AnimationPosition;

            public Missile(Vector2 Position, Vector2 Speed, AnimatedSprite Clone, Color[] Mask)
                : base(Position, Speed, 0, 0, Clone, Mask)
            {
                if (SuperTank2.Difficulty == DifficultyChoices.Normal)
                {
                    Resist = 3;
                    Damage = 4;
                }
                else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
                {
                    Resist = 5;
                    Damage = 9;
                }
                else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
                {
                    Resist = 8;
                    Damage = 12;
                }

                Angle = (float)Math.Atan2(Speed.Y, Speed.X);

                Near = null;
                float DistanceMin = float.MaxValue;
                for (int V = 0; V < Vehicule.ArrayVehicule.Length; V++)
                {
                    float Distance = Vector2.Distance(Position, Vehicule.ArrayVehicule[V].Position);
                    if (Distance < DistanceMin)
                    {
                        DistanceMin = Distance;
                        Near = Vehicule.ArrayVehicule[V];
                    }
                }
            }

            public override void Update(float TimeEllapsed)
            {
                AnimationPosition += 0.2f;
                if (AnimationPosition >= AnimationFrameCount)
                    AnimationPosition -= AnimationFrameCount;

                SpriteSource.X = (int)Math.Floor(AnimationPosition) * SpriteWidth;

                if (Position.Y < 640)
                {
                    Angle = SuperTank2.TurnToFace(Position, Near.Position, Angle, TurnSpeed);
                }

                Speed.X = (float)Math.Cos(Angle) * 3 * TimeEllapsed;
                Speed.Y = (float)Math.Sin(Angle) * 3 * TimeEllapsed;

                Position += Speed;

                UpdateTransformationMatrix();

                if (Position.Y > 740)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        new FlameParticle(Position, (60 + SuperTank2.Randomizer.Next(60)) * SuperTank2.DegToRad, 2 + SuperTank2.Randomizer.NextDouble() * 5).AddParticule();
                        Smoke.AddSmoke(new BombSmoke(Position, (80 + SuperTank2.Randomizer.Next(20)) * SuperTank2.DegToRad, 0));
                    }
                    GroundExplosion.AddGroundExplosion(new GroundExplosion(new Vector2(Position.X, 730)));
                    Resist = 0;
                }
                new Propulsor(Position, Angle, -SuperTank2.Randomizer.NextDouble() * 5).AddParticule();
            }

            public override void Destroyed(Vehicule Destroyer)
            {
                for (int i = 0; i < 5; i++)
                {
                    new Propulsor(Position, SuperTank2.Randomizer.Next(360) * SuperTank2.DegToRad, SuperTank2.Randomizer.NextDouble() * 5).AddParticule();
                    Smoke.AddSmoke(new BombSmoke(Position, Angle - (10 + SuperTank2.Randomizer.Next(20)) * SuperTank2.DegToRad, 1 + SuperTank2.Randomizer.Next(4)));
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

        public class LaserWeapon : EnemyAttack
        {
            public int Lifetime;

            public LaserWeapon(Vector2 Position)
                : base(Position, Vector2.Zero, 1, 1, Boss2.sprLaser, Boss2.MaskLaser)
            {
                AttackType = AttackTypes.Laser;
                Scale.Y = 560;
                Lifetime = 120;
                Angle = 0;

                if (SuperTank2.Difficulty == DifficultyChoices.Normal)
                {
                    Damage = 0.3f;
                }
                else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
                {
                    Damage = 0.4f;
                }
                else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
                {
                    Damage = 0.5f;
                }
            }

            public override void Update(float TimeEllapsed)
            {
                --Lifetime;
                if (Lifetime <= 0)
                    Resist = 0;

                UpdateTransformationMatrix(Scale);
            }

            public override void Destroyed(Vehicule Destroyer)
            {
                throw new NotImplementedException();
            }
        }

        public class LaserParticle : Particule
        {
            public LaserParticle(Vector2 Position, float Angle, double Speed)
                : base(Boss2.sprLaserParticle, Position, Angle, Speed)
            {
            }

            public override void Update(GameTime gameTime)
            {
                Alpha -= 5;
                if (Alpha < 1)
                    IsAlive = false;
            }
        }

        private enum AttackTypes { None, Missiles, Laser };
        AnimatedSprite sprActiveSprite;
        public int MaxResist;
        AttackTypes AttackType;
        int AttackTypeTimer;
        int MissilesTimer;
        int MissilesPhaseTimer;
        int LaserPhaseTimer;
        int CoolDown;
        public int Destroy;
        Color[] MaskMissile;
        AnimatedSprite sprMissile;
        public static AnimatedSprite sprLaserParticle;
        public static AnimatedSprite sprLaser;
        public static Color[] MaskLaser;
        Texture2D sprBossPropulsor;
        float SpeedX = 3;
        LaserWeapon ActiveLaser;

        public Boss2(Vector2 Position)
        {
            this.Position = Position;
            AttackType = AttackTypes.None;
            MissilesPhaseTimer = 0;
            LaserPhaseTimer = 0;
            CoolDown = 0;
            Destroy = 90;
            IsAlive = false;
            Speed = new Vector2(-5, 0);

            if (SuperTank2.Difficulty == DifficultyChoices.Normal)
                MaxResist = 400;
            else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
                MaxResist = 1000;
            else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
                MaxResist = 2150;
        }

        public void Load(ContentManager Content)
        {
            sprBossPropulsor = Content.Load<Texture2D>("Textures/Boss/Boss 2 propulsor");
            sprLaser = new AnimatedSprite(Content.Load<Texture2D>("Textures/Boss/Boss 2 laser"), 1, new Vector2(28, 0));

            MaskLaser = new Color[sprLaser.SpriteWidth * sprLaser.SpriteHeight];
            sprLaser.ActiveSprite.GetData(MaskLaser);

            sprLaserParticle = new AnimatedSprite(Content.Load<Texture2D>("Textures/Boss/Boss 2 laser particle"), 1, new Vector2(15, 14));
            sprActiveSprite = new AnimatedSprite(Content.Load<Texture2D>("Textures/Boss/Boss 2"), 1, new Vector2(104, 0));

            Mask = new Color[sprActiveSprite.SpriteWidth * sprActiveSprite.SpriteHeight];
            sprActiveSprite.ActiveSprite.GetData(Mask);

            sprMissile = new AnimatedSprite(Content.Load<Texture2D>("Textures/Boss/Boss 2 missile"), 4, new Vector2(5, 6));

            MaskMissile = new Color[sprMissile.SpriteWidth * sprMissile.SpriteHeight];
            sprMissile.ActiveSprite.GetData(0, new Rectangle(0, 0, sprMissile.SpriteWidth, sprMissile.SpriteHeight), MaskMissile, 0, sprMissile.SpriteWidth * sprMissile.SpriteHeight);

            SpriteWidth = sprActiveSprite.SpriteWidth;
            SpriteHeight = sprActiveSprite.SpriteHeight;
            Origin = sprActiveSprite.Origin;
        }

        public override void Update(float TimeEllapsed)
        {
            if (AttackType == AttackTypes.None)
            {
                if (Position.X <= 104)
                    Speed.X = SpeedX * TimeEllapsed;
                else if (Position.X >= 920)
                    Speed.X = -SpeedX * TimeEllapsed;

                --AttackTypeTimer;
                if (AttackTypeTimer <= 0)
                    UpdateAttackTypeTimer();
            }
            else if (AttackType == AttackTypes.Missiles)
            {
                --MissilesTimer;
                if (MissilesTimer <= 0)
                    UpdateMissilesTimer();

                ++MissilesPhaseTimer;

                if (MissilesPhaseTimer >= 150)
                {
                    MissilesPhaseTimer = 0;
                    AttackType = AttackTypes.None;
                }
                if (Position.X <= 507)
                    Speed.X = SpeedX * TimeEllapsed;
                else if (Position.X >= 518)
                {
                    Speed.X = -SpeedX * TimeEllapsed;
                }
                else
                {
                    Position.X = 512;
                    Speed.X = 0;
                }
            }
            else if (AttackType == AttackTypes.Laser)
            {
                LaserPhaseTimer += 1;
                if (LaserPhaseTimer < 90)
                    new LaserParticle(new Vector2(Position.X - 20 + SuperTank2.Randomizer.Next(40), Position.Y + 118), SuperTank2.Randomizer.Next(360) * SuperTank2.DegToRad, SuperTank2.Randomizer.NextDouble() * 5).AddParticule();
                else if (LaserPhaseTimer == 90)
                {
                    ActiveLaser = new LaserWeapon(new Vector2(Position.X, Position.Y + 111));
                    EnemyAttack.AddEnemyAttack(ActiveLaser);
                }
                else if (ActiveLaser.Lifetime <= 0)
                {
                    LaserPhaseTimer = 0;
                    AttackType = AttackTypes.None;
                }
                Vehicule Near = null;
                float DistanceMin = float.MaxValue;
                for (int V = 0; V < Vehicule.ArrayVehicule.Length; V++)
                {
                    float Distance = Vector2.Distance(Position, Vehicule.ArrayVehicule[V].Position);
                    if (Distance < DistanceMin)
                    {
                        DistanceMin = Distance;
                        Near = Vehicule.ArrayVehicule[V];
                    }
                }
                if (Position.X <= Near.Position.X - Speed.X)
                {
                    if (LaserPhaseTimer < 90)
                        Speed.X = SpeedX * 0.6f * TimeEllapsed;
                    else
                        Speed.X = SpeedX * 0.2f * TimeEllapsed;
                }
                else if (Position.X >= Near.Position.X + Speed.X)
                {
                    if (LaserPhaseTimer < 90)
                        Speed.X = -SpeedX * 0.6f * TimeEllapsed;
                    else
                        Speed.X = -SpeedX * 0.2f * TimeEllapsed;
                }
                else
                {
                    Position.X = Near.Position.X;
                    Speed.X = 0;
                }
            }

            Position += Speed;

            UpdateTransformationMatrix();

            if (ActiveLaser != null)
                ActiveLaser.Position.X = Position.X;

            if (Resist <= 0)
            {
                if (Destroy > 0)
                {
                    Destroy--;
                    new FlameParticle(new Vector2(Position.X + SuperTank2.Randomizer.Next(300) - 150, Position.Y + SuperTank2.Randomizer.Next(100) - 50), Vector2.Zero).AddParticule();
                }
                else
                    IsAlive = false;
            }
        }

        public void UpdateAttackTypeTimer()
        {
            int Type = SuperTank2.Randomizer.Next(CoolDown + 10);
            Type /= (CoolDown + 10 / 10);

            if (Type < 5)
                AttackType = AttackTypes.None;
            else if (Type < 8)
                AttackType = AttackTypes.Missiles;
            else
                AttackType = AttackTypes.Laser;

            AttackTypeTimer = 150;

            if (AttackType == AttackTypes.None)
            {
                AttackTypeTimer = 50 + CoolDown;
                CoolDown = 0;
            }
            else if (AttackType == AttackTypes.Missiles)
            {
                MissilesTimer = 1;
                CoolDown += 20;
            }
            else
                CoolDown += 50;
        }

        public void UpdateMissilesTimer()
        {
            MissilesTimer = 30;

            EnemyAttack.AddEnemyAttack(new Missile(new Vector2(Position.X - 50, Position.Y + 20), new Vector2(-1.5f, -1.5f), sprMissile, MaskMissile));
            EnemyAttack.AddEnemyAttack(new Missile(new Vector2(Position.X + 50, Position.Y + 20), new Vector2(1.5f, -1.5f), sprMissile, MaskMissile));
        }

        public override void Draw(CustomSpriteBatch g)
        {
            sprActiveSprite.Draw(g, 0, Position, 0, Color.White);
            g.Draw(sprBossPropulsor, new Vector2(Position.X - 104, Position.Y + 107), Color.White);
            g.Draw(sprBossPropulsor, new Vector2(Position.X + 21, Position.Y + 107), Color.White);
            if (Resist > 0)
            {
                g.Draw(sprPixel, new Rectangle((int)Position.X - 50, (int)Position.Y - 50, 100, 6), Color.FromNonPremultiplied(0, 0, 0, 200));
                g.Draw(sprPixel, new Rectangle((int)Position.X - 49, (int)Position.Y - 49, (int)(Resist * (98 / (float)MaxResist)), 4), Color.FromNonPremultiplied(0, 255, 0, 200));
            }
        }

        public override void Destroyed(Vehicule Destroyed)
        {
        }
    }
}