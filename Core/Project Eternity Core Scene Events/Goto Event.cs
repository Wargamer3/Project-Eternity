using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Scene
{
    public class GotoEvent : SceneEvent
    {
        private int GotoIndex;

        public GotoEvent()
            : base("Go to")
        {
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
            return new GotoEvent();
        }
        
        public override void Update(GameTime gameTime)
        {
            Owner.Goto(GotoIndex);
            Owner.ListActiveSceneEvent.Remove(this);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public override void OnMouseDown(int MouseX, int MouseY, MouseButtons MouseButton)
        {
            throw new NotImplementedException();
        }

        public override void OnMouseMove(int MouseX, int MouseY, MouseButtons MouseButton)
        {
            throw new NotImplementedException();
        }

        public override void OnMouseUp(int MouseX, int MouseY, MouseButtons MouseButton)
        {
            throw new NotImplementedException();
        }
    }
}
