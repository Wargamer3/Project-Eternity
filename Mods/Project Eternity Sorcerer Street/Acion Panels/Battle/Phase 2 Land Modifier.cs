using System.Linq;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleLandModifierPhase : ActionPanelBattle
    {
        private const string PanelName = "BattleLandModifierPhase";

        private bool HasTerrainBonus;

        public ActionPanelBattleLandModifierPhase(SorcererStreetMap Map)
            : base(Map, PanelName)
        {
        }

        public override void OnSelect()
        {
            HasTerrainBonus = false;

            switch (Map.ListTerrainType[Map.GlobalSorcererStreetBattleContext.DefenderTerrain.TerrainTypeIndex])
            {
                case "Multi-Element":
                    if (Map.GlobalSorcererStreetBattleContext.Defender.Creature.ArrayAffinity.Length != 1 || Map.GlobalSorcererStreetBattleContext.Defender.Creature.ArrayAffinity[0] != CreatureCard.ElementalAffinity.Neutral)
                    {
                        HasTerrainBonus = true;
                    }
                    break;

                case "Fire":
                    if (Map.GlobalSorcererStreetBattleContext.Defender.Creature.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Fire))
                    {
                        HasTerrainBonus = true;
                    }
                    break;

                case "Water":
                    if (Map.GlobalSorcererStreetBattleContext.Defender.Creature.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Water))
                    {
                        HasTerrainBonus = true;
                    }
                    break;

                case "Earth":
                    if (Map.GlobalSorcererStreetBattleContext.Defender.Creature.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Earth))
                    {
                        HasTerrainBonus = true;
                    }
                    break;

                case "Air":
                    if (Map.GlobalSorcererStreetBattleContext.Defender.Creature.ArrayAffinity.Contains(CreatureCard.ElementalAffinity.Air))
                    {
                        HasTerrainBonus = true;
                    }
                    break;
            }

            FinishPhase();
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!CanUpdate(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            FinishPhase();
        }

        public void FinishPhase()
        {
            if (HasTerrainBonus)
            {
                Map.GlobalSorcererStreetBattleContext.Defender.FinalHP += 10 *  Map.GlobalSorcererStreetBattleContext.DefenderTerrain.LandLevel;
            }

            ContinueBattlePhase();
        }

        private void ContinueBattlePhase()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelBattleCreatureModifierPhase(Map));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ReadPlayerInfo(BR, Map);

            OnSelect();
        }

        public override void DoWrite(ByteWriter BW)
        {
            WritePlayerInfo(BW, Map);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleLandModifierPhase(Map);
        }
    }
}
