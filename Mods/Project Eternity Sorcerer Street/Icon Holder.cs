using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class IconHolder
    {
        public Texture2D sprCreatureCompareBetter;
        public Texture2D sprCreatureCompareEqual;
        public Texture2D sprCreatureCompareWorse;
        public Texture2D sprCreatureNeutral;
        public Texture2D sprCreaturePoison;
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

        public Texture2D sprPlayerDrought;
        public Texture2D sprPlayerHaste;
        public Texture2D sprPlayerSilence;
        public Texture2D sprPlayerTerritory;
        public Texture2D sprPlayerToll;
        public Texture2D sprPlayerToughSong;
        public Texture2D sprPlayerWaste;
        public Texture2D sprPlayerWeak;

        public static IconHolder Icons;

        public static void Load(ContentManager Content)
        {
            Icons = new IconHolder();

            Icons.sprCreatureCompareBetter = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Compare Better");
            Icons.sprCreatureCompareEqual = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Compare Equal");
            Icons.sprCreatureCompareWorse = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Compare Worse");
            Icons.sprCreatureNeutral = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Neutral");
            Icons.sprCreaturePoison = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Poison");
            Icons.sprCreatureSpiritWalk = Content.Load<Texture2D>("Sorcerer Street/Ressources/Creature Effects Icons/Creature Spirit Walk");

            Icons.sprMenuLimitArmor = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Limit Armor");
            Icons.sprMenuLimitCardSacrifice = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Limit Card Sacrifice");
            Icons.sprMenuLimitCardSummon = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Limit Card Summon");
            Icons.sprMenuLimitG = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Limit G");
            Icons.sprMenuLimitLand = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Limit Land");
            Icons.sprMenuLimitScroll = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Limit Scroll");
            Icons.sprMenuLimitWeapon = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Limit Weapon");
            Icons.sprMenuDefensive = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Defensive");
            Icons.sprMenuLvMax = Content.Load<Texture2D>("Sorcerer Street/Ressources/Menu Effects Icons/Menu Lv Max");

            Icons.sprPlayerDrought = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Drought");
            Icons.sprPlayerHaste = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Haste");
            Icons.sprPlayerSilence = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Silence");
            Icons.sprPlayerTerritory = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Territory");
            Icons.sprPlayerToll = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Toll");
            Icons.sprPlayerToughSong = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Tough Song");
            Icons.sprPlayerWaste = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Waste");
            Icons.sprPlayerWeak = Content.Load<Texture2D>("Sorcerer Street/Ressources/Player Effects Icons/Player Weak");
        }
    }
}
