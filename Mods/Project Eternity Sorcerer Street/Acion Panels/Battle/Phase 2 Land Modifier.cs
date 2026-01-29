using System.Linq;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleLandModifierPhase : ActionPanelBattle
    {
        private const string PanelName = "BattleLandModifierPhase";

        private bool HasDefenderTerrainBonus;
        private int InvaderSTBonus;
        private int DefenderSTBonus;

        public ActionPanelBattleLandModifierPhase(SorcererStreetMap Map)
            : base(Map, PanelName)
        {
        }

        public override void OnSelect()
        {
            HasDefenderTerrainBonus = false;

            CardAbilities Abilities = Map.GlobalSorcererStreetBattleContext.DefenderCreature.Creature.GetCurrentAbilities(Map.GlobalSorcererStreetBattleContext.EffectActivationPhase);

            if (Abilities.LandEffectLimit)
            {
                FinishPhase();
                return;
            }

            if (Abilities.LandEffectNoLimit)
            {
                HasDefenderTerrainBonus = true;
            }
            else
            {
                switch (Map.TerrainHolder.ListTerrainType[Map.GlobalSorcererStreetBattleContext.ActiveTerrain.TerrainTypeIndex])
                {
                    case TerrainSorcererStreet.MultiElement:
                        if (Abilities.ArrayElementAffinity.Length != 1 || Abilities.ArrayElementAffinity[0] != CreatureCard.ElementalAffinity.Neutral)
                        {
                            HasDefenderTerrainBonus = true;
                        }
                        break;

                    case TerrainSorcererStreet.FireElement:
                        if (Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Fire))
                        {
                            HasDefenderTerrainBonus = true;
                        }
                        break;

                    case TerrainSorcererStreet.WaterElement:
                        if (Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Water))
                        {
                            HasDefenderTerrainBonus = true;
                        }
                        break;

                    case TerrainSorcererStreet.EarthElement:
                        if (Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Earth))
                        {
                            HasDefenderTerrainBonus = true;
                        }
                        break;

                    case TerrainSorcererStreet.AirElement:
                        if (Abilities.ArrayElementAffinity.Contains(CreatureCard.ElementalAffinity.Air))
                        {
                            HasDefenderTerrainBonus = true;
                        }
                        break;
                }
            }

            InvaderSTBonus = CheckSTBonus(Map.GlobalSorcererStreetBattleContext.InvaderCreature.Owner.TeamIndex);
            DefenderSTBonus = CheckSTBonus(Map.GlobalSorcererStreetBattleContext.DefenderCreature.Owner.TeamIndex);

            FinishPhase();
        }

        private int CheckSTBonus(int PlayerTeamIndex)
        {
            Vector3 StartingPosition = Map.GlobalSorcererStreetBattleContext.ActiveTerrain.WorldPosition;
            int STBonus = 0;

            Vector2[] ArrayPositionToCheck = new Vector2[4] { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, -1), new Vector2(0, 1) };

            foreach (TerrainSorcererStreet ActiveCreature in Map.ListSummonedCreature)
            {
                if (ActiveCreature.PlayerOwner.TeamIndex != PlayerTeamIndex)
                {
                    continue;
                }

                foreach (Vector2 ActivePosition in ArrayPositionToCheck)
                {
                    Vector3 NewPosition = StartingPosition + new Vector3(ActivePosition, 0);
                    if (ActiveCreature.WorldPosition == NewPosition)
                    {
                        ++STBonus;
                    }
                }
            }

            return STBonus;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!HasFinishedUpdatingBars(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            FinishPhase();
        }

        public void FinishPhase()
        {
            if (HasDefenderTerrainBonus)
            {
                Map.GlobalSorcererStreetBattleContext.DefenderCreature.LandHP = 10 *  Map.GlobalSorcererStreetBattleContext.ActiveTerrain.LandLevel;
            }

            Map.GlobalSorcererStreetBattleContext.DefenderCreature.BonusST = DefenderSTBonus * 10;
            Map.GlobalSorcererStreetBattleContext.InvaderCreature.BonusST = InvaderSTBonus * 10;

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
