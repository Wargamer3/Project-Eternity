using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BaseMapLayer
    {
        public int StartupDelay;
        public int ToggleDelayOn;
        public int ToggleDelayOff;

        public List<EventPoint> ListSingleplayerSpawns;
        public List<EventPoint> ListMultiplayerSpawns;
        public List<MapSwitchPoint> ListMapSwitchPoint;
        public List<TeleportPoint> ListTeleportPoint;
        public List<InteractiveProp> ListProp;
    }

    public interface ISubMapLayer
    {

    }
}
