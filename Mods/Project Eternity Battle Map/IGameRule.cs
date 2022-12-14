using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public interface IGameRule
    {
        void Init();
        void Update(GameTime gameTime);
        void OnNewTurn(int ActivePlayerIndex);
        void OnSquadDefeated(int AttackerSquadPlayerIndex, Squad AttackerSquad, int DefeatedSquadPlayerIndex, Squad DefeatedSquad);
        void OnManualVictory();
        void OnManualDefeat();
        void BeginDraw(CustomSpriteBatch g);
        void Draw(CustomSpriteBatch g);
    }
}