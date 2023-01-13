using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelCommanderMenu : ActionPanelDeathmatch
    {
        private const string PanelName = "Commander";

        private SpriteFont fntFinlanderFont;

        private float ScrollTextOffsetSpirits;
        private float ScrollTextOffsetMaxSpirits;

        private const int SpiritTextMaxSize = 160;
        private const float ScrollingTextPixelPerSecondSpeed = 10;

        private readonly Player ActivePlayer;

        public ActionPanelCommanderMenu(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
            this.fntFinlanderFont = Map.fntFinlanderFont;
        }

        public ActionPanelCommanderMenu(DeathmatchMap Map, Player ActivePlayer)
            : base(PanelName, Map, false)
        {
            this.fntFinlanderFont = Map.fntFinlanderFont;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ActiveInputManager.InputConfirmPressed() || ActiveInputManager.InputCancelPressed())
            {
                //Reset the cursor.
                RemoveFromPanelList(this);
            }
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelCommanderMenu(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            GameScreen.DrawBox(g, new Vector2(25, 0), 578, 25, Color.White);

            TextHelper.DrawText(g, "Description",
                new Vector2(32, 3), Color.White);

            const int BoxWidth = 600;
            const int BoxHeight = 350;
            float MenuX = (Constants.Width - BoxWidth) / 2f;
            float MenuY = (Constants.Height - BoxHeight + 40) / 2f;

            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY - 40), 180, 40, Color.Black);
            GameScreen.DrawBox(g, new Vector2(Constants.Width - MenuX - 180, MenuY - 40), 180, 40, Color.Black);
            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY), BoxWidth, 45, Color.Black);
            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY + 45), BoxWidth, BoxHeight - 45, Color.White);

            g.DrawString(Map.fntFinlanderFont, "Commanders", new Vector2(MenuX + 5, MenuY - 35), Color.White);
            g.DrawString(Map.fntFinlanderFont, "CP Left: 20", new Vector2(Constants.Width - MenuX - 170, MenuY - 35), Color.White);

            for (int i = 0; i < 3; ++i)
            {
                float X = MenuX + 10 + i * 170;
                float Y = MenuY + 60;

                TextHelper.DrawTextMultiline(g, TextHelper.FitToWidth(TextHelper.fntShadowFont, "Saotome Institute", 100),
                    TextHelper.TextAligns.Left, X + 60, MenuY + 5, 100);

                for (int S = 0; S < 6; S++)
                {
                    string SkillName = "super powerfull spell";
                    if (fntFinlanderFont.MeasureString(SkillName).X > SpiritTextMaxSize)
                    {
                        TextHelper.DrawText(g, SkillName,
                            new Vector2(X - Math.Max(0, ScrollTextOffsetSpirits), Y + S * 30), Color.White);
                    }
                    else
                    {
                        TextHelper.DrawText(g, SkillName, new Vector2(X, Y + S * 30), Color.White);
                    }

                    TextHelper.DrawTextRightAligned(g, "5", new Vector2(X + 170, Y + S * 30), Color.White);
                }
            }
        }
    }
}
