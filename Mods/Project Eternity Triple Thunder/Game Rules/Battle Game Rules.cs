using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class BattleGameRules : GameRules
    {
        #region Ressources

        private SpriteFont fntArial8;
        private SpriteFont fntNumberHighScore;
        private SpriteFont fntNumberRank;
        private SpriteFont fntNumberScore;
        private SpriteFont fntNumberSmall;
        private SpriteFont fntNumberTime;

        private Texture2D sprGUIMyInfo;
        private Texture2D sprGUITimeDeathmatch;
        private Texture2D sprNumberSeparator;
        private Texture2D sprGUIGameMode;

        private Texture2D sprGUIScoreHeader;
        private Texture2D sprGUIScoreTop;
        private Texture2D sprGUIScoreBottom;
        private Texture2D sprGUIScoreMyInfo;

        #endregion

        private readonly BattleRoomInformations Room;

        private bool ShowRoomSummary;
        private double GameLengthInSeconds;
        Dictionary<int, int> DicPointsPerTeam;
        private List<int> ListCurrentLivesRemainingPerTeam;

        public BattleGameRules(BattleRoomInformations Room, FightingZone Map)
             : base(Map)
        {
            this.Room = Room;

            GameLengthInSeconds = 0;
            DicPointsPerTeam = new Dictionary<int, int>();
            ListCurrentLivesRemainingPerTeam = new List<int>();
        }

        public override void Init()
        {
            GameLengthInSeconds = Room.MaxGameLengthInMinutes * 60;
        }

        public override void Load(ContentManager Content)
        {
            fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
            fntNumberHighScore = Content.Load<SpriteFont>("Triple Thunder/HUD/Number High Score");
            fntNumberRank = Content.Load<SpriteFont>("Triple Thunder/HUD/Number Rank");
            fntNumberScore = Content.Load<SpriteFont>("Triple Thunder/HUD/Number Score");
            fntNumberSmall = Content.Load<SpriteFont>("Triple Thunder/HUD/Number Small");
            fntNumberTime = Content.Load<SpriteFont>("Triple Thunder/HUD/Number Time");

            sprGUIMyInfo = Content.Load<Texture2D>("Triple Thunder/HUD/GUI My Info");
            sprGUITimeDeathmatch = Content.Load<Texture2D>("Triple Thunder/HUD/GUI Time Deathmatch");
            sprNumberSeparator = Content.Load<Texture2D>("Triple Thunder/HUD/Number Seperator");
            sprGUIGameMode = Content.Load<Texture2D>("Triple Thunder/HUD/GUI Game Mode");

            sprGUIScoreHeader = Content.Load<Texture2D>("Triple Thunder/HUD/GUI Score Header");
            sprGUIScoreTop = Content.Load<Texture2D>("Triple Thunder/HUD/GUI Score Top");
            sprGUIScoreBottom = Content.Load<Texture2D>("Triple Thunder/HUD/GUI Score Bottom");
            sprGUIScoreMyInfo = Content.Load<Texture2D>("Triple Thunder/HUD/GUI Score My Info");
        }

        public override void Update(GameTime gameTime)
        {
            ShowRoomSummary = false;
            if (KeyboardHelper.KeyHold(Microsoft.Xna.Framework.Input.Keys.Tab))
            {
                ShowRoomSummary = true;
            }

            if (Room.MaxGameLengthInMinutes > 0 && GameLengthInSeconds > 0)
            {
                GameLengthInSeconds -= gameTime.ElapsedGameTime.TotalSeconds;

                if (GameLengthInSeconds <= 0)
                {
                    EndGame();
                }
            }
        }

        public override void OnKill(RobotAnimation KillerPlayer, RobotAnimation KilledPlayer)
        {
            KillerPlayer.Kill++;
            KilledPlayer.Death++;

            if (DicPointsPerTeam.ContainsKey(KillerPlayer.Team))
            {
                DicPointsPerTeam[KillerPlayer.Team] += 1;
            }
            else
            {
                DicPointsPerTeam.Add(KillerPlayer.Team, 1);
            }

            if (Room.RoomSubtype == "Deathmatch")
            {
                if (Room.MaxKill > 0)
                {
                    if (Room.UseTeams)
                    {
                        bool MaxPointReached = DicPointsPerTeam[KillerPlayer.Team] >= Room.MaxKill;
                        if (MaxPointReached)
                        {
                            EndGame();
                        }
                    }
                    else
                    {
                        if (KillerPlayer.Kill >= Room.MaxKill)
                        {
                            EndGame();
                        }
                    }
                }
            }
        }

        public override void Draw(CustomSpriteBatch g, Player PlayerInfo)
        {
            g.Draw(sprGUIMyInfo, new Vector2(4, 4), Color.White);
            g.Draw(sprGUIGameMode, new Vector2(28, 6), new Rectangle(0, 0, sprGUIGameMode.Width / 3, sprGUIGameMode.Height), Color.White);
            g.DrawString(fntNumberRank, "1", new Vector2(95, 7), Color.White);
            g.DrawString(fntNumberSmall, "1", new Vector2(114, 11), Color.White);
            g.DrawString(fntArial8, "K:" + PlayerInfo.InGameRobot.Kill, new Vector2(134, 9), Color.White);
            g.DrawString(fntArial8, "D:" + PlayerInfo.InGameRobot.Death, new Vector2(164, 9), Color.White);

            g.Draw(sprGUITimeDeathmatch, new Vector2(Constants.Width / 2, 3), null, Color.White, 0f,
                new Vector2(sprGUITimeDeathmatch.Width  / 2, 0), 1f, SpriteEffects.None, 0f);

            int TimeRemaining = (int)GameLengthInSeconds;
            int MinutesRemaining = TimeRemaining / 60;
            int SecondsRemaining = TimeRemaining % 60;
            g.DrawStringRightAligned(fntNumberTime, MinutesRemaining.ToString().PadLeft(2, '0'), new Vector2(Constants.Width / 2 - 5, 12), Color.White);
            g.DrawString(fntNumberTime, SecondsRemaining.ToString().PadLeft(2, '0'), new Vector2(Constants.Width / 2 + 5, 12), Color.White);

            if (ShowRoomSummary)
            {
                g.Draw(sprGUIScoreTop, new Vector2(500, 150), Color.White);
                g.Draw(sprGUIScoreHeader, new Vector2(640, 155), new Rectangle(0, 0, sprGUIScoreHeader.Width / 3, sprGUIScoreHeader.Height), Color.White);
                int Max = Map.ListAllPlayer.Count;
                for (int P = 0; P < Max; ++P)
                {
                    g.Draw(sprGUIScoreMyInfo, new Vector2(500, 175 + P * 26), Color.White);
                    g.DrawString(fntArial8, Map.ListAllPlayer[P].Name, new Vector2(540, 181 + P * 26), Color.White);
                    g.DrawString(fntNumberScore, (P + 1).ToString(), new Vector2(515, 180 + P * 26), Color.White);
                    g.DrawString(fntNumberScore, Map.ListAllPlayer[P].InGameRobot.Kill.ToString(), new Vector2(665, 180 + P * 26), Color.White);
                    g.DrawString(fntNumberScore, Map.ListAllPlayer[P].InGameRobot.Death.ToString(), new Vector2(720, 180 + P * 26), Color.White);
                }
                g.Draw(sprGUIScoreBottom, new Vector2(500, 175 + Max * 26), Color.White);

                g.DrawString(fntArial8, "To Win", new Vector2(520, 182 + Max * 26), Color.White);
                g.DrawString(fntArial8, Room.MaxGameLengthInMinutes + " MIN", new Vector2(620, 182 + Max * 26), Color.White);
                g.DrawString(fntArial8, Room.MaxKill + " KILL", new Vector2(670, 182 + Max * 26), Color.White);
            }
        }
    }
}
