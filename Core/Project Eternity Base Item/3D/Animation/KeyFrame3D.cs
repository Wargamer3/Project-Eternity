using Microsoft.Xna.Framework;
using System;

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

        public KeyFrame3D Clone()
        {
            KeyFrame3D NewKeyFrame3D = new KeyFrame3D();

            NewKeyFrame3D.Time = Time;
            NewKeyFrame3D.Rotation = Rotation;
            NewKeyFrame3D.Translation = Translation;

            return NewKeyFrame3D;
        }
    }
}
