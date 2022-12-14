using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    public abstract class Level : GameScreen
    {
        protected class BackgroundObject
        {
            public Texture2D ActiveTexture;
            public Vector2 Position;
            public bool IsAlive;

            public BackgroundObject(Texture2D ActiveTexture, Vector2 Position)
            {
                this.ActiveTexture = ActiveTexture;
                this.Position = Position;
                IsAlive = true;
            }
        }

        protected class TimePanel
        {
            public Vector2 Position;
            public string Text;

            public TimePanel(Vector2 Position)
            {
                this.Position = Position;
            }
        }

        #region Power up sprites

        private Texture2D sprPUpEnergyRegen;
        private Texture2D sprPUpShield;
        private Texture2D sprPUpRepair;
        private Texture2D sprPUpSpeed;

        private Texture2D sprPUpPower1;
        private Texture2D sprPUpPower2;
        private Texture2D sprPUpPower3;
        private Texture2D sprPUpRateOfFire1;
        private Texture2D sprPUpRateOfFire2;
        private Texture2D sprPUpRateOfFire3;
        private Texture2D sprPUpBulletSpeed1;
        private Texture2D sprPUpBulletSpeed2;
        private Texture2D sprPUpBulletSpeed3;

        private Texture2D sprPUpFlak;
        private Texture2D sprPUpMissile;
        private Texture2D sprPUpGuidedMissile;
        private Texture2D sprPUpLaser;
        private Texture2D sprPUpDefenseOrb;
        private Texture2D sprPUpTurret;
        private Texture2D sprPUpShuriken;

        private AnimatedSprite SprPUpVitVehiHud;
        private AnimatedSprite SprPUpRegVehiHud;
        private AnimatedSprite SprPUpPowHud;
        private AnimatedSprite SprPUpCadHud;
        private AnimatedSprite SprPUpVitHud;
        private AnimatedSprite SprPUpSecHud;

        #endregion

        protected List<BackgroundObject> ListBackgroundObject;
        protected AnimationScreen.AnimationBackground2D Background2D;
        protected float SlowdownSpeed = 50f / 60f;
        public int TimeBeforeBoss;

        public int TimeBeforeTimePanel;
        protected List<TimePanel> ListTimePanel;
        protected List<EnemySpawner> ListEnemySpawner;

        private Texture2D sprHUD;
        private Texture2D sprEnergyMeter;
        private SpriteFont fntArial10;
        private SpriteFont fntArial13;
        private Texture2D sprTimePanel;

        protected FMODSound BackgroundMusic;
        private FMODSound sndHit;

        protected Level()
        {
            SuperTank2.IsPlaying = true;
            ListTimePanel = new List<TimePanel>();
            ListBackgroundObject = new List<BackgroundObject>();
            ListEnemySpawner = new List<EnemySpawner>();
        }

        public override void Load()
        {
            sprHUD = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Misc/Energy ring");
            sprEnergyMeter = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Misc/Energy meter");
            sprTimePanel = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Misc/Time panel");

            sprPUpEnergyRegen = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Energy regen");
            sprPUpShield = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Shield");
            sprPUpRepair = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Repair");
            sprPUpSpeed = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Speed");

            sprPUpPower1 = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Power 1");
            sprPUpPower2 = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Power 2");
            sprPUpPower3 = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Power 3");
            sprPUpRateOfFire1 = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Rate of fire 1");
            sprPUpRateOfFire2 = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Rate of fire 2");
            sprPUpRateOfFire3 = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Rate of fire 3");
            sprPUpBulletSpeed1 = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Bullet speed 1");
            sprPUpBulletSpeed2 = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Bullet speed 2");
            sprPUpBulletSpeed3 = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Bullet speed 3");

            sprPUpFlak = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Flak");
            sprPUpMissile = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Missile");
            sprPUpGuidedMissile = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Guided missile");
            sprPUpLaser = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Laser");
            sprPUpDefenseOrb = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Defense orb");
            sprPUpTurret = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Turret");
            sprPUpShuriken = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Shuriken");

            PowerUp.ActiveMask = new Color[sprPUpPower1.Width * sprPUpPower1.Height];
            sprPUpPower1.GetData(PowerUp.ActiveMask);

            SprPUpVitVehiHud = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Speed Vehicule hud"), 1, new Vector2(0, 0));
            SprPUpRegVehiHud = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Regen Vehicule hud"), 1, new Vector2(0, 0));
            SprPUpPowHud = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Power hud"), 3, new Vector2(0, 0));
            SprPUpCadHud = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Rate of fire hud"), 3, new Vector2(0, 0));
            SprPUpVitHud = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Speed hud"), 3, new Vector2(0, 0));
            SprPUpSecHud = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/PowerUp/Tank Secondary hud"), 1, new Vector2(0, 0));

            fntArial10 = Content.Load<SpriteFont>("Fonts/Arial10");
            fntArial13 = Content.Load<SpriteFont>("Fonts/Arial13");

            sndHit = new FMODSound(FMODSystem, "Content/SFX/hit sound.mp3");

            for (int V = 0; V < Vehicule.ArrayVehicule.Length; V++)
            {
                Vehicule.ArrayVehicule[V].Spawn();
            }
        }

        protected void AddBackgroundObject(BackgroundObject NewBackgroundObject)
        {
            for (int B = 0; B < ListBackgroundObject.Count; B++)
            {
                if (!ListBackgroundObject[B].IsAlive)
                {
                    ListBackgroundObject[B] = NewBackgroundObject;
                    return;
                }
            }

            ListBackgroundObject.Add(NewBackgroundObject);
        }

        public override void Update(GameTime gameTime)
        {
            for (int S = ListEnemySpawner.Count - 1; S >= 0; --S)
            {
                ListEnemySpawner[S].Update(gameTime);
            }

            SuperTank2.SlowDownSpecialActive = false;

            for (int V = 0; V < Vehicule.ArrayVehicule.Length; V++)
            {
                Vehicule.ArrayVehicule[V].BackgroundSpeed = new Vector2(Background2D.MoveSpeed.X, Background2D.MoveSpeed.Y);
                Vehicule.ArrayVehicule[V].Update(gameTime);
            }

            float TimeEllapsed = gameTime.ElapsedGameTime.Milliseconds * 0.0625f;
            if (SuperTank2.SlowDownSpecialActive)
                TimeEllapsed *= 0.5f;

            Background2D.Update(gameTime);

            for (int E = 0; E < Enemy.ListEnemy.Count; E++)
            {
                if (Enemy.ListEnemy[E].IsAlive)
                {
                    Enemy.ListEnemy[E].Update(TimeEllapsed);

                    for (int V = 0; V < Vehicule.ArrayVehicule.Length; V++)
                    {
                        if (Vehicule.ArrayVehicule[V].VehiculeResist > 0)
                        {
                            if (Enemy.ListEnemy[E].PixelIntersect(Vehicule.ArrayVehicule[V]))
                            {
                                sndHit.Play();
                                Enemy.ListEnemy[E].Resist -= 10;

                                if (Enemy.ListEnemy[E].Resist <= 0)
                                    Enemy.ListEnemy[E].Destroyed(Vehicule.ArrayVehicule[V]);

                                if (Vehicule.ArrayVehicule[V].Protec > 0)
                                    Vehicule.ArrayVehicule[V].Protec -= 1;
                                else
                                    Vehicule.ArrayVehicule[V].VehiculeResist -= 1;
                            }
                        }
                    }
                }
            }

            for (int A = 0; A < EnemyAttack.ListEnemyAttack.Count; A++)
            {
                if (EnemyAttack.ListEnemyAttack[A].Resist > 0)
                {
                    EnemyAttack.ListEnemyAttack[A].Update(TimeEllapsed);

                    for (int V = 0; V < Vehicule.ArrayVehicule.Length; V++)
                    {
                        if (Vehicule.ArrayVehicule[V].VehiculeResist > 0)
                        {
                            bool AttackHit;
                            if (EnemyAttack.ListEnemyAttack[A].AttackType == EnemyAttack.AttackTypes.Laser)
                            {
                                AttackHit = EnemyAttack.ListEnemyAttack[A].CollisionBox.Intersects(Vehicule.ArrayVehicule[V].CollisionBox);
                            }
                            else
                            {
                                if (EnemyAttack.ListEnemyAttack[A].PixelIntersect(Vehicule.ArrayVehicule[V]))
                                {
                                    EnemyAttack.ListEnemyAttack[A].Resist = 0;
                                    EnemyAttack.ListEnemyAttack[A].Destroyed(Vehicule.ArrayVehicule[V]);
                                    AttackHit = true;
                                }
                                else
                                    AttackHit = false;
                            }
                            if (AttackHit)
                            {
                                sndHit.Play();
                                Vehicule.ArrayVehicule[V].VehiculeResist -= EnemyAttack.ListEnemyAttack[A].Damage;
                                for (int i = 0; i < EnemyAttack.ListEnemyAttack[A].Damage; i++)
                                {
                                    new FlameParticle(EnemyAttack.ListEnemyAttack[A].Position, SuperTank2.Randomizer.Next(360) * SuperTank2.DegToRad, SuperTank2.Randomizer.NextDouble() * 5).AddParticule();
                                }
                            }
                        }
                    }
                }
            }

            Particule.UpdateParticles(gameTime);

            for (int S = 0; S < Smoke.ListSmoke.Count; S++)
            {
                if (Smoke.ListSmoke[S].IsAlive)
                {
                    Smoke.ListSmoke[S].Update(gameTime);
                }
            }

            for (int E = 0; E < Explosion.ListExplosion.Count; E++)
            {
                if (Explosion.ListExplosion[E].IsAlive)
                {
                    Explosion.ListExplosion[E].Update(gameTime);
                }
            }

            for (int E = 0; E < GroundExplosion.ListGroundExplosion.Count; E++)
            {
                if (GroundExplosion.ListGroundExplosion[E].IsAlive)
                {
                    GroundExplosion.ListGroundExplosion[E].Position.X -= Background2D.MoveSpeed.X;
                    if (GroundExplosion.ListGroundExplosion[E].Position.X < -50)
                        GroundExplosion.ListGroundExplosion[E].IsAlive = false;
                }
            }

            for (int B = 0; B < ListBackgroundObject.Count; B++)
            {
                if (ListBackgroundObject[B].IsAlive)
                {
                    ListBackgroundObject[B].Position.X -= 5;
                    if (ListBackgroundObject[B].Position.X < -200)
                        ListBackgroundObject[B].IsAlive = false;
                }
            }

            for (int T = 0; T < ListTimePanel.Count; T++)
            {
                ListTimePanel[T].Position.X -= Background2D.MoveSpeed.X;
                if (ListTimePanel[T].Position.X < -50)
                    ListTimePanel.RemoveAt(T--);
            }

            for (int P = 0; P < PowerUp.ListPowerUp.Count; P++)
            {
                if (PowerUp.ListPowerUp[P].VisibleTime > 0)
                {
                    PowerUp.ListPowerUp[P].UpdateTransformationMatrix();

                    for (int V = 0; V < Vehicule.ArrayVehicule.Length; V++)
                    {
                        if (PowerUp.ListPowerUp[P].PixelIntersect(Vehicule.ArrayVehicule[V]))
                        {
                            if (Vehicule.ArrayVehicule[V].ActivatePowerUp(PowerUp.ListPowerUp[P].PowerUpType))
                                PowerUp.ListPowerUp[P].VisibleTime = 0;
                        }
                    }
                    if (PowerUp.ListPowerUp[P].Position.Y > 750)
                        --PowerUp.ListPowerUp[P].VisibleTime;
                    else
                        PowerUp.ListPowerUp[P].Position.Y += 1;
                }
            }

            if (TimeBeforeBoss % TimeBeforeTimePanel == 0 && TimeBeforeBoss > 0)
            {
                TimePanel NewTimePanel = new TimePanel(new Vector2(1024, 655));
                NewTimePanel.Text = TimeBeforeBoss / TimeBeforeTimePanel + " km";
                ListTimePanel.Add(NewTimePanel);
            }
            --TimeBeforeBoss;
        }

        protected void EndLevel()
        {
            SuperTank2.CurrentLevel += 1;
            Particule.ClearParticles();
            PushScreen(new Shop());
            RemoveScreen(this);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            Background2D.Draw(g, Constants.Width, Constants.Height);
            g.Begin();

            for (int B = 0; B < ListBackgroundObject.Count; B++)
            {
                if (ListBackgroundObject[B].IsAlive)
                {
                    g.Draw(ListBackgroundObject[B].ActiveTexture, ListBackgroundObject[B].Position, Color.White);
                }
            }

            Particule.Draw(g);

            for (int A = 0; A < EnemyAttack.ListEnemyAttack.Count; A++)
            {
                if (EnemyAttack.ListEnemyAttack[A].Resist > 0)
                {
                    EnemyAttack.ListEnemyAttack[A].Draw(g);
                }
            }

            for (int E = 0; E < Enemy.ListEnemy.Count; E++)
            {
                if (Enemy.ListEnemy[E].IsAlive)
                {
                    Enemy.ListEnemy[E].Draw(g);
                }
            }

            for (int S = 0; S < Smoke.ListSmoke.Count; S++)
            {
                if (Smoke.ListSmoke[S].IsAlive)
                {
                    Smoke.ListSmoke[S].Draw(g);
                }
            }

            for (int P = 0; P < Explosion.ListExplosion.Count; P++)
            {
                if (Explosion.ListExplosion[P].IsAlive)
                {
                    Explosion.ListExplosion[P].Draw(g);
                }
            }

            for (int E = 0; E < GroundExplosion.ListGroundExplosion.Count; E++)
            {
                if (GroundExplosion.ListGroundExplosion[E].IsAlive)
                {
                    GroundExplosion.ListGroundExplosion[E].Draw(g);
                }
            }

            for (int T = 0; T < ListTimePanel.Count; T++)
            {
                g.Draw(sprTimePanel, ListTimePanel[T].Position, Color.White);
                g.DrawStringRightAligned(fntArial13, ListTimePanel[T].Text, new Vector2(ListTimePanel[T].Position.X + 55, ListTimePanel[T].Position.Y + 7), Color.White);
            }

            DrawPowerUps(g);

            for (int V = 0; V < Vehicule.ArrayVehicule.Length; V++)
                Vehicule.ArrayVehicule[V].Draw(g);

            DrawBase(g);
        }

        public void DrawPowerUps(CustomSpriteBatch g)
        {
            for (int P = 0; P < PowerUp.ListPowerUp.Count; P++)
            {
                if (PowerUp.ListPowerUp[P].VisibleTime > 0)
                {
                    int Alpha = (int)(PowerUp.ListPowerUp[P].VisibleTime / 60f * 255f);
                    Color ActiveColor = Color.FromNonPremultiplied(255, 255, 255, Alpha);
                    switch (PowerUp.ListPowerUp[P].PowerUpType)
                    {
                        case PowerUp.PowerUpTypes.VehiculeRegen:
                            g.Draw(sprPUpEnergyRegen, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.Shield:
                            g.Draw(sprPUpShield, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.VehiculeRepair:
                            g.Draw(sprPUpRepair, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.VehiculeSpeed:
                            g.Draw(sprPUpSpeed, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.Power1:
                            g.Draw(sprPUpPower1, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.Power2:
                            g.Draw(sprPUpPower2, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.Power3:
                            g.Draw(sprPUpPower3, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.RateOfFire1:
                            g.Draw(sprPUpRateOfFire1, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.RateOfFire2:
                            g.Draw(sprPUpRateOfFire2, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.RateOfFire3:
                            g.Draw(sprPUpRateOfFire3, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.Speed1:
                            g.Draw(sprPUpBulletSpeed1, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.Speed2:
                            g.Draw(sprPUpBulletSpeed2, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.Speed3:
                            g.Draw(sprPUpBulletSpeed3, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.Flak:
                            g.Draw(sprPUpFlak, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.Missile:
                            g.Draw(sprPUpMissile, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.GuidedMissile:
                            g.Draw(sprPUpGuidedMissile, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.Laser:
                            g.Draw(sprPUpLaser, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.Orb:
                            g.Draw(sprPUpDefenseOrb, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.Turret:
                            g.Draw(sprPUpTurret, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;

                        case PowerUp.PowerUpTypes.Shuriken:
                            g.Draw(sprPUpShuriken, PowerUp.ListPowerUp[P].Position, null, ActiveColor, 0, PowerUp.ListPowerUp[P].Origin, Vector2.One, SpriteEffects.None, 0);
                            break;
                    }
                }
            }
        }

        public void DrawPowerUpHUD(CustomSpriteBatch g, AnimatedSprite ActiveTexture, int AnimationIndex, Vector2 Position)
        {
            ActiveTexture.Draw(g, AnimationIndex, Position, 0, Color.White);
        }

        public void DrawBase(CustomSpriteBatch g)
        {
            for (int Joueur = 0; Joueur < Vehicule.ArrayVehicule.Length; Joueur++)
            {
                g.Draw(sprHUD, new Vector2(Joueur * 270, 0), Color.White);

                int i = 0;

                if (Vehicule.ArrayVehicule[Joueur].ActivePowerUpSpeedTime > 0)
                {
                    i += 1;
                    g.DrawString(fntArial13, Math.Ceiling(Vehicule.ArrayVehicule[Joueur].ActivePowerUpSpeedTime / 60).ToString(), new Vector2(9 + Joueur * 270 + 30 * i, 70), Color.Silver);
                    --Vehicule.ArrayVehicule[Joueur].ActivePowerUpSpeedTime;
                    DrawPowerUpHUD(g, SprPUpVitVehiHud, 0, new Vector2(6 + Joueur * 270 + 30 * i, 45));
                }
                if (Vehicule.ArrayVehicule[Joueur].ActivePowerUpEnergieTime > 0)
                {
                    i += 1;
                    g.DrawString(fntArial13, Math.Ceiling(Vehicule.ArrayVehicule[Joueur].ActivePowerUpEnergieTime / 60).ToString(), new Vector2(9 + Joueur * 270 + 30 * i, 70), Color.Silver);
                    --Vehicule.ArrayVehicule[Joueur].ActivePowerUpEnergieTime;
                    DrawPowerUpHUD(g, SprPUpRegVehiHud, 0, new Vector2(6 + Joueur * 270 + 30 * i, 45));
                }
                for (int W = 0; W < Vehicule.ArrayVehicule[Joueur].ArrayWeaponPrimary.Length; W++)
                {
                    if (Vehicule.ArrayVehicule[Joueur].ArrayWeaponPrimary[W].ActivePowerUpDamage > 0)
                    {
                        i += 1;
                        g.DrawString(fntArial13, Math.Ceiling(Vehicule.ArrayVehicule[Joueur].ArrayWeaponPrimary[W].ActivePowerUpDamage / 60).ToString(), new Vector2(9 + Joueur * 270 + 30 * i, 70), Color.Silver);
                        --Vehicule.ArrayVehicule[Joueur].ArrayWeaponPrimary[W].ActivePowerUpDamage;
                        DrawPowerUpHUD(g, SprPUpPowHud, W, new Vector2(6 + Joueur * 270 + 30 * i, 45));
                    }
                    if (Vehicule.ArrayVehicule[Joueur].ArrayWeaponPrimary[W].ActivePowerUpRateOfFire > 0)
                    {
                        i += 1;
                        g.DrawString(fntArial13, Math.Ceiling(Vehicule.ArrayVehicule[Joueur].ArrayWeaponPrimary[W].ActivePowerUpRateOfFire / 60).ToString(), new Vector2(9 + Joueur * 270 + 30 * i, 70), Color.Silver);
                        --Vehicule.ArrayVehicule[Joueur].ArrayWeaponPrimary[W].ActivePowerUpRateOfFire;
                        DrawPowerUpHUD(g, SprPUpCadHud, W, new Vector2(6 + Joueur * 270 + 30 * i, 45));
                    }
                    if (Vehicule.ArrayVehicule[Joueur].ArrayWeaponPrimary[W].ActivePowerUpSpeed > 0)
                    {
                        i += 1;
                        g.DrawString(fntArial13, Math.Ceiling(Vehicule.ArrayVehicule[Joueur].ArrayWeaponPrimary[W].ActivePowerUpSpeed / 60).ToString(), new Vector2(9 + Joueur * 270 + 30 * i, 70), Color.Silver);
                        --Vehicule.ArrayVehicule[Joueur].ArrayWeaponPrimary[W].ActivePowerUpSpeed;
                        DrawPowerUpHUD(g, SprPUpVitHud, W, new Vector2(6 + Joueur * 270 + 30 * i, 45));
                    }
                }
                for (int W = 0; W < Vehicule.ArrayVehicule[Joueur].ArrayWeaponSecondary.Length; W++)
                {
                    if (Vehicule.ArrayVehicule[Joueur].ArrayWeaponSecondary[W].ActivePowerUpAll > 0)
                    {
                        i += 1;
                        g.DrawString(fntArial13, Math.Ceiling(Vehicule.ArrayVehicule[Joueur].ArrayWeaponSecondary[W].ActivePowerUpAll / 60).ToString(), new Vector2(9 + Joueur * 270 + 30 * i, 70), Color.Silver);
                        --Vehicule.ArrayVehicule[Joueur].ArrayWeaponSecondary[W].ActivePowerUpAll;
                        DrawPowerUpHUD(g, SprPUpSecHud, W, new Vector2(6 + Joueur * 270 + 30 * i, 45));
                    }
                }

                g.Draw(sprPixel, new Rectangle((int)Vehicule.ArrayVehicule[Joueur].Position.X - 50, (int)Vehicule.ArrayVehicule[Joueur].Position.Y - 60, 100, 6), Color.FromNonPremultiplied(0, 0, 0, 200));
                g.Draw(sprPixel, new Rectangle((int)Vehicule.ArrayVehicule[Joueur].Position.X - 49, (int)Vehicule.ArrayVehicule[Joueur].Position.Y - 59, (int)(Vehicule.ArrayVehicule[Joueur].VehiculeResist * (98 / (float)Vehicule.ArrayVehicule[Joueur].VehiculeResistMax)), 4), Color.FromNonPremultiplied(0, 255, 0, 200));

                g.Draw(sprPixel, new Rectangle((int)Vehicule.ArrayVehicule[Joueur].Position.X - 50, (int)Vehicule.ArrayVehicule[Joueur].Position.Y - 54, 100, 6), Color.FromNonPremultiplied(0, 0, 0, 200));
                g.Draw(sprPixel, new Rectangle((int)Vehicule.ArrayVehicule[Joueur].Position.X - 49, (int)Vehicule.ArrayVehicule[Joueur].Position.Y - 53, (int)(Vehicule.ArrayVehicule[Joueur].VehiculeEnergie * (98 / (float)Vehicule.ArrayVehicule[Joueur].VehiculeEnergieMax)), 4), Color.FromNonPremultiplied(0, 0, 255, 200));

                g.DrawStringMiddleAligned(fntArial10, Vehicule.ArrayVehicule[Joueur].Name, new Vector2(Vehicule.ArrayVehicule[Joueur].Position.X, Vehicule.ArrayVehicule[Joueur].Position.Y - 80), Color.Silver);
                g.DrawStringRightAligned(fntArial10, Vehicule.ArrayVehicule[Joueur].Points.ToString(), new Vector2(98 + Joueur * 270, -1), Color.White);
                g.DrawStringRightAligned(fntArial10, Vehicule.ArrayVehicule[Joueur].Name, new Vector2(121 + Joueur * 270, 23), Color.White);

                int EnergyRectMax = (int)(Vehicule.ArrayVehicule[Joueur].VehiculeEnergie * (34 / Vehicule.ArrayVehicule[Joueur].VehiculeEnergieMax));

                if (Vehicule.ArrayVehicule[Joueur].Vide)
                {
                    g.Draw(sprEnergyMeter, new Vector2(2 + Joueur * 270, 2 + sprEnergyMeter.Height - EnergyRectMax), new Rectangle(0, sprEnergyMeter.Height - EnergyRectMax, sprEnergyMeter.Width, EnergyRectMax), Color.Red);

                    g.DrawStringRightAligned(fntArial10, "Régénération en cours", new Vector2(176 + Joueur * 270, 0), Color.FromNonPremultiplied(255, 0, 0, 150));
                }
                else
                    g.Draw(sprEnergyMeter, new Vector2(2 + Joueur * 270, 2 + sprEnergyMeter.Height - EnergyRectMax), new Rectangle(0, sprEnergyMeter.Height - EnergyRectMax, sprEnergyMeter.Width, EnergyRectMax), Color.White);
            }
        }
    }

    public class Level1 : Level
    {
		public Boss1 ActiveBoss;

        public Level1()
            :base()
		{
            TimeBeforeTimePanel = 360;
            int NumberOfPanels = 10;
            TimeBeforeBoss = TimeBeforeTimePanel * NumberOfPanels;
        }

        public override void Load()
        {
            base.Load();

            if (SuperTank2.Difficulty == DifficultyChoices.Normal)
            {
                ListEnemySpawner.Add(new Plane1Spawner(5, 1));
                ListEnemySpawner.Add(new Plane2Spawner(7.5, 4));
                ListEnemySpawner.Add(new Plane3Spawner(10, 10));
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
            {
                ListEnemySpawner.Add(new Plane1Spawner(4, 9));
                ListEnemySpawner.Add(new Plane2Spawner(6, 19));
                ListEnemySpawner.Add(new Plane3Spawner(8, 30));
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
            {
                ListEnemySpawner.Add(new Plane1Spawner(2, 16));
                ListEnemySpawner.Add(new Plane2Spawner(4, 34));
                ListEnemySpawner.Add(new Plane3Spawner(5, 55));
            }

            Background2D = new AnimationScreen.AnimationBackground2D("Backgrounds 2D/Super Tank/Level 1", Content, GraphicsDevice);
            Background2D.MoveSpeed = new Vector3(5, 0, 0);

            BackgroundMusic = new FMODSound(FMODSystem, "Content/Maps/BGM/BGM_Level1.mp3");
            BackgroundMusic.SetLoop(true);
            BackgroundMusic.PlayAsBGM();

            ActiveBoss = new Boss1();
            ActiveBoss.Load(Content);
            ActiveBoss.Resist = ActiveBoss.MaxResist;
        }

        public override void Update(GameTime gameTime)
		{
            if (TimeBeforeBoss <= 0 && !ActiveBoss.IsAlive)
            {
                if (Background2D.MoveSpeed.X >= 0)
                {
                    for (int V = 0; V < Vehicule.ArrayVehicule.Length; V++)
                    {
                        if (SuperTank2.Difficulty == DifficultyChoices.Normal)
                            Vehicule.ArrayVehicule[V].Argent += 250;
                        else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
                            Vehicule.ArrayVehicule[V].Argent += 300;
                        else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
                            Vehicule.ArrayVehicule[V].Argent += 500;
                    }
                    EndLevel();
                }
                else if (ActiveBoss.Resist == ActiveBoss.MaxResist)
                {
                    ActiveBoss.IsAlive = true;
                    Enemy.AddEnemy(ActiveBoss);
                }
                else
                {
                    Background2D.MoveSpeed.X -= SlowdownSpeed;
                    if (Background2D.MoveSpeed.X <= 0)
                        Background2D.MoveSpeed.X = 0;
                }
            }

            base.Update(gameTime);
        }

		public override void Draw (CustomSpriteBatch g)
		{
			base.Draw(g);
		}
    }

    public class Level2 : Level
    {
        private Boss2 ActiveBoss;

        private int TimerBackgroundObject1;
        private int TimerBackgroundObject2;
        private int TimerBackgroundObject3;
        private int TimerBackgroundObject4;
        private int TimerBackgroundObject5;
        private int TimerBackgroundObject6;

        private Texture2D sprBat1;
        private Texture2D sprBat2;
        private Texture2D sprBat3;
        private Texture2D sprBat4;
        private Texture2D sprBat5;
        private Texture2D sprBat6;

        public Level2()
            : base()
        {
            TimerBackgroundObject1 = 0;
            TimerBackgroundObject2 = 0;
            TimerBackgroundObject3 = 0;
            TimerBackgroundObject4 = 0;
            TimerBackgroundObject5 = 0;
            TimerBackgroundObject6 = 0;

            TimeBeforeTimePanel = 360;
            int NumberOfPanels = 1;
            TimeBeforeBoss = TimeBeforeTimePanel * NumberOfPanels;
        }

        public override void Load()
        {
            base.Load();

            if (SuperTank2.Difficulty == DifficultyChoices.Normal)
            {
                ListEnemySpawner.Add(new Plane1Spawner(50, 1));
                ListEnemySpawner.Add(new Plane2Spawner(75, 4));
                ListEnemySpawner.Add(new Plane3Spawner(100, 10));
                ListEnemySpawner.Add(new Plane4Spawner(100, 15));
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
            {
                ListEnemySpawner.Add(new Plane1Spawner(40, 9));
                ListEnemySpawner.Add(new Plane2Spawner(60, 19));
                ListEnemySpawner.Add(new Plane3Spawner(60, 30));
                ListEnemySpawner.Add(new Plane4Spawner(60, 35));
            }
            else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
            {
                ListEnemySpawner.Add(new Plane1Spawner(20, 16));
                ListEnemySpawner.Add(new Plane2Spawner(40, 34));
                ListEnemySpawner.Add(new Plane3Spawner(40, 55));
                ListEnemySpawner.Add(new Plane4Spawner(40, 65));
            }

            Background2D = new AnimationScreen.AnimationBackground2D("Backgrounds 2D/Super Tank/Level 2", Content, GraphicsDevice);
            Background2D.MoveSpeed = new Vector3(5, 0, 0);

            BackgroundMusic = new FMODSound(FMODSystem, "Content/Maps/BGM/BGM_SeasideBase.mp3");
            BackgroundMusic.SetLoop(true);
            BackgroundMusic.PlayAsBGM();

            ActiveBoss = new Boss2(new Vector2(1128, 75));
            ActiveBoss.Load(Content);
            ActiveBoss.Resist = ActiveBoss.MaxResist;

           sprBat1 = Content.Load<Texture2D>("Backgrounds/Niveau 2/Bat1");
           sprBat2 = Content.Load<Texture2D>("Backgrounds/Niveau 2/Bat2");
           sprBat3 = Content.Load<Texture2D>("Backgrounds/Niveau 2/Bat3");
           sprBat4 = Content.Load<Texture2D>("Backgrounds/Niveau 2/Bat4");
           sprBat5 = Content.Load<Texture2D>("Backgrounds/Niveau 2/Bat5");
           sprBat6 = Content.Load<Texture2D>("Backgrounds/Niveau 2/Bat6");
        }

        public override void Update(GameTime gameTime)
        {
            UpdateBackgroundObject(gameTime);

            if (TimeBeforeBoss <= 0 && !ActiveBoss.IsAlive)
            {
                if (Background2D.MoveSpeed.X >= 0)
                {
                    for (int V = 0; V < Vehicule.ArrayVehicule.Length; V++)
                    {
                        if (SuperTank2.Difficulty == DifficultyChoices.Normal)
                            Vehicule.ArrayVehicule[V].Argent += 250;
                        else if (SuperTank2.Difficulty == DifficultyChoices.Hard)
                            Vehicule.ArrayVehicule[V].Argent += 300;
                        else if (SuperTank2.Difficulty == DifficultyChoices.Expert)
                            Vehicule.ArrayVehicule[V].Argent += 500;
                    }
                }
                else if (ActiveBoss.Resist == ActiveBoss.MaxResist)
                {
                    ActiveBoss.IsAlive = true;
                    Enemy.AddEnemy(ActiveBoss);
                }
                else
                {
                    Background2D.MoveSpeed.X -= SlowdownSpeed;
                    if (Background2D.MoveSpeed.X <= 0)
                        Background2D.MoveSpeed.X = 0;
                }
            }

            base.Update(gameTime);
        }

        public void UpdateBackgroundObject(GameTime gameTime)
        {
            TimerBackgroundObject1--;

            if (TimerBackgroundObject1 <= 0)
            {
                TimerBackgroundObject1 = 400 + SuperTank2.Randomizer.Next(400) * 2;
                AddBackgroundObject(new BackgroundObject(sprBat1, new Vector2(Constants.Width, 466)));
            }

            TimerBackgroundObject2--;

            if (TimerBackgroundObject2 <= 0)
            {
                TimerBackgroundObject2 = 200 + SuperTank2.Randomizer.Next(200);
                AddBackgroundObject(new BackgroundObject(sprBat2, new Vector2(Constants.Width, 525)));
            }

            TimerBackgroundObject3--;

            if (TimerBackgroundObject3 <= 0)
            {
                TimerBackgroundObject3 = 300 + SuperTank2.Randomizer.Next(300);
                AddBackgroundObject(new BackgroundObject(sprBat3, new Vector2(Constants.Width, 405)));
            }

            TimerBackgroundObject4--;

            if (TimerBackgroundObject4 <= 0)
            {
                TimerBackgroundObject4 = 800 + SuperTank2.Randomizer.Next(800);
                AddBackgroundObject(new BackgroundObject(sprBat4, new Vector2(Constants.Width, 411)));
            }

            TimerBackgroundObject5--;

            if (TimerBackgroundObject5 <= 0)
            {
                TimerBackgroundObject5 = 600 + SuperTank2.Randomizer.Next(600);
                AddBackgroundObject(new BackgroundObject(sprBat5, new Vector2(Constants.Width, 464)));
            }

            TimerBackgroundObject6--;

            if (TimerBackgroundObject6 <= 0)
            {
                TimerBackgroundObject6 = 1200 + SuperTank2.Randomizer.Next(1200);
                AddBackgroundObject(new BackgroundObject(sprBat6, new Vector2(Constants.Width, 428)));
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);
        }
    }
}
