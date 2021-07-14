using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class GameEndBattleScreen : GameScreen
    {
        #region Ressources

        private Texture2D fntNumberLose;
        private Texture2D fntNumberWin;
        private Texture2D fntNumberPosition;

        private Texture2D fntNumberGray;
        private Texture2D fntNumberGreen;
        private Texture2D fntNumberRed;
        private Texture2D fntNumberWhite;
        private Texture2D fntNumberYellow;

        private Texture2D sprBackground;
        private Texture2D sprKillDeathOverlay;

        private AnimatedSprite sprInfoBarSelected;
        private AnimatedSprite sprInfoBarMission;
        private AnimatedSprite sprInfoBarBattle;
        private AnimatedSprite sprTextResult;
        private AnimatedSprite sprBestMark;

        #endregion

        private readonly FightingZone Owner;

        private double TimeToLiveInSeconds;

        public GameEndBattleScreen(FightingZone Owner)
        {
            this.Owner = Owner;

            TimeToLiveInSeconds = 5;
        }

        public override void Load()
        {
            fntNumberLose = Content.Load<Texture2D>("Triple Thunder/HUD/Menus/Numbers Lose");
            fntNumberWin = Content.Load<Texture2D>("Triple Thunder/HUD/Menus/Numbers Win");
            fntNumberPosition = Content.Load<Texture2D>("Triple Thunder/HUD/Menus/Numbers Position");

            fntNumberGray = Content.Load<Texture2D>("Triple Thunder/HUD/Menus/Numbers Gray");
            fntNumberGreen = Content.Load<Texture2D>("Triple Thunder/HUD/Menus/Numbers Green");
            fntNumberRed = Content.Load<Texture2D>("Triple Thunder/HUD/Menus/Numbers Red");
            fntNumberWhite = Content.Load<Texture2D>("Triple Thunder/HUD/Menus/Numbers White");
            fntNumberYellow = Content.Load<Texture2D>("Triple Thunder/HUD/Menus/Numbers Yellow");

            sprBackground = Content.Load<Texture2D>("Triple Thunder/HUD/Menus/End Game Battle");
            sprKillDeathOverlay = Content.Load<Texture2D>("Triple Thunder/HUD/Menus/Kill Death Overlay");

            sprInfoBarSelected = new AnimatedSprite(Content, "Triple Thunder/HUD/Menus/Info Bar Selected", new Vector2(), 0, 3, 1);
            sprInfoBarMission = new AnimatedSprite(Content, "Triple Thunder/HUD/Menus/Info Bar Mission", new Vector2(), 0, 3, 1);
            sprInfoBarBattle = new AnimatedSprite(Content, "Triple Thunder/HUD/Menus/Info Bar Battle", new Vector2(), 0, 3, 1);
            sprTextResult = new AnimatedSprite(Content, "Triple Thunder/HUD/Menus/Text Result", new Vector2(), 0, 5, 1);
            sprBestMark = new AnimatedSprite(Content, "Triple Thunder/HUD/Menus/Best Mark", new Vector2(), 0, 1, 2);
        }

        public override void Update(GameTime gameTime)
        {
            TimeToLiveInSeconds -= gameTime.ElapsedGameTime.TotalSeconds;

            if (TimeToLiveInSeconds <= 0)
            {
                RemoveScreen(Owner);
                RemoveScreen(this);

                if (Owner.OnlineClient != null)
                {
                    Owner.OnlineClient.Host.IsGameReady = false;
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float CenterX = Constants.Width / 2 - 1;
            g.Draw(sprBackground, new Vector2(0, 0), Color.White);
            sprTextResult.Draw(g, new Vector2(220, 80), Color.White);
            for (int P = 0; P < Owner.ListLocalPlayer.Count; ++P)
            {
                int IndexOfPlayer = Owner.ListAllPlayer.IndexOf(Owner.ListLocalPlayer[P]);
                sprInfoBarSelected.Draw(g, new Vector2(CenterX + 2, 180 + IndexOfPlayer * 35), Color.White);
            }

            for (int i = 0; i < Owner.ListAllPlayer.Count; ++i)
            {
                sprInfoBarBattle.Draw(g, new Vector2(CenterX, 180 + i * 35), Color.White);
                DrawNumberRightAligned(g, fntNumberPosition, i + 1, new Vector2(70, 165 + i * 35));
                DrawNumberLeftAligned(g, fntNumberGreen, i + 1, new Vector2(100, 174 + i * 35));
                TextHelper.DrawTextMiddleAligned(g, Owner.ListAllPlayer[i].Name, new Vector2(210, 172 + i * 35), Color.White);
                sprBestMark.SetFrame(0);
                sprBestMark.Draw(g, new Vector2(350, 180 + i * 35), Color.White);
                DrawNumberRightAligned(g, fntNumberRed, Owner.ListAllPlayer[i].InGameRobot.Kill, new Vector2(385, 174 + i * 35));
                sprBestMark.SetFrame(1);
                sprBestMark.Draw(g, new Vector2(425, 180 + i * 35), Color.White);
                DrawNumberRightAligned(g, fntNumberGreen, Owner.ListAllPlayer[i].InGameRobot.Death, new Vector2(460, 174 + i * 35));
                DrawNumberRightAligned(g, fntNumberWhite, 0, new Vector2(530, 174 + i * 35));
                DrawNumberRightAligned(g, fntNumberWhite, 0, new Vector2(620, 174 + i * 35));
                DrawNumberRightAligned(g, fntNumberWhite, 0, new Vector2(720, 174 + i * 35));
            }
        }
    }
}
