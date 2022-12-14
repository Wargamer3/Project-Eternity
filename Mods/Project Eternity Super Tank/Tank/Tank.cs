using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    class Tank : Vehicule
    {
        private enum SpecialTypes { None, Jump, Laser, FireEverything, BulletTime, Armageddon }

        private class SuperLaser : AnimatedSprite
        {
            public SuperLaser(Vector2 Position, Color[] Mask, Texture2D Clone)
                : base(Clone, Mask, 1, new Vector2(0, 8))
            {
            }
        }

        private class Meteor : Bullet
        {
            public class MeteorTrace1 : Particule
            {
                public static AnimatedSprite TraceSprite;

                public MeteorTrace1(Vector2 Position)
                    : base(TraceSprite, Position, Vector2.Zero)
                {
                }

                public override void Update(GameTime gameTime)
                {
                    Scale.X -= 0.05f;
                    Scale.Y -= 0.05f;
                    if (Scale.X < 0.05)
                        IsAlive = false;
                }
            }

            public class MeteorTrace2 : Particule
            {
                public static AnimatedSprite TraceSprite;

                public MeteorTrace2(Vector2 Position)
                    : base(TraceSprite, Position, Vector2.Zero)
                {
                }

                public override void Update(GameTime gameTime)
                {
                    Scale.X -= 0.05f;
                    Scale.Y -= 0.05f;
                    if (Scale.X < 0.05)
                        IsAlive = false;
                }
            }

            bool UseGlasses;
            bool IsMeteorType1;

            public Meteor(Vector2 Position, Color[] Mask, AnimatedSprite Clone, bool IsMeteorType1)
                : base(Position, Mask, 0, Clone)
            {
                this.IsMeteorType1 = IsMeteorType1;
                Damage = 10;

                UseGlasses = SuperTank2.Randomizer.Next(25) == 0;

                if (UseGlasses)
                    Resist = 10;
                else
                    Resist = 5;

                SetSpeed(-(240 + SuperTank2.Randomizer.Next(60)) * SuperTank2.DegToRad, 4 + SuperTank2.Randomizer.Next(3));
                Angle = SuperTank2.Randomizer.Next(360) * SuperTank2.DegToRad;
            }

            public override void Update(GameTime gameTime)
            {
                Position += Speed;

                UpdateTransformationMatrix();

                if (IsMeteorType1)
                    new MeteorTrace1(Position).AddParticule();
                else
                    new MeteorTrace2(Position).AddParticule();

                if (Position.X < -10 || Position.X > Constants.Width || Position.Y > Constants.Height)
                    Resist = 0;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                base.Draw(g);
                if (UseGlasses && IsMeteorType1)
                    Tank.sprMeteorGlasses.Draw(g, 0, Position, Angle, Color.White);
            }
        }

        #region Ressources

        AnimatedSprite sprBulletNormal;
        AnimatedSprite sprBulletHeavy;
        AnimatedSprite sprBulletLaser;
        AnimatedSprite sprBalleFlak;
        AnimatedSprite sprMissile;
        AnimatedSprite sprMissileG;
        AnimatedSprite sprLaser;
        Texture2D sprOrb;
        AnimatedSprite sprShuriken;
        Texture2D sprSuperLaser;
        public static AnimatedSprite sprMeteorGlasses;
        AnimatedSprite sprMeteor1;
        AnimatedSprite sprMeteor2;
        public Color[][] MaskBulletNormal;
        public Color[][] MaskBulletHeavy;
        public Color[][] MaskBulletLaser;
        public Color[] MaskFlak;
        public Color[] MaskMissile;
        public Color[] MaskMissileG;
        public Color[] MaskLaser;
        public Color[][] MaskShuriken;
        public Color[] MaskSuperLaser;
        public Color[] MaskMeteor1;
        public Color[] MaskMeteor2;

        AnimatedSprite sprBulletCaseNormal;
        AnimatedSprite sprBulletCaseHeavy;
        AnimatedSprite sprLaserParticule;
        AnimatedSprite sprBulletCaseFlak;
        AnimatedSprite sprSmoke;
        
        SpriteFont fntArial26;

        AnimatedSprite sprTank;
        AnimatedSprite sprWheels;
        AnimatedSprite sprTankShield;
        AnimatedSprite sprWeaponIcon;
        Texture2D sprSpecialIcon;
        AnimatedSprite sprShadowBottom;
        AnimatedSprite sprShadowTop;
        AnimatedSprite sprShadowMisc1;
        AnimatedSprite sprShadowMisc2;

        AnimatedSprite sprCanonTank;
        AnimatedSprite sprCanon1_2;
        AnimatedSprite sprCanon1_3;
        AnimatedSprite sprCanon2_1;
        AnimatedSprite sprCanon2_3;
        AnimatedSprite sprCanon3_1;
        AnimatedSprite sprCanon3_2;

        AnimatedSprite sprCanonflak1;
        AnimatedSprite sprCanonflak2;
        AnimatedSprite sprCanonflak3;
        AnimatedSprite sprLanceMissile1;
        AnimatedSprite sprLanceMissile2;
        AnimatedSprite sprLanceMissile3;
        AnimatedSprite sprLanceMissileG1;
        AnimatedSprite sprLanceMissileG2;
        AnimatedSprite sprLanceMissileG3;
        AnimatedSprite sprCanonLaser1;
        AnimatedSprite sprCanonLaser2;
        AnimatedSprite sprCanonLaser3;
        AnimatedSprite sprLanceShuriken1;
        AnimatedSprite sprLanceShuriken2;
        AnimatedSprite sprLanceShuriken3;

        #endregion

        public float ShurikenAngle = 0;

        public bool OrbeActif = false;
        public TankOrb[] ArrayTankOrb;
        public TankTurret[] ArrayTankTourelle;
        Vector2 Speed;
        int Rage;
        SuperLaser ActiveSuperLaser;
        List<Meteor> ListMeteor;

        private int Special;
        private int SpecialTimer3;
        private SpecialTypes ActiveSpecial;

        public Tank()
            : base()
        {
            ListMeteor = new List<Meteor>();
            
            CanonAngle = -90 * SuperTank2.DegToRad;
            VehiculeResist = 100;
        }

        public override void Load(ContentManager Content)
        {
            LoadWeapons();
            LeftLim = -62;
            RightLim = 62;
            Height = 715;
            Protec = 1;
            Position = new Vector2(400, Height);
            PositionOld = Position;

            sprPixel = Content.Load<Texture2D>("Pixel");
            fntArial26 = Content.Load<SpriteFont>("Fonts/Arial26");

            sprBulletNormal = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Bullet normal"), 9, new Vector2(8, 3));
            sprBulletHeavy = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Bullet heavy"), 9, new Vector2(7, 9));
            sprBulletLaser = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Bullet laser"), 9, new Vector2(17, 15));
            
            MaskBulletNormal = sprBulletNormal.CreateMask();
            MaskBulletHeavy = sprBulletHeavy.CreateMask();
            MaskBulletLaser = sprBulletLaser.CreateMask();
            
            sprBalleFlak = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Flak bullet"), 1, new Vector2(5, 4));
            sprBulletCaseFlak = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Flak case"), 1, new Vector2(7, 2));
            FlakFragment.FlakSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Flak fragment"), 1, new Vector2(4, 4));
            FlakTrace.FlakSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Flak trace"), 1, new Vector2(3, 3));

            MaskFlak = new Color[sprBalleFlak.SpriteWidth * sprBalleFlak.SpriteHeight];
            sprBalleFlak.ActiveSprite.GetData(MaskFlak);
            FlakFragment.FlakMask = new Color[FlakFragment.FlakSprite.SpriteWidth * FlakFragment.FlakSprite.SpriteHeight];
            FlakFragment.FlakSprite.ActiveSprite.GetData(FlakFragment.FlakMask);

            sprMissile = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Missile"), 1, new Vector2(9, 6));
            MaskMissile = new Color[sprMissile.SpriteWidth * sprMissile.SpriteHeight];
            sprMissile.ActiveSprite.GetData(MaskMissile);

            sprMissileG = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Guided missile"), 1, new Vector2(9, 8));
            MaskMissileG = new Color[sprMissileG.SpriteWidth * sprMissileG.SpriteHeight];
            sprMissileG.ActiveSprite.GetData(MaskMissileG);

            sprLaser = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Laser"), 1, new Vector2(12, 5));
            MaskLaser = new Color[sprLaser.SpriteWidth * sprLaser.SpriteHeight];
            sprLaser.ActiveSprite.GetData(MaskLaser);

            TankTurret.ImageTourelle = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Turret"), 3, new Vector2(16, 24));
            sprOrb = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Orb");
            sprShuriken = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Shuriken"), 3, new Vector2(19, 19));

            MaskShuriken = sprShuriken.CreateMask();

            sprMeteor1 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Meteor 1"), 1, new Vector2(21, 21));
            sprMeteor2 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Meteor 2"), 1, new Vector2(15, 15));

            MaskMeteor1 = new Color[sprMeteor1.SpriteWidth * sprMeteor1.SpriteHeight];
            sprMeteor1.ActiveSprite.GetData(MaskMeteor1);

            MaskMeteor2 = new Color[sprMeteor2.SpriteWidth * sprMeteor2.SpriteHeight];
            sprMeteor2.ActiveSprite.GetData(MaskMeteor2);

            Meteor.MeteorTrace1.TraceSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Meteor trace 1"), 1, new Vector2(16, 16));
            Meteor.MeteorTrace2.TraceSprite = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Meteor trace 2"), 1, new Vector2(12, 12));
            sprMeteorGlasses = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Meteor glasses"), 1, new Vector2(16, 16));

            sprBulletCaseNormal = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Bullet case normal"), 9, new Vector2(6, 4));
            sprBulletCaseHeavy = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Bullet case heavy"), 9, new Vector2(8, 9));
            sprLaserParticule = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Laser particule"), 1, new Vector2(6, 6));
            sprSmoke = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Smoke"), 5, new Vector2(16, 16));

			sprWeaponIcon = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Weapon icons"), 7, new Vector2(0, 0));
            sprSpecialIcon = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Misc/Special icon");
            sprSuperLaser = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Special laser");

            MaskSuperLaser = new Color[sprSuperLaser.Width * sprSuperLaser.Height];
            sprSuperLaser.GetData(MaskSuperLaser);

            sprTank = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Tank"), 1, new Vector2(90, 41));
            SpriteWidth = sprTank.SpriteWidth;
            SpriteHeight = sprTank.SpriteHeight;
            Origin = sprTank.Origin;
            Mask = new Color[sprTank.SpriteWidth * sprTank.SpriteHeight];
            sprTank.ActiveSprite.GetData(Mask);

			sprTankShield = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Shield"), 1, new Vector2(90, 30));
            sprShadowMisc1 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/TankOmbreMisc"), 1, new Vector2(0, 0));
            sprShadowMisc2 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/TankOmbreMisc"), 1, new Vector2(0, 0));
            sprShadowTop = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/TankOmbreTop"), 1, new Vector2(0, 0));
            sprShadowBottom = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/TankOmbreBas"), 3, new Vector2(0, 0));
            sprWheels = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/TankRoue"), 3, new Vector2(0, 0));

            #region Weapons

            sprCanonTank = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Canon"), 4, new Vector2(0, 3));
            sprCanon1_2 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Canon1_2"), 13, new Vector2(0, 4));
            sprCanon1_3 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Canon1_3"), 13, new Vector2(0, 3));
            sprCanon2_1 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Canon2_1"), 13, new Vector2(0, 4));
            sprCanon2_3 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Canon2_3"), 13, new Vector2(0, 4));
			sprCanon3_1 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Canon3_1"), 13, new Vector2(0, 3));
			sprCanon3_2 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Canon3_2"), 13, new Vector2(0, 4));

            sprCanonflak1 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Flak Canon 1"), 1, new Vector2(0, 3));
            sprCanonflak2 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Flak Canon 2"), 1, new Vector2(0, 3));
            sprCanonflak3 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Flak Canon 3"), 1, new Vector2(0, 3));
            sprLanceMissile1 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Missile launcher 1"), 1, new Vector2(0, 25));
            sprLanceMissile2 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Missile launcher 2"), 1, new Vector2(0, 25));
            sprLanceMissile3 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Missile launcher 3"), 1, new Vector2(0, 25));
            sprLanceMissileG1 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Missile launcher guided 1"), 1, new Vector2(16, 10));
            sprLanceMissileG2 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Missile launcher guided 2"), 1, new Vector2(16, 10));
            sprLanceMissileG3 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Missile launcher guided 3"), 1, new Vector2(25, 10));
            sprCanonLaser1 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Laser canon 1"), 1, new Vector2(0, 5));
            sprCanonLaser2 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Laser canon 2"), 1, new Vector2(0, 15));
            sprCanonLaser3 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Laser canon 3"), 1, new Vector2(0, 16));
            sprLanceShuriken1 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Shuriken launcher 1"), 1, new Vector2(12, 13));
            sprLanceShuriken2 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Shuriken launcher 2"), 1, new Vector2(12, 12));
            sprLanceShuriken3 = new AnimatedSprite(Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Tank/Secondary weapons/Shuriken launcher 3"), 1, new Vector2(19, 19));

            #endregion
        }

		public void LoadWeapons()
        {
            ArrayWeaponPrimary = new Weapon[3];
            ArrayWeaponSecondary = new Weapon[7];
			VehiculeLevel = 0;
			VehiculeRegen = 0.1f;
			VehiculeResist = 100;
			VehiculePerte = 0.1f;
			VehiculeVitesse = 4;
			VehiculeSpecial = 9;
            VehiculeEnergie = VehiculeEnergieMax;
			VehiculeResistLvl = 1;
			VehiculePerteLvl = 1;
			VehiculeVitesseLvl = 1;
			VehiculeSpecialLvl = 1;
			//Le pickup utilise les m?e espaces d'armes que le tank
			for (int i = 0; i < 10; i += 1)//Arme 0-2=canon,3-9=armes sp?iales
			{
				Weapon NewWeapon = new Weapon();

				NewWeapon.WeaponSpecial = 0;
				NewWeapon.WeaponRateOfFireLvl = 1;
				NewWeapon.WeaponSpreadLvl = 1;
				NewWeapon.WeaponEnergyLvl = 1;
				NewWeapon.WeaponDamageLvl = 1;
				NewWeapon.WeaponResistLvl = 1;
				NewWeapon.WeaponSpeedLvl = 1;
				NewWeapon.WeaponSpecialLvl = 0;

                if (i < 3)
                {
                    NewWeapon.ArmeLevel = 0;
                    ArrayWeaponPrimary[i] = NewWeapon;
                }
                else
                {
                    NewWeapon.ArmeLevel = 0;
                    ArrayWeaponSecondary[i - 3] = NewWeapon;
                }
            }
            ArrayWeaponPrimary[0].RateOfFireMod = 0.25f;
            ArrayWeaponPrimary[1].RateOfFireMod = 0.5f;
            ArrayWeaponPrimary[2].RateOfFireMod = 0.2f;
            ArrayWeaponSecondary[0].RateOfFireMod = 0.25f;
            ArrayWeaponSecondary[1].RateOfFireMod = 0.5f;
            ArrayWeaponSecondary[2].RateOfFireMod = 0.75f;
            ArrayWeaponSecondary[3].RateOfFireMod = 1f;
            ArrayWeaponSecondary[4].RateOfFireMod = 1f;
            ArrayWeaponSecondary[5].RateOfFireMod = 0.25f;
            ArrayWeaponSecondary[6].RateOfFireMod = 0.2f;

            ArrayWeaponPrimary[0].SpreadMod = 1;
            ArrayWeaponPrimary[1].SpreadMod = 1;
            ArrayWeaponPrimary[2].SpreadMod = 1;
            ArrayWeaponSecondary[0].SpreadMod = 2;
            ArrayWeaponSecondary[1].SpreadMod = 1;
            ArrayWeaponSecondary[2].SpreadMod = 1;
            ArrayWeaponSecondary[3].SpreadMod = 1;
            ArrayWeaponSecondary[4].SpreadMod = 1;
            ArrayWeaponSecondary[5].SpreadMod = 1;
            ArrayWeaponSecondary[6].SpreadMod = 1;

            ArrayWeaponPrimary[0].EnergyMod = 0.05f;
            ArrayWeaponPrimary[1].EnergyMod = 0.1f;
            ArrayWeaponPrimary[2].EnergyMod = 0.05f;
            ArrayWeaponSecondary[0].EnergyMod = 0.25f;
            ArrayWeaponSecondary[1].EnergyMod = 0.5f;
            ArrayWeaponSecondary[2].EnergyMod = 0.75f;
            ArrayWeaponSecondary[3].EnergyMod = 1;
            ArrayWeaponSecondary[4].EnergyMod = 1;
            ArrayWeaponSecondary[5].EnergyMod = 0.25f;
            ArrayWeaponSecondary[6].EnergyMod = 0.2f;

            ArrayWeaponPrimary[0].PowerMod = 0.25f;
            ArrayWeaponPrimary[1].PowerMod = 0.5f;
            ArrayWeaponPrimary[2].PowerMod = 0.2f;
            ArrayWeaponSecondary[0].PowerMod = 0.25f;
            ArrayWeaponSecondary[1].PowerMod = 0.5f;
            ArrayWeaponSecondary[2].PowerMod = 0.75f;
            ArrayWeaponSecondary[3].PowerMod = 1;
            ArrayWeaponSecondary[4].PowerMod = 1;
            ArrayWeaponSecondary[5].PowerMod = 1;
            ArrayWeaponSecondary[6].PowerMod = 1;

            ArrayWeaponPrimary[0].ResistMod = 0.5f;
            ArrayWeaponPrimary[1].ResistMod = 1f;
            ArrayWeaponPrimary[2].ResistMod = 0.5f;
            ArrayWeaponSecondary[0].ResistMod = 0.5f;
            ArrayWeaponSecondary[1].ResistMod = 1f;
            ArrayWeaponSecondary[2].ResistMod = 0.5f;
            ArrayWeaponSecondary[3].ResistMod = 2;
            ArrayWeaponSecondary[4].ResistMod = 0.5f;
            ArrayWeaponSecondary[5].ResistMod = 0.5f;
            ArrayWeaponSecondary[6].ResistMod = 0.5f;

            ArrayWeaponPrimary[0].SpeedMod = 0.25f;
            ArrayWeaponPrimary[1].SpeedMod = 0.5f;
            ArrayWeaponPrimary[2].SpeedMod = 0.2f;
            ArrayWeaponSecondary[0].SpeedMod = 0.25f;
            ArrayWeaponSecondary[1].SpeedMod = 0.5f;
            ArrayWeaponSecondary[2].SpeedMod = 0.75f;
            ArrayWeaponSecondary[3].SpeedMod = 1;
            ArrayWeaponSecondary[4].SpeedMod = 1;
            ArrayWeaponSecondary[5].SpeedMod = 0.25f;
            ArrayWeaponSecondary[6].SpeedMod = 0.2f;

			//canon 1
			ArrayWeaponPrimary[0].WeaponRateOfFire = 10;
            ArrayWeaponPrimary[0].WeaponSpread = 1;
            ArrayWeaponPrimary[0].WeaponEnergy = 1;
            ArrayWeaponPrimary[0].WeaponDamage = 1;
            ArrayWeaponPrimary[0].WeaponResist = 1;
            ArrayWeaponPrimary[0].WeaponSpeed = 7;
			//canon 2
            ArrayWeaponPrimary[1].WeaponRateOfFire = 20;
            ArrayWeaponPrimary[1].WeaponSpread = 1;
            ArrayWeaponPrimary[1].WeaponEnergy = 2;
            ArrayWeaponPrimary[1].WeaponDamage = 5;
            ArrayWeaponPrimary[1].WeaponResist = 2;
            ArrayWeaponPrimary[1].WeaponSpeed = 6;
			//canon 3
            ArrayWeaponPrimary[2].WeaponRateOfFire = 8;
            ArrayWeaponPrimary[2].WeaponSpread = 1;
            ArrayWeaponPrimary[2].WeaponEnergy = 0.5f;
            ArrayWeaponPrimary[2].WeaponDamage = 0.5f;
            ArrayWeaponPrimary[2].WeaponResist = 0.5f;
            ArrayWeaponPrimary[2].WeaponSpeed = 15;
			//canon flak
			ArrayWeaponSecondary[0].WeaponRateOfFire = 20;
            ArrayWeaponSecondary[0].WeaponSpread = 2;
            ArrayWeaponSecondary[0].WeaponEnergy = 7;
            ArrayWeaponSecondary[0].WeaponDamage = 3;
            ArrayWeaponSecondary[0].WeaponResist = 1;
            ArrayWeaponSecondary[0].WeaponSpeed = 5;
			//lance missile
            ArrayWeaponSecondary[1].WeaponRateOfFire = 25;
            ArrayWeaponSecondary[1].WeaponSpread = 1;
            ArrayWeaponSecondary[1].WeaponEnergy = 10;
            ArrayWeaponSecondary[1].WeaponDamage = 7;
            ArrayWeaponSecondary[1].WeaponResist = 2;
            ArrayWeaponSecondary[1].WeaponSpeed = 8;
			//missile guid?
            ArrayWeaponSecondary[2].WeaponRateOfFire = 28;
            ArrayWeaponSecondary[2].WeaponSpread = 1;
            ArrayWeaponSecondary[2].WeaponEnergy = 10;
            ArrayWeaponSecondary[2].WeaponDamage = 5;
            ArrayWeaponSecondary[2].WeaponResist = 1;
            ArrayWeaponSecondary[2].WeaponSpeed = 10;
			//laser
            ArrayWeaponSecondary[3].WeaponRateOfFire = 40;
            ArrayWeaponSecondary[3].WeaponSpread = 1;
            ArrayWeaponSecondary[3].WeaponEnergy = 13;
            ArrayWeaponSecondary[3].WeaponDamage = 10;
            ArrayWeaponSecondary[3].WeaponResist = 2;
            ArrayWeaponSecondary[3].WeaponSpeed = 40;
			//orbe
            ArrayWeaponSecondary[4].WeaponRateOfFire = 30;
            ArrayWeaponSecondary[4].WeaponSpread = 1;
            ArrayWeaponSecondary[4].WeaponEnergy = 15;
            ArrayWeaponSecondary[4].WeaponDamage = 2;
            ArrayWeaponSecondary[4].WeaponResist = 1;
            ArrayWeaponSecondary[4].WeaponSpeed = 2;
			//tourelle
            ArrayWeaponSecondary[5].WeaponRateOfFire = 15;
            ArrayWeaponSecondary[5].WeaponSpread = 1;
            ArrayWeaponSecondary[5].WeaponEnergy = 6;
            ArrayWeaponSecondary[5].WeaponDamage = 2;
            ArrayWeaponSecondary[5].WeaponResist = 1;
            ArrayWeaponSecondary[5].WeaponSpeed = 15;
			//shuriken
            ArrayWeaponSecondary[6].WeaponRateOfFire = 10;
            ArrayWeaponSecondary[6].WeaponSpread = 1;
            ArrayWeaponSecondary[6].WeaponEnergy = 5;
            ArrayWeaponSecondary[6].WeaponDamage = 1;
            ArrayWeaponSecondary[6].WeaponResist = 0.5f;
            ArrayWeaponSecondary[6].WeaponSpeed = 20;
		}

        public override void Spawn()
        {
            ActivePowerUpSpeedTime = 0;
            ActivePowerUpEnergieTime = 0;

            for (int W = 0; W < ArrayWeaponPrimary.Length; W++)
            {
                ArrayWeaponPrimary[W].ActivePowerUpAll = 0;
                ArrayWeaponPrimary[W].ActivePowerUpRateOfFire = 0;
                ArrayWeaponPrimary[W].ActivePowerUpDamage = 0;
                ArrayWeaponPrimary[W].ActivePowerUpSpeed = 0;
            }

            for (int W = 0; W < ArrayWeaponSecondary.Length; W++)
            {
                ArrayWeaponSecondary[W].ActivePowerUpAll = 0;
                ArrayWeaponSecondary[W].ActivePowerUpRateOfFire = 0;
                ArrayWeaponSecondary[W].ActivePowerUpDamage = 0;
                ArrayWeaponSecondary[W].ActivePowerUpSpeed = 0;
            }

            ArmeSelect2 = -1;

            if (ArrayWeaponSecondary[0].ArmeLevel > 0)
                ArmeSelect2 = 0;
            else if (ArrayWeaponSecondary[1].ArmeLevel > 0)
                ArmeSelect2 = 1;
            else if (ArrayWeaponSecondary[2].ArmeLevel > 0)
                ArmeSelect2 = 2;
            else if (ArrayWeaponSecondary[3].ArmeLevel > 0)
                ArmeSelect2 = 3;
            else if (ArrayWeaponSecondary[4].ArmeLevel > 0)
                OrbeActif = !OrbeActif;
            else if (ArrayWeaponSecondary[5].ArmeLevel > 0)
                ArmeSelect2 = 5;
            else if (ArrayWeaponSecondary[6].ArmeLevel > 0)
                ArmeSelect2 = 6;

            VehiculeEnergie = VehiculeEnergieMax;
            VehiculeResist = VehiculeResistMax;

            #region Créer les orbes défensive

            if (ArrayWeaponSecondary[4].ArmeLevel > 0)
            {
                ArrayTankOrb = new TankOrb[ArrayWeaponSecondary[4].WeaponSpread];
                if (ArrayWeaponSecondary[4].WeaponSpread >= 1)
                {
                    TankOrb TankOrbe1 = new TankOrb();
                    TankOrbe1.Dir = 0;
                    TankOrbe1.Resist = ArrayWeaponSecondary[4].WeaponResist;
                    TankOrbe1.Creator = this;
                    ArrayTankOrb[0] = TankOrbe1;

                    if (ArrayWeaponSecondary[4].WeaponSpread >= 2)
                    {
                        TankOrb TankOrbe2 = new TankOrb();
                        TankOrbe2.Dir = 180 * SuperTank2.DegToRad;
                        TankOrbe2.Resist = ArrayWeaponSecondary[4].WeaponResist;
                        TankOrbe2.Creator = this;
                        ArrayTankOrb[1] = TankOrbe2;

                        if (ArrayWeaponSecondary[4].WeaponSpread >= 3)
                        {
                            TankOrbe2.Dir = 280 * SuperTank2.DegToRad;
                            TankOrb TankOrbe3 = new TankOrb();
                            TankOrbe3.Dir = 120 * SuperTank2.DegToRad;
                            TankOrbe3.Resist = ArrayWeaponSecondary[4].WeaponResist;
                            TankOrbe3.Creator = this;
                            ArrayTankOrb[2] = TankOrbe3;

                            if (ArrayWeaponSecondary[4].WeaponSpread >= 4)
                            {
                                TankOrbe2.Dir = 90 * SuperTank2.DegToRad;
                                TankOrbe3.Dir = 180 * SuperTank2.DegToRad;
                                TankOrb TankOrbe4 = new TankOrb();
                                TankOrbe4.Dir = 270 * SuperTank2.DegToRad;
                                TankOrbe4.Resist = ArrayWeaponSecondary[4].WeaponResist;
                                TankOrbe4.Creator = this;
                                ArrayTankOrb[3] = TankOrbe4;

                                if (ArrayWeaponSecondary[4].WeaponSpread >= 5)
                                {
                                    TankOrbe2.Dir = 72 * SuperTank2.DegToRad;
                                    TankOrbe3.Dir = 144 * SuperTank2.DegToRad;
                                    TankOrbe4.Dir = 216 * SuperTank2.DegToRad;
                                    TankOrb TankOrbe5 = new TankOrb();
                                    TankOrbe5.Dir = 288 * SuperTank2.DegToRad;
                                    TankOrbe5.Resist = ArrayWeaponSecondary[4].WeaponResist;
                                    TankOrbe5.Creator = this;
                                    ArrayTankOrb[4] = TankOrbe5;

                                    if (ArrayWeaponSecondary[4].WeaponSpread >= 6)
                                    {
                                        TankOrbe2.Dir = 60 * SuperTank2.DegToRad;
                                        TankOrbe3.Dir = 120 * SuperTank2.DegToRad;
                                        TankOrbe4.Dir = 180 * SuperTank2.DegToRad;
                                        TankOrbe5.Dir = 240 * SuperTank2.DegToRad;
                                        TankOrb TankOrbe6 = new TankOrb();
                                        TankOrbe6.Dir = 300 * SuperTank2.DegToRad;
                                        TankOrbe6.Resist = ArrayWeaponSecondary[4].WeaponResist;
                                        TankOrbe6.Creator = this;
                                        ArrayTankOrb[5] = TankOrbe6;

                                        if (ArrayWeaponSecondary[4].WeaponSpread >= 7)
                                        {
                                            TankOrbe2.Dir = 51 * SuperTank2.DegToRad;
                                            TankOrbe3.Dir = 103 * SuperTank2.DegToRad;
                                            TankOrbe4.Dir = 154 * SuperTank2.DegToRad;
                                            TankOrbe5.Dir = 206 * SuperTank2.DegToRad;
                                            TankOrbe6.Dir = 257 * SuperTank2.DegToRad;
                                            TankOrb TankOrbe7 = new TankOrb();
                                            TankOrbe7.Dir = 309 * SuperTank2.DegToRad;
                                            TankOrbe7.Resist = ArrayWeaponSecondary[4].WeaponResist;
                                            TankOrbe7.Creator = this;
                                            ArrayTankOrb[6] = TankOrbe7;

                                            if (ArrayWeaponSecondary[4].WeaponSpread >= 8)
                                            {
                                                TankOrbe2.Dir = 45 * SuperTank2.DegToRad;
                                                TankOrbe3.Dir = 90 * SuperTank2.DegToRad;
                                                TankOrbe4.Dir = 135 * SuperTank2.DegToRad;
                                                TankOrbe5.Dir = 180 * SuperTank2.DegToRad;
                                                TankOrbe6.Dir = 225 * SuperTank2.DegToRad;
                                                TankOrbe7.Dir = 270 * SuperTank2.DegToRad;
                                                TankOrb TankOrbe8 = new TankOrb();
                                                TankOrbe8.Dir = 315 * SuperTank2.DegToRad;
                                                TankOrbe8.Resist = ArrayWeaponSecondary[4].WeaponResist;
                                                TankOrbe8.Creator = this;
                                                ArrayTankOrb[7] = TankOrbe8;

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
                ArrayTankOrb = new TankOrb[0];

            #endregion

            #region Créer les tourelles

            if (ArrayWeaponSecondary[5].ArmeLevel > 0)
            {
                ArrayTankTourelle = new TankTurret[ArrayWeaponSecondary[5].WeaponSpread];
                if (ArrayWeaponSecondary[5].WeaponSpreadLvl >= 1)
                {
                    TankTurret NouvelleTankTourelle = new TankTurret(new Vector2(Position.X, Position.Y + 25));
                    NouvelleTankTourelle.Creator = this;
                    ArrayTankTourelle[0] = NouvelleTankTourelle;
                }
                if (ArrayWeaponSecondary[5].WeaponSpread >= 2)
                {
                    TankTurret NouvelleTankTourelle = new TankTurret(new Vector2(Position.X + 150, Position.Y + 25));
                    NouvelleTankTourelle.Creator = this;
                    ArrayTankTourelle[1] = NouvelleTankTourelle;
                }
                if (ArrayWeaponSecondary[5].WeaponSpread >= 3)
                {
                    TankTurret NouvelleTankTourelle = new TankTurret(new Vector2(Position.X - 150, Position.Y + 25));
                    NouvelleTankTourelle.Creator = this;
                    ArrayTankTourelle[2] = NouvelleTankTourelle;
                }
                if (ArrayWeaponSecondary[5].WeaponSpread >= 4)
                {
                    TankTurret NouvelleTankTourelle = new TankTurret(new Vector2(Position.X + 10, Position.Y + 25));
                    NouvelleTankTourelle.Creator = this;
                    ArrayTankTourelle[3] = NouvelleTankTourelle;
                }
                if (ArrayWeaponSecondary[5].WeaponSpread >= 5)
                {
                    TankTurret NouvelleTankTourelle = new TankTurret(new Vector2(Position.X - 100, Position.Y + 25));
                    NouvelleTankTourelle.Creator = this;
                    ArrayTankTourelle[4] = NouvelleTankTourelle;
                }
                if (ArrayWeaponSecondary[5].WeaponSpread >= 6)
                {
                    TankTurret NouvelleTankTourelle = new TankTurret(new Vector2(Position.X + 50, Position.Y + 25));
                    NouvelleTankTourelle.Creator = this;
                    ArrayTankTourelle[5] = NouvelleTankTourelle;
                }
                if (ArrayWeaponSecondary[5].WeaponSpread >= 7)
                {
                    TankTurret NouvelleTankTourelle = new TankTurret(new Vector2(Position.X - 50, Position.Y + 25));
                    NouvelleTankTourelle.Creator = this;
                    ArrayTankTourelle[6] = NouvelleTankTourelle;
                }
                if (ArrayWeaponSecondary[5].WeaponSpread >= 8)
                {
                    TankTurret NouvelleTankTourelle = new TankTurret(new Vector2(Position.X + 200, Position.Y + 25));
                    NouvelleTankTourelle.Creator = this;
                    ArrayTankTourelle[7] = NouvelleTankTourelle;
                }
                if (ArrayWeaponSecondary[5].WeaponSpread >= 9)
                {
                    TankTurret NouvelleTankTourelle = new TankTurret(new Vector2(Position.X - 200, Position.Y + 25));
                    NouvelleTankTourelle.Creator = this;
                    ArrayTankTourelle[8] = NouvelleTankTourelle;
                }
            }
            else
                ArrayTankTourelle = new TankTurret[0];

            #endregion
        }

        public override void Update(GameTime gameTime)
        {
            float TimeEllapsed = gameTime.ElapsedGameTime.Milliseconds * 0.0625f;
            base.Update(gameTime);

            UpdateAngle();
            TirerArme();

            if (!Vide)
                ShurikenAngle -= 5 * SuperTank2.DegToRad;
            if (KeyboardHelper.KeyHold(Microsoft.Xna.Framework.Input.Keys.Space) && ActiveSpecial == SpecialTypes.None)
            {
                if ((Special / 30) == 1)//lvl 1 propulsion
                {
                    ActiveSpecial = SpecialTypes.Jump;
                    Special = 30;
                    God = true;
                    Speed.Y = -60;
                }
                if ((Special / 30) == 2)//lvl 3 charge hyper puissante
                {
                    ActiveSpecial = SpecialTypes.Laser;
                    Special = 60;
                    SuperLaser NewSuperLaser = new SuperLaser(Position, MaskSuperLaser, sprSuperLaser);
                    ActiveSuperLaser = NewSuperLaser;
                }
                if ((Special / 30) == 3)//5 tir simultan? 10 sec
                {
                    Special = 90;
                    ActiveSpecial = SpecialTypes.FireEverything;
                }
                if ((Special / 30) == 4)//7 bullet time
                {
                    Special = 120;
                    ActiveSpecial = SpecialTypes.BulletTime;
                }
                if ((Special / 30) == 5)//8 armageddon
                {
                    Special = 150;
                    ActiveSpecial = SpecialTypes.Armageddon;
                    SpecialTimer3 = 5;
                }
            }
            if (ActiveSpecial == SpecialTypes.Armageddon)//Armageddon
            {
                --SpecialTimer3;
                if (SpecialTimer3 <= 0)
                {
                    Meteor NewMeteor = null;
                    if (SuperTank2.Randomizer.Next(2) == 0)
                        NewMeteor = new Meteor(new Vector2(SuperTank2.Randomizer.Next(Constants.Width), -36), MaskMeteor1, sprMeteor1, true);
                    else
                        NewMeteor = new Meteor(new Vector2(SuperTank2.Randomizer.Next(Constants.Width), -36), MaskMeteor2, sprMeteor2, false);

                    AddBullet(NewMeteor);
                    SpecialTimer3 = 5;
                }
            }

            UpdateTank();
            UpdateOrbs();
            UpdateTurrets();
            if (ActiveSuperLaser != null)
                UpdateSuperLaser();

            PositionOld = Position;
        }

        public void UpdateTank()
        {
            if (ActiveSpecial == SpecialTypes.BulletTime)
                SuperTank2.SlowDownSpecialActive = true;

            if (!Vide)
            {
                VehiculeEnergie += VehiculeRegen;
                if (SuperTank2.CheatTripleEnergy)
                    VehiculeEnergie += VehiculeRegen * 3;
                if (ActivePowerUpEnergieTime > 0)
                    VehiculeEnergie += VehiculeRegen * 20;
            }
            else
            {
                VehiculeEnergie += VehiculeRegen * 20;
                if (ActivePowerUpEnergieTime > 0)
                    VehiculeEnergie += VehiculeRegen * 40;
                if (VehiculeEnergie >= VehiculeEnergieMax)
                    Vide = false;
            }
            VehiculeEnergie = Math.Min(VehiculeEnergieMax, Math.Max(0, VehiculeEnergie));
            if (Math.Floor(VehiculeEnergie) == 0)
                Vide = true;
            //Gravité
            if (Position.Y + Speed.Y < Height)
            {
                Position.Y += Speed.Y;
                Speed.Y += 4;
            }
            else
            {
                Speed.Y = 0;
                Position.Y = Height;
            }
            if (Rage < 100 && VehiculeSpecial >= 4)
                Rage += 1;

            UpdateInputs();

            if (BackgroundSpeed.X < 0)
                AnimationIndex += (-BackgroundSpeed.X / 8) / 2;
            else
                AnimationIndex += 0.5f + (Position.X - PositionOld.X) / 10f;

            if (AnimationIndex >= 3)
                AnimationIndex -= 3;

            if (VehiculeResist <= 0 && !IsDestroyed)
            {
                IsDestroyed = true;
            }

            int CurrentMaxLevel = 9 + ArrayWeaponPrimary[New].ArmeLevel;

            if (Arme1ImgSelect < CurrentMaxLevel)
            {
                Arme1ImgSelect += 0.2f;
                if (Arme1ImgSelect >= CurrentMaxLevel)
                {
                    Tirer1 = 0;
                }
            }

            if (Recul > 0)
                Recul -= 1;
            else
                Recul = 0;
        }

        public void UpdateAngle()
        {
            if (Tirer1 > 0)
                Tirer1 -= 1;
            if (Tirer2 > 0)
                Tirer2 -= 1;
            if (!Vide)
                CanonAngle = GetAngle();
            else
                CanonAngle = -90 * SuperTank2.DegToRad;
        }

        public void UpdateInputs()
        {
            float VitesseBoost = 0;
            if (ActivePowerUpSpeedTime > 0)
                VitesseBoost = 0.5f * VehiculeVitesse;
            if (Position.X - VehiculeVitesse + VitesseBoost + LeftLim > 0)
            {
                if (VehiculeEnergie >= VehiculePerte && !Vide && InputHelper.InputLeftHold())
                {
                    Position.X -= VehiculeVitesse + VitesseBoost;
                    VehiculeEnergie -= VehiculePerte;
                }
            }
            else
                Position.X = -LeftLim + 1;
            if (Position.X + VehiculeVitesse + VitesseBoost + RightLim < Constants.Width)
            {
                if (VehiculeEnergie >= VehiculePerte && !Vide && InputHelper.InputRightHold())
                {
                    Position.X += VehiculeVitesse + VitesseBoost;
                    VehiculeEnergie -= VehiculePerte;
                }
            }
            else
                Position.X = Constants.Width - RightLim - 1;
            if (KeyboardHelper.KeyPressed(NextWeaponKey))
            {
                Tirer1 = -1;

                Old = ArmeSelect1;
                Weapon ActiveWeapon = ArrayWeaponPrimary[ArmeSelect1];

                if (ArmeSelect1 < ArrayWeaponPrimary.Length - 1)
                    ++ArmeSelect1;
                else
                    ArmeSelect1 = 0;

                New = ArmeSelect1;

                Arme1ImgSelect = 3 - ActiveWeapon.ArmeLevel;
            }
            if (KeyboardHelper.KeyPressed(PreviousWeaponKey))
            {
                Tirer1 = -1;

                Old = ArmeSelect1;
                Weapon ActiveWeapon = ArrayWeaponPrimary[ArmeSelect1];

                if (ArmeSelect1 > 0)
                    --ArmeSelect1;
                else
                    ArmeSelect1 = ArrayWeaponPrimary.Length - 1;

                New = ArmeSelect1;

                Arme1ImgSelect = 3 - ActiveWeapon.ArmeLevel;
            }
            if (KeyboardHelper.KeyHold(SpecialKey) && ActiveSpecial == SpecialTypes.None)
            {
                if (Special < 30 && VehiculeSpecial >= 1)
                    Special++;
                else if (Special < 60 && VehiculeSpecial >= 3)
                    Special++;
                else if (Special < 90 && VehiculeSpecial >= 5)
                    Special++;
                else if (Special < 120 && VehiculeSpecial >= 7)
                    Special++;
                else if (Special < 150 && VehiculeSpecial >= 8)
                    Special++;
            }
            if (ActiveSpecial != SpecialTypes.None || !KeyboardHelper.KeyHold(SpecialKey) && Special > 0)
            {
                Special -= 1;
            }
            if (Special == 0)
            {
                ActiveSuperLaser = null;
                ActiveSpecial = SpecialTypes.None;
            }
            //Changement d'arme
            for (int W = 0; W < ArrayWeaponSecondary.Length; ++W)
            {
                if (KeyboardHelper.KeyPressed(ArrayWeaponKey[W]) && ArrayWeaponSecondary[0].ArmeLevel > 0)
                {
                    ArmeSelect2 = W;
                    break;
                }
            }
        }

        public void UpdateSuperLaser()
        {
            ActiveSuperLaser.Scale.X = 2000;
            ActiveSuperLaser.UpdateTransformationMatrix(ActiveSuperLaser.Scale);

            ActiveSuperLaser.Angle = CanonAngle;
            float MinDistance = ActiveSuperLaser.Scale.X;
            int EnemyHitIndex = -1;

            for (int E = 0; E < Enemy.ListEnemy.Count; E++)
            {
                if (Enemy.ListEnemy[E].IsAlive)
                {
                    if (Enemy.ListEnemy[E].PixelIntersect(ActiveSuperLaser))
                    {
                        float Distance = Vector2.Distance(Position, Enemy.ListEnemy[E].Position);
                        if (Distance < MinDistance)
                        {
                            MinDistance = Distance;
                            EnemyHitIndex = E;
                        }
                    }
                }
            }
            if (EnemyHitIndex >= 0)
            {
                ActiveSuperLaser.Scale.X = MinDistance;
                Enemy.ListEnemy[EnemyHitIndex].Resist--;
                Enemy.ListEnemy[EnemyHitIndex].Destroyed(this);
            }
            else
            {
                MinDistance = ActiveSuperLaser.Scale.X;
                int EnemyAttackHitIndex = -1;

                for (int A = 0; A < EnemyAttack.ListEnemyAttack.Count; A++)
                {
                    if (EnemyAttack.ListEnemyAttack[A].Resist > 0)
                    {
                        if (EnemyAttack.ListEnemyAttack[A].PixelIntersect(ActiveSuperLaser))
                        {
                            float Distance = Vector2.Distance(Position, EnemyAttack.ListEnemyAttack[A].Position);
                            if (Distance < MinDistance)
                            {
                                MinDistance = Distance;
                                EnemyAttackHitIndex = A;
                            }
                        }
                    }
                }
                if (EnemyAttackHitIndex >= 0)
                {
                    ActiveSuperLaser.Scale.X = MinDistance;
                    EnemyAttack.ListEnemyAttack[EnemyAttackHitIndex].Resist--;
                    EnemyAttack.ListEnemyAttack[EnemyAttackHitIndex].Destroyed(this);
                }
            }
            ActiveSuperLaser.Position = Position;

            ActiveSuperLaser.UpdateTransformationMatrix(ActiveSuperLaser.Scale);
        }

        void UpdateOrbs()
        {
            for (int O = 0; O < ArrayTankOrb.Length; O++)
            {
                ArrayTankOrb[O].Position.X = Position.X + lengthdir_x(120, ArrayTankOrb[O].Dir);
                ArrayTankOrb[O].Position.Y = Position.Y + lengthdir_y(90, ArrayTankOrb[O].Dir);
                if (VehiculeEnergie >= ArrayWeaponSecondary[4].WeaponEnergy && !Vide)
                {
                    ArrayTankOrb[O].Dir += ArrayWeaponSecondary[4].WeaponSpeed * SuperTank2.DegToRad;
                    if (ArrayTankOrb[O].Regen)
                        ArrayTankOrb[O].Dir += ArrayWeaponSecondary[4].WeaponSpeed * 2 * SuperTank2.DegToRad;
                }
                if (ArrayTankOrb[O].Resist <= 0 && VehiculeEnergie >= ArrayWeaponSecondary[4].WeaponEnergy)
                {
                    VehiculeEnergie -= ArrayWeaponSecondary[4].WeaponEnergy;
                    if (ArrayWeaponSecondary[4].ActivePowerUpAll > 0)
                        VehiculeEnergie += ArrayWeaponSecondary[4].WeaponEnergy / 2;
                    foreach (TankOrb ActiveTankOrbe in ArrayTankOrb)
                    {
                        ActiveTankOrbe.Resist = ArrayWeaponSecondary[4].WeaponResist;
                        ActiveTankOrbe.Regen = true;
                        ActiveTankOrbe.Alarm0 = ArrayWeaponSecondary[4].WeaponRateOfFire;
                        if (ArrayWeaponSecondary[4].ActivePowerUpAll > 0)
                            ActiveTankOrbe.Alarm0 /= 1.5;
                    }
                }
            }
        }

        void UpdateTurrets()
        {
            for (int T = 0; T < ArrayTankTourelle.Length; T++)
            {
                ArrayTankTourelle[T].Update();
            }
        }

        public override bool ActivatePowerUp(PowerUp.PowerUpTypes PowerUpType)
        {
            if (base.ActivatePowerUp(PowerUpType))
                return true;

            int ActiveTime = 600;
            switch (PowerUpType)
            {
                case PowerUp.PowerUpTypes.Flak:
                    ArrayWeaponSecondary[0].ActivePowerUpAll = ActiveTime;
                    return true;
                case PowerUp.PowerUpTypes.Missile:
                    ArrayWeaponSecondary[1].ActivePowerUpAll = ActiveTime;
                    return true;
                case PowerUp.PowerUpTypes.GuidedMissile:
                    ArrayWeaponSecondary[2].ActivePowerUpAll = ActiveTime;
                    return true;
                case PowerUp.PowerUpTypes.Laser:
                    ArrayWeaponSecondary[3].ActivePowerUpAll = ActiveTime;
                    return true;
                case PowerUp.PowerUpTypes.Orb:
                    ArrayWeaponSecondary[4].ActivePowerUpAll = ActiveTime;
                    return true;
                case PowerUp.PowerUpTypes.Turret:
                    ArrayWeaponSecondary[5].ActivePowerUpAll = ActiveTime;
                    return true;
                case PowerUp.PowerUpTypes.Shuriken:
                    ArrayWeaponSecondary[6].ActivePowerUpAll = ActiveTime;
                    return true;
            }
            return false;
        }

        public override void DrawVehicule(CustomSpriteBatch g)
        {
            Vector2 TankCenter = Position;

            if (TankCenter.X > 548)
            {
                float ScaleX = -((TankCenter.X - 512) / 512);
                sprShadowBottom.Draw(g, (int)AnimationIndex, new Vector2(TankCenter.X - 55, TankCenter.Y + 1), new Vector2(ScaleX, 1), 0, Color.White);
                sprShadowTop.Draw(g, 0, new Vector2(TankCenter.X - 6, TankCenter.Y - 23), new Vector2(ScaleX, 1), 0, Color.White);
                sprShadowMisc1.Draw(g, 0, new Vector2(TankCenter.X - 54, TankCenter.Y - 5), new Vector2(ScaleX, 1), 0, Color.White);
                sprShadowMisc2.Draw(g, 0, new Vector2(TankCenter.X, TankCenter.Y), new Vector2(ScaleX, 1), 0, Color.White);
            }
            else if (TankCenter.X < 476)
            {
                float ScaleX = 1 - (TankCenter.X) / 512;
                sprShadowBottom.Draw(g, (int)AnimationIndex, new Vector2(TankCenter.X + 55, TankCenter.Y + 1), new Vector2(ScaleX, 1), 0, Color.White);
                sprShadowTop.Draw(g, 0, new Vector2(TankCenter.X + 6, TankCenter.Y - 23), new Vector2(ScaleX, 1), 0, Color.White);
                sprShadowMisc1.Draw(g, 0, new Vector2(TankCenter.X + 55, TankCenter.Y - 5), new Vector2(ScaleX, 1), 0, Color.White);
                sprShadowMisc2.Draw(g, 0, new Vector2(TankCenter.X, TankCenter.Y), new Vector2(ScaleX, 1), 0, Color.White);
            }

            if (Old == 0 && New == 0)
                sprCanonTank.Draw(g, 3 - ArrayWeaponPrimary[0].ArmeLevel, new Vector2(TankCenter.X - lengthdir_x(Recul, CanonAngle), TankCenter.Y - lengthdir_y(Recul, CanonAngle)), CanonAngle, Color.White);
            else if (Old == 0 && New == 1)
				sprCanon1_2.Draw(g, (int)Arme1ImgSelect, new Vector2(TankCenter.X - lengthdir_x(Recul, CanonAngle), TankCenter.Y - lengthdir_y(Recul, CanonAngle)), CanonAngle, Color.White);
            else if (Old == 0 && New == 2)
                sprCanon1_3.Draw(g, (int)Arme1ImgSelect, new Vector2(TankCenter.X - lengthdir_x(Recul, CanonAngle), TankCenter.Y - lengthdir_y(Recul, CanonAngle)), CanonAngle, Color.White);
            else if (Old == 1 && New == 0)
                sprCanon2_1.Draw(g, (int)Arme1ImgSelect, new Vector2(TankCenter.X - lengthdir_x(Recul, CanonAngle), TankCenter.Y - lengthdir_y(Recul, CanonAngle)), CanonAngle, Color.White);
            else if (Old == 1 && New == 2)
                sprCanon2_3.Draw(g, (int)Arme1ImgSelect, new Vector2(TankCenter.X - lengthdir_x(Recul, CanonAngle), TankCenter.Y - lengthdir_y(Recul, CanonAngle)), CanonAngle, Color.White);
            else if (Old == 2 && New == 0)
                sprCanon3_1.Draw(g, (int)Arme1ImgSelect, new Vector2(TankCenter.X - lengthdir_x(Recul, CanonAngle), TankCenter.Y - lengthdir_y(Recul, CanonAngle)), CanonAngle, Color.White);
            else if (Old == 2 && New == 1)
                sprCanon3_2.Draw(g, (int)Arme1ImgSelect, new Vector2(TankCenter.X - lengthdir_x(Recul, CanonAngle), TankCenter.Y - lengthdir_y(Recul, CanonAngle)), CanonAngle, Color.White);

            if (ArrayWeaponSecondary[0].ArmeLevel == 1)
				sprCanonflak1.Draw(g, 0, new Vector2(TankCenter.X - lengthdir_x(Recul, CanonAngle), TankCenter.Y - lengthdir_y(Recul, CanonAngle)), CanonAngle, Color.White);
            else if (ArrayWeaponSecondary[0].ArmeLevel == 2)
				sprCanonflak2.Draw(g, 0, new Vector2(TankCenter.X - lengthdir_x(Recul, CanonAngle), TankCenter.Y - lengthdir_y(Recul, CanonAngle)), CanonAngle, Color.White);
            else if (ArrayWeaponSecondary[0].ArmeLevel > 2)
				sprCanonflak3.Draw(g, 0, new Vector2(TankCenter.X - lengthdir_x(Recul, CanonAngle), TankCenter.Y - lengthdir_y(Recul, CanonAngle)), CanonAngle, Color.White);

            if (ArrayWeaponSecondary[1].ArmeLevel == 1)
                sprLanceMissile1.Draw(g, 0, new Vector2(TankCenter.X - lengthdir_x(Recul, CanonAngle), TankCenter.Y - lengthdir_y(Recul, CanonAngle)), CanonAngle, Color.White);
            else if (ArrayWeaponSecondary[1].ArmeLevel == 2)
				sprLanceMissile2.Draw(g, 0, new Vector2(TankCenter.X - lengthdir_x(Recul, CanonAngle), TankCenter.Y - lengthdir_y(Recul, CanonAngle)), CanonAngle, Color.White);
            else if (ArrayWeaponSecondary[1].ArmeLevel > 2)
				sprLanceMissile3.Draw(g, 0, new Vector2(TankCenter.X - lengthdir_x(Recul, CanonAngle), TankCenter.Y - lengthdir_y(Recul, CanonAngle)), CanonAngle, Color.White);

            if (ArrayWeaponSecondary[3].ArmeLevel == 1)
				sprCanonLaser1.Draw(g, 0, new Vector2(TankCenter.X - lengthdir_x(Recul, CanonAngle), TankCenter.Y - lengthdir_y(Recul, CanonAngle)), CanonAngle, Color.White);
            else if (ArrayWeaponSecondary[3].ArmeLevel == 2)
				sprCanonLaser2.Draw(g, 0, new Vector2(TankCenter.X - lengthdir_x(Recul, CanonAngle), TankCenter.Y - lengthdir_y(Recul, CanonAngle)), CanonAngle, Color.White);
            else if (ArrayWeaponSecondary[3].ArmeLevel > 2)
				sprCanonLaser3.Draw(g, 0, new Vector2(TankCenter.X - lengthdir_x(Recul, CanonAngle), TankCenter.Y - lengthdir_y(Recul, CanonAngle)), CanonAngle, Color.White);

            sprTank.Draw(g, 0, new Vector2(TankCenter.X, TankCenter.Y), 0, Color.White);
			sprWheels.Draw(g, (int)AnimationIndex, new Vector2(TankCenter.X - 64, TankCenter.Y + 24), 0, Color.White);

            if (ArrayWeaponSecondary[6].ArmeLevel == 1)
				sprLanceShuriken1.Draw(g, 0, new Vector2(TankCenter.X, TankCenter.Y), ShurikenAngle, Color.White);
            else if (ArrayWeaponSecondary[6].ArmeLevel == 2)
                sprLanceShuriken2.Draw(g, 0, new Vector2(TankCenter.X, TankCenter.Y), ShurikenAngle, Color.White);
            else if (ArrayWeaponSecondary[6].ArmeLevel == 3)
                sprLanceShuriken3.Draw(g, 0, new Vector2(TankCenter.X, TankCenter.Y), ShurikenAngle, Color.White);

            if (ArrayWeaponSecondary[2].ArmeLevel == 1)
				sprLanceMissileG1.Draw(g, 0, new Vector2(TankCenter.X, TankCenter.Y), 0, Color.White);
            else if (ArrayWeaponSecondary[2].ArmeLevel == 2)
				sprLanceMissileG2.Draw(g, 0, new Vector2(TankCenter.X, TankCenter.Y), 0, Color.White);
            else if (ArrayWeaponSecondary[2].ArmeLevel > 2)
				sprLanceMissileG3.Draw(g, 0, new Vector2(TankCenter.X, TankCenter.Y), 0, Color.White);

            if (ActiveSuperLaser != null)
                ActiveSuperLaser.Draw(g, 0, new Vector2(TankCenter.X - lengthdir_x(Recul, CanonAngle), TankCenter.Y - lengthdir_y(Recul, CanonAngle)), CanonAngle, Color.White);

            if (Protec > 0)
                sprTankShield.Draw(g, 0, new Vector2(TankCenter.X, TankCenter.Y), 0, Color.FromNonPremultiplied(255, 255, 255, (byte)(Protec / 10 + 0.2)));

            for (int i = 0; i < 7; i += 1)
            {
                if (ArmeSelect2 == i || (i == 4 && OrbeActif))
                    sprWeaponIcon.Draw(g, i, new Vector2(TankCenter.X - 112 + i * 32, 736), 0, Color.FromNonPremultiplied(255, 255, 255, 200));
                else
                    sprWeaponIcon.Draw(g, i, new Vector2(TankCenter.X - 112 + i * 32, 736), 0, Color.FromNonPremultiplied(255, 255, 255, 127));
            }

            if (Special > 0)
            {
                float PosX = TankCenter.X - 90;
                float PosY = TankCenter.Y + 22;
                if (VehiculeSpecial >= 1)
                {
                    g.Draw(sprSpecialIcon, new Vector2(PosX + 15, PosY), Color.White);
                    g.DrawString(fntArial26, "1", new Vector2(PosX + 21, PosY - 4), Color.White);
                }
                if (VehiculeSpecial >= 2)
                {
                    g.Draw(sprSpecialIcon, new Vector2(PosX + 45, PosY), Color.White);
                    g.DrawString(fntArial26, "2", new Vector2(PosX + 51, PosY - 4), Color.White);
                }
                if (VehiculeSpecial >= 3)
                {
                    g.Draw(sprSpecialIcon, new Vector2(PosX + 75, PosY), Color.White);
                    g.DrawString(fntArial26, "3", new Vector2(PosX + 81, PosY - 4), Color.White);
                }
                if (VehiculeSpecial >= 4)
                {
                    g.Draw(sprSpecialIcon, new Vector2(PosX + 105, PosY), Color.White);
                    g.DrawString(fntArial26, "4", new Vector2(PosX + 111, PosY - 4), Color.White);
                }
                if (VehiculeSpecial >= 5)
                {
                    g.Draw(sprSpecialIcon, new Vector2(PosX + 135, PosY), Color.White);
                    g.DrawString(fntArial26, "5", new Vector2(PosX + 141, PosY - 4), Color.White);
                }
                int ChargeLevel = (int)Math.Floor(Special / 30f);
                if (Special < VehiculeSpecial * 30)
                    g.Draw(sprPixel, new Rectangle((int)PosX + 15 + 30 * ChargeLevel, (int)PosY, Special - 30 * ChargeLevel, sprSpecialIcon.Height), Color.FromNonPremultiplied(255, 255, 255, 127));
                if (Special >= 30)
                    g.Draw(sprPixel, new Rectangle((int)PosX + 15, (int)PosY, 30 * ChargeLevel, sprSpecialIcon.Height), Color.FromNonPremultiplied(255, 0, 0, 127));
            }

            for (int T = 0; T < ArrayTankTourelle.Length; T++)
            {
                TankTurret.ImageTourelle.Draw(g, (int)Math.Floor(ArrayTankTourelle[T].AnimationSpritePosition), ArrayTankTourelle[T].Position, 0, Color.White);
            }

            for (int O = 0; O < ArrayTankOrb.Length; O++)
            {
                g.Draw(sprOrb, ArrayTankOrb[O].Position, Color.White);
            }
        }

        public float lengthdir_x(double Length, double Dir)
        {
            return (float)(Math.Cos(Dir) * Length);
        }

        public float lengthdir_y(double Length, double Dir)
        {
            return (float)(Math.Sin(Dir) * Length);
        }

        public void TirerArme()
        {
            if (!Vide)
            {
                if (Tirer1 == 0)
                {
                    bool CanShot = InputHelper.InputConfirmHold();
                    if (CanShot)
                    {
                        Weapon ActiveWeapon1 = ArrayWeaponPrimary[ArmeSelect1];

                        if (ActiveWeapon1.ActivePowerUpRateOfFire > 0)
                        {
                            Tirer1 = ActiveWeapon1.WeaponRateOfFire * 0.5f;
                            if (!SuperTank2.CheatUnlimitedEnergy)
                                VehiculeEnergie -= ActiveWeapon1.WeaponEnergy * 0.5f;
                        }
                        else
                        {
                            Tirer1 = ActiveWeapon1.WeaponRateOfFire;
                            if (!SuperTank2.CheatUnlimitedEnergy)
                                VehiculeEnergie -= ActiveWeapon1.WeaponEnergy;
                        }
                        Recul = Math.Round(Tirer1 / 2);

                        switch (ArmeSelect1)
                        {//Canon 1
                            case 0:
                                if (ActiveWeapon1.WeaponSpread == 1 || ActiveWeapon1.WeaponSpread == 3 || ActiveWeapon1.WeaponSpread == 5)
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 0, CanonAngle);
                                if (ActiveWeapon1.WeaponSpread == 2 || ActiveWeapon1.WeaponSpread == 5)
                                {
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 0, CanonAngle - 2 * SuperTank2.DegToRad);
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 0, CanonAngle + 2 * SuperTank2.DegToRad);
                                }
                                if (ActiveWeapon1.WeaponSpread == 3 || ActiveWeapon1.WeaponSpread == 5)
                                {
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 0, CanonAngle - 4 * SuperTank2.DegToRad);
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 0, CanonAngle + 4 * SuperTank2.DegToRad);
                                }
                                if (ActiveWeapon1.WeaponSpread == 4)
                                {
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 0, CanonAngle - 1 * SuperTank2.DegToRad);
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 0, CanonAngle + 1 * SuperTank2.DegToRad);
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 0, CanonAngle - 3 * SuperTank2.DegToRad);
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 0, CanonAngle + 3 * SuperTank2.DegToRad);
                                }
                                break;
                            //Canon 2
                            case 1:
                                if (ActiveWeapon1.WeaponSpread == 1)
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 1, CanonAngle);
                                else if (ActiveWeapon1.WeaponSpread == 2)
                                {
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 1, CanonAngle - 2 * SuperTank2.DegToRad);
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 1, CanonAngle + 2 * SuperTank2.DegToRad);
                                }
                                else if (ActiveWeapon1.WeaponSpread > 2)
                                {
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 1, CanonAngle);
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 1, CanonAngle - 3 * SuperTank2.DegToRad);
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 1, CanonAngle + 3 * SuperTank2.DegToRad);
                                }
                                break;
                            //Canon 3
                            case 2:
                                if (ActiveWeapon1.WeaponSpread != 2)
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 2, CanonAngle);
                                if (ActiveWeapon1.WeaponSpread >= 2)
                                {
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 2, CanonAngle - 1 * SuperTank2.DegToRad);
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 2, CanonAngle + 1 * SuperTank2.DegToRad);
                                }
                                break;
                        }
                    }
                }
                if (Tirer2 <= 0 && ArmeSelect2 >= 0)
                {
                    bool CanShot = InputHelper.InputCancelHold();
                    if (CanShot)
                    {
                        Weapon ActiveWeapon2 = ArrayWeaponSecondary[ArmeSelect2];
                        Tirer2 = ActiveWeapon2.WeaponRateOfFire;
                        if (ActiveWeapon2.ActivePowerUpRateOfFire > 0)
                        {
                            Tirer2 *= 0.5f;
                            VehiculeEnergie += ActiveWeapon2.WeaponEnergy / 2;
                        }
                        if (!SuperTank2.CheatUnlimitedEnergy)
                            VehiculeEnergie -= ActiveWeapon2.WeaponEnergy;

                        switch (ArmeSelect2)
                        {//Canon flak
                            case 0:
                                for (int i = 0; i < ActiveWeapon2.WeaponSpread; i++)
                                {
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 3, CanonAngle + (15 - SuperTank2.Randomizer.Next(30)) * SuperTank2.DegToRad);
                                }
                                break;
                            //Lance missile
                            case 1:
                                if (ActiveWeapon2.WeaponSpread == 1 || ActiveWeapon2.WeaponSpread == 3 || ActiveWeapon2.WeaponSpread == 5 || ActiveWeapon2.WeaponSpread == 7 || ActiveWeapon2.WeaponSpread == 9)
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 4, CanonAngle);
                                if (ActiveWeapon2.WeaponSpread == 2 || ActiveWeapon2.WeaponSpread == 3 || ActiveWeapon2.WeaponSpread == 8 || ActiveWeapon2.WeaponSpread == 9)
                                {
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 4, CanonAngle + (float)SuperTank2.Randomizer.NextDouble() * 5 * SuperTank2.DegToRad);
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 4, CanonAngle - (float)SuperTank2.Randomizer.NextDouble() * 5 * SuperTank2.DegToRad);
                                }
                                if (ActiveWeapon2.WeaponSpread >= 4 && ActiveWeapon2.WeaponSpread <= 9)
                                {
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle) + lengthdir_x(20, CanonAngle + 90), Position.Y + lengthdir_y(31, CanonAngle) + lengthdir_y(20, CanonAngle + 90)), 4, CanonAngle + (float)SuperTank2.Randomizer.NextDouble() * 5 * SuperTank2.DegToRad);
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle) + lengthdir_x(20, CanonAngle + 90), Position.Y + lengthdir_y(31, CanonAngle) + lengthdir_y(20, CanonAngle + 90)), 4, CanonAngle - (float)SuperTank2.Randomizer.NextDouble() * 5 * SuperTank2.DegToRad);
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle) + lengthdir_x(-20, CanonAngle + 90), Position.Y + lengthdir_y(31, CanonAngle) + lengthdir_y(-20, CanonAngle + 90)), 4, CanonAngle + (float)SuperTank2.Randomizer.NextDouble() * 5 * SuperTank2.DegToRad);
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle) + lengthdir_x(-20, CanonAngle + 90), Position.Y + lengthdir_y(31, CanonAngle) + lengthdir_y(-20, CanonAngle + 90)), 4, CanonAngle - (float)SuperTank2.Randomizer.NextDouble() * 5 * SuperTank2.DegToRad);
                                }
                                if (ActiveWeapon2.WeaponSpread >= 6 && ActiveWeapon2.WeaponSpread <= 9)
                                {
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle) + lengthdir_x(20, CanonAngle + 90), Position.Y + lengthdir_y(31, CanonAngle) + lengthdir_y(20, CanonAngle + 90)), 4, CanonAngle - (float)SuperTank2.Randomizer.NextDouble() * 5 * SuperTank2.DegToRad);
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle) + lengthdir_x(-20, CanonAngle + 90), Position.Y + lengthdir_y(31, CanonAngle) + lengthdir_y(-20, CanonAngle + 90)), 4, CanonAngle + (float)SuperTank2.Randomizer.NextDouble() * 5 * SuperTank2.DegToRad);
                                }
                                break;
                            //Lance missile guidé
                            case 2:
                                for (int i = 0; i < ActiveWeapon2.WeaponSpread; i++)
                                {
                                    Tirer(new Vector2(Position.X, Position.Y), 5, -(75 + SuperTank2.Randomizer.Next(30)) * SuperTank2.DegToRad);
                                }
                                break;
                            //Canon laser
                            case 3:
                                if (ActiveWeapon2.WeaponSpread == 1)
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 6, CanonAngle);
                                if (ActiveWeapon2.WeaponSpread == 2)
                                {
                                    Tirer(new Vector2(Position.X + lengthdir_x(21, CanonAngle) + lengthdir_x(10, CanonAngle + 90), Position.Y + lengthdir_y(21, CanonAngle) + lengthdir_y(10, CanonAngle + 90)), 6, CanonAngle);
                                    Tirer(new Vector2(Position.X + lengthdir_x(21, CanonAngle) + lengthdir_x(-10, CanonAngle + 90), Position.Y + lengthdir_y(21, CanonAngle) + lengthdir_y(-10, CanonAngle + 90)), 6, CanonAngle);
                                }
                                if (ActiveWeapon2.WeaponSpread == 3)
                                {
                                    Tirer(new Vector2(Position.X + lengthdir_x(31, CanonAngle), Position.Y + lengthdir_y(31, CanonAngle)), 6, CanonAngle);
                                    Tirer(new Vector2(Position.X + lengthdir_x(21, CanonAngle) + lengthdir_x(10, CanonAngle + 90), Position.Y + lengthdir_y(21, CanonAngle) + lengthdir_y(10, CanonAngle + 90)), 6, CanonAngle);
                                    Tirer(new Vector2(Position.X + lengthdir_x(21, CanonAngle) + lengthdir_x(-10, CanonAngle + 90), Position.Y + lengthdir_y(21, CanonAngle) + lengthdir_y(-10, CanonAngle + 90)), 6, CanonAngle);
                                }
                                break;
                            //Tourelle
                            case 5:
                                if (ActiveWeapon2.WeaponSpread > 0)
                                {
                                    for (int i = 0; i < ArrayTankTourelle.Length; i += 1)
                                    {
                                        TankTurret Obj = ArrayTankTourelle[i];
                                        Tirer(new Vector2(Obj.Position.X, Obj.Position.Y), 8, -(85 + SuperTank2.Randomizer.Next(10)) * SuperTank2.DegToRad);
                                    }
                                }
                                break;
                            //Lance shurkien
                            case 6:
                                for (int i = 0; i < ActiveWeapon2.WeaponSpread; i++)
                                {
                                    Tirer(new Vector2(Position.X, Position.Y), 9, CanonAngle + (15 - SuperTank2.Randomizer.Next(30)) * SuperTank2.DegToRad);
                                }
                                break;
                        }
                    }
                }
            }
        }

        public void Tirer(Vector2 Position, int WeaponIndex, float AngleBalle)
        {
            Weapon ActiveWeapon = null;
            if (WeaponIndex < 3)
                ActiveWeapon = ArrayWeaponPrimary[WeaponIndex];
            else
                ActiveWeapon = ArrayWeaponSecondary[WeaponIndex - 3];

            Bullet NewBullet = null;
            BulletCase NewBulletCase = null;
            switch (WeaponIndex)
            {
                case 0:
                    NewBullet = new Bullet(Position, MaskBulletNormal[ActiveWeapon.WeaponDamageLvl - 1], ActiveWeapon.WeaponDamageLvl - 1, sprBulletNormal);
                    NewBulletCase = new BulletCase(Position, ActiveWeapon.WeaponDamageLvl - 1, sprBulletCaseNormal);
                    break;
                case 1:
                    NewBullet = new Bullet(Position, MaskBulletHeavy[ActiveWeapon.WeaponDamageLvl - 1], ActiveWeapon.WeaponDamageLvl - 1, sprBulletHeavy);
                    NewBulletCase = new BulletCase(Position, ActiveWeapon.WeaponDamageLvl - 1, sprBulletCaseHeavy);
                    break;
                case 2:
                    NewBullet = new Bullet(Position, MaskBulletLaser[ActiveWeapon.WeaponDamageLvl - 1], ActiveWeapon.WeaponDamageLvl - 1, sprBulletLaser);
                    NewBulletCase = new BulletCase(Position, ActiveWeapon.WeaponDamageLvl - 1, sprLaserParticule);
                    break;
                case 3://Canon flak
                    NewBullet = new FlakBullet(Position, MaskFlak, sprBalleFlak);
                    NewBulletCase = new BulletCase(Position, 0, sprBulletCaseFlak);
                    break;
                case 4://Lance missile
                    NewBullet = new TankMissile(Position, MaskMissile, sprMissile);
                    NewBulletCase = new BulletCase(Position, 0, sprSmoke);
                    break;
                case 5://Lance missile guidé
                    NewBullet = new TankGuidedMissile(Position, ActiveWeapon.WeaponSpeed, MaskMissileG, sprMissileG);
                    NewBulletCase = new BulletCase(Position, 0, sprSmoke);
                    break;
                case 6://Laser
                    NewBullet = new Laser(Position, MaskLaser, sprLaser);
                    NewBulletCase = new BulletCase(Position,0, sprSmoke);
                    break;
                case 8://Tourelle de défense
                    NewBullet = new Bullet(Position, MaskBulletNormal[ActiveWeapon.WeaponDamageLvl - 1], ActiveWeapon.WeaponDamageLvl - 1, sprBulletNormal);
                    NewBulletCase = new BulletCase(Position, ActiveWeapon.WeaponDamageLvl - 1, sprBulletCaseNormal);
                    break;
                case 9://Lance shuriken
                    NewBullet = new Shuriken(Position, MaskShuriken[ActiveWeapon.ArmeLevel - 1], sprShuriken);
                    NewBulletCase = new BulletCase(Position, 0, sprSmoke);
                    break;
            }
            NewBullet.Angle = AngleBalle;
            NewBullet.SetSpeed(NewBullet.Angle, ActiveWeapon.WeaponSpeed);
            NewBullet.Resist = ActiveWeapon.WeaponResist;
            NewBullet.Damage = ActiveWeapon.WeaponDamage;
            NewBulletCase.Angle = (45 + SuperTank2.Randomizer.Next(90) * SuperTank2.DegToRad);
            NewBulletCase.SetSpeed(NewBulletCase.Angle, SuperTank2.Randomizer.Next(3));
            if (WeaponIndex <= 2)//Arme de base. 0 = Arme1, 1 = Arme 2, 2 = Arme 3.
            {
                if (ActiveWeapon.ActivePowerUpDamage > 0)
                {
                    NewBullet.Damage *= 1.5f;
                    NewBullet.Resist *= 1.5f;
                }
                else if (ActiveWeapon.ActivePowerUpSpeed > 0)
                    NewBullet.Speed *= 2;
            }
            else if (ActiveWeapon.ActivePowerUpAll > 0)
            {
                NewBullet.Damage *= 1.5f;
                NewBullet.Resist *= 1.5f;
                NewBullet.Speed *= 2;
            }
            if (SuperTank2.CheatDoubleDamage)
                NewBullet.Damage *= 2;
            if (SuperTank2.CheatDoubleResist)
                NewBullet.Resist *= 2;

            AddBullet(NewBullet);
            AddBulletCase(NewBulletCase);
        }
    }
}