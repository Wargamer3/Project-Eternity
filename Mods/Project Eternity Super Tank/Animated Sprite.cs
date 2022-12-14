using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SuperTankScreen
{
    public class AnimatedSprite
    {
        public float AnimationSpeed;

        public Color[] Mask;
        public Texture2D ActiveSprite;
        public int SpriteWidth;
        public int SpriteHeight;
        public Rectangle SpriteSource;
        public Vector2 Origin;
        public Vector2 Position;
        public readonly int AnimationFrameCount;
        public float Angle;
        public Vector2 Scale;
        private Matrix TransformationMatrix;
		public Rectangle CollisionBox;
        public static Texture2D sprPixel;

        protected AnimatedSprite()
        {
        }

        public AnimatedSprite(Texture2D ActiveSprite, Color[] Mask, int AnimationFrameCount, Vector2 Origin, float AnimationSpeed = 0.5f)
        {
            this.ActiveSprite = ActiveSprite;
            this.Mask = Mask;
            this.Origin = Origin;
            this.AnimationSpeed = AnimationSpeed;
            this.AnimationFrameCount = AnimationFrameCount;
            Scale = Vector2.One;

            SpriteWidth = (int)Math.Ceiling(ActiveSprite.Width / (float)AnimationFrameCount);
            SpriteHeight = ActiveSprite.Height;

            SpriteSource = new Microsoft.Xna.Framework.Rectangle(0, 0, SpriteWidth, SpriteHeight);
        }

        public AnimatedSprite(Texture2D ActiveSprite, int AnimationFrameCount, Vector2 Origin, float AnimationSpeed = 0.5f)
            : this(ActiveSprite, null, AnimationFrameCount, Origin, AnimationSpeed)
        {
        }

        public Color[][] CreateMask()
        {
            int MaskSize = SpriteWidth * SpriteHeight;
            Color[][] Mask = new Color[AnimationFrameCount][];

            for (int i = 0; i < AnimationFrameCount; i++)
            {
                Mask[i] = new Color[MaskSize];
                ActiveSprite.GetData(0, new Rectangle(i * SpriteWidth, 0, SpriteWidth, SpriteHeight), Mask[i], 0, MaskSize);
            }
            return Mask;
        }

        public void UpdateTransformationMatrix()
        {
            TransformationMatrix =
                Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(Angle) *
                Matrix.CreateTranslation(new Vector3(Position, 0.0f));

            GetCollisionBox();
        }
        public void UpdateTransformationMatrix(Vector2 Scale)
        {
            TransformationMatrix =
                Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                 Matrix.CreateScale(new Vector3(Scale, 1.0f)) *
                Matrix.CreateRotationZ(Angle) *
                Matrix.CreateTranslation(new Vector3(Position, 0.0f));

            GetCollisionBox();
        }

        private void GetCollisionBox()
        {
            float MinX = 0;
            float MaxX = SpriteWidth;
            float MinY = 0;
            float MaxY = SpriteHeight;

            // Get all four corners in local space
            Vector2 leftTop = new Vector2(MinX, MinY);
            Vector2 rightTop = new Vector2(MaxX, MinY);
            Vector2 leftBottom = new Vector2(MinX, MaxY);
            Vector2 rightBottom = new Vector2(MaxX, MaxY);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref TransformationMatrix, out leftTop);
            Vector2.Transform(ref rightTop, ref TransformationMatrix, out rightTop);
            Vector2.Transform(ref leftBottom, ref TransformationMatrix, out leftBottom);
            Vector2.Transform(ref rightBottom, ref TransformationMatrix, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            CollisionBox = new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        public bool PixelIntersect(AnimatedSprite Other)
		{
			return CollisionBox.Intersects(Other.CollisionBox) &&
				IntersectPixels(TransformationMatrix, SpriteWidth, SpriteHeight, Mask, Scale.X < 0, Other.TransformationMatrix, Other.SpriteWidth, Other.SpriteHeight, Other.Mask, Other.Scale.X < 0);
        }

        public static bool IntersectPixels(
			Matrix transformA, int widthA, int heightA, Color[] dataA, bool InvertXA,
			Matrix transformB, int widthB, int heightB, Color[] dataB, bool InvertXB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
					if (0 <= xB && xB < widthB &&
					                   0 <= yB && yB < heightB)
					{
						// Get the colors of the overlapping pixels
						Color colorA;
						Color colorB;
						if (InvertXA)
							colorA = dataA [widthA - 1 - xA + yA * widthA];
						else
							colorA = dataA [xA + yA * widthA];
						if (InvertXB)
							colorB = dataB [widthB - 1 - xB + yB * widthB];
						else
							colorB = dataB [xB + yB * widthB];

						// If both pixels are not completely transparent,
						if (colorA.A != 0 && colorB.A != 0)
						{
							// then an intersection has been found
							return true;
						}
					}

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }

        public void Draw(CustomSpriteBatch g, int AnimationIndex, Vector2 Position, float Angle, Color DrawingColor)
        {
            SpriteSource.X = AnimationIndex * SpriteWidth;
            g.Draw(ActiveSprite, Position, SpriteSource, DrawingColor, Angle, Origin, Scale, SpriteEffects.None, 0);
        }
        public void Draw(CustomSpriteBatch g, int AnimationIndex, Vector2 Position, Vector2 Scale, float Angle, Color DrawingColor)
        {
            SpriteSource.X = AnimationIndex * SpriteWidth;
            g.Draw(ActiveSprite, Position, SpriteSource, DrawingColor, Angle, Origin, Scale, SpriteEffects.None, 0);
        }
    }
}
