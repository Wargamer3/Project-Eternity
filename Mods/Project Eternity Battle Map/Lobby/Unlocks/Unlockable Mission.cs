using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class UnlockableMission : UnlockableItem
    {
        public const string MissionType = "Mission";

        public MissionInfo MissionToBuy;

        public UnlockableMission(string Path)
            : base(MissionType)
        {
            this.Path = Path;
            UnlockQuantity = 0;
        }

        public UnlockableMission(string Path, Dictionary<string, string> ActiveHeaderValues)
            : base(MissionType)
        {
            this.Path = Path;
            Load(ActiveHeaderValues);
        }

        public UnlockableMission(string Path, int UnlockQuantity, bool IsInShop)
            : base(Path, UnlockQuantity, IsInShop)
        {
        }

        public override List<string> Unlock(BattleMapPlayer ConditionsOwner)
        {
            List<string> ListUnlockMessage = new List<string>();

            if (UnlockConditions != null)
            {
                UnlockConditions.IsUnlocked = true;
            }

            ConditionsOwner.UnlockInventory.ListLockedMission.Remove(this);
            ConditionsOwner.UnlockInventory.ListUnlockedMission.Add(this);
            ConditionsOwner.Inventory.ListOwnedMission.Add(new MissionInfo(Path, UnlockQuantity));

            string[] ArrayMissionPath = Path.Split('/');
            string MissionName = ArrayMissionPath[ArrayMissionPath.Length - 1];

            if (UnlockQuantity > 0)
            {
                ListUnlockMessage.Add("You just received " + UnlockQuantity + "x " + MissionName + "!");
            }
            else
            {
                ListUnlockMessage.Add("You just received " + MissionName + "!");
            }

            if (IsInShop)
            {
                ListUnlockMessage.Add(Path + " is now available in the shop!");
            }

            return ListUnlockMessage;
        }
    }
}
