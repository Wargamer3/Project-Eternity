using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Magic;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Units.Magic
{
    public class ActionPanelSpellSelection : BattleMapActionPanel
    {
        protected override PlayerInput ActiveInputManager => throw new System.NotImplementedException();

        private const string PanelName = "Spell Selection";

        private UnitMagic ActiveUnit;
        private BattleMap Map;
        private PlayerInput _ActiveInputManager;

        public ActionPanelSpellSelection(BattleMap Map)
            : base(PanelName, Map.ListActionMenuChoice, true)
        {
            this.Map = Map;
        }

        public ActionPanelSpellSelection(BattleMap Map, UnitMagic ActiveUnit)
            : base(PanelName, Map.ListActionMenuChoice, true)
        {
            this.Map = Map;
            this.ActiveUnit = ActiveUnit;

            _ActiveInputManager = new KeyboardInput();
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
            if (NavigateThroughNextChoices(Map.sndSelection))
            {
            }
            else if (ConfirmNextChoices(Map.sndConfirm))
            {
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
