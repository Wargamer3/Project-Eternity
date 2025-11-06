using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleDefenderDefeatedPhase : ActionPanelSorcererStreet
    {
        public static string BattleEndRequirementName = "Sorcerer Street Battle End";
        private const string PanelName = "BattleDefenderDefeated";

        public static string RequirementName = "Sorcerer Street Battle Defender Defeated";

        public ActionPanelBattleDefenderDefeatedPhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
            if (Map.GlobalSorcererStreetBattleContext.SelfCreature.Item != null)
            {
                Map.GlobalSorcererStreetBattleContext.SelfCreature.Owner.ListCardInHand.Remove(Map.GlobalSorcererStreetBattleContext.SelfCreature.Item);
                Map.GlobalSorcererStreetBattleContext.SelfCreature.Owner.Gold -= Map.ListPlayer[Map.GlobalSorcererStreetBattleContext.SelfCreature.PlayerIndex].GetFinalCardCost(Map.GlobalSorcererStreetBattleContext.SelfCreature.Item);
            }
            if (Map.GlobalSorcererStreetBattleContext.OpponentCreature.Item != null)
            {
                Map.GlobalSorcererStreetBattleContext.OpponentCreature.Owner.ListCardInHand.Remove(Map.GlobalSorcererStreetBattleContext.OpponentCreature.Item);
                Map.GlobalSorcererStreetBattleContext.OpponentCreature.Owner.Gold -= Map.ListPlayer[Map.GlobalSorcererStreetBattleContext.OpponentCreature.PlayerIndex].GetFinalCardCost(Map.GlobalSorcererStreetBattleContext.OpponentCreature.Item);
            }

            ReplaceDeadCreature(Map);
        }

        public static void ReplaceDeadCreature(SorcererStreetMap Map)
        {
            TerrainSorcererStreet ActiveTerrain = Map.GlobalSorcererStreetBattleContext.ActiveTerrain;

            Map.GlobalSorcererStreetBattleContext.OpponentCreature.Owner.ListSummonedCreature.Remove(ActiveTerrain);
            Map.GlobalSorcererStreetBattleContext.SelfCreature.Owner.ListSummonedCreature.Add(ActiveTerrain);

            ActiveTerrain.DefendingCreature = Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature;
            ActiveTerrain.PlayerOwner = Map.GlobalSorcererStreetBattleContext.SelfCreature.Owner;

            Map.GlobalSorcererStreetBattleContext.SelfCreature.OwnerTeam.IncreaseChainLevels(ActiveTerrain.TerrainTypeIndex);
            Map.GlobalSorcererStreetBattleContext.OpponentCreature.OwnerTeam.DecreaseChainLevels(ActiveTerrain.TerrainTypeIndex);

            Map.OnCreatureDeath(Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature);
            Map.OnCreatureSummon(Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature, ActiveTerrain);

            Map.UpdateTotalMagic(Map.GlobalSorcererStreetBattleContext.SelfCreature.Owner);
            Map.UpdateTotalMagic(Map.GlobalSorcererStreetBattleContext.OpponentCreature.Owner);
        }

        public static void DestroyDeadCreatures(SorcererStreetMap Map)
        {
            if (Map.GlobalSorcererStreetBattleContext.SelfCreature.FinalHP <= 0)
            {
                TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature.GamePiece.Position);

                if (ActiveTerrain.DefendingCreature == Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature)
                {
                    Map.ListSummonedCreature.Remove(ActiveTerrain);
                    Map.GlobalSorcererStreetBattleContext.SelfCreature.Owner.ListSummonedCreature.Remove(ActiveTerrain);

                    ActiveTerrain.DefendingCreature = null;
                    ActiveTerrain.PlayerOwner = null;

                    Map.GlobalSorcererStreetBattleContext.SelfCreature.OwnerTeam.DecreaseChainLevels(ActiveTerrain.TerrainTypeIndex);

                    Map.OnCreatureDeath(Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature);

                    Map.UpdateTotalMagic(Map.GlobalSorcererStreetBattleContext.SelfCreature.Owner);
                }
            }
            if (Map.GlobalSorcererStreetBattleContext.OpponentCreature.FinalHP <= 0)
            {
                TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature.GamePiece.Position);

                if (ActiveTerrain.DefendingCreature == Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature)
                {
                    Map.ListSummonedCreature.Remove(ActiveTerrain);
                    Map.GlobalSorcererStreetBattleContext.OpponentCreature.Owner.ListSummonedCreature.Remove(ActiveTerrain);

                    ActiveTerrain.DefendingCreature = null;
                    ActiveTerrain.PlayerOwner = null;

                    Map.GlobalSorcererStreetBattleContext.OpponentCreature.OwnerTeam.DecreaseChainLevels(ActiveTerrain.TerrainTypeIndex);

                    Map.OnCreatureDeath(Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature);

                    Map.UpdateTotalMagic(Map.GlobalSorcererStreetBattleContext.OpponentCreature.Owner);
                }
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed())
            {
                RemoveFromPanelList(this);
                AddToPanelListAndSelect(new ActionPanelLandChainUpdate(Map));
            }
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
            return new ActionPanelBattleDefenderDefeatedPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int Y = Constants.Height - Constants.Height / 4;
            MenuHelper.DrawBox(g, new Vector2(Constants.Width / 4, Y), Constants.Width / 2, Constants.Height / 6);
        }
    }
}
