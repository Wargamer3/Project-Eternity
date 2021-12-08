using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class InteractiveProp
    {
        public readonly string PropType;//HP, EN, Ammo, weapon, pillar.
        public Vector3 Position;
        public bool[,] ArrayMapSize;
        public bool CanBlockPath;

        protected InteractiveProp(string PropType, Vector3 Position, bool[,] ArrayMapSize, bool CanBlockPath)
        {
            this.PropType = PropType;
            this.Position = Position;
            this.ArrayMapSize = ArrayMapSize;
            this.CanBlockPath = CanBlockPath;
        }

        public abstract void Load(ContentManager Content);

        public abstract void Update(GameTime gameTime);

        public abstract List<ActionPanel> OnUnitSelected(Squad SelectedUnit);

        public abstract List<ActionPanel> OnUnitBeforeStop(Squad StoppedUnit, Vector3 PositionToStopOn);

        public abstract void OnMovedOverBeforeStop(Squad SelectedUnit, Vector3 PositionMovedOn, Vector3 PositionStoppedOn);

        public abstract void OnUnitStop(Squad StoppedUnit);

        public abstract void OnBattleEnd(Squad Attacker, Squad Defender);

        public abstract void OnTurnEnd(int PlayerIndex);

        public abstract void Draw(CustomSpriteBatch g);
    }

    public class HealthCrate : InteractiveProp
    {
        private Texture2D sprCrate;

        private readonly BattleMap Map;

        private bool ForceHealOnStop;

        public HealthCrate(Vector3 Position, BattleMap Map)
            : base("HP Crate", Position, new bool[,] { { true } }, false)
        {
            this.Map = Map;

            ForceHealOnStop = false;
        }

        public override void Load(ContentManager Content)
        {
            sprCrate = Content.Load<Texture2D>("Maps/Props/HP Crate");
        }

        public void HealSquad(Squad SquadToHeal)
        {
            SquadToHeal.CurrentLeader.HealUnit(SquadToHeal.CurrentLeader.MaxHP / 2);
            Map.ListProp.Remove(this);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override List<ActionPanel> OnUnitSelected(Squad SelectedUnit)
        {
            List<ActionPanel> ListPanel = new List<ActionPanel>();

            if (!ForceHealOnStop && SelectedUnit.X == Position.X && SelectedUnit.Y == Position.Y)
            {
                ListPanel.Add(new ActionPanelPickUpHealthCrate(Map, this, SelectedUnit));
            }

            return ListPanel;
        }

        public override List<ActionPanel> OnUnitBeforeStop(Squad StoppedUnit, Vector3 PositionToStopOn)
        {
            List<ActionPanel> ListPanel = new List<ActionPanel>();

            if (!ForceHealOnStop && PositionToStopOn.X == Position.X && PositionToStopOn.Y == Position.Y)
            {
                ListPanel.Add(new ActionPanelPickUpHealthCrate(Map, this, StoppedUnit));
            }

            return ListPanel;
        }

        public override void OnMovedOverBeforeStop(Squad SelectedUnit, Vector3 PositionMovedOn, Vector3 PositionStoppedOn)
        {
        }

        public override void OnUnitStop(Squad StoppedUnit)
        {
            if (ForceHealOnStop && StoppedUnit.X == Position.X && StoppedUnit.Y == Position.Y)
            {

            }
        }

        public override void OnBattleEnd(Squad Attacker, Squad Defender)
        {
        }

        public override void OnTurnEnd(int PlayerIndex)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float PosX = (Position.X - Map.CameraPosition.X) * Map.TileSize.X;
            float PosY = (Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y;

            g.Draw(sprCrate, new Vector2(PosX, PosY), Color.White);
        }

        public class ActionPanelPickUpHealthCrate : BattleMapActionPanel
        {
            private const string PanelName = "Use Health Crate";

            private readonly BattleMap Map;
            private readonly HealthCrate Owner;
            private readonly Squad ActiveSquad;

            public ActionPanelPickUpHealthCrate(BattleMap Map, HealthCrate Owner, Squad ActiveSquad)
                : base(PanelName, Map.ListActionMenuChoice, null, false)
            {
                this.Map = Map;
                this.Owner = Owner;
                this.ActiveSquad = ActiveSquad;
            }

            public override void OnSelect()
            {
                Owner.HealSquad(ActiveSquad);
                RemoveAllSubActionPanels();
            }

            public override void DoUpdate(GameTime gameTime)
            {
                throw new NotImplementedException();
            }

            protected override void OnCancelPanel()
            {
                throw new NotImplementedException();
            }

            public override void DoRead(ByteReader BR)
            {
                throw new NotImplementedException();
            }

            public override void DoWrite(ByteWriter BW)
            {
                throw new NotImplementedException();
            }

            protected override ActionPanel Copy()
            {
                throw new NotImplementedException();
            }

            public override void Draw(CustomSpriteBatch g)
            {
            }
        }
    }
}
