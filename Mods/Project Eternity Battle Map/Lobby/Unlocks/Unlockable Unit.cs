using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class UnlockableUnit
    {
        public string Path;
        public ItemUnlockConditions UnlockConditions;

        public UnlockableUnit(string Path)
        {
            this.Path = Path;
        }

        public UnlockableUnit(string Path, Dictionary<string, string> ActiveHeaderValues)
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
