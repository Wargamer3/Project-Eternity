using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Magic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Units.Magic
{
    public class ActionPanelSpellSelection : BattleMapActionPanel
    {
        private const string PanelName = "Spell Selection";

        private UnitMagic ActiveUnit;
        private BattleMap Map;

        public ActionPanelSpellSelection(BattleMap Map)
            : base(PanelName, Map.ListActionMenuChoice, null, true)
        {
            this.Map = Map;
        }

        public ActionPanelSpellSelection(BattleMap Map, UnitMagic ActiveUnit)
            : base(PanelName, Map.ListActionMenuChoice, null, true)
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

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelSpellSelection(Map);
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
