using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public partial class UnitFactory
    {
        public class ActionPanelSpawnNinja : ActionPanelWorldMap
        {
            private const string PanelName = "Spawn Ninja From Factory";

            UnitFactory ActiveFactory;

            public ActionPanelSpawnNinja(WorldMap Map)
                : base(PanelName, Map, false)
            {
            }

            public ActionPanelSpawnNinja(WorldMap Map, UnitFactory ActiveFactory)
                : base(PanelName, Map, false)
            {
                this.ActiveFactory = ActiveFactory;
            }

            public override void OnSelect()
            {
                for (int i = 0; i < 10; i++)
                {
                    UnitMap SpawnUnit = new UnitMap(Unit.FromType(ActiveFactory.ListSpawnUnit[0].UnitTypeName, ActiveFactory.ListSpawnUnit[0].UnitStat.Name, Map.Content, Map.DicUnitType,
                        Map.DicRequirement, Map.DicEffect, Map.DicAutomaticSkillTarget));
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

            public override void DoRead(ByteReader BR)
            {
            }

            public override void DoWrite(ByteWriter BW)
            {
            }

            protected override ActionPanel Copy()
            {
                return new ActionPanelSpawnNinja(Map);
            }


            public override void Draw(CustomSpriteBatch g)
            {
            }
        }
    }
}
