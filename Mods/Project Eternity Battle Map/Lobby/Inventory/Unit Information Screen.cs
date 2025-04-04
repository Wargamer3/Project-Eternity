using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class InventoryUnitInformationScreen : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;

        private readonly BattleMapInventoryScreen Owner;
        private readonly Unit ActiveUnit;
        private readonly StatusMenuScreen StatusMenu;

        public InventoryUnitInformationScreen(BattleMapInventoryScreen Owner, Unit ActiveUnit)
        {
            this.Owner = Owner;
            this.ActiveUnit = ActiveUnit;
            StatusMenu = new StatusMenuScreen(null);
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            StatusMenu.Load();
            StatusMenu.ActiveSquad = new Squad("", ActiveUnit);
            StatusMenu.StatusPannel = StatusMenuScreen.StatusPannels.Unit;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            StatusMenu.Draw(g);
        }
    }
}
