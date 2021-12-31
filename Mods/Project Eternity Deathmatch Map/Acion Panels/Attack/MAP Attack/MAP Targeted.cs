using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAttackMAPTargeted : ActionPanelDeathmatch
    {
        private const string PanelName = "AttackMAPTargeted";

        private Squad ActiveSquad;
        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Attack CurrentAttack;
        public List<Vector3> AttackChoice;
        private BattlePreviewer BattlePreview;

        public ActionPanelAttackMAPTargeted(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackMAPTargeted(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            AttackChoice = new List<Vector3>();

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            CurrentAttack = ActiveSquad.CurrentLeader.CurrentAttack;
            BattlePreview = new BattlePreviewer(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack);
        }

        public override void OnSelect()
        {
            AttackChoice = Map.GetAttackChoice(ActiveSquad.CurrentLeader, ActiveSquad.Position);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.ListLayer[(int)ActiveSquad.Position.Z].LayerGrid.AddDrawablePoints(AttackChoice, Color.FromNonPremultiplied(255, 0, 0, 190));

            if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {
                if (AttackChoice.Contains(Map.CursorPosition))
                {
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
            return new ActionPanelAttackMAPTargeted(Map);
        }
        public override void Draw(CustomSpriteBatch g)
        {
            for (int X = 0; X < CurrentAttack.MAPAttributes.ListChoice.Count; X++)
            {
                for (int Y = 0; Y < CurrentAttack.MAPAttributes.ListChoice[X].Count; Y++)
                {
                    if (CurrentAttack.MAPAttributes.ListChoice[X][Y])
                    {
                        g.Draw(GameScreen.sprPixel, new Rectangle((int)(Map.CursorPosition.X + X - Map.CameraPosition.X - CurrentAttack.MAPAttributes.Width) * Map.TileSize.X,
                                                         (int)(Map.CursorPosition.Y + Y - Map.CameraPosition.Y - CurrentAttack.MAPAttributes.Height) * Map.TileSize.Y,
                                                         Map.TileSize.X, Map.TileSize.Y), Color.DarkRed);
                    }
                }
            }

            BattlePreview.DrawDisplayUnit(g);
        }
    }
}
