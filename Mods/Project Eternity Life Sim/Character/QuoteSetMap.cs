using System.IO;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class QuoteSetMap
    {
        public List<QuoteSetVersus> ListQuoteVersus;

        public QuoteSetMap()
        {
            ListQuoteVersus = new List<QuoteSetVersus>();
            ListQuoteVersus.Add(new QuoteSetVersus());//Any Map
        }

        public QuoteSetMap(BinaryReader BR)
        {
            int ListQuoteCount = BR.ReadByte();
            ListQuoteVersus = new List<QuoteSetVersus>(ListQuoteCount);

            for (int I = 0; I < ListQuoteCount; I++)
            {
                ListQuoteVersus.Add(new QuoteSetVersus(BR));
            }

            if (ListQuoteCount == 0)
            {
                ListQuoteVersus.Add(new QuoteSetVersus());//Any Map
            }
        }

        internal void Write(BinaryWriter BW)
        {
            BW.Write((byte)ListQuoteVersus.Count);
            for (int I = 0; I < ListQuoteVersus.Count; I++)
            {
                ListQuoteVersus[I].Write(BW);
            }
        }
    }
}
