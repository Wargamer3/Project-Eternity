using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionLogHolder
    {
        public struct ActionLog
        {
            public static DynamicText TextParser;
            public DynamicText DescriptionText;

            public DateTime TimeLogged;
            public string TextLogged;

            public ActionLog(string TextLogged)
            {
                TimeLogged = DateTime.Now;
                this.TextLogged = TextLogged;
                DescriptionText = null;
                if (TextParser != null)
                {
                    DescriptionText = TextParser.Copy();
                    DescriptionText.ParseText(TextLogged);
                }
            }
        }

        public List<ActionLog> ListLog;

        public ActionLogHolder()
        {
            ListLog = new List<ActionLog>();
        }

        public void AddLog(string NewLog)
        {
            ListLog.Add(new ActionLog(NewLog));
        }

        public static void InitTextParser(ContentManager Content, SpriteFont fntMenuText, Color TextColor)
        {
            ActionLog.TextParser = new DynamicText();
            ActionLog.TextParser.TextMaxWidthInPixel = 300;
            ActionLog.TextParser.LineHeight = fntMenuText.LineSpacing;
            ActionLog.TextParser.TextColor = TextColor;
            ActionLog.TextParser.ListProcessor.Add(new RegularTextProcessor(ActionLog.TextParser, fntMenuText));
            IconProcessor IconParser = new IconProcessor(ActionLog.TextParser);
            ActionLog.TextParser.ListProcessor.Add(IconParser);
            ActionLog.TextParser.ListProcessor.Add(new DiceRollProcessor(ActionLog.TextParser, fntMenuText));
            ActionLog.TextParser.ListProcessor.Add(new DiceResultProcessor(ActionLog.TextParser, fntMenuText));
            ActionLog.TextParser.ListProcessor.Add(new DefaultTextProcessor(ActionLog.TextParser, fntMenuText));
            ActionLog.TextParser.SetDefaultProcessor(new DefaultTextProcessor(ActionLog.TextParser, fntMenuText));
            ActionLog.TextParser.Load(Content);

            //IconParser.PreloadImage("rarityE", Symbols.sprRarityE);
        }
    }
}
