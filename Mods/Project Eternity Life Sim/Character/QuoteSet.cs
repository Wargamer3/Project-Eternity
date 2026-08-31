using System.IO;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class QuoteSet
    {
        public List<QuoteSetMap> ListMapQuote;

        public QuoteSet()
        {
            ListMapQuote = new List<QuoteSetMap>();
            ListMapQuote.Add(new QuoteSetMap());//Any Map
        }

        public QuoteSet(BinaryReader BR)
        {
            int ListQuoteCount = BR.ReadByte();
            ListMapQuote = new List<QuoteSetMap>(ListQuoteCount);

            for (int I = 0; I < ListQuoteCount; I++)
            {
                ListMapQuote.Add(new QuoteSetMap(BR));
            }

            if (ListQuoteCount == 0)
            {
                ListMapQuote.Add(new QuoteSetMap());//Any Map
            }
        }

        public void Write(BinaryWriter BW)
        {
            BW.Write((byte)ListMapQuote.Count);
            for (int I = 0; I < ListMapQuote.Count; I++)
            {
                ListMapQuote[I].Write(BW);
            }
        }
    }
}
