using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BattleEvent : MapEvent
    {
        public BattleEvent(string Name, string[] ArrayNameCondition)
            : base(140, 70, Name, new string[0], ArrayNameCondition)
        {
        }
        public override void Save(BinaryWriter BW)
        {
        }

        public override void Load(BinaryReader BR)
        {
        }

        public override bool IsValid()
        {
            return true;
        }

        public override MapScript CopyScript()
        {
            return new BattleEvent(Name, ArrayNameCondition);
        }
    }

    public abstract class BattleCondition : MapCondition
    {
        protected BattleMap Map;

        public BattleCondition(BattleMap Map, int ScriptWidth, int ScriptHeight, string Name, string[] ArrayNameTrigger, string[] ArrayNameCondition)
            : base(ScriptWidth, ScriptHeight, Name, ArrayNameTrigger, ArrayNameCondition)
        {
            this.Map = Map;
        }
    }

    public abstract class BattleTrigger : MapTrigger
    {
        protected BattleMap Map;

        public BattleTrigger(BattleMap Map, int ScriptWidth, int ScriptHeight, string Name, string[] ArrayNameTrigger, string[] ArrayNameCondition)
            : base(ScriptWidth, ScriptHeight, Name, ArrayNameTrigger, ArrayNameCondition)
        {
            this.Map = Map;
        }
    }

    public partial class BattleMap
    {
        public const string EventTypeGame = "Game";
        public const string EventTypePhase = "Phase";
        public const string EventTypeTurn = "Turn";
        public const string EventTypeUnitMoved = "Unit Moved";
        public const string EventTypeOnBattle = "On Battle";
        public const string WeaponPickedUpMap = "Weapon Picked Up Map";

        public Dictionary<string, MapEvent> DicMapEvent = new Dictionary<string, MapEvent>();
        public Dictionary<string, MapCondition> DicMapCondition = new Dictionary<string, MapCondition>();
        public Dictionary<string, MapTrigger> DicMapTrigger = new Dictionary<string, MapTrigger>();

        public void UpdateMapEvent(string EventType, int Index)
        {
            for (int E = ListMapEvent.Count - 1; E >= 0; --E)
            {
                if (ListMapEvent[E].Name != EventType || !ListMapEvent[E].IsValid())
                    continue;

                ExecuteFollowingScripts(ListMapEvent[E], Index);
            }
        }

        public void ExecuteFollowingScripts(MapScript InputScript, int Index)
        {
            for (int E = 0; E < InputScript.ArrayEvents[Index].Count; E++)
            {
                if (ListMapScript[InputScript.ArrayEvents[Index][E].LinkedScriptIndex].MapScriptType == MapScriptTypes.Condition ||
                    ListMapScript[InputScript.ArrayEvents[Index][E].LinkedScriptIndex].MapScriptType == MapScriptTypes.Trigger)
                {
                    ListMapScript[InputScript.ArrayEvents[Index][E].LinkedScriptIndex].Update(InputScript.ArrayEvents[Index][E].LinkedScriptTriggerIndex);
                }
            }
        }
    }
}
