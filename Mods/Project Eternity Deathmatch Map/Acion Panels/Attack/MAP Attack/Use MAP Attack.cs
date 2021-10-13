using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelUseMAPAttack : ActionPanelDeathmatch
    {
        private const string PanelName = "UseMapAttack";

        private Squad ActiveSquad;
        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Attack CurrentAttack;
        public List<Vector3> AttackChoice;
        private BattlePreviewer BattlePreview;

        public ActionPanelUseMAPAttack(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelUseMAPAttack(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, List<Vector3> AttackChoice)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.AttackChoice = AttackChoice;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            CurrentAttack = ActiveSquad.CurrentLeader.CurrentAttack;
            BattlePreview = new BattlePreviewer(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack);
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

            CurrentAttack = ActiveSquad.CurrentLeader.CurrentAttack;
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
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelUseMAPAttack(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            BattlePreview.DrawDisplayUnit(g);
        }
    }

}
