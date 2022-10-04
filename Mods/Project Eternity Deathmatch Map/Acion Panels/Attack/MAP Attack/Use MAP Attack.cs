using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelUseMAPAttack : ActionPanelDeathmatch
    {
        private const string PanelName = "UseMapAttack";

        private Squad ActiveSquad;
        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private List<Vector3> ListMVHoverPoints;
        private Attack CurrentAttack;
        public List<MovementAlgorithmTile> ListAttackChoice;
        private BattlePreviewer BattlePreview;

        public ActionPanelUseMAPAttack(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelUseMAPAttack(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, List<Vector3> ListMVHoverPoints, List<MovementAlgorithmTile> AttackChoice)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;
            this.ListAttackChoice = AttackChoice;

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
                    if (ActiveInputManager.InputConfirmPressed())
                    {
                    }
                    else
                    {
                        Map.CursorControl(ActiveInputManager);//Move the cursor
                    }
                    break;

                case WeaponMAPProperties.Direction:
                    Map.SelectMAPEnemies(ActivePlayerIndex, ActiveSquadIndex, ListMVHoverPoints, ListAttackChoice);
                    Map.sndConfirm.Play();
                    break;

                case WeaponMAPProperties.Targeted:
                    if (ActiveInputManager.InputConfirmPressed())
                    {
                        Map.SelectMAPEnemies(ActivePlayerIndex, ActiveSquadIndex, ListMVHoverPoints, ListAttackChoice);
                        Map.sndConfirm.Play();
                    }
                    else
                    {
                        Map.CursorControl(ActiveInputManager);//Move the cursor
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
            ListAttackChoice = new List<MovementAlgorithmTile>(AttackChoiceCount);
            for (int A = 0; A < AttackChoiceCount; ++A)
            {
                ListAttackChoice.Add(Map.GetTerrain(new Vector3(BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32())));
            }

            CurrentAttack = ActiveSquad.CurrentLeader.CurrentAttack;
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendString(ActiveSquad.CurrentLeader.ItemName);
            BW.AppendInt32(ListAttackChoice.Count);

            for (int A = 0; A < ListAttackChoice.Count; ++A)
            {
                BW.AppendFloat(ListAttackChoice[A].InternalPosition.X);
                BW.AppendFloat(ListAttackChoice[A].InternalPosition.Y);
                BW.AppendInt32(ListAttackChoice[A].LayerIndex);
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
