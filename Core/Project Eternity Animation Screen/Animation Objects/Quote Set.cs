﻿namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class QuoteSet
    {
        public enum QuoteStyles { MoveIn, Reaction, QuoteSet, Custom };

        public enum QuoteSetChoices { Random, Fixed };

        public QuoteStyles QuoteStyle;
        public string QuoteSetName;
        public bool QuoteSetUseLast;
        public QuoteSetChoices QuoteSetChoice;
        public int QuoteSetChoiceValue;
        public string CustomText;

        public QuoteSet()
        {
            QuoteStyle = QuoteStyles.MoveIn;
            QuoteSetName = "";
            QuoteSetUseLast = false;
            QuoteSetChoice = QuoteSetChoices.Random;
            QuoteSetChoiceValue = 0;
            CustomText = "";
        }
    }
}
