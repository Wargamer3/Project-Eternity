using Microsoft.Xna.Framework;

namespace ProjectEternity.Core
{
    public class KeyFrame3D
    {
        public double Time;
        public Quaternion Rotation;
        public Vector3 Translation;

        public Matrix Transform
        {
            get
            {
                return Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Translation);
            }
            set
            {
                Matrix transform = value;
                transform.Right = Vector3.Normalize(transform.Right);
                transform.Up = Vector3.Normalize(transform.Up);
                transform.Backward = Vector3.Normalize(transform.Backward);
                Rotation = Quaternion.CreateFromRotationMatrix(transform);
                Translation = transform.Translation;
            }
        }
    }
}
