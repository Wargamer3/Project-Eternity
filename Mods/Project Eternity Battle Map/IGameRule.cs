using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public interface IGameRule
    {
        void Update(GameTime gameTime);
        void OnSquadDefeated(int DefeatedSquadPlayerIndex, Squad DefeatedSquad, Unit UnitDefeated);
        void BeginDraw(CustomSpriteBatch g);
        void Draw(CustomSpriteBatch g);
    }
}