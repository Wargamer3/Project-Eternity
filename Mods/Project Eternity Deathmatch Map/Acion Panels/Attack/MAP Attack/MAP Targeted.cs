﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAttackMAPTargeted : ActionPanelDeathmatch
    {
        private readonly Squad ActiveSquad;
        private readonly Attack CurrentAttack;
        public List<Vector3> AttackChoice;
        private BattlePreviewer BattlePreview;

        public ActionPanelAttackMAPTargeted(DeathmatchMap Map, Squad ActiveSquad)
            : base("Attack MAP Targeted", Map)
        {
            this.ActiveSquad = ActiveSquad;
            CurrentAttack = ActiveSquad.CurrentLeader.CurrentAttack;
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
