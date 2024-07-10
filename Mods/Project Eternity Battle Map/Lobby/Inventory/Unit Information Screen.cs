using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.UI;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class InventoryUnitInformationScreen : GameScreen
    {
        private EmptyBoxScrollbar SquadListScrollbar;

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;

        private int SquadScrollbarValue;

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
            SquadListScrollbar = new EmptyBoxScrollbar(new Vector2(BattleMapInventoryScreen.LeftSideWidth - 23, BattleMapInventoryScreen.MiddleSectionY + 3), BattleMapInventoryScreen.MiddleSectionHeight - 5, 10, OnSquadScrollbarChange);

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            int ItemHeight = 30;
            SquadListScrollbar.ChangeMaxValue((ActiveUnit.ListSkin.Count + ActiveUnit.ListAlt.Count) * ItemHeight - BattleMapInventoryScreen.MiddleSectionHeight);

            StatusMenu.Load();
            StatusMenu.ActiveSquad = new Squad("", ActiveUnit);
            StatusMenu.StatusPannel = StatusMenuScreen.StatusPannels.Unit;
        }

        public override void Update(GameTime gameTime)
        {
        }

        private void OnSquadScrollbarChange(float ScrollbarValue)
        {
            SquadScrollbarValue = (int)ScrollbarValue;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            StatusMenu.Draw(g);
        }
    }
}
