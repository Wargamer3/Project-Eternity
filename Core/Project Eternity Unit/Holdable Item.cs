using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Units
{
    public abstract class HoldableItem
    {
        public readonly string ItemType;

        public Vector3 Position;

        public UnitMap3D Item3D;

        public abstract int Width { get; }
        public abstract int Height { get; }

        protected HoldableItem(string ItemType)
        {
            this.ItemType = ItemType;
        }

        public abstract void OnPickedUp(UnitMapComponent ActiveUnit);

        public abstract void OnDroped(UnitMapComponent ActiveUnit);

        public abstract void OnTurnEnd(Squad ActiveUnit, int PlayerIndex);

        public abstract List<ActionPanel> OnUnitSelected(UnitMapComponent ActiveSquad);

        public abstract List<ActionPanel> OnUnitBeforeStop(UnitMapComponent ActiveSquad);

        public abstract void OnMovedOverBeforeStop(UnitMapComponent SelectedUnit, Vector3 PositionMovedOn, Vector3 PositionStoppedOn);

        public abstract void OnUnitStop(UnitMapComponent StoppedUnit);

        public void Draw2DOnMap(CustomSpriteBatch g, Vector3 Position, Color UnitColor)
        {
            Draw2DOnMap(g, Position, Width, Height, UnitColor);
        }

        public abstract void Draw2DOnMap(CustomSpriteBatch g, Vector3 Position, int SizeX, int SizeY, Color UnitColor);

        public abstract void DrawExtraOnMap(CustomSpriteBatch g, Vector3 PositionY, Color UnitColor);

        public abstract void DrawOverlayOnMap(CustomSpriteBatch g, Vector3 Position);

        public abstract void DrawTimeOfDayOverlayOnMap(CustomSpriteBatch g, Vector3 Position, int TimeOfDay);
    }
}
