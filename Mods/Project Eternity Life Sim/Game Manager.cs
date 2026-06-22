using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class NavMapGameManager : GameScreen
    {
        //SFX
        public FMODSound sndConfirm;

        public FMODSound sndSelection;
        public FMODSound sndDeny;
        public FMODSound sndCancel;

        public SpriteFont fntMenuText;

        private List<MapInfo> ListIdleMap;//Loaded but not updated
        private List<MapInfo> ListUpdatedMap;//Loaded and updated but invisible

        public List<PlayerOverseer> ListLocalPlayer;

        public const int MaximumMapLoadDistance = 100;

        private InterMapMovementAlgorithm InterMapPathfinder;
        public MapInfo RootMapContainer;

        public NavMapGameManager()
        {
            ListIdleMap = new List<MapInfo>();
            ListUpdatedMap = new List<MapInfo>();

            InterMapPathfinder = new InterMapMovementAlgorithm();
            LifeSimMap NewMap = new LifeSimMap("Grass Map", new GameModeInfo());
            NewMap.Load();
            NewMap.Init();
            NewMap.TogglePreview(true);
            RootMapContainer = new MapInfo(new MapContainer("Root", NewMap), null, null);

            ListLocalPlayer = new List<PlayerOverseer>();

            ListLocalPlayer.Add(new PlayerOverseer(this, RootMapContainer));

            ListUpdatedMap.Add(RootMapContainer);
        }

        public override void Load()
        {
            LifeSimCharacterParams.Init();
            fntMenuText = Content.Load<SpriteFont>("Fonts/Arial15");
            ActionLogHolder.InitTextParser(Content, fntMenuText, Color.White);

            sndConfirm = new FMODSound(FMODSystem, "Content/SFX/Confirm.mp3");
            sndDeny = new FMODSound(FMODSystem, "Content/SFX/Deny.mp3");
            sndSelection = new FMODSound(FMODSystem, "Content/SFX/Selection.mp3");
            sndCancel = new FMODSound(FMODSystem, "Content/SFX/Cancel.mp3");
        }

        public override void Update(GameTime gameTime)
        {
            foreach (MapInfo ActiveMap in ListUpdatedMap)
            {
                ActiveMap.MapMapContainer.ActiveMap.Update(gameTime);

                foreach (PlayerCharacter ActiveCharacter in ActiveMap.MapMapContainer.ActiveMap.ListCharacter)
                {
                    ActiveCharacter.Update(gameTime);
                }
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
