using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen.Server
{
    public class AskChangeBookScriptServer : OnlineScript
    {
        public const string ScriptName = "Ask Change Book";

        private readonly RoomInformations Owner;

        private string ID;
        private string CharacterModelPath;
        private string BookName;
        private string BookModel;
        private List<Tuple<string, int>> ListCard;

        public AskChangeBookScriptServer(RoomInformations Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
            ListCard = new List<Tuple<string, int>>(50);
        }

        public override OnlineScript Copy()
        {
            return new AskChangeBookScriptServer(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            foreach (Player ActivePlayer in Owner.ListRoomPlayer)
            {
                if (ActivePlayer.ConnectionID == ID)
                {
                    CardBook NewActiveBook = new CardBook(BookName);
                    NewActiveBook.BookModel = BookModel;
                    ActivePlayer.Inventory.ActiveBook = NewActiveBook;
                    for (int C = 0; C < ListCard.Count; ++C)
                    {
                        Card LoadedCard = Card.LoadCard(ListCard[C].Item1);
                        LoadedCard.QuantityOwned = ListCard[C].Item2;
                        NewActiveBook.AddCard(LoadedCard);
                    }
                }
            }

            foreach (IOnlineConnection ActiveOnlinePlayer in Owner.ListUniqueOnlineConnection)
            {
                if (ActiveOnlinePlayer != Sender)
                {
                    ActiveOnlinePlayer.Send(new ChangeBookScriptServer(ID, CharacterModelPath, BookName, BookModel, ListCard));
                }
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            ID = Sender.ReadString();

            CharacterModelPath = Sender.ReadString();

            BookName = Sender.ReadString();
            BookModel = Sender.ReadString();

            int ListCardCount = Sender.ReadInt32();
            for (int C = 0; C < ListCardCount; ++C)
            {
                ListCard.Add(new Tuple<string, int>(Sender.ReadString(), Sender.ReadInt32()));
            }
        }
    }
}
