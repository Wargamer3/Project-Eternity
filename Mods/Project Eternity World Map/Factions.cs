using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Units.Normal;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public enum Factions { Humans, Trees, Insects };

    public partial class WorldMap
    {
        public void LoadFactionPlayerHumans(Player Player)
        {
            List<Texture2D> ListInteractionIcons = new List<Texture2D>();
            ListInteractionIcons.Add(Content.Load<Texture2D>("Maps/World Maps/Constructions/Humans/Upgrade Icon"));

            AnimatedSprite PowerSupply = new AnimatedSprite(Content, "Maps/World Maps/Constructions/Humans/Power Supply/Sprite", Vector2.Zero);
            PowerSupply.Origin = Vector2.Zero;

            Player.ListConstructionChoice.Add( new PowerSupply(
                                                    Content.Load<Texture2D>("Maps/World Maps/Constructions/Humans/Power Supply/Icon"),
                                                    PowerSupply,
                                                    ListInteractionIcons, MaxHP: 500));

            ListInteractionIcons = new List<Texture2D>();
            List<Unit> ListSpawnUnit = new List<Unit>();
            ListSpawnUnit.Add(new UnitNormal("World Map/Humans/Ninja", Content, DicRequirement, DicEffect, DicAutomaticSkillTarget));
            ListInteractionIcons.Add(Content.Load<Texture2D>("Maps/World Maps/Constructions/Humans/Upgrade Icon"));
            ListInteractionIcons.Add(Content.Load<Texture2D>("Maps/World Maps/Constructions/Humans/Unit Factory/Build Ninja Icon"));
            ListInteractionIcons.Add(Content.Load<Texture2D>("Maps/World Maps/Constructions/Humans/Unit Factory/Waypoint Icon"));

            AnimatedSprite UnitFactory = new AnimatedSprite(Content, "Maps/World Maps/Constructions/Humans/Unit Factory/Sprite_strip2", Vector2.Zero, 2f);
            UnitFactory.Origin = Vector2.Zero;

            Player.ListConstructionChoice.Add( new UnitFactory(Content.Load<Texture2D>("Maps/World Maps/Constructions/Humans/Unit Factory/Icon"),
                                                UnitFactory,
                                                ListInteractionIcons, ListSpawnUnit,
                                                MaxHP: 2000));
        }

        public void LoadFactionPlayerTrees(Player Player)
        {
        }

        public void LoadFactionPlayerInsects(Player Player)
        {
        }
    }
}
