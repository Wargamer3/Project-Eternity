using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelAttackPER : ActionPanelSorcererStreet
    {
        private const string PanelName = "AttackPER";

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private PERAttackAttributes PERAttributes;
        private List<Vector3> ListMVHoverPoints;
        private SorcererStreetUnit ActiveSquad;
        public List<MovementAlgorithmTile> ListAttackDirectionHelper;
        public List<MovementAlgorithmTile> ListAttackTerrain;

        public ActionPanelAttackPER(SorcererStreetMap Map)
            : base(PanelName, Map)
        {
            ListAttackDirectionHelper = new List<MovementAlgorithmTile>();
        }

        public ActionPanelAttackPER(SorcererStreetMap Map, int ActivePlayerIndex, int ActiveSquadIndex, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;

            ListAttackDirectionHelper = new List<MovementAlgorithmTile>();

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListCreatureOnBoard[ActiveSquadIndex];
        }

        public override void OnSelect()
        {
            PERAttackPahtfinding Pathfinder = new PERAttackPahtfinding(Map, ActiveSquad);
            Pathfinder.ComputeAttackTargets();
            ListAttackTerrain = Pathfinder.ListUsableAttackTerrain;
        }

        public void SetRobotContext(SorcererStreetUnit ActiveRobotAnimation, Vector3 Angle, Vector3 Position)
        {
           Map.SorcererStreetParams.GlobalSquadContext.SetRobotContext(ActiveRobotAnimation, Angle, Position);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.LayerManager.AddDrawablePoints(ListAttackTerrain, Color.FromNonPremultiplied(255, 0, 0, 190));
            Map.LayerManager.AddDrawablePath(ListAttackDirectionHelper);

            if (ActiveInputManager.InputConfirmPressed())
            {
                Terrain ActiveTerrain = Map.GetTerrain(ActiveSquad.Position);
                Vector3 AttackPosition = new Vector3(ActiveTerrain.WorldPosition.X + 0.5f, ActiveTerrain.WorldPosition.Y + 0.5f, ActiveTerrain.LayerIndex);

                Terrain TargetTerrain = Map.GetTerrain(Map.CursorPosition);
                Vector3 TargetPosition = new Vector3(ActiveTerrain.WorldPosition.X + 0.5f, ActiveTerrain.WorldPosition.Y + 0.5f, ActiveTerrain.LayerIndex);

                if (PERAttributes.AttackType == PERAttackAttributes.AttackTypes.Shoot)
                {
                    AttackPosition.Z = ActiveTerrain.WorldPosition.Z + 0.5f;
                }

                SetRobotContext(ActiveSquad, Vector3.Normalize(TargetPosition - AttackPosition), AttackPosition);

                if (PERAttributes.HasSkills)
                {
                    PERAttributes.UpdateSkills(SquadPERRequirement.OnShoot);
                }
                else
                {
                    CreateAttack(Map, ActivePlayerIndex, ActiveSquad, PERAttributes, AttackPosition, Map.CursorPosition - ActiveSquad.Position, new List<BaseAutomaticSkill>());
                }

                ActiveSquad.EndTurn();

                foreach (InteractiveProp ActiveProp in Map.LayerManager[(int)ActiveSquad.Position.Z].ListProp)
                {
                    ActiveProp.FinishMoving(null, ActiveSquad, ListMVHoverPoints);
                }

                RemoveAllSubActionPanels();

                Map.CursorPosition = ActiveSquad.Position;
                Map.CursorPositionVisible = ActiveSquad.Position;
                Map.sndConfirm.Play();
            }
            else
            {
                /*bool CursorMoved = Map.UpdateMapNavigation(ActiveInputManager);
                if (CursorMoved)
                {
                    ListAttackDirectionHelper = PERAttack.PredictAttackMovement(Map, ActiveSquad.Position + new Vector3(0.5f, 0.5f, 0.5f), Map.CursorPosition + new Vector3(0.5f, 0.5f, 0.5f));
                }*/
            }
        }

        public static List<PERAttack> CreateAttack(SorcererStreetMap Map, int ActivePlayerIndex, SorcererStreetUnit ActiveSquad, PERAttackAttributes PERAttributes, Vector3 AttackPosition, Vector3 AttackForwardVector, List<BaseAutomaticSkill> ListFollowingSkill)
        {
            List<PERAttack> ListNewList = new List<PERAttack>();

            for (int A = 0; A < PERAttributes.NumberOfProjectiles; ++A)
            {
                PERAttack NewAttack = null;

                if (PERAttributes.ProjectileSpeed == 0)
                {
                    NewAttack = new PERAttack(ActiveSquad, ActivePlayerIndex, Map, AttackPosition, Vector3.Zero, PERAttributes.MaxLifetime);
                    NewAttack.IsOnGround = true;
                }
                else
                {
                    float RandLateral = (float)RandomHelper.Random.NextDouble() * PERAttributes.MaxLateralSpread;
                    float RandForward = (float)RandomHelper.Random.NextDouble() * PERAttributes.MaxForwardSpread;
                    float RandUpward = (float)RandomHelper.Random.NextDouble() * PERAttributes.MaxUpwardSpread;

                    AttackForwardVector.Normalize();
                    Vector3 AttackLateralVector = new Vector3(AttackForwardVector.Y, -AttackForwardVector.X, AttackForwardVector.Z);

                    Vector3 AttackSpeed = new Vector3();
                    AttackSpeed -= AttackLateralVector * PERAttributes.MaxLateralSpread / 2;
                    AttackSpeed += AttackLateralVector * RandLateral * PERAttributes.MaxLateralSpread;
                    AttackSpeed += AttackForwardVector * RandForward * PERAttributes.ProjectileSpeed;
                    AttackSpeed += AttackForwardVector * PERAttributes.ProjectileSpeed;

                    NewAttack = new PERAttack(ActiveSquad, ActivePlayerIndex, Map, AttackPosition, AttackSpeed, PERAttributes.MaxLifetime);

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
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListCreatureOnBoard[ActiveSquadIndex];
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
            }

            Map.CursorPosition =  new Vector3(BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32());
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
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
