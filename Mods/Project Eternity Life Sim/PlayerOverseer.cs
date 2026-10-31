using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PlayerOverseer
    {
        private readonly NavMapGameManager MapManager;

        public List<PlayerCharacter> ListControlledCharacter;
        private KnowledgeContainer Knowledge;
        public Camera3D Camera;

        public PlayerInput ActiveInputManager;
        public ActionPanelHolder ActionHolder;

        public PlayerCharacter ActiveControlledCharacter;
        public readonly PlayerCharacter InvisibleCharacterAsCursor;//Making the cursor is like moving a character to use the same map change code.
        public PlaySession Session;
        public Rectangle Pane;
        public int SeatIndex = 1;
        public ZoneDraft ZoneDraft;
        public bool OwnsKeyboard;

        readonly List<GamepadInput> Pads = new List<GamepadInput>();
        readonly ProjectEternity.GameScreens.BattleMapScreen.KeyboardInput Keyboard = new ProjectEternity.GameScreens.BattleMapScreen.KeyboardInput();

        public PlayerOverseer(NavMapGameManager MapManager, MapInfo CurrentMapInfo)
        {
            this.MapManager = MapManager;

            Session = new PlaySession();
            Session.User = this;
            Session.Home = MapManager.Home;
            ListControlledCharacter = Session.Assigned;
            ActiveInputManager = Keyboard;

            InvisibleCharacterAsCursor = new PlayerCharacter(MapManager, CurrentMapInfo, HomeCursor(CurrentMapInfo));

            Knowledge = new KnowledgeContainer();

            ActionHolder = new ActionPanelHolder();
            Session.View = CameraStyle.Life;

            ActionHolder.AddToPanelListAndSelect(new ActionPanelLifeCursor(this, MapManager, ActionHolder));
        }

        public void OfferJoin()
        {
            if (Session == null || !string.IsNullOrEmpty(Session.PartyId))
            {
                return;
            }

            ActionHolder.AddToPanelListAndSelect(new ActionPanelJoinParty(this, MapManager, ActionHolder));
        }

        static Vector3 HomeCursor(MapInfo Here)
        {
            LifeSimMap Map = Here == null || Here.MapMapContainer == null ? null : Here.MapMapContainer.ActiveMap as LifeSimMap;
            if (Map == null || Map.MapSize.X <= 0 || Map.MapSize.Y <= 0)
            {
                return Vector3.Zero;
            }

            float TileX = Map.TileSize.X > 0 ? Map.TileSize.X : 32f;
            float TileY = Map.TileSize.Y > 0 ? Map.TileSize.Y : TileX;
            float X = Map.MapSize.X * TileX * 0.5f;
            float Y = Map.MapSize.Y * TileY * 0.5f;
            return new Vector3(X, HeightField.View(Here, X, Y, 0), Y);
        }

        public void ControlCharacter(PlayerCharacter CharacterToControl)
        {
            Session.ToggleAssigned(CharacterToControl);
            Session.TakeCharacter(CharacterToControl);
            ActionHolder.AddToPanelListAndSelect(new ActionPanelThirdPersonControlCharacter(this, CharacterToControl, MapManager, ActionHolder));
        }

        public void CyclePlayPanel(ActionPanel Current)
        {
            if (Session == null || Current == null)
            {
                return;
            }

            Session.ToggleView();
            ActionPanel Next = null;
            if (Session.View == CameraStyle.Life)
            {
                if (!(Current is ActionPanelLifeCursor))
                {
                    Next = new ActionPanelLifeCursor(this, MapManager, ActionHolder);
                }
            }
            else if (Session.View == CameraStyle.Eyes || Session.View == CameraStyle.Orbit)
            {
                if (!(Current is ActionPanelThirdPersonControlCharacter))
                {
                    PlayerCharacter Who = Session.Controlled ?? Session.LifeSelection;
                    if (Who == null)
                    {
                        return;
                    }

                    Next = new ActionPanelThirdPersonControlCharacter(this, Who, MapManager, ActionHolder);
                }
            }

            if (Next == null)
            {
                return;
            }

            ActionHolder.Remove(Current);
            ActionHolder.AddToPanelListAndSelect(Next);
        }

        public void OpenPracticeMenu()
        {
            if (Session == null || Session.Practice == null || !ActionHolder.HasMainPanel)
            {
                return;
            }

            ActionPanel Top = ActionHolder.HasSubPanels ? ActionHolder.Last() : ActionHolder.GetMainPanel();
            if (!(Top is ActionPanelLifeCursor))
            {
                return;
            }

            ActionHolder.AddToPanelListAndSelect(new ActionPanelPracticeMenu(this, MapManager, ActionHolder));
        }

        public void ClosePracticeMenus()
        {
            if (!ActionHolder.HasMainPanel)
            {
                return;
            }

            List<ActionPanel> Panels = ActionHolder.GetActionPanels();
            for (int i = Panels.Count - 1; i >= 0; i--)
            {
                if (IsPracticePanel(Panels[i]))
                {
                    ActionHolder.Remove(Panels[i]);
                }
            }
        }

        static bool IsPracticePanel(ActionPanel Panel)
        {
            return Panel is ActionPanelPracticeMenu
                || Panel is ActionPanelPracticeStride
                || Panel is ActionPanelPracticeStrike
                || Panel is ActionPanelPracticeEndTurn
                || Panel is ActionPanelPracticeLog
                || Panel is ActionPanelPracticeLeave;
        }

        public bool TakeToolKeys(ActionPanel Current)
        {
            if (KeyPressed(Keys.O))
            {
                MapManager.PushScreen(new OptionMenu());
                EndKeys();
                return true;
            }

            if (KeyPressed(Keys.F1))
            {
                Session.ApplyKey(Keys.F1);
                if (Session.AdminOpen)
                {
                    ActionHolder.AddToPanelListAndSelect(new ActionPanelAdminMenu(this, MapManager, ActionHolder));
                }

                EndKeys();
                return true;
            }

            if (KeyPressed(Keys.V))
            {
                if (Session.Fighting || Session.View == CameraStyle.TopDown)
                {
                    EndKeys();
                    return true;
                }

                ClosePracticeMenus();
                ActionPanel Root = ActionHolder.HasMainPanel ? ActionHolder.GetMainPanel() : Current;
                CyclePlayPanel(Root);
                EndKeys();
                return true;
            }

            if (BuildPressed())
            {
                Session.ApplyKey(Keys.Escape);
                if (Session.BuildOpen)
                {
                    ActionHolder.AddToPanelListAndSelect(new ActionPanelBuildMenu(this, MapManager, ActionHolder));
                }

                EndKeys();
                return true;
            }

            return false;
        }

        public void Bind(List<LocalDevice> Devices, ControllerHelper[] Helpers)
        {
            OwnsKeyboard = false;
            Pads.Clear();
            List<PlayerInput> Sources = new List<PlayerInput>();
            if (Devices != null)
            {
                for (int i = 0; i < Devices.Count; i++)
                {
                    if (Devices[i] == LocalDevice.Keyboard)
                    {
                        OwnsKeyboard = true;
                        Sources.Add(Keyboard);
                        continue;
                    }

                    int Index = (int)Devices[i] - (int)LocalDevice.Pad1;
                    if (Index < 0 || Helpers == null || Index >= Helpers.Length || !Helpers[Index].IsConnected)
                    {
                        continue;
                    }

                    GamepadInput Pad = new GamepadInput(Helpers[Index]);
                    Pads.Add(Pad);
                    Sources.Add(Pad);
                }
            }

            if (Sources.Count == 0)
            {
                ActiveInputManager = Keyboard;
            }
            else if (Sources.Count == 1)
            {
                ActiveInputManager = Sources[0];
            }
            else
            {
                ActiveInputManager = new CombinedInput(Sources);
            }
        }

        public bool KeyPressed(Keys Key)
        {
            return OwnsKeyboard && PlayInput.Pressed(Key);
        }

        public void EndKeys()
        {
            if (OwnsKeyboard)
            {
                PlayInput.EndFrame();
            }

            for (int i = 0; i < Pads.Count; i++)
            {
                Pads[i].Remember();
            }
        }

        public bool BuildPressed()
        {
            if (KeyPressed(Keys.Escape))
            {
                return true;
            }

            for (int i = 0; i < Pads.Count; i++)
            {
                if (Pads[i].BuildPressed())
                {
                    return true;
                }
            }

            return false;
        }

        public bool JumpPressed()
        {
            if (KeyPressed(Keys.Space))
            {
                return true;
            }

            for (int i = 0; i < Pads.Count; i++)
            {
                if (Pads[i].JumpPressed())
                {
                    return true;
                }
            }

            return false;
        }

        public bool PreviousPressed()
        {
            if (KeyPressed(Keys.Q))
            {
                return true;
            }

            for (int i = 0; i < Pads.Count; i++)
            {
                if (Pads[i].PreviousPressed())
                {
                    return true;
                }
            }

            return false;
        }

        public bool NextPressed()
        {
            if (KeyPressed(Keys.E))
            {
                return true;
            }

            for (int i = 0; i < Pads.Count; i++)
            {
                if (Pads[i].NextPressed())
                {
                    return true;
                }
            }

            return false;
        }

        public void Update(GameTime gameTime)
        {
            if (ActionHolder.HasSubPanels)
            {
                ActionHolder.Last().Update(gameTime);
            }
            else if (ActionHolder.HasMainPanel)
            {
                ActionHolder.GetMainPanel().Update(gameTime);
            }
        }

        public void Draw(CustomSpriteBatch g)
        {
            Viewport Previous = g.GraphicsDevice.Viewport;
            if (Pane.Width > 0 && Pane.Height > 0)
            {
                g.GraphicsDevice.Viewport = new Viewport(Pane);
            }
            if (ActionHolder.HasMainPanel)
            {
                ActionHolder.GetMainPanel().Draw(g);
            }
            if (ActionHolder.HasSubPanels)
            {
                ActionHolder.Last().Draw(g);
            }

            if (NeedsPeek())
            {
                PlayerCharacter Who = Session == null ? null : Session.LifeSelection;
                if (Who == null)
                {
                    Who = ActiveControlledCharacter;
                }

                NeedsBar.Draw(g, Who);
            }

            if (Session != null && !ActionHolder.HasSubPanels
                && (Session.View == CameraStyle.Eyes || Session.View == CameraStyle.Orbit)
                && Session.Controlled != null)
            {
                NoiseBar.Draw(g, Session.Controlled, NeedsPeek());
            }

            DrawCameraReadout(g);
            g.GraphicsDevice.Viewport = Previous;
        }

        void DrawCameraReadout(CustomSpriteBatch g)
        {
            if (Camera == null || g == null)
            {
                return;
            }

            Vector3 Eye = Camera.CameraPosition3D;
            Vector3 At = Camera.LookAtTarget;
            Vector3 Delta = At - Eye;
            float Flat = (float)Math.Sqrt(Delta.X * Delta.X + Delta.Z * Delta.Z);
            float Pitch = MathHelper.ToDegrees((float)Math.Atan2(Delta.Y, Flat));
            float Yaw = MathHelper.ToDegrees((float)Math.Atan2(Delta.X, Delta.Z));
            string Line1 = "Cam  " + Eye.X.ToString("0.0") + "  " + Eye.Y.ToString("0.0") + "  " + Eye.Z.ToString("0.0");
            string Line2 = "At  " + At.X.ToString("0.0") + "  " + At.Y.ToString("0.0") + "  " + At.Z.ToString("0.0");
            string Line3 = "Yaw  " + Yaw.ToString("0.0") + "   Pitch  " + Pitch.ToString("0.0");
            int BoxW = 460;
            int BoxH = 58;
            int X = Math.Max(8, Pane.Width - BoxW - 8);
            int Y = 8;
            Texture2D Pixel = GameScreen.sprPixel;
            if (Pixel != null)
            {
                g.Draw(Pixel, new Rectangle(X - 6, Y - 4, BoxW, BoxH), new Color(0, 0, 0, 170));
            }

            TextHelper.DrawText(g, Line1, new Vector2(X, Y), Color.White);
            TextHelper.DrawText(g, Line2, new Vector2(X, Y + 16), Color.White);
            TextHelper.DrawText(g, Line3, new Vector2(X, Y + 32), Color.White);
        }

        public bool NeedsPeek()
        {
            bool Held = false;
            for (int i = 0; i < Pads.Count; i++)
            {
                if (Pads[i].DPadUpHeld())
                {
                    Held = true;
                    break;
                }
            }

            if (!Held || !ActionHolder.HasMainPanel)
            {
                return false;
            }

            ActionPanel Top = ActionHolder.HasSubPanels ? ActionHolder.Last() : ActionHolder.GetMainPanel();
            return Top is ActionPanelLifeCursor
                || Top is ActionPanelPracticeMenu
                || Top is ActionPanelPracticeStride
                || Top is ActionPanelThirdPersonControlCharacter
                || Top is ActionPanelTopDownControlCharacter
                || Top is ActionPanelFreeCam
                || Top is ActionPanelBuildCursor;
        }
    }
}
