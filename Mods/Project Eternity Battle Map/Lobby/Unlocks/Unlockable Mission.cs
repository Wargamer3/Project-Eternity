using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class UnlockableMission
    {
        public string Path;
        public ItemUnlockConditions UnlockConditions;

        public UnlockableMission(string Path)
        {
            this.Path = Path;
        }

        public UnlockableMission(string Path, Dictionary<string, string> ActiveHeaderValues)
        {
            this.Path = Path;
            Load(ActiveHeaderValues);
        }

        public void Load(Dictionary<string, string> ActiveHeaderValues)
        {
            UnlockConditions = new ItemUnlockConditions(ActiveHeaderValues);
        }
    }
}
