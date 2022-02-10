using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class MovementAnimations
    {
        public Dictionary<UnitMapComponent, Vector3> DicMovingMapUnitByPosition;
        public Dictionary<UnitMapComponent, List<Vector3>> DicMovingMapUnitByNextPosition;

        public MovementAnimations()
        {
            DicMovingMapUnitByPosition = new Dictionary<UnitMapComponent, Vector3>();
            DicMovingMapUnitByNextPosition = new Dictionary<UnitMapComponent, List<Vector3>>();
        }

        public void Add(UnitMapComponent MovingMapUnit, Vector3 StartPosition, Vector3 FinalPosition)
        {
            DicMovingMapUnitByPosition.Add(MovingMapUnit, StartPosition);
            DicMovingMapUnitByNextPosition.Add(MovingMapUnit, new List<Vector3>() { FinalPosition });
        }

        public void Add(UnitMapComponent MovingMapUnit, Vector3 StartPosition, List<Vector3> ListNextPosition)
        {
            DicMovingMapUnitByPosition.Add(MovingMapUnit, StartPosition);
            DicMovingMapUnitByNextPosition.Add(MovingMapUnit, new List<Vector3>(ListNextPosition));
        }

        public bool Contains(UnitMapComponent MovingMapUnit)
        {
            return DicMovingMapUnitByPosition.ContainsKey(MovingMapUnit);
        }

        public Vector3 GetPosition(UnitMapComponent MovingMapUnit)
        {
            return DicMovingMapUnitByPosition[MovingMapUnit];
        }

        public void MoveSquad(BattleMap Map)
        {
            const float MovementSpeed = 0.2f;
            List<UnitMapComponent> ListRemovedSquad = new List<UnitMapComponent>();

            foreach(KeyValuePair<UnitMapComponent, List<Vector3>> ActiveUnitMap in DicMovingMapUnitByNextPosition)
            {
                Vector3 NextPosition = ActiveUnitMap.Value[0];
                Vector3 UpdatedPosition = DicMovingMapUnitByPosition[ActiveUnitMap.Key];

                if (UpdatedPosition.X < NextPosition.X - MovementSpeed)
                {
                    UpdatedPosition.X += MovementSpeed;
                    if (UpdatedPosition.X > NextPosition.X + MovementSpeed)
                        UpdatedPosition.X = NextPosition.X;
                }
                else if (UpdatedPosition.X > NextPosition.X + MovementSpeed)
                {
                    UpdatedPosition.X -= MovementSpeed;
                    if (UpdatedPosition.X < NextPosition.X - MovementSpeed)
                        UpdatedPosition.X = NextPosition.X;
                }
                else
                {
                    UpdatedPosition.X = NextPosition.X;
                }

                if (UpdatedPosition.Y < NextPosition.Y - MovementSpeed)
                {
                    UpdatedPosition.Y += MovementSpeed;
                    if (UpdatedPosition.Y > NextPosition.Y + MovementSpeed)
                        UpdatedPosition.Y = NextPosition.Y;
                }
                else if (UpdatedPosition.Y > NextPosition.Y + MovementSpeed)
                {
                    UpdatedPosition.Y -= MovementSpeed;
                    if (UpdatedPosition.Y < NextPosition.Y - MovementSpeed)
                        UpdatedPosition.Y = NextPosition.Y;
                }
                else
                {
                    UpdatedPosition.Y = NextPosition.Y;
                }

                if (UpdatedPosition.Z < NextPosition.Z - MovementSpeed)
                {
                    UpdatedPosition.Z += MovementSpeed;
                    if (UpdatedPosition.Z > NextPosition.Z + MovementSpeed)
                        UpdatedPosition.Z = NextPosition.Z;
                }
                else if (UpdatedPosition.Z > NextPosition.Z + MovementSpeed)
                {
                    UpdatedPosition.Z -= MovementSpeed;
                    if (UpdatedPosition.Z < NextPosition.Z - MovementSpeed)
                        UpdatedPosition.Z = NextPosition.Z;
                }
                else
                {
                    UpdatedPosition.Z = NextPosition.Z;
                }

                DicMovingMapUnitByPosition[ActiveUnitMap.Key] = UpdatedPosition;

                if (UpdatedPosition.X == NextPosition.X && UpdatedPosition.Y == NextPosition.Y && UpdatedPosition.Z == NextPosition.Z)
                {
                    ListRemovedSquad.Add(ActiveUnitMap.Key);
                }
            }

            foreach (UnitMapComponent RemovedSquad in ListRemovedSquad)
            {
                DicMovingMapUnitByNextPosition[RemovedSquad].RemoveAt(0);

                if (DicMovingMapUnitByNextPosition[RemovedSquad].Count == 0)
                {
                    DicMovingMapUnitByPosition.Remove(RemovedSquad);
                    DicMovingMapUnitByNextPosition.Remove(RemovedSquad);
                }
            }

            ListRemovedSquad.Clear();

            if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {
                Map.OnlinePlayers.ExecuteAndSend(new Online.BattleMapLobyScriptHolder.SkipSquadMovementScript(Map));
            }
        }

        public void Skip()
        {
            DicMovingMapUnitByPosition.Clear();
            DicMovingMapUnitByNextPosition.Clear();
        }

        public int Count
        {
            get
            {
                return DicMovingMapUnitByPosition.Count;
            }
        }
    }
}
