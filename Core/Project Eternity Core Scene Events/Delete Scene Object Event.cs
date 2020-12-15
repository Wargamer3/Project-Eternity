using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace ProjectEternity.Core.Scene
{
    public class DeleteSceneObjectEvent : SceneEvent
    {
        private string ItemName;

        public DeleteSceneObjectEvent()
            : base("Delete Scene Object")
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
            return new DeleteSceneObjectEvent();
        }

        public override void Update(GameTime gameTime)
        {
            Owner.DicSceneObjectByName.Remove(ItemName);
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
