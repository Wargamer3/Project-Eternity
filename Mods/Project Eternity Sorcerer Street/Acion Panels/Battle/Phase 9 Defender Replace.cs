using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelDefendeReplace : ActionPanelSorcererStreet
    {
        private const string PanelName = "DefenderReplace";

        public ActionPanelDefendeReplace(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
                TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(Map.GlobalSorcererStreetBattleContext.InvaderPlayer.GamePiece);

                ActiveTerrain.DefendingCreature = Map.GlobalSorcererStreetBattleContext.Invader;
                ActiveTerrain.PlayerOwner = Map.GlobalSorcererStreetBattleContext.InvaderPlayer;

                Map.GlobalSorcererStreetBattleContext.InvaderPlayer.IncreaseChainLevels(ActiveTerrain.TerrainTypeIndex);
                Map.GlobalSorcererStreetBattleContext.DefenderPlayer.DecreaseChainLevels(ActiveTerrain.TerrainTypeIndex);
                Map.UpdateTolls(Map.GlobalSorcererStreetBattleContext.InvaderPlayer);
                Map.UpdateTolls(Map.GlobalSorcererStreetBattleContext.DefenderPlayer);
                RemoveFromPanelList(this);
                Map.EndPlayerPhase();
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActionPanelBattle.ReadPlayerInfo(BR, Map);
            OnSelect();
        }

        public override void DoWrite(ByteWriter BW)
        {
            ActionPanelBattle.WritePlayerInfo(BW, Map);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelDefendeReplace(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Draw creature being replaced
        }
    }
}
