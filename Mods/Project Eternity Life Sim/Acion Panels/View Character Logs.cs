using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelViewCharacterLogs : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "ViewCharacterLogs";

        private readonly PlayerCharacter ControlledCharacter;

        public ActionPanelViewCharacterLogs(PlayerOverseer Owner, PlayerCharacter ControlledCharacter, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, true)
        {
            this.ControlledCharacter = ControlledCharacter;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelViewCharacterLogs(Owner, ControlledCharacter, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            TextHelper.DrawText(g, "Cancel returns", new Vector2(40, 48), Color.LightGray);
            if (ControlledCharacter == null || ControlledCharacter.Logs == null || ControlledCharacter.Logs.ListLog.Count == 0)
            {
                TextHelper.DrawText(g, "No entries.", new Vector2(10, 76), Color.Gray);
                return;
            }

            Vector2 TextPosition = new Vector2(10, 76);
            for (int i = ControlledCharacter.Logs.ListLog.Count - 1; i >= 0; i--)
            {
                ActionLogHolder.ActionLog ActiveLog = ControlledCharacter.Logs.ListLog[i];
                int Body = ActiveLog.DescriptionText == null ? TextHelper.fntShadowFont.LineSpacing : (int)ActiveLog.DescriptionText.SizeBox.Y;
                GameScreen.DrawBox(g, new Vector2(0, TextPosition.Y - 5), 320, Body + 50, Color.White);
                TextHelper.DrawText(g, ControlledCharacter.Name, TextPosition, Color.White);
                TextHelper.DrawTextRightAligned(g, ActiveLog.TimeLogged.ToShortTimeString(), new Vector2(TextPosition.X + 300, TextPosition.Y), Color.White);
                TextPosition.Y += TextHelper.fntShadowFont.LineSpacing;
                if (ActiveLog.DescriptionText != null)
                {
                    ActiveLog.DescriptionText.Draw(g, TextPosition);
                }
                else
                {
                    TextHelper.DrawText(g, ActiveLog.TextLogged, TextPosition, Color.White);
                }

                TextPosition.Y += Body + 30;
            }
        }
    }
}
