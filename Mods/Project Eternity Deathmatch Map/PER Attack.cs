using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class PERAttack
    {
        private readonly DeathmatchMap Map;
        public Attack ActiveAttack;
        public Squad Owner;
        public int PlayerIndex;//Only decrement TurnsRemaining if the current player index correspond
        public Vector3 Position;
        public Vector3 Speed;
        public int Lifetime;
        public Attack3D Map3DComponent;

        public PERAttack(Attack ActiveAttack, Squad Owner, int PlayerIndex, DeathmatchMap Map, Vector3 Position, Vector3 Speed, int Lifetime)
        {
            this.ActiveAttack = ActiveAttack;
            this.Owner = Owner;
            this.PlayerIndex = PlayerIndex;
            this.Map = Map;
            this.Position = Position;
            this.Speed = Speed;
            this.Lifetime = Lifetime;

            if (ActiveAttack.PERAttributes.ProjectileAnimation.IsAnimated)
            {
                Map3DComponent = new Attack3D(GameScreen.GraphicsDevice, Map.Content.Load<Effect>("Shaders/Billboard 3D"), ActiveAttack.PERAttributes.ProjectileAnimation.ActualSprite.ActiveSprite, ActiveAttack.PERAttributes.ProjectileAnimation.ActualSprite.FrameCount);
            }
            else
            {
                Map3DComponent = new Attack3D(GameScreen.GraphicsDevice, Map.Content.Load<Effect>("Shaders/Billboard 3D"), ActiveAttack.PERAttributes.ProjectileAnimation.StaticSprite, 1);
            }

            Map3DComponent.SetPosition(
                Position.X * 32 + 16 + 0.5f,
                Position.Z * 32,
                Position.Y * 32 + 16 + 0.5f);
        }

        public void SetPosition(Vector3 NewPosition)
        {
            Position = NewPosition;

            Map3DComponent.SetPosition(
                Position.X * 32 + 0.5f,
                Position.Z * 32,
                Position.Y * 32 + 0.5f);
        }

        public void ProcessMovement(Vector3 NextPosition, Terrain ActiveTerrain)
        {
            bool IsAttackingSelf = NextPosition == Position;

            Stack<Tuple<int, int>> ListAttackTarget = new Stack<Tuple<int, int>>();

            Tuple<int, int> ActiveTarget = CheckForEnemies(Map, PlayerIndex, new Vector3(ActiveTerrain.Position.X, ActiveTerrain.Position.Y, ActiveTerrain.LayerIndex), true);

            if (ActiveTarget != null)
            {
                if (Map.ListPlayer[ActiveTarget.Item1].ListSquad[ActiveTarget.Item2] == Owner && !IsAttackingSelf)
                {
                    return;
                }

                DestroySelf();

                if (ActiveAttack.ExplosionOption.ExplosionRadius > 0)
                {
                    HandleExplosion();
                }
                else
                {
                    ListAttackTarget.Push(ActiveTarget);
                    Map.AttackWithMAPAttack(PlayerIndex, Map.ListPlayer[PlayerIndex].ListSquad.IndexOf(Owner), ActiveAttack, new List<Vector3>(), ListAttackTarget);
                }
            }
            else if (ActiveTerrain.TerrainTypeIndex == UnitStats.TerrainWallIndex)
            {
                DestroySelf();

                if (ActiveAttack.ExplosionOption.ExplosionRadius > 0)
                {
                    HandleExplosion();
                }
            }

            SetPosition(NextPosition);

            if (Position.X < 0 || Position.X >= Map.MapSize.X || Position.Y < 0 || Position.Y >= Map.MapSize.Y || Position.Z < 0 || Position.Z >= Map.LayerManager.ListLayer.Count)
            {
                DestroySelf();
            }
        }

        public static List<MovementAlgorithmTile> PredictAttackMovement(DeathmatchMap Map, Vector3 StartPosition, Vector3 CursorPosition)
        {
            List<MovementAlgorithmTile> ListCrossedTerrain = new List<MovementAlgorithmTile>();

            Vector3 Diff = CursorPosition - StartPosition;
            Diff.Normalize();

            Terrain NextTerrain = Map.GetTerrain(StartPosition.X, StartPosition.Y, (int)StartPosition.Z);
            ListCrossedTerrain.Add(NextTerrain);

            float Progress = 0;

            int LastX = (int)StartPosition.X;
            int LastY = (int)StartPosition.Y;
            int LastZ = (int)StartPosition.Z;
            int CurrentX = (int)StartPosition.X;
            int CurrentY = (int)StartPosition.Y;
            int CurrentZ = (int)StartPosition.Z;
            int NextX;
            int NextY;
            int NextZ;

            while (NextTerrain != null)
            {
                Progress += 1;

                NextX = (int)(StartPosition.X + Progress * Diff.X);
                NextY = (int)(StartPosition.Y + Progress * Diff.Y);
                NextZ = (int)(StartPosition.Z + Progress * Diff.Z);

                if (NextX < 0 || NextX >= Map.MapSize.X || NextY < 0 || NextY >= Map.MapSize.Y || NextZ < 0 || NextZ >= Map.LayerManager.ListLayer.Count)
                {
                    break;
                }

                #region Corners

                if (CurrentX != NextX && (CurrentY != NextY || CurrentZ != NextZ)
                    || CurrentY != NextY && (CurrentX != NextX || CurrentZ != NextZ)
                    || CurrentZ != NextZ && (CurrentX != NextX || CurrentY != NextY))
                {
                    int CornerNextX = NextX;
                    int CornerNextY = NextY;
                    int CornerNextZ = NextZ;

                    if (LastX == CurrentX)
                    {
                        CornerNextX = CurrentX;
                    }
                    else if (LastY == CurrentY)
                    {
                        CornerNextY = CurrentY;
                    }
                    else
                    {
                        CornerNextZ = CurrentZ;
                    }

                    if (CornerNextX < 0 || CornerNextX >= Map.MapSize.X || CornerNextY < 0 || CornerNextY >= Map.MapSize.Y || CornerNextZ < 0 || CornerNextZ >= Map.LayerManager.ListLayer.Count)
                    {
                        break;
                    }

                    NextTerrain = Map.GetTerrain(CornerNextX, CornerNextY, CornerNextZ);

                    CurrentX = CornerNextX;
                    CurrentY = CornerNextY;
                    CurrentZ = CornerNextZ;

                    if (!ListCrossedTerrain.Contains(NextTerrain))
                    {
                        ListCrossedTerrain.Add(NextTerrain);
                    }

                    if (NextTerrain.TerrainTypeIndex == UnitStats.TerrainWallIndex)
                    {
                        break;
                    }

                    Tuple<int, int> ActiveCornerTarget = CheckForEnemies(Map, Map.ActivePlayerIndex, new Vector3(CornerNextX, CornerNextY, CornerNextZ), true);

                    if (ActiveCornerTarget != null)
                    {
                        break;
                    }
                }

                #endregion

                NextTerrain = Map.GetTerrain(NextX, NextY, NextZ);

                if (!ListCrossedTerrain.Contains(NextTerrain))
                {
                    ListCrossedTerrain.Add(NextTerrain);
                }

                if (NextTerrain.TerrainTypeIndex == UnitStats.TerrainWallIndex)
                {
                    break;
                }

                Tuple<int, int> ActiveTarget = CheckForEnemies(Map, Map.ActivePlayerIndex, new Vector3(NextX, NextY, NextZ), true);

                if (ActiveTarget != null)
                {
                    break;
                }

                LastX = CurrentX;
                LastY = CurrentY;
                LastZ = CurrentZ;

                CurrentX = NextX;
                CurrentY = NextY;
                CurrentZ = NextZ;
            }

            return ListCrossedTerrain;
        }

        public List<Terrain> GetCrossedTerrain(Vector3 NextPosition)
        {
            List<Terrain> ListCrossedTerrain = new List<Terrain>();

            Vector3 Diff = NextPosition - Position;
            float DistanceToTravel = Diff.Length();
            Diff.Normalize();

            Terrain NextTerrain = Map.GetTerrain(Position.X, Position.Y, (int)Position.Z);
            float Progress = 0;

            int LastX = (int)Position.X;
            int LastY = (int)Position.Y;
            int LastZ = (int)Position.Z;
            int CurrentX = (int)Position.X;
            int CurrentY = (int)Position.Y;
            int CurrentZ = (int)Position.Z;
            int NextX;
            int NextY;
            int NextZ;


            while (NextTerrain != null)
            {
                Progress += 1;
                NextX = (int)(Position.X + Progress * Diff.X);
                NextY = (int)(Position.Y + Progress * Diff.Y);
                NextZ = (int)(Position.Z + Progress * Diff.Z);

                if (NextX < 0 || NextX >= Map.MapSize.X || NextY < 0 || NextY >= Map.MapSize.Y || NextZ < 0 || NextZ >= Map.LayerManager.ListLayer.Count)
                {
                    break;
                }

                #region Corners

                if (CurrentX != NextX && (CurrentY != NextY || CurrentZ != NextZ)
                    || CurrentY != NextY && (CurrentX != NextX || CurrentZ != NextZ)
                    || CurrentZ != NextZ && (CurrentX != NextX || CurrentY != NextY))
                {
                    int CornerNextX = NextX;
                    int CornerNextY = NextY;
                    int CornerNextZ = NextZ;

                    if (LastX == CurrentX)
                    {
                        CornerNextX = CurrentX;
                    }
                    else if (LastY == CurrentY)
                    {
                        CornerNextY = CurrentY;
                    }
                    else
                    {
                        CornerNextZ = CurrentZ;
                    }

                    if (CornerNextX < 0 || CornerNextX >= Map.MapSize.X || CornerNextY < 0 || CornerNextY >= Map.MapSize.Y || CornerNextZ < 0 || CornerNextZ >= Map.LayerManager.ListLayer.Count)
                    {
                        break;
                    }

                    NextTerrain = Map.GetTerrain(CornerNextX, CornerNextY, CornerNextZ);

                    CurrentX = CornerNextX;
                    CurrentY = CornerNextY;
                    CurrentZ = CornerNextZ;

                    if (!ListCrossedTerrain.Contains(NextTerrain))
                    {
                        ListCrossedTerrain.Add(NextTerrain);
                    }

                    if (NextTerrain.TerrainTypeIndex == UnitStats.TerrainWallIndex)
                    {
                        break;
                    }

                    Tuple<int, int> ActiveCornerTarget = CheckForEnemies(Map, Map.ActivePlayerIndex, new Vector3(CornerNextX, CornerNextY, CornerNextZ), true);

                    if (ActiveCornerTarget != null)
                    {
                        break;
                    }
                }

                #endregion

                NextTerrain = Map.GetTerrain(NextX, NextY, NextZ);

                if (!ListCrossedTerrain.Contains(NextTerrain))
                {
                    ListCrossedTerrain.Add(NextTerrain);
                }

                if (Progress >= DistanceToTravel || NextTerrain.TerrainTypeIndex == UnitStats.TerrainWallIndex)
                {
                    break;
                }

                LastX = CurrentX;
                LastY = CurrentY;
                LastZ = CurrentZ;

                CurrentX = NextX;
                CurrentY = NextY;
                CurrentZ = NextZ;
            }

            return ListCrossedTerrain;
        }

        public static Tuple<int, int> CheckForEnemies(DeathmatchMap Map, int ActivePlayerIndex, Vector3 PositionToCheck, bool FriendlyFire)
        {
            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                //Find if a Unit is under the cursor.
                int TargetIndex = Map.CheckForSquadAtPosition(P, PositionToCheck, Vector3.Zero);
                //If one was found.
                if (TargetIndex >= 0 && (FriendlyFire || Map.ListPlayer[ActivePlayerIndex].Team != Map.ListPlayer[P].Team))
                {
                    return new Tuple<int, int>(P, TargetIndex);
                }
            }

            return null;
        }

        public void DestroySelf()
        {
            Map.ListPERAttack.Remove(this);
        }

        public void HandleExplosion()
        {
            int StartX = (int)(Position.X - ActiveAttack.ExplosionOption.ExplosionRadius);
            int EndX = (int)(Position.X + ActiveAttack.ExplosionOption.ExplosionRadius);
            int StartY = (int)(Position.Y - ActiveAttack.ExplosionOption.ExplosionRadius);
            int EndY = (int)(Position.Y + ActiveAttack.ExplosionOption.ExplosionRadius);
            int StartZ = (int)(Position.Z - ActiveAttack.ExplosionOption.ExplosionRadius);
            int EndZ = (int)(Position.Z + ActiveAttack.ExplosionOption.ExplosionRadius);

            Stack<Tuple<int, int>> ListAttackTarget = new Stack<Tuple<int, int>>();

            for (int X = StartX; X < EndX; ++X)
            {
                for (int Y = StartY; Y < EndY; ++Y)
                {
                    for (int Z = StartZ; Z < EndZ; ++Z)
                    {
                        Tuple<int, int> ActiveTarget = CheckForEnemies(Map, PlayerIndex, new Vector3(X, Y, Z), true);

                        if (ActiveTarget != null)
                        {
                            ListAttackTarget.Push(ActiveTarget);
                        }
                    }
                }
            }

            if (ListAttackTarget.Count > 0)
            {
                Map.AttackWithMAPAttack(PlayerIndex, Map.ListPlayer[PlayerIndex].ListSquad.IndexOf(Owner), ActiveAttack, new List<Vector3>(), ListAttackTarget);
                foreach (Tuple<int, int> ActiveTarget in ListAttackTarget)
                {
                    Squad SquadToKill = Map.ListPlayer[ActiveTarget.Item1].ListSquad[ActiveTarget.Item2];

                    Terrain SquadTerrain = Map.GetTerrain(SquadToKill.X, SquadToKill.Y, (int)SquadToKill.Z);

                    float DiffX = Math.Abs(Position.X - SquadToKill.Position.X);
                    float DiffY = Math.Abs(Position.Y - SquadToKill.Position.Y);
                    float DiffZ = Math.Abs(Position.Z - (SquadToKill.Position.Z + SquadTerrain.Position.Z));

                    float DiffTotal = (DiffX + DiffY + DiffZ) / 3;

                    if (DiffTotal < ActiveAttack.ExplosionOption.ExplosionRadius)
                    {
                        float WindForce = DiffTotal / ActiveAttack.ExplosionOption.ExplosionRadius;
                        float WindValue = ActiveAttack.ExplosionOption.ExplosionWindPowerAtEdge + WindForce * (ActiveAttack.ExplosionOption.ExplosionWindPowerAtCenter - ActiveAttack.ExplosionOption.ExplosionWindPowerAtEdge);

                        Vector3 FinalSpeed = SquadToKill.Position - Position;
                        FinalSpeed.Normalize();

                        FinalSpeed *= WindValue;
                        SquadToKill.Speed = FinalSpeed;

                        SquadToKill.SetPosition(SquadToKill.Position + new Vector3(0, 0, Map.LayerManager.ListLayer[(int)SquadToKill.Position.Z].ArrayTerrain[(int)SquadToKill.Position.X, (int)SquadToKill.Position.Y].Position.Z));

                        if (FinalSpeed.Z > 0)
                        {
                            SquadToKill.CurrentMovement = UnitStats.TerrainAir;
                            SquadToKill.IsFlying = false;
                        }
                    }
                }
            }
        }
    }
}
