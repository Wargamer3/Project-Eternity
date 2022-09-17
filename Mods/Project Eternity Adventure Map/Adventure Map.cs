using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.AdventureScreen
{
    public class AdventureMap : GameScreen
    {
        private readonly List<Bullet> BulletCollisions;//Don't use a CollisionZone for Bullets as they only check collisions with other objects.
        private readonly CollisionZone2D<Player> PlayerCollisions;
        private readonly CollisionZone2D<WorldObject> WorldCollisions;
        private readonly CollisionZone2D<Bomb> BombCollisions;
        private readonly List<SimpleLinearExplosion> ExplosionCollisions;
        public Rectangle Camera;
        public Rectangle CameraBounds;
        public List<Polygon> ListWorldCollisionPolygon;
        public List<SimpleAnimation> ListImages;
        public List<Prop> ListProp;
        private readonly Player MainPlayer;

        public List<SimpleLinearExplosion> GetExplosions()
        {
            return ExplosionCollisions;
        }

        public AdventureMap()
        {
            BulletCollisions = new List<Bullet>(1024);
            PlayerCollisions = new CollisionZone2D<Player>(2000, 100, -500, -500);
            WorldCollisions = new CollisionZone2D<WorldObject>(2000, 100, -500, -500);
            BombCollisions = new CollisionZone2D<Bomb>(2000, 100, -500, -500);
            ExplosionCollisions = new List<SimpleLinearExplosion>();

            ListWorldCollisionPolygon = new List<Polygon>();
            ListImages = new List<SimpleAnimation>();
            ListProp = new List<Prop>();
        }

        public AdventureMap(string Path)
            : this()
        {
        }

        public override void Load()
        {
        }

        public void Reset()
        {
            BulletCollisions.Clear();
            PlayerCollisions.Clear();
            WorldCollisions.Clear();
            BombCollisions.Clear();
            ExplosionCollisions.Clear();
        }

        #region Collisions

        public void Add(Bullet NewObject)
        {
            BulletCollisions.Add(NewObject);
        }

        public void Add(Player NewObject)
        {
            PlayerCollisions.AddToZone(NewObject);
        }

        public void Add(WorldObject NewObject)
        {
            WorldCollisions.AddToZone(NewObject);
        }

        public void Add(Bomb NewObject)
        {
            BombCollisions.AddToZone(NewObject);
        }

        public void Add(SimpleLinearExplosion NewObject)
        {
            ExplosionCollisions.Add(NewObject);
        }

        public void Remove(Player ActivePlayer)
        {
            PlayerCollisions.Remove(ActivePlayer);
        }

        public void Remove(Bomb ActiveBomb)
        {
            BombCollisions.Remove(ActiveBomb);
        }

        public HashSet<Player> GetCollidingPlayers(Player ActivePlayer)
        {
            return PlayerCollisions.GetCollidableObjects(ActivePlayer);
        }

        public HashSet<Player> GetCollidingPlayers(Bullet ActiveBullet)
        {
            return PlayerCollisions.GetCollidableObjects(ActiveBullet.Collision);
        }

        public HashSet<Player> GetCollidingPlayers(SimpleLinearExplosion ActiveExplosion)
        {
            return PlayerCollisions.GetCollidableObjects(ActiveExplosion.Collision);
        }

        public HashSet<Player> GetCollidingPlayers(ComplexLinearExplosion ActiveExplosion)
        {
            return PlayerCollisions.GetCollidableObjects(ActiveExplosion.Collision);
        }

        public void Save(BinaryWriter BW)
        {
            throw new NotImplementedException();
        }

        public HashSet<WorldObject> GetCollidingWorldObjects(Player ActivePlayer)
        {
            return WorldCollisions.GetCollidableObjects(ActivePlayer.Collision);
        }

        public HashSet<WorldObject> GetCollidingWorldObjects(Bullet ActiveBullet)
        {
            return WorldCollisions.GetCollidableObjects(ActiveBullet.Collision);
        }

        public HashSet<WorldObject> GetCollidingWorldObjects(SimpleLinearExplosion ActiveExplosion)
        {
            return WorldCollisions.GetCollidableObjects(ActiveExplosion.Collision);
        }

        public HashSet<WorldObject> GetCollidingWorldObjects(ComplexLinearExplosion ActiveExplosion)
        {
            return WorldCollisions.GetCollidableObjects(ActiveExplosion.Collision);
        }

        #endregion

        public void Move(Player ActivePlayer, Vector2 Movement)
        {
            PlayerCollisions.Move(ActivePlayer, Movement);
        }

        private void UpdatePlayerControls(GameTime gameTime)
        {
            if (InputHelper.InputUpHold())
            {
                MainPlayer.Walk(new Vector2(0f, -1f));
            }
            else if (InputHelper.InputDownHold())
            {
                MainPlayer.Walk(new Vector2(0f, 1f));
            }
            else if (InputHelper.InputLeftHold())
            {
                MainPlayer.Walk(new Vector2(-1f, 0f));
            }
            else if (InputHelper.InputRightHold())
            {
                MainPlayer.Walk(new Vector2(1f, 0f));
            }

            if (InputHelper.InputConfirmPressed())
            {
                MainPlayer.DropBomb();
            }
        }

        public override void Update(GameTime gameTime)
        {
            for (int B = 0; B < BulletCollisions.Count; ++B)
            {
                BulletCollisions[B].Update(gameTime);
            }

            UpdatePlayerControls(gameTime);

            LinkedListNode<Player> ActivePlayer = PlayerCollisions.ListObjectInZoneAndOverlappingParents.First;
            while(ActivePlayer != null)
            {
                LinkedListNode<Player> NextPlayer = ActivePlayer.Next;
                ActivePlayer.Value.Update(gameTime);
                ActivePlayer = NextPlayer;
            }

            LinkedListNode<Bomb> ActiveBomb = BombCollisions.ListObjectInZoneAndOverlappingParents.First;
            while (ActiveBomb != null)
            {
                LinkedListNode<Bomb> NextBomb = ActiveBomb.Next;
                ActiveBomb.Value.Update(gameTime);
                ActiveBomb = NextBomb;
            }
        }
        
        public override void Draw(CustomSpriteBatch g)
        {
            throw new NotImplementedException();
        }
    }
}
