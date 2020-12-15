using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Online
{
    public class TripleThunderClientGroup : ClientGroup
    {
        public static readonly TripleThunderClientGroup Template = new TripleThunderClientGroup();
        public FightingZone _CurrentGame;
        public IRoomInformations _Room;

        public IOnlineGame CurrentGame => _CurrentGame;

        public IRoomInformations Room => _Room;

        public FightingZone TripleThunderGame { get { return _CurrentGame; } }

        private TripleThunderClientGroup()
        {
        }

        private TripleThunderClientGroup(IRoomInformations Room)
        {
            this._Room = Room;
        }

        public ClientGroup CreateFromTemplate(IRoomInformations Room)
        {
            return new TripleThunderClientGroup(Room);
        }

        public bool IsRunningSlow()
        {
            return false;
        }

        public void SetGame(FightingZone CurrentGame)
        {
            _CurrentGame = CurrentGame;
        }
    }
}
