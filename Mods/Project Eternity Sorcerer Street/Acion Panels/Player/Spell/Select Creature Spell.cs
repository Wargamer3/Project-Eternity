using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelSelectCreatureSpell : ActionPanelViewMap
    {
        private ManualSkill EnchantToAdd;
        private bool AllowSelfPlayer;

        public ActionPanelSelectCreatureSpell(SorcererStreetMap Map, ManualSkill EnchantToAdd, bool AllowSelfPlayer)
            : base("Select Creature Spell", Map, true)
        {
            this.EnchantToAdd = EnchantToAdd;
            this.AllowSelfPlayer = AllowSelfPlayer;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            base.DoUpdate(gameTime);

            if (ActiveInputManager.InputConfirmPressed() && ActiveTerrain != null
                && ActiveTerrain.DefendingCreature != null
                && ActiveTerrain.TerrainTypeIndex != 0)
            {
                if (!AllowSelfPlayer && ActiveTerrain.PlayerOwner == Map.ListPlayer[Map.ActivePlayerIndex])
                {
                    return;
                }
                else
                {
                    AddToPanelListAndSelect(new ActionPanelPlayerSpellConfirm(Map, EnchantToAdd, Map.ListPlayer.IndexOf(ActiveTerrain.PlayerOwner)));
                }
            }
            else if (ActiveInputManager.InputCancelPressed())
            {
                RemoveFromPanelList(this);
                Map.Camera3DOverride = null;
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
            return new ActionPanelSelectCreatureSpell(Map, EnchantToAdd, AllowSelfPlayer);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);
        }

        public override string ToString()
        {
            return "Select a creature to target. When you use this command, your turn will end.";
        }
    }
}
