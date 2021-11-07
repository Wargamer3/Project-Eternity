using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class WeaponLimitScreen : GameScreen
    {
        private Texture2D sprBackground;
        private Texture2D sprAssaultRifle;
        private Texture2D sprGrenadeLauncher;
        private Texture2D sprMachineGun;
        private Texture2D sprShotgun;
        private Texture2D sprSniper;
        private Texture2D sprSubmachineGun;

        private readonly Player ActivePlayer;
        private readonly InteractiveButton.OnClick OnConfirm;

        public WeaponLimitScreen(Player ActivePlayer, InteractiveButton.OnClick OnConfirm)
        {
            this.ActivePlayer = ActivePlayer;
            this.OnConfirm = OnConfirm;
        }

        public override void Load()
        {
            sprBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Weapon Limit/Background");
            sprAssaultRifle = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Weapon Limit/Assault Rifle");
            sprGrenadeLauncher = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Weapon Limit/Grenade Launcher");
            sprMachineGun = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Weapon Limit/Machine Gun");
            sprShotgun = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Weapon Limit/Shotgun");
            sprSniper = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Weapon Limit/Sniper Rifle");
            sprSubmachineGun = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Weapon Limit/Submachine Gun");
        }

        public override void Unload()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
