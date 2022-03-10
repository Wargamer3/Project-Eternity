using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class CommanderTechNode
    {
        public Texture2D sprNode;

        public string Name;
        public string Description;
        public Vector2 Position;
        public Vector2 Size;

        public bool IsUnlocked;
        public bool IsDisabled;//In case the player want to nerf himself.
        public byte UnlockCost;
        public List<CommanderTechNode> ListChildren;
        public List<CommanderTechNode> ListUnlockRequirement;

        public CommanderTechNode(string Name, string Description, Vector2 Position)
        {
            this.Name = Name;
            this.Description = Description;
            this.Position = Position;
            this.Size = new Vector2(32, 32);

            ListChildren = new List<CommanderTechNode>();
            ListUnlockRequirement = new List<CommanderTechNode>();
        }

        public CommanderTechNode(string Name, string Description, Vector2 Position, Texture2D sprNode)
        {
            this.Name = Name;
            this.Description = Description;
            this.Position = Position;
            this.sprNode = sprNode;
            this.Size = new Vector2(32, 32);

            ListChildren = new List<CommanderTechNode>();
            ListUnlockRequirement = new List<CommanderTechNode>();
        }

        public CommanderTechNode(string Name, string Description, Vector2 Position, Vector2 Size)
        {
            this.Name = Name;
            this.Description = Description;
            this.Position = Position;
            this.Size = Size;

            ListChildren = new List<CommanderTechNode>();
            ListUnlockRequirement = new List<CommanderTechNode>();
        }

        public CommanderTechNode(string Name, string Description, Vector2 Position, Texture2D sprNode, Vector2 Size)
        {
            this.Name = Name;
            this.Description = Description;
            this.Position = Position;
            this.sprNode = sprNode;
            this.Size = Size;

            ListChildren = new List<CommanderTechNode>();
            ListUnlockRequirement = new List<CommanderTechNode>();
        }
    }
}
