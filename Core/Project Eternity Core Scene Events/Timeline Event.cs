using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.Core.Scene
{
    public class TimelineEvent : SceneEvent
    {
        AnimationTimelineViewer NewEditor = new AnimationTimelineViewer();

        public class SceneAnimation : AnimationClassEditor
        {
            public SceneAnimation()
                : base(string.Empty)
            {
                ListAnimationLayer = new AnimationLayerHolder();
                ListAnimationLayer.EngineLayer = GameEngineLayer.EmptyGameEngineLayer(this);
            }
        }

        private SceneAnimation ActiveAnimation;

        public TimelineEvent()
            : base("Timeline")
        {
            ActiveAnimation = new SceneAnimation();
            NewEditor.SetActiveAnimation(ActiveAnimation);
        }

        public override Control GetTimelineEditor()
        {
            return NewEditor;
        }

        public override void Load(BinaryReader BR)
        {
            throw new NotImplementedException();
        }

        public override void Save(BinaryWriter BW)
        {
            throw new NotImplementedException();
        }

        public override SceneEvent Copy()
        {
            return new TimelineEvent();
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void OnMouseDown(int MouseX, int MouseY, MouseButtons MouseButton)
        {

        }

        public override void OnMouseMove(int MouseX, int MouseY, MouseButtons MouseButton)
        {
        }

        public override void OnMouseUp(int MouseX, int MouseY, MouseButtons MouseButton)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            ActiveAnimation.Draw(g);
        }
    }
}
