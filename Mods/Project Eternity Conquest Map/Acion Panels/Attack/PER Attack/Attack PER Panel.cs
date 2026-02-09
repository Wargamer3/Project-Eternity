using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelAttackPER : ActionPanelConquest
    {
        private const string PanelName = "AttackPER";

        private int ActivePlayerIndex;
        private int ActiveUnitIndex;
        private List<Vector3> ListMVHoverPoints;
        private UnitConquest ActiveUnit;
        public List<MovementAlgorithmTile> ListAttackDirectionHelper;
        public List<MovementAlgorithmTile> ListAttackTerrain;

        public ActionPanelAttackPER(ConquestMap Map)
            : base(PanelName, Map)
        {
            ListAttackDirectionHelper = new List<MovementAlgorithmTile>();
        }

        public ActionPanelAttackPER(ConquestMap Map, int ActivePlayerIndex, int ActiveUnitIndex, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveUnitIndex = ActiveUnitIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;

            ListAttackDirectionHelper = new List<MovementAlgorithmTile>();

            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
        }

        public override void OnSelect()
        {
            if (ActiveUnit.CurrentAttack.PERAttributes.ProjectileSpeed == 0)
            {
                ListAttackTerrain = new List<MovementAlgorithmTile>();
                ListAttackTerrain.Add(Map.GetMovementTile((int)ActiveUnit.Position.X, (int)ActiveUnit.Position.Y, (int)ActiveUnit.Position.Z));
            }
            else
            {
                PERAttackPahtfinding Pathfinder = new PERAttackPahtfinding(Map, ActiveUnit);
                Pathfinder.ComputeAttackTargets();
                ListAttackTerrain = Pathfinder.ListUsableAttackTerrain;
            }
        }

        public void SetRobotContext(UnitConquest ActiveRobotAnimation, Attack ActiveWeapon, Vector3 Angle, Vector3 Position)
        {
           Map.GlobalBattleParams.GlobalSquadContext.SetRobotContext(ActiveRobotAnimation, ActiveWeapon, Angle, Position);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.LayerManager.AddDrawablePoints(ListAttackTerrain, Color.FromNonPremultiplied(255, 0, 0, 190));
            Map.LayerManager.AddDrawablePath(ListAttackDirectionHelper);

            if (ActiveInputManager.InputConfirmPressed())
            {
                Terrain ActiveTerrain = Map.GetTerrain(ActiveUnit.Components);
                Vector3 AttackPosition = new Vector3(ActiveTerrain.WorldPosition.X + 0.5f, ActiveTerrain.WorldPosition.Y + 0.5f, ActiveTerrain.LayerIndex);

                Terrain TargetTerrain = Map.GetTerrain(Map.CursorPosition);
                Vector3 TargetPosition = new Vector3(ActiveTerrain.WorldPosition.X + 0.5f, ActiveTerrain.WorldPosition.Y + 0.5f, ActiveTerrain.LayerIndex);

                if (ActiveUnit.CurrentAttack.PERAttributes.AttackType == PERAttackAttributes.AttackTypes.Shoot)
                {
                    AttackPosition.Z = ActiveTerrain.WorldPosition.Z + 0.5f;
                }

                SetRobotContext(ActiveUnit, ActiveUnit.CurrentAttack, Vector3.Normalize(TargetPosition - AttackPosition), AttackPosition);

                if (ActiveUnit.CurrentAttack.PERAttributes.HasSkills)
                {
                    ActiveUnit.CurrentAttack.PERAttributes.UpdateSkills(SquadPERRequirement.OnShoot);
                }
                else
                {
                    CreateAttack(Map, ActivePlayerIndex, ActiveUnit, ActiveUnit.CurrentAttack, AttackPosition, Map.CursorPosition - ActiveUnit.Position, new List<BaseAutomaticSkill>());
                }

                if (ActiveUnit.CurrentAttack.MaxAmmo > 0)
                {
                    ActiveUnit.CurrentAttack.ConsumeAmmo();
                }

                ActiveUnit.EndTurn();

                foreach (InteractiveProp ActiveProp in Map.LayerManager[(int)ActiveUnit.Position.Z].ListProp)
                {
                    ActiveProp.FinishMoving(ActiveUnit, ActiveUnit.Components, ListMVHoverPoints);
                }

                RemoveAllSubActionPanels();

                Map.CursorPosition = ActiveUnit.Position;
                Map.CursorPositionVisible = ActiveUnit.Position;
                Map.sndConfirm.Play();
            }
            else
            {
                bool CursorMoved = Map.CursorControl(gameTime, ActiveInputManager);
                if (CursorMoved)
                {
                    ListAttackDirectionHelper = PERAttack.PredictAttackMovement(Map, ActiveUnit.Position + new Vector3(0.5f, 0.5f, 0.5f), Map.CursorPosition + new Vector3(0.5f, 0.5f, 0.5f));
                }
            }
        }

        public static List<PERAttack> CreateAttack(ConquestMap Map, int ActivePlayerIndex, UnitConquest ActiveUnit, Attack AttackUsed, Vector3 AttackPosition, Vector3 AttackForwardVector, List<BaseAutomaticSkill> ListFollowingSkill)
        {
            List<PERAttack> ListNewList = new List<PERAttack>();

            for (int A = 0; A < AttackUsed.PERAttributes.NumberOfProjectiles; ++A)
            {
                PERAttack NewAttack = null;

                if (AttackUsed.PERAttributes.ProjectileSpeed == 0)
                {
                    NewAttack = new PERAttack(AttackUsed, ActiveUnit, ActivePlayerIndex, Map, AttackPosition, Vector3.Zero, AttackUsed.PERAttributes.MaxLifetime);
                    NewAttack.IsOnGround = true;
                }
                else
                {
                    float RandLateral = (float)RandomHelper.Random.NextDouble() * AttackUsed.PERAttributes.MaxLateralSpread;
                    float RandForward = (float)RandomHelper.Random.NextDouble() * AttackUsed.PERAttributes.MaxForwardSpread;
                    float RandUpward = (float)RandomHelper.Random.NextDouble() * AttackUsed.PERAttributes.MaxUpwardSpread;

                    AttackForwardVector.Normalize();
                    Vector3 AttackLateralVector = new Vector3(AttackForwardVector.Y, -AttackForwardVector.X, AttackForwardVector.Z);

                    Vector3 AttackSpeed = new Vector3();
                    AttackSpeed -= AttackLateralVector * AttackUsed.PERAttributes.MaxLateralSpread / 2;
                    AttackSpeed += AttackLateralVector * RandLateral * AttackUsed.PERAttributes.MaxLateralSpread;
                    AttackSpeed += AttackForwardVector * RandForward * AttackUsed.PERAttributes.ProjectileSpeed;
                    AttackSpeed += AttackForwardVector * AttackUsed.PERAttributes.ProjectileSpeed;

                    NewAttack = new PERAttack(AttackUsed, ActiveUnit, ActivePlayerIndex, Map, AttackPosition, AttackSpeed, AttackUsed.PERAttributes.MaxLifetime);

                    ListNewList.Add(NewAttack);
                }

                //Clone the following skills so they are not share by every bullets.
                NewAttack.ListActiveSkill = new List<BaseAutomaticSkill>(ListFollowingSkill.Count);
                foreach (BaseAutomaticSkill ActiveFollowingSkill in ListFollowingSkill)
                {
                    NewAttack.ListActiveSkill.Add(new BaseAutomaticSkill(ActiveFollowingSkill));
                }

                Map.ListPERAttack.Add(NewAttack);
            }

            ActionPanelUpdatePERAttacks ExistingPERAttackPanel = Map.ListActionMenuChoice.Last() as ActionPanelUpdatePERAttacks;
            if (ExistingPERAttackPanel != null)
            {
                ExistingPERAttackPanel.Add(ListNewList);
            }
            else
            {
                Map.ListActionMenuChoice.Add(new ActionPanelUpdatePERAttacks(Map, ListNewList));
            }

            return ListNewList;
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveUnitIndex = BR.ReadInt32();
            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
            string ActiveUnitAttackName = BR.ReadString();
            if (!string.IsNullOrEmpty(ActiveUnitAttackName))
            {
                foreach (Attack ActiveAttack in ActiveUnit.ListAttack)
                {
                    if (ActiveAttack.ItemName == ActiveUnitAttackName)
                    {
                        ActiveUnit.CurrentAttack = ActiveAttack;
                        break;
                    }
                }
            }
            int AttackChoiceCount = BR.ReadInt32();
            ListAttackTerrain = new List<MovementAlgorithmTile>(AttackChoiceCount);
            for (int A = 0; A < AttackChoiceCount; ++A)
            {
                ListAttackTerrain.Add(Map.GetTerrain(new Vector3(BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32())));
            }

            Map.CursorPosition =  new Vector3(BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32());
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveUnitIndex);
            BW.AppendString(ActiveUnit.ItemName);
            BW.AppendInt32(ListAttackTerrain.Count);

            for (int A = 0; A < ListAttackTerrain.Count; ++A)
            {
                BW.AppendInt32(ListAttackTerrain[A].GridPosition.X);
                BW.AppendInt32(ListAttackTerrain[A].GridPosition.Y);
                BW.AppendInt32(ListAttackTerrain[A].LayerIndex);
            }

            BW.AppendInt32((int)Map.CursorPosition.X);
            BW.AppendInt32((int)Map.CursorPosition.Y);
            BW.AppendInt32((int)Map.CursorPosition.Z);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAttackPER(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
