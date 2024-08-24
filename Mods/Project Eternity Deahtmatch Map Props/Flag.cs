using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class Flag : HoldableItem
    {
        public class DropFlagActionPanel : ActionPanelDeathmatch
        {
            private readonly UnitMapComponent Owner;

            public DropFlagActionPanel(DeathmatchMap Map, UnitMapComponent Owner)
                : base("Drop Flag", Map, false)
            {
                this.Owner = Owner;
            }

            public override void OnSelect()
            {
                Map.LayerManager.ListLayer[(int)Owner.Position.Z].ListHoldableItem.Add(Owner.ItemHeld);
                Owner.DropItem();
                RemoveAllSubActionPanels();
            }

            public override void DoUpdate(GameTime gameTime)
            {
            }

            public override void DoRead(ByteReader BR)
            {
            }

            public override void DoWrite(ByteWriter BW)
            {
            }

            public override void Draw(CustomSpriteBatch g)
            {
            }

            protected override ActionPanel Copy()
            {
                throw new NotImplementedException();
            }

            protected override void OnCancelPanel()
            {
            }
        }

        private readonly DeathmatchMap Map;
        public readonly FlagSpawner Owner;

        private bool IsDropped = false;
        private int TurnUsed;
        private int TurnRemaining;
        private const int TurnsBeforeReturn = 2;

        public override int Width => sprItem.Width;

        public override int Height => sprItem.Height;

        public Flag(DeathmatchMap Map, FlagSpawner Owner, Texture2D sprFlag, UnitMap3D Item3D)
            : base("Flag")
        {
            this.Map = Map;
            this.Owner = Owner;
            this.sprItem = sprFlag;
            this.Item3D = Item3D;
        }

        public override void OnPickedUp(UnitMapComponent ActiveUnit)
        {
            IsDropped = false;
            Owner.ReturnFlag();
        }

        public override void OnDroped(UnitMapComponent ActiveUnit)
        {
            TurnUsed = Map.ActivePlayerIndex;
            TurnRemaining = TurnsBeforeReturn;
            IsDropped = true;
        }

        public override List<ActionPanel> OnUnitBeforeStop(UnitMapComponent ActiveSquad)
        {
            List<ActionPanel> ListPanel = new List<ActionPanel>();

            ListPanel.Add(new DropFlagActionPanel(Map, ActiveSquad));

            return ListPanel;
        }

        public override List<ActionPanel> OnUnitSelected(UnitMapComponent ActiveSquad)
        {
            List<ActionPanel> ListPanel = new List<ActionPanel>();

            ListPanel.Add(new DropFlagActionPanel(Map, ActiveSquad));

            return ListPanel;
        }

        public override void OnMovedOverBeforeStop(UnitMapComponent SelectedSquad, Vector3 PositionMovedOn, Vector3 PositionStoppedOn)
        {
            if (IsDropped && PositionMovedOn.X == Position.X && PositionMovedOn.Y == Position.Y)
            {
                if (Map.ListPlayer[Map.ActivePlayerIndex].TeamIndex == Owner.Team)
                {
                    Owner.ReturnFlag();
                }
                else
                {
                    SelectedSquad.PickupItem(this);
                }
                Map.LayerManager.ListLayer[(int)Position.Z].ListHoldableItem.Remove(this);
            }
        }

        public override void OnUnitStop(UnitMapComponent StoppedSquad)
        {
            if (IsDropped && StoppedSquad.X == Position.X && StoppedSquad.Y == Position.Y)
            {
                if (Map.ListPlayer[Map.ActivePlayerIndex].TeamIndex == Owner.Team)
                {
                    Owner.ReturnFlag();
                }
                else
                {
                    StoppedSquad.PickupItem(this);
                }
                Map.LayerManager.ListLayer[(int)Position.Z].ListHoldableItem.Remove(this);
            }
        }

        public override void OnTurnEnd(Squad ActiveSquad, int PlayerIndex)
        {
            if (ActiveSquad == null && IsDropped)
            {
                if (IsDropped && TurnUsed == PlayerIndex && --TurnRemaining <= 0)
                {
                    Owner.ReturnFlag();
                    Map.LayerManager.ListLayer[(int)Position.Z].ListHoldableItem.Remove(this);
                    IsDropped = false;
                }
            }
        }

        public override void Draw2DOnMap(CustomSpriteBatch g, Vector3 Position, int SizeX, int SizeY, Color UnitColor)
        {
            throw new NotImplementedException();
        }

        public override void DrawExtraOnMap(CustomSpriteBatch g, Vector3 PositionY, Color UnitColor)
        {
            throw new NotImplementedException();
        }

        public override void DrawOverlayOnMap(CustomSpriteBatch g, Vector3 Position)
        {
            throw new NotImplementedException();
        }

        public override void DrawTimeOfDayOverlayOnMap(CustomSpriteBatch g, Vector3 Position, int TimeOfDay)
        {
            throw new NotImplementedException();
        }
    }
}
