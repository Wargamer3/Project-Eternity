using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace ProjectEternity.Core.Scene
{
    public abstract class SceneEvent
    {
        protected readonly SceneScreen Owner;

        public readonly string SceneEventType;

        public SceneEvent(string SceneEventType)
        {
            this.SceneEventType = SceneEventType;
        }

        public abstract void Load(BinaryReader BR);

        public abstract void Save(BinaryWriter BW);

        public abstract SceneEvent Copy();

        public abstract void Update(GameTime gameTime);

        public abstract void OnMouseDown(int MouseX, int MouseY, MouseButtons MouseButton);
        public abstract void OnMouseMove(int MouseX, int MouseY, MouseButtons MouseButton);
        public abstract void OnMouseUp(int MouseX, int MouseY, MouseButtons MouseButton);

        public abstract void Draw(CustomSpriteBatch g);

        public virtual Control GetTimelineEditor()
        {
            return null;
        }
    }
}
