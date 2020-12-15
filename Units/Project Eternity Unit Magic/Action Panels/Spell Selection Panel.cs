using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Magic;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Units.Magic
{
    public class ActionPanelSpellSelection : BattleMapActionPanel
    {
        private UnitMagic ActiveUnit;
        private BattleMap Map;

        public ActionPanelSpellSelection(BattleMap Map, UnitMagic ActiveUnit)
            : base("Spell Selection", Map.ListActionMenuChoice, true)
        {
            this.Map = Map;
            this.ActiveUnit = ActiveUnit;
        }

        public override void OnSelect()
        {
            foreach (MagicSpell ActiveSpell in ActiveUnit.ListMagicSpell)
            {
                AddChoiceToCurrentPanel(new ActionPanelSpellEditor(ListActionMenuChoice, ActiveSpell, ActiveUnit.GlobalProjectileContext, ActiveUnit.MagicProjectileParams.SharedParams));
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            NavigateThroughNextChoices(Map.sndSelection, Map.sndConfirm);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }

        protected override void OnCancelPanel()
        {
        }
    }
}
