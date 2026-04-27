using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class NavMapGameManager : GameScreen
    {
        private List<MapContainer> ListIdleMap;//Loaded but not updated
        private List<MapContainer> ListUpdatedMap;//Loaded and updated but invisible

        private List<PlayerOverseer> ListLocalPlayer;

        public const int MaximumMapLoadDistance = 100;

        private InterMapMovementAlgorithm InterMapPathfinder;
        public MapInfo RootMapContainer;

        public NavMapGameManager()
        {
            ListIdleMap = new List<MapContainer>();
            ListUpdatedMap = new List<MapContainer>();

            InterMapPathfinder = new InterMapMovementAlgorithm();
            RootMapContainer = new MapInfo(new MapContainer("Root", null), null, null);

            ListLocalPlayer = new List<PlayerOverseer>();

            ListLocalPlayer.Add(new PlayerOverseer());
            ListLocalPlayer[0].ListControlledCharacter.Add(new PlayerCharacter(null, Vector3.Zero));
        }

        public override void Load()
        {
        }

        public override void Update(GameTime gameTime)
        {
            foreach (MapContainer ActiveMap in ListUpdatedMap)
            {
                ActiveMap.ActiveMap.Update(gameTime);
            }

            foreach (PlayerOverseer ActivePlayer in ListLocalPlayer)
            {
                ActivePlayer.Update(gameTime);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            foreach (PlayerOverseer ActivePlayer in ListLocalPlayer)
            {
                ActivePlayer.Draw(g);
            }
        }

        public List<AIAction> FindPath(MapInfo StartMap, Vector3 StartPosition, string Destination)
        {
            List<Vector3> ListDestination = new List<Vector3>();
            MapInfo IntersectionPoint;

            List<MapInfo> FinalPath = FindLogicRoute(StartMap, Destination, out IntersectionPoint);

            MapInfo LastMapPoint = FinalPath[FinalPath.Count - 1];
            Vector3 LastPosition = LastMapPoint.ArrayExitPoint[0];
            float LastDistance = float.MaxValue;

            for (int i = FinalPath.Count - 2; i > 0; --i)
            {
                MapInfo ActiveMapPath = FinalPath[i];

                for (int j = 0; j < ActiveMapPath.ArrayExitPoint.Length; ++j)
                {
                    float CurrentDistance = (LastPosition - ActiveMapPath.ArrayEntryPointIntoOtherMap[j]).Length();

                    if (CurrentDistance < LastDistance)
                    {
                        LastDistance = CurrentDistance;
                        LastPosition = ActiveMapPath.ArrayExitPoint[j];
                    }
                }


                ListDestination.Add(LastPosition);
            }

            return null;
        }

        private List<MapInfo> FindLogicRoute(MapInfo StartMap, string Destination, out MapInfo IntersectionPoint)
        {
            List<MapInfo> ListFoundPathToRoot = RouteToRoot(StartMap);
            List<MapInfo> ListFoundPathToDestination = InterMapPathfinder.FindPath(RootMapContainer, Destination);

            List<MapInfo> FinalPath = new List<MapInfo>();

            foreach (MapInfo ActiveNode in ListFoundPathToRoot)
            {
                FinalPath.Add(ActiveNode);

                List<MapInfo> ListAllNeighbour = FindAllNeighbour(ActiveNode);
                foreach (MapInfo ActiveNeighbour in ListAllNeighbour)
                {
                    if (ListFoundPathToDestination.Contains(ActiveNeighbour))
                    {
                        int FoundCorrespondence = ListFoundPathToDestination.IndexOf(ActiveNeighbour);

                        for (int i = FoundCorrespondence; i < ListFoundPathToDestination.Count; ++i)
                        {
                            FinalPath.Add(ListFoundPathToDestination[i]);
                        }

                        IntersectionPoint = ActiveNode;

                        return FinalPath;
                    }
                }
            }

            IntersectionPoint = RootMapContainer;
            FinalPath.AddRange(ListFoundPathToDestination);
            return FinalPath;
        }

        private List<MapInfo> FindAllNeighbour(MapInfo StartNode)
        {
            MapInfo ActiveNode = StartNode;
            List<MapInfo> ListRemainingNode = new List<MapInfo>();
            List<MapInfo> ListAllNode = new List<MapInfo>();

            ListRemainingNode.Add(ActiveNode);
            ListRemainingNode.AddRange(StartNode.DicNeighbourByName.Values);

            while (ActiveNode != null)
            {
                ListAllNode.Add(ActiveNode);

                foreach (MapInfo ActiveNeighbour in StartNode.DicNeighbourByName.Values)
                {
                    if (!ListRemainingNode.Contains(ActiveNeighbour))
                    {
                        ListRemainingNode.Add(ActiveNeighbour);
                    }
                }

                ListRemainingNode.RemoveAt(0);

                if (ListRemainingNode.Count > 0)
                {
                    ActiveNode = ListRemainingNode[0];
                }
                else
                {
                    ActiveNode = null;
                }
            }

            return ListAllNode;
        }

        public List<AIAction> FindPath(PlayerCharacter ActiveCharacter, Vector3 Start, Vector3 End)
        {
            return null;
        }

        public void AddMapContainer(MapInfo ItemToAdd)
        {
            RootMapContainer.AddNestedMap(ItemToAdd);
        }

        private List<MapInfo> RouteToRoot(MapInfo StartMap)
        {
            List<MapInfo> ListFoundPathToRoot = new List<MapInfo>();

            MapInfo StartNode = StartMap;

            while (StartNode != null)
            {
                ListFoundPathToRoot.Add(StartNode);

                StartNode = StartNode.HierarchicParent;
            }

            return ListFoundPathToRoot;
        }

        public void AccessNewMap(string MapPath)
        {
        }

        public void AwakeMap(MapInfo MapToSleep)
        {
        }

        public void SleepMap(MapInfo MapToSleep)
        {
        }

        public void UnloadMap(MapInfo MapToUnload)
        {
        }

        public void CreateNewCharacter(string CharacterName, Vector3 Position)
        {
        }
    }
}
