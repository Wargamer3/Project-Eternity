using System;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public static class PlayerManager
    {
        public static string OnlinePlayerID = string.Empty;
        public static string OnlinePlayerName = string.Empty;
        public static int OnlinePlayerLevel = 0;
        //Dummy values used for menus, not be used inside a game.
        public static Dictionary<string, Unit> DicUnitType;
        public static Dictionary<string, BaseSkillRequirement> DicRequirement;
        public static Dictionary<string, BaseEffect> DicEffect;
        public static Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget;
        public static Dictionary<string, ManualSkillTarget> DicManualSkillTarget;

        public static List<OnlinePlayerBase> ListLocalPlayer = new List<OnlinePlayerBase>();
    }
}
