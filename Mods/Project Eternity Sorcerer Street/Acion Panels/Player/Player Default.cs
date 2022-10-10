using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelPlayerDefault : ActionPanelSorcererStreet
    {
        private const string PanelName = "PlayerDefault";

        private int ActivePlayerIndex;

        public ActionPanelPlayerDefault(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelPlayerDefault(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed())
            {
                ConfirmStartOfTurn();
            }
        }

        public void ConfirmStartOfTurn()
        {
            RemoveFromPanelList(this);
            AddToPanelList(new ActionPanelGainPhase(Map, ActivePlayerIndex));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPlayerDefault(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float X = 0;
            float Y = 0;
            int BoxHeight = 70;

            for (int P = 0; P < Map.ListPlayer.Count; ++P)
            {
                X = 30;
                Y = Constants.Height / 20 + P * BoxHeight;

                GameScreen.DrawBox(g, new Vector2(X, Y), 130, BoxHeight, Color.White);

                X += 7;
                //Draw Player name
                g.DrawString(Map.fntArial12, Map.ListPlayer[P].Name, new Vector2(X, Y + 5), Color.White);

                Y += 25;
                //Draw Player Magic
                g.Draw(Map.sprMenuG, new Rectangle((int)X, (int)Y, 18, 18), Color.White);
                g.DrawString(Map.fntArial12, Map.ListPlayer[P].Magic.ToString(), new Vector2(X + 20, Y), Color.White);

                //Draw Player Total Magic
                g.Draw(Map.sprMenuTG, new Rectangle((int)X + 60, (int)Y, 18, 18), Color.White);
                g.DrawString(Map.fntArial12, Map.ListPlayer[P].Magic.ToString(), new Vector2(X + 80, Y), Color.White);

                Y += 20;
                //Draw Player color and it's position
                //Position if based on the number of checkpoints and then player order
                g.Draw(Map.sprPlayerBackground, new Rectangle((int)X, (int)Y, 18, 18), Map.ListPlayer[P].Color);
                g.DrawStringCentered(Map.fntArial12, Map.ListPlayer[P].Magic.ToString(), new Vector2(X + 8, Y + 9), Color.White);

                //Draw checkpoints(empty if not crossed, filled if crossed)
                g.Draw(Map.sprDirectionSouth, new Rectangle((int)X + 60, (int)Y, 18, 18), Color.White);
            }

            int BoxWidth = 110;
            BoxHeight = 55;
            X = (Constants.Width - BoxWidth) / 2;
            Y = Constants.Height * 0.8f;
            GameScreen.DrawBox(g, new Vector2(X, Y), BoxWidth, BoxHeight, Color.White);
            g.Draw(Map.sprMenuHand, new Vector2((int)X + BoxWidth - 23, (int)Y - Map.sprMenuHand.Height + BoxHeight + 8), null, Color.White, 0f, Vector2.Zero, 0.7f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);

            X += 7;
            //Draw Round + round number
            g.DrawString(Map.fntArial12, "Round " + Map.GameTurn, new Vector2(X, Y + 5), Color.White);

            Y += 25;
            //Draw Player Name's turn
            g.DrawString(Map.fntArial12, Map.ListPlayer[Map.ActivePlayerIndex].Name + "'s turn", new Vector2(X, Y + 5), Color.White);
        }
    }
}
