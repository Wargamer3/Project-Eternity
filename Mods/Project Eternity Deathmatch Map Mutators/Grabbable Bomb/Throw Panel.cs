using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.Item.ParticleSystem;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelThrowBomb : ActionPanelDeathmatch
    {
        private const string PanelName = "Throw";

        private static ArcBeamHelper ArcBeamEffect;

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private PERAttack AttackToThrow;
        private Squad ActiveSquad;
        private List<MovementAlgorithmTile> ListThrowLocation;
        private int MaxThrowDistance = 5;
        float Gravity = 1;

        public ActionPanelThrowBomb(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelThrowBomb(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, PERAttack AttackToThrow)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.AttackToThrow = AttackToThrow;

            ListThrowLocation = new List<MovementAlgorithmTile>();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
        }

        public override void OnSelect()
        {
            ListThrowLocation = Map.GetAttackChoice(ActiveSquad, MaxThrowDistance);
            if (ArcBeamEffect == null)
            {
                ArcBeamEffect = new ArcBeamHelper();
                ArcBeamEffect.InitBillboard(GameScreen.GraphicsDevice, Map.Content);
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.LayerManager.AddDrawablePoints(ListThrowLocation, Color.FromNonPremultiplied(255, 0, 0, 190));
            if (ActiveInputManager.InputConfirmPressed())
            {
                int TargetSelect = 0;
                //Verify if the cursor is over one of the possible position.
                while ((Map.CursorPosition.X != ListThrowLocation[TargetSelect].GridPosition.X || Map.CursorPosition.Y != ListThrowLocation[TargetSelect].GridPosition.Y)
                    && ++TargetSelect < ListThrowLocation.Count) ;

                //If nothing was found.
                if (TargetSelect >= ListThrowLocation.Count)
                    return;

                List<PERAttack> ListAttackToUpdate = new List<PERAttack>();
                Vector3 StartPosition = ActiveSquad.Position;
                AttackToThrow.SetPosition(StartPosition);
                AttackToThrow.Speed = new Vector3(ArcBeamEffect.HighAngleSpeed.X, ArcBeamEffect.HighAngleSpeed.Z, ArcBeamEffect.HighAngleSpeed.Y);
                AttackToThrow.IsOnGround = false;
                ListAttackToUpdate.Add(AttackToThrow);
                RemoveAllSubActionPanels();
                Map.ListActionMenuChoice.Add(new ActionPanelUpdatePERAttacks(Map, ListAttackToUpdate));
            }
            else
            {
                if (Map.CursorControl(ActiveInputManager))
                {
                    Vector3 StartPosition = ActiveSquad.Position;
                    Vector3 TargetPosition = Map.CursorPosition;
                    ArcBeamEffect.UpdateList(StartPosition, TargetPosition, 13, Gravity);
                }
            }
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelThrowBomb(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            Vector3 BeamOrigin = new Vector3(ActiveSquad.Position.X + 0.5f, ActiveSquad.Position.Z + 0.5f, ActiveSquad.Position.Y + 0.5f) * 32;
            ArcBeamEffect.DrawLine(g.GraphicsDevice, Map.Camera3D, BeamOrigin);
        }
    }
}
