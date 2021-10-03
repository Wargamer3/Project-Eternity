using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class BattleMapCutsceneScriptHolder : CutsceneScriptHolder
    {
        public abstract class BattleMapScript : CutsceneActionScript
        {
            protected BattleMap Map;

            protected BattleMapScript(BattleMap Map, int ScriptWidth, int ScriptHeight, string Name, string[] NameTriggers, string[] NameEvents)
                : base(ScriptWidth, ScriptHeight, Name, NameTriggers, NameEvents)
            {
                this.Map = Map;
            }
        }
    }
}
