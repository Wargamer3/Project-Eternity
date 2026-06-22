using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core;

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
            if (ControlledCharacter.Logs.ListLog.Count == 0)
            {
                GameScreen.DrawBox(g, new Vector2(), 300, 300, Color.White);
            }

            Vector2 TextPosition = new Vector2(10, 10);

            foreach (ActionLogHolder.ActionLog ActiveLog in ControlledCharacter.Logs.ListLog)
            {
                GameScreen.DrawBox(g, new Vector2(0, TextPosition.Y - 5), 300, ActiveLog.DescriptionText.SizeBox.Y + 50, Color.White);
                TextHelper.DrawText(g, ControlledCharacter.Name, TextPosition, Color.White);
                TextHelper.DrawTextRightAligned(g, ActiveLog.TimeLogged.ToShortTimeString(), new Vector2(TextPosition.X + 280, TextPosition.Y), Color.White);
                TextPosition.Y += TextHelper.fntShadowFont.LineSpacing;
                ActiveLog.DescriptionText.Draw(g, TextPosition);
                TextPosition.Y += ActiveLog.DescriptionText.SizeBox.Y + 30;
            }
        }
    }
}
