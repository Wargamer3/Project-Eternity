using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelControlCharacter : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "ControlCharacter";

        private readonly PlayerCharacter ControlledCharacter;
        private PlayerCharacter HoverCharacter;
        private readonly List<MapInfo> ListVisisbleMap;

        private Camera3D Camera;
        private float Camera3DDistance = 255;
        private float Camera3DYawAngle = 11.5f;
        private float Camera3DPitchAngle = 45;

        public ActionPanelControlCharacter(PlayerOverseer Owner, PlayerCharacter ControlledCharacter, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, false)
        {
            this.ControlledCharacter = ControlledCharacter;

            Camera = new DefaultCamera(GameScreen.GraphicsDevice);
            ListVisisbleMap = new List<MapInfo>();
        }

        public override void OnSelect()
        {
            ListVisisbleMap.Add(MapManager.RootMapContainer);
            Owner.Camera = Camera;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            HoverCharacter = Owner.InvisibleCharacterAsCursor.SharedMapContex.ActiveMap.MoveCursorAndGetCharacterUnderMouse();

            if (ControlledCharacter.ListAutomatedAction.Count == 0)
            {
                Vector3 NextPosition = ControlledCharacter.Position;
                if (ActiveInputManager.InputLeftHold())
                {
                    NextPosition.X -= 1;
                }
                else if (ActiveInputManager.InputRightHold())
                {
                    NextPosition.X += 1;
                }
                if (ActiveInputManager.InputUpHold())
                {
                    NextPosition.Y -= 1;
                }
                else if (ActiveInputManager.InputDownHold())
                {
                    NextPosition.Y += 1;
                }

                ControlledCharacter.SetPosition(NextPosition);

                if (HoverCharacter != null && ActiveInputManager.InputConfirmPressed())
                {
                    HoverCharacter.SharedMapContex.User = Owner;
                    AddToPanelListAndSelect(new ActionPanelViewCharacter(Owner, HoverCharacter, MapManager, ListActionMenuChoice));
                    HoverCharacter = null;
                }
                else if (ActiveInputManager.InputConfirmPressed())
                {
                    AddToPanelListAndSelect(new ActionPanelAdminMenu(Owner, MapManager, ListActionMenuChoice));
                }
                else if (ActiveInputManager.InputCancelPressed())
                {
                    AddToPanelListAndSelect(new ActionPanelViewCharacterLogs(Owner, ControlledCharacter, MapManager, ListActionMenuChoice));
                    HoverCharacter = null;
                }
                else if (ActiveInputManager.InputLButtonPressed())//switch to next character
                {/*
                    PlayerCharacter CharacterToSelect = ControlledCharacter.SharedMapContex.ActiveMap.ListCharacter[0];
                    Owner.ActiveControlledCharacter.RemoveControl();
                    CharacterToSelect.TakeControl(Owner);*/
                }
            }

            SetTarget(new Vector3(ControlledCharacter.Position.X, ControlledCharacter.Position.Z, ControlledCharacter.Position.Y));
        }

        private void SetTarget(Vector3 Target)
        {
            float YawRotation = MathHelper.ToRadians(Camera3DYawAngle);
            float PitchRotation = MathHelper.ToRadians(Camera3DPitchAngle);
            float RollRotation = 0;

            Matrix FinalMatrix = Matrix.CreateTranslation(0, Camera3DDistance, 0) * Matrix.CreateFromYawPitchRoll(YawRotation, PitchRotation, RollRotation);
            Camera.CameraPosition3D = FinalMatrix.Translation + Target;

            Camera.View = Matrix.CreateLookAt(Camera.CameraPosition3D, Target, Vector3.Up);
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelControlCharacter(Owner, ControlledCharacter, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            foreach (MapInfo ActiveMap in ListVisisbleMap)
            {
                ActiveMap.MapMapContainer.ActiveMap.LayerManager.LayerHolderDrawable.SetCamera(Owner.Camera);
                ActiveMap.MapMapContainer.ActiveMap.Draw(g);
            }

            if (HoverCharacter != null)
            {
                ActionPanelViewCharacter.DrawPreview(g, HoverCharacter);
            }

            if (ControlledCharacter != null)
            {
                for (int A = 0; A < ControlledCharacter.ListAutomatedAction.Count; A++)
                {
                    ControlledCharacter.ListAutomatedAction[A].DrawIcon(g, new Vector2(A * 40, 10));
                }
            }
        }
    }
}
