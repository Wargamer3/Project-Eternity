using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class Commander
    {
        public readonly string Name;
        public TagSystem TeamTags;
        public int UnlockPointsAvailable;

        public readonly List<ManualSkill> ListPilotSpirit;
        public readonly List<BaseAutomaticSkill> ListPilotSkill;

        public CommanderTechTree TechTree;

        public Commander(string Name)
        {
            this.Name = Name;

            UnlockPointsAvailable = 0;
            ListPilotSpirit = new List<ManualSkill>();
            ListPilotSkill = new List<BaseAutomaticSkill>();

            TechTree = null;
        }

        internal void SaveProgression(BinaryWriter BW)
        {
        }

        internal void LoadProgression(BinaryReader BR)
        {
        }
    }
}
