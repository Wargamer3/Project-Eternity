using System;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen.Online
{
    public class AskChangBookScriptClient : OnlineScript
    {
        private string ID;
        private readonly string CharacterModelPath;
        private readonly CardBook ActiveBook;

        public AskChangBookScriptClient(string ID, string CharacterModelPath, CardBook ActiveBook)
            : base("Ask Change Book")
        {
            this.ID = ID;
            this.CharacterModelPath = CharacterModelPath;
            this.ActiveBook = ActiveBook;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(ID);

            WriteBuffer.AppendString(CharacterModelPath);
            WriteBuffer.AppendString(ActiveBook.BookName);
            WriteBuffer.AppendString(ActiveBook.BookModel);

            WriteBuffer.AppendInt32(ActiveBook.ListCard.Count);
            foreach (CardInfo ActiveCard in ActiveBook.ListCard)
            {
                WriteBuffer.AppendString(ActiveCard.Card.CardType + "/" + ActiveCard.Card.Path);
                WriteBuffer.AppendInt32(ActiveCard.QuantityOwned);
            }
        }

        protected override void Execute(IOnlineConnection Host)
        {
            throw new NotImplementedException();
        }

        protected override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
