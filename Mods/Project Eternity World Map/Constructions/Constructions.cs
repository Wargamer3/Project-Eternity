using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    //Power supply, Repair station, EN regen station, Unit builders, Pilot academy, Research center, Traps, Defense turrets, Walls, Radars.
    public abstract class Construction : StandaloneUnitMapCompontent
    {
        public struct Interaction
        {
            public Texture2D Sprite;
            public int Index;//The Index is needed because not every Interaction are visible, making an external selection index unreliable.

            public Interaction(Texture2D Sprite, int Index)
            {
                this.Sprite = Sprite;
                this.Index = Index;
            }
        }

        public readonly string Name;
        public readonly int MaxHP;
        public readonly int Price;
        private int _HP;
        public int HP { get { return _HP; } }

        public readonly int BuildingTime;//Number of turns required to build the Construction.
        public int BuildingTimeRemaining;//Number of turns remaining before activation.
        public readonly Texture2D MenuIcon;
        public int UpgadeLevel;

        public List<Texture2D> ListInteraction;//List of possible interations.
        public List<Interaction> ListInteractionVisible;//List of interaction shown by the construction menu.
        public override bool IsActive { get { return HP > 0; } }

        protected Construction(string Name, Texture2D MenuIcon, AnimatedSprite Sprite, List<Texture2D> ListInteraction, int MaxHP, int Price, int BuildingTime)
        {
            this.Name = Name;
            this.MenuIcon = MenuIcon;
            this.SpriteMap = Sprite;
            this.ListInteraction = ListInteraction;
            this.MaxHP = MaxHP;
            _HP = MaxHP;
            this.Price = Price;
            this.BuildingTime = BuildingTime;
            MapSize = new Point(1, 1);
            ActionsRemaining = 1;

            ListInteractionVisible = new List<Interaction>();
        }

        public abstract Construction Copy();

        public abstract bool IsActionAvailable(Player Player, int InteractionIndex);
        
        public abstract ActionPanelWorldMap GetSelectionPanel(WorldMap Map, int InteractionIndex);

        public abstract void DrawExtraMenuInformation(CustomSpriteBatch g, WorldMap Map);

        public virtual void OpenInteractionMenu()
        {
            ListInteractionVisible.Clear();
            for (int I = 0; I < ListInteraction.Count; ++I)
                ListInteractionVisible.Add(new Interaction(ListInteraction[I], I));
        }
    }
}
