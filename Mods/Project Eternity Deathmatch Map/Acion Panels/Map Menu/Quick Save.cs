using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelQuickSave : ActionPanelDeathmatch
    {
        private double TimeSinceSaveInSeconds;

        public ActionPanelQuickSave(DeathmatchMap Map)
            : base("Quick Save", Map, false)
        {
        }

        public override void OnSelect()
        {
            TimeSinceSaveInSeconds = 0;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (TimeSinceSaveInSeconds == 0)
            {
                Map.SaveTemporaryMap();
                TimeSinceSaveInSeconds = gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                TimeSinceSaveInSeconds += gameTime.ElapsedGameTime.TotalSeconds;

                if (ActiveInputManager.InputConfirmPressed() || ActiveInputManager.InputCancelPressed() || TimeSinceSaveInSeconds > 3)
                {
                    RemoveAllSubActionPanels();
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
                g.DrawStringCentered(Map.fntFinlanderFont, "Saving", new Vector2(Constants.Width / 2, Y + 20), Color.White);
            }
            else
            {
                g.DrawStringCentered(Map.fntFinlanderFont, "Game Saved", new Vector2(Constants.Width / 2, Y + 20), Color.White);
            }
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelQuickSave(Map);
        }
    }
}
