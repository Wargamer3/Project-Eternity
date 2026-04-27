using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class SorcererStreetMapCutsceneScriptHolder : CutsceneScriptHolder
    {
        public abstract class SorcererStreetMapScript : CutsceneActionScript
        {
            protected LifeSimMap Map;

            protected SorcererStreetMapScript(LifeSimMap Map, int ScriptWidth, int ScriptHeight, string Name, string[] NameTriggers, string[] NameEvents)
                : base(ScriptWidth, ScriptHeight, Name, NameTriggers, NameEvents)
            {
                this.Map = Map;
            }
        }

        public abstract class SorcererStreetDataContainer : CutsceneDataContainer
        {
            protected LifeSimMap Map;

            protected SorcererStreetDataContainer(LifeSimMap Map, int ScriptWidth, int ScriptHeight, string Name)
                : base(ScriptWidth, ScriptHeight, Name)
            {
                this.Map = Map;
            }
        }
    }
}
