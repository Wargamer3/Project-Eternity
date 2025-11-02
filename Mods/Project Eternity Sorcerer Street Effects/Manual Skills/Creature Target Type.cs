using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetCreatureTargetType : ManualSkillActivationSorcererStreet
    {
        public static string Name = "Sorcerer Street Creature";

        public class ActionPanelSelectCreatureToEnchant : ActionPanelViewMap
        {
            private ManualSkill EnchantToAdd;

            public ActionPanelSelectCreatureToEnchant(SorcererStreetMap Map, ManualSkill EnchantToAdd)
                : base(Map)
            {
                this.EnchantToAdd = EnchantToAdd;
            }

            public override void DoUpdate(GameTime gameTime)
            {
                base.DoUpdate(gameTime);

                if (ActiveTerrain != null && ActiveTerrain.DefendingCreature != null && ActiveInputManager.InputConfirmPressed() && EnchantHelper.CanActivate(EnchantToAdd))
                {
                    Map.SorcererStreetParams.Reset();
                    Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature = ActiveTerrain.DefendingCreature;

                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelConfirmSpell(Map, EnchantToAdd));
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                base.Draw(g);
            }

            protected override ActionPanel Copy()
            {
                return new ActionPanelSelectCreatureToEnchant(Map, EnchantToAdd);
            }
        }

        public SorcererStreetCreatureTargetType()
            : this(null)
        {

        }

        public SorcererStreetCreatureTargetType(SorcererStreetBattleParams Context)
            : base(Name, true, Context)
        {
        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            return true;
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
            Params.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelSelectCreatureToEnchant(Params.Map, ActiveSkill));
        }

        public override ManualSkillTarget Copy()
        {
            return new SorcererStreetCreatureTargetType(Params);
        }

        public override void CopyMembers(ManualSkillTarget Copy)
        {
        }
    }
}
