using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelCreatureMovement : ActionPanelSorcererStreet
    {
        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private TerrainSorcererStreet ActiveTerrain;

        private Vector3 CursorPosition;
        private Camera3D Camera;
        private TerrainSorcererStreet HoverTerrain;

        public ActionPanelCreatureMovement(SorcererStreetMap Map)
            : base("Creature Movement", Map, false)
        {
        }
        public ActionPanelCreatureMovement(SorcererStreetMap Map, int ActivePlayerIndex, TerrainSorcererStreet ActiveTerrain)
            : base("Creature Movement", Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            this.ActiveTerrain = ActiveTerrain;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Vector3 Target = new Vector3(CursorPosition.X, CursorPosition.Z, CursorPosition.Y);
            Camera.CameraPosition3D = Vector3.Transform(new Vector3(0, 0, 200), Matrix.CreateRotationY(MathHelper.ToRadians(40))) + Target;
            Camera.CameraPosition3D = Vector3.Transform(Camera.CameraPosition3D, Matrix.CreateTranslation(0f, 150, 0f));
            Camera.View = Matrix.CreateLookAt(Camera.CameraPosition3D, Target, Vector3.Up);

            if (ActiveInputManager.InputUpHold() && CursorPosition.Y > 0)
            {
                CursorPosition.Y -= (float)gameTime.ElapsedGameTime.TotalSeconds * 100f;
                HoverTerrain = Map.GetTerrain(new Vector3(CursorPosition.X / Map.TileSize.X, CursorPosition.Y / Map.TileSize.Y, CursorPosition.Z));
            }
            if (ActiveInputManager.InputDownHold() && CursorPosition.Y + 1 < Map.MapSize.Y * Map.TileSize.Y)
            {
                CursorPosition.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * 100f;
                HoverTerrain = Map.GetTerrain(new Vector3(CursorPosition.X / Map.TileSize.X, CursorPosition.Y / Map.TileSize.Y, CursorPosition.Z));
            }
            if (ActiveInputManager.InputLeftHold() && CursorPosition.X > 0)
            {
                CursorPosition.X -= (float)gameTime.ElapsedGameTime.TotalSeconds * 100f;
                HoverTerrain = Map.GetTerrain(new Vector3(CursorPosition.X / Map.TileSize.X, CursorPosition.Y / Map.TileSize.Y, CursorPosition.Z));
            }
            if (ActiveInputManager.InputRightHold() && CursorPosition.X + 1 < Map.MapSize.X * Map.TileSize.X)
            {
                CursorPosition.X += (float)gameTime.ElapsedGameTime.TotalSeconds * 100f;
                HoverTerrain = Map.GetTerrain(new Vector3(CursorPosition.X / Map.TileSize.X, CursorPosition.Y / Map.TileSize.Y, CursorPosition.Z));
            }
            if (ActiveInputManager.InputConfirmPressed() && HoverTerrain.TerrainTypeIndex != 0
                && (Map.ListPassedTerrein.Contains(HoverTerrain)))
            {
                if (ActiveTerrain.DefendingCreature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).FreeTravel)
                {
                }

            }
            else if (ActiveInputManager.InputCancelPressed())
            {
                RemoveFromPanelList(this);
                Map.Camera3DOverride = null;
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelCreatureMovement(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            Vector3 Visible3DPosition = new Vector3(CursorPosition.X, CursorPosition.Z * Map.LayerHeight, CursorPosition.Y);
            Vector3 Position = new Vector3(Visible3DPosition.X, Visible3DPosition.Y, Visible3DPosition.Z);

            Vector3 Position2D = g.GraphicsDevice.Viewport.Project(Position, Camera.Projection, Camera.View, Matrix.Identity);
            MenuHelper.DrawFingerIcon(g, new Vector2(Position2D.X - 85, Position2D.Y - 25));

            if (HoverTerrain.TerrainTypeIndex != 0)
            {
                ActionPanelPlayerDefault.DrawLandInformationTop(g, Map, HoverTerrain);
                if (HoverTerrain.DefendingCreature != null)
                {
                    HoverTerrain.DefendingCreature.DrawCardInfo(g, Map.Symbols, Map.fntMenuText, ActivePlayer, 0, 0);

                    int BoxWidth = (int)(Constants.Width / 2.8);
                    int BoxHeight = (int)(Constants.Height / 8);
                    float InfoBoxX = Constants.Width - Constants.Width / 12 - BoxWidth;
                    float InfoBoxY = Constants.Height / 2 + Constants.Height / 5;

                    GameScreen.DrawBox(g, new Vector2(InfoBoxX, InfoBoxY - 20), BoxWidth, 20, Color.White);
                    g.DrawString(Map.fntMenuText, "Support", new Vector2(InfoBoxX + 10, InfoBoxY - 20), Color.White);
                    GameScreen.DrawBox(g, new Vector2(InfoBoxX, InfoBoxY), BoxWidth, BoxHeight, Color.White);

                    float CurrentX = InfoBoxX + 10;
                    float CurrentY = InfoBoxY - 10;

                    CurrentY += 20;

                    g.DrawString(Map.fntMenuText, "Support / Land Effect", new Vector2(CurrentX, CurrentY), Color.White);
                    CurrentY += 20;
                    g.DrawString(Map.fntMenuText, "+" + 10, new Vector2(CurrentX + 10, CurrentY), Color.White);
                    g.DrawString(Map.fntMenuText, "/", new Vector2(CurrentX + 60, CurrentY), Color.White);
                    g.DrawString(Map.fntMenuText, "+" + 10, new Vector2(CurrentX + 85, CurrentY), Color.White);
                }
            }
        }
    }
}
