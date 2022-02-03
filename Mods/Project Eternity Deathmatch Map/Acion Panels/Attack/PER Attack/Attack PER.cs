using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Attacks;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAttackPER : ActionPanelDeathmatch
    {
        private const string PanelName = "AttackPER";

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Squad ActiveSquad;
        public List<Vector3> ListAttackChoice;
        public List<MovementAlgorithmTile> ListAttackDirectionHelper;
        public List<MovementAlgorithmTile> ListAttackTerrain;
        private BattlePreviewer BattlePreview;

        public ActionPanelAttackPER(DeathmatchMap Map)
            : base(PanelName, Map)
        {
            ListAttackDirectionHelper = new List<MovementAlgorithmTile>();
        }

        public ActionPanelAttackPER(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;

            ListAttackDirectionHelper = new List<MovementAlgorithmTile>();

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            BattlePreview = new BattlePreviewer(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack);
        }

        public override void OnSelect()
        {
            PERAttackPahtfinding Pathfinder = new PERAttackPahtfinding(Map, ActiveSquad);
            Pathfinder.ComputeAttackTargets();
            ListAttackTerrain = Pathfinder.ListUsableAttackTerrain;

            ListAttackChoice = new List<Vector3>();
            foreach (MovementAlgorithmTile ActiveTerrain in ListAttackTerrain)
            {
                ListAttackChoice.Add(new Vector3(ActiveTerrain.Position.X, ActiveTerrain.Position.Y, ActiveTerrain.LayerIndex));
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.LayerManager.AddDrawablePoints(ListAttackTerrain, Color.FromNonPremultiplied(255, 0, 0, 190));
            Map.LayerManager.AddDrawablePath(ListAttackDirectionHelper);

            if (ActiveInputManager.InputConfirmPressed())
            {
                CreateAttack(ActiveSquad.CurrentLeader.CurrentAttack);
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

        private void CreateAttack(Attack AttackUsed)
        {
            Terrain ActiveTerrain = Map.GetTerrain(ActiveSquad);

            for (int A = 0; A < AttackUsed.PERAttributes.NumberOfProjectiles; ++A)
            {
                float RandLateral = (float)RandomHelper.Random.NextDouble() * AttackUsed.PERAttributes.MaxLateralSpread;
                float RandForward = (float)RandomHelper.Random.NextDouble() * AttackUsed.PERAttributes.MaxForwardSpread;
                float RandUpward = (float)RandomHelper.Random.NextDouble() * AttackUsed.PERAttributes.MaxUpwardSpread;

                Vector3 AttackForwardVector = Map.CursorPosition - ActiveSquad.Position;
                AttackForwardVector.Normalize();
                Vector3 AttackLateralVector = new Vector3(AttackForwardVector.Y, -AttackForwardVector.X, AttackForwardVector.Z);

                Vector3 AttackSpeed = new Vector3();
                AttackSpeed -= AttackLateralVector * AttackUsed.PERAttributes.MaxLateralSpread / 2;
                AttackSpeed += AttackLateralVector * RandLateral * AttackUsed.PERAttributes.ProjectileSpeed;
                AttackSpeed += AttackForwardVector * RandForward * AttackUsed.PERAttributes.ProjectileSpeed;
                AttackSpeed += AttackForwardVector * AttackUsed.PERAttributes.ProjectileSpeed;

                Vector3 AttackPosition = new Vector3(ActiveSquad.Position.X + 0.5f, ActiveSquad.Position.Y + 0.5f, ActiveSquad.Position.Z + ActiveTerrain.Position.Z + 0.5f);
                Vector3 NextPosition = AttackPosition + AttackSpeed;

                PERAttack NewAttack = new PERAttack(AttackUsed, ActiveSquad, ActivePlayerIndex, Map, AttackPosition, AttackSpeed, AttackUsed.PERAttributes.MaxLifetime);

                Map.ListPERAttack.Add(NewAttack);


                if (AttackUsed.MaxAmmo > 0)
                {
                    AttackUsed.ConsumeAmmo();
                }
                RemoveAllSubActionPanels();
                Map.ListActionMenuChoice.Add(new ActionPanelUpdatePERAttacks(Map, NewAttack));
            }
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
            ListAttackChoice = new List<Vector3>(AttackChoiceCount);
            ListAttackTerrain = new List<MovementAlgorithmTile>(AttackChoiceCount);
            for (int A = 0; A < AttackChoiceCount; ++A)
            {
                Vector3 NewTerrain = new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadInt32());
                ListAttackChoice.Add(NewTerrain);
                ListAttackTerrain.Add(Map.GetTerrain(NewTerrain.X, NewTerrain.Y, (int)NewTerrain.Z));
            }

            bool IsBattlePreviewOpen = BR.ReadBoolean();
            if (IsBattlePreviewOpen)
            {
                int PlayerIndex = BR.ReadInt32();
                int SquadIndex = BR.ReadInt32();
                BattlePreview = new BattlePreviewer(Map, PlayerIndex, SquadIndex, null);
            }

            Map.CursorPosition = new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat());
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendString(ActiveSquad.CurrentLeader.ItemName);
            BW.AppendInt32(ListAttackChoice.Count);

            for (int A = 0; A < ListAttackChoice.Count; ++A)
            {
                BW.AppendFloat(ListAttackChoice[A].X);
                BW.AppendFloat(ListAttackChoice[A].Y);
                BW.AppendInt32((int)ListAttackChoice[A].Z);
            }

            BW.AppendBoolean(BattlePreview != null);
            if (BattlePreview != null)
            {
                BW.AppendInt32(BattlePreview.PlayerIndex);
                BW.AppendInt32(BattlePreview.SquadIndex);
            }

            BW.AppendFloat(Map.CursorPosition.X);
            BW.AppendFloat(Map.CursorPosition.Y);
            BW.AppendFloat(Map.CursorPosition.Z);
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
