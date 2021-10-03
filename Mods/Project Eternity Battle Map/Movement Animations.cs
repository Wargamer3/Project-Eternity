using System;
using System.Collections.Generic;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class MovementAnimations
    {
        public List<float> ListPosX;
        public List<float> ListPosY;
        public List<UnitMapComponent> ListMovingMapUnit;

        public MovementAnimations()
        {
            ListPosX = new List<float>();
            ListPosY = new List<float>();
            ListMovingMapUnit = new List<UnitMapComponent>();
        }

        public void Add(float PosX, float PosY, UnitMapComponent MovingMapUnit)
        {
            ListPosX.Add(PosX);
            ListPosY.Add(PosY);
            ListMovingMapUnit.Add(MovingMapUnit);
        }

        public bool Contains(UnitMapComponent MovingMapUnit)
        {
            return ListMovingMapUnit.Contains(MovingMapUnit);
        }

        public int IndexOf(UnitMapComponent MovingMapUnit)
        {
            for (int i = ListMovingMapUnit.Count - 1; i >= 0; --i)
            {
                if (ListMovingMapUnit.IndexOf(MovingMapUnit) >= 0)
                    return ListMovingMapUnit.IndexOf(MovingMapUnit);
            }
            return -1;
        }

        public int Count
        {
            get
            {
                return ListPosX.Count;
            }
        }

        public void RemoveAt(int Index)
        {
            ListPosX.RemoveAt(Index);
            ListPosY.RemoveAt(Index);
            ListMovingMapUnit.RemoveAt(Index);
        }
    }
}
