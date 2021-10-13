using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class SorcererStreetMapCutsceneScriptHolder : CutsceneScriptHolder
    {
        public abstract class SorcererStreetMapScript : CutsceneActionScript
        {
            protected SorcererStreetMap Map;

            protected SorcererStreetMapScript(SorcererStreetMap Map, int ScriptWidth, int ScriptHeight, string Name, string[] NameTriggers, string[] NameEvents)
                : base(ScriptWidth, ScriptHeight, Name, NameTriggers, NameEvents)
            {
                this.Map = Map;
            }
        }

        public abstract class SorcererStreetDataContainer : CutsceneDataContainer
        {
            protected SorcererStreetMap Map;

            protected SorcererStreetDataContainer(SorcererStreetMap Map, int ScriptWidth, int ScriptHeight, string Name)
                : base(ScriptWidth, ScriptHeight, Name)
            {
                this.Map = Map;
            }
        }
    }
}
