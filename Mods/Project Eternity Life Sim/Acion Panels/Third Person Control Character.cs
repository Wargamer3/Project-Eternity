using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelThirdPersonControlCharacter : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "ThirdPersonControlCharacter";

        private readonly ThirdPersonRig Rig = new ThirdPersonRig();
        private Vector3 CameraOffset;
        private Vector3 TargetOffset;
        private Camera3D Camera;

        private readonly PlayerCharacter ControlledCharacter;
        private readonly List<MapInfo> ListVisisbleMap;

        private PlayerCharacter HoverCharacter;

        public ActionPanelThirdPersonControlCharacter(PlayerOverseer Owner, PlayerCharacter ControlledCharacter, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, false)
        {
            this.ControlledCharacter = ControlledCharacter;

            Camera = new DefaultCamera(GameScreen.GraphicsDevice);
            ListVisisbleMap = new List<MapInfo>();
            Rig.Yaw = ThirdPersonRig.YawOf(ControlledCharacter.RotationMatrix);

            CameraOffset = new Vector3(0, -10f, 100f);
            TargetOffset = new Vector3(0, 5, 0);
        }

        public override void OnSelect()
        {
            ListVisisbleMap.Add(MapManager.RootMapContainer);
            Owner.Camera = Camera;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (Owner.Session != null)
            {
                ReadPlayKeys();
                Owner.Session.WatchFight(ControlledCharacter);
                if (Owner.Session.Fighting && Owner.Session.Controlled != null)
                {
                    List<Point> Walls = WallCells(ControlledCharacter);
                    LifeSimMap Map = CurrentMap();
                    MapInfo Stood = ControlledCharacter.SharedMapContex == null ? null : ControlledCharacter.SharedMapContex.CurrentMapInfo;
                    IList<PlayerCharacter> Seen = Stood != null && Stood.People != null ? Stood.People : (Map == null ? null : Map.ListCharacter);
                    if (Seen != null)
                    {
                        BattleSight.Apply(Owner.Session.Controlled, Seen, Walls, Map != null ? Map.TileSize.X : 8f, Facing(ControlledCharacter));
                    }
                }
            }

            HoverCharacter = null;

            if (Owner.Session != null && Owner.Session.InDungeon)
            {
                float DX = 0f;
                float DY = 0f;
                Microsoft.Xna.Framework.Input.KeyboardState Held = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                if (ActiveInputManager.InputRightHold() || Held.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) || Held.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                {
                    DX = 1f;
                }
                else if (ActiveInputManager.InputLeftHold() || Held.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left) || Held.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
                {
                    DX = -1f;
                }

                if (ActiveInputManager.InputUpHold() || Held.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up) || Held.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
                {
                    DY = -1f;
                }
                else if (ActiveInputManager.InputDownHold() || Held.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down) || Held.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
                {
                    DY = 1f;
                }

                DungeonRun Run = MapManager.Home.Run();
                PartyMember Member = Run.Party.Find(Owner.Session.PartyId);
                PlayerCharacter Who = Member != null && Member.Body != null ? Member.Body : ControlledCharacter;
                bool Crouching = Held.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) || Held.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl);
                float Strength = 1f;
                if (Who != null && Crouching)
                {
                    Who.CurrentPose = Pose.Crouch;
                    Strength = 0.5f;
                }
                else if (Who != null && Who.CurrentPose == Pose.Crouch)
                {
                    Who.CurrentPose = Pose.Stand;
                }

                if (!Crouching && (Held.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) || Held.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)))
                {
                    Strength = 1.75f;
                }

                bool Attack = Owner.KeyPressed(Microsoft.Xna.Framework.Input.Keys.F);
                bool Interact = Owner.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter) || ActiveInputManager.InputConfirmPressed();
                bool Hop = Owner.Session.Hop;
                Owner.Session.Hop = false;
                DungeonOrders.Step(MapManager, Owner.Session, DX, DY, (float)gameTime.ElapsedGameTime.TotalSeconds, Attack, Interact, Who, Strength, Hop);
                DungeonView.Frame(Owner.Camera, MapManager.Home.Dungeon, Owner.Session.PartyId);
                Owner.EndKeys();
                return;
            }

            CameraControl();

            if (ControlledCharacter.ListCurrentAIObjectiveStep.Count == 0 && (Owner.Session == null || !Owner.Session.Fighting))
            {
                Vector2 PlayerInput = Vector2.Zero;

                if (ActiveInputManager.InputRightHold())
                {
                    PlayerInput.X -= 1f;
                }
                else if (ActiveInputManager.InputLeftHold())
                {
                    PlayerInput.X += 1f;
                }

                if (ActiveInputManager.InputUpHold())
                {
                    PlayerInput.Y += 1f;
                }
                else if (ActiveInputManager.InputDownHold())
                {
                    PlayerInput.Y -= 1f;
                }

                float DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Microsoft.Xna.Framework.Input.KeyboardState Keys = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                bool Crouching = Keys.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl)
                    || Keys.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl);
                if (Crouching)
                {
                    ControlledCharacter.CurrentPose = Pose.Crouch;
                }
                else if (ControlledCharacter.CurrentPose == Pose.Crouch)
                {
                    ControlledCharacter.CurrentPose = Pose.Stand;
                }

                MoveWish Wish = new MoveWish();
                Wish.Stick = PlayerInput;
                Wish.Facing = Owner.Session != null && Owner.Session.View == CameraStyle.Eyes
                    ? BodyFacing.Camera
                    : BodyFacing.Travel;
                GroundStep Step = Rig.Plan(Wish, ThirdPersonRig.YawOf(ControlledCharacter.RotationMatrix));

                float Strength = 0f;
                if (TownLife.Riding(ControlledCharacter))
                {
                    Step.Direction = Vector3.Zero;
                    Strength = 0f;
                }
                else if (Step.Direction != Vector3.Zero)
                {
                    Strength = 1f;
                    if (Crouching)
                    {
                        Strength = 0.5f;
                    }
                    else if (Keys.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift)
                        || Keys.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift))
                    {
                        Strength = 1.75f;
                    }
                }

                float? Face = Step.FaceTravel && Step.Direction == Vector3.Zero ? (float?)null : Step.BodyYaw;
                ControlledCharacter.Move(Step.Direction, Strength, DeltaTime, Face);
                /*
                if (ActiveInputManager.InputCommand1Pressed())
                {
                    CharacterController.StanceManager.DesiredStance = Stance.Prone;
                }
                else if (ActiveInputManager.InputCommand2Pressed())
                {
                    CharacterController.StanceManager.DesiredStance = Stance.Crouching;
                }
                else
                {
                    CharacterController.StanceManager.DesiredStance = Stance.Standing;
                }

                //Jumping
                if (ActiveInputManager.InputSkipPressed())
                {
                    CharacterController.Jump();
                }
                */
            }

            CameraOffset = Rig.Offset();
            UpdateCamera(gameTime);
            Owner.EndKeys();
        }

        void ReadPlayKeys()
        {
            if (Owner.KeyPressed(Microsoft.Xna.Framework.Input.Keys.O))
            {
                MapManager.PushScreen(new OptionMenu());
            }
            else if (Owner.KeyPressed(Microsoft.Xna.Framework.Input.Keys.F1))
            {
                Owner.Session.ApplyKey(Microsoft.Xna.Framework.Input.Keys.F1);
                if (Owner.Session.AdminOpen)
                {
                    AddToPanelListAndSelect(new ActionPanelAdminMenu(Owner, MapManager, ListActionMenuChoice));
                }
            }
            else if (Owner.BuildPressed())
            {
                Owner.Session.ApplyKey(Microsoft.Xna.Framework.Input.Keys.Escape);
                if (Owner.Session.BuildOpen)
                {
                    AddToPanelListAndSelect(new ActionPanelBuildMenu(Owner, MapManager, ListActionMenuChoice));
                }
            }
            else if (Owner.PreviousPressed())
            {
                Owner.Session.ApplyKey(Microsoft.Xna.Framework.Input.Keys.Q);
                if (Owner.Session.Controlled != null)
                {
                    ControlledCharacterField(Owner.Session.Controlled);
                }
            }
            else if (Owner.NextPressed())
            {
                Owner.Session.ApplyKey(Microsoft.Xna.Framework.Input.Keys.E);
                if (Owner.Session.Controlled != null)
                {
                    ControlledCharacterField(Owner.Session.Controlled);
                }
            }
            else if (Owner.JumpPressed())
            {
                Owner.Session.Jump();
            }
            else if (Owner.KeyPressed(Microsoft.Xna.Framework.Input.Keys.V))
            {
                Owner.CyclePlayPanel(this);
            }
            else if (Owner.KeyPressed(Microsoft.Xna.Framework.Input.Keys.M))
            {
                AddToPanelListAndSelect(new ActionPanelCities(Owner, MapManager, ListActionMenuChoice));
            }
            else
            {
                Microsoft.Xna.Framework.Input.Keys[] Numbers = new Microsoft.Xna.Framework.Input.Keys[]
                {
                    Microsoft.Xna.Framework.Input.Keys.D1,
                    Microsoft.Xna.Framework.Input.Keys.D2,
                    Microsoft.Xna.Framework.Input.Keys.D3,
                    Microsoft.Xna.Framework.Input.Keys.D4,
                    Microsoft.Xna.Framework.Input.Keys.D5,
                    Microsoft.Xna.Framework.Input.Keys.D6,
                    Microsoft.Xna.Framework.Input.Keys.D7,
                    Microsoft.Xna.Framework.Input.Keys.D8,
                    Microsoft.Xna.Framework.Input.Keys.D9,
                };
                for (int n = 0; n < Numbers.Length; n++)
                {
                    if (Owner.KeyPressed(Numbers[n]))
                    {
                        Owner.Session.SelectSlot(n + 1);
                        if (Owner.Session.Controlled != null)
                        {
                            ControlledCharacterField(Owner.Session.Controlled);
                        }

                        break;
                    }
                }
            }
        }

        void ControlledCharacterField(PlayerCharacter Next)
        {
            // The panel keeps the character it was opened with until a switch replaces the panel.
            if (Next == ControlledCharacter)
            {
                return;
            }

            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelThirdPersonControlCharacter(Owner, Next, MapManager, ListActionMenuChoice));
        }

        LifeSimMap CurrentMap()
        {
            MapInfo Here = ControlledCharacter.SharedMapContex == null ? null : ControlledCharacter.SharedMapContex.CurrentMapInfo;
            if (Here == null || Here.MapMapContainer == null)
            {
                return null;
            }

            return Here.MapMapContainer.ActiveMap as LifeSimMap;
        }

        static List<Point> WallCells(PlayerCharacter Who)
        {
            List<Point> Walls = new List<Point>();
            LifeSimMap Map = Who.SharedMapContex == null || Who.SharedMapContex.CurrentMapInfo == null || Who.SharedMapContex.CurrentMapInfo.MapMapContainer == null
                ? null
                : Who.SharedMapContex.CurrentMapInfo.MapMapContainer.ActiveMap as LifeSimMap;
            if (Map == null || Map.LayerManager.ListLayer.Count == 0)
            {
                return Walls;
            }

            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    if (Map.LayerManager.ListLayer[0].ArrayTerrain[X, Y].TerrainTypeIndex == ProjectEternity.Core.Units.UnitStats.TerrainWallIndex)
                    {
                        Walls.Add(new Point(X, Y));
                    }
                }
            }

            return Walls;
        }

        static Vector3 Facing(PlayerCharacter Who)
        {
            return new Vector3(Who.RotationMatrix.M31, Who.RotationMatrix.M33, 0f);
        }

        private void UpdateCamera(GameTime gameTime)
        {
            double CameraFollowSpeed = 12;
            float DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector3 TarterPosition = ChunkOrigin.Camera(ControlledCharacter.SharedMapContex == null ? null : ControlledCharacter.SharedMapContex.CurrentMapInfo, new Vector3(ControlledCharacter.Position.X, ControlledCharacter.Position.Z, ControlledCharacter.Position.Y));
            CameraStyle Style = Owner.Session == null ? CameraStyle.Orbit : Owner.Session.View;
            if (Style == CameraStyle.Eyes)
            {
                CameraOffset = Rig.Offset(0f, 2f);
                TargetOffset = new Vector3(0f, 8f, 0f);
            }
            else if (Style == CameraStyle.TopDown)
            {
                Camera.CameraPosition3D = TarterPosition + new Vector3(0f, 80f, 40f);
                Camera.LookAtTarget = TarterPosition;
                Camera.View = Matrix.CreateLookAt(Camera.CameraPosition3D, Camera.LookAtTarget, Vector3.Up);
                Camera.CameraRotation = Camera.View;
                return;
            }

            Vector3 CameraPosition = TarterPosition + CameraOffset;
            Vector3 TarterPositionFinal = TarterPosition + TargetOffset;

            if (Camera.CameraPosition3D == Vector3.Zero)
            {
                Camera.CameraPosition3D = CameraPosition;
                Camera.LookAtTarget = TarterPositionFinal;
            }
            else
            {
                float CameraTransition = 1.0f - (float)Math.Exp(-(float)CameraFollowSpeed * DeltaTime);
                Camera.CameraPosition3D = Vector3.Lerp(Camera.CameraPosition3D, CameraPosition, CameraTransition);
                Camera.LookAtTarget = Vector3.Lerp(Camera.LookAtTarget, TarterPositionFinal, CameraTransition);
            }

            Camera.View = Matrix.CreateLookAt(Camera.CameraPosition3D, Camera.LookAtTarget, Vector3.Up);
            Camera.CameraRotation = Camera.View;
        }

        private void CameraControl()
        {
            if (MouseHelper.InputRightButtonHold())
            {
                float MouseX = MouseHelper.MouseStateCurrent.X - MouseHelper.MouseStateLast.X;
                float MouseY = MouseHelper.MouseStateCurrent.Y - MouseHelper.MouseStateLast.Y;
                Rig.Orbit(MouseX, MouseY);
            }

            int ScrollValue = MouseHelper.InputMouseWheelValue();
            if (ScrollValue != 0)
            {
                Rig.Zoom(ScrollValue / 120f);
            }
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
            return new ActionPanelThirdPersonControlCharacter(Owner, ControlledCharacter, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            bool TopDown = Owner.Session != null && Owner.Session.View == CameraStyle.TopDown;
            PlayerCharacter Cursor = Owner.InvisibleCharacterAsCursor;
            MapInfo Here = Cursor != null && Cursor.SharedMapContex != null ? Cursor.SharedMapContex.CurrentMapInfo : null;
            MapManager.DrawChunks(g, Owner.Camera, delegate (LifeSimMap Map)
            {
                Map.ShowCursor = false;
                if (Map.BuildPlacing)
                {
                    return;
                }

                Map.HideFloorsAbove = TopDown;
                Map.ShownFloor = ControlledCharacter == null ? 0 : BuildFloor.Index(ControlledCharacter.Position.Z);
                Map.ShowCursor = Here != null && (object.ReferenceEquals(Map.Chunk, Here) || (Here.MapMapContainer != null && object.ReferenceEquals(Here.MapMapContainer.ActiveMap, Map)));
            }, null, Owner.Session);
        }

        public override string ToString()
        {
            return ControlledCharacter.Name;
        }
    }
}
