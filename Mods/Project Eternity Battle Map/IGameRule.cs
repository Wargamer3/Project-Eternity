using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public struct GameRuleError
    {
        public string Description;
        public object ErrorTarget;

        public GameRuleError(string Description, object ErrorTarget)
        {
            this.Description = Description;
            this.ErrorTarget = ErrorTarget;
        }
    }

    public interface IGameRule
    {
        string Name { get; }
        void Init();
        void Update(GameTime gameTime);
        int GetRemainingResapwn(int PlayerIndex);
        void OnTurnEnd(int ActivePlayerIndex);
        void OnNewTurn(int ActivePlayerIndex);
        void OnSquadDefeated(int AttackerSquadPlayerIndex, Squad AttackerSquad, int DefeatedSquadPlayerIndex, Squad DefeatedSquad);
        void OnManualVictory(int EXP, uint Money);
        void OnManualDefeat(int EXP, uint Money);
        void BeginDraw(CustomSpriteBatch g);
        void Draw(CustomSpriteBatch g);
        List<GameRuleError> Validate(RoomInformations Room);
    }
}