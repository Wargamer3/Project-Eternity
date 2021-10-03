using Microsoft.Xna.Framework;

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