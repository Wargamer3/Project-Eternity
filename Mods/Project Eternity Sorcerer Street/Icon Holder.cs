using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class IconHolder
    {
        public Texture2D sprCreatureCompareBetter;
        public Texture2D sprCreatureCompareEqual;
        public Texture2D sprCreatureCompareWorse;

        public Texture2D sprCreatureAntiElement;
        public Texture2D sprCreatureBulimia;
        public Texture2D sprCreatureConfusion;
        public Texture2D sprCreatureDrought;
        public Texture2D sprCreatureFistfight;
        public Texture2D sprCreatureFog;
        public Texture2D sprCreatureGreed;
        public Texture2D sprCreatureHolyAsylum;
        public Texture2D sprCreatureHustle;
        public Texture2D sprCreatureIllness;
        public Texture2D sprCreatureLandProtection;
        public Texture2D sprCreatureLiquidForm;
        public Texture2D sprCreatureMimicry;
        public Texture2D sprCreaturePeace;
        public Texture2D sprCreatureQuicksand;
        public Texture2D sprCreatureParalysis;
        public Texture2D sprCreaturePhantasm;
        public Texture2D sprCreaturePoison;
        public Texture2D sprCreatureReflect;
        public Texture2D sprCreatureSenility;
        public Texture2D sprCreatureSoulHunt;
        public Texture2D sprCreatureSimulacrum;
        public Texture2D sprCreatureSleep;
        public Texture2D sprCreatureSpiritWalk;

        public Texture2D sprMenuLimitArmor;
        public Texture2D sprMenuLimitCardSacrifice;
        public Texture2D sprMenuLimitCardSummon;
        public Texture2D sprMenuLimitG;
        public Texture2D sprMenuLimitLand;
        public Texture2D sprMenuLimitScroll;
        public Texture2D sprMenuLimitWeapon;
        public Texture2D sprMenuDefensive;
        public Texture2D sprMenuLvMax;

        public Texture2D sprPlayerAntiMagic;
        public Texture2D sprPlayerBraveSong;
        public Texture2D sprPlayerToughSong;
        public Texture2D sprPlayerSinkingSong;
        public Texture2D sprPlayerConspiracy;
        public Texture2D sprPlayerDreamTerrain;//Tolls are affected in a positive way (either by paying less, or collecting more).
        public Texture2D sprPlayerDrought;
        public Texture2D sprPlayerFairyLight;
        public Texture2D sprPlayerFear;
        public Texture2D sprPlayerHoldCurse;
        public Texture2D sprPlayerInnocence;
        public Texture2D sprPlayerImprisonment;
        public Texture2D sprPlayerMesozoicSong;
        public Texture2D sprPlayerMovement;
        public Texture2D sprPlayerSilence;
        public Texture2D sprPlayerTerritory;//Revelation, Telegnosis
        public Texture2D sprPlayerToll;//Tolls are affected in a negative way (either by paying more, or collecting less).
        public Texture2D sprPlayerTriumph;
        public Texture2D sprPlayerWaste;

        public Texture2D sprConfirm;
        public Texture2D sprCancel;
        public Texture2D sprOption;

        public static IconHolder Icons;

        public static void Load(ContentManager Content)
        {
            Icons = new IconHolder();

            Icons.sprCreatureCompareBetter = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Compare Better");
            Icons.sprCreatureCompareEqual = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Compare Equal");
            Icons.sprCreatureCompareWorse = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Compare Worse");

            Icons.sprCreatureAntiElement = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Anti-Element");
            Icons.sprCreatureBulimia = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Bulimia");
            Icons.sprCreatureConfusion = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Confusion");
            Icons.sprCreatureDrought = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Drought");
            Icons.sprCreatureFistfight = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Fistfight");
            Icons.sprCreatureFog = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Fog");
            Icons.sprCreatureGreed = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Greed");
            Icons.sprCreatureHolyAsylum = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Holy Asylum");
            Icons.sprCreatureHustle = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Hustle");
            Icons.sprCreatureIllness = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Illness");
            Icons.sprCreatureLandProtection = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Land Protection");
            Icons.sprCreatureLiquidForm = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Liquid Form");
            Icons.sprCreatureMimicry = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Mimicry");
            Icons.sprCreaturePeace = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Peace");
            Icons.sprCreatureParalysis = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Paralysis");
            Icons.sprCreaturePoison = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Poison");
            Icons.sprCreaturePhantasm = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Phantasm");
            Icons.sprCreatureQuicksand = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Quicksand");
            Icons.sprCreatureReflect = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Reflect");
            Icons.sprCreatureSimulacrum = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Simulacrum");
            Icons.sprCreatureSpiritWalk = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Spirit Walk");
            Icons.sprCreatureSenility = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Senelity");
            Icons.sprCreatureSleep = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Sleep");
            Icons.sprCreatureSoulHunt = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Soul Hunt");

            Icons.sprMenuLimitArmor = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Limit Armor");
            Icons.sprMenuLimitCardSacrifice = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Limit Card Sacrifice");
            Icons.sprMenuLimitCardSummon = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Limit Card Summon");
            Icons.sprMenuLimitG = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Limit G");
            Icons.sprMenuLimitLand = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Limit Land");
            Icons.sprMenuLimitScroll = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Limit Scroll");
            Icons.sprMenuLimitWeapon = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Limit Weapon");
            Icons.sprMenuDefensive = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Defensive");
            Icons.sprMenuLvMax = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Lv Max");

            Icons.sprPlayerAntiMagic = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Anti-Magic");
            Icons.sprPlayerBraveSong = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Brave Song");
            Icons.sprPlayerToughSong = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Tough Song");
            Icons.sprPlayerSinkingSong = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Sinking Song");
            Icons.sprPlayerConspiracy = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Conspiracy");
            Icons.sprPlayerDreamTerrain = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Dream Terrain");
            Icons.sprPlayerDrought = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Drought");
            Icons.sprPlayerFairyLight = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Fairy Light");
            Icons.sprPlayerFear = null;
            Icons.sprPlayerHoldCurse = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Hold Curse");
            Icons.sprPlayerInnocence = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Innocence");
            Icons.sprPlayerImprisonment = null;
            Icons.sprPlayerTerritory = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Territory");
            Icons.sprPlayerMesozoicSong = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Mesozoic Song");
            Icons.sprPlayerMovement = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Movement");
            Icons.sprPlayerSilence = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Silence");
            Icons.sprPlayerToll = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Toll");
            Icons.sprPlayerTriumph = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Triumph");
            Icons.sprPlayerWaste = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Waste");

            Icons.sprConfirm = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/A");
            Icons.sprCancel = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/B");
            Icons.sprOption = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menus/RB");
        }
    }
}
