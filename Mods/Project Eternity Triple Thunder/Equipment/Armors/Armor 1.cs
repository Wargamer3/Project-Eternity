using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    class Armor1 : ArmorBase
    {
        public Armor1(RobotAnimation Owner)
            : base(Owner, 10)
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void OnFall()
        {
        }

        public override void OnJetpackRest(GameTime gameTime)
        {
        }

        public override void OnJetpackUse(GameTime gameTime)
        {
        }

        public override void OnJump()
        {
        }

        public override void OnLand()
        {
        }

        public override void OnStopJump()
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
