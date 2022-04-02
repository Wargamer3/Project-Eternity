using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public interface BattleMapOverlay
    {
        void Update(GameTime gameTime);
        void BeginDraw(CustomSpriteBatch g);
        void Draw(CustomSpriteBatch g);
        void EndDraw(CustomSpriteBatch g);
    }
}
