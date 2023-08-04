using System;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public struct IndexedLines
    {
        public VertexPositionColor[] ArrayVertex;
        private short[] ArrayIndices;

        public IndexedLines(VertexPositionColor[] ArrayVertex, short[] ArrayIndices)
        {
            this.ArrayVertex = ArrayVertex;
            this.ArrayIndices = ArrayIndices;
        }

        internal void Draw(CustomSpriteBatch g)
        {
            g.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.LineList,
                ArrayVertex,
                0,  // vertex buffer offset to add to each element of the index buffer
                ArrayVertex.Length,  // number of vertices in pointList
                ArrayIndices,  // the index buffer
                0,  // first index element to read
                ArrayIndices.Length / 2 - 1   // number of primitives to draw
            );
        }
    }

    public class BattleMapInventoryScreen : GameScreen
    {
        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private SpriteFont fntArial12;

        private BasicEffect IndexedLinesEffect;
        private IndexedLines BackgroundGrid;

        public Vector3 BackgroundEmiterPosition;
        public Vector3[] ArrayNextPosition;
        public int CurrentPositionIndex;
        public int OldLineIndex;
        public int CurrentLineIndex;
        int CylinderParts = 10;
        int SegmentIncrement = 10;
        int Segments;
        TunnelBehaviorSpeedManager TunnelBehaviorSpeed;
        TunnelBehaviorColorManager TunnelBehaviorColor;
        TunnelBehaviorSizeManager TunnelBehaviorSize;
        TunnelBehaviorRotationManager TunnelBehaviorRotation;
        TunnelBehaviorDirectionManager TunnelBehaviorDirection;

        private BoxButton ReturnToLobbyButton;

        private BoxButton UnitFilterButton;
        private BoxButton CharacterFilterButton;
        private BoxButton EquipmentFilterButton;
        private BoxButton ConsumableFilterButton;
        private DropDownButton CurrentLocalPlayerButton;

        private IUIElement[] ArrayUIElement;

        public static int LeftSideWidth;
        public static int TopSectionHeight;
        public static int HeaderSectionY;
        public static int HeaderSectionHeight;

        public static int BottomSectionHeight;
        public static int BottomSectionY;

        public static int MiddleSectionY;
        public static int MiddleSectionHeight;

        private GameScreen[] ArraySubScreen;

        private GameScreen ActiveSubScreen;

        public BattleMapPlayer ActivePlayer;

        public BattleMapInventoryScreen()
        {
            ActivePlayer = (BattleMapPlayer)PlayerManager.ListLocalPlayer[0];

            Segments = 360 / SegmentIncrement * 4;

            LeftSideWidth = (int)(Constants.Width * 0.5 + 20);
            TopSectionHeight = (int)(Constants.Height * 0.1);
            HeaderSectionY = TopSectionHeight;
            HeaderSectionHeight = (int)(Constants.Height * 0.05);

            BottomSectionHeight = (int)(Constants.Height * 0.07);
            BottomSectionY = Constants.Height - BottomSectionHeight;

            MiddleSectionY = (HeaderSectionY + HeaderSectionHeight);
            MiddleSectionHeight = BottomSectionY - MiddleSectionY;

            TunnelBehaviorSpeed = new TunnelBehaviorSpeedManager();
            TunnelBehaviorColor = new TunnelBehaviorColorManager();
            TunnelBehaviorSize = new TunnelBehaviorSizeManager();
            TunnelBehaviorRotation = new TunnelBehaviorRotationManager();
            TunnelBehaviorDirection = new TunnelBehaviorDirectionManager();
        }

        public override void Load()
        {
            IndexedLinesEffect = new BasicEffect(GraphicsDevice);
            IndexedLinesEffect.VertexColorEnabled = true;
            IndexedLinesEffect.DiffuseColor = new Vector3(1, 1, 1);

            int SegmentIncrement = 10;
            int Segments = 360 / SegmentIncrement;
            int Parts = 1 * Segments;
            int ArrayLength = (int)(Parts * 4);
            ArrayNextPosition = new Vector3[ArrayLength];
            VertexPositionColor[] ArrayBackgroundGridVertex = new VertexPositionColor[ArrayLength];
            // Initialize an array of indices of type short.
            short[] ArrayBackgroundGridIndices = new short[(ArrayBackgroundGridVertex.Length * 2) - 2];
            for (int Index = 0; Index < ArrayBackgroundGridVertex.Length; ++Index)
            {
                ArrayBackgroundGridVertex[Index] = new VertexPositionColor(
                    new Vector3(), Color.White);

                ArrayBackgroundGridIndices[Index] = (short)(Index);
            }

            BackgroundGrid = new IndexedLines(ArrayBackgroundGridVertex, ArrayBackgroundGridIndices);
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");

            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            ReturnToLobbyButton = new BoxButton(new Rectangle((int)(Constants.Width * 0.7f), 0, (int)(Constants.Width * 0.3), TopSectionHeight), fntArial12, "Back To Lobby", OnButtonOver, SelectBackToLobbyButton);

            UnitFilterButton = new BoxButton(new Rectangle(LeftSideWidth + 4, HeaderSectionY + 4, 60, HeaderSectionHeight - 8), fntArial12, "Units", OnButtonOver, SelectUnitFilterButton);
            CharacterFilterButton = new BoxButton(new Rectangle(LeftSideWidth + 64, HeaderSectionY + 4, 90, HeaderSectionHeight - 8), fntArial12, "Characters", OnButtonOver, SelectCharacterFilterButton);
            EquipmentFilterButton = new BoxButton(new Rectangle(LeftSideWidth + 154, HeaderSectionY + 4, 90, HeaderSectionHeight - 8), fntArial12, "Equipment", OnButtonOver, SelectEquipmentFilterButton);
            ConsumableFilterButton = new BoxButton(new Rectangle(LeftSideWidth + 244, HeaderSectionY + 4, 100, HeaderSectionHeight - 8), fntArial12, "Consumable", OnButtonOver, SelectConsumableFilterButton);

            CurrentLocalPlayerButton = new DropDownButton(new Rectangle(400, 8, 120, 45), fntArial12, "M&K",
                new string[] { "M&K", "Gamepad 1", "Gamepad 2", "Gamepad 3", "Gamepad 4" }, OnButtonOver, null);

            UnitFilterButton.CanBeChecked = true;
            CharacterFilterButton.CanBeChecked = true;
            EquipmentFilterButton.CanBeChecked = true;
            ConsumableFilterButton.CanBeChecked = true;

            UnitFilterButton.Select();

            ArrayUIElement = new IUIElement[]
            {
                CurrentLocalPlayerButton, ReturnToLobbyButton,
                UnitFilterButton, CharacterFilterButton, EquipmentFilterButton, ConsumableFilterButton,
            };

            InventorySquadScreen2 NewShopCharacterScreen = new InventorySquadScreen2(this);
            InventoryCharacterScreen NewShopEquipmentScreen = new InventoryCharacterScreen(this);
            InventoryEquipmentScreen NewShopWeaponsScreen = new InventoryEquipmentScreen();
            InventoryConsumableScreen NewShopItemsScreen = new InventoryConsumableScreen();

            ArraySubScreen = new GameScreen[] { NewShopCharacterScreen, NewShopEquipmentScreen, NewShopWeaponsScreen, NewShopItemsScreen };

            foreach (GameScreen ActiveScreen in ArraySubScreen)
            {
                ActiveScreen.Content = Content;
                ActiveScreen.ListGameScreen = ListGameScreen;
                ActiveScreen.Load();
            }

            ActiveSubScreen = NewShopCharacterScreen;
        }

        public override void Unload()
        {
            SoundSystem.ReleaseSound(sndButtonOver);
            SoundSystem.ReleaseSound(sndButtonClick);

            foreach (GameScreen ActiveScreen in ArraySubScreen)
            {
                ActiveScreen.Unload();
            }
        }

        public override void Update(GameTime gameTime)
        {
            TunnelBehaviorSpeed.Update(gameTime);
            TunnelBehaviorColor.Update(gameTime);
            TunnelBehaviorSize.Update(gameTime);
            TunnelBehaviorRotation.Update(gameTime);
            TunnelBehaviorDirection.Update(gameTime);

            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Update(gameTime);
            }

            CreateSimpleBackground(gameTime);
            ActiveSubScreen.Update(gameTime);
        }

        private void CreateAnimatedBackground(GameTime gameTime)
        {
            Vector3 Up = Vector3.Up;

            int Parts = CylinderParts * Segments;
            int ArrayLength = Parts;

            float CylinderSize = TunnelBehaviorSize.TunnelSizeFinal;

            if (ArrayNextPosition == null || ArrayNextPosition.Length != ArrayLength)
            {
                ArrayNextPosition = new Vector3[ArrayLength];
                VertexPositionColor[] ArrayBackgroundGridVertex = new VertexPositionColor[ArrayLength];
                // Initialize an array of indices of type short.
                short[] ArrayBackgroundGridIndices = new short[(ArrayBackgroundGridVertex.Length * 2) - 2];
                for (int i = 0; i < ArrayBackgroundGridVertex.Length; ++i)
                {
                    ArrayBackgroundGridVertex[i] = new VertexPositionColor(
                        new Vector3(), Color.White);

                    ArrayBackgroundGridIndices[i] = (short)(i);
                }

                BackgroundGrid = new IndexedLines(ArrayBackgroundGridVertex, ArrayBackgroundGridIndices);
            }

            float Speed = 5;
            float SpeedX = (float)(Math.Cos(TunnelBehaviorDirection.ActiveDirection) * TunnelBehaviorSpeed.ActiveSpeed * gameTime.ElapsedGameTime.TotalSeconds);
            float SpeedY = (float)(Math.Sin(TunnelBehaviorDirection.ActiveDirection) * TunnelBehaviorSpeed.ActiveSpeed * gameTime.ElapsedGameTime.TotalSeconds);
            BackgroundEmiterPosition += new Vector3(SpeedX, SpeedY, (float)(Speed * 0.01f));

            ++CurrentPositionIndex;

            if (CurrentPositionIndex >= ArrayLength)
            {
                CurrentPositionIndex = 0;
            }

            ArrayNextPosition[CurrentPositionIndex] = BackgroundEmiterPosition;

            int NextLineIndex = (int)Math.Floor(CurrentPositionIndex / (float)Segments) * Segments;
            if (CurrentLineIndex != NextLineIndex)
            {
                OldLineIndex = CurrentLineIndex;
                TunnelBehaviorDirection.ActiveDirection = TunnelBehaviorDirection.TunnelDirectionFinal;
                TunnelBehaviorSpeed.ActiveSpeed = TunnelBehaviorSpeed.TunnelSpeedFinal;
            }

            CurrentLineIndex = NextLineIndex;

            int OldIndex = OldLineIndex;
            int Index = CurrentLineIndex;

            Color LineColor = ColorFromHSV(TunnelBehaviorColor.TunnelHueFinal, 1, 1);

            for (int X = 0; X < 360; X += SegmentIncrement)
            {
                float FinalRotation = X + TunnelBehaviorRotation.TunnelRotationFinal;
                Vector3 OldPosition = BackgroundGrid.ArrayVertex[OldIndex + 1].Position;
                Vector3 CurrentRightDistance = Vector3.Transform(Up, Matrix.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(FinalRotation))) * CylinderSize;
                Vector3 NextRightDistance = Vector3.Transform(Up, Matrix.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(FinalRotation + SegmentIncrement))) * CylinderSize;

                float CurrentX = BackgroundEmiterPosition.X;
                float CurrentY = BackgroundEmiterPosition.Y;
                float CurrentZ = BackgroundEmiterPosition.Z/* + X / 60f*/;

                //Draw cylinder lines
                BackgroundGrid.ArrayVertex[Index] = new VertexPositionColor(
                    OldPosition, LineColor);

                BackgroundGrid.ArrayVertex[Index + 1] = new VertexPositionColor(
                    new Vector3(CurrentX, CurrentY, CurrentZ) + CurrentRightDistance, LineColor);

                //Draw ring lines
                BackgroundGrid.ArrayVertex[Index + 2] = new VertexPositionColor(
                    new Vector3(CurrentX, CurrentY, CurrentZ) + CurrentRightDistance, LineColor);

                BackgroundGrid.ArrayVertex[Index + 3] = new VertexPositionColor(
                    new Vector3(CurrentX, CurrentY, CurrentZ) + NextRightDistance, LineColor);

                OldIndex += 4;
                Index += 4;
            }
        }

        private void CreateSimpleBackground(GameTime gameTime)
        {
            Vector3 Up = Vector3.Up;

            int Parts = CylinderParts * Segments;
            int ArrayLength = Parts;

            float CylinderSize = 1;

            if (ArrayNextPosition == null || ArrayNextPosition.Length != ArrayLength)
            {
                ArrayNextPosition = new Vector3[ArrayLength];
                VertexPositionColor[] ArrayBackgroundGridVertex = new VertexPositionColor[ArrayLength];
                // Initialize an array of indices of type short.
                short[] ArrayBackgroundGridIndices = new short[(ArrayBackgroundGridVertex.Length * 2) - 2];
                for (int i = 0; i < ArrayBackgroundGridVertex.Length; ++i)
                {
                    ArrayBackgroundGridVertex[i] = new VertexPositionColor(
                        new Vector3(), Color.White);

                    ArrayBackgroundGridIndices[i] = (short)(i);
                }

                BackgroundGrid = new IndexedLines(ArrayBackgroundGridVertex, ArrayBackgroundGridIndices);
            }

            float Speed = 1;
            BackgroundEmiterPosition += new Vector3(0, 0, (float)(Speed * 0.01f));

            ++CurrentPositionIndex;

            if (CurrentPositionIndex >= ArrayLength)
            {
                CurrentPositionIndex = 0;
            }

            ArrayNextPosition[CurrentPositionIndex] = BackgroundEmiterPosition;

            int NextLineIndex = (int)Math.Floor(CurrentPositionIndex / (float)Segments) * Segments;
            if (CurrentLineIndex != NextLineIndex)
            {
                OldLineIndex = CurrentLineIndex;
            }

            CurrentLineIndex = NextLineIndex;

            int OldIndex = OldLineIndex;
            int Index = CurrentLineIndex;

            Color LineColor = Color.White;

            for (int X = 0; X < 360; X += SegmentIncrement)
            {
                Vector3 OldPosition = BackgroundGrid.ArrayVertex[OldIndex + 1].Position;
                Vector3 CurrentRightDistance = Vector3.Transform(Up, Matrix.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(X))) * CylinderSize;
                Vector3 NextRightDistance = Vector3.Transform(Up, Matrix.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(X + SegmentIncrement))) * CylinderSize;

                float CurrentX = BackgroundEmiterPosition.X;
                float CurrentY = BackgroundEmiterPosition.Y;
                float CurrentZ = BackgroundEmiterPosition.Z + X / 60f;

                //Draw cylinder lines
                BackgroundGrid.ArrayVertex[Index] = new VertexPositionColor(
                    OldPosition, LineColor);

                BackgroundGrid.ArrayVertex[Index + 1] = new VertexPositionColor(
                    new Vector3(CurrentX, CurrentY, CurrentZ) + CurrentRightDistance, LineColor);

                //Draw ring lines
                BackgroundGrid.ArrayVertex[Index + 2] = new VertexPositionColor(
                    new Vector3(CurrentX, CurrentY, CurrentZ) + CurrentRightDistance, LineColor);

                BackgroundGrid.ArrayVertex[Index + 3] = new VertexPositionColor(
                    new Vector3(CurrentX, CurrentY, CurrentZ) + NextRightDistance, LineColor);

                OldIndex += 4;
                Index += 4;
            }
        }

        private Color ColorFromHSV(float Hue, float Value, float Saturation)
        {
            double hh, p, q, t, ff;
            long i;
            hh = Hue;
            if (hh >= 360.0) hh = 0.0;
            hh /= 60.0;
            i = (long)hh;
            ff = hh - i;
            p = Value * (1.0 - Saturation) * 255;
            q = Value * (1.0 - (Saturation * ff)) * 255;
            t = Value * (1.0 - (Saturation * (1.0 - ff))) * 255;
            Value *= 255;

            switch (i)
            {
                case 0:
                    return Color.FromNonPremultiplied((int)Value, (int)t, (int)p, 255);
                case 1:
                    return Color.FromNonPremultiplied((int)q, (int)Value, (int)p, 255);
                case 2:
                    return Color.FromNonPremultiplied((int)p, (int)Value, (int)t, 255);
                case 3:
                    return Color.FromNonPremultiplied((int)p, (int)q, (int)Value, 255);
                case 4:
                    return Color.FromNonPremultiplied((int)t, (int)p, (int)Value, 255);
                default:
                    return Color.FromNonPremultiplied((int)Value, (int)p, (int)q, 255);

            }
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        #region Buttons

        private void SelectBackToLobbyButton()
        {
            sndButtonClick.Play();
            RemoveScreen(this);
        }

        private void SelectUnitFilterButton()
        {
            sndButtonClick.Play();
            ActiveSubScreen = ArraySubScreen[0];

            CharacterFilterButton.Unselect();
            EquipmentFilterButton.Unselect();
            ConsumableFilterButton.Unselect();
        }

        private void SelectCharacterFilterButton()
        {
            sndButtonClick.Play();
            ActiveSubScreen = ArraySubScreen[1];

            UnitFilterButton.Unselect();
            EquipmentFilterButton.Unselect();
            ConsumableFilterButton.Unselect();
        }

        private void SelectEquipmentFilterButton()
        {
            sndButtonClick.Play();
            ActiveSubScreen = ArraySubScreen[2];

            UnitFilterButton.Unselect();
            CharacterFilterButton.Unselect();
            ConsumableFilterButton.Unselect();
        }

        private void SelectConsumableFilterButton()
        {
            sndButtonClick.Play();
            ActiveSubScreen = ArraySubScreen[3];

            UnitFilterButton.Unselect();
            CharacterFilterButton.Unselect();
            EquipmentFilterButton.Unselect();
        }

        #endregion

        public override void Draw(CustomSpriteBatch g)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            float aspectRatio = Constants.Width / Constants.Height;

            int DrawOffset = 400;
            int DrawLineIndex = CurrentPositionIndex - DrawOffset % ArrayNextPosition.Length;
            if (DrawLineIndex < 0)
            {
                DrawLineIndex += ArrayNextPosition.Length;
            }

            int DrawTargetLineIndex = (DrawLineIndex + 80) % ArrayNextPosition.Length;

            Vector3 position = new Vector3(ArrayNextPosition[DrawLineIndex].X,
                                            ArrayNextPosition[DrawLineIndex].Y,
                                            ArrayNextPosition[DrawLineIndex].Z);

            Vector3 target = new Vector3(ArrayNextPosition[DrawTargetLineIndex].X,
                                            ArrayNextPosition[DrawTargetLineIndex].Y,
                                            ArrayNextPosition[DrawTargetLineIndex].Z);

            Vector3 up = Vector3.Up;
            Matrix View = Matrix.CreateLookAt(position, target, up);
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    0.1f, 1000);

            IndexedLinesEffect.View = View;
            IndexedLinesEffect.Projection = Projection;
            IndexedLinesEffect.World = Matrix.Identity;

            foreach (EffectPass pass in IndexedLinesEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                BackgroundGrid.Draw(g);
            }

            g.End();
            g.Begin();

            DrawBox(g, new Vector2(), Constants.Width, Constants.Height, Color.FromNonPremultiplied(255, 255, 255, 0));

            DrawBox(g, new Vector2(0, MiddleSectionY), LeftSideWidth, MiddleSectionHeight, Color.FromNonPremultiplied(255, 255, 255, 0));
            DrawBox(g, new Vector2(LeftSideWidth, MiddleSectionY), Constants.Width - LeftSideWidth, MiddleSectionHeight, Color.FromNonPremultiplied(255, 255, 255, 0));

            ActiveSubScreen.Draw(g);

            //Left side
            DrawBox(g, new Vector2(0, BottomSectionY), LeftSideWidth, Constants.Height - BottomSectionY, Color.FromNonPremultiplied(255, 255, 255, 0));

            DrawBox(g, new Vector2(0, MiddleSectionY), LeftSideWidth, MiddleSectionHeight, Color.FromNonPremultiplied(255, 255, 255, 0));
            DrawBox(g, new Vector2(LeftSideWidth, MiddleSectionY), Constants.Width - LeftSideWidth, MiddleSectionHeight, Color.FromNonPremultiplied(255, 255, 255, 0));

            DrawBox(g, new Vector2(0, HeaderSectionY), LeftSideWidth, HeaderSectionHeight, Color.FromNonPremultiplied(255, 255, 255, 0));
            DrawBox(g, new Vector2(0, 0), (int)(Constants.Width * 0.7), TopSectionHeight, Color.FromNonPremultiplied(255, 255, 255, 0));
            g.DrawString(fntArial12, "INVENTORY", new Vector2(10, 20), Color.White);

            DrawBox(g, new Vector2(LeftSideWidth, HeaderSectionY), LeftSideWidth, HeaderSectionHeight, Color.FromNonPremultiplied(255, 255, 255, 0));
            g.DrawStringRightAligned(fntArial12, "Money: 14360 cr", new Vector2(LeftSideWidth - 12, BottomSectionY + 11), Color.FromNonPremultiplied(255, 255, 255, 0));

            foreach (IUIElement ActiveButton in ArrayUIElement)
            {
                ActiveButton.Draw(g);
            }
        }
    }
}
