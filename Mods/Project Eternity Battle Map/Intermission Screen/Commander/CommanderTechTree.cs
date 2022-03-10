using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class CommanderTechTree
    {
        public Texture2D sprLogo;

        public string Name;
        public byte UnlockPointAvailable;
        public CommanderTechNode RootNode;
        public List<CommanderTechNode> ListAllNode;

        public CommanderTechTree(string Name)
        {
            this.Name = Name;

            ListAllNode = new List<CommanderTechNode>();
        }
    }
}
