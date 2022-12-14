using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.AdventureScreen
{
    public class Bomb : ICollisionObjectHolder2D<Bomb>
    {
        private const float Size = 32;
        private readonly AdventureMap Owner;

        private CollisionObject2D<Bomb> CollisionBox;
        public CollisionObject2D<Bomb> Collision => CollisionBox;

        private double SecondsBeforeExplosion;

        public Bomb(AdventureMap Owner, Vector2 StartingPoint)
        {
            this.Owner = Owner;
            Vector2[] LocalPoints = new Vector2[4]
            {
                new Vector2(StartingPoint.X - Size, StartingPoint.Y - Size),
                new Vector2(StartingPoint.X - Size, StartingPoint.Y + Size),
                new Vector2(StartingPoint.X + Size, StartingPoint.Y + Size),
                new Vector2(StartingPoint.X + Size, StartingPoint.Y - Size),
            };

            CollisionBox = new CollisionObject2D<Bomb>(LocalPoints, 32, 32);
            SecondsBeforeExplosion = 5;
        }

        public void Update(GameTime gameTime)
        {
            SecondsBeforeExplosion -= gameTime.ElapsedGameTime.TotalSeconds;

            if (SecondsBeforeExplosion <= 0)
            {
                Owner.Remove(this);
                SimpleLinearExplosion ExplosionUp = new SimpleLinearExplosion(Owner, CollisionBox.Position, new Vector2(0f, -1f));
                SimpleLinearExplosion ExplosionDown = new SimpleLinearExplosion(Owner, CollisionBox.Position, new Vector2(0f, 1f));
                SimpleLinearExplosion ExplosionLeft = new SimpleLinearExplosion(Owner, CollisionBox.Position, new Vector2(-1f, 0f));
                SimpleLinearExplosion ExplosionRight = new SimpleLinearExplosion(Owner, CollisionBox.Position, new Vector2(1f, 0f));

                Owner.Add(ExplosionUp);
                Owner.Add(ExplosionDown);
                Owner.Add(ExplosionLeft);
                Owner.Add(ExplosionRight);
            }
        }
    }

    public class SimpleLinearExplosion : Projectile2D, ICollisionObjectHolder2D<SimpleLinearExplosion>
    {
        private const float Size = 32;

        private readonly AdventureMap Owner;
        private readonly CollisionObject2D<SimpleLinearExplosion> CollisionBox;
        public CollisionObject2D<SimpleLinearExplosion> Collision => CollisionBox;

        private Vector2 ForwardLeftVector { get { return CollisionBox.ListCollisionPolygon[0].ArrayVertex[IndexForwardLeftVector]; } }
        private Vector2 ForwardRightVector { get { return CollisionBox.ListCollisionPolygon[0].ArrayVertex[IndexForwardRightVector]; } }
        private Vector2 BackLeftVector { get { return CollisionBox.ListCollisionPolygon[0].ArrayVertex[IndexBackLeftVector]; } }
        private Vector2 BackRightVector { get { return CollisionBox.ListCollisionPolygon[0].ArrayVertex[IndexBackRightVector]; } }

        private readonly int IndexForwardLeftVector;
        private readonly int IndexForwardRightVector;
        private readonly int IndexBackLeftVector;
        private readonly int IndexBackRightVector;
        
        public SimpleLinearExplosion(AdventureMap Owner, Vector2 StartingPoint, Vector2 Speed)
        {
            this.Owner = Owner;
            this.Speed = Speed;

            Vector2[] LocalPoints = new Vector2[4]
            {
                new Vector2(StartingPoint.X - Size, StartingPoint.Y - Size),
                new Vector2(StartingPoint.X - Size, StartingPoint.Y + Size),
                new Vector2(StartingPoint.X + Size, StartingPoint.Y + Size),
                new Vector2(StartingPoint.X + Size, StartingPoint.Y - Size),
            };

            CollisionBox = new CollisionObject2D<SimpleLinearExplosion>(LocalPoints, 32, 32);

            //Going Left
            if (Speed.X < 0)
            {
                IndexForwardLeftVector = 1;
                IndexBackLeftVector = 2;
                IndexForwardRightVector = 0;
                IndexBackRightVector = 3;
            }
            //Going Right
            else if (Speed.X > 0)
            {
                IndexForwardLeftVector = 3;
                IndexBackLeftVector = 0;
                IndexForwardRightVector = 2;
                IndexBackRightVector = 1;
            }
            //Going Up
            else if (Speed.Y < 0)
            {
                IndexForwardLeftVector = 0;
                IndexBackLeftVector = 1;
                IndexForwardRightVector = 3;
                IndexBackRightVector = 2;
            }
            //Going Down
            else if (Speed.Y > 0)
            {
                IndexForwardLeftVector = 2;
                IndexBackLeftVector = 3;
                IndexForwardRightVector = 1;
                IndexBackRightVector = 0;
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            PolygonCollisionResult FinalCollisionResult;
            Polygon FinalActiveWorldObject;
            Polygon FinalExplosionPolygon;

            HashSet<Player> SetOtherPlayer = Owner.GetCollidingPlayers(this);
            foreach (Player ActivePlayer in SetOtherPlayer)
            {
                if (ActivePlayer.Collision.CollideWith(CollisionBox, Speed, out FinalCollisionResult, out FinalActiveWorldObject, out FinalExplosionPolygon))
                {
                    OnCollision(ActivePlayer, FinalCollisionResult, FinalActiveWorldObject, FinalExplosionPolygon);
                }
            }

            bool CollisionWithWall = false;
            HashSet<WorldObject> SetWorldObject = Owner.GetCollidingWorldObjects(this);
            LinkedList<WorldObject> ListCollidingWorldObject = new LinkedList<WorldObject>();
            foreach (WorldObject ActiveWorldObject in SetWorldObject)
            {
                if (ActiveWorldObject.Collision.CollideWith(CollisionBox, Speed, out FinalCollisionResult, out FinalActiveWorldObject, out FinalExplosionPolygon))
                {
                    ListCollidingWorldObject.AddLast(ActiveWorldObject);
                    OnCollision(ActiveWorldObject, FinalCollisionResult, FinalExplosionPolygon, FinalActiveWorldObject);
                    CollisionWithWall = true;
                }
            }

            if (!CollisionWithWall)
            {
                Move(Speed);
            }
        }

        private void OnCollision(Player Other, PolygonCollisionResult FinalCollisionResult, Polygon FinalBombPolygon, Polygon FinalPlayerPolygon)
        {
        }

        private void OnCollision(WorldObject Other, PolygonCollisionResult FinalCollisionResult, Polygon FinalBombPolygon, Polygon FinalWorldPolygon)
        {
            float DirectionAngle = (float)Math.Atan2(Speed.Y, Speed.X);
            float WallMinAngle = DirectionAngle - MathHelper.PiOver4;
            float WallMaxAngle = DirectionAngle + MathHelper.PiOver4;
            Vector2 WallAxis = new Vector2(-FinalCollisionResult.Axis.Y, FinalCollisionResult.Axis.X);
            double FinalCollisionResultAngle = Math.Atan2(WallAxis.X, WallAxis.Y);

            Vector2 MovementCorection = FinalCollisionResult.Axis * FinalCollisionResult.Distance;
            Vector2 FinalMovement = Speed + MovementCorection;

            //Ground detection
            if (FinalCollisionResultAngle >= WallMinAngle && FinalCollisionResultAngle <= WallMaxAngle)
            {
                Speed.X = 0;
                Speed.Y = 0;

                Move(FinalMovement);
            }
            //Slope
            else
            {
                if (FinalWorldPolygon.PolygonCollisionPerTriangle(ForwardLeftVector.X + FinalMovement.X, ForwardLeftVector.Y + FinalMovement.Y))
                {
                    Vector2 PerpendicularAxis = BackLeftVector - ForwardLeftVector;
                    PerpendicularAxis = new Vector2(-PerpendicularAxis.Y, PerpendicularAxis.X);
                    PerpendicularAxis.Normalize();

                    Vector2 CollisionLineAxis;
                    Vector2 LeftCollisionPoint = Polygon.GetCollisionPointFromLine(BackLeftVector, ForwardLeftVector, Speed, FinalWorldPolygon, out CollisionLineAxis);
                    Move(LeftCollisionPoint - ForwardLeftVector);

                    //Collision on the left, the collision axis should divert to the right
                    if (Vector2.Dot(PerpendicularAxis, CollisionLineAxis) < 0)
                    {
                        double WallAngle = Math.Atan2(CollisionLineAxis.Y, CollisionLineAxis.X);
                        Vector2 WallPerpendicularAxis = new Vector2(-CollisionLineAxis.Y, CollisionLineAxis.X);
                        WallPerpendicularAxis.Normalize();
                        double WallPerpendicularAngle = Math.Atan2(WallPerpendicularAxis.Y, WallPerpendicularAxis.X);
                        Vector2 NewExplosionForwardLeftVector = ForwardLeftVector;
                        Vector2 NewExplosionForwardRightVector = NewExplosionForwardLeftVector + new Vector2((float)Math.Cos(WallPerpendicularAngle) * Size, (float)Math.Sin(WallPerpendicularAngle) * Size);

                        SimpleLinearExplosion FollowingExplosion = new SimpleLinearExplosion(Owner, NewExplosionForwardLeftVector + (NewExplosionForwardRightVector - NewExplosionForwardLeftVector) / 2,
                            new Vector2((float)Math.Cos(WallAngle) * 3, (float)Math.Sin(WallAngle) * 3));

                        Owner.Add(FollowingExplosion);
                    }
                }
                else if (FinalWorldPolygon.PolygonCollisionPerTriangle(ForwardRightVector.X + FinalMovement.X, ForwardRightVector.Y + FinalMovement.Y))
                {
                    Vector2 PerpendicularAxis = BackRightVector - ForwardRightVector;
                    PerpendicularAxis = new Vector2(-PerpendicularAxis.Y, PerpendicularAxis.X);
                    PerpendicularAxis.Normalize();

                    Vector2 CollisionLineAxis;
                    Vector2 RightCollisionPoint = Polygon.GetCollisionPointFromLine(BackRightVector, ForwardRightVector, Speed, FinalWorldPolygon, out CollisionLineAxis);
                    Move(RightCollisionPoint - ForwardRightVector);

                    //Collision on the right, the collision axis should divert to the left
                    if (Vector2.Dot(PerpendicularAxis, CollisionLineAxis) < 0)
                    {

                    }
                }
                else
                {
                    //Will happen if the collision is on a corner, consider it as a wall.
                }
            }
        }
        
        private void Move(Vector2 Movement)
        {
            CollisionBox.Position += Movement / 2;
            CollisionBox.Radius += (Movement.X + Movement.Y) / 2;
            CollisionBox.ListCollisionPolygon[0].ArrayVertex[IndexForwardLeftVector] = CollisionBox.ListCollisionPolygon[0].ArrayVertex[IndexForwardLeftVector] + Movement;
            CollisionBox.ListCollisionPolygon[0].ArrayVertex[IndexForwardRightVector] = CollisionBox.ListCollisionPolygon[0].ArrayVertex[IndexForwardRightVector] + Movement;
        }

        public override void SetAngle(float Angle)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            throw new NotImplementedException();
        }
    }

    public class ComplexLinearExplosion : Projectile2D, ICollisionObjectHolder2D<ComplexLinearExplosion>
    {
        private readonly AdventureMap Owner;
        CollisionObject2D<ComplexLinearExplosion> CollisionBox;
        public CollisionObject2D<ComplexLinearExplosion> Collision => CollisionBox;

        private Vector2 ForwardLeftVector { get { return CollisionBox.ListCollisionPolygon[0].ArrayVertex[IndexForwardLeftVector]; } }
        private Vector2 ForwardRightVector { get { return CollisionBox.ListCollisionPolygon[0].ArrayVertex[IndexForwardRightVector]; } }
        private Vector2 BackLeftVector { get { return CollisionBox.ListCollisionPolygon[0].ArrayVertex[IndexBackLeftVector]; } }
        private Vector2 BackRightVector { get { return CollisionBox.ListCollisionPolygon[0].ArrayVertex[IndexBackRightVector]; } }

        private readonly int IndexForwardLeftVector;
        private readonly int IndexForwardRightVector;
        private readonly int IndexBackLeftVector;
        private readonly int IndexBackRightVector;
        float Size = 32;

        private Vector2 TopLeftVector { get { return CollisionBox.ListCollisionPolygon[0].ArrayVertex[0]; } }
        private Vector2 BottomLeftVector { get { return CollisionBox.ListCollisionPolygon[0].ArrayVertex[1]; } }
        private Vector2 BottomRightVector { get { return CollisionBox.ListCollisionPolygon[0].ArrayVertex[2]; } }
        private Vector2 TopRightVector { get { return CollisionBox.ListCollisionPolygon[0].ArrayVertex[3]; } }

        public ComplexLinearExplosion(AdventureMap Owner, Vector2 StartingPoint, Vector2 Speed)
        {
            this.Owner = Owner;
            this.Speed = Speed;

            Vector2[] LocalPoints = new Vector2[4]
            {
                new Vector2(StartingPoint.X - Size, StartingPoint.Y - Size),
                new Vector2(StartingPoint.X - Size, StartingPoint.Y + Size),
                new Vector2(StartingPoint.X + Size, StartingPoint.Y + Size),
                new Vector2(StartingPoint.X + Size, StartingPoint.Y - Size),
            };

            CollisionBox = new CollisionObject2D<ComplexLinearExplosion>(LocalPoints, 32, 32);

            //Going Left
            if (Speed.X < 0)
            {
                IndexForwardLeftVector = 1;
                IndexBackLeftVector = 2;
                IndexForwardRightVector = 0;
                IndexBackRightVector = 3;
            }
            //Going Right
            else if (Speed.X > 0)
            {
                IndexForwardLeftVector = 3;
                IndexBackLeftVector = 0;
                IndexForwardRightVector = 2;
                IndexBackRightVector = 1;
            }
            //Going Up
            else if (Speed.Y < 0)
            {
                IndexForwardLeftVector = 0;
                IndexBackLeftVector = 1;
                IndexForwardRightVector = 3;
                IndexBackRightVector = 2;
            }
            //Going Down
            else if (Speed.Y > 0)
            {
                IndexForwardLeftVector = 2;
                IndexBackLeftVector = 3;
                IndexForwardRightVector = 1;
                IndexBackRightVector = 0;
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            PolygonCollisionResult FinalCollisionResult;
            Polygon FinalCollisionPolygon;
            Polygon FinalOtherPolygon;

            HashSet<Player> SetOtherPlayer = Owner.GetCollidingPlayers(this);
            foreach (Player ActivePlayer in SetOtherPlayer)
            {
                if (ActivePlayer.Collision.CollideWith(CollisionBox, Speed, out FinalCollisionResult, out FinalCollisionPolygon, out FinalOtherPolygon))
                {
                    OnCollision(ActivePlayer, FinalCollisionResult, FinalCollisionPolygon, FinalOtherPolygon);
                }
            }

            bool CollisionWithWall = false;
            HashSet<WorldObject> SetWorldObject = Owner.GetCollidingWorldObjects(this);
            LinkedList<WorldObject> ListCollidingWorldObject = new LinkedList<WorldObject>();
            foreach (WorldObject ActiveWorldObject in SetWorldObject)
            {
                if (ActiveWorldObject.Collision.CollideWith(CollisionBox, Speed, out FinalCollisionResult, out FinalCollisionPolygon, out FinalOtherPolygon))
                {
                    ListCollidingWorldObject.AddLast(ActiveWorldObject);
                    OnCollision(ActiveWorldObject, FinalCollisionResult, FinalCollisionPolygon, FinalOtherPolygon);
                    CollisionWithWall = true;
                }
            }

            CreateFollowingExplosions(ListCollidingWorldObject);

            if (!CollisionWithWall)
            {
                Move(Speed);
            }
        }

        private void OnCollision(Player Other, PolygonCollisionResult FinalCollisionResult, Polygon FinalBombPolygon, Polygon FinalPlayerPolygon)
        {
        }

        private void OnCollision(WorldObject Other, PolygonCollisionResult FinalCollisionResult, Polygon FinalBombPolygon, Polygon FinalWorldPolygon)
        {
            //TODO: Allow explosions to split into multiple smaller explosion that goes through multiple holes.
            float DirectionAngle = (float)Math.Atan2(Speed.Y, Speed.X);
            float WallMinAngle = DirectionAngle - MathHelper.PiOver4;
            float WallMaxAngle = DirectionAngle + MathHelper.PiOver4;
            Vector2 WallAxis = new Vector2(-FinalCollisionResult.Axis.Y, FinalCollisionResult.Axis.X);
            double FinalCollisionResultAngle = Math.Atan2(WallAxis.X, WallAxis.Y);

            //Ground detection
            if (FinalCollisionResultAngle >= WallMinAngle && FinalCollisionResultAngle <= WallMaxAngle)
            {
            }
            //Slope
            else
            {
                if (FinalWorldPolygon.PolygonCollisionPerTriangle(ForwardLeftVector.X, ForwardLeftVector.Y))
                {
                    Vector2 PerpendicularAxis = BackLeftVector - ForwardLeftVector;
                    PerpendicularAxis = new Vector2(-PerpendicularAxis.Y, PerpendicularAxis.X);
                    PerpendicularAxis.Normalize();

                    Vector2 CollisionLineAxis;
                    Vector2 LeftCollisionPoint = Polygon.GetCollisionPointFromLine(BackLeftVector, ForwardLeftVector, Speed, FinalWorldPolygon, out CollisionLineAxis);
                    Move(LeftCollisionPoint - ForwardLeftVector);

                    //Collision on the left, the collision axis should divert to the right
                    if (Vector2.Dot(PerpendicularAxis, CollisionLineAxis) > 0)
                    {

                    }
                }
                else if (FinalWorldPolygon.PolygonCollisionPerTriangle(ForwardRightVector.X, ForwardRightVector.Y))
                {
                    Vector2 PerpendicularAxis = BackRightVector - ForwardRightVector;
                    PerpendicularAxis = new Vector2(-PerpendicularAxis.Y, PerpendicularAxis.X);
                    PerpendicularAxis.Normalize();

                    Vector2 CollisionLineAxis;
                    Vector2 RightCollisionPoint = Polygon.GetCollisionPointFromLine(BackRightVector, ForwardRightVector, Speed, FinalWorldPolygon, out CollisionLineAxis);
                    Move(RightCollisionPoint - ForwardRightVector);

                    //Collision on the right, the collision axis should divert to the left
                    if (Vector2.Dot(PerpendicularAxis, CollisionLineAxis) < 0)
                    {

                    }
                }
                else
                {
                    //Will happen if the collision is on a corner, consider it as a wall.
                }
            }

            Vector2 MovementCorection = FinalCollisionResult.Axis * FinalCollisionResult.Distance;
            Vector2 FinalMovement = Speed + MovementCorection;
            Speed.X = 0;
            Speed.Y = 0;

            Move(FinalMovement);
        }

        private void CreateFollowingExplosions(LinkedList<WorldObject> ListCollidingWorldObject)
        {
            //compute vertical lines to detect collisions

            int StartIndex = 0;
            int EndIndex = 0;

            Vector2 WallAxis;
            WorldObject CollidingObject;

            while (EndIndex < Size)
            {
                int HasCollision = CheckClosestCollisionType(ListCollidingWorldObject, ForwardLeftVector, EndIndex, out CollidingObject, out WallAxis);

                switch (HasCollision)
                {
                    case 0://no collision
                        FillEmptySpace(ListCollidingWorldObject, StartIndex, out EndIndex);
                        break;

                    case 1://Wall
                        SkipSolidSpace(ListCollidingWorldObject, StartIndex, CollidingObject, out EndIndex);
                        break;

                    case 2://Slope
                        CreateSlopeExplosion(ListCollidingWorldObject, StartIndex, CollidingObject, WallAxis, out EndIndex);
                        break;
                }

                StartIndex = EndIndex;
            }
        }

        private int CheckClosestCollisionType(LinkedList<WorldObject> ListCollidingWorldObject, Vector2 ForwardLeftVector, int EndIndex, out WorldObject CollidingObject, out Vector2 WallAxis)
        {
            CollidingObject = null;
            WallAxis = Vector2.Zero;
            Vector2 CollisionLineAxis;
            Vector2 HorizontalAxis = ForwardRightVector - ForwardLeftVector;
            HorizontalAxis.Normalize();
            Vector2 HorizontalTranslation = HorizontalAxis * EndIndex;

            foreach (WorldObject FinalWorldPolygon in ListCollidingWorldObject)
            {
                Vector2 NextCollisionPoint = Polygon.GetCollisionPointFromLine(
                BackLeftVector + HorizontalTranslation,
                ForwardLeftVector + HorizontalTranslation,
                Speed, FinalWorldPolygon.Collision.ListCollisionPolygon[0], out CollisionLineAxis);

                if (CollisionLineAxis != Vector2.Zero)
                {
                    float DirectionAngle = (float)Math.Atan2(Speed.Y, Speed.X);
                    float WallMinAngle = DirectionAngle - MathHelper.PiOver4;
                    float WallMaxAngle = DirectionAngle + MathHelper.PiOver4;
                    WallAxis = new Vector2(-CollisionLineAxis.Y, CollisionLineAxis.X);
                    double FinalCollisionResultAngle = Math.Atan2(WallAxis.Y, WallAxis.X);
                    CollidingObject = FinalWorldPolygon;

                    //Ground detection
                    if (FinalCollisionResultAngle >= WallMinAngle && FinalCollisionResultAngle <= WallMaxAngle)
                    {
                        return 1;
                    }
                    else//Slope
                    {
                        return 2;
                    }
                }
            }

            return 0;
        }

        private void CreateCollisionBox(int StartIndex, int EndIndex, Vector2 Axis, float Length)
        {
            Vector2 HorizontalAxis = ForwardRightVector - ForwardLeftVector;
            HorizontalAxis.Normalize();

            Vector2 StartingPoint = ForwardLeftVector + HorizontalAxis * StartIndex;
            Vector2[] NewCollisionBox = new Vector2[4];

            NewCollisionBox[IndexBackLeftVector] = StartingPoint;
            NewCollisionBox[IndexBackRightVector] = ForwardLeftVector + HorizontalAxis * EndIndex;

            NewCollisionBox[IndexForwardLeftVector] = NewCollisionBox[IndexForwardLeftVector] + Axis * Length;
            NewCollisionBox[IndexForwardRightVector] = NewCollisionBox[IndexForwardRightVector] + Axis * Length + HorizontalAxis * EndIndex;
        }

        private void FillEmptySpace(LinkedList<WorldObject> ListCollidingWorldObject, int StartIndex, out int EndIndex)
        {
            EndIndex = StartIndex;
            
            for (int i = StartIndex; i < Size; ++i)
            {
                int CollisionCheck = CheckClosestCollisionType(ListCollidingWorldObject, ForwardLeftVector, i, out _, out _);
                float DistanceToCollision = Speed.Length();//Use full length since no obstacles are blocking.

                switch (CollisionCheck)
                {
                    case 0://no collision, keep going.
                        break;

                    case 1://Wall
                        CreateCollisionBox(StartIndex, EndIndex, Vector2.Normalize(Speed), DistanceToCollision);
                        return;

                    case 2://Slope
                        CreateCollisionBox(StartIndex, EndIndex, Vector2.Normalize(Speed), DistanceToCollision);
                        return;
                }

                EndIndex = i;
            }
        }

        private void SkipSolidSpace(LinkedList<WorldObject> ListCollidingWorldObject, int StartIndex, WorldObject CollidingObject, out int EndIndex)
        {
            EndIndex = StartIndex;

            for (int i = StartIndex; i < Size; ++i)
            {
                WorldObject NextCollidingObject;
                int CollisionCheck = CheckClosestCollisionType(ListCollidingWorldObject, ForwardLeftVector, i, out NextCollidingObject, out _);
                float DistanceToCollision = 5;

                switch (CollisionCheck)
                {
                    case 0://no collision
                        CreateCollisionBox(StartIndex, EndIndex, Vector2.Normalize(Speed), DistanceToCollision);
                        return;

                    case 1://Wall, check if we hit a different wall object
                        if ( CollidingObject != NextCollidingObject)
                        {
                            CreateCollisionBox(StartIndex, EndIndex, Vector2.Normalize(Speed), DistanceToCollision);
                            return;
                        }
                        break;

                    case 2://Slope
                        CreateCollisionBox(StartIndex, EndIndex, Vector2.Normalize(Speed), DistanceToCollision);
                        return;
                }

                EndIndex = i;
            }
        }

        private void CreateSlopeExplosion(LinkedList<WorldObject> ListCollidingWorldObject, int StartIndex, WorldObject CollidingObject, Vector2 WallAxis, out int EndIndex)
        {
            EndIndex = StartIndex;

            for (int i = StartIndex; i < Size; ++i)
            {
                WorldObject NextCollidingObject;
                WorldObject NextCollidingSlopeObject;
                Vector2 NextWallAxis;
                Vector2 NextSlopeAxis;
                //Since explosion can follow slopes we need to cheat and extend the distance
                int CollisionCheck = CheckClosestCollisionType(ListCollidingWorldObject, ForwardLeftVector, i, out NextCollidingObject, out NextWallAxis);
                int CollisionCheckSlope = CheckClosestCollisionType(ListCollidingWorldObject, ForwardLeftVector + Speed * 1000, i, out NextCollidingSlopeObject, out NextSlopeAxis);
                float DistanceToCollision = 5;

                switch (CollisionCheck)
                {
                    case 0://no collision
                        if (CollisionCheckSlope == 2 && WallAxis == NextSlopeAxis && CollidingObject == NextCollidingSlopeObject)
                        {
                            break;
                        }
                        CreateCollisionBox(StartIndex, EndIndex, Vector2.Normalize(Speed), DistanceToCollision);
                        return;

                    case 1://Wall
                        CreateCollisionBox(StartIndex, EndIndex, Vector2.Normalize(Speed), DistanceToCollision);
                        return;

                    case 2://Slope, check if the slope angle changed otherwise keep going.
                        if (WallAxis != NextWallAxis || CollidingObject != NextCollidingObject)
                        {
                            CreateCollisionBox(StartIndex, EndIndex, Vector2.Normalize(Speed), DistanceToCollision);
                            return;
                        }
                        break;
                }

                EndIndex = i;
            }
        }

        private void Move(Vector2 Movement)
        {
            CollisionBox.Position += Movement / 2;
            CollisionBox.Radius += (Movement.X + Movement.Y) / 2;
            CollisionBox.ListCollisionPolygon[0].ArrayVertex[IndexForwardLeftVector] = CollisionBox.ListCollisionPolygon[0].ArrayVertex[IndexForwardLeftVector] + Movement;
            CollisionBox.ListCollisionPolygon[0].ArrayVertex[IndexForwardRightVector] = CollisionBox.ListCollisionPolygon[0].ArrayVertex[IndexForwardRightVector] + Movement;
        }

        public override void SetAngle(float Angle)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            throw new NotImplementedException();
        }
    }
}
