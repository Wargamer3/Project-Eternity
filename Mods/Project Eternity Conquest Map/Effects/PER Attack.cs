using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class PERAttack : Projectile3D
    {
        private readonly ConquestMap Map;
        public Attack ActiveAttack;
        public UnitConquest Owner;
        public int PlayerIndex;//Only decrement TurnsRemaining if the current player index correspond
        public Vector3 Position;
        public Attack3D Map3DComponent;
        public AnimatedModel Unit3DModel;
        public bool IsOnGround = false;//Follow slopes if on ground
        public bool AllowRicochet = true;//Follow 45 degree walls

        public PERAttack(Attack ActiveAttack, UnitConquest Owner, int PlayerIndex, ConquestMap Map, Vector3 Position, Vector3 Speed, int Lifetime)
            : base(Lifetime)
        {
            this.ActiveAttack = ActiveAttack;
            this.Owner = Owner;
            this.PlayerIndex = PlayerIndex;
            this.Map = Map;
            this.Position = Position;
            this.Speed = Speed;

            if (!string.IsNullOrEmpty(ActiveAttack.PERAttributes.ProjectileAnimation.Path))
            {
                if (ActiveAttack.PERAttributes.ProjectileAnimation.IsAnimated)
                {
                    Map3DComponent = new Attack3D(GameScreen.GraphicsDevice, Map.Content.Load<Effect>("Shaders/Billboard 3D"), ActiveAttack.PERAttributes.ProjectileAnimation.ActualSprite.ActiveSprite, ActiveAttack.PERAttributes.ProjectileAnimation.ActualSprite.FrameCount);
                }
                else if (ActiveAttack.PERAttributes.ProjectileAnimation.ActualSprite != null)
                {
                    Map3DComponent = new Attack3D(GameScreen.GraphicsDevice, Map.Content.Load<Effect>("Shaders/Billboard 3D"), ActiveAttack.PERAttributes.ProjectileAnimation.ActualSprite.ActiveSprite, ActiveAttack.PERAttributes.ProjectileAnimation.ActualSprite.FrameCount);
                }
                else
                {
                    Map3DComponent = new Attack3D(GameScreen.GraphicsDevice, Map.Content.Load<Effect>("Shaders/Billboard 3D"), ActiveAttack.PERAttributes.ProjectileAnimation.StaticSprite, 1);
                }

                Map3DComponent.SetPosition(
                    Position.X * Map.TileSize.X + 16 + 0.5f,
                    Position.Z * Map.LayerHeight,
                    Position.Y * Map.TileSize.Y + 16 + 0.5f);
            }

            Unit3DModel = ActiveAttack.PERAttributes.Projectile3DModel;
        }

        public void SetPosition(Vector3 NewPosition)
        {
            Position = NewPosition;

            if (Map3DComponent != null)
            {
                Map3DComponent.SetPosition(
                    Position.X * Map.TileSize.X + 0.5f,
                    Position.Z * Map.LayerHeight,
                    Position.Y * Map.TileSize.Y + 0.5f);
            }
        }

        public bool ProcessMovement(Vector3 NextPosition, Terrain ActiveTerrain)
        {
            bool IsAttackingSelf = NextPosition == Position;

            Stack<Tuple<int, int>> ListAttackTarget = new Stack<Tuple<int, int>>();

            Tuple<int, int> ActiveTarget = Map.CheckForEnemies(PlayerIndex, new Vector3(ActiveTerrain.WorldPosition.X, ActiveTerrain.WorldPosition.Y, ActiveTerrain.LayerIndex), true);

            if (ActiveTarget != null)
            {
                if (Map.ListPlayer[ActiveTarget.Item1].ListUnit[ActiveTarget.Item2] == Owner && !IsAttackingSelf)
                {
                    return false;
                }

                if (ActiveAttack.ExplosionOption.ExplosionRadius > 0)
                {
                    Map.AttackWithExplosion(PlayerIndex, Owner, ActiveAttack, Position);
                }
                else
                {
                    ListAttackTarget.Push(ActiveTarget);
                    ActionPanelUseMAPAttack.AttackWithMAPAttack(Map, PlayerIndex, Map.ListPlayer[PlayerIndex].ListUnit.IndexOf(Owner), ActiveAttack, new List<Vector3>(), ListAttackTarget);
                }

                return true;
            }
            else if (ActiveTerrain.TerrainTypeIndex == UnitStats.TerrainWallIndex)
            {
                DamageTemporaryTerrain(new Vector3(ActiveTerrain.WorldPosition.X, ActiveTerrain.WorldPosition.Y, ActiveTerrain.LayerIndex));

                if (ActiveAttack.ExplosionOption.ExplosionRadius > 0)
                {
                    Map.AttackWithExplosion(PlayerIndex, Owner, ActiveAttack, Position);
                }

                return true;
            }

            SetPosition(NextPosition);

            if (Position.X < 0 || Position.X >= Map.MapSize.X || Position.Y < 0 || Position.Y >= Map.MapSize.Y || Position.Z < 0 || Position.Z >= Map.LayerManager.ListLayer.Count)
            {
                return true;
            }

            return false;
        }

        public static List<MovementAlgorithmTile> PredictAttackMovement(ConquestMap Map, Vector3 StartPosition, Vector3 CursorPosition)
        {
            List<MovementAlgorithmTile> ListCrossedTerrain = new List<MovementAlgorithmTile>();

            Vector3 Diff = CursorPosition - StartPosition;
            Diff.Normalize();

            Terrain NextTerrain = Map.GetTerrain(StartPosition);
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

                    Vector3 NextCornerPosition = new Vector3(CornerNextX, CornerNextY, CornerNextZ);
                    NextTerrain = Map.GetTerrain(NextCornerPosition);

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

                    Tuple<int, int> ActiveCornerTarget = Map.CheckForEnemies(Map.ActivePlayerIndex, NextCornerPosition, true);

                    if (ActiveCornerTarget != null)
                    {
                        break;
                    }
                }

                #endregion

                Vector3 NextPosition = new Vector3(NextX, NextY, NextZ);
                NextTerrain = Map.GetTerrain(NextPosition);

                if (!ListCrossedTerrain.Contains(NextTerrain))
                {
                    ListCrossedTerrain.Add(NextTerrain);
                }

                if (NextTerrain.TerrainTypeIndex == UnitStats.TerrainWallIndex)
                {
                    break;
                }

                Tuple<int, int> ActiveTarget = Map.CheckForEnemies(Map.ActivePlayerIndex, NextPosition, true);

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

            Terrain NextTerrain = Map.GetTerrain(Position);
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

                    Vector3 CornerNextPosition = new Vector3(CornerNextX, CornerNextY, CornerNextZ);

                    NextTerrain = Map.GetTerrain(CornerNextPosition);

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

                    Tuple<int, int> ActiveCornerTarget = Map.CheckForEnemies(Map.ActivePlayerIndex, CornerNextPosition, true);

                    if (ActiveCornerTarget != null)
                    {
                        break;
                    }
                }

                #endregion

                Vector3 FinalNextPosition = new Vector3(NextX, NextY, NextZ);
                NextTerrain = Map.GetTerrain(FinalNextPosition);

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

        public void DestroySelf()
        {
            Map.ListPERAttack.Remove(this);
        }

        private void DamageTemporaryTerrain(Vector3 Position)
        {
            DestructibleTerrain TemporaryTerrain;

            if (Map.DicTemporaryTerrain.TryGetValue(Position, out TemporaryTerrain))
            {
                TemporaryTerrain.DamageTile();

                if (TemporaryTerrain.RemainingHP <= 0)
                {
                    Map.DicTemporaryTerrain.Remove(Position);

                    MapLayer ActiveLayer = Map.LayerManager.ListLayer[(int)Position.Z];
                    for (int P = 0; P < ActiveLayer.ListProp.Count; P++)
                    {
                        if (ActiveLayer.ListProp[P].Position == Position)
                        {
                            ActiveLayer.ListProp.RemoveAt(P);
                        }
                    }
                }

                Map.Reset();
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            throw new NotImplementedException();
        }

        public override void SetAngle(float Angle)
        {
            throw new NotImplementedException();
        }
    }
}
