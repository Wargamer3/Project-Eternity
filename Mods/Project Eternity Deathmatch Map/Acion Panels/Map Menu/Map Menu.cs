﻿using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.DeathmatchMap;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class MapMenu : ActionPanelDeathmatch
    {
        #region Ressources

        protected Texture2D sprMenuText;
        protected Texture2D sprMenuHighlight;
        protected Texture2D sprCursorConfirmEndNo;
        protected Texture2D sprCursorConfirmEndYes;
        protected Texture2D sprMapMenuBackground;

        public static Texture2D sprBarLargeBackground;
        public static Texture2D sprBarLargeEN;
        public static Texture2D sprBarLargeHP;

        public static Texture2D sprLand;
        public static Texture2D sprSea;
        public static Texture2D sprSky;
        public static Texture2D sprSpace;

        public FMODSound sndConfirm;
        public FMODSound sndSelection;
        public FMODSound sndDeny;
        public FMODSound sndCancel;

        protected SpriteFont fntFinlanderFont;

        #endregion

        public MapMenu(DeathmatchMap Map)
                : base("Map Menu", Map, true)
        {
        }

        public void Load(ContentManager Content, SoundSystem FMODSystem)
        {
            sprMenuText = Content.Load<Texture2D>("Battle/Cursor/MenuText");
            sprMenuHighlight = Content.Load<Texture2D>("Battle/Cursor/MenuHighlight");
            sprCursorConfirmEndNo = Content.Load<Texture2D>("Battle/Cursor/ConfirmEndNo");
            sprCursorConfirmEndYes = Content.Load<Texture2D>("Battle/Cursor/ConfirmEndYes");
            sprMapMenuBackground = Content.Load<Texture2D>("Status Screen/Background Black");

            sprBarLargeBackground = Content.Load<Texture2D>("Battle/Bars/Large Bar");
            sprBarLargeEN = Content.Load<Texture2D>("Battle/Bars/Large Energy");
            sprBarLargeHP = Content.Load<Texture2D>("Battle/Bars/Large Health");

            sprLand = Content.Load<Texture2D>("Status Screen/Ground");
            sprSea = Content.Load<Texture2D>("Status Screen/Sea");
            sprSky = Content.Load<Texture2D>("Status Screen/Sky");
            sprSpace = Content.Load<Texture2D>("Status Screen/Space");

            sndConfirm = new FMODSound(FMODSystem, "Content/SFX/Confirm.mp3");
            sndDeny = new FMODSound(FMODSystem, "Content/SFX/Deny.mp3");
            sndSelection = new FMODSound(FMODSystem, "Content/SFX/Selection.mp3");
            sndCancel = new FMODSound(FMODSystem, "Content/SFX/Cancel.mp3");

            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");
        }

        public override void OnSelect()
        {
            ActionMenuCursor = 0;
            ListNextChoice.Clear();
            AddChoiceToCurrentPanel(new ActionPanelEndTurn(Map, sprCursorConfirmEndNo, sprCursorConfirmEndYes));
            AddChoiceToCurrentPanel(new ActionPanelObjectives(Map, fntFinlanderFont));
            AddChoiceToCurrentPanel(new ActionPanelUnitList(Map, sprBarLargeBackground, sprBarLargeEN, sprBarLargeHP, sprMapMenuBackground, sprLand, sprSea, sprSky,
                sprSpace, fntFinlanderFont));
            AddChoiceToCurrentPanel(new ActionPanelQuickSave(Map));
            AddChoiceToCurrentPanel(new ActionPanelOptions(Map));

            if (GameScreen.UseDebugMode)
            {
                AddChoiceToCurrentPanel(new ActionPanelDebugScreen(Map));
            }

            if (ActionPanelMapSwitch.GetActiveSubMaps(Map).Count > 1)
            {
                AddChoiceToCurrentPanel(new ActionPanelMapChange(Map, fntFinlanderFont));
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputUpPressed())
            {
                ActionMenuCursor -= (ActionMenuCursor > 0) ? 1 : 0;
                sndSelection.Play();
            }
            else if (InputHelper.InputDownPressed())
            {
                ActionMenuCursor += ActionMenuCursor < ListNextChoice.Count ? 1 : 0;
                sndSelection.Play();
            }
            else if (MouseHelper.MouseMoved())
            {
                int X = 490;
                int Y = 10;

                if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X < X + MinActionMenuWidth
                    && MouseHelper.MouseStateCurrent.Y >= Y + 6 && MouseHelper.MouseStateCurrent.Y < Y + ListNextChoice.Count * 22 - 12)
                {
                    int NewValue = (MouseHelper.MouseStateCurrent.Y - Y - 6) / fntFinlanderFont.LineSpacing;
                    if (NewValue != ActionMenuCursor)
                    {
                        ActionMenuCursor = NewValue;

                        sndSelection.Play();
                    }
                }
            }
            else if (InputHelper.InputConfirmPressed())
            {
                AddToPanelListAndSelect(ListNextChoice[ActionMenuCursor]);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Draw the action panel.
            int X = 490;
            int Y = 10;
            GameScreen.DrawBox(g, new Vector2(X, Y), 150, ListNextChoice.Count * 22, Color.White);
            //g.Draw(sprMenuText, new Vector2(X, Y), Color.White);

            for (int i = 0; i < ListNextChoice.Count; i++)
            {
                g.DrawString(fntFinlanderFont, ListNextChoice[i].Name, new Vector2(X + 5, Y + i * fntFinlanderFont.LineSpacing), Color.White);
            }

            //Draw the menu cursor.
            g.Draw(sprMenuHighlight, new Vector2(X + 5, Y + 6 + ActionMenuCursor * fntFinlanderFont.LineSpacing), Color.White);
        }
    }
}
