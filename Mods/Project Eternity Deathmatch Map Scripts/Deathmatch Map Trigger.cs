using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class DeathmatchTrigger : MapTrigger
    {
        protected DeathmatchMap Map;

        public DeathmatchTrigger(int ScriptWidth, int ScriptHeight, string Name, string[] ArrayNameTrigger, string[] ArrayNameCondition)
            : base(ScriptWidth, ScriptHeight, Name, ArrayNameTrigger, ArrayNameCondition)
        {
        }
    }
}
