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

            Tuple<int, int> ActiveTarget = CheckForEnemies(new Vector3(ActiveTerrain.Position.X, ActiveTerrain.Position.Y, ActiveTerrain.LayerIndex), true);

            if (ActiveTarget != null)
            {
                if (Map.ListPlayer[ActiveTarget.Item1].ListSquad[ActiveTarget.Item2] == Owner && !IsAttackingSelf)
                {
                    return;
                }

                ListAttackTarget.Push(ActiveTarget);
                Map.AttackWithMAPAttack(PlayerIndex, Map.ListPlayer[PlayerIndex].ListSquad.IndexOf(Owner), ActiveAttack, ListAttackTarget);
                DestroySelf();
            }
            else if (ActiveTerrain.TerrainTypeIndex == UnitStats.TerrainWallIndex)
            {
                DestroySelf();
            }

            SetPosition(NextPosition);

            if (Position.X < 0 || Position.X >= Map.MapSize.X || Position.Y < 0 || Position.Y >= Map.MapSize.Y || Position.Z < 0 || Position.Z >= Map.LayerManager.ListLayer.Count)
            {
                DestroySelf();
            }
        }

        public List<Terrain> GetCrossedTerrain(Vector3 NextPosition)
        {
            List<Terrain> ListCrossedTerrain = new List<Terrain>();

            Vector3 Diff = NextPosition - Position;
            float DistanceToTravel = Diff.Length();
            Diff.Normalize();

            Terrain NextTerrain = Map.GetTerrain(Position.X, Position.Y, (int)Position.Z);
            float Progress = 0;

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

                NextTerrain = Map.GetTerrain(NextX, NextY, NextZ);

                ListCrossedTerrain.Add(NextTerrain);

                if (Progress >= DistanceToTravel || NextTerrain.TerrainTypeIndex == UnitStats.TerrainWallIndex)
                {
                    break;
                }
            }

            return ListCrossedTerrain;
        }

        public Tuple<int, int> CheckForEnemies(Vector3 PositionToCheck, bool FriendlyFire)
        {
            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                //Find if a Unit is under the cursor.
                int TargetIndex = Map.CheckForSquadAtPosition(P, PositionToCheck, Vector3.Zero);
                //If one was found.
                if (TargetIndex >= 0 && (FriendlyFire || Map.ListPlayer[PlayerIndex].Team != Map.ListPlayer[P].Team))
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
    }
}
