using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class EquipmentLoadout
    {
        private readonly UsableEquipment[] ArrayEquipment;
        
        public UsableEquipment EquipedEtc { get { return ArrayEquipment[0]; } }
        public UsableEquipment EquipedHead { get { return ArrayEquipment[1]; } }
        public ArmorBase EquipedArmor { get; private set; }
        public UsableEquipment EquipedWeaponOption { get { return ArrayEquipment[3]; } }
        public JetpackBase EquipedBooster { get; private set; }
        public UsableEquipment EquipedShoes { get { return ArrayEquipment[5]; } }

        public EquipmentLoadout()
        {
            ArrayEquipment = new UsableEquipment[0];
            EquipedBooster = new DummyJetpack();
        }

        public EquipmentLoadout(PlayerInventory Equipment, RobotAnimation Owner)
        {
            if (Equipment.EquipedArmor != null && Equipment.EquipedArmor.Name == "Armor 1")
            {
                EquipedArmor = new Armor1(Owner);
            }
            else
            {
                EquipedArmor = new DefaultArmor(Owner);
            }

            EquipedBooster = new JumpJetpack(Owner.PlayerSFXGenerator, Owner);

            ArrayEquipment = new UsableEquipment[6];
            ArrayEquipment[0] = new EmptyEquipment(Owner);
            ArrayEquipment[1] = new EmptyEquipment(Owner);
            ArrayEquipment[2] = EquipedArmor;
            ArrayEquipment[3] = new EmptyEquipment(Owner);
            ArrayEquipment[4] = EquipedBooster;
            //ArrayEquipment[5] = new LongJumpShoes(Owner);
            //ArrayEquipment[5] = new WallJumpShoes(Owner);
            ArrayEquipment[5] = new RegularShoes(Owner);
        }

        public void Update(GameTime gameTime)
        {
            foreach (UsableEquipment ActiveEquipment in ArrayEquipment)
            {
                ActiveEquipment.Update(gameTime);
            }
        }

        public void Move(MovementInputs MovementInput)
        {
            foreach (UsableEquipment ActiveEquipment in ArrayEquipment)
            {
                ActiveEquipment.Move(MovementInput);
            }
        }

        public void OnIdle()
        {
            foreach (UsableEquipment ActiveEquipment in ArrayEquipment)
            {
                ActiveEquipment.OnIdle();
            }
        }

        public void OnJetpackUse(GameTime gameTime)
        {
            foreach (UsableEquipment ActiveEquipment in ArrayEquipment)
            {
                ActiveEquipment.OnJetpackUse(gameTime);
            }
        }

        public void OnJetpackRest(GameTime gameTime)
        {
            foreach (UsableEquipment ActiveEquipment in ArrayEquipment)
            {
                ActiveEquipment.OnJetpackRest(gameTime);
            }
        }

        public void OnJump()
        {
            foreach (UsableEquipment ActiveEquipment in ArrayEquipment)
            {
                ActiveEquipment.OnJump();
            }
        }

        public void OnStopJump()
        {
            foreach (UsableEquipment ActiveEquipment in ArrayEquipment)
            {
                ActiveEquipment.OnStopJump();
            }
        }

        public void OnLand()
        {
            foreach (UsableEquipment ActiveEquipment in ArrayEquipment)
            {
                ActiveEquipment.OnLand();
            }
        }

        public void OnFall()
        {
            foreach (UsableEquipment ActiveEquipment in ArrayEquipment)
            {
                ActiveEquipment.OnFall();
            }
        }

        public void OnAnyCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListAllCollidingPolygon)
        {
            foreach (UsableEquipment ActiveEquipment in ArrayEquipment)
            {
                ActiveEquipment.OnAnyCollision(ListAllCollidingPolygon);
            }
        }

        public void OnFloorCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListFloorCollidingPolygon)
        {
            foreach (UsableEquipment ActiveEquipment in ArrayEquipment)
            {
                ActiveEquipment.OnFloorCollision(ListFloorCollidingPolygon);
            }
        }

        public void OnCeilingCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListCeilingCollidingPolygon)
        {
            foreach (UsableEquipment ActiveEquipment in ArrayEquipment)
            {
                ActiveEquipment.OnCeilingCollision(ListCeilingCollidingPolygon);
            }
        }

        public void OnWallCollision(List<Tuple<PolygonCollisionResult, Polygon>> ListWallCollidingPolygon)
        {
            foreach (UsableEquipment ActiveEquipment in ArrayEquipment)
            {
                ActiveEquipment.OnWallCollision(ListWallCollidingPolygon);
            }
        }
    }
}
