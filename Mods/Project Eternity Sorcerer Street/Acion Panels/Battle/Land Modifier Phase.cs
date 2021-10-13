using System.Linq;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleLandModifierPhase : BattleMapActionPanel
    {
        private const string PanelName = "BattleLandModifierPhase";

        private readonly SorcererStreetMap Map;
        private SorcererStreetUnit PlayerUnit;
        private TerrainSorcererStreet ActiveTerrain;

        private bool HasTerrainBonus;

        public ActionPanelBattleLandModifierPhase(SorcererStreetMap Map)
            : base(PanelName, Map.ListActionMenuChoice, false)
        {
            this.Map = Map;
        }

        public ActionPanelBattleLandModifierPhase(ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map, SorcererStreetUnit PlayerUnit)
            : base(PanelName, ListActionMenuChoice, false)
        {
            this.Map = Map;
            this.PlayerUnit = PlayerUnit;
            this.ActiveTerrain = Map.GetTerrain(PlayerUnit);
        }

        public override void OnSelect()
        {
            HasTerrainBonus = false;

            switch (Map.ListTerrainType[ActiveTerrain.TerrainTypeIndex])
            {
                case "Multi-Element":
                    if (Map.GlobalSorcererStreetBattleContext.Defender.ArrayAffinity.Length != 1 || Map.GlobalSorcererStreetBattleContext.Defender.ArrayAffinity[0] != CreatureCard.ElementalAffinity.Neutral)
                    {
                        HasTerrainBonus = true;
                    }
                    break;

                case "Fire":
                    if (Map.GlobalSorcererStreetBattleContext.Defender.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Fire))
                    {
                        HasTerrainBonus = true;
                    }
                    break;

                case "Water":
                    if (Map.GlobalSorcererStreetBattleContext.Defender.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Water))
                    {
                        HasTerrainBonus = true;
                    }
                    break;

                case "Earth":
                    if (Map.GlobalSorcererStreetBattleContext.Defender.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Earth))
                    {
                        HasTerrainBonus = true;
                    }
                    break;

                case "Air":
                    if (Map.GlobalSorcererStreetBattleContext.Defender.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Air))
                    {
                        HasTerrainBonus = true;
                    }
                    break;
            }

            //Skip this phase if it won't activate.
            if (!HasTerrainBonus)
            {
                ContinueBattlePhase();
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public void FinishPhase()
        {
            if (HasTerrainBonus)
            {
                Map.GlobalSorcererStreetBattleContext.DefenderFinalHP += 10 * ActiveTerrain.TerrainLevel;
            }

            ContinueBattlePhase();
        }

        private void ContinueBattlePhase()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelBattleCreatureModifierPhase(ListActionMenuChoice, Map));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            float X = BR.ReadFloat();
            float Y = BR.ReadFloat();
            int LayerIndex = BR.ReadInt32();
            Map.GetTerrain((int)X, (int)Y, LayerIndex);
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendFloat(PlayerUnit.X);
            BW.AppendFloat(PlayerUnit.Y);
            BW.AppendInt32(PlayerUnit.LayerIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleLandModifierPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
