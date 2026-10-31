using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelTopDownControlCharacter : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "TopDownControlCharacter";

        private readonly PlayerCharacter ControlledCharacter;
        private PlayerCharacter HoverCharacter;
        private readonly List<MapInfo> ListVisisbleMap;

        private Camera3D Camera;
        private float Camera3DDistance = 255;
        private float Camera3DYawAngle = 11.5f;
        private float Camera3DPitchAngle = 45;

        public ActionPanelTopDownControlCharacter(PlayerOverseer Owner, PlayerCharacter ControlledCharacter, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
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

            if (ControlledCharacter.ListCurrentAIObjectiveStep.Count == 0)
            {
                if (Owner.Session != null)
                {
                    Owner.Session.WatchFight(ControlledCharacter);
                }
            }

            SetTarget(ChunkOrigin.Camera(ControlledCharacter.SharedMapContex == null ? null : ControlledCharacter.SharedMapContex.CurrentMapInfo, new Vector3(ControlledCharacter.Position.X, ControlledCharacter.Position.Z, ControlledCharacter.Position.Y)));
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
            return new ActionPanelTopDownControlCharacter(Owner, ControlledCharacter, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            PlayerCharacter Cursor = Owner.InvisibleCharacterAsCursor;
            MapInfo Here = Cursor != null && Cursor.SharedMapContex != null ? Cursor.SharedMapContex.CurrentMapInfo : null;
            MapManager.DrawChunks(g, Owner.Camera, delegate (LifeSimMap Map)
            {
                Map.ShowCursor = false;
                if (Map.BuildPlacing)
                {
                    return;
                }

                Map.HideFloorsAbove = true;
                Map.ShownFloor = ControlledCharacter == null ? 0 : BuildFloor.Index(ControlledCharacter.Position.Z);
                Map.ShowCursor = Here != null && (object.ReferenceEquals(Map.Chunk, Here) || (Here.MapMapContainer != null && object.ReferenceEquals(Here.MapMapContainer.ActiveMap, Map)));
            });

            MapManager.BeginOverlay(g);
            if (HoverCharacter != null)
            {
                ActionPanelViewCharacter.DrawPreview(g, HoverCharacter);
            }

            if (ControlledCharacter != null)
            {
                for (int A = 0; A < ControlledCharacter.ListCurrentAIObjectiveStep.Count; A++)
                {
                    ControlledCharacter.ListCurrentAIObjectiveStep[A].DrawIcon(g, new Vector2(A * 40, 10));
                }
            }
        }
    }
}
