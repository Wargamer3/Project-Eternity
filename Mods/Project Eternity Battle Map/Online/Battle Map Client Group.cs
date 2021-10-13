using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public class BattleMapClientGroup : GameClientGroup
    {
        public static readonly BattleMapClientGroup Template = new BattleMapClientGroup();
        public BattleMap _CurrentGame;
        public IRoomInformations _Room;

        public IOnlineGame CurrentGame => _CurrentGame;

        public IRoomInformations Room => _Room;

        public BattleMap BattleMapGame { get { return _CurrentGame; } }

        private BattleMapClientGroup()
        {
        }

        private BattleMapClientGroup(IRoomInformations Room)
        {
            this._Room = Room;
        }

        public GameClientGroup CreateFromTemplate(IRoomInformations Room)
        {
            return new BattleMapClientGroup(Room);
        }

        public bool IsRunningSlow()
        {
            return false;
        }

        public void SetGame(BattleMap CurrentGame)
        {
            _CurrentGame = CurrentGame;
        }
    }
}
