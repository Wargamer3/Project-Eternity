using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.RacingScreen
{
    public class ActionPanelAIFirstAction : ActionPanelRacing
    {
        public ActionPanelAIFirstAction(Vehicule ActiveVehicule)
            : base("AI First Action", ActiveVehicule, false)
        {
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            ActiveVehicule.CurrentAITunnel = ActiveVehicule.GetTunnelsInCollision()[0];
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelFollowAITunnel(ActiveVehicule));
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelFollowAITunnel : ActionPanelRacing
    {
        public ActionPanelFollowAITunnel(Vehicule ActiveVehicule)
            : base("Follow AI Tunnel", ActiveVehicule, false)
        {
        }

        public override void OnSelect()
        {
            PickNextTunnel();
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ActiveVehicule.CollideWith(ActiveVehicule.NextAITunnel))
            {
                EnterNextTunnel();
            }

            if (IsOutOfBound())
            {
                if (ActiveVehicule.IsBehindTunnel())
                {
                    ActiveVehicule.Accelerate(gameTime);
                    MoveTowardNextAITunnelEntryPoint(gameTime);
                }
                else if (ActiveVehicule.IsInFrontOfTunnel())
                {
                    EnterNextTunnel();
                }
                else
                {
                    float HorizontalDistance = Math.Sign(ActiveVehicule.GetHorizontalDistanceToAITunnel(ActiveVehicule.CurrentAITunnel));
                    if (HorizontalDistance == 0)
                    {
                        ActiveVehicule.Accelerate(gameTime);
                    }
                    else
                    {
                        ActiveVehicule.PrepareBoost(gameTime);
                        ActiveVehicule.Turn(gameTime, HorizontalDistance);
                    }
                }
            }
            else
            {
                if (ActiveVehicule.IsWrongWay())
                {
                    float HorizontalDistance = Math.Sign(ActiveVehicule.GetHorizontalDistanceToEntryPointOfAITunnel(ActiveVehicule.NextAITunnel));
                    ActiveVehicule.PrepareBoost(gameTime);
                    if (HorizontalDistance != 0)
                    {
                        ActiveVehicule.Turn(gameTime, HorizontalDistance);
                    }
                    else
                    {
                        ActiveVehicule.Turn(gameTime, 1f);
                    }
                }
                else
                {
                    FollowCurrentAITunnel(gameTime);
                }
            }
        }

        private void PickNextTunnel()
        {
            List<AITunnel> ListNextTunnel = ActiveVehicule.GetNextAITunnels();
            ActiveVehicule.NextAITunnel = ListNextTunnel[0];
        }

        private void EnterNextTunnel()
        {
            ActiveVehicule.CurrentAITunnel = ActiveVehicule.NextAITunnel;
            ActiveVehicule.CurrentAITunnel.Select();
            ActiveVehicule.NextAITunnel = null;

            PickNextTunnel();
        }

        private bool CanEnterNextAITunnel()
        {
            float HorizontalDistance = ActiveVehicule.GetHorizontalDistanceToEntryPointOfAITunnel(ActiveVehicule.NextAITunnel);
            return HorizontalDistance == 0;
        }

        private void MoveTowardNextAITunnelEntryPoint(GameTime gameTime)
        {
            float HorizontalDistance = Math.Sign(ActiveVehicule.GetHorizontalDistanceToEntryPointOfAITunnel(ActiveVehicule.NextAITunnel));
            ActiveVehicule.PrepareBoost(gameTime);
            ActiveVehicule.Turn(gameTime, HorizontalDistance);
        }

        private bool IsOutOfBound()
        {
            return !ActiveVehicule.CollideWith(ActiveVehicule.CurrentAITunnel);
        }

        private void FollowCurrentAITunnel(GameTime gameTime)
        {
            bool NeedToTurnRight;
            Vehicule.TunnelChangePredictionResults Prediction = ActiveVehicule.GetTunnelChangePrediction(ActiveVehicule.NextAITunnel, out NeedToTurnRight);

            if (Prediction == Vehicule.TunnelChangePredictionResults.Undershoot)
            {
                float HorizontalDistance = ActiveVehicule.GetDirectionFromInsideTunnel(ActiveVehicule.CurrentAITunnel);
                ActiveVehicule.Turn(gameTime, HorizontalDistance);

                float HorizontalDistance2 = Math.Sign(ActiveVehicule.GetDirectionFromInsideTunnel(ActiveVehicule.CurrentAITunnel));
                if (HorizontalDistance2 != 0 && HorizontalDistance != HorizontalDistance2)
                {
                    ActiveVehicule.AlignWithTunnel(ActiveVehicule.CurrentAITunnel);
                }
                ActiveVehicule.Accelerate(gameTime);
            }
            else if (Prediction == Vehicule.TunnelChangePredictionResults.Overshoot)
            {

                ActiveVehicule.PrepareBoost(gameTime);
                ActiveVehicule.Turn(gameTime, NeedToTurnRight ? -1 : 1);
            }
            else if (Prediction == Vehicule.TunnelChangePredictionResults.Turn)
            {
                ActiveVehicule.Accelerate(gameTime);
                ActiveVehicule.Turn(gameTime, NeedToTurnRight ? -1 : 1);
            }
            else if (Prediction == Vehicule.TunnelChangePredictionResults.Aligned)
            {
                ActiveVehicule.Accelerate(gameTime);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
