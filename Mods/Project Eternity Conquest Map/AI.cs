using ProjectEternity.Core.AI;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public abstract class ConquestAIScriptHolder : AIScriptHolder
    {
    }

    public class ConquestScripAIContainer : AIContainer
    {
        public ConquestAIInfo Info { get { return _Info; } }

        private ConquestAIInfo _Info;

        public ConquestScripAIContainer(ConquestAIInfo Info)
        {
            _Info = Info;
        }

        public ConquestScripAIContainer(ConquestScripAIContainer Clone)
        {
            _Info = Clone._Info;
        }

        public override AIContainer Copy()
        {
            return new ConquestScripAIContainer(this);
        }

        public override void OnScriptLoad(AIScript NewScript)
        {
            ConquestScript NewConquestScript = NewScript as ConquestScript;
            if (NewConquestScript != null)
            {
                NewConquestScript.Info = _Info;
            }
        }
    }
    /// <summary>
    /// This class is passed by reference to a AI and shared to all its scripts.
    /// </summary>
    public class ConquestAIInfo
    {
        public ConquestMap Map;
        public UnitConquest ActiveUnit;

        public ConquestAIInfo(ConquestMap Map, UnitConquest Owner)
        {
            this.Map = Map;
            this.ActiveUnit = Owner;
        }
    }

    public abstract class ConquestScript : AIScript
    {
        public ConquestAIInfo Info;

        public ConquestScript(int ScriptWidth, int ScriptHeight, string Name, string[] ArrayFollowingScript, string[] ArrayReferences)
            : base(ScriptWidth, ScriptHeight, Name, ArrayFollowingScript, ArrayReferences)
        {
        }
    }
}
