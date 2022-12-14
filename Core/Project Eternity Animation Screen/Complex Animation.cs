using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class ComplexAnimation : AnimationClass
    {
        public SpriteEffects ActiveSpriteEffects;
        public Vector2 Position;
        public float Scale;
        public float Angle;

        protected string CurrentAnimation;
        protected Dictionary<string, AnimationClass> DicAnimation;
        protected Dictionary<string, PartialAnimation> DicPartialAnimation;
        protected List<RenderTarget2D> ListRenderTarget;
        protected Dictionary<string, VisibleTimeline> DicActiveAnimationObject;//Used to access a particular AnimationObject part at any time.
        protected List<PartialAnimation> ListActivePartialAnimation;
        protected List<string> ListActivePartialAnimationName;

        public ComplexAnimation()
            : base()
        {
            Scale = 1;
            RequireFocus = true;
            RequireDrawFocus = true;
            IsOnTop = false;
            ListRenderTarget = new List<RenderTarget2D>();
            DicActiveAnimationObject = new Dictionary<string, VisibleTimeline>();
            ListActivePartialAnimation = new List<PartialAnimation>();
            ListActivePartialAnimationName = new List<string>();
            ActiveSpriteEffects = SpriteEffects.None;

            DicAnimation = new Dictionary<string, AnimationClass>();
            DicPartialAnimation = new Dictionary<string, PartialAnimation>();
        }

        public override void Load()
        {
            InitMembers();
        }

        public AnimationClass SetAnimation(string NewAnimation)
        {
            if (CurrentAnimation == NewAnimation)
                return DicAnimation[NewAnimation];

            DicActiveAnimationObject.Clear();

            AnimationClass NewAnimationClass;

            DicAnimation.TryGetValue(NewAnimation, out NewAnimationClass);

            if (NewAnimationClass == null)
            {
                NewAnimationClass = CreateAnimation(NewAnimation, false);
                DicAnimation.Add(NewAnimation, NewAnimationClass);
            }

            CurrentAnimation = NewAnimation;

            if (ListAnimationLayer != null)
            {
                for (int L = 0; L < ListAnimationLayer.Count; L++)
                {
                    foreach (VisibleTimeline ActiveBitmap in ListAnimationLayer[L].ListVisibleObject)
                    {
                        ActiveBitmap.OnDeathFrame(this);
                    }
                    foreach (MarkerTimeline ActiveMarker in ListAnimationLayer[L].ListActiveMarker)
                    {
                        ActiveMarker.OnDeathFrame(this);
                    }
                    foreach (PolygonCutterTimeline ActivePolygonCutter in ListAnimationLayer[L].ListPolygonCutter)
                    {
                        ActivePolygonCutter.OnDeathFrame(this);
                    }
                }
            }

            ListAnimationLayer = DicAnimation[CurrentAnimation].ListAnimationLayer;
            ActiveKeyFrame = 0;

            LoopStart = DicAnimation[CurrentAnimation].LoopStart;
            LoopEnd = DicAnimation[CurrentAnimation].LoopEnd;

            for (int L = 0; L < ListAnimationLayer.Count; L++)
            {
                ListAnimationLayer[L].ResetAnimationLayer();
            }

            Init();

            return NewAnimationClass;
        }

        public PartialAnimation AddPartialAnimation(string NewAnimation)
        {
            if (ListActivePartialAnimationName.Contains(NewAnimation))
                return ListActivePartialAnimation[ListActivePartialAnimationName.IndexOf(NewAnimation)];

            PartialAnimation TempAnimation;
            DicPartialAnimation.TryGetValue(NewAnimation, out TempAnimation);

            if (TempAnimation == null)
            {
                TempAnimation = (PartialAnimation)CreateAnimation(NewAnimation, true);
                DicPartialAnimation.Add(NewAnimation, TempAnimation);
            }
            ListActivePartialAnimation.Add(TempAnimation);
            ListActivePartialAnimationName.Add(NewAnimation);

            ClearPartialAnimation(TempAnimation);

            return TempAnimation;
        }

        public void RemovePartialAnimation(string ActivePartialAnimation)
        {
            int Index = ListActivePartialAnimationName.IndexOf(ActivePartialAnimation);
            if (Index >= 0)
            {
                ListActivePartialAnimation[Index].ActiveKeyFrame = ListActivePartialAnimation[Index].LoopStart;
                ListActivePartialAnimation.RemoveAt(Index);
                ListActivePartialAnimationName.RemoveAt(Index);
            }
        }

        public void RemovePartialAnimation(PartialAnimation ActivePartialAnimation)
        {
            int Index = ListActivePartialAnimation.IndexOf(ActivePartialAnimation);
            if (Index >= 0)
            {
                ListActivePartialAnimation[Index].ActiveKeyFrame = ListActivePartialAnimation[Index].LoopStart;
                ListActivePartialAnimation.RemoveAt(Index);
                ListActivePartialAnimationName.RemoveAt(Index);
            }
        }

        protected AnimationClass CreateAnimation(string AnimationName, bool IsPartialAnimation)
        {
            AnimationClass ActiveAnimation = null;

            if (IsPartialAnimation)
                ActiveAnimation = new PartialAnimation(AnimationName, this);
            else
                ActiveAnimation = new AnimationClass(AnimationName);

            ActiveAnimation.DicTimeline = DicTimeline;
            ActiveAnimation.LoadFromFile();

            for(int A = ActiveAnimation.ListAnimationLayer.Count - 1; A >= 0; --A)
            {
                ActiveAnimation.ListAnimationLayer[A].Owner = this;
            }

            return ActiveAnimation;
        }

        #region Animation class methods

        public override void OnVisibleTimelineSpawn(AnimationLayer ActiveLayer, VisibleTimeline ActiveBitmap)
        {
            base.OnVisibleTimelineSpawn(ActiveLayer, ActiveBitmap);

            if (DicActiveAnimationObject.ContainsKey(ActiveBitmap.Name))
                DicActiveAnimationObject[ActiveBitmap.Name] = ActiveBitmap;
            else
                DicActiveAnimationObject.Add(ActiveBitmap.Name, ActiveBitmap);
        }

        public override void OnMarkerTimelineSpawn(AnimationLayer ActiveLayer, MarkerTimeline ActiveMarker)
        {
            base.OnMarkerTimelineSpawn(ActiveLayer, ActiveMarker);

            if (DicActiveAnimationObject.ContainsKey(ActiveMarker.Name))
                DicActiveAnimationObject[ActiveMarker.Name] = ActiveMarker;
            else
                DicActiveAnimationObject.Add(ActiveMarker.Name, ActiveMarker);
        }

        public override void OnPolygonCutterTimelineSpawn(AnimationLayer ActiveLayer, PolygonCutterTimeline ActivePolygonCutter)
        {
            base.OnPolygonCutterTimelineSpawn(ActiveLayer, ActivePolygonCutter);

            if (DicActiveAnimationObject.ContainsKey(ActivePolygonCutter.Name))
                DicActiveAnimationObject[ActivePolygonCutter.Name] = ActivePolygonCutter;
            else
                DicActiveAnimationObject.Add(ActivePolygonCutter.Name, ActivePolygonCutter);
        }

        public override void OnVisibleTimelineDeath(VisibleTimeline RemovedBitmap)
        {
            base.OnVisibleTimelineDeath(RemovedBitmap);

            if (DicActiveAnimationObject.ContainsKey(RemovedBitmap.Name))
                DicActiveAnimationObject[RemovedBitmap.Name] = null;
        }

        public override void OnMarkerTimelineDeath(MarkerTimeline RemovedMarker)
        {
            base.OnMarkerTimelineDeath(RemovedMarker);

            if (DicActiveAnimationObject.ContainsKey(RemovedMarker.Name))
                DicActiveAnimationObject[RemovedMarker.Name] = null;
        }

        public override void OnPolygonCutterTimelineDeath(PolygonCutterTimeline RemovedPolygonCutter)
        {
            base.OnPolygonCutterTimelineDeath(RemovedPolygonCutter);

            if (DicActiveAnimationObject.ContainsKey(RemovedPolygonCutter.Name))
                DicActiveAnimationObject[RemovedPolygonCutter.Name] = null;
        }

        #endregion

        protected virtual void OnLoopEnd()
        {
            CurrentQuote = "";
            ListActiveSFX.Clear();
            DicActiveAnimationObject.Clear();

            for (int L = 0; L < ListAnimationLayer.Count; L++)
            {
                foreach (VisibleTimeline ActiveBitmap in ListAnimationLayer[L].ListVisibleObject)
                {
                    ActiveBitmap.OnDeathFrame(this);
                }
                foreach (MarkerTimeline ActiveMarker in ListAnimationLayer[L].ListActiveMarker)
                {
                    ActiveMarker.OnDeathFrame(this);
                }
                foreach (PolygonCutterTimeline ActivePolygonCutter in ListAnimationLayer[L].ListPolygonCutter)
                {
                    ActivePolygonCutter.OnDeathFrame(this);
                }

                ListAnimationLayer[L].ResetAnimationLayer();
            }

            ActiveKeyFrame = LoopStart;

            for (int F = 0; F <= ActiveKeyFrame; F++)
            {
                UpdateKeyFrame(F);
            }
        }

        protected virtual void OnPartialAnimationLoopEnd(PartialAnimation ActivePartialAnimation)
        {
            ClearPartialAnimation(ActivePartialAnimation);
        }

        private void ClearPartialAnimation(PartialAnimation ActivePartialAnimation)
        {
            for (int L = 0; L < ActivePartialAnimation.ListAnimationLayer.Count; L++)
            {
                foreach (VisibleTimeline ActiveBitmap in ActivePartialAnimation.ListAnimationLayer[L].ListVisibleObject)
                {
                    ActiveBitmap.OnDeathFrame(this);
                }
                foreach (MarkerTimeline ActiveMarker in ActivePartialAnimation.ListAnimationLayer[L].ListActiveMarker)
                {
                    ActiveMarker.OnDeathFrame(this);
                }
                foreach (PolygonCutterTimeline ActivePolygonCutter in ActivePartialAnimation.ListAnimationLayer[L].ListPolygonCutter)
                {
                    ActivePolygonCutter.OnDeathFrame(this);
                }
                ActivePartialAnimation.ListAnimationLayer[L].ResetAnimationLayer();
            }

            ActivePartialAnimation.ActiveKeyFrame = ActivePartialAnimation.LoopStart;

            for (int F = 0; F < ActivePartialAnimation.ActiveKeyFrame; F++)
                ActivePartialAnimation.UpdateKeyFrame(F);
        }

        public int GetPartialAnimationKeyFrame(string AnimationName)
        {
            for (int P = ListActivePartialAnimation.Count - 1; P >= 0; --P)
            {
                if (ListActivePartialAnimation[P].AnimationPath == AnimationName)
                    return ListActivePartialAnimation[P].ActiveKeyFrame;
            }

            return -1;
        }

        public override void Init()
        {
            base.Init();

            for (int A = 0; A < ListActivePartialAnimation.Count; A++)
            {
                ListActivePartialAnimation[A].Init();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ActiveKeyFrame >= LoopEnd)
            {
                OnLoopEnd();
            }

            for (int A = 0; A < ListActivePartialAnimation.Count; A++)
            {
                PartialAnimation ActivePartialAnimation = ListActivePartialAnimation[A];
                ActivePartialAnimation.Update(gameTime);
                foreach (KeyValuePair<string, VisibleTimeline> ActiveObject in ActivePartialAnimation.DicInheritedObject)
                {
                    if (ActiveObject.Value == null)
                        continue;

                    VisibleTimeline ActiveAnimationObject;
                    DicActiveAnimationObject.TryGetValue(ActiveObject.Key, out ActiveAnimationObject);
                    if (ActiveAnimationObject != null)
                    {
                        ActiveAnimationObject.Alpha = 0;
                    }
                }

                if (ActivePartialAnimation.ActiveKeyFrame >= ActivePartialAnimation.LoopEnd)
                {
                    OnPartialAnimationLoopEnd(ActivePartialAnimation);
                }
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            g.End();
            Matrix OriginalMatrix = TransformationMatrix;

            while (ListRenderTarget.Count < ListAnimationLayer.BasicLayerCount)
            {
                ListRenderTarget.Add(new RenderTarget2D(
                    GraphicsDevice,
                    GraphicsDevice.PresentationParameters.BackBufferWidth,
                    GraphicsDevice.PresentationParameters.BackBufferHeight));
            }

            for (int L = 0; L < ListAnimationLayer.BasicLayerCount; L++)
            {
                if (ListRenderTarget[L].Width != GraphicsDevice.PresentationParameters.BackBufferWidth ||
                    ListRenderTarget[L].Height != GraphicsDevice.PresentationParameters.BackBufferHeight)
                {
                    ListRenderTarget[L] = new RenderTarget2D(
                        GraphicsDevice,
                        GraphicsDevice.PresentationParameters.BackBufferWidth,
                        GraphicsDevice.PresentationParameters.BackBufferHeight);
                }

                GraphicsDevice.SetRenderTarget(ListRenderTarget[L]);
                GraphicsDevice.Clear(Color.Transparent);

                DrawLayer(g, ListAnimationLayer[L], false, false, null, true);

                for (int A = 0; A < ListActivePartialAnimation.Count; A++)
                {
                    TransformationMatrix = ListActivePartialAnimation[A].TransformationMatrix;
                    
                    if (L < ListActivePartialAnimation[A].ListAnimationLayer.BasicLayerCount)
                    {
                        DrawLayer(g, ListActivePartialAnimation[A].ListAnimationLayer[L], false, false, null, true);
                    }
                }
            }

            for (int A = 0; A < ListActivePartialAnimation.Count; A++)
            {
                TransformationMatrix = ListActivePartialAnimation[A].TransformationMatrix;

                for (int L = ListAnimationLayer.BasicLayerCount; L < ListActivePartialAnimation[A].ListAnimationLayer.BasicLayerCount; L++)
                {
                    DrawLayer(g, ListActivePartialAnimation[A].ListAnimationLayer[L], false, false, null, true);
                }
            }

            TransformationMatrix = OriginalMatrix;

            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        }

        private void DecomposeMatrix(ref Matrix matrix, out Vector2 position, out float rotation, out Vector2 scale)
        {
            Vector3 position3, scale3;
            Quaternion rotationQ;

            matrix.Decompose(out scale3, out rotationQ, out position3);

            Vector2 direction = Vector2.Transform(Vector2.UnitX, rotationQ);
            rotation = (float)Math.Atan2((double)(direction.Y), (double)(direction.X));
            position = new Vector2(position3.X, position3.Y);
            scale = new Vector2(scale3.X, scale3.Y);
        }

        public void Draw(CustomSpriteBatch g, Vector2 CameraOffset)
        {
            for (int L = ListRenderTarget.Count - 1; L >= 0; --L)
            {
                Vector2 DrawOrigin = AnimationOrigin.Position;
                if (ActiveSpriteEffects == SpriteEffects.FlipHorizontally)
                {
                    DrawOrigin.X = Constants.Width - AnimationOrigin.Position.X;
                }

                g.Draw(ListRenderTarget[L], Position - CameraOffset, null, Color.White, Angle, DrawOrigin, Scale, ActiveSpriteEffects, 0);
            }
        }
    }
}
