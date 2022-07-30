using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class DataSaveScreen : GameScreen
    {
        private SpriteFont fntFinlanderFont;

        private readonly BattleMapPlayer Player;
        private readonly Roster PlayerRoster;
        private double TimeSinceSaveInSeconds;

        public DataSaveScreen(BattleMapPlayer Player, Roster PlayerRoster)
        {
            this.Player = Player;
            this.PlayerRoster = PlayerRoster;
            TimeSinceSaveInSeconds = 0;
        }

        public override void Load()
        {
            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");
        }

        public override void Update(GameTime gameTime)
        {
            if (TimeSinceSaveInSeconds == 0)
            {
                //Create the Part file.
                FileStream FS = new FileStream("User Data/Saves/SRWE Save.bin", FileMode.Create, FileAccess.Write);
                BinaryWriter BW = new BinaryWriter(FS);

                DataScreen.SaveProgression(BW, Player, PlayerRoster);

                FS.Close();
                BW.Close();
                TimeSinceSaveInSeconds = gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                TimeSinceSaveInSeconds += gameTime.ElapsedGameTime.TotalSeconds;

                if (KeyboardHelper.InputConfirmPressed() || KeyboardHelper.InputCancelPressed() || MouseHelper.InputLeftButtonPressed() || MouseHelper.InputRightButtonPressed() || TimeSinceSaveInSeconds > 3)
                {
                    RemoveScreen(this);
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float X = Constants.Width * 0.1f;
            float Y = Constants.Height * 0.4f;
            GameScreen.DrawBox(g, new Vector2(X, Y), (int)(Constants.Width * 0.8), (int)(Constants.Height * 0.1), Color.White);

            if (TimeSinceSaveInSeconds == 0)
            {
                g.DrawStringCentered(fntFinlanderFont, "Saving", new Vector2(Constants.Width / 2, Y + 20), Color.White);
            }
            else
            {
                g.DrawStringCentered(fntFinlanderFont, "Game Saved", new Vector2(Constants.Width / 2, Y + 20), Color.White);
            }
        }
    }
}
