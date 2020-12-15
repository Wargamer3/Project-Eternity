using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public class UnitFactory : Construction
    {
        public List<Unit> ListSpawnUnit;
        public Vector3 Waypoint;

        public UnitFactory(Texture2D MenuIcon, AnimatedSprite Sprite, List<Texture2D> ListInteraction, List<Unit> ListSpawnUnit, int MaxHP)
            : base("Unit factory", MenuIcon, Sprite, ListInteraction, MaxHP, Price: 100, BuildingTime: 1)
        {
            this.ListSpawnUnit = ListSpawnUnit;
            Waypoint = new Vector3(-1, -1, -1);
            MapSize = new Point(2, 2);
        }

        public override Construction Copy()
        {
            return new UnitFactory(MenuIcon, SpriteMap, ListInteraction, ListSpawnUnit, MaxHP);
        }

        public override bool IsActionAvailable(Player Player, int InteractionIndex)
        {
            switch (InteractionIndex)
            {//Upgrade
                case 0:
                    if (Player.EnergyReserve >= UpgadeLevel * 200)
                        return true;
                    return false;

                //Build Unit
                case 1:
                    if (Player.EnergyReserve >= UpgadeLevel * 200)
                        return true;
                    return false;

                //Set Waypoint
                case 2:
                    return true;
            }

            return false;
        }
        
        public override void DrawExtraMenuInformation(CustomSpriteBatch g, WorldMap Map)
        {
            if (Waypoint.X != -1)
                g.Draw(Map.sprWaypoint, new Vector2((Waypoint.X - Map.CameraPosition.X) * Map.TileSize.X, (Waypoint.Y - Map.CameraPosition.Y) * Map.TileSize.Y), Color.White);
        }

        public override ActionPanelWorldMap GetSelectionPanel(WorldMap Map, int InteractionIndex)
        {
            switch (InteractionIndex)
            {
                //Upgrade.
                case 0:
                    return new ActionPanelUpgrade(Map, this);

                #region Spawn Ninja

                case 1:
                    return new ActionPanelSpawnNinja(Map, this);

                #endregion

                case 2:
                    return new ActionPanelSetWaypoint(Map, this);
            }

            return null;
        }

        public class ActionPanelUpgrade : ActionPanelWorldMap
        {
            UnitFactory ActiveFactory;

            public ActionPanelUpgrade(WorldMap Map, UnitFactory ActiveFactory)
                : base("Upgrade Factory", Map, false)
            {
                this.ActiveFactory = ActiveFactory;
            }

            public override void OnSelect()
            {
                ++ActiveFactory.UpgadeLevel;
                RemoveFromPanelList(this);
            }

            public override void DoUpdate(GameTime gameTime)
            {
            }


            public override void Draw(CustomSpriteBatch g)
            {
            }
        }

        public class ActionPanelSpawnNinja : ActionPanelWorldMap
        {
            UnitFactory ActiveFactory;

            public ActionPanelSpawnNinja(WorldMap Map, UnitFactory ActiveFactory)
                : base("Spawn Ninja From Factory", Map, false)
            {
                this.ActiveFactory = ActiveFactory;
            }

            public override void OnSelect()
            {
                for (int i = 0; i < 10; i++)
                {
                    UnitMap SpawnUnit = new UnitMap(Unit.FromType(ActiveFactory.ListSpawnUnit[0].UnitTypeName, ActiveFactory.ListSpawnUnit[0].UnitStat.Name, Map.Content, Map.DicUnitType, Map.DicRequirement, Map.DicEffect));
                    SpawnUnit.ActiveUnit.ArrayCharacterActive = new Core.Characters.Character[0];

                    Vector3 StartPosition;

                    //No waypoint found.
                    if (ActiveFactory.Waypoint.X == -1)
                    {
                        //Unit hidden in the Construction
                        StartPosition = ActiveFactory.Position;
                    }
                    else//Waypoint found
                    {
                        StartPosition = ActiveFactory.Waypoint;
                    }
                    
                    Map.SpawnUnit(Map.ActivePlayerIndex, SpawnUnit, ActiveFactory.Position, StartPosition);
                }

                RemoveFromPanelList(this);
            }

            public override void DoUpdate(GameTime gameTime)
            {
            }


            public override void Draw(CustomSpriteBatch g)
            {
            }
        }

        public class ActionPanelSetWaypoint : ActionPanelWorldMap
        {
            UnitFactory ActiveFactory;

            public ActionPanelSetWaypoint(WorldMap Map, UnitFactory ActiveFactory)
                : base("Set Waypoint Factory", Map, false)
            {
                this.ActiveFactory = ActiveFactory;
            }

            public override void OnSelect()
            {
            }

            public override void DoUpdate(GameTime gameTime)
            {
                Map.MoveCursor();

                if (InputHelper.InputConfirmPressed())
                {
                    ActiveFactory.Waypoint = Map.CursorPosition;
                    Map.sndConfirm.Play();
                    RemoveFromPanelList(this);
                }
                else if (InputHelper.InputCancelPressed())
                {
                    Map.sndCancel.Play();
                }
            }


            public override void Draw(CustomSpriteBatch g)
            {
                g.Draw(Map.sprWaypoint, new Vector2((Map.CursorPosition.X - Map.CameraPosition.X) * Map.TileSize.X, (Map.CursorPosition.Y - Map.CameraPosition.Y) * Map.TileSize.Y), Color.White);
            }
        }
    }
}
