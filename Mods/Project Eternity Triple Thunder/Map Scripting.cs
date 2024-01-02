using System;
using System.IO;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class FightingZoneEvent : MapEvent
    {
        public const string EventTypeGame = "Game";
        public const string EventTypeAllEnemiesDefeated = "All Enemies Defeated";
        public const string EventOnStep = "On Step";

        public FightingZoneEvent(string Name, string[] ArrayNameCondition)
            : base(140, 70, Name, new string[0], ArrayNameCondition)
        {
        }

        public override void Load(BinaryReader BR)
        {
        }

        public override void Save(BinaryWriter BW)
        {
        }

        public override bool IsValid()
        {
            return true;
        }

        public override MapScript CopyScript()
        {
            return new FightingZoneEvent(Name, ArrayNameCondition);
        }
    }

    public abstract class FightingZoneCondition : MapCondition
    {
        protected FightingZone Map;

        public FightingZoneCondition(FightingZone Map, int ScriptWidth, int ScriptHeight, string Name, string[] ArrayNameTrigger, string[] ArrayNameCondition)
            : base(ScriptWidth, ScriptHeight, Name, ArrayNameTrigger, ArrayNameCondition)
        {
            this.Map = Map;
        }
    }

    public abstract class FightingZoneTrigger : MapTrigger
    {
        protected FightingZone Map;

        public FightingZoneTrigger(FightingZone Map, int ScriptWidth, int ScriptHeight, string Name, string[] ArrayNameTrigger, string[] ArrayNameCondition)
            : base(ScriptWidth, ScriptHeight, Name, ArrayNameTrigger, ArrayNameCondition)
        {
            this.Map = Map;
        }
    }
}
