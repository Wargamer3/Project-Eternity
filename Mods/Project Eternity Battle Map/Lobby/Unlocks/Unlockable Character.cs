using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class UnlockableCharacter
    {
        public string Path;
        public int UnlockQuantity;
        public ItemUnlockConditions UnlockConditions;

        public UnlockableCharacter(string Path)
        {
            this.Path = Path;
            UnlockQuantity = 0;
        }

        public UnlockableCharacter(string Path, Dictionary<string, string> ActiveHeaderValues)
        {
            this.Path = Path;
            Load(ActiveHeaderValues);
        }

        public void Load(Dictionary<string, string> ActiveHeaderValues)
        {
            string UnlockQuantityValue;
            if (ActiveHeaderValues.TryGetValue("UnlockQuantity", out UnlockQuantityValue))
            {
                UnlockQuantity = int.Parse(UnlockQuantityValue);
            }

            UnlockConditions = new ItemUnlockConditions(ActiveHeaderValues);
        }
    }
}
