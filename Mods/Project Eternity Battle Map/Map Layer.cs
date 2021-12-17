using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BaseMapLayer
    {
        public int StartupDelay;
        public int ToggleDelayOn;
        public int ToggleDelayOff;

        public List<InteractiveProp> ListProp;
    }

    public interface ISubMapLayer
    {

    }
}
