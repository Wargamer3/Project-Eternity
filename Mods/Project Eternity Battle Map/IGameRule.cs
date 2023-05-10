using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public interface IGameRule
    {
        string Name { get; }
        void Init();
        void Update(GameTime gameTime);
        void OnNewTurn(int ActivePlayerIndex);
        void OnSquadDefeated(int AttackerSquadPlayerIndex, Squad AttackerSquad, int DefeatedSquadPlayerIndex, Squad DefeatedSquad);
        void OnManualVictory(int EXP, uint Money);
        void OnManualDefeat(int EXP, uint Money);
        void BeginDraw(CustomSpriteBatch g);
        void Draw(CustomSpriteBatch g);
    }
}