using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAttackPER : ActionPanelDeathmatch
    {
        private const string PanelName = "AttackPER";

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private List<Vector3> ListMVHoverPoints;
        private Squad ActiveSquad;
        public List<MovementAlgorithmTile> ListAttackDirectionHelper;
        public List<MovementAlgorithmTile> ListAttackTerrain;
        private BattlePreviewer BattlePreview;

        public ActionPanelAttackPER(DeathmatchMap Map)
            : base(PanelName, Map)
        {
            ListAttackDirectionHelper = new List<MovementAlgorithmTile>();
        }

        public ActionPanelAttackPER(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;

            ListAttackDirectionHelper = new List<MovementAlgorithmTile>();

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            BattlePreview = new BattlePreviewer(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack);
        }

        public override void OnSelect()
        {
            if (ActiveSquad.CurrentLeader.CurrentAttack.PERAttributes.ProjectileSpeed == 0)
            {
                ListAttackTerrain = new List<MovementAlgorithmTile>();
                ListAttackTerrain.Add(Map.GetMovementTile((int)ActiveSquad.Position.X, (int)ActiveSquad.Position.Y, (int)ActiveSquad.Position.Z));
            }
            else
            {
                PERAttackPahtfinding Pathfinder = new PERAttackPahtfinding(Map, ActiveSquad);
                Pathfinder.ComputeAttackTargets();
                ListAttackTerrain = Pathfinder.ListUsableAttackTerrain;
            }
        }

        public void SetRobotContext(Squad ActiveRobotAnimation, Attack ActiveWeapon, Vector3 Angle, Vector3 Position)
        {
           Map.GlobalBattleParams.GlobalSquadContext.SetRobotContext(ActiveRobotAnimation, ActiveWeapon, Angle, Position);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.LayerManager.AddDrawablePoints(ListAttackTerrain, Color.FromNonPremultiplied(255, 0, 0, 190));
            Map.LayerManager.AddDrawablePath(ListAttackDirectionHelper);

            if (ActiveInputManager.InputConfirmPressed())
            {
                Terrain ActiveTerrain = Map.GetTerrain(ActiveSquad);
                Vector3 AttackPosition = new Vector3(ActiveTerrain.WorldPosition.X + 0.5f, ActiveTerrain.WorldPosition.Y + 0.5f, ActiveTerrain.LayerIndex);

                Terrain TargetTerrain = Map.GetTerrain(Map.CursorPosition);
                Vector3 TargetPosition = new Vector3(ActiveTerrain.WorldPosition.X + 0.5f, ActiveTerrain.WorldPosition.Y + 0.5f, ActiveTerrain.LayerIndex);

                if (ActiveSquad.CurrentLeader.CurrentAttack.PERAttributes.AttackType == PERAttackAttributes.AttackTypes.Shoot)
                {
                    AttackPosition.Z = ActiveTerrain.WorldPosition.Z + 0.5f;
                }

                SetRobotContext(ActiveSquad, ActiveSquad.CurrentLeader.CurrentAttack, Vector3.Normalize(TargetPosition - AttackPosition), AttackPosition);

                if (ActiveSquad.CurrentLeader.CurrentAttack.PERAttributes.HasSkills)
                {
                    ActiveSquad.CurrentLeader.CurrentAttack.PERAttributes.UpdateSkills(SquadPERRequirement.OnShoot);
                }
                else
                {
                    CreateAttack(Map, ActivePlayerIndex, ActiveSquad, ActiveSquad.CurrentLeader.CurrentAttack, AttackPosition, Map.CursorPosition - ActiveSquad.Position, new List<BaseAutomaticSkill>());
                }

                if (ActiveSquad.CurrentLeader.CurrentAttack.MaxAmmo > 0)
                {
                    ActiveSquad.CurrentLeader.CurrentAttack.ConsumeAmmo();
                }

                ActiveSquad.EndTurn();

                foreach (InteractiveProp ActiveProp in Map.LayerManager[(int)ActiveSquad.Position.Z].ListProp)
                {
                    ActiveProp.FinishMoving(ActiveSquad, ListMVHoverPoints);
                }

                RemoveAllSubActionPanels();

                Map.CursorPosition = ActiveSquad.Position;
                Map.CursorPositionVisible = ActiveSquad.Position;
                Map.sndConfirm.Play();
            }
            else
            {
                bool CursorMoved = Map.UpdateMapNavigation(ActiveInputManager);
                if (CursorMoved)
                {
                    BattlePreview = new BattlePreviewer(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack);
                    ListAttackDirectionHelper = PERAttack.PredictAttackMovement(Map, ActiveSquad.Position + new Vector3(0.5f, 0.5f, 0.5f), Map.CursorPosition + new Vector3(0.5f, 0.5f, 0.5f));
                }
                BattlePreview.UpdateUnitDisplay();
            }
        }

        public static List<PERAttack> CreateAttack(DeathmatchMap Map, int ActivePlayerIndex, Squad ActiveSquad, Attack AttackUsed, Vector3 AttackPosition, Vector3 AttackForwardVector, List<BaseAutomaticSkill> ListFollowingSkill)
        {
            List<PERAttack> ListNewList = new List<PERAttack>();

            for (int A = 0; A < AttackUsed.PERAttributes.NumberOfProjectiles; ++A)
            {
                PERAttack NewAttack = null;

                if (AttackUsed.PERAttributes.ProjectileSpeed == 0)
                {
                    NewAttack = new PERAttack(AttackUsed, ActiveSquad, ActivePlayerIndex, Map, AttackPosition, Vector3.Zero, AttackUsed.PERAttributes.MaxLifetime);
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

                    NewAttack = new PERAttack(AttackUsed, ActiveSquad, ActivePlayerIndex, Map, AttackPosition, AttackSpeed, AttackUsed.PERAttributes.MaxLifetime);

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
            ActiveSquadIndex = BR.ReadInt32();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            string ActiveSquadAttackName = BR.ReadString();
            if (!string.IsNullOrEmpty(ActiveSquadAttackName))
            {
                foreach (Attack ActiveAttack in ActiveSquad.CurrentLeader.ListAttack)
                {
                    if (ActiveAttack.ItemName == ActiveSquadAttackName)
                    {
                        ActiveSquad.CurrentLeader.CurrentAttack = ActiveAttack;
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

            bool IsBattlePreviewOpen = BR.ReadBoolean();
            if (IsBattlePreviewOpen)
            {
                int PlayerIndex = BR.ReadInt32();
                int SquadIndex = BR.ReadInt32();
                BattlePreview = new BattlePreviewer(Map, PlayerIndex, SquadIndex, null);
            }

            Map.CursorPosition =  new Vector3(BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32());
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendString(ActiveSquad.CurrentLeader.ItemName);
            BW.AppendInt32(ListAttackTerrain.Count);

            for (int A = 0; A < ListAttackTerrain.Count; ++A)
            {
                BW.AppendInt32(ListAttackTerrain[A].InternalPosition.X);
                BW.AppendInt32(ListAttackTerrain[A].InternalPosition.Y);
                BW.AppendInt32(ListAttackTerrain[A].LayerIndex);
            }

            BW.AppendBoolean(BattlePreview != null);
            if (BattlePreview != null)
            {
                BW.AppendInt32(BattlePreview.PlayerIndex);
                BW.AppendInt32(BattlePreview.SquadIndex);
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
            BattlePreview.DrawDisplayUnit(g);
        }
    }
}
