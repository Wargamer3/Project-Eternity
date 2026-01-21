using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelSelectPlayerSpell : ActionPanelSorcererStreet
    {
        private ManualSkill EnchantToAdd;
        private List<Player> ListPlayer;
        private bool AllowSelf;

        private Player ActivePlayer;
        private double AITimer;

        public ActionPanelSelectPlayerSpell(SorcererStreetMap Map, ManualSkill EnchantToAdd, bool AllowSelf)
            : base("Select Player Spell", Map, true)
        {
            this.EnchantToAdd = EnchantToAdd;
            this.AllowSelf = AllowSelf;
            ListPlayer = new List<Player>();
            ActivePlayer = Map.ListPlayer[Map.ActivePlayerIndex];
        }

        public override void OnSelect()
        {
            ListPlayer.Add(ActivePlayer);

            for (int P = 0; P < Map.ListPlayer.Count; ++P)
            {
                if (Map.ListPlayer[P].TeamIndex == -1)
                {
                    break;
                }

                if (!ListPlayer.Contains(Map.ListPlayer[P]))
                {
                    ListPlayer.Add(Map.ListPlayer[P]);
                }
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!ActivePlayer.IsPlayerControlled)
            {
                AITimer += gameTime.ElapsedGameTime.TotalSeconds;

                if (AITimer >= 1)
                {
                    AddToPanelListAndSelect(new ActionPanelPlayerSpellConfirm(Map, EnchantToAdd, Map.ListPlayer.IndexOf(ListPlayer[ActionMenuCursor])));
                }

                return;
            }

            if (ActiveInputManager.InputConfirmPressed())
            {
                if (ActionMenuCursor < ListPlayer.Count)
                {
                    AddToPanelListAndSelect(new ActionPanelPlayerSpellConfirm(Map, EnchantToAdd, Map.ListPlayer.IndexOf(ListPlayer[ActionMenuCursor])));
                }
                else
                {
                    RemoveFromPanelList(this);
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
            return new ActionPanelSelectPlayerSpell(Map, EnchantToAdd, AllowSelf);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 720f;
            int BoxWidth = (int)(400 * Ratio);
            int BoxHeight = (int)(67 * (ListPlayer.Count + 1) * Ratio);
            int BoxX = (Constants.Width - BoxWidth) / 2;
            int BoxY = Constants.Height - BoxHeight - (int)(74 * Ratio);
            MenuHelper.DrawBorderlessBox(g, new Vector2(BoxX, BoxY), BoxWidth, BoxHeight);

            g.DrawStringMiddleAligned(Map.fntMenuText, "Select a player", new Vector2(BoxX + BoxWidth / 2, (int)(BoxY + 16 * Ratio)), Color.White);
            for (int P = 0; P < ListPlayer.Count; ++P)
            {
                g.DrawStringMiddleAligned(Map.fntMenuText, ListPlayer[P].Name, new Vector2(BoxX + BoxWidth / 2, (int)(BoxY + 54 * Ratio + 36 * P * Ratio)), Color.White);
            }

            g.DrawStringMiddleAligned(Map.fntMenuText, "Return", new Vector2(BoxX + BoxWidth / 2, (int)(BoxY + 54 * Ratio + 36 * ListPlayer.Count * Ratio)), Color.White);

            MenuHelper.DrawFingerIcon(g, new Vector2(Constants.Width / 2 - 150, (int)(BoxY + 54 * Ratio + ActionMenuCursor * 36 * Ratio)));
        }

        public override string ToString()
        {
            return "Confirm Spell.";
        }
    }
}
