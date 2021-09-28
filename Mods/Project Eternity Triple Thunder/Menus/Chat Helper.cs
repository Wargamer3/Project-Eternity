using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.TripleThunderScreen
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

        public static void DrawChat(CustomSpriteBatch g, AnimatedSprite sprTabChat, SpriteFont fntArial12, ChatManager Chat, TextInput ChatInput)
        {
            ChatInput.Draw(g);

            int T = 0;

            foreach (KeyValuePair<string, ChatManager.ChatTab> ActiveTab in Chat.DicTab)
            {
                int X = 23 + T * 103;
                int Y = 405;

                if (ActiveTab.Value.Name == Chat.ActiveTabName)
                {
                    sprTabChat.SetFrame(3);
                }
                else if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X < X + 103
                    && MouseHelper.MouseStateCurrent.Y >= Y && MouseHelper.MouseStateCurrent.Y < Y + 24)
                {
                    sprTabChat.SetFrame(3);
                }
                else
                {
                    sprTabChat.SetFrame(0);
                }

                sprTabChat.Draw(g, new Vector2(X + 51, Y + 12), Color.White);
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

            int M = 0;
            foreach (string ActiveMessage in Chat.ListActiveTabHistory)
            {
                float X = 30;
                float Y = 430 + M * fntArial12.LineSpacing;
                g.DrawString(fntArial12, ActiveMessage, new Vector2(X, Y), Color.White);
                ++M;
            }
        }
    }
}