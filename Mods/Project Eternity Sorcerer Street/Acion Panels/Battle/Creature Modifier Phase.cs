using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleCreatureModifierPhase : BattleMapActionPanel
    {
        private const string PanelName = "BattleCreatureModifierPhase";

        public static string RequirementName = "Sorcerer Street Creature Phase";

        private readonly SorcererStreetMap Map;

        public ActionPanelBattleCreatureModifierPhase(SorcererStreetMap Map)
            : base(PanelName, Map.ListActionMenuChoice, false)
        {
            this.Map = Map;
        }

        public ActionPanelBattleCreatureModifierPhase(ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map)
            : base(PanelName, ListActionMenuChoice, false)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
            Init();

            ContinueBattlePhase();
        }

        private void Init()
        {
            Map.GlobalSorcererStreetBattleContext.UserCreature = Map.GlobalSorcererStreetBattleContext.Invader;
            Map.GlobalSorcererStreetBattleContext.OpponentCreature = Map.GlobalSorcererStreetBattleContext.Defender;

            Map.GlobalSorcererStreetBattleContext.UserCreature.ActivateSkill(RequirementName);

            Map.GlobalSorcererStreetBattleContext.UserCreature = Map.GlobalSorcererStreetBattleContext.Defender;
            Map.GlobalSorcererStreetBattleContext.OpponentCreature = Map.GlobalSorcererStreetBattleContext.Invader;

            Map.GlobalSorcererStreetBattleContext.UserCreature.ActivateSkill(RequirementName);

        }

        public override void DoUpdate(GameTime gameTime)
        {
        }
        
        private void ContinueBattlePhase()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelBattleEnchantModifierPhase(ListActionMenuChoice, Map));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            Init();
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleCreatureModifierPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
