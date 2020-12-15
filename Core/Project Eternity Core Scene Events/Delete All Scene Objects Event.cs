using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace ProjectEternity.Core.Scene
{
    public class DeleteAllSceneObjectsEvent : SceneEvent
    {
        public DeleteAllSceneObjectsEvent()
            : base("Delete All Scene Objects")
        {
        }

        public override void Load(BinaryReader BR)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            Owner.DicSceneObjectByName.Clear();
            Owner.ListActiveSceneEvent.Remove(this);
        }

        public override void Save(BinaryWriter BW)
        {
        }

        public override SceneEvent Copy()
        {
            return new DeleteAllSceneObjectsEvent();
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
