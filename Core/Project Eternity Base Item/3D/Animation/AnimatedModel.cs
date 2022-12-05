using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.Core
{
    public class AnimatedModel
    {
        public readonly string FilePath = "";

        private Model OriginalModel = null;
        private ModelAnimationInfo AnimationInformation = null;
        private List<AnimationBone> ListBone = new List<AnimationBone>();
        private Animation3D ActiveAnimation = null;
        private Dictionary<string, Animation3D> ListAnimation = new Dictionary<string, Animation3D>();
        private Dictionary<string, AnimatedModel> ListAnimationModel = new Dictionary<string, AnimatedModel>();

        private float AnimationTimeElapsed = 0;
        public bool IsLooping = false;

        public AnimatedModel(string FilePath)
        {
            this.FilePath = FilePath;
        }

        public AnimatedModel(AnimatedModel Clone)
        {
            FilePath = Clone.FilePath;
            OriginalModel = Clone.OriginalModel;
            AnimationInformation = ((ModelAnimationInfo)OriginalModel.Tag).Clone();

            foreach (ModelBone ActiveBone in OriginalModel.Bones)
            {
                AnimationBone NewBone = new AnimationBone(ActiveBone.Name, ActiveBone.Transform, ActiveBone.Parent != null ? ListBone[ActiveBone.Parent.Index] : null);

                ListBone.Add(NewBone);
            }

            ActiveAnimation = AnimationInformation.ListAnimation[0];
        }

        public void LoadContent(ContentManager Content)
        {
            this.OriginalModel = Content.Load<Model>(FilePath);
            AnimationInformation = ((ModelAnimationInfo)OriginalModel.Tag).Clone();

            foreach (ModelBone ActiveBone in OriginalModel.Bones)
            {
                AnimationBone NewBone = new AnimationBone(ActiveBone.Name, ActiveBone.Transform, ActiveBone.Parent != null ? ListBone[ActiveBone.Parent.Index] : null);

                ListBone.Add(NewBone);
            }

            ActiveAnimation = AnimationInformation.ListAnimation[0];
        }

        public void AddAnimation(string AnimationPath, string AnimationName, ContentManager Content)
        {
            AnimatedModel NewAnimation = new AnimatedModel(AnimationPath);
            NewAnimation.LoadContent(Content);

            foreach (BoneTimeline ActiveTimeline in NewAnimation.AnimationInformation.ListAnimation[0].ListTimeline)
            {
                ActiveTimeline.AssignBoneFromBaseModel(this.FindBone(ActiveTimeline.Name));
            }

            ListAnimation.Add(AnimationName, NewAnimation.AnimationInformation.ListAnimation[0]);
            ListAnimationModel.Add(AnimationName, NewAnimation);
        }

        public void PlayAnimation(string AnimationName)
        {
            if (ActiveAnimation != ListAnimation[AnimationName])
            {
                ActiveAnimation = ListAnimation[AnimationName];
                Rewind();
            }
        }

        public AnimationBone FindBone(string name)
        {
            foreach (AnimationBone ActiveTimelineObject in ListBone)
            {
                if (ActiveTimelineObject.Name == name)
                    return ActiveTimelineObject;
            }

            return null;
        }

        public void Rewind()
        {
            AnimationTimeElapsed = 0;
            foreach (BoneTimeline ActiveBone in ActiveAnimation.ListTimeline)
            {
                ActiveBone.Rewind();
            }
        }

        public void Update(GameTime gameTime)
        {
            AnimationTimeElapsed = AnimationTimeElapsed + (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (ActiveAnimation.IsLooping && AnimationTimeElapsed >= (float)ActiveAnimation.Duration)
            {
                Rewind();
            }
            else
            {
                foreach (BoneTimeline ActiveBone in ActiveAnimation.ListTimeline)
                {
                    ActiveBone.SetPosition(AnimationTimeElapsed);
                }
            }
        }

        public void Draw(Matrix View, Matrix Projection, Matrix World)
        {
            if (OriginalModel == null)
                return;

            Matrix[] ArrayBoneTransform = new Matrix[AnimationInformation.ListBoneIndex.Count];

            for (int B = 0; B < AnimationInformation.ListBoneIndex.Count; B++)
            {
                AnimationBone ActiveBone = ListBone[AnimationInformation.ListBoneIndex[B]];
                ArrayBoneTransform[B] = ActiveBone.InverseTransform * ActiveBone.AbsoluteTransform;
            }

            foreach (ModelMesh ActiveMesh in OriginalModel.Meshes)
            {
                foreach (Effect ActiveEffect in ActiveMesh.Effects)
                {
                    if (ActiveEffect is BasicEffect)
                    {
                        BasicEffect BaseEffect = ActiveEffect as BasicEffect;
                        BaseEffect.World = ListBone[ActiveMesh.ParentBone.Index].AbsoluteTransform * World;
                        BaseEffect.View = View;
                        BaseEffect.Projection = Projection;
                        BaseEffect.EnableDefaultLighting();
                        BaseEffect.PreferPerPixelLighting = true;
                        BaseEffect.DirectionalLight0.DiffuseColor = new Vector3(5.7f, 5.7f, 5.7f);
                        BaseEffect.DirectionalLight0.Direction = new Vector3(0.2f, -0.4f, -0.8f);
                        BaseEffect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0);
                    }
                    else if (ActiveEffect is SkinnedEffect)
                    {
                        SkinnedEffect SkinEffect = ActiveEffect as SkinnedEffect;
                        SkinEffect.World = ListBone[ActiveMesh.ParentBone.Index].AbsoluteTransform * World;
                        SkinEffect.View = View;
                        SkinEffect.Projection = Projection;
                        SkinEffect.EnableDefaultLighting();
                        SkinEffect.PreferPerPixelLighting = true;
                        SkinEffect.SetBoneTransforms(ArrayBoneTransform);
                        SkinEffect.DirectionalLight0.DiffuseColor = new Vector3(5.7f, 5.7f, 5.7f);
                        SkinEffect.DirectionalLight0.Direction = new Vector3(0.2f, -0.4f, -0.8f);
                        SkinEffect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0);
                    }
                }

                ActiveMesh.Draw();
            }
        }
    }
}
