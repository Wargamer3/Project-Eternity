using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelCreatureSummon : ActionPanelSorcererStreet
    {
        private const string PanelName = "CreatureSummon";

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private CreatureCard SelectedCard;

        public ActionPanelCreatureSummon(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelCreatureSummon(SorcererStreetMap Map, int ActivePlayerIndex, CreatureCard SelectedCard)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.SelectedCard = SelectedCard;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            PlaceCreature();
            Map.EndPlayerPhase();
        }

        private void PlaceCreature()
        {
            TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(ActivePlayer.GamePiece);

            ActivePlayer.ListCardInHand.Remove(SelectedCard);
            ActiveTerrain.DefendingCreature = SelectedCard;
            ActiveTerrain.PlayerOwner = ActivePlayer;
            ActivePlayer.Magic -= SelectedCard.MagicCost;

            ActivePlayer.IncreaseChainLevels(ActiveTerrain.TerrainTypeIndex);
            Map.UpdateTolls(ActivePlayer);
            Map.LayerManager.TogglePreview(true);
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            string CardType = BR.ReadString();
            string CardPath = BR.ReadString();
            foreach (Card ActiveCard in ActivePlayer.ListCardInHand)
            {
                if (ActiveCard.CardType == CardType && ActiveCard.Path == CardPath)
                {
                    SelectedCard = (CreatureCard)ActiveCard;
                    break;
                }
            }

            PlaceCreature();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendString(SelectedCard.CardType);
            BW.AppendString(SelectedCard.Path);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelCreatureSummon(Map);
        }

        //Do creature spawn animation
        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
