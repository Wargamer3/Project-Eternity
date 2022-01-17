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
        public List<MovementAlgorithmTile> ListAttackTerrain;
        private BattlePreviewer BattlePreview;

        public ActionPanelAttackPER(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackPER(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;

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
                }
                BattlePreview.UpdateUnitDisplay();
            }
        }

        private void CreateAttack(Attack AttackUsed)
        {
            for (int A = 0; A < AttackUsed.PERAttributes.NumberOfProjectiles; ++A)
            {
                float RandLateral = (float)RandomHelper.Random.NextDouble() * AttackUsed.PERAttributes.MaxLateralSpread;
                float RandForward = (float)RandomHelper.Random.NextDouble() * AttackUsed.PERAttributes.MaxForwardSpread;
                float RandUpward = (float)RandomHelper.Random.NextDouble() * AttackUsed.PERAttributes.MaxUpwardSpread;

                Vector3 AttackForwardVector = Map.CursorPosition - ActiveSquad.Position;
                AttackForwardVector.Normalize();
                Vector3 AttackLateralVector = new Vector3(AttackForwardVector.Y, -AttackForwardVector.X, AttackForwardVector.Z);

                Vector3 AttackSpeed = AttackLateralVector * RandLateral + AttackForwardVector * RandForward * AttackUsed.PERAttributes.ProjectileSpeed;

                RandLateral = (float)RandomHelper.Random.NextDouble() * AttackUsed.PERAttributes.MaxLateralSpread;
                RandForward = (float)RandomHelper.Random.NextDouble() * AttackUsed.PERAttributes.MaxForwardSpread;
                RandUpward = (float)RandomHelper.Random.NextDouble() * AttackUsed.PERAttributes.MaxUpwardSpread;

                Vector3 AttackPosition = new Vector3(ActiveSquad.Position.X, ActiveSquad.Position.Y, ActiveSquad.Position.Z);
                AttackPosition -= AttackLateralVector * AttackUsed.PERAttributes.MaxLateralSpread / 2;
                AttackPosition += AttackForwardVector * RandForward;
                AttackPosition += AttackLateralVector * RandLateral + AttackForwardVector * RandForward * AttackUsed.PERAttributes.ProjectileSpeed;
                Terrain ActiveTerrain = Map.GetTerrain(ActiveSquad);
                AttackPosition.Z += ActiveTerrain.Position.Z;

                Map.ListPERAttack.Add(new PERAttack(AttackUsed, ActiveSquad, ActivePlayerIndex, Map.Content, AttackPosition, AttackSpeed, AttackUsed.PERAttributes.MaxLifetime));
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            ActiveSquad.CurrentLeader.AttackIndex = BR.ReadInt32();
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
            BW.AppendInt32(ActiveSquad.CurrentLeader.AttackIndex);
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
