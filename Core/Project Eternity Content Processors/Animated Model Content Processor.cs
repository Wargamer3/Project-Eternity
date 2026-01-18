using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.ContentProcessor
{
    [ContentProcessor(DisplayName = "Animated Model - Project Eternity")]
    public class AnimationProcessor : ModelProcessor
    {
        private const float TinyLength = 1e-8f;
        private const float TinyCosAngle = 0.9999999f;

        private ModelContent ActiveModel;

        private ModelAnimationInfo AnimationInfo = new ModelAnimationInfo();

        private Matrix[] ArrayBoneTransforms;
        private Dictionary<string, int> DicBoneIndexByName = new Dictionary<string, int>();

        public AnimationProcessor()
        {
        }

        public override ModelContent Process(NodeContent Input, ContentProcessorContext Context)
        {
            ProcessSkeleton(Input);

            ActiveModel = base.Process(Input, Context);

            ProcessAnimations(ActiveModel, Input);

            ActiveModel.Tag = AnimationInfo;

            return ActiveModel;
        }

        private void ProcessSkeleton(NodeContent Input)
        {
            BoneContent RootNode = MeshHelper.FindSkeleton(Input);

            if (RootNode == null)
                return;

            List<NodeContent> ListNode = FlattenTransforms(Input, RootNode);

            Dictionary<NodeContent, int> BoneIndexByNode = new Dictionary<NodeContent, int>();
            for (int N = 0; N < ListNode.Count; N++)
            {
                BoneIndexByNode.Add(ListNode[N], N);
            }

            IList<BoneContent> ListBone = MeshHelper.FlattenSkeleton(RootNode);
            foreach (BoneContent ActiveBone in ListBone)
            {
                AnimationInfo.ListBoneIndex.Add(BoneIndexByNode[ActiveBone]);
            }

            SwapSkinnedMaterial(Input, new Dictionary<MaterialContent, SkinnedMaterialContent>());
        }

        private List<NodeContent> FlattenTransforms(NodeContent Input, BoneContent RootNode)
        {
            foreach (NodeContent ActiveChildNode in Input.Children)
            {
                if (ActiveChildNode == RootNode)
                    continue;

                if (IsSkinned(ActiveChildNode))
                {
                    FlattenAllTransforms(ActiveChildNode);
                }
            }

            TrimSkeleton(RootNode);

            List<NodeContent> ListNode = new List<NodeContent>();
            FlattenHeirarchy(ListNode, Input);

            return ListNode;
        }

        private void FlattenAllTransforms(NodeContent ActiveNode)
        {
            // Bake the local transform into the actual geometry.
            MeshHelper.TransformScene(ActiveNode, ActiveNode.Transform);

            // Having baked it, we can now set the local
            // coordinate system back to identity.
            ActiveNode.Transform = Matrix.Identity;

            foreach (NodeContent ActiveChildNode in ActiveNode.Children)
            {
                FlattenAllTransforms(ActiveChildNode);
            }
        }

        private void FlattenHeirarchy(List<NodeContent> ListNode, NodeContent ActiveNode)
        {
            ListNode.Add(ActiveNode);
            foreach (NodeContent child in ActiveNode.Children)
            {
                FlattenHeirarchy(ListNode, child);
            }
        }

        /// <summary>
        /// 3D Studio Max includes an extra help bone at the end of each
        /// IK chain that doesn't effect the skinning system and is 
        /// redundant as far as any game is concerned.  This function
        /// looks for children who's name ends with "Nub" and removes
        /// them from the heirarchy.
        /// </summary>
        /// <param name="RootNode">Root of the skeleton tree</param>
        private void TrimSkeleton(NodeContent RootNode)
        {
            List<NodeContent> ListNodeToDelete = new List<NodeContent>();

            foreach (NodeContent ActiveChildNode in RootNode.Children)
            {
                if (ActiveChildNode.Name.EndsWith("Nub") || ActiveChildNode.Name.EndsWith("Footsteps"))
                    ListNodeToDelete.Add(ActiveChildNode);
                else
                    TrimSkeleton(ActiveChildNode);
            }

            foreach (NodeContent child in ListNodeToDelete)
            {
                RootNode.Children.Remove(child);
            }
        }

        private bool IsSkinned(NodeContent NodeToCheck)
        {
            MeshContent ActiveMesh = NodeToCheck as MeshContent;
            if (ActiveMesh != null)
            {
                foreach (GeometryContent ActiveGeometry in ActiveMesh.Geometry)
                {
                    foreach (VertexChannel ActiveChannel in ActiveGeometry.Vertices.Channels)
                    {
                        if (ActiveChannel is VertexChannel<BoneWeightCollection>)
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// If a node is skinned, we need to use the skinned model 
        /// effect rather than basic effect. This function runs through the 
        /// geometry and finds the meshes that have bone weights associated 
        /// and swaps in the skinned effect. 
        /// </summary>
        /// <param name="node"></param>
        private void SwapSkinnedMaterial(NodeContent node, Dictionary<MaterialContent, SkinnedMaterialContent> toSkinnedMaterial)
        {
            MeshContent ActiveMesh = node as MeshContent;
            if (ActiveMesh != null)
            {
                foreach (GeometryContent geometry in ActiveMesh.Geometry)
                {
                    bool swap = false;
                    foreach (VertexChannel vchannel in geometry.Vertices.Channels)
                    {
                        if (vchannel is VertexChannel<BoneWeightCollection>)
                        {
                            swap = true;
                            break;
                        }
                    }

                    if (swap)
                    {
                        if (toSkinnedMaterial.ContainsKey(geometry.Material))
                        {
                            geometry.Material = toSkinnedMaterial[geometry.Material];
                        }
                        else
                        {
                            SkinnedMaterialContent smaterial = new SkinnedMaterialContent();
                            BasicMaterialContent bmaterial = geometry.Material as BasicMaterialContent;

                            smaterial.Alpha = bmaterial.Alpha;
                            smaterial.DiffuseColor = bmaterial.DiffuseColor;
                            smaterial.EmissiveColor = bmaterial.EmissiveColor;
                            smaterial.SpecularColor = bmaterial.SpecularColor;
                            smaterial.SpecularPower = bmaterial.SpecularPower;
                            smaterial.Texture = bmaterial.Texture;
                            smaterial.WeightsPerVertex = 4;

                            toSkinnedMaterial.Add(geometry.Material, smaterial);
                            geometry.Material = smaterial;
                        }
                    }
                }
            }

            foreach (NodeContent child in node.Children)
            {
                SwapSkinnedMaterial(child, toSkinnedMaterial);
            }
        }

        private void ProcessAnimations(ModelContent ActiveModel, NodeContent Input)
        {
            Dictionary<string, Animation3D> DicAnimationByName = new Dictionary<string, Animation3D>();
            for (int B = 0; B < ActiveModel.Bones.Count; B++)
            {
                DicBoneIndexByName[ActiveModel.Bones[B].Name] = B;
            }

            ArrayBoneTransforms = new Matrix[ActiveModel.Bones.Count];

            ProcessAnimationsRecursive(Input, DicAnimationByName);

            if (AnimationInfo.ListAnimation.Count == 0)
            {
                Animation3D NewAnimation = new Animation3D();
                AnimationInfo.ListAnimation.Add(NewAnimation);

                string DefaultAnimationName = "Take 001";

                DicAnimationByName.Add(DefaultAnimationName, NewAnimation);

                NewAnimation.Name = DefaultAnimationName;
                foreach (ModelBoneContent bone in ActiveModel.Bones)
                {
                    BoneTimeline clipBone = new BoneTimeline();
                    clipBone.Name = bone.Name;

                    NewAnimation.ListTimeline.Add(clipBone);
                }
            }

            // Ensure all animations have a first key frame for every bone
            foreach (Animation3D clip in AnimationInfo.ListAnimation)
            {
                for (int B = 0; B < DicBoneIndexByName.Count; B++)
                {
                    List<KeyFrame3D> keyframes = clip.ListTimeline[B].ListKeyFrame;
                    if (keyframes.Count == 0 || keyframes[0].Time > 0)
                    {
                        KeyFrame3D keyframe = new KeyFrame3D();
                        keyframe.Time = 0;
                        keyframe.Transform = ArrayBoneTransforms[B];
                        keyframes.Insert(0, keyframe);
                    }
                }
            }
        }

        private void ProcessAnimationsRecursive(NodeContent input, Dictionary<string, Animation3D> DicAnimationByName)
        {
            int InputBoneIndex;
            if (DicBoneIndexByName.TryGetValue(input.Name, out InputBoneIndex))
            {
                ArrayBoneTransforms[InputBoneIndex] = input.Transform;
            }

            foreach (KeyValuePair<string, AnimationContent> ActiveAnimation in input.Animations)
            {
                Animation3D NewAnimation = new Animation3D();
                string AnimationName = ActiveAnimation.Key;

                if (!DicAnimationByName.TryGetValue(AnimationName, out NewAnimation))
                {
                    NewAnimation = new Animation3D();
                    AnimationInfo.ListAnimation.Add(NewAnimation);

                    DicAnimationByName.Add(AnimationName, NewAnimation);

                    NewAnimation.Name = AnimationName;
                    foreach (ModelBoneContent bone in ActiveModel.Bones)
                    {
                        BoneTimeline NewTimeline = new BoneTimeline();
                        NewTimeline.Name = bone.Name;

                        NewAnimation.ListTimeline.Add(NewTimeline);
                    }
                }

                if (ActiveAnimation.Value.Duration.TotalSeconds > NewAnimation.Duration)
                    NewAnimation.Duration = ActiveAnimation.Value.Duration.TotalSeconds;

                foreach (KeyValuePair<string, AnimationChannel> ActiveChannel in ActiveAnimation.Value.Channels)
                {
                    int BoneIndex;
                    if (!DicBoneIndexByName.TryGetValue(ActiveChannel.Key, out BoneIndex))
                        continue;

                    if (!IsBoneUsed(BoneIndex))
                        continue;

                    LinkedList<KeyFrame3D> ListNewKeyFrame = new LinkedList<KeyFrame3D>();

                    foreach (AnimationKeyframe ActiveKeyFrame in ActiveChannel.Value)
                    {
                        KeyFrame3D NewKeyFrame = new KeyFrame3D();

                        NewKeyFrame.Time = ActiveKeyFrame.Time.TotalSeconds;
                        NewKeyFrame.Transform = ActiveKeyFrame.Transform;

                        ListNewKeyFrame.AddLast(NewKeyFrame);
                    }

                    LinearKeyFrameReduction(ListNewKeyFrame);
                    foreach (KeyFrame3D keyframe in ListNewKeyFrame)
                    {
                        NewAnimation.ListTimeline[BoneIndex].ListKeyFrame.Add(keyframe);
                    }
                }
            }

            foreach (NodeContent ActiveChild in input.Children)
            {
                ProcessAnimationsRecursive(ActiveChild, DicAnimationByName);
            }
        }

        private void LinearKeyFrameReduction(LinkedList<KeyFrame3D> ListKeyFrame)
        {
            if (ListKeyFrame.Count < 3)
                return;

            LinkedListNode<KeyFrame3D> CurrentNode = ListKeyFrame.First.Next;

            do
            {
                LinkedListNode<KeyFrame3D> NextNode = CurrentNode.Next;
                LinkedListNode<KeyFrame3D> PreviousNode = CurrentNode.Previous;

                //Smimulate where a node would based on the current node position in time between the previous and next node.
                //Is the simulated node is close to the actual node, remove the real node and let the engine simulate it.
                float CurrentNodeWeight = (float)((CurrentNode.Value.Time - PreviousNode.Value.Time) / (NextNode.Value.Time - PreviousNode.Value.Time));

                Vector3 TranslationDifference = Vector3.Lerp(PreviousNode.Value.Translation, NextNode.Value.Translation, CurrentNodeWeight);
                Quaternion RotationDifference = Quaternion.Slerp(PreviousNode.Value.Rotation, NextNode.Value.Rotation, CurrentNodeWeight);

                if ((TranslationDifference - CurrentNode.Value.Translation).LengthSquared() < TinyLength &&
                   Quaternion.Dot(RotationDifference, CurrentNode.Value.Rotation) > TinyCosAngle)
                {
                    ListKeyFrame.Remove(CurrentNode);
                }

                CurrentNode = NextNode;

            } while (CurrentNode.Next != null);
        }

        bool IsBoneUsed(int BoneIndex)
        {
            foreach (ModelMeshContent ActiveMesh in ActiveModel.Meshes)
            {
                if (ActiveMesh.ParentBone.Index == BoneIndex)
                {
                    return true;
                }
            }

            foreach (int ActiveBoneIndex in AnimationInfo.ListBoneIndex)
            {
                if (BoneIndex == ActiveBoneIndex)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
