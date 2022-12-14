using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    public class Shop : GameScreen
    {
        public struct VehiculeShopInfo
        {
            public bool IsOpen;
            public int ItemSelected;
            public int VehiPos;
            public int[] ArmePos;
            public ShopItemInfo VehiculePriceTable;
            public ShopItemInfo[] WeaponPriceTable;
        }

        public struct ShopItemInfo
        {
            public string ItemText;
            public string ItemDescription;
            public int BasePrice;
            public int RateOfFirePrice;
            public int SpreadPrice;
            public int EnergyPrice;
            public int DamagePrice;
            public int ResistPrice;
            public int SpeedPrice;
            public int SpecialPrice;

            public int PriceMultiplier;
        }

        private Vehicule ActiveVehicule;
        private int Actif;
        private int SelectPos;
        private float pas;
        private VehiculeShopInfo ActiveSelection;
        private VehiculeShopInfo[] VehiSelect;
        private Point MousePos;
        private bool MouseLeftPressed;
        private bool MouseRightPressed;
        private ShopItemInfo ActiveShopItemInfo;

        #region Ressources

        private SpriteFont fntArial13;
        private SpriteFont fntArial15;

        private FMODSound BackgroundMusic;
        private Texture2D Cursor;
        private Texture2D Background;
        private Texture2D Shop_monter;
        private Texture2D Shop_descendre;
        private Texture2D SprShopAmeliorer;
        private Texture2D SprShopAmeliorerP;
        private Texture2D sprBoutonChoice;
        private Texture2D sprBoutonChoiceP;
        private Texture2D[] VehiIco;
        private Texture2D[] VehiIco_p;
        private Texture2D[,] sprWeaponIcons;
        private Texture2D[,] sprWeaponIconsP;

        #endregion

        public Shop()
            : base()
        {
            SuperTank2.IsPlaying = false;
            Actif = 2;
            SelectPos = 0;
            pas = 0;
            VehiSelect = new VehiculeShopInfo[2];
            VehiSelect[0].IsOpen = false;
            VehiSelect[1].IsOpen = false;
            VehiSelect[0].ArmePos = new int[10];
            VehiSelect[1].ArmePos = new int[5];
            VehiSelect[0].VehiPos = 0;
            VehiSelect[1].VehiPos = 11;
            for (int j = 0; j < 2; j += 1)
            {
                for (int i = 0; i < VehiSelect[j].ArmePos.Length; i += 1)
                    VehiSelect[j].ArmePos[i] = i + VehiSelect[j].VehiPos + 1;//arme, véhicule
            }
            VehiSelect[1].VehiPos = 1;
        }

        public void TypewritterText(CustomSpriteBatch g, int X, int Y, int Speed)
        {
            if (pas < ActiveShopItemInfo.ItemDescription.Length)
            {
                pas += Speed;
                if (pas >= ActiveShopItemInfo.ItemDescription.Length)
                    pas = ActiveShopItemInfo.ItemDescription.Length - 1;
            }
            string FinalText = ActiveShopItemInfo.ItemDescription.Substring(0, (int)pas);
            g.DrawString(fntArial15, FinalText, new Vector2(X, Y), Color.Silver);
        }

        public override void Load()
        {
            ActiveVehicule = Vehicule.ArrayVehicule[0];
            LoadShop();
            BackgroundMusic = new FMODSound(FMODSystem, "Content/Maps/BGM/BGM_Shop.mp3");
            BackgroundMusic.SetLoop(true);
            BackgroundMusic.PlayAsBGM();

            fntArial13 = Content.Load<SpriteFont>("Fonts/Arial13");
            fntArial15 = Content.Load<SpriteFont>("Fonts/Arial15");

            SprShopAmeliorer = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Shop/ShopAmeliorer");
            SprShopAmeliorerP = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Shop/ShopAmeliorerP");
            Shop_monter = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Shop/Shop_monter");
            Shop_descendre = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Shop/Shop_descendre");
            Cursor = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Misc/Crosshair 2");

            Background = Content.Load<Texture2D>("Animations/Backgrounds 3D/Shop");
            VehiIco = new Texture2D[2];
            VehiIco_p = new Texture2D[2];
            VehiIco[0] = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Shop/Tank");
            VehiIco_p[0] = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Shop/Tank_p");
            VehiIco[1] = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Shop/Pickup");
            VehiIco_p[1] = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Shop/Pickup_p");
            sprBoutonChoice = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Shop/Shop_choix");
            sprBoutonChoiceP = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Shop/Shop_choix_p");

            sprWeaponIcons = new Texture2D[10, 2];
            sprWeaponIconsP = new Texture2D[10, 2];
            for (int j = 0; j < 2; j += 1)
            {
                for (int i = 0; i < 10; i += 1)
                {
                    sprWeaponIcons[i, j] = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Shop/TankCanon" + i);
                    sprWeaponIconsP[i, j] = Content.Load<Texture2D>("Animations/Bitmap Animations/Super Tank 2/Shop/TankCanon_p" + i);
                }
            }
        }

        public void LoadShop()
        {
            VehiSelect[0].WeaponPriceTable = new ShopItemInfo[10];
            for (int i = 0; i < 3; i++)
            {
                ShopItemInfo NewPriceTable = new ShopItemInfo();

                NewPriceTable.BasePrice = 500;
                NewPriceTable.PriceMultiplier = 500;
                NewPriceTable.RateOfFirePrice = 250;
                NewPriceTable.SpreadPrice = 325;
                NewPriceTable.EnergyPrice = 200;
                NewPriceTable.DamagePrice = 275;
                NewPriceTable.ResistPrice = 300;
                NewPriceTable.SpeedPrice = 225;
                NewPriceTable.SpecialPrice = 0;

                VehiSelect[0].WeaponPriceTable[i] = NewPriceTable;
            }
            for (int i = 0; i < 7; i++)
            {
                ShopItemInfo NewPriceTable = new ShopItemInfo();

                NewPriceTable.BasePrice = 500;
                NewPriceTable.PriceMultiplier = 650;
                NewPriceTable.RateOfFirePrice = 250;
                NewPriceTable.SpreadPrice = 325;
                NewPriceTable.EnergyPrice = 200;
                NewPriceTable.DamagePrice = 275;
                NewPriceTable.ResistPrice = 300;
                NewPriceTable.SpeedPrice = 225;
                NewPriceTable.SpecialPrice = 0;

                VehiSelect[0].WeaponPriceTable[3 + i] = NewPriceTable;
            }

            VehiSelect[1].WeaponPriceTable = new ShopItemInfo[1];

            VehiSelect[0].VehiculePriceTable.ItemDescription = "Le tank, véhicule surpuissant américain créé de toute pièce en chine.\r\nOutre sa formidable capacité à s'équiper d'armes de plus en plus\r\ndévastatrices, il est aussi très polyvalant et peux se promener sur\r\ntout les type de terrains. Ce goinfre de technologie moderne peut\r\névoluer en tout temps pour mieux écraser les ennemies.";
            VehiSelect[0].VehiculePriceTable.ItemText = "Tank";
            VehiSelect[0].WeaponPriceTable[0].ItemDescription = "Le canon 1 est l'arme de base, alliant puissance et rapidité de tir.\r\nCe canon tir des obus sans laisser de chance a l'ennemie de riposter,\r\ntout en vous offrant une bonne couverture.";
            VehiSelect[0].WeaponPriceTable[0].ItemText = "Canon 1";
            VehiSelect[0].WeaponPriceTable[1].ItemDescription = "Le canon 2 est une arme de très grande puissance, capable de\r\ndétruire complètement un adversaire d'un coup. Malheureusement,\r\ntant de puissance diminue la rapidité de tir et augmente son cout\r\nen énergie.";
            VehiSelect[0].WeaponPriceTable[1].ItemText = "Canon 2";
            VehiSelect[0].WeaponPriceTable[2].ItemDescription = "Le canon 3 est très rapide que ce sois pour tirer ou pour atteindre\r\nun ennemie. Cette arme envoie des décharges d'énergie concentré,\r\nce qui diminue son coup en énergie. Malheureusement si peu d'énergie,\r\ndiminue considérablement sa puissance de feu.";
            VehiSelect[0].WeaponPriceTable[2].ItemText = "Canon 3";
            VehiSelect[0].WeaponPriceTable[3].ItemDescription = "Le canon flak lance des morceaux de fer en fusion instable, ce qui\r\nles rend très dangereux. La fusion permet d'augmenter les dégâts\r\nrapidement et a faible couts. L'amélioration spéciale permet de rompre\r\nle noyau en plusieurs débris coupant pour finir le travail.";
            VehiSelect[0].WeaponPriceTable[3].ItemText = "flak";
            VehiSelect[0].WeaponPriceTable[4].ItemDescription = "Le lance missile est une arme de puissance redoutable grâce a sa\r\ncoque en titane. Cette coque lui permet de transpercer plusieurs\r\nennemies, ce qui lui donne une bonne ambivalence.";
            VehiSelect[0].WeaponPriceTable[4].ItemText = "Lance missile";
            VehiSelect[0].WeaponPriceTable[5].ItemDescription = "Les missiles guidés sont d'une précision mortelle, ce qu'ils perdent\r\nen puissance ils le regagnent grâce à un système de guidage thermique.\r\nCe système empèche tout système de visé au lancement.";
            VehiSelect[0].WeaponPriceTable[5].ItemText = "Missiles guidé";
            VehiSelect[0].WeaponPriceTable[6].ItemDescription = "Le canon laser est une arme de haute technologie utilisant des rayons\r\nd'énergie pure. Ce laser est doté d'une puissance de feu inoué et\r\nne s'arrète devant presque rien. Cette puissance demande donc une\r\nquantité considérable d'énergie pour être utilisé ainsi qu'un lourd\r\ntemps de rechargement";
            VehiSelect[0].WeaponPriceTable[6].ItemText = "Canon laser";
            VehiSelect[0].WeaponPriceTable[7].ItemDescription = "L'orbe défensif est un concentré d'énergie pure, ce qui lui offre une\r\nrésistance énorme ainsi que la capacité de gravité autour d'un champ\r\nénergétique assez puissant pour le contenir. Cette énergie est utilisée\r\npour absorber tout impact avec un objet plus petit que lui-même avant\r\nde se vider.";
            VehiSelect[0].WeaponPriceTable[7].ItemText = "Orbe defensive";
            VehiSelect[0].WeaponPriceTable[8].ItemDescription = "La tourelle défensive est une reproduction de votre tank à plus petit\r\néchelle. Elle s'améliore avec le même système de tir que le tank et\r\npossède donc trois type de tir. Il s'agit d'un mini vous en quelque sorte.";
            VehiSelect[0].WeaponPriceTable[8].ItemText = "Tourelle  défensive";
            VehiSelect[0].WeaponPriceTable[9].ItemDescription = "Le lance shuriken est une arme antique de haut rang qui découpe vos\r\nennemies comme du babeurre. En plus D'un pouvoir hautement\r\ndestructeur et d'une précision inégalé, Son am?lioration spécial\r\nl'enrobe de bande élastique et lui permette de rebondir.";
            VehiSelect[0].WeaponPriceTable[9].ItemText = "Lance shuriken";

            VehiSelect[1].VehiculePriceTable.ItemDescription = "Le pickup, formidable véhicule d'endurance et de puissance. Élevé dans\r\nles montagnes par de grands maitres de la mécanique, cette version\r\npossède une robustesse inégalée tout en possèdant une vaste panoplie\r\nd'armes destructrices. Malheureusement tant de poids et de puissance\r\naugmente sa consommation d'énergie et doit se rechargé souvent.";
            VehiSelect[1].VehiculePriceTable.ItemText = "Pickup";
            VehiSelect[1].WeaponPriceTable[0].ItemDescription = "Le canon 1 du pickup est littéralement un canon pris sur un bateau\r\nantique nommé La Perle Noire. Modifié par des ingénieurs militaires\r\nretraités de la guerre du Vietnam, cette arme est maintenant plus\r\nrapide et destructrice qu'avant, tout en disposant de porte verre\r\ningénieusement situé dans la chambre de munition pour garder les\r\nliquides au chaud.";
            VehiSelect[1].WeaponPriceTable[0].ItemText = "Canon 1";
        }

        public override void Update(GameTime gameTime)
        {
            MousePos.X = MouseHelper.MouseStateCurrent.X;
            MousePos.Y = MouseHelper.MouseStateCurrent.Y;

            MouseLeftPressed = MouseHelper.InputLeftButtonPressed();
            MouseRightPressed = MouseHelper.InputRightButtonPressed();

            if (MousePos.X > 720 && MousePos.Y > 28 && MousePos.Y < 46 && MouseLeftPressed)
            {
                if (SelectPos > 0)
                    SelectPos -= 1;
            }
            if (MousePos.X > 720 && MousePos.Y > 713 && MousePos.Y < 731 && MouseLeftPressed)
            {
                if (SelectPos < Actif - 14)
                    SelectPos += 1;
            }
            for (int j = 0; j < 2; j += 1)
            {
                if (MousePos.X > 720 && MousePos.Y > (VehiSelect[j].VehiPos - SelectPos + 1) * 48 && MousePos.Y < (VehiSelect[j].VehiPos - SelectPos + 1) * 48 + 38)
                {
                    if (MouseLeftPressed)
                    {
                        if (!VehiSelect[j].IsOpen)
                        {
                            VehiSelect[j].IsOpen = true;
                            ActiveShopItemInfo = VehiSelect[j].VehiculePriceTable;
                            Actif += VehiSelect[j].ArmePos.Length;
                            for (int k = j + 1; k < 2; k += 1)
                                VehiSelect[k].VehiPos = VehiSelect[j].VehiPos + VehiSelect[j].ArmePos.Length + k;
                        }
                        if (VehiSelect[j].ItemSelected == VehiSelect[j].VehiPos)
                        {
                            pas = ActiveShopItemInfo.ItemDescription.Length;
                        }
                        else
                        {
                            ActiveShopItemInfo = VehiSelect[j].VehiculePriceTable;
                            VehiSelect[j].ItemSelected = 0;
                            pas = 0;
                        }
                    }
                    if (MouseRightPressed)
                    {
                        VehiSelect[j].IsOpen = false;
                        Actif -= VehiSelect[j].ArmePos.Length;
                        for (int k = j + 1; k < 2; k += 1)
                            VehiSelect[k].VehiPos = VehiSelect[j].VehiPos + k;
                    }
                }
            }
        }

        public void UpgradeSpread(Weapon ActiveWeapon, int Price)
        {
            ActiveVehicule.Argent -= Price;
            ActiveWeapon.WeaponSpread += ActiveWeapon.SpreadMod;
            ++ActiveWeapon.WeaponSpreadLvl;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Deferred, null);
            g.Draw(Background, Vector2.Zero, Color.White);
            DrawPart1(g);
            DrawPart2(g);

            if (MousePos.X >= 721 && MousePos.X <= 721 + sprBoutonChoice.Width && MousePos.Y >= Constants.Height - sprBoutonChoice.Height - 3 && MousePos.Y <= Constants.Height)
            {
                g.Draw(sprBoutonChoiceP, new Vector2(721, Constants.Height - sprBoutonChoice.Height - 3), Color.White);
            }
            else
            {
                g.Draw(sprBoutonChoice, new Vector2(721, Constants.Height - sprBoutonChoice.Height - 3), Color.White);
            }

            g.DrawString(fntArial15, "Sauvegarder", new Vector2(741, Constants.Height - sprBoutonChoice.Height), Color.White);

            if (MousePos.X >= 873 && MousePos.X <= 873 + sprBoutonChoice.Width && MousePos.Y >= Constants.Height - sprBoutonChoice.Height - 3 && MousePos.Y <= Constants.Height)
            {
                if (MouseLeftPressed)
                {
                    RemoveScreen(this);
                    switch (SuperTank2.CurrentLevel)
                    {
                        case 0:
                            PushScreen(new Level1());
                            break;

                        case 1:
                            PushScreen(new Level2());
                            break;
                    }
                }
                g.Draw(sprBoutonChoiceP, new Vector2(873, Constants.Height - sprBoutonChoice.Height - 3), Color.White);
            }
            else
                g.Draw(sprBoutonChoice, new Vector2(873, Constants.Height - sprBoutonChoice.Height - 3), Color.White);

            g.DrawString(fntArial15, "Continuer", new Vector2(903, Constants.Height - sprBoutonChoice.Height), Color.White);

            g.Draw(Cursor, new Vector2(MousePos.X - (Cursor.Width / 2), MousePos.Y - (Cursor.Height / 2)), Color.White);
        }

        public void DrawPart1(CustomSpriteBatch g)
        {
            g.DrawString(fntArial13, "Joueur 1" + ": " + ActiveVehicule.Name, new Vector2(5, 540), Color.White);
            g.DrawStringRightAligned(fntArial13, "Argent " + ActiveVehicule.Argent + " $", new Vector2(710, 540), Color.White);

            //Armes Tank
            if (ActiveSelection.ItemSelected > 0)
            {
                int Select = ActiveSelection.ItemSelected;
                int PrixTampon = 0;
                Weapon ActiveWeapon = null;
                if (Select <= 3)
                {
                    ActiveWeapon = ActiveVehicule.ArrayWeaponPrimary[Select - 1];
                }
                else
                {
                    ActiveWeapon = ActiveVehicule.ArrayWeaponSecondary[Select - 4];
                }
                ShopItemInfo ActivePriceTable = ActiveSelection.WeaponPriceTable[Select - 1];

                g.DrawString(fntArial15, ActiveWeapon.ArmeLevel.ToString(), new Vector2(530, 569), Color.Black);
                g.DrawString(fntArial15, ActiveWeapon.WeaponRateOfFire.ToString(), new Vector2(100, 594), Color.Black);
                g.DrawString(fntArial15, ActiveWeapon.WeaponSpread.ToString(), new Vector2(120, 619), Color.Black);
                g.DrawString(fntArial15, ActiveWeapon.WeaponEnergy.ToString(), new Vector2(160, 644), Color.Black);
                g.DrawString(fntArial15, ActiveWeapon.WeaponDamage.ToString(), new Vector2(120, 669), Color.Black);
                g.DrawString(fntArial15, ActiveWeapon.WeaponResist.ToString(), new Vector2(120, 694), Color.Black);
                g.DrawString(fntArial15, ActiveWeapon.WeaponSpeed.ToString(), new Vector2(170, 719), Color.Black);
                g.DrawString(fntArial15, ActiveWeapon.WeaponSpecial.ToString(), new Vector2(205, 744), Color.Black);

                g.DrawString(fntArial15, ActiveWeapon.WeaponRateOfFireLvl.ToString(), new Vector2(530, 594), Color.Black);
                g.DrawString(fntArial15, ActiveWeapon.WeaponSpreadLvl.ToString(), new Vector2(530, 619), Color.Black);
                g.DrawString(fntArial15, ActiveWeapon.WeaponEnergyLvl.ToString(), new Vector2(530, 644), Color.Black);
                g.DrawString(fntArial15, ActiveWeapon.WeaponDamageLvl.ToString(), new Vector2(530, 669), Color.Black);
                g.DrawString(fntArial15, ActiveWeapon.WeaponResistLvl.ToString(), new Vector2(530, 694), Color.Black);
                g.DrawString(fntArial15, ActiveWeapon.WeaponSpeedLvl.ToString(), new Vector2(530, 719), Color.Black);
                g.DrawString(fntArial15, ActiveWeapon.WeaponSpecialLvl.ToString(), new Vector2(530, 744), Color.Black);

                //Affichage prix si non niveau maximum
                //Arme
                if (ActiveWeapon.ArmeLevel < 3)
                {
                    PrixTampon = ActivePriceTable.BasePrice + ActiveWeapon.ArmeLevel * ActivePriceTable.PriceMultiplier;
                    if (ActiveVehicule.Argent >= PrixTampon)
                    {
                        g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 569), Color.Black);
                        if (MousePos.X >= 555 && MousePos.X <= 717 && MousePos.Y >= 570 && MousePos.Y <= 589)
                        {
                            g.Draw(SprShopAmeliorerP, new Vector2(555, 570), Color.White);
                            if (MouseLeftPressed)
                            {
                                ActiveVehicule.Argent -= PrixTampon;
                                ++ActiveWeapon.ArmeLevel;
                            }
                        }
                    }
                    else
                        g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 569), Color.Red);
                }
                //Cadence
                if (ActiveWeapon.WeaponRateOfFireLvl < ActiveWeapon.ArmeLevel * 3)
                {
                    PrixTampon = ActivePriceTable.RateOfFirePrice * ActiveWeapon.WeaponRateOfFireLvl / 3;
                    if (ActiveVehicule.Argent >= PrixTampon)
                    {
                        g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 594), Color.Black);
                        if (MousePos.X >= 555 && MousePos.X <= 717 && MousePos.Y >= 595 && MousePos.Y <= 614)
                        {
                            g.Draw(SprShopAmeliorerP, new Vector2(555, 595), Color.White);
                            if (MouseLeftPressed)
                            {
                                ActiveVehicule.Argent -= PrixTampon;
                                ActiveWeapon.WeaponRateOfFire -= ActiveWeapon.RateOfFireMod;
                                ++ActiveWeapon.WeaponRateOfFireLvl;
                            }
                        }
                    }
                    else
                        g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 594), Color.Red);
                }

                #region Dispertion

                switch (Select)
                {
                    case 1:
                        if (ActiveWeapon.WeaponSpreadLvl < (ActiveWeapon.ArmeLevel * 2) - 1)
                        {
                            if (ActiveWeapon.WeaponSpreadLvl >= 1)
                                PrixTampon = ActivePriceTable.SpreadPrice;
                            else if (ActiveWeapon.WeaponSpreadLvl >= 3)
                                PrixTampon = ActivePriceTable.SpreadPrice * 2;
                            else if (ActiveWeapon.WeaponSpreadLvl >= 4)
                                PrixTampon = ActivePriceTable.SpreadPrice * 3;

                            PrixTampon *= ActiveWeapon.WeaponSpreadLvl;
                            if (ActiveVehicule.Argent >= PrixTampon)
                            {
                                g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 619), Color.Black);

                                if (MousePos.X >= 555 && MousePos.X <= 717 && MousePos.Y >= 619 && MousePos.Y <= 638)
                                {
                                    g.Draw(SprShopAmeliorerP, new Vector2(555, 619), Color.White);
                                    if (MouseLeftPressed)
                                    {
                                        UpgradeSpread(ActiveWeapon, PrixTampon);
                                    }
                                }
                            }
                            else
                                g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 619), Color.Red);
                        }
                        break;

                    case 2:
                    case 3:
                        if (ActiveWeapon.WeaponSpreadLvl < ActiveWeapon.ArmeLevel)
                        {
                            if (ActiveWeapon.WeaponSpreadLvl >= 1)
                                PrixTampon = ActivePriceTable.SpreadPrice;
                            else if (ActiveWeapon.WeaponSpreadLvl >= 3)
                                PrixTampon = ActivePriceTable.SpreadPrice * 2;
                            else if (ActiveWeapon.WeaponSpreadLvl >= 4)
                                PrixTampon = ActivePriceTable.SpreadPrice * 3;

                            PrixTampon *= ActiveWeapon.WeaponSpreadLvl;
                            if (ActiveVehicule.Argent >= PrixTampon)
                            {
                                g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 619), Color.Black);

                                if (MousePos.X >= 555 && MousePos.X <= 717 && MousePos.Y >= 619 && MousePos.Y <= 638)
                                {
                                    g.Draw(SprShopAmeliorerP, new Vector2(555, 619), Color.White);
                                    if (MouseLeftPressed)
                                    {
                                        UpgradeSpread(ActiveWeapon, PrixTampon);
                                    }
                                }
                            }
                            else
                                g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 619), Color.Red);
                        }
                        break;

                    case 7:
                        if (ActiveWeapon.WeaponSpreadLvl < ActiveWeapon.ArmeLevel)
                        {
                            if (ActiveWeapon.WeaponSpreadLvl >= 0)
                                PrixTampon = ActivePriceTable.SpreadPrice;
                            else if (ActiveWeapon.WeaponSpreadLvl >= 3)
                                PrixTampon = ActivePriceTable.SpreadPrice * 2;
                            else if (ActiveWeapon.WeaponSpreadLvl >= 6)
                                PrixTampon = ActivePriceTable.SpreadPrice * 3;

                            PrixTampon *= ActiveWeapon.WeaponSpreadLvl;
                            if (ActiveVehicule.Argent >= PrixTampon)
                            {
                                g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 619), Color.Black);

                                if (MousePos.X >= 555 && MousePos.X <= 717 && MousePos.Y >= 619 && MousePos.Y <= 638)
                                {
                                    g.Draw(SprShopAmeliorerP, new Vector2(555, 619), Color.White);
                                    if (MouseLeftPressed)
                                    {
                                        UpgradeSpread(ActiveWeapon, PrixTampon);
                                    }
                                }
                            }
                            else
                                g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 619), Color.Red);
                        }
                        break;

                    default:
                        if (ActiveWeapon.WeaponSpreadLvl < ActiveWeapon.ArmeLevel * 3)
                        {
                            if (ActiveWeapon.WeaponSpreadLvl >= 0)
                                PrixTampon = ActivePriceTable.SpreadPrice;
                            else if (ActiveWeapon.WeaponSpreadLvl >= 3)
                                PrixTampon = ActivePriceTable.SpreadPrice * 2;
                            else if (ActiveWeapon.WeaponSpreadLvl >= 6)
                                PrixTampon = ActivePriceTable.SpreadPrice * 3;

                            PrixTampon *= ActiveWeapon.WeaponSpreadLvl;
                            if (ActiveVehicule.Argent >= PrixTampon)
                            {
                                g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 619), Color.Black);

                                if (MousePos.X >= 555 && MousePos.X <= 717 && MousePos.Y >= 619 && MousePos.Y <= 638)
                                {
                                    g.Draw(SprShopAmeliorerP, new Vector2(555, 619), Color.White);
                                    if (MouseLeftPressed)
                                    {
                                        UpgradeSpread(ActiveWeapon, PrixTampon);
                                    }
                                }
                            }
                            else
                                g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 619), Color.Red);
                        }
                        break;
                }

                #endregion

                //Énergie
                if (ActiveWeapon.WeaponEnergyLvl < ActiveWeapon.ArmeLevel * 3)
                {
                    PrixTampon = ActivePriceTable.EnergyPrice * ActiveWeapon.WeaponEnergyLvl / 3;
                    if (ActiveVehicule.Argent >= PrixTampon)
                    {
                        g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 644), Color.Black);

                        if (MousePos.X >= 555 && MousePos.X <= 717 && MousePos.Y >= 645 && MousePos.Y <= 664)
                        {
                            g.Draw(SprShopAmeliorerP, new Vector2(555, 645), Color.White);
                            if (MouseLeftPressed)
                            {
                                ActiveVehicule.Argent -= PrixTampon;
                                ActiveWeapon.WeaponEnergy -= ActiveWeapon.EnergyMod;
                                ++ActiveWeapon.WeaponEnergyLvl;
                            }
                        }
                    }
                    else
                        g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 644), Color.Red);
                }
                //Dégât
                if (ActiveWeapon.WeaponDamageLvl < ActiveWeapon.ArmeLevel * 3)
                {
                    PrixTampon = ActivePriceTable.DamagePrice * ActiveWeapon.WeaponDamageLvl / 3;
                    if (ActiveVehicule.Argent >= PrixTampon)
                    {
                        g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 669), Color.Black);

                        if (MousePos.X >= 555 && MousePos.X <= 717 && MousePos.Y >= 670 && MousePos.Y <= 689)
                        {
                            g.Draw(SprShopAmeliorerP, new Vector2(555, 670), Color.White);
                            if (MouseLeftPressed)
                            {
                                ActiveVehicule.Argent -= PrixTampon;
                                ActiveWeapon.WeaponDamage += ActiveWeapon.PowerMod;
                                ++ActiveWeapon.WeaponDamageLvl;
                            }
                        }
                    }
                    else
                        g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 669), Color.Red);
                }
                //Résistence
                if (ActiveWeapon.WeaponResistLvl < ActiveWeapon.ArmeLevel * 3)
                {
                    PrixTampon = ActivePriceTable.ResistPrice * ActiveWeapon.WeaponResistLvl / 3;
                    if (ActiveVehicule.Argent >= PrixTampon)
                    {
                        g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 694), Color.Black);
                        if (MousePos.X >= 555 && MousePos.X <= 717 && MousePos.Y >= 695 && MousePos.Y <= 714)
                        {
                            g.Draw(SprShopAmeliorerP, new Vector2(555, 695), Color.White);
                            if (MouseLeftPressed)
                            {
                                ActiveVehicule.Argent -= PrixTampon;
                                ActiveWeapon.WeaponResist += ActiveWeapon.ResistMod;
                                ++ActiveWeapon.WeaponResistLvl;
                            }
                        }
                    }
                    else
                        g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 694), Color.Red);
                }
                //Vitesse
                if (ActiveWeapon.WeaponSpeedLvl < ActiveWeapon.ArmeLevel * 3)
                {
                    PrixTampon = ActivePriceTable.SpeedPrice * ActiveWeapon.WeaponSpeedLvl / 3;
                    if (ActiveVehicule.Argent >= PrixTampon)
                    {
                        g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 719), Color.Black);
                        if (MousePos.X >= 555 && MousePos.X <= 717 && MousePos.Y >= 720 && MousePos.Y <= 739)
                        {
                            g.Draw(SprShopAmeliorerP, new Vector2(555, 720), Color.White);
                            if (MouseLeftPressed)
                            {
                                ActiveVehicule.Argent -= PrixTampon;
                                ActiveWeapon.WeaponSpeed += ActiveWeapon.SpeedMod;
                                ++ActiveWeapon.WeaponSpeedLvl;
                            }
                        }
                    }
                    else
                        g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 719), Color.Red);
                }
            }

            if (ActiveShopItemInfo.ItemText != null)
            {
                TypewritterText(g, 50, 100, 2);
                g.DrawString(fntArial15, ActiveShopItemInfo.ItemText, new Vector2(150, 569), Color.Black);
            }
            switch (ActiveSelection.ItemSelected)
            {
                //Tank choisi
                case 0:

                    g.DrawString(fntArial15, ActiveVehicule.VehiculeLevel.ToString(), new Vector2(530, 569), Color.Black);
                    g.DrawString(fntArial15, ActiveVehicule.VehiculePerte.ToString(), new Vector2(160, 644), Color.Black);
                    g.DrawString(fntArial15, ActiveVehicule.VehiculeResist.ToString(), new Vector2(120, 694), Color.Black);
                    g.DrawString(fntArial15, ActiveVehicule.VehiculeVitesse.ToString(), new Vector2(170, 719), Color.Black);
                    g.DrawString(fntArial15, ActiveVehicule.VehiculeSpecial.ToString(), new Vector2(205, 744), Color.Black);
                    g.DrawString(fntArial15, ActiveVehicule.VehiculePerteLvl.ToString(), new Vector2(530, 644), Color.Black);
                    g.DrawString(fntArial15, ActiveVehicule.VehiculeResistLvl.ToString(), new Vector2(530, 694), Color.Black);
                    g.DrawString(fntArial15, ActiveVehicule.VehiculeVitesseLvl.ToString(), new Vector2(530, 719), Color.Black);
                    g.DrawString(fntArial15, ActiveVehicule.VehiculeSpecialLvl.ToString(), new Vector2(530, 744), Color.Black);

                    //Véhicule
                    if (ActiveVehicule.VehiculeLevel < 3)
                    {
                        int PrixTampon = ActiveSelection.VehiculePriceTable.BasePrice * ActiveVehicule.VehiculeLevel;
                        if (ActiveVehicule.Argent >= PrixTampon)
                        {
                            g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 569), Color.Black);

                            if (MousePos.X >= 555 && MousePos.X <= 717 && MousePos.Y >= 570 && MousePos.Y <= 589)
                            {
                                g.Draw(SprShopAmeliorerP, new Vector2(555, 570), Color.White);
                                if (MouseLeftPressed)
                                {
                                    ActiveVehicule.Argent -= PrixTampon;
                                    ++ActiveVehicule.VehiculeLevel;
                                }
                            }
                            else
                                g.Draw(SprShopAmeliorer, new Vector2(555, 570), Color.White);
                        }
                        else
                            g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 569), Color.Red);
                    }
                    //Énergie
                    if (ActiveVehicule.VehiculePerteLvl < ActiveVehicule.VehiculeLevel * 3)
                    {
                        int PrixTampon = ActiveSelection.VehiculePriceTable.EnergyPrice * ActiveVehicule.VehiculePerteLvl / 3;
                        if (ActiveVehicule.Argent >= PrixTampon)
                        {
                            g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 644), Color.Black);

                            if (MousePos.X >= 555 && MousePos.X <= 717 && MousePos.Y >= 645 && MousePos.Y <= 664)
                            {
                                g.Draw(SprShopAmeliorerP, new Vector2(555, 645), Color.White);
                                if (MouseLeftPressed)
                                {
                                    ActiveVehicule.Argent -= PrixTampon;
                                    ActiveVehicule.VehiculePerte -= ActiveVehicule.EnergyMod;
                                    ++ActiveVehicule.VehiculePerteLvl;
                                }
                            }
                            else
                                g.Draw(SprShopAmeliorer, new Vector2(555, 645), Color.White);
                        }
                        else
                            g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 644), Color.Red);
                    }
                    //Résistence
                    if (ActiveVehicule.VehiculeResistLvl < ActiveVehicule.VehiculeLevel * 3)
                    {
                        int PrixTampon = ActiveSelection.VehiculePriceTable.ResistPrice * ActiveVehicule.VehiculeResistLvl / 3;
                        if (ActiveVehicule.Argent >= PrixTampon)
                        {
                            g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 694), Color.Black);
                            if (MousePos.X >= 555 && MousePos.X <= 717 && MousePos.Y >= 695 && MousePos.Y <= 714)
                            {
                                g.Draw(SprShopAmeliorerP, new Vector2(555, 695), Color.White);
                                if (MouseLeftPressed)
                                {
                                    ActiveVehicule.Argent -= PrixTampon;
                                    ActiveVehicule.VehiculeResist -= ActiveVehicule.ResistMod;
                                    ++ActiveVehicule.VehiculeResistLvl;
                                }
                            }
                            else
                                g.Draw(SprShopAmeliorer, new Vector2(555, 695), Color.White);
                        }
                        else
                            g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 694), Color.Red);
                    }
                    //Vitesse
                    if (ActiveVehicule.VehiculeVitesseLvl < ActiveVehicule.VehiculeLevel * 3)
                    {
                        int PrixTampon = ActiveSelection.VehiculePriceTable.SpeedPrice * ActiveVehicule.VehiculeVitesseLvl / 3;
                        if (ActiveVehicule.Argent >= PrixTampon)
                        {
                            g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 719), Color.Black);
                            if (MousePos.X >= 555 && MousePos.X <= 717 && MousePos.Y >= 720 && MousePos.Y <= 739)
                            {
                                g.Draw(SprShopAmeliorerP, new Vector2(555, 720), Color.White);
                                if (MouseLeftPressed)
                                {
                                    ActiveVehicule.Argent -= PrixTampon;
                                    ActiveVehicule.VehiculeVitesse -= ActiveVehicule.SpeedMod;
                                    ActiveVehicule.VehiculeVitesseLvl += 1;
                                }
                            }
                            else
                                g.Draw(SprShopAmeliorer, new Vector2(555, 720), Color.White);
                        }
                        else
                            g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 719), Color.Red);
                    }
                    //Spécial
                    if (ActiveVehicule.VehiculeSpecialLvl < ActiveVehicule.VehiculeLevel * 3)
                    {
                        int PrixTampon = ActiveSelection.VehiculePriceTable.SpecialPrice * ActiveVehicule.VehiculeSpecialLvl / 3;
                        if (ActiveVehicule.Argent >= PrixTampon)
                        {
                            g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 744), Color.Black);
                            if (MousePos.X >= 555 && MousePos.X <= 717 && MousePos.Y >= 745 && MousePos.Y <= 764)
                            {
                                g.Draw(SprShopAmeliorerP, new Vector2(555, 745), Color.White);
                                if (MouseLeftPressed)
                                {
                                    ActiveVehicule.Argent -= PrixTampon;
                                    ++ActiveVehicule.VehiculeSpecialLvl;
                                    //Niveau 4 = Barre de rage
                                    if (ActiveVehicule.VehiculeSpecialLvl == 2)
                                        ActiveVehicule.VehiculeEnergieMax += 50;
                                    if (ActiveVehicule.VehiculeSpecialLvl == 6)
                                        ActiveVehicule.VehiculeResistMax += 50;
                                    if (ActiveVehicule.VehiculeSpecialLvl == 8)
                                        ActiveVehicule.VehiculeResistMax += 50;
                                    if (ActiveVehicule.VehiculeSpecialLvl % 2 == 1)//1, 3, 5, 7, 9
                                        ActiveVehicule.VehiculeSpecial += 1;
                                }
                            }
                            else
                                g.Draw(SprShopAmeliorer, new Vector2(555, 745), Color.White);
                        }
                        else
                            g.DrawStringRightAligned(fntArial15, PrixTampon + "$", new Vector2(440, 744), Color.Red);
                    }
                    break;
            }
        }

        public void DrawPart2(CustomSpriteBatch g)
        {//48pixel de haut 14 icones affiché
            g.Draw(Shop_monter, new Vector2(720, 28), Color.White);
            g.Draw(Shop_descendre, new Vector2(720, 713), Color.White);
            for (int j = 0; j < 2; j += 1)
            {//Véhicule
                if (VehiSelect[j].VehiPos - SelectPos >= 0 && VehiSelect[j].VehiPos - SelectPos <= 13)
                {
                    if (VehiSelect[j].IsOpen)
                        g.Draw(VehiIco_p[j], new Vector2(720, (VehiSelect[j].VehiPos - SelectPos + 1) * 48), Color.White);
                    else
                    {
                        if (MousePos.X > 720 && MousePos.Y > (VehiSelect[j].VehiPos - SelectPos + 1) * 48 && MousePos.Y < (VehiSelect[j].VehiPos - SelectPos + 1) * 48 + 38)
                            g.Draw(VehiIco_p[j], new Vector2(720, (VehiSelect[j].VehiPos - SelectPos + 1) * 48), Color.FromNonPremultiplied(255, 255, 255, 200));
                        else
                            g.Draw(VehiIco_p[j], new Vector2(720, (VehiSelect[j].VehiPos - SelectPos + 1) * 48), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                }//Armes
                if (VehiSelect[j].IsOpen)
                {
                    for (int i = 0; i < VehiSelect[j].ArmePos.Length; i += 1)
                    {
                        if (VehiSelect[j].VehiPos + i - SelectPos + 1 >= 0 && VehiSelect[j].VehiPos + i - SelectPos + 1 <= 13)
                        {
                            if (ActiveSelection.ItemSelected == i + 1)
                            {
                                g.Draw(sprWeaponIconsP[i, j], new Vector2(720, (VehiSelect[j].VehiPos + i - SelectPos + 1) * 48 + 48), Color.White);
                                if (MouseLeftPressed)
                                {
                                    if (MousePos.X > 720 && MousePos.Y > (VehiSelect[j].VehiPos + i - SelectPos + 1) * 48 + 48 && MousePos.Y < (VehiSelect[j].VehiPos + i - SelectPos + 1) * 48 + 38 + 48)
                                        pas = ActiveShopItemInfo.ItemDescription.Length;
                                }
                            }
                            else
                            {
                                if (MousePos.X > 720 && MousePos.Y > (VehiSelect[j].VehiPos + i - SelectPos + 1) * 48 + 48 && MousePos.Y < (VehiSelect[j].VehiPos + i - SelectPos + 1) * 48 + 38 + 48)
                                {
                                    g.Draw(sprWeaponIconsP[i, j], new Vector2(720, (VehiSelect[j].VehiPos + i - SelectPos + 1) * 48 + 48), Color.FromNonPremultiplied(255, 255, 255, 200));
                                    if (MouseLeftPressed)
                                    {
                                        ActiveShopItemInfo = VehiSelect[j].WeaponPriceTable[i];
                                        ActiveSelection = VehiSelect[j];
                                        ActiveSelection.ItemSelected = i + 1;
                                        pas = 0;
                                    }
                                }
                                else
                                    g.Draw(sprWeaponIcons[i, j], new Vector2(720, (VehiSelect[j].VehiPos + i - SelectPos + 1) * 48 + 48), Color.FromNonPremultiplied(255, 255, 255, 127));
                            }
                        }
                    }
                }
            }
        }
    }
}
