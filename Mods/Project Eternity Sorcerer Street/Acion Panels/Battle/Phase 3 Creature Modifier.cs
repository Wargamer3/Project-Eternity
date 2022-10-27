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
            : base(PanelName, Map.ListActionMenuChoice, null, false)
        {
            this.Map = Map;
        }

        public ActionPanelBattleCreatureModifierPhase(ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map)
            : base(PanelName, ListActionMenuChoice, null, false)
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
            Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Invader, Map.GlobalSorcererStreetBattleContext.Defender, Map.GlobalSorcererStreetBattleContext.InvaderPlayer, Map.GlobalSorcererStreetBattleContext.DefenderPlayer, RequirementName);
            Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Defender, Map.GlobalSorcererStreetBattleContext.Invader, Map.GlobalSorcererStreetBattleContext.DefenderPlayer, Map.GlobalSorcererStreetBattleContext.InvaderPlayer, RequirementName);

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
