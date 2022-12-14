using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public interface BattleMapOverlay
    {
        void SetCrossfadeValue(double Value);
        void Update(GameTime gameTime);
        void BeginDraw(CustomSpriteBatch g);
        void Draw(CustomSpriteBatch g);
        void EndDraw(CustomSpriteBatch g);
        void Reset();
    }
}
