using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelMapSwitch : ActionPanelConquest
    {
        private const string PanelName = "Map Switch";

        private UnitConquest ActiveUnit;
        private MapSwitchPoint ActiveSwitchPoint;

        public ActionPanelMapSwitch(ConquestMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelMapSwitch(ConquestMap Map, UnitConquest ActiveUnit, MapSwitchPoint ActiveSwitchPoint)
            : base(PanelName, Map, false)
        {
            this.ActiveUnit = ActiveUnit;
            this.ActiveSwitchPoint = ActiveSwitchPoint;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            ChangeSquadBetweenMaps(Map, ActiveUnit, ActiveSwitchPoint);
            RemoveAllSubActionPanels();
        }

        public static void ChangeSquadBetweenMaps(ConquestMap Map, UnitConquest ActiveUnit, MapSwitchPoint ActiveSwitchPoint)
        {
            Map.ListPlayer[Map.ActivePlayerIndex].ListUnit.Remove(ActiveUnit);
            Map.ListPlayer[Map.ActivePlayerIndex].UpdateAliveStatus();
            ConquestMap SwitchMap = (ConquestMap)Map.ListSubMap.Find(x => x.GetMapType() + "/" + x.BattleMapPath == ActiveSwitchPoint.SwitchMapPath);

            if (!SwitchMap.IsInit)
            {
                SwitchMap.Init();
                SwitchMap.TogglePreview(true);
            }

            ActiveUnit.ReinitializeMembers(Map.Params.DicUnitType[ActiveUnit.UnitTypeName]);

            ActiveUnit.ReloadSkills(SwitchMap.Params.DicUnitType[ActiveUnit.UnitTypeName], SwitchMap.Params.DicRequirement, SwitchMap.Params.DicEffect, SwitchMap.Params.DicAutomaticSkillTarget, SwitchMap.Params.DicManualSkillTarget);
            SwitchMap.ListPlayer[Map.ActivePlayerIndex].ListUnit.Add(ActiveUnit);
            SwitchMap.ListPlayer[Map.ActivePlayerIndex].UpdateAliveStatus();
            ActiveUnit.SetPosition(new Vector3(ActiveSwitchPoint.OtherMapEntryPoint.X, ActiveSwitchPoint.OtherMapEntryPoint.Y, ActiveUnit.Z));

            Map.ListGameScreen.Remove(Map);
            Map.ListGameScreen.Insert(0, SwitchMap);
        }

        public static List<BattleMap> GetActiveSubMaps(ConquestMap Map)
        {
            List<BattleMap> ListActiveSubMaps = new List<BattleMap>();

            for (int i = 0; i < Map.ListSubMap.Count; i++)
            {
                ConquestMap ActiveMap = (ConquestMap)Map.ListSubMap[i];

                // Only update map with an active player on it
                if (ActiveMap.ListPlayer.Count > 0 && ActiveMap.ListPlayer[0].IsAlive)
                {
                    ListActiveSubMaps.Add(ActiveMap);
                }
            }

            return ListActiveSubMaps;
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelMapSwitch(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
