using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using FMOD;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class GameOptionsGameRulesScreen : GameScreen
    {
        private FMODSound sndButtonOver;
        protected FMODSound sndButtonClick;

        private SpriteFont fntArial12;
        private SpriteFont fntOxanimumBold;
        private SpriteFont fntOxanimumLightBigger;

        private Texture2D sprFrame;
        private Texture2D sprInputSmall;

        private NumericUpDown MaxBotNumberTextbox;
        private NumericUpDown MaxSquadPerBotNumberTextbox;
        private EmptyNumericUpDown GoalScoreTextbox;
        private EmptyNumericUpDown TimeLimitTextbox;
        private EmptyNumericUpDown TurnLimitTextbox;
        private NumericUpDown MinPlayerTextbox;
        private NumericUpDown MaxPlayerTextbox;
        private NumericUpDown MaxSquadPerPlayerTextbox;

        private List<IUIElement> ListMenuButton;

        private readonly RoomInformations Room;

        int PanelX;
        int PanelY;
        int PanelWidth;
        int PanelHeight;
        int RightColumnX;
        int NumericUpDownHeight;

        public GameOptionsGameRulesScreen(RoomInformations Room)
        {
            this.Room = Room;

            float Ratio = Constants.Height / 2160f;
            PanelX = (int)(200 * Ratio);
            PanelY = (int)(324 * Ratio);
            PanelWidth = (int)(Constants.Width * 0.94);
            PanelHeight = (int)(Constants.Height * 0.22);
            RightColumnX = (int)(PanelWidth / 2);
            NumericUpDownHeight = (int)(114 * Ratio);
        }

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntOxanimumBold = Content.Load<SpriteFont>("Fonts/Oxanium Bold");
            fntOxanimumLightBigger = Content.Load<SpriteFont>("Fonts/Oxanium Light Bigger");

            sprFrame = Content.Load<Texture2D>("Menus/Lobby/Room/GameRules Frame");
            sprInputSmall = Content.Load<Texture2D>("Menus/Lobby/Interactive/Input Small");

            float Ratio = Constants.Height / 2160f;

            int DrawX = PanelX;
            int DrawY = PanelY + 100;

            DrawY += (int)(150 * Ratio);
            MinPlayerTextbox = CreateUpDown(new Vector2(DrawX + 800 * Ratio, DrawY), new Vector2(490 * Ratio, NumericUpDownHeight), null);

            DrawY += (int)(170 * Ratio);
            MaxPlayerTextbox = CreateUpDown(new Vector2(DrawX + 800 * Ratio, DrawY), new Vector2(490 * Ratio, NumericUpDownHeight), null);

            DrawY += (int)(170 * Ratio);
            MaxSquadPerPlayerTextbox = CreateUpDown(new Vector2(DrawX + 800 * Ratio, DrawY), new Vector2(490 * Ratio, NumericUpDownHeight), null);

            DrawY += 30;
            GoalScoreTextbox = new EmptyNumericUpDown(fntArial12, sprPixel, sprPixel, new Vector2(RightColumnX - 145, DrawY - 2), new Vector2(140, 20));
            TimeLimitTextbox = new EmptyNumericUpDown(fntArial12, sprPixel, sprPixel, new Vector2(PanelWidth - 145, DrawY - 2), new Vector2(140, 20));
            DrawY += 30;
            TurnLimitTextbox = new EmptyNumericUpDown(fntArial12, sprPixel, sprPixel, new Vector2(PanelWidth - 145, DrawY - 2), new Vector2(140, 20));

            DrawX = (int)(PanelX + 2000 * Ratio);
            DrawY = PanelY + 100;
            DrawY += (int)(150 * Ratio);

            MaxBotNumberTextbox = CreateUpDown(new Vector2(DrawX + 800 * Ratio, DrawY), new Vector2(490 * Ratio, NumericUpDownHeight), OnMaxBotsChanged);
            DrawY += (int)(170 * Ratio);
            MaxSquadPerBotNumberTextbox = CreateUpDown(new Vector2(DrawX + 800 * Ratio, DrawY), new Vector2(490 * Ratio, NumericUpDownHeight), OnMaxSquadPerBotChanged);

            MaxBotNumberTextbox.SetText(Room.MaxNumberOfBots.ToString());
            MaxSquadPerBotNumberTextbox.SetText(Room.MaxSquadsPerBot.ToString());

            ListMenuButton = new List<IUIElement>()
            {
                MaxBotNumberTextbox, MaxSquadPerBotNumberTextbox, MinPlayerTextbox, MaxPlayerTextbox, MaxSquadPerPlayerTextbox,// GoalScoreTextbox, TimeLimitTextbox, TurnLimitTextbox,
            };
        }

        public override void Update(GameTime gameTime)
        {
            foreach (IUIElement ActiveButton in ListMenuButton)
            {
                ActiveButton.Update(gameTime);
            }
        }

        #region Buttons

        private NumericUpDown CreateUpDown(Vector2 Position, Vector2 Size, NumericUpDown.OnConfirmDelegate Confirm)
        {
            float Ratio = Constants.Height / 2160f;

            NumericUpDown NewNumericUpDown = new NumericUpDown(fntOxanimumBold, sprPixel, sprPixel, new Vector2(Position.X + 20 * Ratio, Position.Y + 10 * Ratio), new Vector2(Size.X - 40 * Ratio, Size.Y - 20 * Ratio), Ratio, Confirm);

            TextButton ButtonUp = new TextButton(Content, "", "Menus/Lobby/Interactive/Arrow Up", new Vector2(Position.X + Size.X + 112 / 2 * Ratio, Position.Y + 56 / 2 * Ratio), 4, 1, Ratio, OnButtonOver, () => { OnConfirmUpButton(NewNumericUpDown); });
            TextButton ButtonDown = new TextButton(Content, "", "Menus/Lobby/Interactive/Arrow Down", new Vector2(Position.X + Size.X + 112 / 2 * Ratio, Position.Y + 56 / 2 * Ratio + Size.Y / 2), 4, 1, Ratio, OnButtonOver, () => { OnConfirmDownButton(NewNumericUpDown); });

            NewNumericUpDown.ButtonUp = ButtonUp;
            NewNumericUpDown.ButtonDown = ButtonDown;

            return NewNumericUpDown;
        }

        private void OnConfirmUpButton(NumericUpDown Button)
        {
            int InternalValue = int.Parse(Button.Text);
            Button.SetText(MathHelper.Min(int.MaxValue, InternalValue + 1).ToString());
            sndButtonClick.Play();
        }

        private void OnConfirmDownButton(NumericUpDown Button)
        {
            int InternalValue = int.Parse(Button.Text);
            Button.SetText(MathHelper.Max(0, InternalValue - 1).ToString());
            sndButtonClick.Play();
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        public void UpdateGameParameters()
        {
            float Ratio = Constants.Height / 2160f;

            ListMenuButton.Clear();
            ListMenuButton.Add(MaxBotNumberTextbox);
            ListMenuButton.Add(MaxSquadPerBotNumberTextbox);
            ListMenuButton.Add(MinPlayerTextbox);
            ListMenuButton.Add(MaxPlayerTextbox);
            ListMenuButton.Add(MaxSquadPerPlayerTextbox);

            int DrawY = PanelY + 255;

            Dictionary<string, List<GameModeInfo.GameModeParameter>> DicGameModeParametersByCategory = Room.GameInfo.GetDescriptionFromEnumValue();
            foreach (KeyValuePair<string, List<GameModeInfo.GameModeParameter>> ActiveCategory in DicGameModeParametersByCategory)
            {
                int DrawX = RightColumnX - 145;
                bool IsFirstColumn = true;
                foreach (GameModeInfo.GameModeParameter ActiveParameter in ActiveCategory.Value)
                {
                    if (!ActiveParameter.IsVisible)
                    {
                        continue;
                    }

                    Type ObjectType = ActiveParameter.Value.PropertyType;
                    if (typeof(int) == ObjectType)
                    {
                        NumericUpDown NewNumericUpDown = CreateUpDown(new Vector2(DrawX, DrawY), new Vector2(490 * Ratio, NumericUpDownHeight), (Sender, InputMessage) => { UpdateValue(Sender, InputMessage, ActiveParameter); });
                        NewNumericUpDown.SetText(ActiveParameter.Value.GetValue(ActiveParameter.Owner).ToString());
                        ListMenuButton.Add(NewNumericUpDown);
                    }
                    else if (typeof(bool) == ObjectType)
                    {
                        ListMenuButton.Add(new EmptyNumericUpDown(fntArial12, sprPixel, sprPixel, new Vector2(DrawX, DrawY), new Vector2(140, 20), (Sender, InputMessage) => { UpdateValue(Sender, InputMessage, ActiveParameter); }, ActiveParameter.Value.GetValue(ActiveParameter.Owner).ToString()));
                    }

                    if (IsFirstColumn)
                    {
                        ListMenuButton.Add(new UIText(Content, ActiveParameter.Name, new Vector2(PanelX + 10, DrawY), 200));
                        DrawX = PanelWidth - 145;
                    }
                    else
                    {
                        ListMenuButton.Add(new UIText(Content, ActiveParameter.Name, new Vector2(RightColumnX + 10, DrawY), 200));
                        DrawX = RightColumnX - 145;
                    }

                    IsFirstColumn = !IsFirstColumn;
                }
            }
        }

        private void OnMaxBotsChanged(TextInput SenderInput, string InputMessage)
        {
            Room.MaxNumberOfBots = int.Parse(InputMessage);

            Room.GameInfo.OnBotChanged(Room);
        }

        private void OnMaxSquadPerBotChanged(TextInput SenderInput, string InputMessage)
        {
            Room.MaxSquadsPerBot = int.Parse(InputMessage);

            foreach (BattleMapPlayer ActivePlayer in Room.ListRoomBot)
            {
                ActivePlayer.FillLoadout(Room.MaxSquadsPerBot);
            }
        }

        private void UpdateValue(TextInput SenderInput, string InputMessage, GameModeInfo.GameModeParameter Sender)
        {
            Type ObjectType = Sender.Value.PropertyType;
            if (typeof(int) == ObjectType)
            {
                Sender.Value.SetValue(Sender.Owner, int.Parse(InputMessage));
                SenderInput.SetText(Sender.Value.GetValue(Sender.Owner).ToString());
            }
        }

        #endregion

        public override void Draw(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            int DrawX = PanelX;
            int DrawY = PanelY + 100;

            Color WhiteText = Color.FromNonPremultiplied(233, 233, 233, 255);
            Color BlackText = Color.FromNonPremultiplied(65, 70, 65, 255);

            g.Draw(sprFrame, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);

            g.DrawString(fntOxanimumBold, "Game", new Vector2(DrawX + 40 * Ratio, DrawY + 24 * Ratio), WhiteText);

            DrawY += (int)(150 * Ratio);
            g.DrawString(fntOxanimumLightBigger, "Min Players", new Vector2(DrawX + 80 * Ratio, DrawY + 26 * Ratio), BlackText);
            g.Draw(sprInputSmall, new Vector2(DrawX + 800 * Ratio, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.8f);

            DrawY += (int)(170 * Ratio);
            g.DrawString(fntOxanimumLightBigger, "Max Players", new Vector2(DrawX + 80 * Ratio, DrawY + 26 * Ratio), BlackText);
            g.Draw(sprInputSmall, new Vector2(DrawX + 800 * Ratio, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.8f);

            DrawY += (int)(170 * Ratio);
            g.DrawString(fntOxanimumLightBigger, "Max Squad Per Player", new Vector2(DrawX + 80 * Ratio, DrawY + 26 * Ratio), BlackText);
            g.Draw(sprInputSmall, new Vector2(DrawX + 800 * Ratio, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.8f);

            DrawX = (int)(PanelX + 2000 * Ratio);
            DrawY = PanelY + 100;

            g.Draw(sprFrame, new Vector2(DrawX, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.9f);

            g.DrawString(fntOxanimumBold, "Bots", new Vector2(DrawX + 40 * Ratio, DrawY + 24 * Ratio), WhiteText);

            DrawY += (int)(150 * Ratio);
            g.DrawString(fntOxanimumLightBigger, "Number of Bots", new Vector2(DrawX + 80 * Ratio, DrawY + 26 * Ratio), BlackText);
            g.Draw(sprInputSmall, new Vector2(DrawX + 800 * Ratio, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.8f);
            DrawY += (int)(170 * Ratio);
            g.DrawString(fntOxanimumLightBigger, "Max Squads Per Bots", new Vector2(DrawX + 80 * Ratio, DrawY + 26 * Ratio), BlackText);
            g.Draw(sprInputSmall, new Vector2(DrawX + 800 * Ratio, DrawY), null, Color.White, 0f, Vector2.Zero, Ratio, SpriteEffects.None, 0.8f);

            foreach (IUIElement ActiveButton in ListMenuButton)
            {
                ActiveButton.Draw(g);
            }
        }

        public override string ToString()
        {
            return "Game Rules";
        }
    }
}
