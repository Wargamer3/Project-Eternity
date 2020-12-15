using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class DeathmatchCondition : MapCondition
    {
        protected DeathmatchMap Map;

        public DeathmatchCondition(int ScriptWidth, int ScriptHeight, string Name, string[] ArrayNameTrigger, string[] ArrayNameCondition)
            : base(ScriptWidth, ScriptHeight, Name, ArrayNameTrigger, ArrayNameCondition)
        {
        }
    }
}
