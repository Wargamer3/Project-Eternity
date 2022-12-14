using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Item
{
    public interface IUIElement
    {
        void Select();
        void Unselect();
        void Enable();
        void Disable();
        void Update(GameTime gameTime);
        void Draw(CustomSpriteBatch g);
    }
}