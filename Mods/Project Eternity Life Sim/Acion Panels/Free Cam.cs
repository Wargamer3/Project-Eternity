using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelFreeCam : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "FreeCam";

        private readonly List<MapInfo> ListVisisbleMap;
        private readonly Camera3D Camera;

        private PlayerCharacter HoverCharacter;

        public ActionPanelFreeCam(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, false)
        {
            Camera = new LifeSimFreeCamera(GameScreen.GraphicsDevice, Owner);
            ListVisisbleMap = new List<MapInfo>();
        }

        public override void OnSelect()
        {
            ListVisisbleMap.Add(MapManager.RootMapContainer);
            Owner.Camera = Camera;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            PlayerCharacter FoundCharacter = Owner.InvisibleCharacterAsCursor.SharedMapContex.ActiveMap.MoveCursorAndGetCharacterUnderMouse();

            if (FoundCharacter != null)
            {
                HoverCharacter = FoundCharacter;
            }
            
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

            Camera.Update(gameTime);
            Owner.EndKeys();
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
            return new ActionPanelFreeCam(Owner, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            PlayerCharacter Cursor = Owner.InvisibleCharacterAsCursor;
            MapInfo Here = Cursor != null && Cursor.SharedMapContex != null ? Cursor.SharedMapContex.CurrentMapInfo : null;
            MapManager.DrawChunks(g, Owner.Camera, delegate (LifeSimMap Map)
            {
                bool Owns = Here != null && (object.ReferenceEquals(Map.Chunk, Here) || (Here.MapMapContainer != null && object.ReferenceEquals(Here.MapMapContainer.ActiveMap, Map)));
                Map.ShowCursor = Owns && !Map.BuildPlacing;
                if (Owns && !Map.BuildPlacing)
                {
                    Map.HideFloorsAbove = false;
                    Map.CursorPosition = new Vector3(Cursor.Position.X, Cursor.Position.Z, Cursor.Position.Y);
                    Map.CursorPositionVisible = Map.CursorPosition;
                }
            });
        }
    }
}
