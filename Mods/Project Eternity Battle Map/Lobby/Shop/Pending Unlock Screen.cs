using System;
using FMOD;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class PendingUnlockScreen
    {
        public enum UnlockTypes { Character, Units, Consumable }
        public enum UnlockConditions { TimePlayed, GamePlayed, CampaignProgression, UnitKilled }

        public DateTimeOffset UnlockTime;
        public string Message;
        public FMODSound UnlockSound;
        public UnlockTypes UnlockType;

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(CustomSpriteBatch g)
        {

        }
    }
}
