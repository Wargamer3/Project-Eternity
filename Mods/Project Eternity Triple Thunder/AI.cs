using ProjectEternity.Core.AI;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    //Triggers: OnNewFrame, OnEnemyDetected, OnEnemyLost, OnHit, OnHited, OnMiss, OnMissed
    //Actions: SetVariable, Aim, Shoot, Jump, Walk, Jetpack

    public abstract class TripleThunderScriptHolder : AIScriptHolder
    {
    }

    public class TripleThunderScripAIContainer : AIContainer
    {
        public TripleThunderAIInfo Info { get { return _Info; } }

        private TripleThunderAIInfo _Info;

        public TripleThunderScripAIContainer(TripleThunderAIInfo Info)
        {
            _Info = Info;
        }

        public TripleThunderScripAIContainer(TripleThunderScripAIContainer Clone)
        {
            _Info = Clone._Info;
        }

        public override AIContainer Copy()
        {
            return new TripleThunderScripAIContainer(this);
        }

        public override void OnScriptLoad(AIScript NewScript)
        {
            TripleThunderScript NewTriplerThunderScript = NewScript as TripleThunderScript;
            if (NewTriplerThunderScript != null)
            {
                NewTriplerThunderScript.Info = _Info;
            }
        }
    }

    /// <summary>
    /// This class is passed by reference to a AI and shared to all its scripts.
    /// </summary>
    public class TripleThunderAIInfo
    {
        public RobotAnimation Owner;
        public Layer OwnerLayer;
        public FightingZone OwnerMap;

        public TripleThunderAIInfo(RobotAnimation Owner, Layer OwnerLayer, FightingZone OwnerMap)
        {
            this.Owner = Owner;
            this.OwnerLayer = OwnerLayer;
            this.OwnerMap = OwnerMap;
        }
    }

    public abstract class TripleThunderScript : AIScript
    {
        public TripleThunderAIInfo Info;

        public TripleThunderScript(int ScriptWidth, int ScriptHeight, string Name, string[] ArrayFollowingScript, string[] ArrayReferences)
            : base(ScriptWidth, ScriptHeight, Name, ArrayFollowingScript, ArrayReferences)
        {
        }
    }
}
