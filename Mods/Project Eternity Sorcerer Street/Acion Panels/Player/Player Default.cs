using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelPlayerDefault : ActionPanelSorcererStreet
    {
        private readonly Player ActivePlayer;

        public ActionPanelPlayerDefault(SorcererStreetMap Map, Player ActivePlayer)
            : base("Player Default", Map, false)
        {
            this.ActivePlayer = ActivePlayer;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed())
            {
                RemoveFromPanelList(this);
                AddToPanelList(new ActionPanelGainPhase(Map, ActivePlayer));
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float X = 0;
            float Y = 0;

            for (int P = 0; P < Map.ListPlayer.Count; ++P)
            {
                X = 30;
                int BoxHeight = 70;
                Y = Constants.Height / 20 + P * BoxHeight;

                GameScreen.DrawBox(g, new Vector2(X, Y), 130, BoxHeight, Color.White);

                X += 7;
                //Draw Player name
                g.DrawString(Map.fntArial12, Map.ListPlayer[P].Name, new Vector2(X, Y + 5), Color.White);

                Y += 25;
                //Draw Player Magic
                g.Draw(GameScreen.sprPixel, new Rectangle((int)X, (int)Y, 18, 18), Color.White);
                g.DrawString(Map.fntArial12, Map.ListPlayer[P].Magic.ToString(), new Vector2(X + 20, Y), Color.White);

                //Draw Player Total Magic
                g.Draw(GameScreen.sprPixel, new Rectangle((int)X + 60, (int)Y, 18, 18), Color.White);
                g.DrawString(Map.fntArial12, Map.ListPlayer[P].Magic.ToString(), new Vector2(X + 80, Y), Color.White);

                Y += 20;
                //Draw Player color and it's position
                //Position if based on the number of checkpoints and then player order
                g.Draw(GameScreen.sprPixel, new Rectangle((int)X, (int)Y, 18, 18), Map.ListPlayer[P].Color);
                g.DrawString(Map.fntArial12, Map.ListPlayer[P].Magic.ToString(), new Vector2(X, Y), Color.White);

                //Draw checkpoints(empty if not crossed, filled if crossed)
                g.Draw(GameScreen.sprPixel, new Rectangle((int)X + 60, (int)Y, 18, 18), Color.White);
            }

            int BoxWidth = 100;
            X = (Constants.Width - BoxWidth) / 2;
            Y = Constants.Height * 0.8f;
            GameScreen.DrawBox(g, new Vector2(X, Y), BoxWidth, 55, Color.White);

            X += 7;
            //Draw Round + round number
            g.DrawString(Map.fntArial12, "Round " + Map.GameTurn, new Vector2(X, Y + 5), Color.White);

            Y += 25;
            //Draw Player Name's turn
            g.DrawString(Map.fntArial12, Map.ListPlayer[Map.ActivePlayerIndex].Name + "'s turn", new Vector2(X, Y + 5), Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle((int)X + BoxWidth - 29, (int)Y + 8, 18, 18), Color.White);
        }
    }
}
