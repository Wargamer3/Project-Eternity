using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using FMOD;
using ProjectEternity.GameScreens.TripleThunderScreen.Online;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class BattleModeSelect : GameScreen
    {
        #region Ressources

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private Texture2D sprBackground;
        private InteractiveButton DeathmatchButton;
        private InteractiveButton CaptureTheFlagButton;
        private InteractiveButton SurvivalButton;
        private InteractiveButton CloseButton;

        private SpriteFont fntText;

        #endregion

        private readonly RoomInformations Room;
        private readonly BattleSelect Owner;
        private readonly TripleThunderOnlineClient OnlineClient;

        public BattleModeSelect(RoomInformations Room, BattleSelect Owner, TripleThunderOnlineClient OnlineClient)
        {
            this.Room = Room;
            this.Owner = Owner;
            this.OnlineClient = OnlineClient;
        }

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntText = Content.Load<SpriteFont>("Fonts/Arial10");

            sprBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Mode Select Background");

            DeathmatchButton = new InteractiveButton(Content, "Triple Thunder/Menus/Create Room/Deathmatch Button", new Vector2(397, 249), OnButtonOver, OnDeathmatchSelected);
            DeathmatchButton.CanBeChecked = true;
            CaptureTheFlagButton = new InteractiveButton(Content, "Triple Thunder/Menus/Create Room/Capture The Flag Button", new Vector2(397, 286), OnButtonOver, OnCaptureTheFlagSelected);
            CaptureTheFlagButton.CanBeChecked = true;
            SurvivalButton = new InteractiveButton(Content, "Triple Thunder/Menus/Create Room/Survival Button", new Vector2(397, 323), OnButtonOver, OnSurvivalSelected);
            SurvivalButton.CanBeChecked = true;
            CloseButton = new InteractiveButton(Content, "Triple Thunder/Menus/Common/Close Button", new Vector2(460, 392), OnButtonOver, Close);

            if (Room.RoomSubtype == "Deathmatch")
            {
                DeathmatchButton.Select();
            }
            else if (Room.RoomSubtype == "Capture The Flag")
            {
                CaptureTheFlagButton.Select();
            }
            else if (Room.RoomSubtype == "Survival")
            {
                SurvivalButton.Select();
            }
        }

        public override void Update(GameTime gameTime)
        {
            DeathmatchButton.Update(gameTime);
            CaptureTheFlagButton.Update(gameTime);
            SurvivalButton.Update(gameTime);
            CloseButton.Update(gameTime);
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        private void OnDeathmatchSelected()
        {
            if (OnlineClient != null)
            {
                OnlineClient.Host.Send(new AskChangeRoomSubtypeScriptClient("Deathmatch"));
            }
            else
            {
                Owner.UpdateRoomSubtype("Deathmatch");
            }

            CaptureTheFlagButton.Unselect();
            SurvivalButton.Unselect();
        }

        private void OnCaptureTheFlagSelected()
        {
            if (OnlineClient != null)
            {
                OnlineClient.Host.Send(new AskChangeRoomSubtypeScriptClient("Capture The Flag"));
            }
            else
            {
                Owner.UpdateRoomSubtype("Capture The Flag");
            }

            DeathmatchButton.Unselect();
            SurvivalButton.Unselect();
        }

        private void OnSurvivalSelected()
        {
            if (OnlineClient != null)
            {
                OnlineClient.Host.Send(new AskChangeRoomSubtypeScriptClient("Survival"));
            }
            else
            {
                Owner.UpdateRoomSubtype("Survival");
            }

            DeathmatchButton.Unselect();
            CaptureTheFlagButton.Unselect();
        }

        private void Close()
        {
            RemoveScreen(this);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int DrawX = Constants.Width / 2 - sprBackground.Width / 2;
            int DrawY = Constants.Height / 2 - sprBackground.Height / 2;
            g.Draw(sprBackground, new Vector2(DrawX, DrawY), Color.White);

            DeathmatchButton.Draw(g);
            CaptureTheFlagButton.Draw(g);
            SurvivalButton.Draw(g);
            CloseButton.Draw(g);
        }
    }
}
