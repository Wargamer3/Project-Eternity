using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class PendingUnlockScreen : GameScreen
    {
        public DateTimeOffset UnlockTime;
        public string Message;
        public FMODSound UnlockSound;
        private DynamicText Text;

        public BattleMapPlayerUnlockInventory UnlockInventory;

        public static List<GameScreen> ListPendingUnlocks = new List<GameScreen>();
        private static DateTime LastUpdate = DateTime.UtcNow;

        int BoxWidth;

        public PendingUnlockScreen(string Message)
        {
            this.Message = Message;

            BoxWidth = Constants.Width / 2;
            UnlockTime = DateTimeOffset.Now;

            Text = new DynamicText();
            Text.TextMaxWidthInPixel = BoxWidth;
            Text.LineHeight = 20;
            Text.ListProcessor.Add(new RegularTextProcessor(Text));
            Text.ListProcessor.Add(new IconProcessor(Text));
            Text.ListProcessor.Add(new DefaultTextProcessor(Text));
            Text.SetDefaultProcessor(new DefaultTextProcessor(Text));
        }

        public static void CheckForUnlocks(GameScreen Owner)
        {
            if ((DateTime.UtcNow - LastUpdate).TotalSeconds > 1)
            {
                foreach (OnlinePlayerBase ActivePlayer in PlayerManager.ListLocalPlayer)
                {
                    ListPendingUnlocks.AddRange(ActivePlayer.UnlocksEvaluator.Evaluate(Owner.Content));
                }
            }
        }

        public static void UpdateUnlockScreens(GameScreen Owner)
        {
            if (ListPendingUnlocks.Count > 0)
            {
                Owner.PushScreen(ListPendingUnlocks[0]);
                ListPendingUnlocks.RemoveAt(0);
            }
        }

        public override void Load()
        {
            Text.Load(Content);
            Text.ParseText(Message);
        }

        public override void Update(GameTime gameTime)
        {
            Text.Update(gameTime);

            if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                RemoveScreen(this);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int BoxHeight = Constants.Height / 5;
            int BoxX = Constants.Width / 2 - BoxWidth / 2;
            int BoxY = Constants.Height / 2 - BoxHeight / 2;
            DrawBox(g, new Vector2(BoxX, BoxY), BoxWidth, BoxHeight, Color.White);
            Text.Draw(g, new Vector2(BoxX + 5, BoxY + 5));
            DrawBox(g, new Vector2(BoxX, BoxY + BoxHeight), BoxWidth, 40, Color.DarkGreen);
            DrawBox(g, new Vector2(BoxX + 25, BoxY + BoxHeight + 5), BoxWidth - 50, 30, Color.Black);
            TextHelper.DrawTextMiddleAligned(g, "2022/12/31 12:00:00", new Vector2(BoxX + BoxWidth / 2, BoxY + BoxHeight + 10), Color.Gray);
        }
    }
}
