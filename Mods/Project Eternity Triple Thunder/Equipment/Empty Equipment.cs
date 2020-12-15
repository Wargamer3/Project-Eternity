using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class EmptyEquipment : UsableEquipment
    {
        public EmptyEquipment(RobotAnimation Owner)
            : base(Owner)
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void OnJump()
        {
        }

        public override void OnFall()
        {
        }

        public override void OnLand()
        {
        }

        public override void OnStopJump()
        {
        }

        public override void OnJetpackUse(GameTime gameTime)
        {
        }

        public override void OnJetpackRest(GameTime gameTime)
        {
        }

        public override void Move(MovementInputs MovementInput)
        {
        }

        public override void OnIdle()
        {
        }

        public override void OnAnyCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListAllCollidingPolygon)
        {
        }

        public override void OnFloorCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListFloorCollidingPolygon)
        {
        }

        public override void OnCeilingCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListCeilingCollidingPolygon)
        {
        }

        public override void OnWallCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListWallCollidingPolygon)
        {
        }
    }
}
