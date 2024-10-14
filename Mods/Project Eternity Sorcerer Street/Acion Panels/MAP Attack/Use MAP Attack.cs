using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelUseMAPAttack : ActionPanelSorcererStreet
    {
        private const string PanelName = "UseMapAttack";

        private SorcererStreetUnit ActiveUnit;
        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private List<Vector3> ListMVHoverPoints;
        public List<MovementAlgorithmTile> ListAttackChoice;

        private WeaponMAPProperties WeaponMAPProperty;
        public MAPAttackAttributes MAPAttributes;

        public ActionPanelUseMAPAttack(SorcererStreetMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelUseMAPAttack(SorcererStreetMap Map, int ActivePlayerIndex, int ActiveSquadIndex, List<Vector3> ListMVHoverPoints, List<MovementAlgorithmTile> AttackChoice)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;
            this.ListAttackChoice = AttackChoice;

            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListCreatureOnBoard[ActiveSquadIndex];
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            switch (WeaponMAPProperty)
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
                    Map.SelectMAPEnemies(ActivePlayerIndex, ActiveSquadIndex, MAPAttributes, ListMVHoverPoints, ListAttackChoice);
                    Map.sndConfirm.Play();
                    break;

                case WeaponMAPProperties.Targeted:
                    if (ActiveInputManager.InputConfirmPressed())
                    {
                        Map.SelectMAPEnemies(ActivePlayerIndex, ActiveSquadIndex, MAPAttributes, ListMVHoverPoints, ListAttackChoice);
                        Map.sndConfirm.Play();
                    }
                    else
                    {
                        Map.CursorControl(ActiveInputManager);//Move the cursor
                    }
                    break;
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListCreatureOnBoard[ActiveSquadIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelUseMAPAttack(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

}
