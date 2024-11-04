using ProjectEternity.Core.AI;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class BattleAIScriptHolder : AIScriptHolder
    {
    }

    public class BattleLightScripAIContainer : AIContainer
    {
        public BattleLightAIInfo Info { get { return _Info; } }

        private BattleLightAIInfo _Info;

        public BattleLightScripAIContainer(BattleLightAIInfo Info)
        {
            _Info = Info;
        }

        public BattleLightScripAIContainer(BattleLightScripAIContainer Clone)
        {
            _Info = Clone._Info;
        }

        public override AIContainer Copy()
        {
            return new BattleLightScripAIContainer(this);
        }

        public override void OnScriptLoad(AIScript NewScript)
        {
            BattleScript NewBattleScript = NewScript as BattleScript;
            if (NewBattleScript != null)
            {
                NewBattleScript.Info = _Info;
            }
        }
    }
    /// <summary>
    /// This class is passed by reference to a AI and shared to all its scripts.
    /// </summary>
    public class BattleLightAIInfo
    {
        public BattleMap Map;
        public BattleMapLight ActiveLight;

        public BattleLightAIInfo(BattleMap Map, BattleMapLight Owner)
        {
            this.Map = Map;
            this.ActiveLight = Owner;
        }
    }

    public abstract class BattleScript : AIScript
    {
        public BattleLightAIInfo Info;

        public BattleScript(int ScriptWidth, int ScriptHeight, string Name, string[] ArrayFollowingScript, string[] ArrayReferences)
            : base(ScriptWidth, ScriptHeight, Name, ArrayFollowingScript, ArrayReferences)
        {
        }
    }
}
