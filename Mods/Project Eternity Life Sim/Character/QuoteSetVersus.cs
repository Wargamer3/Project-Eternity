using System.IO;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class QuoteSetVersus
    {
        public List<string> ListQuote;
        public List<string> ListPortraitPath;

        public QuoteSetVersus()
        {
            ListQuote = new List<string>();
            ListPortraitPath = new List<string>();
        }

        public QuoteSetVersus(BinaryReader BR)
        {
            int ListQuoteCount = BR.ReadByte();
            ListQuote = new List<string>(ListQuoteCount);
            ListPortraitPath = new List<string>(ListQuoteCount);

            for (int I = 0; I < ListQuoteCount; I++)
            {
                ListQuote.Add(BR.ReadString());
                ListPortraitPath.Add(BR.ReadString());
            }
        }

        internal void Write(BinaryWriter BW)
        {
            BW.Write((byte)ListQuote.Count);
            for (int I = 0; I < ListQuote.Count; I++)
            {
                BW.Write(ListQuote[I]);
                BW.Write(ListPortraitPath[I]);
            }
        }
    }
}
