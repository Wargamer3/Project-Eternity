using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.Core.Attacks
{
    public struct MAPAttackAttributes
    {
        public int Width;
        public int Height;
        public List<List<bool>> ListChoice; // When Directional, default direction is down.
        public WeaponMAPProperties Property;
        public bool FriendlyFire;
        public int Delay;

        public MAPAttackAttributes(BinaryReader BR)
        {
            Width = BR.ReadInt32();
            Height = BR.ReadInt32();
            ListChoice = new List<List<bool>>(Width * 2 + 1);
            for (int X = 0; X < Width * 2 + 1; X++)
            {
                ListChoice.Add(new List<bool>(Height * 2 + 1));
                for (int Y = 0; Y < Height * 2 + 1; Y++)
                    ListChoice[X].Add(BR.ReadBoolean());
            }

            Property = (WeaponMAPProperties)BR.ReadByte();
            FriendlyFire = BR.ReadBoolean();
            Delay = BR.ReadInt32();
        }

        public bool CanAttackTarget(Vector3 StartPosition, Vector3 TargetPosition, int StartingMV, int FinishingMV)
        {
            int StartX = (int)StartPosition.X;
            int StartY = (int)StartPosition.Y;
            int TargetX = (int)TargetPosition.X;
            int TargetY = (int)TargetPosition.Y;
            
            if (Property == WeaponMAPProperties.Spread)
            {
                for (int X = ListChoice.Count - 1; X >= 0; --X)
                {
                    for (int Y = ListChoice[X].Count - 1; Y >= 0; --Y)
                    {
                        //Not an active tile.
                        if (!ListChoice[X][Y])
                            continue;
                        
                        //Start a the cursor position.
                        float PosX = StartX - Width + X;
                        float PosY = StartY - Height + Y;

                        //If a Unit is in range.
                        if (PosX == TargetX && PosY == TargetY)
                        {
                            return true;
                        }
                    }
                }
            }

            #region Dirrection

            else if (Property == WeaponMAPProperties.Direction)
            {
                if (GetTargetsDirectionalUp(StartPosition).Contains(TargetPosition))
                    return true;

                if (GetTargetsDirectionalDown(StartPosition).Contains(TargetPosition))
                    return true;

                if (GetTargetsDirectionalLeft(StartPosition).Contains(TargetPosition))
                    return true;

                if (GetTargetsDirectionalRight(StartPosition).Contains(TargetPosition))
                    return true;
            }

            #endregion

            else if (Property == WeaponMAPProperties.Targeted)
            {
                int DistanceX = Math.Abs(StartX - TargetX);
                int DistanceY = Math.Abs(StartY - TargetY);
                //If a Unit is in range.
                if (DistanceX + Width >= StartingMV && DistanceX <= FinishingMV + Width &&
                    DistanceY + Height >= StartingMV && DistanceY <= FinishingMV + Height)
                {
                    for (int X = ListChoice.Count - 1; X >= 0; --X)
                    {
                        for (int Y = ListChoice[X].Count - 1; Y >= 0; --Y)
                        {
                            //Not an active tile.
                            if (!ListChoice[X][Y])
                                continue;
                            int DistanceCenterX = X - Width;
                            int DistanceCenterY = Y - Height;

                            if (DistanceX >= StartingMV + DistanceCenterX && DistanceX <= FinishingMV + DistanceCenterX &&
                                DistanceY >= StartingMV + DistanceCenterY && DistanceY <= FinishingMV + DistanceCenterY)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public List<Vector3> GetTargetsDirectionalUp(Vector3 Position)
        {
            List<Vector3> AttackChoice = new List<Vector3>();

            float PosX = Position.X - Width;
            float PosY = Position.Y - 1;

            for (int X = 0; X < ListChoice.Count; X++)
            {
                for (int Y = 0; Y < ListChoice[X].Count; Y++)
                {
                    if (ListChoice[X][Y])
                    {
                        AttackChoice.Add(new Vector3(PosX + X, PosY - Y, Position.Z));
                    }
                }
            }

            return AttackChoice;
        }

        public List<Vector3> GetTargetsDirectionalDown(Vector3 Position)
        {
            List<Vector3> AttackChoice = new List<Vector3>();

            float PosX = Position.X - Width;
            float PosY = Position.Y + 1;

            for (int X = 0; X < ListChoice.Count; X++)
            {
                for (int Y = 0; Y < ListChoice[X].Count; Y++)
                {
                    if (ListChoice[X][Y])
                    {
                        AttackChoice.Add(new Vector3(PosX + X, PosY + Y, Position.Z));
                    }
                }
            }

            return AttackChoice;
        }

        public List<Vector3> GetTargetsDirectionalLeft(Vector3 Position)
        {
            List<Vector3> AttackChoice = new List<Vector3>();

            float PosX = Position.X - 1;
            float PosY = Position.Y - Width;

            for (int X = 0; X < ListChoice.Count; X++)
            {
                for (int Y = 0; Y < ListChoice[X].Count; Y++)
                {
                    if (ListChoice[X][Y])
                    {
                        AttackChoice.Add(new Vector3(PosX - Y, PosY + X, Position.Z));
                    }
                }
            }

            return AttackChoice;
        }

        public List<Vector3> GetTargetsDirectionalRight(Vector3 Position)
        {
            List<Vector3> AttackChoice = new List<Vector3>();

            float PosX = Position.X + 1;
            float PosY = Position.Y - Width;

            for (int X = 0; X < ListChoice.Count; X++)
            {
                for (int Y = 0; Y < ListChoice[X].Count; Y++)
                {
                    if (ListChoice[X][Y])
                    {
                        AttackChoice.Add(new Vector3(PosX + Y, PosY + X, Position.Z));
                    }
                }
            }

            return AttackChoice;
        }
    }
}
