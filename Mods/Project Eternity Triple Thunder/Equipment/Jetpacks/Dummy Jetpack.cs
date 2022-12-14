using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class DummyJetpack : JetpackBase
    {
        public DummyJetpack()
            : base(null, 100)
        {
        }

        public override void Move(MovementInputs MovementInput)
        {
        }

        public override void OnFall()
        {
        }

        public override void OnIdle()
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

        public override void Update(GameTime gameTime)
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
