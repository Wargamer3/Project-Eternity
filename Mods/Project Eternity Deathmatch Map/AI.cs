using ProjectEternity.Core.AI;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class DeathmatchAIScriptHolder : AIScriptHolder
    {
    }

    public class DeathmatchScripAIContainer : AIContainer
    {
        public DeathmatchAIInfo Info { get { return _Info; } }

        private DeathmatchAIInfo _Info;

        public DeathmatchScripAIContainer(DeathmatchAIInfo Info)
        {
            _Info = Info;
        }

        public DeathmatchScripAIContainer(DeathmatchScripAIContainer Clone)
        {
            _Info = Clone._Info;
        }

        public override AIContainer Copy()
        {
            return new DeathmatchScripAIContainer(this);
        }

        public override void OnScriptLoad(AIScript NewScript)
        {
            DeathmatchScript NewDeathmatchScript = NewScript as DeathmatchScript;
            if (NewDeathmatchScript != null)
            {
                NewDeathmatchScript.Info = _Info;
            }
        }
    }
    /// <summary>
    /// This class is passed by reference to a AI and shared to all its scripts.
    /// </summary>
    public class DeathmatchAIInfo
    {
        public DeathmatchMap Map;
        public Squad ActiveSquad;

        public DeathmatchAIInfo(DeathmatchMap Map, Squad Owner)
        {
            this.Map = Map;
            this.ActiveSquad = Owner;
        }
    }

    public abstract class DeathmatchScript : AIScript
    {
        public DeathmatchAIInfo Info;

        public DeathmatchScript(int ScriptWidth, int ScriptHeight, string Name, string[] ArrayFollowingScript, string[] ArrayReferences)
            : base(ScriptWidth, ScriptHeight, Name, ArrayFollowingScript, ArrayReferences)
        {
        }
    }
}
