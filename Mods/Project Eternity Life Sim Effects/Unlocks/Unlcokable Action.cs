using System.IO;
using System;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class UnlcokableAction : UnlcokableItemType
    {
        public const string UnlockableTypeName = "Character Action";

        public string ActionPath;

        public UnlcokableAction()
            : base(UnlockableTypeName)
        {
            ActionPath = string.Empty;
        }

        public override void Unlock()
        {
            throw new NotImplementedException();
        }

        public override void DoWrite(BinaryWriter BW)
        {
            throw new NotImplementedException();
        }

        public override UnlcokableItemType Copy()
        {
            return new UnlcokableAction();
        }

        public override UnlcokableItemType LoadCopy(BinaryReader BR)
        {
            throw new NotImplementedException();
        }
    }
}
