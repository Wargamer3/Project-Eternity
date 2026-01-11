using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelPlayerSpellConfirm : ActionPanelSorcererStreet
    {
        private ManualSkill EnchantToAdd;
        private int ActivePlayerIndex;

        private Player ActivePlayer;
        private double AITimer;

        public ActionPanelPlayerSpellConfirm(SorcererStreetMap Map, ManualSkill EnchantToAdd, int ActivePlayerIndex)
            : base("Confirm Player Spell", Map, true)
        {
            this.EnchantToAdd = EnchantToAdd;
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[Map.ActivePlayerIndex];
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!ActivePlayer.IsPlayerControlled)
            {
                AITimer += gameTime.ElapsedGameTime.TotalSeconds;

                if (AITimer >= 1)
                {
                    EnchantHelper.ActivateOnPlayer(Map, Map.GlobalSorcererStreetBattleContext, EnchantToAdd, ActivePlayerIndex, null);
                    Map.GlobalPlayerContext.ActivePlayer.ListCardInHand.Remove(Map.GlobalPlayerContext.ActiveCard);
                    RemoveAllSubActionPanels();
                }

                return;
            }
            if (ActiveInputManager.InputConfirmPressed())
            {
                if (ActionMenuCursor == 0)
                {
                    EnchantHelper.ActivateOnPlayer(Map, Map.GlobalSorcererStreetBattleContext, EnchantToAdd, ActivePlayerIndex, null);
                    Map.GlobalPlayerContext.ActivePlayer.ListCardInHand.Remove(Map.GlobalPlayerContext.ActiveCard);
                    RemoveAllSubActionPanels();
                }
                else if (ActionMenuCursor == 1)
                {
                    RemoveAllSubActionPanels();
                }
            }
            else if (ActiveInputManager.InputUpPressed())
            {
                ++ActionMenuCursor;
                if (ActionMenuCursor > 1)
                    ActionMenuCursor = 0;

                if (Map.OnlineClient != null)
                {
                    Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
                }
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                --ActionMenuCursor;
                if (ActionMenuCursor < 0)
                    ActionMenuCursor = 1;

                if (Map.OnlineClient != null)
                {
                    Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
                }
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
            return new ActionPanelViewMap(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 720f;
            int BoxWidth = (int)(400 * Ratio);
            int BoxHeight = (int)(134 * Ratio);
            int BoxX = (Constants.Width - BoxWidth) / 2;
            int BoxY = Constants.Height - BoxHeight - (int)(74 * Ratio);
            MenuHelper.DrawBorderlessBox(g, new Vector2(BoxX, BoxY), BoxWidth, BoxHeight);

            g.DrawStringMiddleAligned(Map.fntMenuText, "Is this correct?", new Vector2(BoxX + BoxWidth / 2, (int)(BoxY + 16 * Ratio)), Color.White);
            g.DrawStringMiddleAligned(Map.fntMenuText, "Yes", new Vector2(BoxX + BoxWidth / 2, (int)(BoxY + 54 * Ratio)), Color.White);
            g.DrawStringMiddleAligned(Map.fntMenuText, "No", new Vector2(BoxX + BoxWidth / 2, (int)(BoxY + 90 * Ratio)), Color.White);
            MenuHelper.DrawFingerIcon(g, new Vector2(Constants.Width / 2 - 150, (int)(BoxY + 54 * Ratio + ActionMenuCursor * 36 * Ratio)));
        }

        public override string ToString()
        {
            return "Confirm Spell.";
        }
    }
}
