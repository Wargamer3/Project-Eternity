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

        private Player ActivePlayer;
        private double AITimer;

        public ActionPanelBattleDefenderDefeatedPhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
            ActivePlayer = Map.ListPlayer[Map.ActivePlayerIndex];

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

            KillCreature(Map, ActiveTerrain);

            Map.GlobalSorcererStreetBattleContext.SelfCreature.Owner.ListSummonedCreature.Add(ActiveTerrain);

            ActiveTerrain.DefendingCreature = Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature;
            ActiveTerrain.PlayerOwner = Map.GlobalSorcererStreetBattleContext.SelfCreature.Owner;

            Map.GlobalSorcererStreetBattleContext.SelfCreature.OwnerTeam.IncreaseChainLevels(ActiveTerrain.TerrainTypeIndex);

            Map.OnCreatureSummon(Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature, ActiveTerrain);

            Map.UpdateTotalMagic();
        }

        public static void DestroyDeadCreatures(SorcererStreetMap Map)
        {
            if (Map.GlobalSorcererStreetBattleContext.SelfCreature.FinalHP <= 0)
            {
                TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature.GamePiece.Position);

                if (ActiveTerrain.DefendingCreature == Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature)
                {
                    KillCreature(Map, ActiveTerrain);
                }
            }
            if (Map.GlobalSorcererStreetBattleContext.OpponentCreature.FinalHP <= 0)
            {
                TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature.GamePiece.Position);

                if (ActiveTerrain.DefendingCreature == Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature)
                {
                    KillCreature(Map, ActiveTerrain);
                }
            }

            Map.UpdateTotalMagic();
        }

        public static void KillCreature(SorcererStreetMap Map, TerrainSorcererStreet ActiveTerrain)
        {
            Map.ListSummonedCreature.Remove(ActiveTerrain);

            ActiveTerrain.PlayerOwner.ListSummonedCreature.Remove(ActiveTerrain);
            Map.DicTeam[ActiveTerrain.PlayerOwner.TeamIndex].DecreaseChainLevels(ActiveTerrain.TerrainTypeIndex);
            Map.OnCreatureDeath(ActiveTerrain.DefendingCreature);

            ActiveTerrain.DefendingCreature = null;
            ActiveTerrain.PlayerOwner = null;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!ActivePlayer.IsPlayerControlled)
            {
                AITimer += gameTime.ElapsedGameTime.TotalSeconds;

                if (AITimer >= 1)
                {
                    RemoveFromPanelList(this);
                    AddToPanelListAndSelect(new ActionPanelLandChainUpdate(Map));
                }
                return;
            }

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
