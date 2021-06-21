using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAttackPart2 : ActionPanelDeathmatch
    {
        private Squad ActiveSquad;
        private int ActivePlayerIndex;
        public List<Vector3> AttackChoice;
        private BattlePreviewer BattlePreview;

        public ActionPanelAttackPart2(DeathmatchMap Map, Squad ActiveSquad, int ActivePlayerIndex)
            : base("Attack2", Map)
        {
            this.ActiveSquad = ActiveSquad;
            this.ActivePlayerIndex = ActivePlayerIndex;
            BattlePreview = new BattlePreviewer(Map, ActiveSquad, ActiveSquad.CurrentLeader.CurrentAttack);
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
                                Map.ListPlayer[P].ListSquad[TargetSelect].CurrentMovement, ActiveSquad.CanMove);

                            if (!ActiveSquad.CurrentLeader.CurrentAttack.CanAttack)
                            {
                                Map.sndDeny.Play();
                                return;
                            }

                            Map.PrepareSquadsForBattle(ActiveSquad, Map.ListPlayer[P].ListSquad[TargetSelect]);

                            SupportSquadHolder ActiveSquadSupport = new SupportSquadHolder();
                            ActiveSquadSupport.PrepareAttackSupport(Map, ActivePlayerIndex, ActiveSquad, Map.ListPlayer[P].ListSquad[TargetSelect]);

                            SupportSquadHolder TargetSquadSupport = new SupportSquadHolder();
                            TargetSquadSupport.PrepareDefenceSupport(Map, P, Map.ListPlayer[P].ListSquad[TargetSelect]);

                            Map.ComputeTargetPlayerDefence(ActiveSquad, ActiveSquadSupport, ActivePlayerIndex, Map.ListPlayer[P].ListSquad[TargetSelect], TargetSquadSupport, P);

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
                    BattlePreview = new BattlePreviewer(Map, ActiveSquad, ActiveSquad.CurrentLeader.CurrentAttack);
                }
                BattlePreview.UpdateUnitDisplay();
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            BattlePreview.DrawDisplayUnit(g);
        }
    }
}
