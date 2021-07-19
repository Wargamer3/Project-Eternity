using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class QuoteSetTimeline : FixedTimeline
    {
        public class QuoteSetKeyFrame : AnimationObjectKeyFrame
        {
            private Quote _QuoteSet;

            internal QuoteSetKeyFrame()
            {
                _QuoteSet = new Quote();
            }

            public QuoteSetKeyFrame(AnimationObjectKeyFrame Copy)
            {
                _QuoteSet = new Quote();
            }

            public QuoteSetKeyFrame(BinaryReader BR)
            {
                _QuoteSet = new Quote();
                _QuoteSet.Target = (Quote.Targets)BR.ReadInt32();

                int ListQuoteSetCount = BR.ReadInt32();
                _QuoteSet.ListQuoteSet = new List<QuoteSet>(ListQuoteSetCount);

                _QuoteSet.PortraitPath = BR.ReadString();

                for (int Q = 0; Q < ListQuoteSetCount; Q++)
                {
                    QuoteSet NewQuoteSet = new QuoteSet();
                    NewQuoteSet.QuoteStyle = (QuoteSet.QuoteStyles)BR.ReadInt32();
                    NewQuoteSet.QuoteSetName = BR.ReadString();
                    NewQuoteSet.QuoteSetUseLast = BR.ReadBoolean();
                    NewQuoteSet.QuoteSetChoice = (QuoteSet.QuoteSetChoices)BR.ReadInt32();
                    NewQuoteSet.QuoteSetChoiceValue = BR.ReadInt32();
                    NewQuoteSet.CustomText = BR.ReadString();
                    _QuoteSet.ListQuoteSet.Add(NewQuoteSet);
                }
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write((int)_QuoteSet.Target);
                BW.Write(_QuoteSet.ListQuoteSet.Count);
                BW.Write(_QuoteSet.PortraitPath);
                for (int Q = 0; Q < _QuoteSet.ListQuoteSet.Count; Q++)
                {
                    BW.Write((int)_QuoteSet.ListQuoteSet[Q].QuoteStyle);
                    BW.Write(_QuoteSet.ListQuoteSet[Q].QuoteSetName);
                    BW.Write(_QuoteSet.ListQuoteSet[Q].QuoteSetUseLast);
                    BW.Write((int)_QuoteSet.ListQuoteSet[Q].QuoteSetChoice);
                    BW.Write(_QuoteSet.ListQuoteSet[Q].QuoteSetChoiceValue);
                    BW.Write(_QuoteSet.ListQuoteSet[Q].CustomText);
                }
            }

            public override AnimationObjectKeyFrame Copy(AnimationClass.AnimationLayer ActiveLayer)
            {
                QuoteSetKeyFrame NewQuoteSetKeyFrame = new QuoteSetKeyFrame(this);
                return NewQuoteSetKeyFrame;
            }

            #region Properties

            [Editor(typeof(QuoteSetSelector), typeof(UITypeEditor)),
            CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute(".")]
            public Quote QuoteSet
            {
                get
                {
                    return _QuoteSet;
                }
                set
                {
                    _QuoteSet = value;
                }
            }

            #endregion
        }

        public QuoteSetTimeline()
            : base("Quote", "Quotes")
        {
        }

        public QuoteSetTimeline(BinaryReader BR)
            : this()
        {
            int DicAnimationSpriteKeyFrameCount = BR.ReadInt32();
            DicAnimationKeyFrame = new Dictionary<int, AnimationObjectKeyFrame>();
            for (int K = 0; K < DicAnimationSpriteKeyFrameCount; K++)
            {
                int Key = BR.ReadInt32();
                DicAnimationKeyFrame.Add(Key, new QuoteSetKeyFrame(BR));
            }
        }

        protected override FixedTimeline DoLoadCopy(BinaryReader BR)
        {
            return new QuoteSetTimeline(BR);
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            QuoteSetTimeline NewSetQuoteSetEvent = new QuoteSetTimeline();

            NewSetQuoteSetEvent.UpdateFrom(this, ActiveLayer);

            return NewSetQuoteSetEvent;
        }

        public override void SpawnItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame)
        {
            Quote ActiveQuoteSetFrame = ((QuoteSetKeyFrame)DicAnimationKeyFrame[KeyFrame]).QuoteSet;
            ActiveAnimation.ActiveCharacterName = ActiveQuoteSetFrame.ActiveCharacterName;
            ActiveAnimation.ActiveQuoteSet = ActiveQuoteSetFrame.ActiveText;
            ActiveAnimation.ActiveCharacterSprite = ActiveQuoteSetFrame.ActiveCharacterSprite;
        }

        protected override AnimationObjectKeyFrame CreateFirstKeyFrame()
        {
            return new QuoteSetKeyFrame();
        }
    }
}
