using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    public enum DifficultyChoices { Normal, Hard, Expert }

    public class SuperTank2 : GameScreen
    {
        public static bool CheatTripleEnergy = false;
        public static bool CheatUnlimitedEnergy = false;
        public static bool CheatDoubleDamage = false;
        public static bool CheatDoubleResist = false;
        public static bool SlowDownSpecialActive = false;
        public static int CurrentLevel = 0;
        public static DifficultyChoices Difficulty;
        public static Vector2 Gravity = new Vector2(0f, 0.03f);
        public static Random Randomizer;
        public static float DegToRad = 0.0174532925f;
        public static bool IsPlaying = false;
        
        SpriteFont fntFPS;
        int TotalFrames = 0;
        double ElapsedTime = 0.0f;
        int FramesLastSecond = 0;

        public SuperTank2()
        {
            int MaxParticle = 65535 / 4;
            Particle3DSample.ParticleSettings FlameParticleSettings = new Particle3DSample.ParticleSettings();
            FlameParticleSettings.TextureName = "Animations/Bitmap Animations/Super Tank 2/Misc/Particule";
            FlameParticleSettings.MaxParticles = 20000;
            FlameParticleSettings.Duration = TimeSpan.FromSeconds(1);
            FlameParticleSettings.Gravity = new Vector2(0, 0.5f);
            FlameParticleSettings.MinScale = new Vector2(0, 0);
            FlameParticleSettings.StartingAlpha = 1f;
            FlameParticleSettings.EndAlpha = 0.4f;
            FlameParticle.ParticleSystem = new Particle3DSample.ParticleSystem(FlameParticleSettings);

            Particle3DSample.ParticleSettings PropulsorParticleSettings = new Particle3DSample.ParticleSettings();
            PropulsorParticleSettings.TextureName = "Animations/Bitmap Animations/Super Tank 2/Misc/Propulsor";
            PropulsorParticleSettings.MaxParticles = MaxParticle;
            PropulsorParticleSettings.Duration = TimeSpan.FromSeconds(1);
            PropulsorParticleSettings.Gravity = new Vector2(0, 0);
            PropulsorParticleSettings.MinScale = new Vector2(0, 0);
            PropulsorParticleSettings.StartingAlpha = 1f;
            PropulsorParticleSettings.EndAlpha = 0.4f;
            Propulsor.ParticleSystem = new Particle3DSample.ParticleSystem(PropulsorParticleSettings);

            Particle3DSample.ParticleSettings BombFlameParticleSettings = new Particle3DSample.ParticleSettings();
            BombFlameParticleSettings.TextureName = "Animations/Bitmap Animations/Super Tank 2/Bombs/Boss bomb";
            BombFlameParticleSettings.MaxParticles = 5000;
            BombFlameParticleSettings.Duration = TimeSpan.FromSeconds(0.5);
            BombFlameParticleSettings.Gravity = new Vector2(0, 0);
            BombFlameParticleSettings.MinScale = new Vector2(0.5f, 0.5f);
            BombFlameParticleSettings.StartingAlpha = 1f;
            BombFlameParticleSettings.EndAlpha = 0.4f;
            Boss1.BombFlame.ParticleSystem = new Particle3DSample.ParticleSystem(BombFlameParticleSettings);

            Particle3DSample.ParticleSettings FlakTraceParticleSettings = new Particle3DSample.ParticleSettings();
            FlakTraceParticleSettings.TextureName = "Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Flak trace";
            FlakTraceParticleSettings.MaxParticles = MaxParticle;
            FlakTraceParticleSettings.Duration = TimeSpan.FromSeconds(0.5);
            FlakTraceParticleSettings.Gravity = new Vector2(0, 0);
            FlakTraceParticleSettings.MinScale = new Vector2(1, 1);
            FlakTraceParticleSettings.StartingAlpha = 1f;
            FlakTraceParticleSettings.EndAlpha = 0.4f;
            FlakTrace.ParticleSystem = new Particle3DSample.ParticleSystem(FlakTraceParticleSettings);
        }

        public override void Load()
        {
            Matrix view = Matrix.Identity;

            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 0, 1);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            Projection = view * ( Projection);

            FlameParticle.ParticleSystem.LoadContent(Content, GraphicsDevice, Projection);
            Propulsor.ParticleSystem.LoadContent(Content, GraphicsDevice, Projection);
            Boss1.BombFlame.ParticleSystem.LoadContent(Content, GraphicsDevice, Projection);
            FlakTrace.ParticleSystem.LoadContent(Content, GraphicsDevice, Projection);

            fntFPS = Content.Load<SpriteFont>("Fonts/Arial16");
            Randomizer = new Random();
            Enemy.ListEnemy = new List<Enemy>();
            PowerUp.ArrayPowerUpChoice = new PowerUp.PowerUpTypes[20];
            PowerUp.ArrayPowerUpChoice[0] = PowerUp.PowerUpTypes.VehiculeSpeed;
            PowerUp.ArrayPowerUpChoice[1] = PowerUp.PowerUpTypes.Shield;
            PowerUp.ArrayPowerUpChoice[2] = PowerUp.PowerUpTypes.VehiculeRegen;
            PowerUp.ArrayPowerUpChoice[3] = PowerUp.PowerUpTypes.VehiculeRepair;
            PowerUp.ArrayPowerUpChoice[4] = PowerUp.PowerUpTypes.Power1;
            PowerUp.ArrayPowerUpChoice[5] = PowerUp.PowerUpTypes.Power2;
            PowerUp.ArrayPowerUpChoice[6] = PowerUp.PowerUpTypes.Power3;
            PowerUp.ArrayPowerUpChoice[7] = PowerUp.PowerUpTypes.RateOfFire1;
            PowerUp.ArrayPowerUpChoice[8] = PowerUp.PowerUpTypes.RateOfFire2;
            PowerUp.ArrayPowerUpChoice[9] = PowerUp.PowerUpTypes.RateOfFire3;
            PowerUp.ArrayPowerUpChoice[10] = PowerUp.PowerUpTypes.Speed1;
            PowerUp.ArrayPowerUpChoice[11] = PowerUp.PowerUpTypes.Speed2;
            PowerUp.ArrayPowerUpChoice[12] = PowerUp.PowerUpTypes.Speed3;
            PowerUp.ArrayPowerUpChoice[13] = PowerUp.PowerUpTypes.Flak;
            PowerUp.ArrayPowerUpChoice[14] = PowerUp.PowerUpTypes.Missile;
            PowerUp.ArrayPowerUpChoice[15] = PowerUp.PowerUpTypes.GuidedMissile;
            PowerUp.ArrayPowerUpChoice[16] = PowerUp.PowerUpTypes.Laser;
            PowerUp.ArrayPowerUpChoice[17] = PowerUp.PowerUpTypes.Orb;
            PowerUp.ArrayPowerUpChoice[18] = PowerUp.PowerUpTypes.Turret;
            PowerUp.ArrayPowerUpChoice[19] = PowerUp.PowerUpTypes.Shuriken;

			Plane1.PlaneSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Enemies/Plane1"), 1, new Vector2(130, 26));
			Plane2.PlaneSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Enemies/Plane2"), 1, new Vector2(54, 15));
			Plane3.PlaneSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Enemies/Plane3"), 1, new Vector2(70, 42));
            Plane4.PlaneSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Enemies/Plane4"), 1, new Vector2(52, 19));
            Plane4.sprLaser = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Bombs/Laser"), 1, new Vector2(8, 0));
            Plane4.sprLaserBottom = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Bombs/Laser bottom"), 11, new Vector2(11, 7));
            Plane1.PlaneMask = new Color[Plane1.PlaneSprite.SpriteWidth * Plane1.PlaneSprite.SpriteHeight];
            Plane1.PlaneSprite.ActiveSprite.GetData(Plane1.PlaneMask);

            Plane2.PlaneMask = new Color[Plane2.PlaneSprite.SpriteWidth * Plane2.PlaneSprite.SpriteHeight];
            Plane2.PlaneSprite.ActiveSprite.GetData(Plane2.PlaneMask);

            Plane3.PlaneMask = new Color[Plane3.PlaneSprite.SpriteWidth * Plane3.PlaneSprite.SpriteHeight];
            Plane3.PlaneSprite.ActiveSprite.GetData(Plane3.PlaneMask);

            Plane4.MaskPlane = new Color[Plane4.PlaneSprite.SpriteWidth * Plane4.PlaneSprite.SpriteHeight];
            Plane4.PlaneSprite.ActiveSprite.GetData(Plane4.MaskPlane);

            Plane4.MaskLaser = new Color[Plane4.sprLaser.SpriteWidth * Plane4.sprLaser.SpriteHeight];
            Plane4.sprLaser.ActiveSprite.GetData(Plane4.MaskLaser);

            GroundExplosion.GroundExplosion1 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Misc/Bomb explosion 1"), 1, new Vector2(16, 0));
            GroundExplosion.GroundExplosion2 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Misc/Bomb explosion 2"), 1, new Vector2(16, 0));

            BombSmoke.SmokeSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Misc/Bomb smoke"), 5, new Vector2(32, 26));
			PlaneSmoke.SmokeSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Misc/Plane smoke"), 9, new Vector2(64, 74));
			Explosion1.ExplosionSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Misc/Explosion 1"), 17, new Vector2(35, 50));

            Propulsor.PropulsorSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Misc/Propulsor"), 1, new Vector2(4, 4));
            FlameParticle.FlameParticleSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Misc/Particule"), 1, new Vector2(8, 8));

            Bomb1.BombSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Bombs/Bomb 1"), 1, new Vector2(16, 10));
            Bomb1.BombMask = new Color[Bomb1.BombSprite.SpriteWidth * Bomb1.BombSprite.SpriteHeight];
            Bomb1.BombSprite.ActiveSprite.GetData(Bomb1.BombMask);

            Bomb2.BombSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Bombs/Bomb 2"), 1, new Vector2(13, 10));
            Bomb2.BombMask = new Color[Bomb2.BombSprite.SpriteWidth * Bomb2.BombSprite.SpriteHeight];
            Bomb2.BombSprite.ActiveSprite.GetData(Bomb2.BombMask);

            AnimatedSprite BombSpriteLaser = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Bombs/Bomb 1"), 1, new Vector2(12, 12));
            AnimatedSprite.sprPixel = Content.Load<Texture2D>("Pixel");

            Vehicule ActiveVehicule = new Tank();
            ActiveVehicule.Argent = 10000;
            ActiveVehicule.Load(Content);
            Vehicule.ArrayVehicule = new Vehicule[1];
            Vehicule.ArrayVehicule[0] = ActiveVehicule;
			PushScreen(new Shop());
        }

        public static float TurnToFace(Vector2 position, Vector2 faceThis, float currentAngle, float turnSpeed)
        {
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            float desiredAngle = (float)Math.Atan2(y, x);

            float difference = WrapAngle(desiredAngle - currentAngle);

            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            return WrapAngle(currentAngle + difference);
        }

        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        public override void Update(GameTime gameTime)
        {
            PushScreen(new Shop());
            // Update
            ElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
 
            // 1 Second has passed
            if (ElapsedTime >= 1.0f)
            {
                FramesLastSecond = TotalFrames;
                TotalFrames = 0;
                ElapsedTime = 0;
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawFPS(g);
        }

        protected void DrawFPS(CustomSpriteBatch g)
        {
            // Only update total frames when drawing
            TotalFrames++;
            g.DrawString(fntFPS, string.Format("FPS={0}", FramesLastSecond),
                new Vector2(10.0f, 20.0f), Color.White);
        }
    }
}