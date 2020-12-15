using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    public abstract class Vehicule : AnimatedSprite
    {
        public static Vehicule[] ArrayVehicule;
        protected const Keys NextWeaponKey = Keys.E;
        protected const Keys PreviousWeaponKey = Keys.Q;
        protected const Keys SpecialKey = Keys.Space;
        protected readonly Keys[] ArrayWeaponKey = { Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7 };

        #region Variables
        
        public int LeftLim;
        public int RightLim;
        public int Height;
        public int Protec = 1;

        public float ActivePowerUpSpeedTime;
        public float ActivePowerUpEnergieTime;

        public Vector2 PositionOld;
        public Vector2 BackgroundSpeed;
        public bool IsDestroyed;
        public bool Vide;
        public float AnimationIndex;

        public int ArmeSelect1;
        public int ArmeSelect2;
        public int Old;
        public int New;
        public float Arme1ImgSelect;

        public int VehiculeLevel;

        public double VehiculeEnergieMax;
        public double VehiculeEnergie;
        public float VehiculeRegen;
        public float VehiculeResist;
        public int VehiculeResistMax;
        public float VehiculePerte;
        public int VehiculeVitesse;
        public int VehiculeSpecial;

        public int VehiculeEnergieLvl;
        public int VehiculeRegenLvl;
        public int VehiculeResistLvl;
        public int VehiculePerteLvl;
        public int VehiculeVitesseLvl;
        public int VehiculeSpecialLvl;

        public int EnergyMod;
        public int ResistMod;
        public int SpeedMod;

        public double Tirer1;
        public double Tirer2;
        public double Recul;
        public bool God;
        public int Argent;
        public int Points;
        public string Name;
        public float CanonAngle;

        private List<Bullet> ListBullet;
        private List<BulletCase> ListBulletCase;

        public Weapon[] ArrayWeaponPrimary;
        public Weapon[] ArrayWeaponSecondary;

        #endregion

        public Vehicule()
        {
            Name = "";
            ListBullet = new List<Bullet>();
            ListBulletCase = new List<BulletCase>();
            VehiculeEnergieMax = 100;
            VehiculeResistMax = 100;
        }

        public abstract void Spawn();

        public float GetAngle()
        {
            double Angle = Math.Atan2(MouseHelper.MouseStateCurrent.Y - Position.Y, MouseHelper.MouseStateCurrent.X - Position.X);

            if (Angle > 0)
            {
                if (Angle < Math.PI * 0.5f)
                    Angle = 0;
                else
                    Angle = Math.PI;
            }
            return (float)Angle;
        }

        public abstract void Load(ContentManager Content);

        public virtual void Update(GameTime gameTime)
        {
            UpdateTransformationMatrix();

            for (int B = 0; B < ListBullet.Count; B++)
            {
				if (ListBullet[B].Resist > 0)
                {
                    ListBullet[B].Update(gameTime);
                    for (int P = 0; P < Enemy.ListEnemy.Count; P++)
                    {
                        if (Enemy.ListEnemy[P].IsAlive)
                        {
                            if (ListBullet[B].PixelIntersect(Enemy.ListEnemy[P]))
                            {
                                ListBullet[B].Resist--;
                                Enemy.ListEnemy[P].Resist -= ListBullet[B].Damage;
                                for (int i = 0; i < ListBullet[B].Damage; i++)
                                {
                                    new FlameParticle(Enemy.ListEnemy[P].Position, SuperTank2.Randomizer.Next(360) * SuperTank2.DegToRad, SuperTank2.Randomizer.NextDouble() * 5).AddParticule();
                                }
                                if (Enemy.ListEnemy[P].Resist <= 0)
                                {
                                    Enemy.ListEnemy[P].Destroyed(this);
                                }
                            }
                        }
                    }
                    for (int BombIndex = 0; BombIndex < EnemyAttack.ListEnemyAttack.Count; BombIndex++)
                    {
                        if (EnemyAttack.ListEnemyAttack[BombIndex].Resist > 0)
                        {
                            if (ListBullet[B].PixelIntersect(EnemyAttack.ListEnemyAttack[BombIndex]))
                            {
                                if (EnemyAttack.ListEnemyAttack[BombIndex].AttackType == EnemyAttack.AttackTypes.LaserBomb)
                                {
                                    if (ListBullet[B].BulletType == Bullet.BulletTypes.Normal)
                                    {
                                        ListBullet[B].Resist -= 0.5f;
                                    }
                                    else if (ListBullet[B].BulletType == Bullet.BulletTypes.Laser)
                                    {
                                        ListBullet[B].Resist -= 0.5f;
                                        EnemyAttack.ListEnemyAttack[BombIndex].Resist -= ListBullet[B].Damage;
                                    }
                                }
                                else if (EnemyAttack.ListEnemyAttack[BombIndex].AttackType == EnemyAttack.AttackTypes.Laser)
                                {
                                    ListBullet[B].Resist -= 0.5f;
                                }
                                else
                                {
                                    ListBullet[B].Resist -= 0.5f;
                                    EnemyAttack.ListEnemyAttack[BombIndex].Resist -= ListBullet[B].Damage;
                                    for (int i = 0; i < 5; i++)
                                    {
                                        new FlameParticle(EnemyAttack.ListEnemyAttack[BombIndex].Position, SuperTank2.Randomizer.Next(360) * SuperTank2.DegToRad, SuperTank2.Randomizer.NextDouble() * 5).AddParticule();
                                    }
                                }
                                if (EnemyAttack.ListEnemyAttack[BombIndex].Resist <= 0)
                                {
                                    EnemyAttack.ListEnemyAttack[BombIndex].Destroyed(this);
                                }
                            }
                        }
                    }
                    if (ListBullet[B].Resist <= 0)
                        ListBullet[B].Destroyed(this);
                }
            }
            for (int B = 0; B < ListBulletCase.Count; B++)
            {
                if (ListBulletCase[B].IsAlive)
                    ListBulletCase[B].Update(gameTime);
            }
        }

        public virtual bool ActivatePowerUp(PowerUp.PowerUpTypes PowerUpType)
        {
            int ActiveTime = 600;
            switch (PowerUpType)
            {
                case PowerUp.PowerUpTypes.VehiculeRegen:
                    ActivePowerUpEnergieTime = ActiveTime;
                    return true;
                case PowerUp.PowerUpTypes.Shield:
                    return true;
                case PowerUp.PowerUpTypes.VehiculeRepair:
                    return true;
                case PowerUp.PowerUpTypes.VehiculeSpeed:
                    ActivePowerUpSpeedTime = ActiveTime;
                    return true;
                case PowerUp.PowerUpTypes.Power1:
                    ArrayWeaponPrimary[0].ActivePowerUpDamage = ActiveTime;
                    return true;
                case PowerUp.PowerUpTypes.Power2:
                    ArrayWeaponPrimary[1].ActivePowerUpDamage = ActiveTime;
                    return true;
                case PowerUp.PowerUpTypes.Power3:
                    ArrayWeaponPrimary[2].ActivePowerUpDamage = ActiveTime;
                    return true;
                case PowerUp.PowerUpTypes.RateOfFire1:
                    ArrayWeaponPrimary[0].ActivePowerUpRateOfFire = ActiveTime;
                    return true;
                case PowerUp.PowerUpTypes.RateOfFire2:
                    ArrayWeaponPrimary[1].ActivePowerUpRateOfFire = ActiveTime;
                    return true;
                case PowerUp.PowerUpTypes.RateOfFire3:
                    ArrayWeaponPrimary[2].ActivePowerUpRateOfFire = ActiveTime;
                    return true;
                case PowerUp.PowerUpTypes.Speed1:
                    ArrayWeaponPrimary[0].ActivePowerUpSpeed = ActiveTime;
                    return true;
                case PowerUp.PowerUpTypes.Speed2:
                    ArrayWeaponPrimary[1].ActivePowerUpSpeed = ActiveTime;
                    return true;
                case PowerUp.PowerUpTypes.Speed3:
                    ArrayWeaponPrimary[2].ActivePowerUpSpeed = ActiveTime;
                    return true;
            }
            return false;
        }

        public void AddBullet(Bullet NewBullet)
        {
            for (int B = 0; B < ListBullet.Count; B++)
            {
				if (ListBullet[B].Resist <= 0)
                {
                    ListBullet[B] = NewBullet;
                    return;
                }
            }
            ListBullet.Add(NewBullet);
        }

        public void AddBulletCase(BulletCase NewBulletCase)
        {
            for (int B = 0; B < ListBulletCase.Count; B++)
            {
                if (!ListBulletCase[B].IsAlive)
                {
                    ListBulletCase[B] = NewBulletCase;
                    return;
                }
            }
            ListBulletCase.Add(NewBulletCase);
        }

        public abstract void DrawVehicule(CustomSpriteBatch g);

        public void Draw(CustomSpriteBatch g)
        {
            for (int B = 0; B < ListBullet.Count; B++)
            {
				if (ListBullet[B].Resist > 0)
                    ListBullet[B].Draw(g);
            }
            DrawVehicule(g);
            for (int B = 0; B < ListBulletCase.Count; B++)
            {
                if (ListBulletCase[B].IsAlive)
                    ListBulletCase[B].Draw(g, ListBulletCase[B].Position, 1, ListBulletCase[B].Angle, Color.White);
            }
        }
    }
}