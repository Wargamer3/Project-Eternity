using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public static class ChatHelper
    {
        public static void UpdateChat(GameTime gameTime, ChatManager Chat, TextInput ChatInput)
        {
            ChatInput.Update(gameTime);

            int T = 0;

            foreach (KeyValuePair<string, ChatManager.ChatTab> ActiveTab in Chat.DicTab)
            {
                if (ActiveTab.Key == Chat.ActiveTabID)
                {
                    ++T;
                    continue;
                }

                int X = 23 + T * 103;
                int Y = 405;

                if (MouseHelper.InputLeftButtonPressed())
                {
                    if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X < X + 103
                    && MouseHelper.MouseStateCurrent.Y >= Y && MouseHelper.MouseStateCurrent.Y < Y + 24)
                    {
                        Chat.SelectTab(ActiveTab.Key);
                    }
                }

                ++T;
            }
        }

        public static void DrawChat(CustomSpriteBatch g, SpriteFont fntArial12, ChatManager Chat, TextInput ChatInput)
        {
            ChatInput.Draw(g);

            int T = 0;

            GameScreen.DrawBox(g, new Vector2(5, 448), 550, 148, Color.White);
            GameScreen.DrawBox(g, new Vector2(5, 568), 550, 28, Color.White);

            foreach (KeyValuePair<string, ChatManager.ChatTab> ActiveTab in Chat.DicTab)
            {
                int X = 5 + T * 103;
                int Y = 425;

                if (ActiveTab.Value.Name == Chat.ActiveTabName)
                {
                    GameScreen.DrawBox(g, new Vector2(X, Y), 102, 24, Color.White);
                }
                else if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X < X + 103
                    && MouseHelper.MouseStateCurrent.Y >= Y && MouseHelper.MouseStateCurrent.Y < Y + 24)
                {
                    GameScreen.DrawBox(g, new Vector2(X, Y), 102, 24, Color.White);
                }
                else
                {
                    GameScreen.DrawBox(g, new Vector2(X, Y + 2), 102, 22, Color.White);
                }

                if (ActiveTab.Value.HasUnreadMessages)
                {
                    g.DrawString(fntArial12, ActiveTab.Value.Name + " *", new Vector2(X + 32, Y + 3), Color.White);
                }
                else
                {
                    g.DrawString(fntArial12, ActiveTab.Value.Name, new Vector2(X + 32, Y + 3), Color.White);
                }
                ++T;
            }

            int LineSpacing = fntArial12.LineSpacing;
            int LastMessageIndex = Math.Max(0, Chat.ListActiveTabHistory.Count - Chat.ActiveTabScrollUpValue / LineSpacing);
            int FirstMessageIndex = Math.Max(0, LastMessageIndex - 4);

            for (int M = FirstMessageIndex, i = 0; M < LastMessageIndex; ++M, ++i)
            {
                ChatManager.ChatMessage ActiveMessage = Chat.ListActiveTabHistory[M];
                float X = 30;
                float Y = 430 + i * fntArial12.LineSpacing - Chat.ActiveTabScrollUpValue;
                g.DrawString(fntArial12, ActiveMessage.Message, new Vector2(X, Y), Color.White);
            }
        }
    }
}