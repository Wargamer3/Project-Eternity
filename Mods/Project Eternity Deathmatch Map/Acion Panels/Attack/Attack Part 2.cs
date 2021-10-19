using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAttackPart2 : ActionPanelDeathmatch
    {
        private const string PanelName = "Attack2";

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Squad ActiveSquad;
        public List<Vector3> AttackChoice;
        private BattlePreviewer BattlePreview;

        public ActionPanelAttackPart2(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackPart2(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            BattlePreview = new BattlePreviewer(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack);
        }

        public override void OnSelect()
        {
            AttackChoice = Map.GetAttackChoice(ActiveSquad.CurrentLeader, ActiveSquad.Position);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.ListLayer[Map.ActiveLayerIndex].LayerGrid.AddDrawablePoints(AttackChoice, Color.FromNonPremultiplied(255, 0, 0, 190));

            if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {
                int TargetSelect = 0;
                //Verify if the cursor is over one of the possible MV position.
                while ((Map.CursorPosition.X != AttackChoice[TargetSelect].X || Map.CursorPosition.Y != AttackChoice[TargetSelect].Y)
                    && ++TargetSelect < AttackChoice.Count) ;
                //If nothing was found.
                if (TargetSelect >= AttackChoice.Count)
                    return;

                Map.TargetSquadIndex = -1;

                for (int P = 0; P < Map.ListPlayer.Count; P++)
                {
                    //Find if a Unit is under the cursor.
                    TargetSelect = Map.CheckForSquadAtPosition(P, Map.CursorPosition, Vector3.Zero);
                    //If one was found.
                    if (TargetSelect >= 0)
                    {
                        if (Map.ListPlayer[ActivePlayerIndex].Team != Map.ListPlayer[P].Team)//If it's an ennemy.
                        {
                            ActiveSquad.CurrentLeader.CurrentAttack.UpdateAttack(ActiveSquad.CurrentLeader, ActiveSquad.Position, Map.CursorPosition,
                                ActiveSquad.ArrayMapSize, Map.ListPlayer[P].ListSquad[TargetSelect].CurrentMovement, ActiveSquad.CanMove);

                            if (!ActiveSquad.CurrentLeader.CurrentAttack.CanAttack)
                            {
                                Map.sndDeny.Play();
                                return;
                            }

                            Map.PrepareSquadsForBattle(ActivePlayerIndex, ActiveSquadIndex,P, TargetSelect);

                            SupportSquadHolder ActiveSquadSupport = new SupportSquadHolder();
                            ActiveSquadSupport.PrepareAttackSupport(Map, ActivePlayerIndex, ActiveSquad, P, TargetSelect);

                            SupportSquadHolder TargetSquadSupport = new SupportSquadHolder();
                            TargetSquadSupport.PrepareDefenceSupport(Map, P, Map.ListPlayer[P].ListSquad[TargetSelect]);

                            Map.ComputeTargetPlayerDefence(ActivePlayerIndex, ActiveSquadIndex, ActiveSquadSupport, P, TargetSelect, TargetSquadSupport);

                            break;
                        }
                    }
                }
                Map.sndConfirm.Play();
            }
            else
            {
                bool CursorMoved = Map.UpdateMapNavigation();
                if (CursorMoved)
                {
                    BattlePreview = new BattlePreviewer(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack);
                }
                BattlePreview.UpdateUnitDisplay();
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            ActiveSquad.CurrentLeader.AttackIndex = BR.ReadInt32();
            int AttackChoiceCount = BR.ReadInt32();
            AttackChoice = new List<Vector3>(AttackChoiceCount);
            for (int A = 0; A < AttackChoiceCount; ++A)
            {
                AttackChoice.Add(new Vector3(BR.ReadFloat(), BR.ReadFloat(), 0f));
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
            BW.AppendInt32(AttackChoice.Count);

            for (int A = 0; A < AttackChoice.Count; ++A)
            {
                BW.AppendFloat(AttackChoice[A].X);
                BW.AppendFloat(AttackChoice[A].Y);
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
            return new ActionPanelAttackPart2(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            BattlePreview.DrawDisplayUnit(g);
        }
    }
}
