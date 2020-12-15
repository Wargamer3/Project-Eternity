using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelUseMAPAttack : ActionPanelDeathmatch
    {
        private Squad ActiveSquad;
        private int ActivePlayerIndex;
        private List<Vector3> AttackChoice;
        private BattlePreviewer BattlePreview;

        public ActionPanelUseMAPAttack(DeathmatchMap Map, Squad ActiveSquad, int ActivePlayerIndex, List<Vector3> AttackChoice)
            : base("UseMapAttack", Map)
        {
            this.ActiveSquad = ActiveSquad;
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.AttackChoice = AttackChoice;
            BattlePreview = new BattlePreviewer(Map, ActiveSquad, ActiveSquad.CurrentLeader.CurrentAttack);
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            switch (ActiveSquad.CurrentLeader.CurrentAttack.MAPAttributes.Property)
            {
                case WeaponMAPProperties.Spread:
                    if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
                    {
                    }
                    else
                    {
                        Map.CursorControl();//Move the cursor
                    }
                    break;

                case WeaponMAPProperties.Direction:
                    Map.SelectMAPEnemies(ActiveSquad, ActivePlayerIndex, AttackChoice);
                    Map.sndConfirm.Play();
                    break;

                case WeaponMAPProperties.Targeted:
                    if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
                    {
                        Map.SelectMAPEnemies(ActiveSquad, ActivePlayerIndex, AttackChoice);
                        Map.sndConfirm.Play();
                    }
                    else
                    {
                        Map.CursorControl();//Move the cursor
                        BattlePreview.UpdateUnitDisplay();
                    }
                    break;
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            BattlePreview.DrawDisplayUnit(g);
        }
    }

}
