using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class InventoryScreen : GameScreen
    {
        private enum InventoryCategories { Units, Characters, Equipment, Consumable }

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;

        private BattleMapPlayer ActivePlayer;

        public InventoryScreen()
        {
            ActivePlayer = PlayerManager.ListLocalPlayer[0];
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");
        }

        public override void Unload()
        {
            SoundSystem.ReleaseSound(sndButtonOver);
            SoundSystem.ReleaseSound(sndButtonClick);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawBox(g, new Vector2(), Constants.Width, Constants.Height, Color.White);

            int LeftSideWidth = (int)(Constants.Width * 0.5);
            int TopSectionHeight = (int)(Constants.Height * 0.1);
            int HeaderSectionY = TopSectionHeight;
            int HeaderSectionHeight = (int)(Constants.Height * 0.05);

            int BottomSectionHeight = (int)(Constants.Height * 0.07);
            int BottomSectionY = Constants.Height - BottomSectionHeight;

            int MiddleSectionY = (int)(HeaderSectionY + HeaderSectionHeight);
            int MiddleSectionHeight = BottomSectionY - MiddleSectionY;

            DrawBox(g, new Vector2(0, 0), (int)(Constants.Width * 0.7), TopSectionHeight, Color.White);
            DrawBox(g, new Vector2(Constants.Width * 0.7f, 0), (int)(Constants.Width * 0.3), TopSectionHeight, Color.White);
            g.DrawString(fntArial12, "INVENTORY", new Vector2(10, 20), Color.White);
            g.DrawString(fntArial12, "Back To Lobby", new Vector2(Constants.Width * 0.7f + 70, 20), Color.White);

            //Left side
            DrawBox(g, new Vector2(0, HeaderSectionY), LeftSideWidth, HeaderSectionHeight, Color.White);
            DrawBox(g, new Vector2(0, MiddleSectionY), LeftSideWidth, MiddleSectionHeight, Color.White);
            DrawBox(g, new Vector2(0, BottomSectionY), LeftSideWidth, Constants.Height - BottomSectionY, Color.White);
            g.DrawString(fntArial12, "Loadouts", new Vector2(10, TopSectionHeight + 5), Color.White);

            float DrawY = MiddleSectionY + 5;
            for (int i = 0; i < 4; ++i)
            {
                DrawBox(g, new Vector2(5, DrawY), LeftSideWidth - 10, 45, Color.White);
                DrawBox(g, new Vector2(LeftSideWidth - 95, DrawY + 5), 85, 35, Color.White);
                g.DrawString(fntArial12, "Rename", new Vector2(LeftSideWidth - 88, DrawY + 11), Color.White);
                g.DrawString(fntArial12, "Loadout " + (i + 1), new Vector2(11, DrawY + 11), Color.White);
                DrawBox(g, new Vector2(101, DrawY + 6), 32, 32, Color.White);
                DrawBox(g, new Vector2(141, DrawY + 6), 32, 32, Color.White);
                DrawBox(g, new Vector2(181, DrawY + 6), 32, 32, Color.White);
                DrawBox(g, new Vector2(221, DrawY + 6), 32, 32, Color.White);
                DrawBox(g, new Vector2(261, DrawY + 6), 32, 32, Color.White);
                DrawY += 50;
            }
            g.DrawStringRightAligned(fntArial12, "Money: 14360 cr", new Vector2(LeftSideWidth - 12, BottomSectionY + 11), Color.White);

            DrawBox(g, new Vector2(LeftSideWidth, HeaderSectionY), LeftSideWidth, HeaderSectionHeight, Color.White);
            DrawBox(g, new Vector2(LeftSideWidth, MiddleSectionY), LeftSideWidth, MiddleSectionHeight, Color.White);
            DrawBox(g, new Vector2(LeftSideWidth, BottomSectionY), LeftSideWidth, Constants.Height - BottomSectionY, Color.White);
            g.DrawString(fntArial12, "Units/Characters/Equipment/Consumable", new Vector2(LeftSideWidth + 10, TopSectionHeight + 5), Color.White);

            DrawY = MiddleSectionY + 5;
            for (int i = 0; i < 4; ++i)
            {
                DrawBox(g, new Vector2(LeftSideWidth + 5, DrawY), LeftSideWidth - 95, 45, Color.White);
                g.DrawString(fntArial12, "Mazinger", new Vector2(LeftSideWidth + 48, DrawY + 11), Color.White);
                DrawBox(g, new Vector2(LeftSideWidth + 11, DrawY + 6), 32, 32, Color.White);
                DrawY += 50;
            }
        }
    }
}
