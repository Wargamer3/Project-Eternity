﻿using System;
using Microsoft.Xna.Framework;

namespace ProjectEternity.Core.Graphics
{
	public class AnimationBone
    {
        public string Name = "";

        private readonly AnimationBone Parent = null;

        private readonly Matrix Transform = Matrix.Identity;
        private Vector3 Scale = Vector3.One;
        private Vector3 Translation = Vector3.Zero;
        private Quaternion Rotation = Quaternion.Identity;

        public readonly Matrix InverseTransform;
        public Matrix AbsoluteTransform = Matrix.Identity;

        public AnimationBone(string Name, Matrix Transform, AnimationBone Parent)
        {
            this.Name = Name;
            this.Parent = Parent;

            // I am not supporting scaling in animation in this
            // example, so I extract the bind scaling from the 
            // bind transform and save it. 

            this.Scale = new Vector3(Transform.Right.Length(),
                Transform.Up.Length(), Transform.Backward.Length());

            Transform.Right = Transform.Right / Scale.X;
            Transform.Up = Transform.Up / Scale.Y;
            Transform.Backward = Transform.Backward / Scale.Y;

            this.Transform = Transform;

            ComputeAbsoluteTransform();
            InverseTransform = Matrix.Invert(AbsoluteTransform);
        }

        private void ComputeAbsoluteTransform()
        {
            Matrix TransformMatrix = Matrix.CreateScale(Scale) *
                Matrix.CreateFromQuaternion(Rotation) *
                Matrix.CreateTranslation(Translation) *
                Transform;

            if (Parent != null)
            {
                AbsoluteTransform = TransformMatrix * Parent.AbsoluteTransform;
            }
            else
            {
                AbsoluteTransform = TransformMatrix;
            }
        }

        public void SetCompleteTransform(Matrix m)
        {
            Matrix setTo = m * Matrix.Invert(Transform);

            Translation = setTo.Translation;
            Rotation = Quaternion.CreateFromRotationMatrix(setTo);
            ComputeAbsoluteTransform();
        }
    }
}
