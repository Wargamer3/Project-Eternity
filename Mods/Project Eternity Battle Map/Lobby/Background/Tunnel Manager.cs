using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class TunnelManager
    {
        private BasicEffect IndexedLinesEffect;
        private IndexedLines BackgroundGrid;

        private Vector3 BackgroundEmiterPosition;
        private Vector3[] ArrayNextPosition;
        private int CurrentPositionIndex;
        private int OldLineIndex;
        private int CurrentLineIndex;
        private int CylinderParts = 10;
        private int SegmentIncrement = 10;
        private int Segments;
        private TunnelBehaviorSpeedManager TunnelBehaviorSpeed;
        private TunnelBehaviorColorManager TunnelBehaviorColor;
        private TunnelBehaviorSizeManager TunnelBehaviorSize;
        private TunnelBehaviorRotationManager TunnelBehaviorRotation;
        private TunnelBehaviorDirectionManager TunnelBehaviorDirection;

        public TunnelManager()
        {
            Segments = 360 / SegmentIncrement * 4;

            TunnelBehaviorSpeed = new TunnelBehaviorSpeedManager();
            TunnelBehaviorColor = new TunnelBehaviorColorManager();
            TunnelBehaviorSize = new TunnelBehaviorSizeManager();
            TunnelBehaviorRotation = new TunnelBehaviorRotationManager();
            TunnelBehaviorDirection = new TunnelBehaviorDirectionManager();
        }

        public void Load(GraphicsDevice GraphicsDevice)
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
        }

        public void Update(double TimeElapsedInSeconds)
        {
            TunnelBehaviorSpeed.Update(TimeElapsedInSeconds);
            TunnelBehaviorColor.Update(TimeElapsedInSeconds);
            TunnelBehaviorSize.Update(TimeElapsedInSeconds);
            TunnelBehaviorRotation.Update(TimeElapsedInSeconds);
            TunnelBehaviorDirection.Update(TimeElapsedInSeconds);

            CreateSimpleBackground(TimeElapsedInSeconds);
        }

        public void UpdateColored(double TimeElapsedInSeconds)
        {
            TunnelBehaviorSpeed.Update(TimeElapsedInSeconds);
            TunnelBehaviorColor.Update(TimeElapsedInSeconds);
            TunnelBehaviorSize.Update(TimeElapsedInSeconds);
            TunnelBehaviorRotation.Update(TimeElapsedInSeconds);
            TunnelBehaviorDirection.Update(TimeElapsedInSeconds);

            CreateAnimatedBackground(TimeElapsedInSeconds);
        }

        private void CreateSimpleBackground(double TimeElapsedInSeconds)
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
                Vector3 CurrentRightDistance = Vector3.Transform(Up, Matrix.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(X + 2))) * CylinderSize;
                Vector3 NextRightDistance = Vector3.Transform(Up, Matrix.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(X + SegmentIncrement + 2))) * CylinderSize;

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

        private void CreateAnimatedBackground(double TimeElapsedInSeconds)
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
            float SpeedX = (float)(Math.Cos(TunnelBehaviorDirection.ActiveDirection) * TunnelBehaviorSpeed.ActiveSpeed * TimeElapsedInSeconds);
            float SpeedY = (float)(Math.Sin(TunnelBehaviorDirection.ActiveDirection) * TunnelBehaviorSpeed.ActiveSpeed * TimeElapsedInSeconds);
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

        public void Draw(CustomSpriteBatch g)
        {
            float aspectRatio = Constants.Width / Constants.Height;

            int DrawOffset = 700;
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
        }

        public void DrawAnimated(CustomSpriteBatch g)
        {
            float aspectRatio = Constants.Width / Constants.Height;

            int DrawOffset = 700;
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

        }
    }
}
