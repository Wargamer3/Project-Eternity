using System.Collections.Generic;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class Quote
    {
        public enum Targets { Attacker, Defender };

        public Targets Target;
        public int SelectedQuoteSet;
        public List<QuoteSet> ListQuoteSet;
        public string ActiveCharacterName;
        public List<string> ActiveText;
        public SimpleAnimation ActiveCharacterSprite;
        public string PortraitPath;

        public Quote()
        {
            Target = Targets.Attacker;
            SelectedQuoteSet = 0;
            PortraitPath = "";
            ActiveText = new List<string>();

            ListQuoteSet = new List<QuoteSet>();
            ListQuoteSet.Add(new QuoteSet());
        }

        public QuoteSet ActiveQuoteSet
        {
            get
            {
                return ListQuoteSet[SelectedQuoteSet];
            }
        }
    }
}
