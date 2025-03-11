using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen.Server
{
    public class ChangeBookScriptServer : OnlineScript
    {
        public const string ScriptName = "Change Book";

        private readonly string ID;
        private readonly string CharacterModelPath;
        private readonly string BookName;
        private readonly string BookModel;
        private readonly List<Tuple<string, byte>> ListCard;

        public ChangeBookScriptServer(string ID, string CharacterModelPath, string BookName, string BookModel, List<Tuple<string, byte>> ListCard)
            : base(ScriptName)
        {
            this.ID = ID;
            this.CharacterModelPath = CharacterModelPath;
            this.BookName = BookName;
            this.BookModel = BookModel;
            this.ListCard = ListCard;
        }

        public override OnlineScript Copy()
        {
            throw new NotImplementedException();
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(ID);

            WriteBuffer.AppendString(CharacterModelPath);

            WriteBuffer.AppendString(BookName);
            WriteBuffer.AppendString(BookModel);

            WriteBuffer.AppendInt32(ListCard.Count);
            foreach (Tuple<string, byte> ActiveCard in ListCard)
            {
                WriteBuffer.AppendString(ActiveCard.Item1);
                WriteBuffer.AppendByte(ActiveCard.Item2);
            }
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            throw new NotImplementedException();
        }

        protected override void Read(OnlineReader Sender)
        {
            throw new NotImplementedException();
        }
    }
}
