using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.ControlHelper;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAttackMAPSpread : ActionPanelDeathmatch
    {
        private readonly Squad ActiveSquad;
        private int ActivePlayerIndex;
        private readonly Attack CurrentAttack;
        public List<Vector3> AttackChoice;
        private BattlePreviewer BattlePreview;

        public ActionPanelAttackMAPSpread(DeathmatchMap Map, Squad ActiveSquad, int ActivePlayerIndex)
            : base("Attack MAP Spread", Map)
        {
            this.ActiveSquad = ActiveSquad;
            this.ActivePlayerIndex = ActivePlayerIndex;
            CurrentAttack = ActiveSquad.CurrentLeader.CurrentAttack;
            BattlePreview = new BattlePreviewer(Map, ActiveSquad, ActiveSquad.CurrentLeader.CurrentAttack);
        }

        public override void OnSelect()
        {
            Map.BattleMenuOffenseFormationChoice = FormationChoices.ALL;

            AttackChoice = new List<Vector3>();
            for (int X = 0; X < CurrentAttack.MAPAttributes.ListChoice.Count; X++)
            {
                for (int Y = 0; Y < CurrentAttack.MAPAttributes.ListChoice[X].Count; Y++)
                {
                    if (CurrentAttack.MAPAttributes.ListChoice[X][Y])
                    {
                        AttackChoice.Add(new Vector3(Map.CursorPosition.X + X - CurrentAttack.MAPAttributes.Width,
                                               Map.CursorPosition.Y + Y - CurrentAttack.MAPAttributes.Height, Map.CursorPosition.Z));
                    }
                }
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {
                if (AttackChoice.Contains(Map.CursorPosition))
                {
                    Map.SelectMAPEnemies(ActiveSquad, ActivePlayerIndex, AttackChoice);
                    Map.sndConfirm.Play();
                }
                else
                {
                    Map.sndDeny.Play();
                }
            }
            else
            {
                Map.CursorControl();//Move the cursor
                BattlePreview.UpdateUnitDisplay();
            }

            Map.ListLayer[Map.ActiveLayerIndex].LayerGrid.AddDrawablePoints(AttackChoice, Color.FromNonPremultiplied(255, 0, 0, 190));
        }

        public override void Draw(CustomSpriteBatch g)
        {
            BattlePreview.DrawDisplayUnit(g);
        }
    }
}
