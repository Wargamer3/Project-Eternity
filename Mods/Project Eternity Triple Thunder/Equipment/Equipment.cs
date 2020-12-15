using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public enum EquipmentTypes { Character, Booster, Armor, Head, Shoes, Etc, Weapon, WeaponOption, Grenade }

    public abstract class UsableEquipment
    {
        protected readonly RobotAnimation Owner;

        protected UsableEquipment(RobotAnimation Owner)
        {
            this.Owner = Owner;
        }

        public abstract void Update(GameTime gameTime);

        public abstract void Move(MovementInputs MovementInput);

        public abstract void OnIdle();

        public abstract void OnJetpackUse(GameTime gameTime);

        public abstract void OnJetpackRest(GameTime gameTime);

        public abstract void OnJump();

        public abstract void OnStopJump();

        public abstract void OnLand();

        public abstract void OnFall();

        public abstract void OnAnyCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListAllCollidingPolygon);

        public abstract void OnFloorCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListFloorCollidingPolygon);

        public abstract void OnCeilingCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListCeilingCollidingPolygon);

        public abstract void OnWallCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListWallCollidingPolygon);
    }
}
