using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class MissionGameRules : GameRules
    {
        #region Ressources

        #endregion

        private MissionRoomInformations Room;


        public MissionGameRules(MissionRoomInformations Room, FightingZone Map)
             : base(Map)
        {
            this.Room = Room;
        }

        public override void Init()
        {
        }

        public override void Load(ContentManager Content)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (KeyboardHelper.KeyPressed(Keys.Escape))
            {
                Map.PushScreen(new PauseMenu());
            }
        }

        public override void OnKill(RobotAnimation KillerPlayer, RobotAnimation KilledPlayer)
        {
            KillerPlayer.Kill++;
            KilledPlayer.Death++;
        }

        public override void Draw(CustomSpriteBatch g, Player PlayerInfo)
        {
        }
    }
}
